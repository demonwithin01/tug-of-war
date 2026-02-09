using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

public class EnemyManager : MonoBehaviour
{
    public event EventHandler<UnitController> NewTargetAcquired;
    public event EventHandler NoTargetsInRange;

    /// <summary>
    /// The list of enemies that are within range of this unit.
    /// </summary>
    private List<UnitController> enemiesWithinRange = new();

    /// <summary>
    /// The list of enemies that are within range of this unit.
    /// </summary>
    /// <remarks>
    /// This list should not be able to be modified outside of this object.
    /// </remarks>
    public IReadOnlyList<UnitController> EnemiesWithinRange => this.enemiesWithinRange;

    public UnitController CurrentTarget { get; private set; }

    private void Awake()
    {
         // Register enemy attraction detection.
        EnemyDetection enemyDetection = GetComponentInChildren<EnemyDetection>();
        enemyDetection.EnemyDetected += this.EnemyDetection_EnemyDetected;
        enemyDetection.EnemyLeft += this.EnemyDetection_EnemyLeft;
    }

    private void OnDestroy()
    {
        this.enemiesWithinRange.Clear();
    }

    /// <summary>
    /// Handler for when something enters the 'attraction' zone.
    /// </summary>
    private void EnemyDetection_EnemyDetected( object sender, EnemyDetection.EnemyDetectionEventArgs e )
    {
         // If the unit is not already in the list of enemies within range, add it to the list and subscribe to its death event.
        if ( this.enemiesWithinRange.Contains( e.OpposingTeamUnit ) == false )
        {
            this.enemiesWithinRange.Add( e.OpposingTeamUnit );

            e.OpposingTeamUnit.UnitDied += OpposingTeamUnit_UnitDied;

            if ( this.CurrentTarget == null )
            {
                this.CurrentTarget = e.OpposingTeamUnit;

                NewTargetAcquired?.Invoke(this, this.CurrentTarget);
            }
        }
    }

    /// <summary>
    /// Handler for when something leaves the 'attraction' zone.
    /// </summary>
    private void EnemyDetection_EnemyLeft( object sender, EnemyDetection.EnemyDetectionEventArgs e )
    {
        this.RemoveTrackedEnemy( e.OpposingTeamUnit );
    }

    /// <summary>
    /// Handler for when a tracked enemy dies. This will remove the unit from the list of tracked enemies and update the current target if necessary.
    /// </summary>
    private void OpposingTeamUnit_UnitDied(object sender, UnitController e)
    {
        this.RemoveTrackedEnemy( e );
    }

    /// <summary>
    /// Finds the greatest threat within range to this unit.
    /// </summary>
    private UnitController FindGreatestThreatWithinRange()
    {
        UnitController closestTarget = null;

        // If the object has been destroyed, do not find a new unit to attack.
        if ( this == null || this.isActiveAndEnabled == false )
        {
            return null;
        }

        // If the object is dead, do not find a new unit to attack.
        // if ( this.cop)

        float? closestDistance = null;

        this.enemiesWithinRange = this.enemiesWithinRange.FindAll( unit => unit != null && unit.isActiveAndEnabled && unit.IsAlive );

        // If there are no enemies within range, return null (no targets).
        if ( this.enemiesWithinRange.Count == 0 )
        {
            return null;
        }

        // Otherwise find the closest target and return that.
        foreach( UnitController unit in this.enemiesWithinRange )
        {
            float distance = this.GetDistanceTo( unit );

            if ( closestDistance == null || distance < closestDistance )
            {
                closestTarget = unit;
                closestDistance = distance;
            }
        }

        return closestTarget;
    }

    /// <summary>
    /// Gets the distance to the specified unit from the current unit.
    /// </summary>
    private float GetDistanceTo( UnitController unit )
    {
        return Vector3.Distance( this.transform.position, unit.transform.position );
    }

    /// <summary>
    /// Removes a tracked enemy from the list of enemies within range and updates the current target if necessary.
    /// </summary>
    private void RemoveTrackedEnemy( UnitController unit )
    {
        if ( this.enemiesWithinRange.Contains( unit ) )
        {
            this.enemiesWithinRange.Remove( unit );
            unit.UnitDied -= OpposingTeamUnit_UnitDied;

            if ( this.CurrentTarget == unit )
            {
                this.CurrentTarget = FindGreatestThreatWithinRange();

                if ( this.CurrentTarget != null )
                {
                    NewTargetAcquired?.Invoke(this, this.CurrentTarget);
                }
                else
                {
                    NoTargetsInRange?.Invoke(this, EventArgs.Empty);
                }
            }
        }
    }
}
