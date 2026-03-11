using UnityEngine;

// Player platform
// Tag: Player
public class PlatformController : MonoBehaviour
{
    [SerializeField] private float playerVelocity = 0.5f;
    [SerializeField] private float boundary = 9.8f;

    private Vector3 playerPosition;

    void Start() => playerPosition = transform.position;

    void Update()
    {
        if (MenuController.Instance != null && !MenuController.Instance.IsGameStarted) return;

        playerPosition.x += Input.GetAxis("Horizontal") * playerVelocity;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

        transform.position = new Vector3(
            Mathf.Clamp(playerPosition.x, -boundary, boundary),
            playerPosition.y,
            playerPosition.z);

        playerPosition = transform.position;
    }
}
