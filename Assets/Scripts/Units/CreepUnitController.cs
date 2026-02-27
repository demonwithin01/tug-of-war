using System;
using UnityEngine;

[DefaultExecutionOrder( 20 )]
public abstract class CreepUnitController : UnitController
{
    public abstract string UnitTypeName { get; }

    protected override void OnAwake()
    {
        base.OnAwake();
    }

    protected override void OnStart()
    {
        // Creeper units should start moving immediately.
        // this.GetComponent<Animator>().Play( "Run" );
        // this.GetComponent<UnitAnimationController>().StartRunning();

        base.EnemyManager.TargetWithinAttackRange += EnemyManager_TargetWithinAttackRange;

        base.OnStart();
    }

    protected override void EnemyManager_NewTargetAcquired(object sender, UnitController e)
    {
        base.MoveToAttack( e );
    }

    protected override void EnemyManager_NoTargetsInRange(object sender, EventArgs e)
    {
        base.RemoveAttackTarget( TeamsManager.Instance.FindOpposingTeamBase( this.TeamNumber ) );
    }

    private void EnemyManager_TargetWithinAttackRange(object sender, UnitController e)
    {
        base.TargetWithinAttackRange( e );
    }

    /// <summary>
    /// Initialises the combat unit instance that maintains the unit's team.
    /// </summary>
    protected override void TeamInitialised()
    {
        GetComponentInChildren<UnitAttackRangeTrigger>().Initialise( this.TeamNumber );

        this.AttackTimer = new TimedAction( this.baseAttackTime / base.TeamController.Multipliers.AttackSpeed, PerformAttack );
        this.AttackTimer.ResetToTrigger();

        this.NavMeshAgent.speed = this.baseSpeed * base.TeamController.Multipliers.MovementSpeed;
    }

    /// <summary>
    /// Handle when the attack lands on the unit.
    /// </summary>
    public override void AttackHits( UnitController target )
    {
        // Get the target to take damage.
        int damage = Mathf.RoundToInt( this.baseDamage * base.TeamController.Multipliers.AttackDamage );
        target.TakeDamage( damage );
    }
}
