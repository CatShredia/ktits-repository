using Unity.VisualScripting;
using UnityEngine;

public class GameController : MonoBehaviour
{

    [SerializeField] private int heartCount = 2;

    public static GameController Instance { get; private set; }

    private GameObject heartContainer;


    void Awake()
    {
        Instance = this;
    }

    public GameObject heartPrefab;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        heartContainer = new GameObject("HeartContainer");
        heartContainer.transform.position = new Vector3(-8, 3, 0);

        SpawnHeartUI();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void DescreaseHeart()
    {
        BallController.Instance.isActiveBalls = false;
        heartCount--;
        Debug.Log("Heart count: " + heartCount);
        ClearHearts();
        SpawnHeartUI();
        if (heartCount <= 0)
        {
            Debug.Log("Game Over");
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
