using UnityEngine;

public class ArcherUnitController : CreepUnitController
{
    override public string UnitTypeName => "Archer";
    
    [SerializeField]
    private Transform arrowSpawnPoint;

    [SerializeField]
    private GameObject arrowPrefab;

    protected override void OnAwake()
    {
        UnitAttackAnimationTrigger animationController = GetComponentInParent<UnitAttackAnimationTrigger>();

        animationController.AttackTriggered += ( sender, args ) => this.SpawnArrow();

        base.OnAwake();
    }

    private void SpawnArrow()
    {
        if ( base.UnitAttackTarget != null )
        {
            GameObject arrowGameObject = GameObject.Instantiate( this.arrowPrefab, this.arrowSpawnPoint.position, Quaternion.identity );
            
            ArrowController arrow = arrowGameObject.GetComponent<ArrowController>();

            arrow.LaunchAtTarget( base.UnitAttackTarget, this.arrowSpawnPoint.position );

            base.ClearPerformingAttackAgainst();
        }
        else
        {
            Debug.LogError( "Attempted to spawn an arrow projectile without a valid attack target." );
        }
    }
}
