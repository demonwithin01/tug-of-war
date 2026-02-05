using System.Collections.Generic;

/// <summary>
/// Aids the Combat Manager in managing units for a team.
/// </summary>
public class CombatUnit
{
    /// <summary>
    /// Gets the multipliers that apply to the team.
    /// </summary>
    public CombatMultipliers Multipliers { get; private set; }

    /// <summary>
    /// The unit that is the focus for tracked enemies, etc.
    /// </summary>
    public CreepUnitController Unit { get; private set; }

    public CombatUnit( CreepUnitController unit, CombatMultipliers multipliers )
    {
        this.Unit = unit;
        this.Multipliers = multipliers;

        unit.InitialiseCombatUnit( this );
    }
}
