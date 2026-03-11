using UnityEngine;

// Player platform
// Tag: Player
public class PlatformController : MonoBehaviour
{
    [SerializeField] private float playerVelocity = 0.5f;
    [SerializeField] private float boundary = 9.8f;
    [SerializeField] private float baseWidth = 2f;
    [SerializeField] private float expandWidth = 3f;
    [SerializeField] private float shrinkWidth = 1f;

    private Vector3 playerPosition;
    private float currentWidth;
    private Vector3 originalScale;

    void Start()
    {
        playerPosition = transform.position;
        originalScale = transform.localScale;
        currentWidth = baseWidth;
    }

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

    public void ExpandPlatform()
    {
        currentWidth = expandWidth;
        transform.localScale = new Vector3(expandWidth / baseWidth, originalScale.y, originalScale.z);
    }

    public void ShrinkPlatform()
    {
        currentWidth = shrinkWidth;
        transform.localScale = new Vector3(shrinkWidth / baseWidth, originalScale.y, originalScale.z);
    }

    public void ResetPlatform()
    {
        currentWidth = baseWidth;
        transform.localScale = originalScale;
    }
}
