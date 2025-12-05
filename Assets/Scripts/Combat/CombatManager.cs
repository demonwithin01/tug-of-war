using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CombatManager : MonoBehaviour
{
    public static CombatManager Instance { get; private set; }

    private List<CombatTeam> teams = new();

    private void Awake()
    {
        Instance = this;
    }

    /// <summary>
    /// Registers a newly spawned unit into the Combat Manager.
    /// </summary>
    public void RegisterUnit( UnitController unit )
    {
        CombatTeam team = FindTeamForUnit( unit );

        team.RegisterUnit( unit );
    }

    /// <summary>
    /// Registers a team base location.
    /// </summary>
    /// <param name="teamNumber">The team number to register the location of.</param>
    /// <param name="target">The target location.</param>
    public void RegisterTeamTarget( int teamNumber, Transform target )
    {
        CombatTeam combatTeam = FindTeam( teamNumber );

        combatTeam.RegisterDefaultTarget( target );
    }

    /// <summary>
    /// Handle when a unit has entered the 'attraction' range of another unit.
    /// </summary>
    /// <param name="teamUnit">The unit that was attracted to another.</param>
    /// <param name="opposingTeamUnit">The unit that was the attraction.</param>
    public void UnitEntersRange( UnitController teamUnit, UnitController opposingTeamUnit )
    {
        CombatTeam team = FindTeamForUnit( teamUnit );

        team.UnitEntersRange( teamUnit, opposingTeamUnit );
    }

    /// <summary>
    /// Handle when a unit has left the 'attraction' range of another unit.
    /// </summary>
    /// <param name="teamUnit">The unit that was attracted to another.</param>
    /// <param name="opposingTeamUnit">The unit that was the attraction.</param>
    public void UnitLeavesRange( UnitController teamUnit, UnitController opposingTeamUnit )
    {
        CombatTeam team = FindTeamForUnit( teamUnit );

        team.UnitLeavesRange( teamUnit, opposingTeamUnit );
    }

    /// <summary>
    /// Handle when a unit has died by removing it from the current team and stopping opposing teams from tracking it.
    /// </summary>
    public void UnitDied( UnitController unit )
    {
        // Find the team that the unit died on.
        CombatTeam team = FindTeamForUnit( unit );

        // Remove the unit from their team.
        team.TeamUnitDied( unit );

        // Remove the unit from tracking for opposing teams.
        foreach( CombatTeam opposingTeam in this.teams )
        {
            if ( opposingTeam != team )
            {
                opposingTeam.OpposingTeamUnitDied( unit );
            }
        }
    }

    /// <summary>
    /// Finds an opposing team base location.
    /// </summary>
    /// <param name="currentTeamNumber">The team number of the unit requesting an opposing base location.</param>
    public Vector3 FindOpposingTeamBase( int currentTeamNumber )
    {
        foreach( CombatTeam team in this.teams )
        {
            if ( team.TeamNumber != currentTeamNumber )
            {
                return team.DefaultTarget.position;
            }
        }

        // This should be unreachable...
        Debug.LogError( "Not enough teams exist." );
        return Vector3.zero;
    }

    /// <summary>
    /// Finds the team that a unit belongs to.
    /// </summary>
    private CombatTeam FindTeamForUnit( UnitController unit )
    {
        return FindTeam( unit.TeamNumber );
    }

    /// <summary>
    /// Finds the team.
    /// </summary>
    private CombatTeam FindTeam( int teamNumber )
    {
        // Find the team by checking against the units Team Number.
        CombatTeam team = this.teams.FirstOrDefault( s => s.TeamNumber == teamNumber );

        // If the team doesn't exist, then it hasn't been created yet...
        if ( team == null )
        {
            // In which case, create it and add it to the teams list.
            team = new CombatTeam( teamNumber );

            this.teams.Add( team );
        }

        return team;
    }
}
