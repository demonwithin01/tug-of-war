using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent( typeof( UnitHealth ) )]
public class AttackController : MonoBehaviour
{
    private const string State_IsRunning = "IsRunning";
    private const string State_IsAttacking = "IsAttacking";

    private const string Animation_Attack = "Attack";
    private const string Animation_Death = "Death";

    [SerializeField]
    private float attackRange = 1f;

    [SerializeField]
    private float baseAttackTime = 3f;

    [SerializeField]
    private int baseDamage = 50;

    private UnitHealth unitHealth;

    private bool isMoving = true;
    private bool isAttacking = false;

    private float currentAttackTime = 0f;

    private Animator animator;
    private NavMeshAgent navMeshAgent;

    private AttackController currentAttackTarget;
    private Transform mainAttackTarget;

    private List<AttackController> alternateAttackTargets = new();

    public int TeamNumber { get; private set; }

    private void Awake()
    {
        this.animator = GetComponent<Animator>();
        this.navMeshAgent = GetComponent<NavMeshAgent>();
        this.unitHealth = GetComponent<UnitHealth>();

        EnemyDetection enemyDetection = GetComponentInChildren<EnemyDetection>();
        enemyDetection.EnemyDetected += this.EnemyDetection_EnemyDetected;
        enemyDetection.EnemyLeft += this.EnemyDetection_EnemyLeft;

        this.unitHealth.Died += this.UnitHealth_LostAllHealth;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        this.animator.SetBool( State_IsRunning, isMoving );
        this.animator.SetBool( State_IsAttacking, isAttacking );
    }

    // Update is called once per frame
    private void Update()
    {
        if ( this.unitHealth.IsAlive == false )
        {
            return;
        }

        if ( this.isAttacking )
        {
            if ( Vector3.Distance( this.currentAttackTarget.transform.position, this.transform.position ) > this.attackRange )
            {
                LogTargetMessage( "has moved out of ranging, attempting to find closer target." );
                this.alternateAttackTargets.Add( this.currentAttackTarget );
                this.currentAttackTarget = null;
                ChangeAttackingState( false );
                FindNewTarget();
            }
            else if ( this.currentAttackTarget.unitHealth.IsAlive == false )
            {
                LogTargetMessage( "is dead, attack halted, finding new target" );
                this.currentAttackTarget = null;
                ChangeAttackingState( false );
                FindNewTarget();
            }
            else
            {
                LogTargetMessage( "reached and performing attack" );
                AttemptAttack();
            }
        }
        else if ( this.currentAttackTarget != null )
        {
            if ( this.currentAttackTarget.unitHealth.IsAlive == false )
            {
                LogTargetMessage( "is dead, finding a new target" );
                this.currentAttackTarget = null;
                FindNewTarget();
            }
            else if ( Vector3.Distance( this.currentAttackTarget.transform.position, this.transform.position ) <= this.attackRange )
            {
                LogTargetMessage( "reached" );
                CurrentTargetReached();
            }
            else
            {
                LogTargetMessage( "is too far (" + Vector3.Distance( this.currentAttackTarget.transform.position, this.transform.position ) + ") moving towards it to: " + this.currentAttackTarget.transform.position.ToString() );
                this.navMeshAgent.SetDestination( this.currentAttackTarget.transform.position );
            }
        }

        //this.navMeshAgent.speed = 3.5f + TraitsManager.Instance.SwordsmanMoveSpeedModifier;
    }

    private void OnDestroy()
    {
        
    }

    private void EnemyDetection_EnemyDetected( object sender, EnemyDetection.EnemyDetectionEventArgs e )
    {
        if ( e.Unit.unitHealth.IsAlive == false )
        {
            return;
        }

        if ( this.currentAttackTarget == null )
        {
            AttackNewTarget( e.Unit );
        }
        else
        {
            this.alternateAttackTargets.Add( e.Unit );
        }

        //e.UnitHealth.Died += TrackedEnemyDied;
    }

    private void EnemyDetection_EnemyLeft( object sender, EnemyDetection.EnemyDetectionEventArgs e )
    {
        if ( this.currentAttackTarget == e.Unit )
        {
            this.currentAttackTarget = null;

            FindNewTarget();
        }
        else if ( this.alternateAttackTargets.Contains( e.Unit ) )
        {
            this.alternateAttackTargets.Remove( e.Unit );
        }

        //e.UnitHealth.Died -= TrackedEnemyDied;
    }

    private void UnitHealth_LostAllHealth( object sender, UnitHealth.DiedEventArgs e )
    {
        this.animator.Play( Animation_Death, layer: -1, normalizedTime: 0f );
        Collider[] colliders = this.GetComponents<Collider>();

        for( int i = 0 ; i < colliders.Length ; i++ )
        {
            colliders[ i ].enabled = false;
        }
    }

    private void TrackedEnemyDied( object sender, UnitHealth.DiedEventArgs e )
    {
        if ( this.alternateAttackTargets.Contains( e.Unit ) )
        {
            this.alternateAttackTargets.Remove( e.Unit );
        }

        if ( this.currentAttackTarget == e.Unit )
        {
            this.currentAttackTarget = null;

            FindNewTarget();
        }
    }

    public void DeathAnimationEnd( string s )
    {
        Destroy( gameObject );
    }

    public void Initialise( int teamNumber, Vector3 startLocation )
    {
        this.TeamNumber = teamNumber;
        this.navMeshAgent.Warp( startLocation );
    }

    public void SetMainTarget( Transform target )
    {
        this.mainAttackTarget = target;
        this.navMeshAgent.SetDestination( target.transform.position );
    }

    private void FindNewTarget()
    {
        this.alternateAttackTargets = this.alternateAttackTargets.Where( s => s.unitHealth.IsAlive && s.transform.IsDestroyed() ).ToList();
        if ( this.alternateAttackTargets.Count > 0 )
        {
            float shortest = 999f;
            AttackController closestTarget = null;

            foreach( AttackController alternateTarget in this.alternateAttackTargets )
            {
                float distance = Vector3.Distance( this.transform.position, alternateTarget.transform.position );

                if ( distance < shortest )
                {
                    closestTarget = alternateTarget;
                    shortest = distance;
                }
            }

            if ( closestTarget != null )
            {
                AttackNewTarget( closestTarget );
                return;
            }
        }

        this.ChangeMovingState( true );
        this.animator.SetBool( State_IsAttacking, false );
        this.navMeshAgent.SetDestination( this.mainAttackTarget.transform.position );
    }

    private void ChangeMovingState( bool isMoving )
    {
        this.isMoving = isMoving;
        this.animator.SetBool( State_IsRunning, isMoving );
    }

    private void ChangeAttackingState( bool isAttacking )
    {
        this.isAttacking = isAttacking;
        this.animator.SetBool( State_IsAttacking, isAttacking );
    }

    private void AttackNewTarget( AttackController target )
    {
        ChangeMovingState( true );
        ChangeAttackingState( false );

        this.currentAttackTarget = target;
    }

    private void CurrentTargetReached()
    {
        this.navMeshAgent.SetDestination( this.transform.position );
        this.navMeshAgent.isStopped = true;
        this.transform.LookAt( this.currentAttackTarget.transform );

        this.currentAttackTime = this.baseAttackTime;

        ChangeAttackingState( true );
    }

    private void AttemptAttack()
    {
        this.currentAttackTime += Time.deltaTime;

        if ( this.currentAttackTime >= this.baseAttackTime )
        {
            this.currentAttackTime = 0;
            this.animator.SetBool( State_IsAttacking, true );
            this.animator.Play( Animation_Attack, layer: -1, normalizedTime: 0f );

            int damage = this.baseDamage; // Modifiers here.
            this.currentAttackTarget.unitHealth.TakeDamage( damage );
        }
    }

    private void LogTargetMessage( string messageSuffix )
    {
        if ( this.name == "Blue 4" )
        {
            Debug.Log( this.name + ": Target (" + this.currentAttackTarget.name + ") " + messageSuffix );
        }
    }
}
