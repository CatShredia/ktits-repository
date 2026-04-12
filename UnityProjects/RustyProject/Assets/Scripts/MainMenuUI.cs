using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Главное меню: Старт, Чат, Таблица лидеров, Аккаунт, Выйти.
/// Вешается на объект MainMenu на сцене меню.
/// Привяжите методы к кнопкам через Unity Inspector (On Click).
/// </summary>
public class MainMenuUI : MonoBehaviour
{
    [Header("Scene Settings")]
    [Tooltip("Название игровой сцены для кнопки 'Старт'")]
    public string gameSceneName = "SampleScene";

    /// <summary>
    /// Кнопка "Старт" — запускает игровую сцену.
    /// Привяжите к кнопке через Inspector: On Click → MainMenuUI.StartGame
    /// </summary>
    public void StartGame()
    {
        SceneManager.LoadScene(gameSceneName);
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
        // TODO: реализовать таблицу лидеров
    }

    /// <summary>
    /// Кнопка "Аккаунт" — пока заглушка.
    /// Привяжите к кнопке через Inspector: On Click → MainMenuUI.OpenAccount
    /// </summary>
    public void OpenAccount()
    {
        Debug.Log("Открыть аккаунт");
        // TODO: реализовать страницу аккаунта
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
}
