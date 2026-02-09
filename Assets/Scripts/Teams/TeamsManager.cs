using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TeamsManager : MonoBehaviour
{
    private List<TeamController> teams = new();
    public static TeamsManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    /// <summary>
    /// Registers a team within the Teams Manager.
    /// </summary>
    public void RegisterTeam( TeamController teamController )
    {
        this.teams.Add( teamController );
    }

    public TeamController FindPlayerTeam()
    {
        // Find the team by checking against the units Team Number.
        TeamController team = this.teams.FirstOrDefault( s => s.TeamNumber == 1 );

        if ( team == null )
        {
            Debug.LogError( "Player team doesn't exist." );
        }

        return team;
    }

    /// <summary>
    /// Finds an opposing team base location.
    /// </summary>
    /// <param name="currentTeamNumber">The team number of the unit requesting an opposing base location.</param>
    public Vector3 FindOpposingTeamBase( int currentTeamNumber )
    {
        foreach( TeamController team in this.teams )
        {
            if ( team.TeamNumber != currentTeamNumber )
            {
                return team.TeamBaseLocation.position;
            }
        }

        // This should be unreachable...
        Debug.LogError( "Not enough teams exist." );
        return Vector3.zero;
    }

}
