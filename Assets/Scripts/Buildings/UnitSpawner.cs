using UnityEngine;

public class UnitSpawner : MonoBehaviour
{
    [SerializeField]
    private Transform spawnLocation;

    [SerializeField]
    private GameObject unitPrefab;

    [SerializeField]
    private Material unitMaterial;

    [SerializeField]
    private Transform target;

    [SerializeField]
    private float spawnTime = 10f;

    [SerializeField]
    private int TeamNumber = 1;

    private float spawnTimer = 0f;

    private int spawnCount = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        this.spawnTimer += Time.deltaTime;

        if ( this.spawnTimer >= this.spawnTime )
        {
            this.spawnTimer = 0f;

            SpawnUnit();
        }
    }

    private void SpawnUnit()
    {
        GameObject unit = Instantiate( this.unitPrefab );

        this.spawnCount++;

        if ( this.TeamNumber == 1 )
        {
            unit.name = "Blue " + this.spawnCount;
        }
        else
        {
            unit.name = "Red " + this.spawnCount;
        }

        unit.GetComponentInChildren<UnitVisual>().ApplyUnitColour( this.unitMaterial );

        AttackController attackController = unit.GetComponent<AttackController>();
        attackController.Initialise( this.TeamNumber, this.spawnLocation.transform.position );
        attackController.SetMainTarget( this.target );

        EnemyDetection enemyDetection = unit.GetComponentInChildren<EnemyDetection>();
        enemyDetection.Initialise( this.TeamNumber );
    }
}
