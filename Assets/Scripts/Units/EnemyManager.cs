using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

public class EnemyManager : MonoBehaviour
{

    public event EventHandler<UnitController> NewTargetAcquired;
    public event EventHandler<UnitController> TargetWithinAttackRange;
    public event EventHandler NoTargetsInRange;

    /// <summary>
    /// The list of enemies that are within range of this unit for it to start moving towards them. This is the 'attraction' range, which is larger than the attack range. Enemies within this range will be tracked and the unit will attempt to move towards them, but they will not be attacked until they are within the attack range.
    /// </summary>
    private List<UnitController> enemiesWithinPullRange = new();

    /// <summary>
    /// The list of enemies that are within range of this unit for it to start attacking. This is the 'attack' range, which is smaller than the attraction range. Enemies within this range will be attacked by the unit.
    /// </summary>
    private List<UnitController> enemiesWithinAttackRange = new();

    /// <remarks>
    /// This list should not be able to be modified outside of this object.
    /// </remarks>
    public IReadOnlyList<UnitController> EnemiesWithinRange => this.enemiesWithinPullRange;

    public UnitController CurrentTarget { get; private set; }

    private void Awake()
    {
         // Register enemy attraction detection.
        EnemyDetection enemyDetection = GetComponentInChildren<EnemyDetection>();
        enemyDetection.EnemyDetected += this.EnemyDetection_EnemyDetected;
        enemyDetection.EnemyLeft += this.EnemyDetection_EnemyLeft;

        UnitAttackRangeTrigger attackRangeTrigger = GetComponentInChildren<UnitAttackRangeTrigger>();
        attackRangeTrigger.EnemyEnteredAttackRange += this.AttackRangeTrigger_EnemyEnteredAttackRange;
        attackRangeTrigger.EnemyLeftAttackRange += this.AttackRangeTrigger_EnemyLeft;
    }

    private void OnDestroy()
    {
        this.enemiesWithinPullRange.Clear();
    }

    /// <summary>
    /// Handler for when something enters the 'attraction' zone.
    /// </summary>
    private void EnemyDetection_EnemyDetected( object sender, EnemyDetection.EnemyDetectionEventArgs e )
    {
         // If the unit is not already in the list of enemies within range, add it to the list and subscribe to its death event.
        if ( this.enemiesWithinPullRange.Contains( e.OpposingTeamUnit ) == false )
        {
            this.enemiesWithinPullRange.Add( e.OpposingTeamUnit );

            e.OpposingTeamUnit.UnitDied += OpposingTeamUnit_UnitDied;

            if ( this.CurrentTarget == null )
            {
                this.ApplyTarget( e.OpposingTeamUnit );
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

    private void AttackRangeTrigger_EnemyLeft(object sender, UnitAttackRangeTrigger.UnitWithinRangeArgs e)
    {
        if ( this.enemiesWithinAttackRange.Contains( e.OpposingTeamUnit ) )
        {
            this.enemiesWithinAttackRange.Remove( e.OpposingTeamUnit );
        }
    }

    private void AttackRangeTrigger_EnemyEnteredAttackRange(object sender, UnitAttackRangeTrigger.UnitWithinRangeArgs e)
    {
        if ( this.enemiesWithinAttackRange.Contains( e.OpposingTeamUnit ) == false )
        {
            this.enemiesWithinAttackRange.Add( e.OpposingTeamUnit );
        }

        if ( this.CurrentTarget == null || this.CurrentTarget == e.OpposingTeamUnit )
        {
            this.AttackTargetWithinRange( e.OpposingTeamUnit );
        }

        if ( this.CurrentTarget != e.OpposingTeamUnit && this.enemiesWithinAttackRange.Contains( this.CurrentTarget ) == false )
        {
            AttackTargetWithinRange( e.OpposingTeamUnit );
        }
    }

    /// <summary>
    /// Handler for when a tracked enemy dies. This will remove the unit from the list of tracked enemies and update the current target if necessary.
    /// </summary>
    private void OpposingTeamUnit_UnitDied(object sender, UnitController e)
    {
        this.RemoveTrackedEnemy( e );
    }

    private void AttackTargetWithinRange( UnitController target )
    {
        this.CurrentTarget = target;
        this.ApplyTarget( target );
        TargetWithinAttackRange?.Invoke(this, target);
    }

    /// <summary>
    /// Applies the specified unit as the current target and raises the NewTargetAcquired event.
    /// </summary>
    private void ApplyTarget( UnitController unitController )
    {
        this.CurrentTarget = unitController;

        NewTargetAcquired?.Invoke(this, unitController);

        if ( this.enemiesWithinAttackRange.Contains( unitController ) )
        {
            TargetWithinAttackRange?.Invoke(this, unitController);
        }
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

        this.enemiesWithinPullRange = this.enemiesWithinPullRange.FindAll( unit => unit != null && unit.isActiveAndEnabled && unit.IsAlive );

        // If there are no enemies within range, return null (no targets).
        if ( this.enemiesWithinPullRange.Count == 0 )
        {
            return null;
        }

        // Otherwise find the closest target and return that.
        foreach( UnitController unit in this.enemiesWithinPullRange )
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
        if ( this.enemiesWithinPullRange.Contains( unit ) )
        {
            this.enemiesWithinPullRange.Remove( unit );
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

        if ( this.enemiesWithinAttackRange.Contains( unit ) )
        {
            this.enemiesWithinAttackRange.Remove( unit );
        }
    }

    public void FindTarget()
    {
        if ( this.CurrentTarget == null )
        {
            UnitController newTarget = FindGreatestThreatWithinRange();

            if ( newTarget != null )
            {
                this.ApplyTarget( newTarget );
            }
        }
    }

    public void ClearCurrentTarget()
    {
        this.CurrentTarget = null;
    }
}
