using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BallController : MonoBehaviour
{
    private Rigidbody2D rb;
    public bool isActiveBalls;
    public bool isClone = false;
    private Vector3 ballPosition;
    private Vector2 ballInitialForce;
    public float ballSpeed = 10f;

    public GameObject playerObject;
    public float boundary = 10f;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        if (rb == null)
        {
            Debug.LogError("Rigidbody2D not found on " + gameObject.name);
            rb = gameObject.AddComponent<Rigidbody2D>();
        }

        ballInitialForce = new Vector2(150.0f, 300.0f);
    }

    void Start()
    {
        Debug.Log($"[Ball] Start() - isActiveBalls={isActiveBalls}, position={transform.position}");

        // Only find player if ball is not already active (spawned from block)
        if (!isActiveBalls)
        {
            playerObject = GameObject.FindWithTag("Player");
            ballPosition = transform.position;
            Debug.Log($"[Ball] Attached to player: {playerObject}");
        }

        if (GameController.Instance != null)
        {
            GameController.Instance.RegisterBall(this);
        }
    }

    private void OnDestroy()
    {
        GameController.Instance.UnregisterBall(this);
    }

    public void LaunchBall()
    {
        Debug.Log($"[Ball] LaunchBall() - rb={rb}, force={ballInitialForce}, position={transform.position}");
        if (rb == null)
        {
            Debug.LogError("Rigidbody2D is null on " + gameObject.name);
            return;
        }
        rb.WakeUp();
        rb.linearVelocity = new Vector2(ballInitialForce.x, ballInitialForce.y).normalized * ballSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isActiveBalls && playerObject != null)
        {
            ballPosition.x = playerObject.transform.position.x;
            transform.position = ballPosition;
        }

        if (Input.GetButtonDown("Jump") && !isActiveBalls)
        {
            rb.linearVelocity = new Vector2(ballInitialForce.x, ballInitialForce.y).normalized * ballSpeed;
            isActiveBalls = !isActiveBalls;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Maintain speed after collision (optional, for consistent gameplay)
        if (isActiveBalls && rb.linearVelocity.magnitude > 0)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * ballSpeed;
        }

        if (collision.gameObject.CompareTag("WallDown"))
        {
            Debug.Log("WallDown - Ball: " + gameObject.name + ", isClone: " + isClone);
            
            if (isClone)
            {
                // Clone ball - just destroy itself
                Destroy(gameObject);
            }
            else
            {
                // Original ball - decrease heart and destroy all clones
                GameController.Instance.DescreaseHeart();
                GameController.Instance.DestroyAllBalls();
            }
        }
    }
}
