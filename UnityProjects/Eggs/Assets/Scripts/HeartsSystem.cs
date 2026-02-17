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
    [SerializeField] private GameObject madnessEffectPrefab; // Красный фильтр безумия
    private List<GameObject> spawnedHearts = new List<GameObject>();
    private bool isGameOver = false;
    private GameObject gameOverUI;
    private GameObject madnessEffectUI;

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

        // Создать панель GameOver и скрыть её изначально
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

        // Создать эффект безумия и скрыть его изначально
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
        // Отключить слушателей перед уничтожением
        foreach (GameObject heart in spawnedHearts)
        {
            Heart heartComponent = heart.GetComponent<Heart>();
            if (heartComponent != null)
            {
                heartComponent.DisableHeartListener();
            }
        }

        // Удалить старые сердца
        foreach (GameObject heart in spawnedHearts)
        {
            Destroy(heart);
        }
        spawnedHearts.Clear();

        Debug.Log("RedrawHearts");

        // Создать новые сердца
        SpawnHearts();

        // Обновить эффект безумия
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

        // Вариант 1: Удаление по ссылке (проверьте, тот ли это объект)
        bool removed = spawnedHearts.Remove(heart);

        if (!removed)
        {
            Debug.LogWarning("Сердце не найдено в списке! Возможно, передан неверный GameObject.");
            // Попытка найти и удалить уничтоженный объект или использовать другой подход
            return;
        }

        // Обновляем счетчик
        heartCount = spawnedHearts.Count;
        Debug.Log("Сердец осталось: " + heartCount);

        // Отключить слушатель перед уничтожением
        Heart heartComponent = heart.GetComponent<Heart>();
        if (heartComponent != null)
        {
            heartComponent.DisableHeartListener();
        }

        // Проверка поражения до уничтожения объекта
        if (heartCount <= 0 && !isGameOver)
        {
            GameOver();
        }

        // Уничтожаем визуальный объект
        Destroy(heart);

        // Обновить эффект безумия
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
        Time.timeScale = 0f; // Остановить игру

        // Показать UI с сообщением о поражении
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
