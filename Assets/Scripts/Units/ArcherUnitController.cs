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
            GameObject arrowGameObject = GameObject.Instantiate( this.arrowPrefab, this.arrowSpawnPoint.position, Quaternion.identity );
            
            ArrowController arrow = arrowGameObject.GetComponent<ArrowController>();

            arrow.LaunchAtTarget( base.UnitAttackTarget, this.arrowSpawnPoint.position );
        }
        else
        {
            Debug.LogError( "Attempted to spawn an arrow projectile without a valid attack target." );
        }
    }
}
