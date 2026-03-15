using UnityEngine;
using UnityEngine.Video;

// BackGroundImage
// VideoPlayer
public class VideoBackgroundController : MonoBehaviour
{
    public static VideoBackgroundController Instance { get; private set; }

    [SerializeField] private VideoClip[] levelVideos = new VideoClip[4];

    [SerializeField] private bool autoPlayOnStart = true;

    [SerializeField] private UnityEngine.UI.RawImage backgroundRawImage;

    private VideoPlayer videoPlayer;
    private int currentLevelIndex = -1;

    void Awake()
    {
        Instance = this;

        if (videoPlayer == null)
            videoPlayer = GetComponent<VideoPlayer>();

        if (videoPlayer == null)
            videoPlayer = gameObject.AddComponent<VideoPlayer>();

        if (backgroundRawImage == null)
            backgroundRawImage = GetComponent<UnityEngine.UI.RawImage>();

        videoPlayer.enabled = true;
        videoPlayer.gameObject.SetActive(true);

        videoPlayer.playOnAwake = false;
        videoPlayer.isLooping = true;
        videoPlayer.skipOnDrop = true;
        videoPlayer.waitForFirstFrame = false;
    }

    void Start()
    {
        if (autoPlayOnStart)
        {
            Invoke(nameof(PlayFirstVideo), 0.5f);
        }
    }

    void PlayFirstVideo()
    {
        PlayVideo(0);
    }

    public void PlayVideoForLevel(int levelIndex)
    {
        if (levelIndex == currentLevelIndex)
        {
            return;
        }

        PlayVideo(levelIndex);
    }

    public void PlayVideo(int levelIndex)
    {
        Debug.Log(levelIndex);
        Debug.Log(levelVideos.Length);

        if (levelVideos[levelIndex] == null)
        {
            Debug.LogWarning($"[Video] No video assigned for level {levelIndex}!");
        }

        if (levelVideos == null || levelVideos.Length == 0)
        {
            return;
        }

        levelIndex = (levelIndex % levelVideos.Length + levelVideos.Length) % levelVideos.Length;

        if (levelVideos[levelIndex] == null)
        {
            return;
        }

        if (videoPlayer == null)
        {
            return;
        }

        videoPlayer.enabled = true;
        videoPlayer.gameObject.SetActive(true);
        videoPlayer.playOnAwake = false;
        videoPlayer.isLooping = true;

        if (backgroundRawImage != null && videoPlayer.targetTexture != null)
        {
            backgroundRawImage.texture = videoPlayer.targetTexture;
        }

        currentLevelIndex = levelIndex;

        if (videoPlayer.isPlaying)
        {
            videoPlayer.Stop();
        }

        videoPlayer.clip = levelVideos[levelIndex];
        videoPlayer.Prepare();

        Invoke(nameof(PlayVideoDelayed), 0.5f);
    }

    void PlayVideoDelayed()
    {
        videoPlayer.Play();
    }

    public void StopVideo()
    {
        if (videoPlayer != null && videoPlayer.isPlaying)
        {
            videoPlayer.Stop();
            currentLevelIndex = -1;
        }
    }

    public void ResumeVideo()
    {
        if (videoPlayer != null && !videoPlayer.isPlaying && currentLevelIndex >= 0)
        {
            videoPlayer.Play();
        }
    }

    public int GetCurrentLevelIndex()
    {
        return currentLevelIndex;
    }
}
