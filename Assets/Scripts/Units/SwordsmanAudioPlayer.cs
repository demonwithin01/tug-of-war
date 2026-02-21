using System;
using UnityEngine;

[RequireComponent( typeof( AudioSource ) )]
public class SwordsmanAudioPlayer : MonoBehaviour
{
    [SerializeField]
    private AudioClip attackAudioClip;

    [SerializeField]
    private AudioClip deathAudioClip;

    [SerializeField]
    private float attackVolume = 1f;

    [SerializeField]
    private UnitController unitController;

    private AudioSource audioSource;

    /// <summary>
    /// Initializes the audio source component and prepares the attack audio clip.
    /// </summary>
    private void Awake()
    {
        this.audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        this.unitController.OnPerformAttack += UnitController_OnPerformAttack;
        this.unitController.UnitDied += UnitController_UnitDied;
    }

    private void UnitController_OnPerformAttack(object sender, UnitController e)
    {
        this.audioSource.PlayOneShot( this.attackAudioClip, this.attackVolume );
    }

    private void UnitController_UnitDied(object sender, UnitController e)
    {
        this.audioSource.PlayOneShot( this.deathAudioClip, this.attackVolume );
    }

}
