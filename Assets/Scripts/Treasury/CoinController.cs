using System.Collections;
using UnityEngine;

public class CoinController : MonoBehaviour
{
    private int value;

    private bool isCollected = false;

    private AudioSource audioSource;

    public int Value => value;

    private void Awake()
    {
        this.audioSource = GetComponent<AudioSource>();
    }

    public void SetCoinValue( int value )
    {
        this.value = value;
    }

    public void Collected()
    {
        if ( this.isCollected )
        {
            return;
        }

        this.isCollected = true;

        PlayerTreasury.Instance.CoinCollected( this.value );

        this.audioSource.Play();
        
        GetComponentInChildren<MeshRenderer>().enabled = false; // Hide the coin visually
        StartCoroutine(WaitForAudioEnd());
    }
    private IEnumerator WaitForAudioEnd()
    {
        // Wait until the audio source is no longer playing
        while (audioSource.isPlaying)
        {
            yield return null; // Wait for the next frame
        }

        Destroy( this.gameObject );
    }
}
