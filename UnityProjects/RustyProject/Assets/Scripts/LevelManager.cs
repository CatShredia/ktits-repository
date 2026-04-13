using UnityEngine;
using System;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    private GameObject currentLevelInstance;
    private LevelData currentLevelData;

    /// <summary> Состояние сбора звёзд: индекс → собран ли </summary>
    private bool[] collectedStars = new bool[3];

    /// <summary> Вызывается при сборе любой звезды. Аргумент — индекс (0, 1, 2) </summary>
    public event Action<int> OnStarCollected;

    /// <summary> Вызывается при смене уровня. Сбрасывает звёзды. </summary>
    public event Action OnLevelChanged;

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
        ResetStars();

        Vector3 finalPosition = levelData.spawnOffset;
        currentLevelInstance = Instantiate(levelData.levelPrefab, finalPosition, Quaternion.identity);
        currentLevelData = levelData;

        OnLevelChanged?.Invoke();
        Debug.Log($"LevelManager: Уровень '{levelData.name}' загружен на позицию {finalPosition}");
    }

    public void UnloadCurrentLevel()
    {
        if (currentLevelInstance != null)
        {
            Destroy(currentLevelInstance);
            Debug.Log($"LevelManager: Уровень '{currentLevelData?.name}' выгружен");
            currentLevelInstance = null;
            currentLevelData = null;
        }
    }

    public void CollectStar(int index)
    {
        if (index < 0 || index >= collectedStars.Length) return;
        if (collectedStars[index]) return;

        collectedStars[index] = true;
        OnStarCollected?.Invoke(index);
        Debug.Log($"LevelManager: Звезда {index} собрана!");
    }

    public bool IsStarCollected(int index)
    {
        if (index < 0 || index >= collectedStars.Length) return false;
        return collectedStars[index];
    }

    private void ResetStars()
    {
        for (int i = 0; i < collectedStars.Length; i++)
            collectedStars[i] = false;
    }

    public bool IsLevelLoaded() => currentLevelInstance != null;

    public LevelData GetCurrentLevelData() => currentLevelData;

    public GameObject GetCurrentLevelInstance() => currentLevelInstance;
}
