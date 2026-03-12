using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Attach to: MusicPlayer GameObject in MusicPanel Canvas
// Required: AudioSource component
// Assign: 3 tracks, 3 covers, UI elements
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
        // Auto-play next track when current ends
        // Only check if track has been playing for at least 0.5 seconds
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

        // Clamp index
        currentTrackIndex = (index % tracks.Length + tracks.Length) % tracks.Length;

        // Stop current
        if (audioSource.isPlaying)
            audioSource.Stop();

        // Play new track
        audioSource.clip = tracks[currentTrackIndex];
        audioSource.Play();
        isPlaying = true;
        isPaused = false;
        trackStartTime = Time.time;  // Track when playback started

        UpdateUI();
    }

    public void NextTrack()
    {
        int nextIndex = (currentTrackIndex + 1) % tracks.Length;
        PlayTrack(nextIndex);
        Debug.Log($"[Music] Next track: {currentTrackIndex + 1}");
    }

    public void PrevTrack()
    {
        int prevIndex = currentTrackIndex - 1;
        if (prevIndex < 0)
            prevIndex = tracks.Length - 1;
        PlayTrack(prevIndex);
        Debug.Log($"[Music] Previous track: {currentTrackIndex + 1}");
    }

    public void TogglePause()
    {
        if (!isPlaying)
            return;

        isPaused = !isPaused;

        if (isPaused)
            audioSource.Pause();
        else
            audioSource.UnPause();

        UpdatePauseButton();
        Debug.Log($"[Music] {(isPaused ? "Paused" : "Resumed")}");
    }

    public void SetVolume(float newVolume)
    {
        musicVolume = Mathf.Clamp01(newVolume);
        if (audioSource != null)
            audioSource.volume = musicVolume;
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
            return;

        string trackName = tracks[currentTrackIndex].name;
        trackTitle.text = trackName;
    }

    void UpdateCoverImage()
    {
        if (musicImage == null || trackCovers == null || currentTrackIndex >= trackCovers.Length)
            return;

        musicImage.texture = trackCovers[currentTrackIndex];
    }

    void UpdatePauseButton()
    {
        if (pauseTrackBtn == null)
            return;

        // Update text
        var textComp = pauseTrackBtn.GetComponentInChildren<TextMeshProUGUI>();
        if (textComp != null)
        {
            textComp.text = isPaused ? "▶ Resume" : "⏸ Pause";
        }

        // Update image
        UpdatePauseButtonImage();
    }

    void UpdatePauseButtonImage()
    {
        if (pauseButtonImage == null)
            return;

        if (isPaused && playSprite != null)
            pauseButtonImage.sprite = playSprite;
        else if (!isPaused && pauseSprite != null)
            pauseButtonImage.sprite = pauseSprite;
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
