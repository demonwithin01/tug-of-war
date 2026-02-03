using UnityEngine;
using UnityEngine.AI;

public class PlayerKing : MonoBehaviour
{
    [SerializeField]
    private PlayerDestinationMarker destinationMarker;

    private NavMeshAgent navMeshAgent;

    private void Awake()
    {
        this.navMeshAgent = this.GetComponent<NavMeshAgent>();
    }

    public void SetDestination( Vector3 position )
    {
        this.navMeshAgent.SetDestination( position );
        this.destinationMarker.SetDestinationMarker( position );
    }
}
