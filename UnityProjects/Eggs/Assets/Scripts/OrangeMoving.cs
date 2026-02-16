using UnityEngine;

public class OrangeMoving : MonoBehaviour
{
    public float torque = 500f;
    public float rightBoundary = 9f;
    public float leftBoundary = -9f;

    void Start()
    {
        float direction = (transform.position.x >= 0) ? 1f : -1f;
        GetComponent<Rigidbody2D>().AddTorque(direction * torque);
    }

    void Update()
    {
        // if (transform.position.x <= leftBoundary || transform.position.x >= rightBoundary)
        // {
        //     if (GameController.Instance != null)
        //     {
        //         GameController.Instance.OnOrangeDestroyed();
        //         HeartsSystem.Instance.heartCount--;
        //         HeartsSystem.Instance.RedrawHearts();
        //     }
        //     Destroy(gameObject);
        // }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Danger"))
        {
            GameController.Instance.OnOrangeDestroyed();
            HeartsSystem.Instance.heartCount--;
            HeartsSystem.Instance.RedrawHearts();
            Destroy(gameObject);
            Debug.Log("Object destroyed by touching: " + collision.gameObject.name);
        }
    }
}