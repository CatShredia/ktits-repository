using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class HeartsSystem : MonoBehaviour
{
    public static HeartsSystem Instance { get; private set; }

    [SerializeField] private GameObject heartPrefab;
    [SerializeField] public int heartCount = 3;
    [SerializeField] private Vector3 firstHeartPosition = Vector3.zero;
    [SerializeField] private float heartSpacing = 1f;
    [SerializeField] private GameObject gameOverPrefab;
    [SerializeField] private GameObject madnessEffectPrefab;
    [SerializeField] private GameObject pausePrefab;
    private List<GameObject> spawnedHearts = new List<GameObject>();
    private bool isGameOver = false;
    private bool isPaused = false;
    private GameObject gameOverUI;
    private GameObject madnessEffectUI;
    private GameObject pauseUI;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    void Start()
    {
        SpawnHearts();

        if (gameOverPrefab != null)
        {
            gameOverUI = Instantiate(gameOverPrefab);
            gameOverUI.SetActive(false);
            Debug.Log("GameOver UI создана и скрыта");
        }
        else
        {
            Debug.LogWarning("gameOverPrefab не назначен!");
        }

        if (madnessEffectPrefab != null)
        {
            madnessEffectUI = Instantiate(madnessEffectPrefab);
            madnessEffectUI.SetActive(false);
            Debug.Log("Madness effect создан и скрыт");
        }
        else
        {
            Debug.LogWarning("madnessEffectPrefab не назначен!");
        }

        if (pausePrefab != null)
        {
            pauseUI = Instantiate(pausePrefab);
            pauseUI.SetActive(false);
            Debug.Log("Pause UI создана и скрыта");
        }
        else
        {
            Debug.LogWarning("pausePrefab не назначен!");
        }

        UpdateMadnessEffect();
    }

    void SpawnHearts()
    {
        for (int i = 0; i < heartCount; i++)
        {
            Vector3 heartPosition = firstHeartPosition + Vector3.right * (i * heartSpacing);
            GameObject heart = Instantiate(heartPrefab, heartPosition, Quaternion.identity);
            spawnedHearts.Add(heart);
        }
    }

    public void RedrawHearts()
    {
        foreach (GameObject heart in spawnedHearts)
        {
            Heart heartComponent = heart.GetComponent<Heart>();
            if (heartComponent != null)
            {
                heartComponent.DisableHeartListener();
            }
        }

        foreach (GameObject heart in spawnedHearts)
        {
            Destroy(heart);
        }
        spawnedHearts.Clear();

        Debug.Log("RedrawHearts");

        SpawnHearts();

        UpdateMadnessEffect();
    }

    public void LoseHeart()
    {
        if (spawnedHearts.Count > 0)
        {
            GameObject heart = spawnedHearts[spawnedHearts.Count - 1];
            RemoveHeart(heart);
        }
    }

    public void RemoveHeart(GameObject heart)
    {
        Debug.Log("Попытка удалить сердце. Всего в списке: " + spawnedHearts.Count);

        bool removed = spawnedHearts.Remove(heart);

        if (!removed)
        {
            Debug.LogWarning("Сердце не найдено в списке! Возможно, передан неверный GameObject.");
            return;
        }

        heartCount = spawnedHearts.Count;
        Debug.Log("Сердец осталось: " + heartCount);

        Heart heartComponent = heart.GetComponent<Heart>();
        if (heartComponent != null)
        {
            heartComponent.DisableHeartListener();
        }

        if (heartCount <= 0 && !isGameOver)
        {
            GameOver();
        }

        Destroy(heart);

        UpdateMadnessEffect();
    }

    private void UpdateMadnessEffect()
    {
        if (madnessEffectUI != null)
        {
            madnessEffectUI.SetActive(heartCount < 2);
        }
    }

    private void GameOver()
    {
        Debug.Log("Поражение вызвано");

        isGameOver = true;
        Time.timeScale = 0f;

        if (gameOverUI != null)
        {
            Debug.Log("gameOverUI найдена, показываю панель");
            gameOverUI.SetActive(true);

            ButtonListener buttonListener = gameOverUI.GetComponent<ButtonListener>();
            if (buttonListener != null)
            {
                Debug.Log("ButtonListener найден, устанавливаю действие");
                buttonListener.SetOnClickAction(RestartGame);
            }
            else
            {
                Debug.LogWarning("ButtonListener не найден на gameOverUI!");
            }
        }
        else
        {
            Debug.LogWarning("gameOverUI = null! Панель не была создана.");
        }
    }

    private void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void TogglePause()
    {
        if (isGameOver) return;

        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0f : 1f;

        if (pauseUI != null)
        {
            pauseUI.SetActive(isPaused);
            Debug.Log(isPaused ? "Игра на паузе" : "Игра продолжена");
        }
    }

    public void Resume()
    {
        if (isPaused)
        {
            isPaused = false;
            Time.timeScale = 1f;
            if (pauseUI != null)
            {
                pauseUI.SetActive(false);
                Debug.Log("Игра продолжена");
            }
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !isGameOver)
        {
            TogglePause();
        }

        if (isGameOver && Input.anyKeyDown && !Input.GetKeyDown(KeyCode.Return))
        {
            RestartGame();
        }
    }
}
