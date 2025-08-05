using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    [Header("Música de fondo")]
    [SerializeField] private AudioSource menuMusic;
    [SerializeField] private AudioSource gameMusic;
    private float GameMusicVolume;
    private bool isGameMusicPaused = false;

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


    //Musica de fondo
    public void PlayMenuMusic()
    {
        if (!menuMusic.isPlaying)
        {
            StopAllMusic();
            menuMusic.Play();
        }
    }


    public void PlayGameMusic()
    {
        if (!gameMusic.isPlaying)
        {
            StopAllMusic();
            gameMusic.Play();
        }
    }

    public void PauseGameMusic()
    {
        if (gameMusic.isPlaying && !isGameMusicPaused)
        {
            GameMusicVolume = gameMusic.volume;
            gameMusic.volume = GameMusicVolume/2;
            isGameMusicPaused = true;
        }
    }

    public void ResumeGameMusic()
    {
        if (gameMusic.isPlaying && isGameMusicPaused)
        {
            gameMusic.volume = GameMusicVolume;
            isGameMusicPaused = false;
        }
    }

    private void StopAllMusic()
    {
        menuMusic.Stop();
        gameMusic.Stop();
    }
}
