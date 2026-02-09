using System;
using System.Collections.Generic;
using UnityEngine;

public class TeamController : MonoBehaviour
{
    [SerializeField]
    private int teamNumber;

    [SerializeField]
    private Transform teamBaseLocation;

    private List<CreepUnitController> teamCreeperUnits = new();

    public int TeamNumber => this.teamNumber;

    public Transform TeamBaseLocation => this.teamBaseLocation;

    /// <summary>
    /// Gets the multipliers that apply to the team.
    /// </summary>
    public CombatMultipliers Multipliers { get; private set; } = new();

    /// <summary>
    /// Initialises the team controller.
    /// </summary>
    private void Start()
    {
        TeamsManager.Instance.RegisterTeam( this );
    }

    /// <summary>
    /// Registers a unit on the team.
    /// </summary>
    public void RegisterUnit( CreepUnitController unit )
    {
        unit.InitialiseWithTeamController( this );
        unit.UnitDied += OnUnitDied;
        this.teamCreeperUnits.Add( unit );
    }

    /// <summary>
    /// Handles when a unit on the team has died.
    /// </summary>
    private void OnUnitDied(object sender, UnitController e)
    {
        CreepUnitController creeperUnit = e as CreepUnitController;

        // If the unit is a creeper unit and is on the team, remove it from the team collection.
        if ( creeperUnit != null && this.teamCreeperUnits.Contains( creeperUnit ) )
        {
            this.teamCreeperUnits.Remove( creeperUnit );
        }
    }
}
