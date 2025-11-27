using UnityEngine;
using UnityEngine.AI;

public class UnitMovement : MonoBehaviour
{
    private NavMeshAgent navMeshAgent;

    private Vector3 targetLocation;

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {

    }

    private void Update()
    {
        this.navMeshAgent.speed = 3.5f + TraitsManager.Instance.SwordsmanMoveSpeedModifier;
    }

    public void SetStartLocation( Vector3 location )
    {
        this.navMeshAgent.Warp( location );
    }

    public void SetTargetLocation( Vector3 targetLocation )
    {
        this.navMeshAgent.SetDestination( targetLocation );
        this.targetLocation = targetLocation;
    }
}
