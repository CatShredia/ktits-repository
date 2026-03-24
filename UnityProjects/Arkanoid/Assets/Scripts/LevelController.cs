using UnityEngine;

// GameManager
public class LevelController : MonoBehaviour
{
    public static LevelController Instance { get; private set; }

    [SerializeField] private GameObject[] levelPrefabs;

    [SerializeField] private Vector3 spawnPosition = new Vector3(-4f, 1.2f, 0f);

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

    }

    public void LoadLevel(int index)
    {
        if (currentLevelInstance != null)
        {
            ResetBonusEffects();
        }

        if (currentLevelInstance != null)
        {
            Destroy(currentLevelInstance);
        }

        currentLevelIndex = (index % levelPrefabs.Length + levelPrefabs.Length) % levelPrefabs.Length;

        if (levelPrefabs[currentLevelIndex] != null)
        {
            currentLevelInstance = Instantiate(levelPrefabs[currentLevelIndex], spawnPosition, Quaternion.identity);
            currentBlocks = currentLevelInstance.GetComponentsInChildren<BlockController>(true);

            totalBlocksAtStart = GetDestructibleBlocksCount();

            Debug.Log($"[Level] Loaded level {currentLevelIndex + 1}: {levelPrefabs[currentLevelIndex].name}");
            Debug.Log($"[Level] Total blocks at start: {totalBlocksAtStart}");

            VideoBackgroundController.Instance?.PlayVideoForLevel(currentLevelIndex);
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

        Invoke(nameof(CheckRemainingBlocks), 0.1f);
    }

    void CheckRemainingBlocks()
    {
        if (currentBlocks == null) return;

        int remainingBlocks = GetDestructibleBlocksCount();

        // TODO: счетчик
        Debug.Log($"[Level] Blocks remaining: {remainingBlocks} / {totalBlocksAtStart}");

        if (remainingBlocks <= 0)
        {
            Debug.Log("[Level] Level complete!");
            Invoke(nameof(LoadNextLevel), 0.5f);
        }
    }

    int GetDestructibleBlocksCount()
    {
        if (currentBlocks == null) return 0;

        int count = 0;
        foreach (var block in currentBlocks)
        {
            // Count only active blocks that are not Invulnerable
            if (block != null && block.gameObject.activeInHierarchy && !block.IsInvulnerable)
            {
                count++;
            }
        }
        return count;
    }

    void ResetBonusEffects()
    {
        var platform = FindObjectOfType<PlatformController>();
        platform?.ResetPlatform();

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
