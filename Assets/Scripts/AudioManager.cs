using UnityEngine;
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("BGM Clips")]
    [SerializeField]
    private AudioClip startGameBGM;
    [SerializeField]
    private AudioClip inGameBGM;
    
    [Header("SFX Clips")]
    public AudioClip clickSound;

    private AudioSource sfxSource;
    private AudioSource bgmSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);            
            sfxSource = gameObject.AddComponent<AudioSource>();
            
            // Setup BGM Source
            bgmSource = gameObject.AddComponent<AudioSource>();
            bgmSource.loop = true;
            bgmSource.playOnAwake = false;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }

    public void PlayClickSound()
    {
        PlaySFX(clickSound);
    }

    public void PlayStartGameBGM()
    {
        PlayBGM(startGameBGM);
    }

    public void PlayInGameBGM()
    {
        PlayBGM(inGameBGM);
    }

    private void PlayBGM(AudioClip clip)
    {
        if (clip == null || bgmSource == null) return;
        
        if (bgmSource.clip == clip && bgmSource.isPlaying) return;

        bgmSource.clip = clip;
        bgmSource.Play();
    }
}
