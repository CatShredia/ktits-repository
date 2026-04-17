using UnityEngine;
using System;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    private class LoadedLevel
    {
        public GameObject instance;
        public LevelData data;
    }

    private List<LoadedLevel> loadedLevels = new List<LoadedLevel>();

    /// <summary> Состояние сбора звёзд: индекс → собран ли </summary>
    private bool[] collectedStars = new bool[3];

    /// <summary> Вызывается при сборе любой звезды. Аргумент — индекс (0, 1, 2) </summary>
    public event Action<int> OnStarCollected;

    /// <summary> Вызывается при смене уровня. Сбрасывает звёзды. </summary>
    public event Action OnLevelChanged;

    /// <summary> Активный уровень, в котором сейчас находится игрок </summary>
    public LevelData ActiveLevelData { get; private set; }

    public void SetActiveLevel(LevelData levelData)
    {
        ActiveLevelData = levelData;
    }

    /// <summary>
    /// Сохраняет индекс активного уровня в PlayerPrefs, чтобы SelectedLevelLoader мог его восстановить после перезагрузки сцены.
    /// </summary>
    public void SaveActiveLevelIndex(LevelData[] levels)
    {
        if (ActiveLevelData == null || levels == null) return;
        for (int i = 0; i < levels.Length; i++)
        {
            if (levels[i] == ActiveLevelData)
            {
                PlayerPrefs.SetInt("SelectedLevelIndex", i);
                PlayerPrefs.Save();
                return;
            }
        }
    }

    /// <summary>
    /// Respawn игрока: перезагружает активный уровень (если выгружен) и телепортирует игрока на spawn point.
    /// </summary>
    public void RespawnPlayer()
    {
        if (ActiveLevelData == null)
        {
            Debug.LogWarning("LevelManager: ActiveLevelData не задан, respawn невозможен.");
            return;
        }

        // Перезагружаем уровень если он был выгружен
        bool isLoaded = false;
        foreach (var lvl in loadedLevels)
        {
            if (lvl.data == ActiveLevelData) { isLoaded = true; break; }
        }
        if (!isLoaded)
        {
            ResetStars();
            Vector3 finalPosition = ActiveLevelData.spawnOffset;
            GameObject levelInstance = Instantiate(ActiveLevelData.levelPrefab, finalPosition, Quaternion.identity);
            loadedLevels.Add(new LoadedLevel { instance = levelInstance, data = ActiveLevelData });
            OnLevelChanged?.Invoke();
        }

        // Телепортируем игрока
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Vector3 spawnPos = ActiveLevelData.useSpawnPlayerPoint
                ? ActiveLevelData.spawnPlayerPoint
                : ActiveLevelData.playerSpawnPosition;
            player.transform.position = spawnPos;
            Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
            if (rb != null) rb.linearVelocity = Vector2.zero;
            Debug.Log($"[Respawn] Уровень смерти: '{ActiveLevelData.name}' → возрождение на {spawnPos}");
        }
    }

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
            return;

        // Проверяем, загружен ли уже этот уровень
        foreach (var lvl in loadedLevels)
        {
            if (lvl.data == levelData) return;
        }

        ResetStars();

        Vector3 finalPosition = levelData.spawnOffset;
        GameObject levelInstance = Instantiate(levelData.levelPrefab, finalPosition, Quaternion.identity);
        loadedLevels.Add(new LoadedLevel { instance = levelInstance, data = levelData });

        OnLevelChanged?.Invoke();
    }

    public void UnloadLevel(LevelData levelData)
    {
        for (int i = loadedLevels.Count - 1; i >= 0; i--)
        {
            if (loadedLevels[i].data == levelData)
            {
                Destroy(loadedLevels[i].instance);
                loadedLevels.RemoveAt(i);
                return;
            }
        }
    }

    public void UnloadCurrentLevel()
    {
        for (int i = loadedLevels.Count - 1; i >= 0; i--)
        {
            Destroy(loadedLevels[i].instance);
        }
        loadedLevels.Clear();
    }

    public void CollectStar(int index)
    {
        if (index < 0 || index >= collectedStars.Length) return;
        if (collectedStars[index]) return;

        collectedStars[index] = true;
        OnStarCollected?.Invoke(index);
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

    public bool IsLevelLoaded() => loadedLevels.Count > 0;

    public LevelData GetCurrentLevelData()
    {
        return loadedLevels.Count > 0 ? loadedLevels[0].data : null;
    }

    public GameObject GetCurrentLevelInstance()
    {
        return loadedLevels.Count > 0 ? loadedLevels[0].instance : null;
    }
}
