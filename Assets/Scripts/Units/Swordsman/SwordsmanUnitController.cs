public class SwordsmanUnitController : CreepUnitController
{
    override public string UnitTypeName => "Swordsman";

    protected override void OnAwake()
    {
        UnitAttackAnimationTrigger animationTrigger = GetComponentInParent<UnitAttackAnimationTrigger>();
        animationTrigger.AttackTriggered += ( sender, args ) => this.SlashPointReached();

        base.OnAwake();
    }


    /// <summary>
    /// Handle when the attack lands on the unit.
    /// </summary>
    private void SlashPointReached()
    {
        // Ensure that we are still attacking the same unit, just in case the unit is no longer the target when the animation ends.
        if ( this.PerformingAttackAgainst == this.UnitAttackTarget )
        {
            // Get the target to take damage.
            this.AttackHits( this.PerformingAttackAgainst );
        }

        // Remove the perform attack against value.
        base.ClearPerformingAttackAgainst();
    }
}
