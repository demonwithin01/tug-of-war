using UnityEngine;
using System.Collections.Generic;
using System;

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

    /// <summary>
    /// Handler for when something enters the 'attraction' zone.
    /// </summary>
    private void EnemyDetection_EnemyDetected( object sender, EnemyDetection.EnemyDetectionEventArgs e )
    {
        if ( this.enemiesWithinRange.Contains( e.OpposingTeamUnit ) == false )
        {
            this.enemiesWithinRange.Add( e.OpposingTeamUnit );

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
        if ( this.enemiesWithinRange.Contains( e.OpposingTeamUnit ) )
        {
            this.enemiesWithinRange.Remove( e.OpposingTeamUnit );

            if ( this.CurrentTarget == e.OpposingTeamUnit )
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

    /// <summary>
    /// Finds the greatest threat within range to this unit.
    /// </summary>
    private UnitController FindGreatestThreatWithinRange()
    {
        UnitController closestTarget = null;

        // If there are no enemies within range, return null (no targets).
        if ( this.enemiesWithinRange.Count == 0 )
        {
            return null;
        }

        float? closestDistance = null;

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
}
