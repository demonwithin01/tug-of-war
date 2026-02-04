using UnityEngine;

public class CreepUnitController : UnitController
{

    /// <summary>
    /// The combat unit that manages the team information.
    /// </summary>
    private CombatUnit combatUnit;

    private void Start()
    {
        this.GetComponent<UnitAnimationController>().StartRunning();
    }

    /// <summary>
    /// Initialises the combat unit instance that maintains the unit's team.
    /// </summary>
    public void InitialiseCombatUnit( CombatUnit combatUnit )
    {
        this.combatUnit = combatUnit;

        this.attackTimer = new TimedAction( this.baseAttackTime / this.combatUnit.Multipliers.AttackSpeed, PerformAttack );
        this.attackTimer.ResetToTrigger();

        this.navMeshAgent.speed = this.baseSpeed * this.combatUnit.Multipliers.MovementSpeed;
    }

    /// <summary>
    /// Handle when the attack lands on the unit.
    /// </summary>
    public override void AttackHits( UnitController target )
    {
        // Get the target to take damage.
        int damage = Mathf.RoundToInt( this.baseDamage * this.combatUnit.Multipliers.AttackDamage );
        target.TakeDamage( damage );
    }
}
