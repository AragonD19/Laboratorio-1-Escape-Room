using UnityEngine;

public class UIAudioManager : MonoBehaviour
{
    public static UIAudioManager Instance;

    [Header("Sonidos de UI")]
    [SerializeField] private AudioSource uiAudioSource;
    [SerializeField] private AudioClip clickClip;
    [SerializeField] private AudioClip hoverClip;

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

    public void PlayClickSound()
    {
        if (clickClip != null)
            uiAudioSource.PlayOneShot(clickClip);
    }

    public void PlayHoverSound()
    {
        if (hoverClip != null)
            uiAudioSource.PlayOneShot(hoverClip);
    }
}
