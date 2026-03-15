using UnityEngine;

// GameManager
// AudioSource
public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [SerializeField] private AudioClip ballStartedSound;
    [SerializeField] private AudioClip blockDestroyedSound;
    [SerializeField] private AudioClip heartCollectedSound;
    [SerializeField] private AudioClip startGameSound;
    [SerializeField] private AudioClip blockHitSound;

    [SerializeField] private float sfxVolume = 0.7f;

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
        {
            audioSource.PlayOneShot(clip);
        }
    }

    public void PlayBallStarted()
    {
        if (ballStartedSound != null)
        {
            audioSource.PlayOneShot(ballStartedSound);
        }
    }

    public void PlayBlockDestroyed()
    {
        if (blockDestroyedSound != null)
        {
            audioSource.PlayOneShot(blockDestroyedSound);
        }
    }

    public void PlayHeartCollected()
    {
        if (heartCollectedSound != null)
        {
            audioSource.PlayOneShot(heartCollectedSound);
        }
    }

    public void PlayStartGame()
    {
        if (startGameSound != null)
        {
            audioSource.PlayOneShot(startGameSound);
        }
    }

    public void PlayBlockHit()
    {
        if (blockHitSound != null)
        {
            audioSource.PlayOneShot(blockHitSound);
        }
    }

    public void SetVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        if (audioSource != null)
        {
            audioSource.volume = sfxVolume;
        }
    }
}
