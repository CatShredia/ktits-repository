using UnityEngine;

// Attach to: SoundManager GameObject (persistent in scene)
// Required: AudioSource component (auto-added if missing)
// Assign: All sound effect clips
public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("Sound Effects")]
    [SerializeField] private AudioClip ballStartedSound;
    [SerializeField] private AudioClip blockDestroyedSound;
    [SerializeField] private AudioClip heartCollectedSound;
    [SerializeField] private AudioClip startGameSound;

    [Header("Settings")]
    [SerializeField] private float sfxVolume = 0.7f;  // Louder than music

    private AudioSource audioSource;

    void Awake()
    {
        Instance = this;

        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.playOnAwake = false;
        audioSource.volume = sfxVolume;
    }

    void Start()
    {
        PlayStartGame();
    }

    public void PlaySound(AudioClip clip)
    {
        if (clip == null) return;
        audioSource.PlayOneShot(clip);
    }

    public void PlayBallStarted()
    {
        if (ballStartedSound != null)
            audioSource.PlayOneShot(ballStartedSound);
    }

    public void PlayBlockDestroyed()
    {
        if (blockDestroyedSound != null)
            audioSource.PlayOneShot(blockDestroyedSound);
    }

    public void PlayHeartCollected()
    {
        if (heartCollectedSound != null)
            audioSource.PlayOneShot(heartCollectedSound);
    }

    public void PlayStartGame()
    {
        if (startGameSound != null)
            audioSource.PlayOneShot(startGameSound);
    }

    public void SetVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        if (audioSource != null)
            audioSource.volume = sfxVolume;
    }
}
