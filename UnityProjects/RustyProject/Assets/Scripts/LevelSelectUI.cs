using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

/// <summary>
/// Меню выбора уровней. Кнопки уже расставлены на сцене вручную.
/// Для каждого уровня назначьте кнопку и LevelData в Inspector.
/// </summary>
public class LevelSelectUI : MonoBehaviour
{
    [System.Serializable]
    public class LevelButtonPair
    {
        [Tooltip("Кнопка на сцене")]
        public GameObject button;

        [Tooltip("Данные уровня (LevelData ScriptableObject)")]
        public LevelData levelData;
    }

    [Header("Level Buttons")]
    [Tooltip("Список пар: кнопка + уровень")]
    public LevelButtonPair[] levelButtons;

    [Header("Scene Settings")]
    [Tooltip("Название игровой сцены для загрузки")]
    public string gameSceneName = "SampleScene";

    void Start()
    {
        if (levelButtons == null || levelButtons.Length == 0)
        {
            Debug.LogWarning("LevelSelectUI: Список кнопок пуст!");
            return;
        }

        for (int i = 0; i < levelButtons.Length; i++)
        {
            var pair = levelButtons[i];
            if (pair.button == null || pair.levelData == null) continue;

            // Устанавливаем текст кнопки из названия уровня
            TMP_Text buttonText = pair.button.GetComponentInChildren<TMP_Text>();
            if (buttonText != null)
            {
                buttonText.text = pair.levelData.name;
            }
            else
            {
                Text uiText = pair.button.GetComponentInChildren<Text>();
                if (uiText != null)
                {
                    uiText.text = pair.levelData.name;
                }
            }

            // Вешаем обработчик
            Button btn = pair.button.GetComponent<Button>();
            if (btn != null)
            {
                int index = i;
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() => OnLevelSelected(index));
            }
        }
    }

    private void OnLevelSelected(int index)
    {
        if (index < 0 || index >= levelButtons.Length) return;

        var pair = levelButtons[index];
        if (pair.levelData == null || pair.levelData.levelPrefab == null)
        {
            Debug.LogWarning($"LevelSelectUI: Уровень '{pair.levelData?.name}' не имеет префаб!");
            return;
        }

        Debug.Log($"LevelSelectUI: Выбран уровень '{pair.levelData.name}' — загрузка сцены {gameSceneName}");

        // Сохраняем индекс выбранного уровня для LevelManager
        PlayerPrefs.SetInt("SelectedLevelIndex", index);
        PlayerPrefs.Save();

        SceneManager.LoadScene(gameSceneName);
    }

    /// <summary>
    /// Кнопка "Назад" — вернуться в главное меню.
    /// </summary>
    public void GoBack()
    {
        Debug.Log("LevelSelectUI: Возврат в главное меню");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }
}
