using UnityEngine;

public class ArrowController : MonoBehaviour
{
    private Rigidbody arrowRigidbody;

    private float timeAlive = 0f;

    private float maxTimeAlived = 5f; // Destroy the arrow after 5 seconds to prevent it from existing indefinitely

    private float arrowVelocity = 10f;

    private UnitController targetUnit;
    
    private void Awake()
    {
        this.arrowRigidbody = this.GetComponent<Rigidbody>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    private void Update()
    {
        timeAlive += Time.deltaTime;
        if (timeAlive >= maxTimeAlived)
        {
            Destroy( this.gameObject );
        }
    }

    private void OnTriggerEnter( Collider other )
    {
        if ( other.gameObject.layer == LayerMask.NameToLayer( "Ground" ) )
        {
            Destroy( this.gameObject ); // Destroy the arrow if it hits the ground
        }
        else if ( other.gameObject.TryGetComponent<UnitController>( out UnitController hitUnit ) )
        {
            if ( hitUnit == targetUnit )
            {
                hitUnit.TakeDamage( 10 ); // Apply damage to the unit.
                Destroy( this.gameObject ); // Destroy the arrow after hitting the target
            }
        }
    }

    public void LaunchAtTarget( UnitController target, Vector3 spawnPosition )
    {
        this.targetUnit = target;

        this.transform.LookAt( target.transform ); 
        Rigidbody arrowRigidbody = this.GetComponent<Rigidbody>();
        arrowRigidbody.linearVelocity = ( target.UnitTargetPoint.position - spawnPosition ).normalized * arrowVelocity;
    }
}
