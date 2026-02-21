using Unity.VisualScripting;
using UnityEngine;

public class BackgroundMusic : MonoBehaviour
{
    [SerializeField]
    private AudioClip backgroundMusicClip;

    [SerializeField]
    private float backgroundMusicVolume = 0.15f;

    private AudioSource audioSource;

    private void Awake()
    {
        this.audioSource = GetComponent<AudioSource>();
        this.audioSource.clip = this.backgroundMusicClip;
        this.audioSource.volume = this.backgroundMusicVolume;
        this.audioSource.loop = true;
        this.audioSource.Play();
    }
}
