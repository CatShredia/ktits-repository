using UnityEngine;
using UnityEngine.SceneManagement;

// Attach to: SystemCanvas or GameManager GameObject
// Required: None
// UI Hierarchy: SystemCanvas → [MainMenuPanel, PausePanel, GameOverPanel, GameplayUI]
public class MenuController : MonoBehaviour
{
    public static MenuController Instance { get; private set; }

    [Header("UI Panels (auto-find if not assigned)")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject gameplayUI;

    [Header("Animations")]
    [SerializeField] private Animator mainMenuAnimator;
    [SerializeField] private string boolParameterName = "Open";
    [SerializeField] private float hideAnimationDuration = 0.3f;

    private bool isGamePaused = false;
    private bool gameStarted = false;
    private bool isMenuHiding = false;

    public bool IsGameStarted => gameStarted;

    void Awake()
    {
        Instance = this;
        
        // Auto-find panels by name if not assigned
        if (mainMenuPanel == null)
            mainMenuPanel = GameObject.Find("MainMenuPanel");
        if (pausePanel == null)
            pausePanel = GameObject.Find("PausePanel");
        if (gameOverPanel == null)
            gameOverPanel = GameObject.Find("GameOverPanel");
        if (gameplayUI == null)
            gameplayUI = GameObject.Find("GameplayUI");

        // Auto-find animator if not assigned
        if (mainMenuAnimator == null && mainMenuPanel != null)
            mainMenuAnimator = mainMenuPanel.GetComponent<Animator>();
    }

    void Start()
    {
        ShowMainMenu();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!gameStarted) return;
            if (isGamePaused) ResumeGame();
            else PauseGame();
        }
    }

    #region Menu Buttons

    public void StartGame()
    {
        if (gameStarted || isMenuHiding) return;
        
        gameStarted = true;
        isMenuHiding = true;
        
        if (mainMenuAnimator != null)
        {
            mainMenuAnimator.SetBool(boolParameterName, false);
        }
        
        if (gameplayUI != null) gameplayUI.SetActive(true);
        Time.timeScale = 1f;
        
        StartCoroutine(HidePanelAfterAnimation());
    }

    private System.Collections.IEnumerator HidePanelAfterAnimation()
    {
        yield return new WaitForSecondsRealtime(hideAnimationDuration);
        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        isMenuHiding = false;
    }

    public void ExitGame() => Application.Quit();

    public void RetryGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ShowMainMenu()
    {
        gameStarted = false;
        isMenuHiding = false;
        Time.timeScale = 1f;
        
        // Reset animator to open state
        if (mainMenuAnimator != null)
        {
            mainMenuAnimator.SetBool(boolParameterName, true);
        }
        
        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
        if (gameplayUI != null) gameplayUI.SetActive(false);
        if (pausePanel != null) pausePanel.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
    }

    #endregion

    #region Pause Menu

    public void PauseGame()
    {
        if (gameplayUI != null && gameplayUI.activeSelf)
        {
            if (pausePanel != null) pausePanel.SetActive(true);
            Time.timeScale = 0f;
            isGamePaused = true;
        }
    }

    public void ResumeGame()
    {
        if (pausePanel != null) pausePanel.SetActive(false);
        Time.timeScale = 1f;
        isGamePaused = false;
    }

    #endregion

    #region Game Over

    public void ShowGameOver()
    {
        if (gameOverPanel != null) gameOverPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    #endregion
}