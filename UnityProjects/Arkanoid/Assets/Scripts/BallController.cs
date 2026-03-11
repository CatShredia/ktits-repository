using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BallController : MonoBehaviour
{
    private Rigidbody2D rb;
    public bool isActiveBalls;
    private Vector3 ballPosition;
    private Vector2 ballInitialForce;

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
        playerObject = GameObject.FindWithTag("Player");
        isActiveBalls = false;
        ballPosition = transform.position;
        
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
        if (rb == null)
        {
            Debug.LogError("Rigidbody2D is null on " + gameObject.name);
            return;
        }
        rb.WakeUp();
        rb.AddForce(ballInitialForce);
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
            rb.AddForce(ballInitialForce);
            isActiveBalls = !isActiveBalls;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("WallDown"))
        {
            Debug.Log("WallDown");
            GameController.Instance.DescreaseHeart();
        }
    }
}
