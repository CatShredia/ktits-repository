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
        if (levels == null || levels.Length == 0)
        {
            Debug.LogWarning("SelectedLevelLoader: Список уровней пуст!");
            return;
        }

        int selectedIndex = PlayerPrefs.GetInt("SelectedLevelIndex", 0);

        if (selectedIndex < 0 || selectedIndex >= levels.Length)
        {
            Debug.LogWarning($"SelectedLevelLoader: Невалидный индекс {selectedIndex}, загружаю первый уровень");
            selectedIndex = 0;
        }

        LevelData selectedLevel = levels[selectedIndex];
        if (selectedLevel == null || selectedLevel.levelPrefab == null)
        {
            Debug.LogError($"SelectedLevelLoader: Уровень '{selectedLevel?.name}' не имеет префаб!");
            return;
        }

        Debug.Log($"SelectedLevelLoader: Загружаю уровень '{selectedLevel.name}' (индекс: {selectedIndex})");

        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.LoadLevel(selectedLevel, selectedLevel.spawnOffset);
        }
        else
        {
            Debug.LogError("SelectedLevelLoader: LevelManager.Instance == null!");
            return;
        }

        // Перемещаем игрока к позиции спавна уровня
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Vector3 spawnPos;
            if (selectedLevel.useSpawnPlayerPoint)
            {
                spawnPos = selectedLevel.spawnPlayerPoint;
                Debug.Log($"SelectedLevelLoader: Используем spawnPlayerPoint для спавна игрока на {spawnPos}");
            }
            else
            {
                spawnPos = selectedLevel.playerSpawnPosition;
                Debug.Log($"SelectedLevelLoader: Используем playerSpawnPosition для спавна игрока на {spawnPos}");
            }

            player.transform.position = spawnPos;
            Debug.Log($"SelectedLevelLoader: Игрок перемещён на {spawnPos}");
        }
        else
        {
            Debug.LogWarning("SelectedLevelLoader: Игрок с тегом 'Player' не найден!");
        }

        // Очищаем PlayerPrefs
        PlayerPrefs.DeleteKey("SelectedLevelIndex");
    }
}
