using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject gameOverPanel;
    public TextMeshProUGUI scoreText;

    [Header("Buttons")]
    public Button restartButton;
    public Button mainMenuButton;

    [Header("References")]
    [Tooltip("Ссылка на объект игрока")]
    public GameObject playerObject;

    [Tooltip("Скрипт движения игрока (будет отключен при Game Over)")]
    public PlayerMovement playerMovementScript;

    [Header("Animation")]
    public float fadeInTime = 0.5f;

    private CanvasGroup canvasGroup;
    private bool isGameOver = false;

    void Start()
    {
        if (gameOverPanel == null)
            gameOverPanel = transform.Find("GameOverPanel")?.gameObject;

        if (scoreText == null)
            scoreText = transform.Find("ScoreText")?.GetComponent<TextMeshProUGUI>();

        if (restartButton == null)
            restartButton = transform.Find("RestartButton")?.GetComponent<Button>();

        if (mainMenuButton == null)
            mainMenuButton = transform.Find("MainMenuButton")?.GetComponent<Button>();

        // Авто-поиск игрока, если не назначен
        if (playerObject == null)
        {
            playerObject = GameObject.FindGameObjectWithTag("Player");
        }

        if (playerMovementScript == null && playerObject != null)
        {
            playerMovementScript = playerObject.GetComponent<PlayerMovement>();
        }

        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        HideMenu();

        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameOver += ShowMenu;
        }

        SetupButtons();
    }

    void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameOver -= ShowMenu;
        }
    }

    private void SetupButtons()
    {
        if (restartButton != null)
            restartButton.onClick.AddListener(RestartLevel);

        if (mainMenuButton != null)
            mainMenuButton.onClick.AddListener(GoToMainMenu);
    }

    private void ShowMenu()
    {
        if (isGameOver) return;
        isGameOver = true;

        Debug.Log("GameOverUI: Show Menu");

        // 1. ОТКЛЮЧАЕМ ДВИЖЕНИЕ ИГРОКА
        if (playerMovementScript != null)
        {
            playerMovementScript.enabled = false;
        }
        // Также можно обнулить скорость, чтобы игрок не скользил
        if (playerObject != null)
        {
            Rigidbody2D rb = playerObject.GetComponent<Rigidbody2D>();
            if (rb != null) rb.linearVelocity = Vector2.zero;
        }

        if (scoreText != null && GameManager.Instance != null)
        {
            scoreText.text = $"Score: {GameManager.Instance.Score}";
        }

        StartCoroutine(FadeIn());
    }

    private System.Collections.IEnumerator FadeIn()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        canvasGroup.alpha = 0;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

        float elapsedTime = 0f;
        while (elapsedTime < fadeInTime)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0, 1, elapsedTime / fadeInTime);
            yield return null;
        }
        canvasGroup.alpha = 1;
    }

    private void HideMenu()
    {
        isGameOver = false;
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        // При скрытии меню (например, при рестарте) включаем движение обратно
        if (playerMovementScript != null)
        {
            playerMovementScript.enabled = true;
        }
    }

    private void RestartLevel()
    {
        Debug.Log("GameOverUI: Restart");

        if (GameManager.Instance != null)
        {
            GameManager.Instance.ResetScore();
            GameManager.Instance.ResetLives();
        }

        // Сцена перезагрузится, и скрипт движения включится автоматически в Start() нового объекта
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void GoToMainMenu()
    {
        Debug.Log("GameOverUI: Main Menu");

        if (GameManager.Instance != null)
        {
            GameManager.Instance.ResetScore();
            GameManager.Instance.ResetLives();
        }

        SceneManager.LoadScene("SystemUIs");
    }

    public void TestGameOver()
    {
        ShowMenu();
    }
}