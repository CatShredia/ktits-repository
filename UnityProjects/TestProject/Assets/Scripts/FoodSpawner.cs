using UnityEngine;

public class FoodSpawner : MonoBehaviour
{
    public GameObject foodPrefab;
    private GameObject currentFood;

    private void Start()
    {
        SpawnFood();
    }

    public void SpawnFood()
    {
        if (currentFood != null) Destroy(currentFood);

        Camera cam = Camera.main;
        float verticalExtent = cam.orthographicSize;
        float horizontalExtent = verticalExtent * cam.aspect;

        // Уменьши немного, чтобы еда не появлялась прямо у края
        float margin = 0.5f;

        Vector3 pos = new Vector3(
            Random.Range(-horizontalExtent + margin, horizontalExtent - margin),
            Random.Range(-verticalExtent + margin, verticalExtent - margin)
        );

        currentFood = Instantiate(foodPrefab, pos, Quaternion.identity);
        currentFood.GetComponent<PlayerFoodEating>().SetSpawner(this);
    }
}