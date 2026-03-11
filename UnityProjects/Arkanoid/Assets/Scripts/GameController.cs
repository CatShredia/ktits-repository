using Unity.VisualScripting;
using UnityEngine;
using System.Collections.Generic;

[DefaultExecutionOrder(-1)]
public class GameController : MonoBehaviour
{

    [SerializeField] private int heartCount = 2;

    public static GameController Instance { get; private set; }

    private GameObject heartContainer;
    private List<BallController> activeBalls = new List<BallController>();

    void Awake()
    {
        Instance = this;
    }

    public GameObject heartPrefab;
    public GameObject ballPrefab;
    public Transform playerTransform;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
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

    // Update is called once per frame
    void Update()
    {

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
        foreach (var ball in activeBalls)
        {
            if (ball != null)
                ball.isActiveBalls = false;
        }
        heartCount--;
        Debug.Log("Heart count: " + heartCount);
        ClearHearts();
        SpawnHeartUI();
        if (heartCount <= 0)
        {
            Debug.Log("Game Over");
        }
    }

    public void IncreaseHearts()
    {
        heartCount++;
        Debug.Log("Heart count: " + heartCount);
        ClearHearts();
        SpawnHeartUI();
    }

    public void SpawnExtraBall(Vector3 position)
    {
        if (ballPrefab == null)
        {
            Debug.LogError("BallPrefab not set!");
            return;
        }

        var newBall = Instantiate(ballPrefab, position, Quaternion.identity);
        var ballController = newBall.GetComponent<BallController>();
        if (ballController != null)
        {
            ballController.isActiveBalls = true;
            ballController.playerObject = GameObject.FindWithTag("Player"); ;
            ballController.LaunchBall();
        }
    }

    private void ClearHearts()
    {
        foreach (Transform child in heartContainer.transform)
        {
            Destroy(child.gameObject);
        }
    }

    public void SpawnHeartUI()
    {
        if (heartContainer == null)
        {
            Debug.LogError("Canvas not found!");
            return;
        }

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
