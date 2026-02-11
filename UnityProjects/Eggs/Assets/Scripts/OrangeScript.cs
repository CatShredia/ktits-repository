using UnityEngine;

public class OrangeScript : MonoBehaviour
{
    [SerializeField] private GameObject orangePrefab;
    [SerializeField] private Transform launchPoint; // точка выброса (напр., рука Чебурашки)
    [SerializeField] private float throwForce = 5f;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
            LaunchOrange(Vector2.left);
        else if (Input.GetKeyDown(KeyCode.E))
            LaunchOrange(Vector2.right);
        else if (Input.GetKeyDown(KeyCode.A))
            LaunchOrange(Vector2.down);
        else if (Input.GetKeyDown(KeyCode.D))
            LaunchOrange(Vector2.up);
    }

    void LaunchOrange(Vector2 direction)
    {
        GameObject orange = Instantiate(orangePrefab, launchPoint.position, Quaternion.identity);
        Rigidbody2D rb = orange.GetComponent<Rigidbody2D>();
        rb.linearVelocity = direction.normalized * throwForce;
    }
}
