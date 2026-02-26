using System.Collections;
using System.Collections.Generic;
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

    [SerializeField]
    private bool canSpawnUnits = true;

    private float spawnTimer = 0f;

    private int spawnCount = 0;

    private Dictionary<GameObject, Coroutine> spawnedUnitsToCoroutines = new Dictionary<GameObject, Coroutine>();

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

        // Register the new unit with the team controller that this spawner is a child of.
        TeamController teamController = this.GetComponentInParent<TeamController>();

        // Set the team number based on the name.
        if ( teamController.TeamNumber == 1 )
        {
            unit.name = "Blue " + this.spawnCount;
        }
        else
        {
            unit.name = "Red " + this.spawnCount;
        }

        // Apply the colour for the unit, which will be based on the team.
        unit.GetComponentInChildren<UnitVisual>().ApplyUnitColour( this.unitMaterial );

        CreepUnitController unitController = unit.GetComponent<CreepUnitController>();
        unitController.InitialiseWithTeamController( teamController );

        teamController.RegisterUnit( unitController );

        // Set the initial unit position.
        Vector3 opposingBaseLocation = TeamsManager.Instance.FindOpposingTeamBase( teamController.TeamNumber );
        unit.transform.LookAt( opposingBaseLocation );

        NavMeshAgent unitNavMeshAgent = unit.GetComponent<NavMeshAgent>();
        unitNavMeshAgent.Warp( this.spawnLocation.transform.position );

        unitNavMeshAgent.SetDestination( opposingBaseLocation );

        Coroutine coroutine = StartCoroutine(StartRunning( unit ));
        this.spawnedUnitsToCoroutines.Add( unit, coroutine );
    }


    private IEnumerator StartRunning( GameObject unit )
    {
        if ( this.spawnedUnitsToCoroutines.ContainsKey( unit ) == false )
        {
            yield return new WaitForSeconds( 0.1f );
        }

        unit.GetComponent<UnitAnimationController>().StartRunning();

        Coroutine coroutine = this.spawnedUnitsToCoroutines[ unit ];

        StopCoroutine( coroutine );
        this.spawnedUnitsToCoroutines.Remove( unit );

        yield return null;
    }
}
