using UnityEngine;
using UnityEngine.SceneManagement;

// GameManager
public class LevelController : MonoBehaviour
{
    public static LevelController Instance { get; private set; }

    [Header("Scene Names")]
    [SerializeField] private string[] sceneOrder = { "FirstLevel", "SecondLevel", "ThirdLevel" };

    private int remainingBlocks = 0;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        CountBlocks();
    }

    void Update()
    {
        // Debug: Load specific level with Ctrl+1/2/3
        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                Debug.Log("Ctrl+1 pressed - Loading FirstLevel");
                SceneManager.LoadScene(sceneOrder[0]);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                Debug.Log("Ctrl+2 pressed - Loading SecondLevel");
                SceneManager.LoadScene(sceneOrder[1]);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                Debug.Log("Ctrl+3 pressed - Loading ThirdLevel");
                SceneManager.LoadScene(sceneOrder[2]);
            }
        }
    }

    void CountBlocks()
    {
        remainingBlocks = FindObjectsByType<BlockController>(FindObjectsInactive.Include, FindObjectsSortMode.None).Length;
        Debug.Log($"Blocks remaining: {remainingBlocks}");
    }

    public void BlockDestroyed()
    {
        remainingBlocks--;

        if (remainingBlocks <= 0)
        {
            LoadNextLevel();
        }
    }

    void LoadNextLevel()
    {
        // Find current scene index in sceneOrder
        string currentSceneName = SceneManager.GetActiveScene().name;
        int currentIndexInArray = -1;

        for (int i = 0; i < sceneOrder.Length; i++)
        {
            if (sceneOrder[i] == currentSceneName)
            {
                currentIndexInArray = i;
                break;
            }
        }

        // If current scene not found, start from first
        if (currentIndexInArray == -1)
        {
            currentIndexInArray = 0;
        }

        // Move to next scene
        currentIndexInArray++;

        // Loop back to first scene
        if (currentIndexInArray >= sceneOrder.Length)
        {
            currentIndexInArray = 0;
        }

        string nextScene = sceneOrder[currentIndexInArray];
        Debug.Log($"Loading next level: {nextScene}");
        SceneManager.LoadScene(nextScene);
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
