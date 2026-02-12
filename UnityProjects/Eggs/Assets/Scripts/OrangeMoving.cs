using UnityEngine;

public class OrangeMoving : MonoBehaviour
{

    public float torque = 1000f;

    [SerializeField] private GameObject prefabToSpawn;
    [SerializeField] private Transform[] spawnPoints;

    public void Spawn()
    {
        if (spawnPoints.Length < 4)
        {
            Debug.LogWarning("Нужно указать ровно 4 точки спавна!");
            return;
        }

        int randomIndex = Random.Range(0, 4);
        Instantiate(prefabToSpawn, spawnPoints[randomIndex].position, Quaternion.identity);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GetComponent<Rigidbody2D>().AddTorque(-torque);
        Spawn();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
