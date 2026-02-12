using UnityEngine;

public class OrangeMoving : MonoBehaviour
{
    public float torque = 500f;
    public float rightBoundary = 9f;
    public float leftBoundary = -9f;

    void Start()
    {
        GetComponent<Rigidbody2D>().AddTorque(-torque);
    }

    void Update()
    {
        if (transform.position.x <= leftBoundary || transform.position.x >= rightBoundary)
        {
            if (GameController.Instance != null)
            {
                GameController.Instance.OnOrangeDestroyed();
            }
            Destroy(gameObject);
        }
    }
}