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

    private void OnTriggerEnter(Collider other)
    {
        if ( other.CompareTag( "Coin" ) )
        {
            CoinController coin = other.transform.GetComponent<CoinController>();

            if ( coin != null )
            {
                coin.Collected();
            }
        }
    }

    public void SetDestination( Vector3 position )
    {
        this.navMeshAgent.SetDestination( position );
        this.destinationMarker.SetDestinationMarker( position );
    }
}
