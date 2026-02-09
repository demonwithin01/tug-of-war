using UnityEngine;
using UnityEngine.AI;

public class PlayerKing : MonoBehaviour
{
    [SerializeField]
    private PlayerDestinationMarker destinationMarker;

    // Unity components
    private UnitAnimationController animationController;
    private NavMeshAgent navMeshAgent;

    private void Awake()
    {
        // Get the components that this controller will rely on.
        this.animationController = GetComponent<UnitAnimationController>();
        this.navMeshAgent = this.GetComponent<NavMeshAgent>();

        
    }

    private void Update()
    {
        if ( !this.navMeshAgent.pathPending && this.navMeshAgent.remainingDistance <= this.navMeshAgent.stoppingDistance)
        {
            this.animationController.StopRunning();
        }
    }

    public void SetDestination( Vector3 position )
    {
        this.navMeshAgent.SetDestination( position );
        this.destinationMarker.SetDestinationMarker( position );
        this.animationController.StartRunning();
    }
}
