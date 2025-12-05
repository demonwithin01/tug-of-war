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
    /// Holds the default target that other teams should attack when there are no units within range.
    /// </summary>
    public Transform DefaultTarget { get; private set; }

    /// <summary>
    /// Gets the Team Number.
    /// </summary>
    public int TeamNumber => this.teamNumber;

    public CombatTeam( int teamNumber )
    {
        this.teamNumber = teamNumber;
    }

    public void RegisterDefaultTarget( Transform defaultTarget )
    {
        this.DefaultTarget = defaultTarget;
    }

    /// <summary>
    /// Registers a unit within this team.
    /// </summary>
    public void RegisterUnit( UnitController unit )
    {
        CombatUnit combatUnit = new CombatUnit( unit );

        // Assume we haven't added this unit before.
        this.teamUnits.Add( combatUnit );
    }

    /// <summary>
    /// Registers that the specified opposing unit has entered the range of the given team unit, enabling interactions
    /// such as combat or targeting.
    /// </summary>
    /// <param name="teamUnit">The team unit for which the range entry is being tracked.</param>
    /// <param name="opposingTeamUnit">The opposing unit that has entered the range of the team unit.</param>
    public void UnitEntersRange( UnitController teamUnit, UnitController opposingTeamUnit )
    {
        if ( TryFindUnit( teamUnit, out CombatUnit combatUnit ) )
        {
            combatUnit.AddEnemyWithinRange( opposingTeamUnit );
        }
    }

    /// <summary>
    /// Removes the specified opposing unit from the list of enemies within range of the given team unit.
    /// </summary>
    /// <param name="teamUnit">The team unit that is leaving the range of the opposing unit.</param>
    /// <param name="opposingTeamUnit">The opposing team unit from whose range the team unit is being removed.</param>
    public void UnitLeavesRange( UnitController teamUnit, UnitController opposingTeamUnit )
    {
        if ( TryFindUnit( teamUnit, out CombatUnit combatUnit ) )
        {
            combatUnit.RemoveEnemyWithinRange( opposingTeamUnit );
        }
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
    /// Notifies the team that a unit from the opposing team has died, allowing team units to update their enemy
    /// tracking accordingly.
    /// </summary>
    /// <param name="opposingTeamUnit">The unit controller representing the opposing team unit that has died.</param>
    public void OpposingTeamUnitDied( UnitController opposingTeamUnit )
    {
        foreach( CombatUnit teamUnit in this.teamUnits )
        {
            teamUnit.RemoveEnemyWithinRange( opposingTeamUnit );
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
