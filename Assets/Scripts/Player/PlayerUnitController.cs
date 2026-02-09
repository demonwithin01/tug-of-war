using UnityEngine;

[DefaultExecutionOrder(100)] 
public class PlayerUnitController : UnitController
{

    private void Start()
    {
        TeamController playerTeam = TeamsManager.Instance.FindPlayerTeam();
        this.InitialiseWithTeamController( playerTeam );
    }

    protected override void TeamInitialised()
    {
        
    }

    public override void AttackHits(UnitController target)
    {
        int damage = Mathf.RoundToInt( this.baseDamage * base.TeamController.Multipliers.AttackDamage );
        target.TakeDamage( damage );
    }
}
