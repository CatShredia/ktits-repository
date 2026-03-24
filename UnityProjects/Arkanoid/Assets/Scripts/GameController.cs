using UnityEngine;
using System.Collections.Generic;

// GameManager
public class GameController : MonoBehaviour
{
    [SerializeField] private int heartCount = 3;
    [SerializeField] private bool debugMode = false;

    public static GameController Instance { get; private set; }

    private GameObject heartContainer;
    private readonly List<BallController> activeBalls = new();
    private bool gameHasStarted = false;

    public GameObject heartPrefab;
    public GameObject ballPrefab;
    public GameObject ballClonePrefab;
    public Transform playerTransform;

    void Awake() => Instance = this;

    void Start()
    {
        if (heartCount < 1)
        {
            heartCount = 3;
            Debug.LogWarning("[GameController] heartCount was less than 1, reset to 3");
        }

        if (debugMode) Debug.Log($"[GameController] Starting with {heartCount} hearts");

        heartContainer = new GameObject("HeartContainer");
        heartContainer.transform.position = new Vector3(0, 4, 0);

        if (playerTransform == null)
        {
            var platform = GameObject.FindWithTag("Player");
            if (platform != null)
                playerTransform = platform.transform;
        }

        SoundManager.Instance?.PlayStartGame();
        SpawnHeartUI();

        gameHasStarted = true;
    }

    public void RegisterBall(BallController ball)
    {
        if (!activeBalls.Contains(ball))
        {
            activeBalls.Add(ball);
        }
    }

    public void UnregisterBall(BallController ball)
    {
        activeBalls.Remove(ball);
    }

    public void DescreaseHeart()
    {
        if (debugMode) Debug.Log($"[GameController] Decreasing heart. Current: {heartCount}");

        foreach (var ball in activeBalls)
        {
            if (ball != null)
                ball.isActiveBalls = false;
        }

        heartCount--;
        ClearHearts();
        SpawnHeartUI();

        if (heartCount < 1 && gameHasStarted)
        {
            if (debugMode) Debug.Log("[GameController] Gameover triggered!");
            MenuController.Instance?.ShowGameover();
        }
    }

    public void IncreaseHearts()
    {
        heartCount++;
        SoundManager.Instance?.PlayHeartCollected();
        ClearHearts();
        SpawnHeartUI();
    }

    public void DestroyAllBalls()
    {
        foreach (var ball in activeBalls)
        {
            if (ball != null && ball.isClone)
            {
                Destroy(ball.gameObject);
            }
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
        {
            Destroy(child.gameObject);
        }
    }

    public void SpawnHeartUI()
    {
        for (int i = 0; i < heartCount; i++)
        {
            var heartXYZ = new Vector3(
                heartContainer.transform.position.x + i * 0.5f,
                heartContainer.transform.position.y,
                heartContainer.transform.position.z);
            Instantiate(heartPrefab, heartXYZ, Quaternion.identity, heartContainer.transform);
        }
    }
}
