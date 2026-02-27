using System;
using UnityEngine;

[DefaultExecutionOrder( 23 )]
public class UnitAttackRangeTrigger : MonoBehaviour
{
    public class UnitWithinRangeArgs
    {
        public UnitController OpposingTeamUnit { get; private set; }

        public UnitWithinRangeArgs( UnitController unit )
        {
            this.OpposingTeamUnit = unit;
        }
    }

    public event EventHandler<UnitWithinRangeArgs> EnemyEnteredAttackRange;
    public event EventHandler<UnitWithinRangeArgs> EnemyLeftAttackRange;

    private int teamNumber;

    private void OnTriggerEnter( Collider other )
    {
        if ( TryGetUnitController( other, out UnitController opposingTeamUnit ) )
        {
            EnemyEnteredAttackRange?.Invoke( this, new UnitWithinRangeArgs( opposingTeamUnit ) );
        }
    }

    private void OnTriggerExit( Collider other )
    {
        if ( TryGetUnitController( other, out UnitController opposingTeamUnit ) )
        {
            EnemyLeftAttackRange?.Invoke( this, new UnitWithinRangeArgs( opposingTeamUnit ) );
        }
    }

    /// <summary>
    /// Attempts to determine if the collider is attached to an opposing unit.
    /// </summary>
    private bool TryGetUnitController( Collider other, out UnitController unitController )
    {
        // Only check game objects that have the 'Unit' tag.
        if ( other.CompareTag( GameTags.Unit ) )
        {
            // Get the UnitController.
            unitController = other.GetComponent<UnitController>();

            // Return true if we have a unit controller and if the Team Number is different to that on the current unit.
            if ( unitController != null && this.teamNumber != unitController.TeamNumber )
            {
                return true;
            }
        }

        // Assign a default and return false.
        unitController = null;
        return false;
    }

    public void Initialise( int teamNumber )
    {
        this.teamNumber = teamNumber;
    }
}
