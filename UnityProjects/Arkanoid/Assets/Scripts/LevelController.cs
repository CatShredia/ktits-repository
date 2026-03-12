using UnityEngine;
using UnityEngine.SceneManagement;

// GameManager
public class LevelController : MonoBehaviour
{
    public static LevelController Instance { get; private set; }

    [Header("Scene Names")]
    [SerializeField] private string[] sceneOrder = { "FirstLevel", "SecondLevel", "ThirdLevel" };

    private int remainingBlocks;

    void Awake() { Instance = this; }

    void Start() { CountBlocks(); }

    void Update()
    {
        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                SceneManager.LoadScene(sceneOrder[0]);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                SceneManager.LoadScene(sceneOrder[1]);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                SceneManager.LoadScene(sceneOrder[2]);
            }
        }
    }

    void CountBlocks()
    {
        remainingBlocks = FindObjectsByType<BlockController>(FindObjectsInactive.Include, FindObjectsSortMode.None).Length;
    }

    public void BlockDestroyed()
    {
        remainingBlocks--;

        if (remainingBlocks <= 0)
            LoadNextLevel();
    }

    void LoadNextLevel()
    {
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

        if (currentIndexInArray == -1)
            currentIndexInArray = 0;

        currentIndexInArray++;

        if (currentIndexInArray >= sceneOrder.Length)
            currentIndexInArray = 0;

        SceneManager.LoadScene(sceneOrder[currentIndexInArray]);
    }

    public void RestartLevel() { SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); }
}
