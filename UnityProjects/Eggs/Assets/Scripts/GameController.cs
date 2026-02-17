using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController Instance;

    public GameObject orangePrefab;
    public Transform[] spawnPoints;
    private int activeOranges = 0;
    public int maxOranges = 4;
    public int score = 0;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        InvokeRepeating("TrySpawnOrange", 0f, 2f);
    }

    public void OnOrangeDestroyed()
    {
        activeOranges--;
    }

    public void OnOrangeCollected()
    {
        score++;
        Debug.Log("Orange collected! Score: " + score);
    }

    void TrySpawnOrange()
    {
        if (activeOranges < maxOranges && spawnPoints.Length >= 4)
        {
            int i = Random.Range(0, 4);
            GameObject obj = Instantiate(orangePrefab, spawnPoints[i].position, Quaternion.identity) as GameObject;
            if (obj != null)
            {
                try
                {
                    obj.tag = "Orange";
                }
                catch (UnityException)
                {
                    Debug.LogWarning("GameController: Tag 'Orange' is not defined.");
                }
            }
            activeOranges++;
        }
    }
}