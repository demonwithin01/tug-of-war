using System;
using UnityEngine;
using UnityEngine.AI;

[DefaultExecutionOrder(100)] 
public class PlayerUnitController : UnitController
{
    [SerializeField]
    private PlayerDestinationMarker destinationMarker;

    private Vector3? lastStandingPosition = null;
    private Vector3? userIntendedDestination = null;

    private void Start()
    {
        TeamController playerTeam = TeamsManager.Instance.FindPlayerTeam();
        this.InitialiseWithTeamController( playerTeam );
    }

    protected override void OnUpdate()
    {
        if ( this.userIntendedDestination.HasValue && this.NavMeshAgent.remainingDistance <= this.NavMeshAgent.stoppingDistance )
        {
            this.userIntendedDestination = null;
            base.EnemyManager.FindTarget();
        }
    }

    protected override void TeamInitialised()
    {
        
    }

    protected override void EnemyManager_NewTargetAcquired(object sender, UnitController e)
    {
        // Do not attempt to attack a target if the player has explicitly set a destination for the unit to go to.
        if ( this.userIntendedDestination.HasValue )
        {
            base.EnemyManager.ClearCurrentTarget();
            return;
        }

        if ( this.lastStandingPosition.HasValue == false )
        {
            this.lastStandingPosition = this.transform.position;
        }

        base.AttackTarget( e );
    }

    protected override void EnemyManager_NoTargetsInRange(object sender, EventArgs e)
    {
        Vector3 targetPosition = this.lastStandingPosition ?? this.userIntendedDestination ?? this.transform.position;
        base.RemoveAttackTarget( targetPosition );
    }

    public override void AttackHits(UnitController target)
    {
        int damage = Mathf.RoundToInt( this.baseDamage * base.TeamController.Multipliers.AttackDamage );
        target.TakeDamage( damage );
    }

    public void SetDestination( Vector3 position )
    {
        this.userIntendedDestination = position;
        this.destinationMarker.SetDestinationMarker( position );
        base.RemoveAttackTarget( position );
    }
}
