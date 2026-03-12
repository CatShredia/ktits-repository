using UnityEngine;
using UnityEngine.UI;
using TMPro;

// MusicPlayer
// AudioSource
public class MusicPlayerUIController : MonoBehaviour
{
    public static MusicPlayerUIController Instance { get; private set; }

    [Header("Audio")]
    [SerializeField] private AudioClip[] tracks;
    [SerializeField] private AudioSource audioSource;

    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI trackTitle;
    [SerializeField] private RawImage musicImage;
    [SerializeField] private Texture2D[] trackCovers;

    [Header("Buttons")]
    [SerializeField] private Button lastTrackBtn;
    [SerializeField] private Button nextTrackBtn;
    [SerializeField] private Button pauseTrackBtn;

    [Header("Pause Button Images")]
    [SerializeField] private Image pauseButtonImage;
    [SerializeField] private Sprite pauseSprite;
    [SerializeField] private Sprite playSprite;

    [Header("Settings")]
    [SerializeField] private float musicVolume = 0.1f;

    private int currentTrackIndex = 0;
    private bool isPaused = false;
    private bool isPlaying = false;
    private float trackStartTime = 0f;

    void Awake()
    {
        Instance = this;

        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.playOnAwake = false;
        audioSource.loop = false;
        audioSource.volume = musicVolume;
    }

    void Start()
    {
        SetupButtons();
        PlayTrack(0);
    }

    void Update()
    {
        if (isPlaying && !isPaused && Time.time - trackStartTime > 1f)
        {
            if (!audioSource.isPlaying || audioSource.time >= audioSource.clip.length - 0.1f)
            {
                NextTrack();
            }
        }
    }

    void SetupButtons()
    {
        if (lastTrackBtn != null)
            lastTrackBtn.onClick.AddListener(PrevTrack);

        if (nextTrackBtn != null)
            nextTrackBtn.onClick.AddListener(NextTrack);

        if (pauseTrackBtn != null)
            pauseTrackBtn.onClick.AddListener(TogglePause);
    }

    public void PlayTrack(int index)
    {
        if (tracks == null || tracks.Length == 0)
            return;

        currentTrackIndex = (index % tracks.Length + tracks.Length) % tracks.Length;

        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }

        audioSource.clip = tracks[currentTrackIndex];
        audioSource.Play();
        isPlaying = true;
        isPaused = false;
        trackStartTime = Time.time;

        UpdateUI();
    }

    public void NextTrack()
    {
        int nextIndex = (currentTrackIndex + 1) % tracks.Length;
        PlayTrack(nextIndex);
    }

    public void PrevTrack()
    {
        int prevIndex = currentTrackIndex - 1;
        if (prevIndex < 0)
            prevIndex = tracks.Length - 1;
        PlayTrack(prevIndex);
    }

    public void TogglePause()
    {
        if (!isPlaying)
        {
            return;
        }

        isPaused = !isPaused;

        if (isPaused)
        {
            audioSource.Pause();
        }
        else
        {
            audioSource.UnPause();
        }
        UpdatePauseButton();
    }

    public void SetVolume(float newVolume)
    {
        musicVolume = Mathf.Clamp01(newVolume);
        if (audioSource != null)
        {
            audioSource.volume = musicVolume;
        }
    }

    void UpdateUI()
    {
        UpdateTrackTitle();
        UpdateCoverImage();
        UpdatePauseButton();
    }

    void UpdateTrackTitle()
    {
        if (trackTitle == null || tracks == null || currentTrackIndex >= tracks.Length)
        {
            return;
        }

        string trackName = tracks[currentTrackIndex].name;
        trackTitle.text = trackName;
    }

    void UpdateCoverImage()
    {
        if (musicImage == null || trackCovers == null || currentTrackIndex >= trackCovers.Length)
        {
            return;
        }

        musicImage.texture = trackCovers[currentTrackIndex];
    }

    void UpdatePauseButton()
    {
        if (pauseTrackBtn == null)
        {
            return;
        }

        var textComp = pauseTrackBtn.GetComponentInChildren<TextMeshProUGUI>();

        UpdatePauseButtonImage();
    }

    void UpdatePauseButtonImage()
    {
        if (pauseButtonImage == null)
        {
            return;
        }

        if (isPaused && playSprite != null)
        {
            pauseButtonImage.sprite = playSprite;
        }
        else if (!isPaused && pauseSprite != null)
        {
            pauseButtonImage.sprite = pauseSprite;
        }
    }

    void OnDestroy()
    {
        if (lastTrackBtn != null)
            lastTrackBtn.onClick.RemoveListener(PrevTrack);

        if (nextTrackBtn != null)
            nextTrackBtn.onClick.RemoveListener(NextTrack);

        if (pauseTrackBtn != null)
            pauseTrackBtn.onClick.RemoveListener(TogglePause);
    }
}
