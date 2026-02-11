using UnityEngine;

public class Basket : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Orange"))
        {
            Debug.Log("Попал!");
            // Удалить апельсин или вызвать событие победы
            Destroy(col.gameObject);
            // Например: GameManager.OnScore();
        }
    }
}
