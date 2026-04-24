using UnityEngine;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RustyProject.Network;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }
    public const int StarsPerLevel = 3;

    private const string LevelStarsPrefsPrefix = "LevelStars_";

    [Header("Auto Star Spawning")]
    [Tooltip("Префаб звезды, который автоматически создается на уровнях без расставленных звезд")]
    public GameObject starPrefab;

    [Tooltip("Насколько выше верхней границы уровня автоматически ставить звезды")]
    public float autoStarHeightOffset = 0.5f;

    [Header("Scene Settings")]
    [Tooltip("Сцена главного меню, которая откроется после сбора последней звезды на последнем уровне")]
    public string mainMenuSceneName = "SystemUIs";

    private class LoadedLevel
    {
        public GameObject instance;
        public LevelData data;
    }

    private List<LoadedLevel> loadedLevels = new List<LoadedLevel>();
    private readonly Dictionary<LevelData, bool[]> levelStars = new Dictionary<LevelData, bool[]>();
    private readonly Dictionary<LevelData, int> levelOrder = new Dictionary<LevelData, int>();
    private bool isTransitioningToMainMenu;

    /// <summary> Вызывается при сборе любой звезды. Аргумент — индекс (0, 1, 2) </summary>
    public event Action<int> OnStarCollected;

    /// <summary> Вызывается при смене уровня. Сбрасывает звёзды. </summary>
    public event Action OnLevelChanged;

    /// <summary> Активный уровень, в котором сейчас находится игрок </summary>
    public LevelData ActiveLevelData { get; private set; }

    public void SetActiveLevel(LevelData levelData)
    {
        if (levelData == null) return;

        EnsureRuntimeLevelStateExists(levelData);
        ActiveLevelData = levelData;
        OnLevelChanged?.Invoke();
    }

    public void ConfigureLevelSequence(LevelData[] levels)
    {
        levelOrder.Clear();
        if (levels == null) return;

        for (int i = 0; i < levels.Length; i++)
        {
            if (levels[i] == null || levelOrder.ContainsKey(levels[i])) continue;
            levelOrder.Add(levels[i], i);
        }
    }

    public void ResetRuntimeLevelStates()
    {
        levelStars.Clear();
        ActiveLevelData = null;
        isTransitioningToMainMenu = false;
    }

    public bool CanLoadLevel(LevelData levelData)
    {
        if (levelData == null) return false;
        if (!levelOrder.TryGetValue(levelData, out int levelIndex)) return true;
        if (levelIndex <= 0) return true;

        LevelData previousLevel = GetLevelByIndex(levelIndex - 1);
        return previousLevel == null || HasCompletedLevel(previousLevel);
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
            Vector3 finalPosition = ActiveLevelData.spawnOffset;
            GameObject levelInstance = Instantiate(ActiveLevelData.levelPrefab, finalPosition, Quaternion.identity);
            EnsureLevelHasStars(levelInstance, ActiveLevelData);
            loadedLevels.Add(new LoadedLevel { instance = levelInstance, data = ActiveLevelData });
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

    void Update()
    {
        if (!Input.GetKeyDown(KeyCode.Escape)) return;

        string activeSceneName = SceneManager.GetActiveScene().name;
        if (activeSceneName == mainMenuSceneName) return;

        ReturnToMainMenu();
    }

    public void LoadLevel(LevelData levelData, Vector3 spawnPosition)
    {
        if (levelData == null || levelData.levelPrefab == null)
            return;
        if (!CanLoadLevel(levelData))
            return;

        // Проверяем, загружен ли уже этот уровень
        foreach (var lvl in loadedLevels)
        {
            if (lvl.data == levelData) return;
        }

        EnsureRuntimeLevelStateExists(levelData);

        Vector3 finalPosition = levelData.spawnOffset;
        GameObject levelInstance = Instantiate(levelData.levelPrefab, finalPosition, Quaternion.identity);
        EnsureLevelHasStars(levelInstance, levelData);
        loadedLevels.Add(new LoadedLevel { instance = levelInstance, data = levelData });
    }

    public void UnloadLevel(LevelData levelData)
    {
        for (int i = loadedLevels.Count - 1; i >= 0; i--)
        {
            if (loadedLevels[i].data == levelData)
            {
                Destroy(loadedLevels[i].instance);
                loadedLevels.RemoveAt(i);
                UpdateActiveLevelAfterUnload(levelData);
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

    public void CollectStar(LevelData levelData, int index)
    {
        if (levelData == null) return;

        bool[] starState = EnsureRuntimeLevelStateExists(levelData);
        if (index < 0 || index >= starState.Length) return;
        if (starState[index]) return;

        starState[index] = true;
        SaveStarProgress(levelData);

        if (levelData == ActiveLevelData)
        {
            OnStarCollected?.Invoke(index);
        }

        if (levelData == ActiveLevelData && GetCollectedStarCount(levelData) >= StarsPerLevel && IsLastLevel(levelData))
        {
            _ = CompleteFinalLevelAsync(levelData);
        }
        else if (levelData == ActiveLevelData)
        {
            _ = SyncProgressAsync(levelData);
        }
    }

    public bool IsStarCollected(int index)
    {
        return IsStarCollected(ActiveLevelData, index);
    }

    public bool IsStarCollected(LevelData levelData, int index)
    {
        if (levelData == null) return false;

        bool[] starState = EnsureRuntimeLevelStateExists(levelData);
        if (index < 0 || index >= starState.Length) return false;
        return starState[index];
    }

    public int GetCollectedStarCount()
    {
        return GetCollectedStarCount(ActiveLevelData);
    }

    public int GetCollectedStarCount(LevelData levelData)
    {
        if (levelData == null) return 0;

        bool[] starState = EnsureRuntimeLevelStateExists(levelData);
        int count = 0;
        for (int i = 0; i < starState.Length; i++)
        {
            if (starState[i])
                count++;
        }

        return count;
    }

    public static int GetSavedStarsForLevel(LevelData levelData)
    {
        if (levelData == null) return 0;

        return Mathf.Clamp(PlayerPrefs.GetInt(GetLevelStarsKey(levelData), 0), 0, StarsPerLevel);
    }

    public static bool HasCompletedLevel(LevelData levelData)
    {
        return GetSavedStarsForLevel(levelData) >= StarsPerLevel;
    }

    private void SaveStarProgress(LevelData levelData)
    {
        if (levelData == null) return;

        int savedStars = GetSavedStarsForLevel(levelData);
        int currentStars = GetCollectedStarCount(levelData);
        PlayerPrefs.SetInt(GetLevelStarsKey(levelData), Mathf.Max(savedStars, currentStars));
        PlayerPrefs.Save();
    }

    private static string GetLevelStarsKey(LevelData levelData)
    {
        return $"{LevelStarsPrefsPrefix}{levelData.name}";
    }

    private void EnsureLevelHasStars(GameObject levelInstance, LevelData levelData)
    {
        if (levelInstance == null || levelData == null) return;

        Star[] stars = levelInstance.GetComponentsInChildren<Star>(true);
        if (stars.Length == 0)
        {
            AutoSpawnStars(levelInstance);
            stars = levelInstance.GetComponentsInChildren<Star>(true);
        }

        NormalizeStarIndices(stars, levelData);
    }

    private void AutoSpawnStars(GameObject levelInstance)
    {
        if (starPrefab == null)
        {
            Debug.LogWarning("LevelManager: starPrefab не назначен, автоспавн звёзд пропущен.");
            return;
        }

        Bounds bounds;
        if (!TryGetLevelBounds(levelInstance, out bounds))
        {
            bounds = new Bounds(levelInstance.transform.position, new Vector3(12f, 6f, 0f));
        }

        GameObject starContainer = new GameObject("AutoSpawnedStars");
        starContainer.transform.SetParent(levelInstance.transform, false);

        float minX = bounds.min.x;
        float maxX = bounds.max.x;
        float spawnY = bounds.max.y + autoStarHeightOffset;

        for (int i = 0; i < StarsPerLevel; i++)
        {
            float t = (i + 1f) / (StarsPerLevel + 1f);
            Vector3 spawnPosition = new Vector3(Mathf.Lerp(minX, maxX, t), spawnY, 0f);
            GameObject starInstance = Instantiate(starPrefab, spawnPosition, Quaternion.identity, starContainer.transform);
            starInstance.name = $"AutoStar_{i}";
        }
    }

    private void NormalizeStarIndices(Star[] stars, LevelData levelData)
    {
        Array.Sort(stars, CompareStarsByPosition);
        HashSet<int> assignedIndices = new HashSet<int>();

        for (int i = 0; i < stars.Length; i++)
        {
            if (stars[i] == null) continue;

            int assignedIndex = stars[i].starIndex;
            if (assignedIndex < 0 || assignedIndex >= StarsPerLevel || assignedIndices.Contains(assignedIndex))
            {
                assignedIndex = GetNextFreeStarIndex(assignedIndices);
            }

            if (assignedIndex >= 0 && assignedIndex < StarsPerLevel)
            {
                assignedIndices.Add(assignedIndex);
                stars[i].starIndex = assignedIndex;
                stars[i].AssignLevel(levelData);

                if (IsStarCollected(levelData, assignedIndex))
                {
                    Destroy(stars[i].gameObject);
                }
            }
            else
            {
                Debug.LogWarning($"LevelManager: на уровне найдено больше {StarsPerLevel} звёзд, лишняя звезда отключена.");
                stars[i].gameObject.SetActive(false);
            }
        }
    }

    private static int CompareStarsByPosition(Star left, Star right)
    {
        if (left == null && right == null) return 0;
        if (left == null) return 1;
        if (right == null) return -1;

        int compareX = left.transform.position.x.CompareTo(right.transform.position.x);
        if (compareX != 0) return compareX;

        return left.transform.position.y.CompareTo(right.transform.position.y);
    }

    private static int GetNextFreeStarIndex(HashSet<int> assignedIndices)
    {
        for (int i = 0; i < StarsPerLevel; i++)
        {
            if (!assignedIndices.Contains(i))
                return i;
        }

        return -1;
    }

    private bool TryGetLevelBounds(GameObject levelInstance, out Bounds bounds)
    {
        Renderer[] renderers = levelInstance.GetComponentsInChildren<Renderer>(true);
        bool hasBounds = false;
        bounds = default;

        for (int i = 0; i < renderers.Length; i++)
        {
            if (!hasBounds)
            {
                bounds = renderers[i].bounds;
                hasBounds = true;
            }
            else
            {
                bounds.Encapsulate(renderers[i].bounds);
            }
        }

        if (hasBounds) return true;

        Collider2D[] colliders = levelInstance.GetComponentsInChildren<Collider2D>(true);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (!hasBounds)
            {
                bounds = colliders[i].bounds;
                hasBounds = true;
            }
            else
            {
                bounds.Encapsulate(colliders[i].bounds);
            }
        }

        return hasBounds;
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

    private bool[] EnsureRuntimeLevelStateExists(LevelData levelData)
    {
        if (levelData == null) return null;
        if (levelStars.TryGetValue(levelData, out bool[] existingState))
            return existingState;

        bool[] starState = new bool[StarsPerLevel];
        levelStars[levelData] = starState;
        return starState;
    }

    private LevelData GetLevelByIndex(int index)
    {
        foreach (var pair in levelOrder)
        {
            if (pair.Value == index)
                return pair.Key;
        }

        return null;
    }

    private bool IsLastLevel(LevelData levelData)
    {
        if (levelData == null) return false;
        if (!levelOrder.TryGetValue(levelData, out int levelIndex)) return false;

        return levelIndex == levelOrder.Count - 1;
    }

    private int GetLevelIndex(LevelData levelData)
    {
        if (levelData == null) return 0;
        return levelOrder.TryGetValue(levelData, out int levelIndex) ? levelIndex : 0;
    }

    private async Task SyncProgressAsync(LevelData levelData)
    {
        if (levelData == null || PlayerAccountManager.Instance == null || !PlayerAccountManager.Instance.IsAuthenticated)
        {
            return;
        }

        int levelIndex = GetLevelIndex(levelData);
        int starsCollected = GetCollectedStarCount(levelData);
        bool completed = starsCollected >= StarsPerLevel;
        int completedIndex = PlayerAccountManager.Instance.GetKnownCompletedLevelIndex();
        if (completed)
        {
            completedIndex = Mathf.Max(completedIndex, levelIndex + 1);
        }

        LevelProgressDto progress = new LevelProgressDto
        {
            levelKey = levelData.name,
            levelIndex = levelIndex,
            starsCollected = starsCollected,
            completed = completed
        };

        await PlayerAccountManager.Instance.SaveProgressAsync(completedIndex, new[] { progress });
    }

    private async Task CompleteFinalLevelAsync(LevelData levelData)
    {
        await SyncProgressAsync(levelData);
        ReturnToMainMenuAfterFinalLevel();
    }

    private void ReturnToMainMenuAfterFinalLevel()
    {
        if (isTransitioningToMainMenu) return;
        isTransitioningToMainMenu = true;

        ReturnToMainMenu();
    }

    private void ReturnToMainMenu()
    {
        ResetRuntimeLevelStates();
        UnloadCurrentLevel();

        if (GameManager.Instance != null)
        {
            GameManager.Instance.ResetScore();
            GameManager.Instance.ResetLives();
        }

        SceneManager.LoadScene(mainMenuSceneName);
    }

    private void UpdateActiveLevelAfterUnload(LevelData unloadedLevelData)
    {
        if (ActiveLevelData != unloadedLevelData) return;

        ActiveLevelData = loadedLevels.Count > 0 ? loadedLevels[loadedLevels.Count - 1].data : null;
        OnLevelChanged?.Invoke();
    }
}
