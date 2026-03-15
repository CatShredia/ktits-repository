using UnityEngine;
using UnityEngine.SceneManagement;

// Menus UI
public class MenuController : MonoBehaviour
{
    public static MenuController Instance { get; private set; }

    [Header("UI Panels")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject gameoverPanel;
    [SerializeField] private GameObject gameplayUI;

    [Header("Animations")]
    [SerializeField] private Animator mainMenuAnimator;
    [SerializeField] private Animator gameoverAnimator;
    [SerializeField] private string boolParameterName = "Open";
    [SerializeField] private string showTriggerParameterName = "Show";
    [SerializeField] private string hideTriggerParameterName = "Hidden";
    [SerializeField] private float hideAnimationDuration = 0.3f;

    [Header("Game Objects")]
    [SerializeField] private BlockController[] allBlocks;

    private bool isGamePaused;
    private bool gameStarted;
    private bool isMenuHiding;
    private bool isGameoverShowing;

    public bool IsGameStarted => gameStarted;

    void Awake()
    {
        Instance = this;

        // TODO: Криво считает
        allBlocks = FindObjectsOfType<BlockController>(true);
        HideAllBlocks();
    }

    void Start() => ShowMainMenu();

    void HideAllBlocks()
    {
        if (allBlocks == null) return;

        foreach (var block in allBlocks)
        {
            if (block != null)
                block.gameObject.SetActive(false);
        }
    }

    void ShowAllBlocks()
    {
        if (allBlocks == null) return;

        foreach (var block in allBlocks)
        {
            if (block != null)
                block.gameObject.SetActive(true);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!gameStarted) return;
            if (isGameoverShowing) return;
            if (isGamePaused) ResumeGame();
            else PauseGame();
        }
    }

    public void StartGame()
    {
        if (gameStarted || isMenuHiding) return;

        gameStarted = true;
        isMenuHiding = true;

        mainMenuAnimator?.SetBool(boolParameterName, false);

        gameplayUI?.SetActive(true);
        Time.timeScale = 1f;

        ShowAllBlocks();

        StartCoroutine(HidePanelAfterAnimation());
    }

    private System.Collections.IEnumerator HidePanelAfterAnimation()
    {
        yield return new WaitForSecondsRealtime(hideAnimationDuration);
        mainMenuPanel?.SetActive(false);
        isMenuHiding = false;
    }

    public void ExitGame() => Application.Quit();

    public void RetryGame()
    {
        Time.timeScale = 1f;
        HideGameover();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ShowMainMenu()
    {
        gameStarted = false;
        isMenuHiding = false;
        isGameoverShowing = false;
        Time.timeScale = 1f;

        mainMenuAnimator?.SetBool(boolParameterName, true);

        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
        if (gameplayUI != null) gameplayUI.SetActive(false);
        if (pausePanel != null) pausePanel.SetActive(false);
        if (gameoverPanel != null) gameoverPanel.SetActive(false);

        HideAllBlocks();
    }

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

    public void ShowGameover()
    {
        if (isGameoverShowing) return;

        isGameoverShowing = true;
        gameStarted = false;

        // Set timescale AFTER showing the panel
        if (gameoverPanel != null)
        {
            gameoverPanel.SetActive(true);

            // Get or use Animator from gameoverPanel
            if (gameoverAnimator == null)
            {
                gameoverAnimator = gameoverPanel.GetComponent<Animator>();
            }

            // Trigger gameover animation BEFORE freezing time
            if (gameoverAnimator != null)
            {
                // Set animator to use unscaled time
                gameoverAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;

                // Use Trigger instead of Bool - trigger fires once and doesn't loop
                gameoverAnimator.SetTrigger(showTriggerParameterName);
            }
        }
        else
        {
            Debug.LogError("[MenuController] GameoverPanel not assigned!");
        }

        // Now freeze time
        Time.timeScale = 0f;

        if (gameplayUI != null) gameplayUI.SetActive(false);
        if (pausePanel != null) pausePanel.SetActive(false);

        HideAllBlocks();
    }

    public void HideGameover()
    {
        if (!isGameoverShowing) return;

        isGameoverShowing = false;
        Time.timeScale = 1f;

        if (gameoverAnimator != null)
        {
            gameoverAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;
            gameoverAnimator.SetTrigger(hideTriggerParameterName);

            // Hide panel after animation
            StartCoroutine(HideGameoverAfterAnimation());
        }
        else
        {
            // No animator - hide immediately
            if (gameoverPanel != null) gameoverPanel.SetActive(false);
        }
    }

    private System.Collections.IEnumerator HideGameoverAfterAnimation()
    {
        yield return new WaitForSecondsRealtime(hideAnimationDuration);
        if (gameoverPanel != null) gameoverPanel.SetActive(false);
    }
}
