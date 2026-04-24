using UnityEngine;

/// <summary>
/// Вешается на объект в игровой сцене. При старте загружает выбранный уровень через LevelManager.
/// Индекс уровня берётся из PlayerPrefs (устанавливается в LevelSelectUI).
/// </summary>
public class SelectedLevelLoader : MonoBehaviour
{
    [Header("Level List")]
    [Tooltip("Список всех доступных уровней (должен совпадать с LevelSelectUI)")]
    public LevelData[] levels;

    void Start()
    {
        if (levels == null || levels.Length == 0) return;

        int selectedIndex = PlayerPrefs.GetInt("SelectedLevelIndex", 0);
        if (selectedIndex < 0 || selectedIndex >= levels.Length) selectedIndex = 0;
        selectedIndex = GetFirstAllowedLevelIndex(selectedIndex);

        LevelData selectedLevel = levels[selectedIndex];
        if (selectedLevel == null || selectedLevel.levelPrefab == null) return;

        if (LevelManager.Instance == null) return;

        LevelManager.Instance.ResetRuntimeLevelStates();
        LevelManager.Instance.ConfigureLevelSequence(levels);
        LevelManager.Instance.LoadLevel(selectedLevel, selectedLevel.spawnOffset);
        LevelManager.Instance.SetActiveLevel(selectedLevel);

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Vector3 spawnPos = selectedLevel.useSpawnPlayerPoint
                ? selectedLevel.spawnPlayerPoint
                : selectedLevel.playerSpawnPosition;
            player.transform.position = spawnPos;
        }

        PlayerPrefs.DeleteKey("SelectedLevelIndex");
    }

    private int GetFirstAllowedLevelIndex(int requestedIndex)
    {
        int highestUnlockedIndex = 0;

        for (int i = 1; i < levels.Length; i++)
        {
            if (LevelManager.HasCompletedLevel(levels[i - 1]))
            {
                highestUnlockedIndex = i;
            }
            else
            {
                break;
            }
        }

        return Mathf.Clamp(requestedIndex, 0, highestUnlockedIndex);
    }
}
