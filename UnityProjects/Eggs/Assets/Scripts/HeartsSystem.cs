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
    private List<GameObject> spawnedHearts = new List<GameObject>();
    private bool isGameOver = false;

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
        // Удалить старые сердца
        foreach (GameObject heart in spawnedHearts)
        {
            Destroy(heart);
        }
        spawnedHearts.Clear();

        // Создать новые сердца
        SpawnHearts();
    }

    public void RemoveHeart(GameObject heart)
    {
        spawnedHearts.Remove(heart);
        heartCount = spawnedHearts.Count;

        Debug.Log("Сердце: " + heartCount);

        // Проверить, остались ли сердца
        if (heartCount < 1 && !isGameOver)
        {
            GameOver();
        }
    }

    private void GameOver()
    {
        Debug.Log("Поражение");

        isGameOver = true;
        Time.timeScale = 0f; // Остановить игру

        // Показать UI с сообщением о поражении
        if (gameOverPrefab != null)
        {
            Debug.Log("Условие поражения");

            GameObject gameOverUI = Instantiate(gameOverPrefab);
            ButtonListener buttonListener = gameOverUI.GetComponent<ButtonListener>();
            if (buttonListener != null)
            {
                buttonListener.SetOnClickAction(RestartGame);
            }
        }
    }

    private void RestartGame()
    {
        Time.timeScale = 1f; // Возобновить время
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void Update()
    {
        // Перезапуск при нажатии любой кнопки кроме Enter (если игра закончилась)
        if (isGameOver && Input.anyKeyDown && !Input.GetKeyDown(KeyCode.Return))
        {
            RestartGame();
        }
    }
}
