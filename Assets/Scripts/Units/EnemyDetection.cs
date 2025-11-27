using System;
using UnityEngine;

public class EnemyDetection : MonoBehaviour
{
    public class EnemyDetectionEventArgs
    {
        public AttackController Unit { get; private set; }
        public UnitHealth UnitHealth { get; private set; }

        public EnemyDetectionEventArgs( AttackController unit, UnitHealth unitHealth )
        {
            this.Unit = unit;
            this.UnitHealth = unitHealth;
        }
    }

    public event EventHandler<EnemyDetectionEventArgs> EnemyDetected;
    public event EventHandler<EnemyDetectionEventArgs> EnemyLeft;

    private int teamNumber;

    private void OnTriggerEnter( Collider other )
    {
        if ( IsAttackableUnit( other, out AttackController attackController, out UnitHealth unitHealth ) )
        {
            EnemyDetected?.Invoke( this, new EnemyDetectionEventArgs( attackController, unitHealth ) );
        }
    }

    private void OnTriggerExit( Collider other )
    {
        if ( IsAttackableUnit( other, out AttackController attackController, out UnitHealth unitHealth ) )
        {
            EnemyLeft?.Invoke( this, new EnemyDetectionEventArgs( attackController, unitHealth ) );
        }
    }

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
