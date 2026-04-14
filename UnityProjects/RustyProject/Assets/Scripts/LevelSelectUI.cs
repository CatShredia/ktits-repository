using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

/// <summary>
/// Меню выбора уровней. Вешается на пустой объект в сцене SystemUIs 1.
/// Автоматически создаёт кнопки для каждого уровня из levels.
/// Привяжите LevelData через Inspector.
/// </summary>
public class LevelSelectUI : MonoBehaviour
{
    [Header("Level Data")]
    [Tooltip("Список уровней (LevelData ScriptableObject)")]
    public LevelData[] levels;

    [Header("UI Settings")]
    [Tooltip("Родительский объект для кнопок уровней")]
    public Transform buttonsParent;

    [Tooltip("Префаб кнопки уровня (должен иметь Button и TextMeshProUGUI)")]
    public GameObject levelButtonPrefab;

    [Header("Scene Settings")]
    [Tooltip("Название игровой сцены для загрузки")]
    public string gameSceneName = "SampleScene";

    void Start()
    {
        if (buttonsParent == null)
        {
            Debug.LogWarning("LevelSelectUI: buttonsParent не назначен! Создаю пустой объект.");
            var obj = new GameObject("LevelButtonsContainer");
            buttonsParent = obj.transform;
        }

        if (levelButtonPrefab == null)
        {
            Debug.LogWarning("LevelSelectUI: levelButtonPrefab не назначен! Создаю кнопки программно.");
        }

        SpawnLevelButtons();
    }

    private void SpawnLevelButtons()
    {
        if (levels == null || levels.Length == 0)
        {
            Debug.LogWarning("LevelSelectUI: Список уровней пуст!");
            return;
        }

        for (int i = 0; i < levels.Length; i++)
        {
            LevelData levelData = levels[i];
            if (levelData == null) continue;

            GameObject buttonObj;

            if (levelButtonPrefab != null)
            {
                buttonObj = Instantiate(levelButtonPrefab, buttonsParent);

                // Для префаба — только сбрасываем позицию, размеры и якоря берём из префаба
                RectTransform rect = buttonObj.GetComponent<RectTransform>();
                if (rect != null)
                {
                    rect.localPosition = Vector3.zero;
                    rect.localRotation = Quaternion.identity;
                    rect.localScale = Vector3.one;
                    rect.anchoredPosition = new Vector2(0, -50 * i);
                }
            }
            else
            {
                buttonObj = CreateButtonProgrammatically(levelData.name, i);
            }

            // Устанавливаем текст кнопки
            TMP_Text buttonText = buttonObj.GetComponentInChildren<TMP_Text>();
            if (buttonText != null)
            {
                buttonText.text = levelData.name;
            }
            else
            {
                Text uiText = buttonObj.GetComponentInChildren<Text>();
                if (uiText != null)
                {
                    uiText.text = levelData.name;
                }
            }

            // Захватываем индекс для замыкания
            int levelIndex = i;
            Button btn = buttonObj.GetComponent<Button>();
            if (btn != null)
            {
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() => OnLevelSelected(levelIndex));
            }
        }
    }

    private GameObject CreateButtonProgrammatically(string name, int index)
    {
        GameObject buttonObj = new GameObject($"Button_{name}");
        buttonObj.transform.SetParent(buttonsParent, false);

        // RectTransform
        RectTransform rect = buttonObj.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(150, 40);
        rect.anchorMin = new Vector2(0.5f, 1);
        rect.anchorMax = new Vector2(0.5f, 1);
        rect.pivot = new Vector2(0.5f, 1);
        rect.anchoredPosition = new Vector2(0, -50 * index);

        // Button
        Button btn = buttonObj.AddComponent<Button>();

        // Image (фон)
        Image img = buttonObj.AddComponent<Image>();
        img.color = new Color(0.3f, 0.3f, 0.3f);

        // TextMeshProUGUI или Text
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(buttonObj.transform, false);
        textObj.AddComponent<RectTransform>().sizeDelta = new Vector2(150, 40);

        // Пробуем TMP
        var tmp = textObj.AddComponent<TextMeshProUGUI>();
        tmp.text = name;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.fontSize = 16;
        tmp.color = Color.white;

        return buttonObj;
    }

    private void OnLevelSelected(int index)
    {
        if (index < 0 || index >= levels.Length) return;

        LevelData selectedLevel = levels[index];
        if (selectedLevel == null || selectedLevel.levelPrefab == null)
        {
            Debug.LogWarning($"LevelSelectUI: Уровень '{levels[index].name}' не имеет префаб!");
            return;
        }

        Debug.Log($"LevelSelectUI: Выбран уровень '{selectedLevel.name}' — загрузка сцены {gameSceneName}");

        // Загружаем игровую сцену
        SceneManager.LoadScene(gameSceneName);

        // После загрузки сцены загружаем уровень через LevelManager
        // Используем Coroutine чтобы дождаться загрузки
        StartCoroutine(LoadSelectedLevelAfterSceneLoad(selectedLevel));
    }

    private System.Collections.IEnumerator LoadSelectedLevelAfterSceneLoad(LevelData levelData)
    {
        yield return null; // Ждём один кадр чтобы SceneManager обновился

        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.LoadLevel(levelData, levelData.spawnOffset);
            Debug.Log($"LevelSelectUI: Уровень '{levelData.name}' загружен через LevelManager");
        }
        else
        {
            Debug.LogError("LevelSelectUI: LevelManager.Instance == null!");
        }
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
