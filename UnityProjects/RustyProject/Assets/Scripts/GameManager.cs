using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Score")]
    public int Score;

    public event System.Action<int> OnScoreChanged;

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
        OnScoreChanged?.Invoke(Score);
    }

    public void ResetScore()
    {
        Score = 0;
        OnScoreChanged?.Invoke(Score);
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
