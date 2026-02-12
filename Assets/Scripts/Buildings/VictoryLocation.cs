using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class VictoryLocation : MonoBehaviour
{
    public event EventHandler OnVictoryReached;

    [SerializeField]
    private int teamNumber;

    private void OnTriggerEnter( Collider other )
    {
        UnitController unitController = other.GetComponent<UnitController>();
        if ( unitController != null && unitController.TeamNumber != teamNumber )
        {
            OnVictoryReached?.Invoke( this, EventArgs.Empty );
            Debug.Log( "Congratulations, you win!" );
        }
    }
}
