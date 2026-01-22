using UnityEngine;

public class VictoryLocation : MonoBehaviour
{
    [SerializeField]
    private int teamNumber;

    private void OnTriggerEnter( Collider other )
    {
        UnitController unitController = other.GetComponent<UnitController>();
        if ( unitController != null && unitController.TeamNumber != teamNumber )
        {
            Debug.Log( "Congratulations, you win!" );
        }
    }
}
