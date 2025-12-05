using UnityEngine;
using UnityEngine.AI;

public class UnitController : MonoBehaviour
{
    /// <summary>
    /// Animation state names.
    /// </summary>
    private static class AnimationState
    {
        public const string IsRunning = "IsRunning";
        public const string IsAttacking = "IsAttacking";
    }

    /// <summary>
    /// Animation names.
    /// </summary>
    private static class AnimationName
    {
        public const string Attack = "Attack";
        public const string Death = "Death";
    }

    [SerializeField]
    private float attackRange = 1f;

    [SerializeField]
    private float baseAttackTime = 3f;

    [SerializeField]
    private int baseDamage = 50;

    [SerializeField]
    private int baseHealth = 100;

    // States
    private bool isMoving = true;
    private bool isAttacking = false;

    // Heath
    private UnitHealth2 unitHealth;

    // Timers
    private TimedAction attackTimer;

    // Unity components
    private Animator animator;
    private NavMeshAgent navMeshAgent;

    /// <summary>
    /// The current unit that is being targeted.
    /// </summary>
    private UnitController unitAttackTarget;
    /// <summary>
    /// The current unit that this unit is currently performing an attack against.
    /// </summary>
    /// <remarks>
    /// This is primarily used in case the animation frame ends when on another target.
    /// </remarks>
    private UnitController performingAttackAgainst;

    /// <summary>
    /// Gets the team number that this unit is assigned to.
    /// </summary>
    public int TeamNumber { get; private set; }

    /// <summary>
    /// Gets whether this unit is still alive.
    /// </summary>
    public bool IsAlive => this.unitHealth.IsAlive;

    /// <summary>
    /// Gets the current health.
    /// </summary>
    public int Health => this.unitHealth.CurrentHealth;

    /// <summary>
    /// Gets the current enemy that this unit is targeting.
    /// </summary>
    public UnitController UnitAttackTarget => this.unitAttackTarget;


    private void Awake()
    {
        // Get the components that this controller will rely on.
        this.animator = GetComponent<Animator>();
        this.navMeshAgent = GetComponent<NavMeshAgent>();

        // Create the health management for this unit.
        this.unitHealth = new UnitHealth2( this.baseHealth );

        // Register enemy attraction detection.
        EnemyDetection enemyDetection = GetComponentInChildren<EnemyDetection>();
        enemyDetection.EnemyDetected += this.EnemyDetection_EnemyDetected;
        enemyDetection.EnemyLeft += this.EnemyDetection_EnemyLeft;

        // Create the attack timer.
        this.attackTimer = new TimedAction( this.baseAttackTime, PerformAttack );
        this.attackTimer.ResetToTrigger();
    }

    private void Start()
    {
        // Set the initial animation states.
        this.animator.SetBool( AnimationState.IsRunning, isMoving );
        this.animator.SetBool( AnimationState.IsAttacking, isAttacking );
    }

    private void Update()
    {
        // If the unit is not alive, do nothing.
        if ( this.IsAlive == false )
        {
            return;
        }

        if ( this.unitAttackTarget != null )
        {
            HandleAttackTarget();
        }

        // Trigger the attack timer to update. Only let it raise the event if the unit is currently in attack mode.
        // This will allow the attack cooldown to expire when the unit is moving.
        this.attackTimer.Tick( isAttacking );
    }

    /// <summary>
    /// Handler for when something enters the 'attraction' zone.
    /// </summary>
    private void EnemyDetection_EnemyDetected( object sender, EnemyDetection.EnemyDetectionEventArgs e )
    {
        // Update the combat manager.
        CombatManager.Instance.UnitEntersRange( this, e.OpposingTeamUnit );
    }

    /// <summary>
    /// Handler for when something leaves the 'attraction' zone.
    /// </summary>
    private void EnemyDetection_EnemyLeft( object sender, EnemyDetection.EnemyDetectionEventArgs e )
    {
        // Update the combat manager.
        CombatManager.Instance.UnitLeavesRange( this, e.OpposingTeamUnit );
    }

    /// <summary>
    /// Handles when there is an attack target.
    /// </summary>
    private void HandleAttackTarget()
    {
        // If the target is within attacking range...
        if ( IsWithinAttackRange() )
        {
            if ( isAttacking == false )
            {
                this.navMeshAgent.SetDestination( this.transform.position );
                this.navMeshAgent.isStopped = true;
                this.transform.LookAt( this.unitAttackTarget.transform );

                isAttacking = true;

                // Make sure the movement animation is not running.
                this.isMoving = false;
                this.animator.SetBool( AnimationState.IsRunning, isMoving );
            }
        }
        else
        {
            // Otherwise tell the unit to move towards the attack target.
            this.navMeshAgent.SetDestination( this.unitAttackTarget.transform.position );
        }
    }

    /// <summary>
    /// Triggers the attack animation.
    /// </summary>
    private void PerformAttack()
    {
        // Set the appropriate animations.
        this.animator.SetBool( AnimationState.IsAttacking, true );
        this.animator.Play( AnimationName.Attack, layer: -1, normalizedTime: 0f );

        this.performingAttackAgainst = this.unitAttackTarget;
    }

    /// <summary>
    /// Checks whether the unit is within attack range of its target.
    /// </summary>
    private bool IsWithinAttackRange()
    {
        float distanceToTarget = GetDistanceTo( this.unitAttackTarget );

        return distanceToTarget <= this.attackRange;
    }

    /// <summary>
    /// Sets the destination for the unit.
    /// </summary>
    private void MoveToTarget( Vector3 targetPosition )
    {
        // Ensure that the nav mesh agent is running.
        this.navMeshAgent.isStopped = false;
        this.navMeshAgent.SetDestination( targetPosition );
    }

    /// <summary>
    /// Sets the move animation.
    /// </summary>
    private void ApplyMoveAnimation()
    {
        this.isMoving = true;
        this.isAttacking = false;

        this.animator.SetBool( AnimationState.IsRunning, isMoving );
        this.animator.SetBool( AnimationState.IsAttacking, isAttacking );
    }

    /// <summary>
    /// Initialises the units team number.
    /// </summary>
    public void InitialiseTeamNumber( int teamNumber  )
    {
        this.TeamNumber = teamNumber;
    }

    /// <summary>
    /// Adds damage to the current unit.
    /// </summary>
    /// <param name="damage">The amount of damage recieved.</param>
    public void TakeDamage( int damage )
    {
        // TakeDamage on UnitHealth will return true if the unit ends up with zero or less health.
        if ( this.unitHealth.TakeDamage( damage ) )
        {
            // Disable all colliders so that other units can pass over it.
            Collider[] colliders = this.GetComponents<Collider>();

            for ( int i = 0 ; i < colliders.Length ; i++ )
            {
                colliders[ i ].enabled = false;
            }

            // Play the death animation.
            this.animator.Play( AnimationName.Death, layer: -1, normalizedTime: 0f );

            // Update the Combat Manager so that it can update other units.
            CombatManager.Instance.UnitDied( this );
        }
    }

    /// <summary>
    /// Calculates the distance between the current unit and a target unit.
    /// </summary>
    public float GetDistanceTo( UnitController target )
    {
        return Vector3.Distance( target.transform.position, this.transform.position );
    }

    /// <summary>
    /// Assigns an enemy unit to attack.
    /// </summary>
    public void AssignAttackTargetUnit( UnitController opposingUnit )
    {
        this.unitAttackTarget = opposingUnit;
        MoveToTarget( opposingUnit.transform.position );

        // If they are not within attack range...
        if ( IsWithinAttackRange() == false )
        {
            // Then reset them to moving and not attacking.
            ApplyMoveAnimation();
        }
    }

    /// <summary>
    /// Removes the current attack target and redirect the unit towards another location.
    /// </summary>
    /// <param name="targetPosition">The new destination to send the unit to.</param>
    public void RemoveAttackTargetUnit( Vector3 targetPosition )
    {
        // Remove the attack target.
        this.unitAttackTarget = null;

        // Tell the unit to go towards the new target.
        MoveToTarget( targetPosition );

        // Ensure that the unit is in the moving animation.
        ApplyMoveAnimation();
    }

    /// <summary>
    /// Handle when the death animation completes.
    /// </summary>
    public void DeathAnimationEnd()
    {
        Destroy( gameObject );
    }

    /// <summary>
    /// Handle when the attack lands on the unit.
    /// </summary>
    public void AttackLands()
    {
        // Ensure that we are still attacking the same unit, just in case the unit is no longer the target when the animation ends.
        if ( this.performingAttackAgainst == this.unitAttackTarget )
        {
            // Get the target to take damage.
            int damage = this.baseDamage; // Modifiers here.
            this.performingAttackAgainst.TakeDamage( damage );
        }

        // Remove the perform attack against value.
        this.performingAttackAgainst = null;
    }
}
