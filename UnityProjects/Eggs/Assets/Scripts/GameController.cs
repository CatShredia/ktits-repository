using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController Instance;

    public GameObject orangePrefab;
    public Transform[] spawnPoints;
    private int activeOranges = 0;
    public int maxOranges = 4;

    // Separate scores
    public int genaScore = 0;
    public int cheburashkaScore = 0;

    public enum Collector { Gena, Cheburashka }

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

    // New unified method with collector type
    public void OnOrangeCollected(Collector collector)
    {
        switch (collector)
        {
            case Collector.Gena:
                genaScore++;
                Debug.Log($"Gena caught an orange! Gena Score: {genaScore}");
                break;
            case Collector.Cheburashka:
                cheburashkaScore++;
                Debug.Log($"Cheburashka caught an orange! Cheburashka Score: {cheburashkaScore}");
                break;
        }
    }

    void TrySpawnOrange()
    {
        if (activeOranges < maxOranges && spawnPoints.Length >= 4)
        {
            int i = Random.Range(0, spawnPoints.Length); // Fixed index range
            GameObject obj = Instantiate(orangePrefab, spawnPoints[i].position, Quaternion.identity);

            if (obj != null)
            {
                try { obj.tag = "Orange"; }
                catch (UnityException) { Debug.LogWarning("Tag 'Orange' not defined."); }
            }
            activeOranges++;
        }
    }
}