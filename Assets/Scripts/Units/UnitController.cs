using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

[RequireComponent( typeof( UnitAnimationController ) )]
public abstract class UnitController : MonoBehaviour
{
    public event EventHandler<UnitController> UnitDied;

    [SerializeField]
    private float attackRange = 1.2f;

    [SerializeField]
    protected float baseAttackTime = 3f;

    [SerializeField]
    protected int baseDamage = 50;

    [SerializeField]
    private int baseHealth = 100;

    [SerializeField]
    protected float baseSpeed = 3.5f;

    [SerializeField]
    private int baseGold = 1;

    [SerializeField]
    private Transform coinPrefab;

    // States
    private bool isAttacking = false;

    // Health
    private UnitHealth2 unitHealth;

    // Timers
    protected TimedAction attackTimer;

    // Unity components
    protected NavMeshAgent navMeshAgent;

    // Other components
    /// <summary>
    /// The animation controller for this unit.
    /// </summary>
    private UnitAnimationController animationController;
    /// <summary>
    /// The enemy manager for this unit, which manages the units that are within range of this unit.
    /// </summary>
    private EnemyManager enemyManager;
    /// <summary>
    /// The current unit that is being targeted.
    /// </summary>
    private UnitController unitAttackTarget = null;
    /// <summary>
    /// The current unit that this unit is currently performing an attack against.
    /// </summary>
    /// <remarks>
    /// This is primarily used in case the animation frame ends when on another target.
    /// </remarks>
    private UnitController performingAttackAgainst = null;

    /// <summary>
    /// The team controller that this unit belongs to.
    /// </summary>
    public TeamController TeamController { get; private set; }

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

    public abstract void AttackHits( UnitController target );


    private void Awake()
    {
        // Get the components that this controller will rely on.
        this.animationController = GetComponent<UnitAnimationController>();
        this.enemyManager = GetComponent<EnemyManager>();
        this.navMeshAgent = GetComponent<NavMeshAgent>();

        // Create the health management for this unit.
        this.unitHealth = new UnitHealth2( this.baseHealth );

        this.enemyManager.NewTargetAcquired += EnemyManager_NewTargetAcquired;
        this.enemyManager.NoTargetsInRange += EnemyManager_NoTargetsInRange;

        // Create the attack timer.
        this.attackTimer = new TimedAction( this.baseAttackTime, PerformAttack );
        this.attackTimer.ResetToTrigger();
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

    private void EnemyManager_NewTargetAcquired(object sender, UnitController e)
    {
        this.unitAttackTarget = e;
        MoveToTarget( e.transform.position );

        // If they are not within attack range...
        if ( IsWithinAttackRange() == false )
        {
            // Then reset them to moving and not attacking.
            ApplyMoveAnimation();
        }
    }

    private void EnemyManager_NoTargetsInRange(object sender, EventArgs e)
    {
        // Remove the attack target.
        this.unitAttackTarget = null;

        // Tell the unit to go towards the new target.
        MoveToTarget( TeamsManager.Instance.FindOpposingTeamBase( this.TeamNumber ) );

        // Ensure that the unit is in the moving animation.
        ApplyMoveAnimation();
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
                this.animationController.StopRunning();
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
    protected void PerformAttack()
    {
        // Set the appropriate animations.
        this.animationController.PerformAttack();

        this.performingAttackAgainst = this.unitAttackTarget;
    }

    protected abstract void TeamInitialised();

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
        if ( this == null || this.navMeshAgent.enabled == false ||this.isActiveAndEnabled == false )
        {
            return;
        }

        try
        {
            // Ensure that the nav mesh agent is running.
            this.navMeshAgent.isStopped = false;
            this.navMeshAgent.SetDestination( targetPosition );
        }
        catch( Exception ex )
        {
            Debug.LogError( $"Error acquiring new target: {ex}" );
            return;
        }
    }

    /// <summary>
    /// Sets the move animation.
    /// </summary>
    private void ApplyMoveAnimation()
    {
        this.animationController.StartRunning();
    }

    /// <summary>
    /// Initialises the unit controller with the team controller that it belongs to.
    /// </summary>
    public void InitialiseWithTeamController( TeamController teamController )
    {
        this.TeamController = teamController;
        this.TeamNumber = teamController.TeamNumber;

        // Make sure we initialise the enemy attraction detection.
        EnemyDetection enemyDetection = this.GetComponentInChildren<EnemyDetection>();
        enemyDetection.Initialise( this.TeamNumber );

        this.TeamInitialised();
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

            this.navMeshAgent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
            this.navMeshAgent.radius = 0f;
            this.navMeshAgent.isStopped = true;
            this.navMeshAgent.enabled = false;

            this.enemyManager.enabled = false;

            // Play the death animation.
            this.animationController.PlayDeathAnimation();

            // Raise an event so that other units can respond to this unit's death.
            this.UnitDied?.Invoke( this, this );
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
    /// Handle when the death animation completes.
    /// </summary>
    public void DeathAnimationEnd()
    {
        // Only spawn if not the players team.
        if ( this.TeamNumber != 1 )
        {
            Transform coinTransform = Instantiate( this.coinPrefab );
            coinTransform.position = this.transform.position;
            coinTransform.GetComponent<CoinController>().SetCoinValue( this.baseGold );
        }

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
            this.AttackHits( this.performingAttackAgainst );
            // int damage = Mathf.RoundToInt( this.baseDamage * this.combatUnit.Multipliers.AttackDamage );
            // this.performingAttackAgainst.TakeDamage( damage );
        }

        // Remove the perform attack against value.
        this.performingAttackAgainst = null;
    }
}
