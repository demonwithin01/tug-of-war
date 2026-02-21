using UnityEngine;

public class ArcherUnitController : CreepUnitController
{
    [SerializeField]
    private Transform arrowSpawnPoint;

    [SerializeField]
    private GameObject arrowPrefab;

    protected override void OnAwake()
    {
        ArcherUnitAnimationController animationController = GetComponentInParent<ArcherUnitAnimationController>();

        animationController.AttackTriggered += ( sender, args ) => SpawnArrow();

        base.OnAwake();
    }

    private void SpawnArrow()
    {
        if ( base.UnitAttackTarget != null )
        {
            GameObject arrow = GameObject.Instantiate( this.arrowPrefab, this.arrowSpawnPoint.position, Quaternion.identity );
        
            arrow.transform.LookAt( base.UnitAttackTarget.transform );
            // arrow.GetComponent<ArrowProjectile>().Initialize( base.UnitAttackTarget.transform );
        }
        else
        {
            Debug.LogError( "Attempted to spawn an arrow projectile without a valid attack target." );
        }
    }
}
