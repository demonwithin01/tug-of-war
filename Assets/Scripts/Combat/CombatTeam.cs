using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Aids the Combat Manager in managing teams.
/// </summary>
public class CombatTeam
{
    /// <summary>
    /// The current team number that this object belongs to.
    /// </summary>
    private readonly int teamNumber;

    /// <summary>
    /// Holds this list of units that belong to this team.
    /// </summary>
    private List<CombatUnit> teamUnits = new List<CombatUnit>();

    /// <summary>
    /// Holds the team base location.
    /// </summary>
    public Transform TeamBaseLocation { get; private set; }

    /// <summary>
    /// Gets the multipliers that apply to the team.
    /// </summary>
    public CombatMultipliers Multipliers { get; private set; } = new();

    /// <summary>
    /// Gets the Team Number.
    /// </summary>
    public int TeamNumber => this.teamNumber;

    public CombatTeam( int teamNumber )
    {
        this.teamNumber = teamNumber;
    }

    /// <summary>
    /// Registers the team base location.
    /// </summary>
    public void RegisterTeamBaseLocation( Transform teamBaseLocation )
    {
        this.TeamBaseLocation = teamBaseLocation;
    }

    /// <summary>
    /// Registers a unit within this team.
    /// </summary>
    public void RegisterUnit( CreepUnitController unit )
    {
        CombatUnit combatUnit = new CombatUnit( unit, this.Multipliers );

        // Assume we haven't added this unit before.
        this.teamUnits.Add( combatUnit );
    }

    /// <summary>
    /// Removes the specified team unit from the collection of active team units after it has died.
    /// </summary>
    /// <param name="teamUnit">The team unit to remove from the active team units.</param>
    public void TeamUnitDied( UnitController teamUnit )
    {
        if ( TryFindUnit( teamUnit, out CombatUnit combatUnit ) )
        {
            this.teamUnits.Remove( combatUnit );
        }
    }

    /// <summary>
    /// Tries to get the combat unit for the team unit.
    /// </summary>
    /// <param name="teamUnit">The unit controller to find the Combat Unit instance of.</param>
    /// <returns>Returns true if the Combat Unit is found.</returns>
    private bool TryFindUnit( UnitController teamUnit, out CombatUnit combatUnit )
    {
        combatUnit = this.teamUnits.FirstOrDefault( s => s.Unit == teamUnit );

        return combatUnit != null;
    }
}
