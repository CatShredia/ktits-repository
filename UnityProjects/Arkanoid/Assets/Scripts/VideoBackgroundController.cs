using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

// Attach to: SystemCanvas or BackGroundImage GameObject
// Required: VideoPlayer component on BackGroundImage
// Assign: 3 video clips for each level
public class VideoBackgroundController : MonoBehaviour
{
    public static VideoBackgroundController Instance { get; private set; }

    [Header("Video Clips")]
    [SerializeField] private VideoClip[] levelVideos = new VideoClip[3];

    private VideoPlayer videoPlayer;

    void Awake()
    {
        Instance = this;

        videoPlayer = GetComponent<VideoPlayer>();
        if (videoPlayer == null)
        {
            Debug.LogError("[VideoBackground] VideoPlayer component not found! Adding one...");
            videoPlayer = gameObject.AddComponent<VideoPlayer>();
        }

        // Enable VideoPlayer
        videoPlayer.enabled = true;

        // Setup video player
        videoPlayer.playOnAwake = false;
        videoPlayer.isLooping = true;
        videoPlayer.skipOnDrop = true;
        videoPlayer.waitForFirstFrame = true;

        // Debug: Check Render Texture
        Debug.Log("[VideoBackground] Render Mode: " + videoPlayer.renderMode);
        Debug.Log("[VideoBackground] Target Texture: " + videoPlayer.targetTexture);
        Debug.Log("[VideoBackground] VideoPlayer.enabled: " + videoPlayer.enabled);
        Debug.Log("[VideoBackground] Initialized. VideoPlayer: " + (videoPlayer != null));
    }

    void Start()
    {
        Debug.Log("[VideoBackground] Start() - Current scene: " + SceneManager.GetActiveScene().name);
        Invoke(nameof(PlayVideoForCurrentLevel), 0.1f);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PlayVideoForCurrentLevel();
    }

    public void PlayVideoForCurrentLevel()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        int levelIndex = -1;

        Debug.Log("[VideoBackground] PlayVideoForCurrentLevel() - Scene: " + sceneName);

        // Find level index from scene name
        if (sceneName.Contains("FirstLevel"))
            levelIndex = 0;
        else if (sceneName.Contains("SecondLevel"))
            levelIndex = 1;
        else if (sceneName.Contains("ThirdLevel"))
            levelIndex = 2;

        Debug.Log("[VideoBackground] Level index: " + levelIndex);

        if (levelIndex >= 0 && levelIndex < levelVideos.Length)
            PlayVideo(levelIndex);
        else
            Debug.LogWarning("[VideoBackground] Invalid level index or no video assigned");
    }

    public void PlayVideo(int levelIndex)
    {
        Debug.Log("[VideoBackground] PlayVideo(" + levelIndex + ")");

        if (levelIndex < 0 || levelIndex >= levelVideos.Length)
        {
            Debug.LogError("[VideoBackground] Invalid level index: " + levelIndex);
            return;
        }

        if (levelVideos[levelIndex] == null)
        {
            Debug.LogError("[VideoBackground] Video clip at index " + levelIndex + " is null!");
            return;
        }

        if (videoPlayer == null)
        {
            Debug.LogError("[VideoBackground] VideoPlayer is null!");
            return;
        }

        Debug.Log("[VideoBackground] Playing clip: " + levelVideos[levelIndex].name);
        Debug.Log("[VideoBackground] Clip length: " + levelVideos[levelIndex].length + "s");
        videoPlayer.clip = levelVideos[levelIndex];
        videoPlayer.Play();

        Debug.Log("[VideoBackground] VideoPlayer.isPlaying after Play(): " + videoPlayer.isPlaying);
        Debug.Log("[VideoBackground] VideoPlayer.frame: " + videoPlayer.frame);

        Invoke(nameof(CheckVideoStatus), 0.1f);
    }

    void CheckVideoStatus()
    {
        if (videoPlayer != null)
            Debug.Log("[VideoBackground] CheckVideoStatus - isPlaying: " + videoPlayer.isPlaying + ", frame: " + videoPlayer.frame);
    }

    public void StopVideo()
    {
        if (videoPlayer != null && videoPlayer.isPlaying)
            videoPlayer.Stop();
    }
}
