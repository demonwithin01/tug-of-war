using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

[RequireComponent( typeof( UnitAnimationController ) )]
public abstract class UnitController : MonoBehaviour
{
    public event EventHandler<UnitController> UnitDied;
    public event EventHandler<UnitController> OnPerformAttack;

    /// <summary>
    /// The point that this unit can be attacked at. This is used for determining where projectiles should aim at, and also for determining whether the unit is within attack range of another unit.
    /// </summary>
    [SerializeField]
    private Transform unitTargetPoint;

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

    // States
    private bool isAttacking = false;

    // Health
    private UnitHealth unitHealth;

    // Timers
    private TimedAction attackTimer;

    // Unity components
    private NavMeshAgent navMeshAgent;

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
    private UnitController movingToAttackTarget = null;
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
    public UnitHealth Health => this.unitHealth;

    /// <summary>
    /// Gets the current enemy that this unit is targeting.
    /// </summary>
    public UnitController UnitAttackTarget => this.unitAttackTarget;

    /// <summary>
    /// Gets the point that this unit can be attacked at. This is used for determining where projectiles should aim at, and also for determining whether the unit is within attack range of another unit.
    /// </summary>
    public Transform UnitTargetPoint => this.unitTargetPoint;

    /// <summary>
    /// Gets the NavMeshAgent component for this unit.
    /// </summary>
    protected NavMeshAgent NavMeshAgent => this.navMeshAgent;

    /// <summary>
    /// Gets the animation controller for this unit.
    /// </summary>
    protected UnitAnimationController AnimationController => this.animationController;

    /// <summary>
    /// Gets the enemy manager for this unit, which manages the units that are within range of this unit.
    /// </summary>
    protected EnemyManager EnemyManager => this.enemyManager;

    /// <summary>
    /// Gets the unit that we are performing the attack against.
    /// </summary>
    protected UnitController PerformingAttackAgainst => this.performingAttackAgainst;

    /// <summary>
    /// Gets the attack timer.
    /// </summary>
    protected TimedAction AttackTimer { get; set; }

    public abstract void AttackHits( UnitController target );

    private void Start()
    {
        this.OnStart();
    }

    private void Awake()
    {
        // Get the components that this controller will rely on.
        this.animationController = GetComponent<UnitAnimationController>();
        this.enemyManager = GetComponent<EnemyManager>();
        this.navMeshAgent = GetComponent<NavMeshAgent>();

        // Create the health management for this unit.
        this.unitHealth = new UnitHealth( this, this.baseHealth );

        this.enemyManager.NewTargetAcquired += EnemyManager_NewTargetAcquired;
        this.enemyManager.NoTargetsInRange += EnemyManager_NoTargetsInRange;

        // Create the attack timer.
        this.attackTimer = new TimedAction( this.baseAttackTime, PerformAttack );
        this.attackTimer.ResetToTrigger();

        this.OnAwake();
    }

    private void Update()
    {
        // If the unit is not alive, do nothing.
        if ( this.IsAlive == false )
        {
            return;
        }

        if ( this.animationController.IsMoving && this.navMeshAgent.remainingDistance <= this.navMeshAgent.stoppingDistance )
        {
            this.animationController.StopRunning();
        }

        this.OnUpdate();

        if ( this.unitAttackTarget != null )
        {
            HandleAttackTarget();
        }

        // Trigger the attack timer to update. Only let it raise the event if the unit is currently in attack mode.
        // This will allow the attack cooldown to expire when the unit is moving.
        this.attackTimer.Tick( isAttacking );
    }

    protected virtual void OnStart()
    {
        
    }

    protected virtual void OnAwake()
    {
        
    }

    protected virtual void OnUpdate()
    {
        
    }

    protected abstract void EnemyManager_NewTargetAcquired(object sender, UnitController e);

    protected abstract void EnemyManager_NoTargetsInRange(object sender, EventArgs e);

    /// <summary>
    /// Handles when there is an attack target.
    /// </summary>
    private void HandleAttackTarget()
    {
        // If the target is within attacking range...
        if ( this.unitAttackTarget != null )
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
        else if ( this.movingToAttackTarget != null)
        {
            // Otherwise tell the unit to move towards the attack target.
            this.navMeshAgent.SetDestination( this.movingToAttackTarget.transform.position );
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

        this.OnPerformAttack?.Invoke( this, this.performingAttackAgainst );
    }

    protected void MoveToAttack( UnitController target )
    {
        if ( this.unitAttackTarget == target )
        {
            return;
        }

        this.ClearAttackUnitTarget();

        this.movingToAttackTarget = target;

        MoveToTarget( target.transform.position );
        ApplyMoveAnimation();
    }

    protected void TargetWithinAttackRange( UnitController target )
    {
        if ( this.unitAttackTarget == null )
        {
            this.unitAttackTarget = target;
            this.movingToAttackTarget = target;
        }
    }

    protected void RemoveAttackTarget( Vector3 moveToPosition )
    {
        // Remove the attack target.
        this.ClearAttackUnitTarget();
        this.movingToAttackTarget = null;

        // Tell the unit to go towards the new target.
        MoveToTarget( moveToPosition );
        
        // Ensure that the unit is in the moving animation.
        ApplyMoveAnimation();
    }

    /// <summary>
    /// Raised when the team controller has been assigned to this unit, allowing the unit to perform any initialisation that relies on the team controller being assigned.
    /// </summary>
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
    /// Removes the current attack target (not the target that we might be moving towards).
    /// </summary>
    private void ClearAttackUnitTarget()
    {
        this.unitAttackTarget = null;
        this.isAttacking = false;
    }

    /// <summary>
    /// Sets the destination for the unit.
    /// </summary>
    private void MoveToTarget( Vector3 targetPosition )
    {
        // Ensure that the nav mesh agent is running.
        if ( this == null || this.navMeshAgent.enabled == false ||this.isActiveAndEnabled == false )
        {
            return;
        }

        try
        {
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

            this.GetComponent<Rigidbody>().useGravity = false;

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
            PlayerTreasury.Instance.SpawnCoin( new Vector3( this.transform.position.x, 0f, this.transform.position.z ), this.baseGold );
        }

        Destroy( gameObject );
    }

    /// <summary>
    /// Removes the 'performingAttackAgainst' value, which is used for determining whether the unit should apply damage when the attack animation finishes. This is used in case the unit is no longer attacking the same target when the attack animation finishes.
    /// </summary>
    protected void ClearPerformingAttackAgainst()
    {
        this.performingAttackAgainst = null;
    }
}
