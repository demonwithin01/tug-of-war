using System.Collections.Generic;

/// <summary>
/// Aids the Combat Manager in managing units for a team.
/// </summary>
public class CombatUnit
{
    /// <summary>
    /// The list of enemies that are within range of this unit.
    /// </summary>
    private List<UnitController> enemiesWithinRange;

    /// <summary>
    /// The unit that is the focus for tracked enemies, etc.
    /// </summary>
    public UnitController Unit { get; private set; }

    /// <summary>
    /// The list of enemies that are within range of this unit.
    /// </summary>
    /// <remarks>
    /// This list should not be able to be modified outside of this object.
    /// </remarks>
    public IReadOnlyList<UnitController> EnemiesWithinRange => this.enemiesWithinRange;

    public CombatUnit( UnitController unit )
    {
        this.Unit = unit;
        this.enemiesWithinRange = new();

        FindNewAttackTarget();
    }

    /// <summary>
    /// Adds a unit that has come into the attaction range.
    /// </summary>
    /// <param name="unit">The unit that has entered the range.</param>
    public void AddEnemyWithinRange( UnitController unit )
    {
        // Only add the unit if it isn't already in the list.
        if ( this.enemiesWithinRange.Contains( unit ) == false )
        {
            this.enemiesWithinRange.Add( unit );

            if ( this.Unit.UnitAttackTarget == null )
            {
                this.Unit.AssignAttackTargetUnit( unit );
            }
        }
    }

    /// <summary>
    /// Adds a unit that has left the attaction range.
    /// </summary>
    /// <param name="unit">The unit that has left the range.</param>
    public void RemoveEnemyWithinRange( UnitController unit )
    {
        // Only attempt to remove the unit if it is in the list.
        if ( this.enemiesWithinRange.Contains( unit ) )
        {
            this.enemiesWithinRange.Remove( unit );

            if ( this.Unit.UnitAttackTarget == unit )
            {
                FindNewAttackTarget();
            }
        }
    }

    /// <summary>
    /// Finds a new target for the unit to attack and sends them to it.
    /// </summary>
    private void FindNewAttackTarget()
    {
        UnitController newAttackTarget = FindClosestTarget();

        // If there is a unit to attack...
        if ( newAttackTarget != null )
        {
            // Then send them to attack that unit.
            this.Unit.AssignAttackTargetUnit( newAttackTarget );
        }
        else
        {
            // Otherwise send them to the main target.
            this.Unit.RemoveAttackTargetUnit( CombatManager.Instance.FindOpposingTeamBase( this.Unit.TeamNumber ) );
        }
    }

    /// <summary>
    /// Finds the target that is closest within range.
    /// </summary>
    private UnitController FindClosestTarget()
    {
        UnitController closestTarget = null;

        if ( this.enemiesWithinRange.Count == 0 )
        {
            return null;
        }

        float? closestDistance = null;

        foreach( UnitController unit in this.enemiesWithinRange )
        {
            float distance = this.Unit.GetDistanceTo( unit );

            if ( closestDistance == null || distance < closestDistance )
            {
                closestTarget = unit;
                closestDistance = distance;
            }
        }

        return closestTarget;
    }
}
