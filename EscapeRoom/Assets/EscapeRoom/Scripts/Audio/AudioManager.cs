using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;


    [Header("Sonidos del Jugador")]
    [SerializeField] private AudioSource playerAudioSource;
    [SerializeField] private List<AudioClip> walkClips;
    [SerializeField] private List<AudioClip> runClips;
    [SerializeField] private AudioClip jumpStartClip;
    [SerializeField] private AudioClip jumpLandClip;


    private Coroutine stepCoroutine;

    private void Awake()
    {

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }


    // Sonidos del jugador
    public void PlayFootsteps(bool isRunning)
    {
        if (stepCoroutine != null) return;
        stepCoroutine = StartCoroutine(PlayFootstepSequence(isRunning));
    }

    public void StopFootsteps()
    {
        if (stepCoroutine != null)
        {
            StopCoroutine(stepCoroutine);
            stepCoroutine = null;
            playerAudioSource.Stop();
        }
    }

    private IEnumerator PlayFootstepSequence(bool isRunning)
    {
        List<AudioClip> clips = isRunning ? runClips : walkClips;
        float delay = isRunning ? 0.3f : 0.5f;

        int index = 0;
        while (true)
        {
            playerAudioSource.PlayOneShot(clips[index]);
            index = (index + 1) % clips.Count;
            yield return new WaitForSeconds(delay);
        }
    }

    public void PlayJumpStart()
    {
        playerAudioSource.PlayOneShot(jumpStartClip);
    }

    public void PlayJumpLand()
    {
        playerAudioSource.PlayOneShot(jumpLandClip);
    }


    public void SetPlayerAudioSource(AudioSource source)
    {
        playerAudioSource = source;
    }

}
