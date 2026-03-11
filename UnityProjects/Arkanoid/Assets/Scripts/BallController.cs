using UnityEngine;

public class BallController : MonoBehaviour
{
    private Rigidbody2D rb;
    public bool isActiveBalls;
    private Vector3 ballPosition;
    private Vector2 ballInitialForce;

    public GameObject playerObject;
    public float boundary = 10f;

    public static BallController Instance { get; private set; }

    void Awake()
    {
        Instance = this;
    }

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
