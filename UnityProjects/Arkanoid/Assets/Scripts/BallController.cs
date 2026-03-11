using UnityEngine;

// Ball prefab
// Rigidbody2D, Collider2D
[RequireComponent(typeof(Rigidbody2D))]
public class BallController : MonoBehaviour
{
    [SerializeField] private float ballSpeed = 10f;
    [SerializeField] private Vector2 ballInitialForce = new Vector2(150f, 300f);

    public bool isActiveBalls;
    public bool isClone;
    public GameObject playerObject;

    private Rigidbody2D rb;
    private Vector3 ballPosition;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
            rb = gameObject.AddComponent<Rigidbody2D>();
    }

    void Start()
    {
        if (!isActiveBalls)
        {
            playerObject = GameObject.FindWithTag("Player");
            ballPosition = transform.position;
        }

        GameController.Instance?.RegisterBall(this);
    }

    void OnDestroy()
    {
        GameController.Instance?.UnregisterBall(this);
    }

    public void LaunchBall()
    {
        if (rb == null) return;
        rb.WakeUp();
        rb.linearVelocity = ballInitialForce.normalized * ballSpeed;
    }

    void Update()
    {
        if (MenuController.Instance != null && !MenuController.Instance.IsGameStarted) return;

        if (!isActiveBalls && playerObject != null)
        {
            ballPosition.x = playerObject.transform.position.x;
            transform.position = ballPosition;
        }

        if (Input.GetButtonDown("Jump") && !isActiveBalls)
        {
            rb.linearVelocity = ballInitialForce.normalized * ballSpeed;
            isActiveBalls = true;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (isActiveBalls && rb.linearVelocity.magnitude > 0)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * ballSpeed;
        }

        if (collision.gameObject.CompareTag("WallDown"))
        {
            if (isClone)
            {
                Destroy(gameObject);
            }
            else
            {
                GameController.Instance.DescreaseHeart();
                GameController.Instance.DestroyAllBalls();
            }
        }
    }
}
