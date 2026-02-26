using UnityEngine;

public class ShipMoving : MonoBehaviour
{
    public float speed = 5f;
    private Rigidbody2D _rb;

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector2 movement = new Vector2(h, v) * speed;

        if (_rb != null)
            _rb.linearVelocity = movement;
        else
            transform.Translate(movement * Time.deltaTime, Space.World);
    }
}
