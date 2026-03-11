using UnityEngine;

// Player platform GameObject
// Tag: "Player"
public class PlatformController : MonoBehaviour
{
    [SerializeField] private float playerVelocity = 0.5f;
    [SerializeField] private float boundary = 10f;

    private Vector3 playerPosition;

    void Start()
    {
        playerPosition = transform.position;
    }

    void Update()
    {
        playerPosition.x += Input.GetAxis("Horizontal") * playerVelocity;

        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();

        transform.position = playerPosition;
        transform.position = new Vector3(
            Mathf.Clamp(transform.position.x, -boundary, boundary),
            transform.position.y,
            transform.position.z);
    }
}
