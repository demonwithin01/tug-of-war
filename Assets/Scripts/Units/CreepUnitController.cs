using UnityEngine;

public class CreepUnitController : UnitController
{
    private void Start()
    {
        // Creeper units should start moving immediately.
        this.GetComponent<UnitAnimationController>().StartRunning();
    }

    /// <summary>
    /// Initialises the combat unit instance that maintains the unit's team.
    /// </summary>
    protected override void TeamInitialised()
    {
        this.attackTimer = new TimedAction( this.baseAttackTime / base.TeamController.Multipliers.AttackSpeed, PerformAttack );
        this.attackTimer.ResetToTrigger();

        this.navMeshAgent.speed = this.baseSpeed * base.TeamController.Multipliers.MovementSpeed;
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
