using System;
using UnityEngine;

public class PlayerVictory : MonoBehaviour
{
    private enum PlayState
    {
        NotPlayed,
        Playing,
        Played
    }
    
    [SerializeField]
    private AudioClip victoryAudioClip;
    [SerializeField]
    private AudioClip cheeringAudioClip;
    [SerializeField]
    private VictoryLocation victoryLocation;

    private AudioSource audioSource;

    private PlayState playState = PlayState.NotPlayed;

    private void Awake()
    {
        this.audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        this.victoryLocation.OnVictoryReached += VictoryLocation_OnVictoryReached; 
    }

    private void Update()
    {
        if ( this.playState == PlayState.Playing && !this.audioSource.isPlaying )
        {
            this.playState = PlayState.Played;
            this.audioSource.PlayOneShot( cheeringAudioClip );
        }
    }

    private void VictoryLocation_OnVictoryReached(object sender, EventArgs e)
    {
        if ( this.playState == PlayState.NotPlayed )
        {
            PlayVictoryAudio();
        }
    }

    private void PlayVictoryAudio()
    {
        this.audioSource.PlayOneShot( victoryAudioClip );
        this.playState = PlayState.Playing;
    }
}
