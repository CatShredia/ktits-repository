using UnityEngine;
using System.Collections.Generic;

// Empty GameObject in scene
[DefaultExecutionOrder(-1)]
public class GameController : MonoBehaviour
{
    [SerializeField] private int heartCount = 2;

    public static GameController Instance { get; private set; }

    private GameObject heartContainer;
    private readonly List<BallController> activeBalls = new List<BallController>();

    public GameObject heartPrefab;
    public GameObject ballPrefab;
    public GameObject ballClonePrefab;
    public Transform playerTransform;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        heartContainer = new GameObject("HeartContainer");
        heartContainer.transform.position = new Vector3(-8, 3, 0);

        if (playerTransform == null)
        {
            var platform = GameObject.FindWithTag("Player");
            if (platform != null)
                playerTransform = platform.transform;
        }

        SpawnHeartUI();
    }

    public void RegisterBall(BallController ball)
    {
        if (!activeBalls.Contains(ball))
            activeBalls.Add(ball);
    }

    public void UnregisterBall(BallController ball)
    {
        activeBalls.Remove(ball);
    }

    public void DescreaseHeart()
    {
        foreach (var ball in activeBalls)
        {
            if (ball != null)
                ball.isActiveBalls = false;
        }

        heartCount--;
        ClearHearts();
        SpawnHeartUI();

        if (heartCount <= 0)
        {
            // Game Over
        }
    }

    public void IncreaseHearts()
    {
        heartCount++;
        ClearHearts();
        SpawnHeartUI();
    }

    public void DestroyAllBalls()
    {
        foreach (var ball in activeBalls)
        {
            if (ball != null && ball.isClone)
                Destroy(ball.gameObject);
        }
        activeBalls.RemoveAll(b => b == null || b.isClone);
    }

    public void SpawnExtraBall(Vector3 position)
    {
        if (ballClonePrefab == null) return;

        var newBall = Instantiate(ballClonePrefab, position, Quaternion.identity);
        var ballController = newBall.GetComponent<BallController>();
        if (ballController != null)
        {
            ballController.isActiveBalls = true;
            ballController.isClone = true;
            ballController.LaunchBall();
        }
    }

    void ClearHearts()
    {
        foreach (Transform child in heartContainer.transform)
            Destroy(child.gameObject);
    }

    public void SpawnHeartUI()
    {
        if (heartContainer == null) return;

        for (int i = 0; i < heartCount; i++)
        {
            var heartXYZ = new Vector3(
                heartContainer.transform.position.x + i * 1.5f,
                heartContainer.transform.position.y,
                heartContainer.transform.position.z);
            Instantiate(heartPrefab, heartXYZ, Quaternion.identity, heartContainer.transform);
        }
    }
}
