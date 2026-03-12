using UnityEngine;
using UnityEngine.SceneManagement;

// Menus UI
public class MenuController : MonoBehaviour
{
    public static MenuController Instance { get; private set; }

    [Header("UI Panels")]
    [SerializeField] private GameObject mainMenuPanel;
    // TODO: Панелька паузы
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject gameplayUI;

    [Header("Animations")]
    [SerializeField] private Animator mainMenuAnimator;
    [SerializeField] private string boolParameterName = "Open";
    [SerializeField] private float hideAnimationDuration = 0.3f;

    [Header("Game Objects")]
    [SerializeField] private BlockController[] allBlocks;

    private bool isGamePaused;
    private bool gameStarted;
    private bool isMenuHiding;

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
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ShowMainMenu()
    {
        gameStarted = false;
        isMenuHiding = false;
        Time.timeScale = 1f;

        mainMenuAnimator?.SetBool(boolParameterName, true);

        mainMenuPanel?.SetActive(true);
        gameplayUI?.SetActive(false);
        pausePanel?.SetActive(false);

        HideAllBlocks();
    }

    public void PauseGame()
    {
        if (gameplayUI is { activeSelf: true })
        {
            pausePanel?.SetActive(true);
            Time.timeScale = 0f;
            isGamePaused = true;
        }
    }

    public void ResumeGame()
    {
        pausePanel?.SetActive(false);
        Time.timeScale = 1f;
        isGamePaused = false;
    }

}
