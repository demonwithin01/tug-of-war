using System;
using UnityEngine;

[DefaultExecutionOrder(100)] 
[RequireComponent( typeof( AudioSource ) )]
public class PlayerUnitController : UnitController
{
    [SerializeField]
    private PlayerDestinationMarker destinationMarker;

    [SerializeField]
    private AudioClip onMoveAudioClip;

    private AudioSource audioSource;

    private void Start()
    {
        TeamController playerTeam = TeamsManager.Instance.FindPlayerTeam();
        this.InitialiseWithTeamController( playerTeam );

        GameInput.Instance.PlayerMovedRequested += this.GameInput_PlayerMovedRequested;
    }

    protected override void OnAwake()
    {
        this.audioSource = GetComponent<AudioSource>();
    }

    protected override void OnUpdate()
    {
        
    }

    protected override void TeamInitialised()
    {
        
    }

    protected override void EnemyManager_NewTargetAcquired(object sender, UnitController e)
    {
        
    }

    protected override void EnemyManager_NoTargetsInRange(object sender, EventArgs e)
    {
        
    }

    private void GameInput_PlayerMovedRequested(object sender, Vector3 e)
    {
        this.SetDestination( e );

        this.audioSource.PlayOneShot( this.onMoveAudioClip, 0.5f );
    }

    public override void AttackHits(UnitController target)
    {
        int damage = Mathf.RoundToInt( this.baseDamage * base.TeamController.Multipliers.AttackDamage );
        target.TakeDamage( damage );
    }

    public void SetDestination( Vector3 position )
    {
        this.destinationMarker.SetDestinationMarker( position );
        base.RemoveAttackTarget( position );
    }
}
