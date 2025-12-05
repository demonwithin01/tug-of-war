using System;
using UnityEngine;

public class EnemyDetection : MonoBehaviour
{
    public class EnemyDetectionEventArgs
    {
        public UnitController OpposingTeamUnit { get; private set; }

        public EnemyDetectionEventArgs( UnitController unit )
        {
            this.OpposingTeamUnit = unit;
        }
    }

    public event EventHandler<EnemyDetectionEventArgs> EnemyDetected;
    public event EventHandler<EnemyDetectionEventArgs> EnemyLeft;

    private int teamNumber;

    /// <summary>
    /// Trigger handler for when something enters the 'attraction' zone.
    /// </summary>
    private void OnTriggerEnter( Collider other )
    {
        // We only care about opposing Units.
        if ( TryGetUnitController( other, out UnitController opposingTeamUnit ) )
        {
            EnemyDetected?.Invoke( this, new EnemyDetectionEventArgs( opposingTeamUnit ) );
        }
    }

    /// <summary>
    /// Trigger handler for when something leaves the 'attraction' zone.
    /// </summary>
    private void OnTriggerExit( Collider other )
    {
        // We only care about opposing Units.
        if ( TryGetUnitController( other, out UnitController opposingTeamUnit ) )
        {
            EnemyLeft?.Invoke( this, new EnemyDetectionEventArgs( opposingTeamUnit ) );
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

    //private void OnTriggerEnter( Collider other )
    //{
    //    if ( IsAttackableUnit( other, out AttackController attackController, out UnitHealth unitHealth ) )
    //    {
    //        EnemyDetected?.Invoke( this, new EnemyDetectionEventArgs( attackController, unitHealth ) );
    //    }
    //}

    //private void OnTriggerExit( Collider other )
    //{
    //    if ( IsAttackableUnit( other, out AttackController attackController, out UnitHealth unitHealth ) )
    //    {
    //        EnemyLeft?.Invoke( this, new EnemyDetectionEventArgs( attackController, unitHealth ) );
    //    }
    //}

    public void Initialise( int teamNumber )
    {
        this.teamNumber = teamNumber;
    }

    private bool IsAttackableUnit( Collider other, out AttackController attackController, out UnitHealth unitHealth )
    {
        if ( other.CompareTag( GameTags.Unit ) )
        {
            attackController = other.GetComponent<AttackController>();

            if ( attackController != null && this.teamNumber != attackController.TeamNumber )
            {
                unitHealth = other.GetComponent<UnitHealth>();
                return true;
            }
        }

        attackController = null;
        unitHealth = null;
        return false;
    }
}
