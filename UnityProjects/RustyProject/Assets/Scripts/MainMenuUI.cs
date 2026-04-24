using System.Collections;
using RustyProject.Network;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Главное меню: Старт, Чат, Таблица лидеров, Аккаунт, Выйти.
/// Вешается на объект MainMenu на сцене меню.
/// Привяжите методы к кнопкам через Unity Inspector (On Click).
/// </summary>
public class MainMenuUI : MonoBehaviour
{
    private MainMenuApiUI apiUi;

    [Header("Animation")]
    [Tooltip("Аниматор меню")]
    public Animator menuAnimator;

    [Tooltip("Время задержки перед загрузкой сцены (секунды)")]
    public float animationDelay = 1f;

    [Header("Debug")]
    [Tooltip("Пропускать начальную анимацию (не ставить IsGameStarted = false)")]
    public bool skipInitialAnimation = false;

    private bool isGameStarted = false;

    [Header("Scene Settings")]
    [Tooltip("Название игровой сцены для кнопки 'Старт'")]
    public string gameSceneName = "SampleScene";

    [Header("Panels")]
    [Tooltip("Панель выбора уровней")]
    public GameObject levelSelectPanel;

    void OnEnable()
    {
        Debug.Log("=== MainMenuUI: OnEnable вызван ===");
    }

    void OnDisable()
    {
        Debug.Log("=== MainMenuUI: OnDisable вызван ===");
    }

    void Awake()
    {
        Debug.Log("=== MainMenuUI: Awake вызван ===");
        Debug.Log($"Активная сцена: {SceneManager.GetActiveScene().name}");
        Debug.Log($"Объект активен в иерархии: {gameObject.activeInHierarchy}");
        Debug.Log($"Объект активен локально: {gameObject.activeSelf}");
    }

    void Start()
    {
        Debug.Log("=== MainMenuUI: Start вызван ===");
        EnsureApiUi();

        if (PlayerPrefs.GetInt("OpenLevelSelect", 0) == 1)
        {
            PlayerPrefs.DeleteKey("OpenLevelSelect");
            OpenLevelSelect();
        }

        if (menuAnimator == null)
        {
            Debug.LogWarning("WARNING: menuAnimator НЕ назначен!");
        }
        else
        {
            Debug.Log($"menuAnimator найден: {menuAnimator.name}");

            if (!skipInitialAnimation)
            {
                Debug.Log("Устанавливаем IsGameStarted = false (начальная анимация)");
                menuAnimator.SetBool("IsGameStarted", false);
            }
            else
            {
                Debug.Log("Начальная анимация пропущена (skipInitialAnimation = true)");
            }

            Debug.Log($"Текущее значение IsGameStarted в Animator: {menuAnimator.GetBool("IsGameStarted")}");
            Debug.Log($"Количество параметров в Animator: {menuAnimator.parameterCount}");
            for (int i = 0; i < menuAnimator.parameterCount; i++)
            {
                var param = menuAnimator.parameters[i];
                Debug.Log($"  Параметр {i}: {param.name} ({param.type}) = {GetParameterValue(param)}");
            }
        }

        Debug.Log("=== MainMenuUI: Start завершён ===");
    }

    private string GetParameterValue(AnimatorControllerParameter param)
    {
        switch (param.type)
        {
            case AnimatorControllerParameterType.Bool:
                return menuAnimator.GetBool(param.name).ToString();
            case AnimatorControllerParameterType.Float:
                return menuAnimator.GetFloat(param.name).ToString();
            case AnimatorControllerParameterType.Int:
                return menuAnimator.GetInteger(param.name).ToString();
            default:
                return "N/A";
        }
    }

    /// <summary>
    /// Кнопка "Старт" — запускает игровую сцену.
    /// Привяжите к кнопке через Inspector: On Click → MainMenuUI.StartGame
    /// </summary>
    public void StartGame()
    {
        if (isGameStarted)
        {
            Debug.LogWarning("WARNING: StartGame вызван повторно, игнорируем!");
            return;
        }

        isGameStarted = true;
        Debug.Log("Нажата кнопка START — запуск анимации");

        if (menuAnimator != null)
        {
            Debug.Log($"Аниматор: установлен параметр IsGameStarted = true (задержка {animationDelay} сек)");
            Debug.Log($"Предыдущее значение IsGameStarted: {menuAnimator.GetBool("IsGameStarted")}");
            menuAnimator.SetBool("IsGameStarted", true);
            Debug.Log($"Новое значение IsGameStarted: {menuAnimator.GetBool("IsGameStarted")}");
        }
        else
        {
            Debug.LogWarning("Аниматор не назначен — сразу загружаем сцену");
        }

        StartCoroutine(LoadSceneAfterDelay(animationDelay));
    }

    private IEnumerator LoadSceneAfterDelay(float delay)
    {
        Debug.Log($"Ожидание {delay} сек перед загрузкой сцены...");
        yield return new WaitForSeconds(delay);
        Debug.Log($"Загрузка сцены: {gameSceneName}");
        SceneManager.LoadScene(gameSceneName);
    }

    /// <summary>
    /// Кнопка "Выбрать уровень" — открывает панель выбора уровней.
    /// Привяжите к кнопке через Inspector: On Click → MainMenuUI.OpenLevelSelect
    /// </summary>
    public void OpenLevelSelect()
    {
        Debug.Log("Открыть сцену выбора уровней");
        SceneManager.LoadScene("SystemUIs 1");
    }

    /// <summary>
    /// Кнопка "Чат" — пока заглушка.
    /// Привяжите к кнопке через Inspector: On Click → MainMenuUI.OpenChat
    /// </summary>
    public void OpenChat()
    {
        Debug.Log("Открыть чат");
        // TODO: реализовать открытие чата
    }

    /// <summary>
    /// Кнопка "Таблица лидеров" — пока заглушка.
    /// Привяжите к кнопке через Inspector: On Click → MainMenuUI.OpenLeaderboard
    /// </summary>
    public void OpenLeaderboard()
    {
        Debug.Log("Открыть таблицу лидеров");
        EnsureApiUi();
        apiUi?.OpenLeaderboardPanel();
    }

    /// <summary>
    /// Кнопка "Аккаунт" — пока заглушка.
    /// Привяжите к кнопке через Inspector: On Click → MainMenuUI.OpenAccount
    /// </summary>
    public void OpenAccount()
    {
        Debug.Log("Открыть аккаунт");
        EnsureApiUi();
        apiUi?.OpenAccountPanel();
    }

    /// <summary>
    /// Кнопка "Выйти" — закрывает приложение.
    /// Привяжите к кнопке через Inspector: On Click → MainMenuUI.QuitGame
    /// </summary>
    public void QuitGame()
    {
        Debug.Log("Выход из игры");
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    private void EnsureApiUi()
    {
        if (apiUi == null)
        {
            apiUi = GetComponent<MainMenuApiUI>();
        }

        if (apiUi == null)
        {
            apiUi = gameObject.AddComponent<MainMenuApiUI>();
        }
    }
}
