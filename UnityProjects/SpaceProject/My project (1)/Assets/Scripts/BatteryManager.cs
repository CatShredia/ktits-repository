using UnityEngine;

public class BatteryManager : MonoBehaviour
{

    [SerializeField] private GameManager gameManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Battery collected!");
        Destroy(gameObject);
        gameManager.BatteryCollected();
    }
}
