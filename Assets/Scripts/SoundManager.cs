using UnityEngine;
using Unity.Cinemachine;

[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip jumpSound;
    [SerializeField] private AudioClip landingSound;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }        
    }

    public void PlayJumpSound()
    {
        audioSource.PlayOneShot(jumpSound);
    }

    public void PlayLandingSound()
    {
        audioSource.PlayOneShot(landingSound);
    }
}
