using UnityEngine;

public class BallController : MonoBehaviour
{
    private Rigidbody2D rb;
    private bool isActiveBalls;
    private Vector3 ballPosition;
    private Vector2 ballInitialForce;

    public GameObject playerObject;
    public float boundary = 10f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        ballInitialForce = new Vector2(100.0f, 300.0f);
        isActiveBalls = false;

        ballPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        // следование за платформой до запуска
        if (!isActiveBalls && playerObject != null)
        {
            ballPosition.x = playerObject.transform.position.x;
            transform.position = ballPosition;
        }

        // проверка нажатия на пробел
        if (Input.GetButtonDown("Jump") && !isActiveBalls)
        {
            rb.AddForce(ballInitialForce);
            isActiveBalls = !isActiveBalls;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // отскок от стен
        if (collision.gameObject.CompareTag("Wall"))
        {
            Vector2 reflectDir = Vector2.Reflect(rb.linearVelocity, collision.contacts[0].normal);
            rb.linearVelocity = reflectDir;
        }
    }
}
