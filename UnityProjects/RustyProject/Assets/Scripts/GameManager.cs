using UnityEngine;

/// <summary>
/// GameManager — управление очками и состоянием игры.
/// Для управления уровнями см. LevelManager и LevelTrigger.
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Score")]
    public int Score;

    [Header("Lives")]
    [SerializeField] private int maxLives = 3;
    public int Lives = 3;

    public event System.Action<int> OnScoreChanged;
    public event System.Action<int> OnLivesChanged;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddScore(int amount)
    {
        Score += amount;
        Debug.Log($"GameManager: Добавлено {amount} очков. Текущий счёт: {Score}");
        OnScoreChanged?.Invoke(Score);
    }

    public void ResetScore()
    {
        Debug.Log("GameManager: Счёт сброшен");
        Score = 0;
        OnScoreChanged?.Invoke(Score);
    }

    public void AddLife(int amount = 1)
    {
        int oldLives = Lives;
        Lives = Mathf.Min(Lives + amount, maxLives);
        Debug.Log($"GameManager: Добавлено {amount} жизнь(и). Жизни: {oldLives} → {Lives}");
        OnLivesChanged?.Invoke(Lives);
    }

    public void RemoveLife(int amount = 1)
    {
        int oldLives = Lives;
        Lives = Mathf.Max(Lives - amount, 0);
        Debug.Log($"GameManager: Снято {amount} жизнь(и). Жизни: {oldLives} → {Lives}");
        OnLivesChanged?.Invoke(Lives);

        if (Lives <= 0)
        {
            Debug.Log("GameManager: Game Over! Все жизни потеряны.");
            OnGameOver?.Invoke();
        }
    }

    public event System.Action OnGameOver;

    public void ResetLives()
    {
        Debug.Log($"GameManager: Жизни сброшены. {Lives} → {maxLives}");
        Lives = maxLives;
        OnLivesChanged?.Invoke(Lives);
    }

    public void SetMaxLives(int max)
    {
        int oldMax = maxLives;
        maxLives = Mathf.Max(1, max);
        Lives = Mathf.Min(Lives, maxLives);
        Debug.Log($"GameManager: Максимальное количество жизней изменено: {oldMax} → {maxLives}");
        OnLivesChanged?.Invoke(Lives);
    }

    public void MakeIndestructible(GameObject obj)
    {
        if (obj == null) return;
        DontDestroyOnLoad(obj);
        obj.AddComponent<IndestructibleComponent>();
    }

    public void MakeIndestructible(Component component)
    {
        if (component == null) return;
        DontDestroyOnLoad(component.gameObject);
        component.gameObject.AddComponent<IndestructibleComponent>();
    }
}

public class IndestructibleComponent : MonoBehaviour
{
    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }
}
