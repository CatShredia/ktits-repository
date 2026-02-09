using UnityEngine;

public class PlayerFoodEating : MonoBehaviour
{
    private FoodSpawner spawner;

    public void SetSpawner(FoodSpawner s) => spawner = s;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            ScoreManager.Instance?.AddScore(); 
            spawner?.SpawnFood();
        }
    }
}
