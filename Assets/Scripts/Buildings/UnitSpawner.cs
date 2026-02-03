using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.AI;

public class UnitSpawner : MonoBehaviour
{
    [SerializeField]
    private Transform spawnLocation;

    [SerializeField]
    private GameObject unitPrefab;

    [SerializeField]
    private Material unitMaterial;

    [SerializeField]
    private float spawnTime = 10f;

    private TeamConfig teamConfig;

    [SerializeField]
    private bool canSpawnUnits = true;

    private float spawnTimer = 0f;

    private int spawnCount = 0;

    private int TeamNumber => this.teamConfig.TeamNumber;

    /// <summary>
    /// Configures the spawner.
    /// </summary>
    private void Awake()
    {
        this.teamConfig = this.GetComponentInParent<TeamConfig>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.spawnTimer = this.spawnTime;
    }

    // Update is called once per frame
    void Update()
    {
        if ( this.canSpawnUnits )
        {
            this.spawnTimer += Time.deltaTime;

            if ( this.spawnTimer >= this.spawnTime )
            {
                this.spawnTimer = 0f;

                SpawnUnit();
            }
        }
        else
        {
            this.spawnTimer = 0f;
        }
    }

    private void SpawnUnit()
    {
        // Instantiate the unit.
        GameObject unit = Instantiate( this.unitPrefab );

        // Record how many units were spawned. We'll use this for generating the name.
        this.spawnCount++;

        // Set the team number based on the name.
        if ( this.TeamNumber == 1 )
        {
            unit.name = "Blue " + this.spawnCount;
        }
        else
        {
            unit.name = "Red " + this.spawnCount;
        }

        // Apply the colour for the unit, which will be based on the team.
        unit.GetComponentInChildren<UnitVisual>().ApplyUnitColour( this.unitMaterial );

        UnitController unitController = unit.GetComponent<UnitController>();
        unitController.InitialiseTeamNumber( this.TeamNumber );

        // Set the initial unit position.
        Vector3 opposingBaseLocation = CombatManager.Instance.FindOpposingTeamBase( this.TeamNumber );
        unit.transform.LookAt( opposingBaseLocation );
        unit.GetComponent<NavMeshAgent>().Warp( this.spawnLocation.transform.position );

        // Register the unit with the combat manager.
        CombatManager.Instance.RegisterUnit( unitController );

        // Make sure we initialise the enemy attraction detection.
        EnemyDetection enemyDetection = unit.GetComponentInChildren<EnemyDetection>();
        enemyDetection.Initialise( this.TeamNumber );
    }
}
