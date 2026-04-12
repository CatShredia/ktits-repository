using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    private GameObject currentLevelInstance;
    private LevelData currentLevelData;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadLevel(LevelData levelData, Vector3 spawnPosition)
    {
        if (levelData == null || levelData.levelPrefab == null)
        {
            Debug.LogWarning("LevelManager: LevelData или префаб не указаны!");
            return;
        }

        if (currentLevelData == levelData && currentLevelInstance != null)
        {
            return;
        }

        UnloadCurrentLevel();

        Vector3 finalPosition = spawnPosition + levelData.spawnOffset;
        currentLevelInstance = Instantiate(levelData.levelPrefab, finalPosition, Quaternion.identity);
        currentLevelData = levelData;

        Debug.Log($"LevelManager: Уровень '{levelData.name}' загружен на позицию {finalPosition}");
    }

    public void UnloadCurrentLevel()
    {
        if (currentLevelInstance != null)
        {
            Destroy(currentLevelInstance);
            Debug.Log($"LevelManager: Уровень '{currentLevelData.name}' выгружен");
            currentLevelInstance = null;
            currentLevelData = null;
        }
    }

    public bool IsLevelLoaded() => currentLevelInstance != null;

    public LevelData GetCurrentLevelData() => currentLevelData;

    public GameObject GetCurrentLevelInstance() => currentLevelInstance;
}
