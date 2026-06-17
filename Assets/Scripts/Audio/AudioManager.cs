using UnityEngine;

/// <summary>
/// Central one-shot SFX player. Survives the objects that trigger sounds
/// (enemy death, projectile destroy) because it plays on its own AudioSource.
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("SFX")]
    [SerializeField] private AudioSource sfxSource;
    [Range(0f, 1f)] [SerializeField] private float masterVolume = 1f;

    [Header("Music")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioClip backgroundMusic;
    [Range(0f, 1f)] [SerializeField] private float musicVolume = 0.5f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (sfxSource == null)
            sfxSource = GetComponent<AudioSource>();

        if (musicSource == null)
        {
            musicSource = gameObject.AddComponent<AudioSource>();
            musicSource.playOnAwake = false;
        }

        musicSource.loop = true;
    }

    private void Start()
    {
        if (backgroundMusic != null)
            PlayMusic(backgroundMusic);
    }

    public void PlayMusic(AudioClip clip)
    {
        if (clip == null || musicSource == null) return;
        if (musicSource.clip == clip && musicSource.isPlaying) return;

        musicSource.clip = clip;
        musicSource.volume = musicVolume * masterVolume;
        musicSource.Play();
    }

    public void StopMusic()
    {
        if (musicSource != null)
            musicSource.Stop();
    }

    public void PlaySFX(AudioClip clip, float volume = 1f)
    {
        if (clip == null || sfxSource == null) return;
        sfxSource.PlayOneShot(clip, volume * masterVolume);
    }

    public void PlaySFXAt(AudioClip clip, Vector3 position, float volume = 1f)
    {
        if (clip == null) return;
        AudioSource.PlayClipAtPoint(clip, position, volume * masterVolume);
    }
}
