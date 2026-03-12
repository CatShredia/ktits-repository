using UnityEngine;

// GameManager or empty GameObject in scene
// Assign: All level prefabs from Assets/Prefabs/Levels/
public class LevelController : MonoBehaviour
{
    public static LevelController Instance { get; private set; }

    [Header("Level Prefabs")]
    [SerializeField] private GameObject[] levelPrefabs;  // FirstLevel, SecondLevel, ThirdLevel

    [Header("Spawn Position")]
    [SerializeField] private Vector3 spawnPosition = new Vector3(-4f, 2f, 0f);

    [Header("Debug")]
    [SerializeField] private int currentLevelIndex = 0;

    private GameObject currentLevelInstance;
    private BlockController[] currentBlocks;
    private int totalBlocksAtStart = 0;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        LoadLevel(0);  // Start with first level
    }

    public void LoadLevel(int index)
    {
        // Reset bonus effects before loading new level (except first load)
        if (currentLevelInstance != null)
        {
            ResetBonusEffects();
        }

        // Clear current level
        if (currentLevelInstance != null)
        {
            Destroy(currentLevelInstance);
        }

        // Clamp index (handle negative and overflow)
        currentLevelIndex = (index % levelPrefabs.Length + levelPrefabs.Length) % levelPrefabs.Length;

        // Spawn new level
        if (levelPrefabs[currentLevelIndex] != null)
        {
            currentLevelInstance = Instantiate(levelPrefabs[currentLevelIndex], spawnPosition, Quaternion.identity);
            currentBlocks = currentLevelInstance.GetComponentsInChildren<BlockController>(true);
            totalBlocksAtStart = currentBlocks.Length;
            
            Debug.Log($"[Level] Loaded level {currentLevelIndex + 1}: {levelPrefabs[currentLevelIndex].name}");
            Debug.Log($"[Level] Total blocks at start: {totalBlocksAtStart}");
        }
        else
        {
            Debug.LogError($"[Level] Level prefab at index {currentLevelIndex} is null!");
        }
    }

    public void LoadNextLevel()
    {
        int nextIndex = (currentLevelIndex + 1) % levelPrefabs.Length;
        LoadLevel(nextIndex);
    }

    public void BlockDestroyed()
    {
        if (currentBlocks == null) return;

        // Count remaining active blocks with a small delay
        Invoke(nameof(CheckRemainingBlocks), 0.1f);
    }

    void CheckRemainingBlocks()
    {
        if (currentBlocks == null) return;

        // Count remaining active blocks
        int remainingBlocks = 0;
        foreach (var block in currentBlocks)
        {
            if (block != null && block.gameObject.activeInHierarchy)
                remainingBlocks++;
        }

        Debug.Log($"[Level] Blocks remaining: {remainingBlocks} / {totalBlocksAtStart}");

        if (remainingBlocks <= 0)
        {
            Debug.Log("[Level] Level complete!");
            Invoke(nameof(LoadNextLevel), 1f);  // Delay before next level
        }
    }

    void ResetBonusEffects()
    {
        // Reset platform
        var platform = FindObjectOfType<PlatformController>();
        platform?.ResetPlatform();

        // Reset ball to platform
        var balls = FindObjectsOfType<BallController>();
        foreach (var ball in balls)
        {
            if (ball != null && !ball.isClone && ball.playerObject != null)
            {
                ball.isActiveBalls = false;
                var platformPos = ball.playerObject.transform.position;
                ball.transform.position = new Vector3(platformPos.x, ball.transform.position.y, ball.transform.position.z);
            }
        }

        // Clear bonus UI
        BonusUIManager.Instance?.ClearEffectText();

        Debug.Log("[Level] Bonus effects reset");
    }

    public int GetCurrentLevelIndex()
    {
        return currentLevelIndex;
    }

    public string GetCurrentLevelName()
    {
        if (levelPrefabs == null || levelPrefabs.Length == 0 || currentLevelIndex >= levelPrefabs.Length)
            return "Unknown";

        return levelPrefabs[currentLevelIndex].name;
    }
}
