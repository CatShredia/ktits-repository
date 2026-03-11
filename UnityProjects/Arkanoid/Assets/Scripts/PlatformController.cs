using UnityEngine;

// Player platform
// Tag: Player
public class PlatformController : MonoBehaviour
{
    [SerializeField] private float playerVelocity = 0.5f;
    [SerializeField] private float boundary = 9.8f;
    [SerializeField] private float baseScaleX = 2f;
    [SerializeField] private float expandScaleX = 3f;
    [SerializeField] private float shrinkScaleX = 1f;

    private Vector3 playerPosition;
    private Vector3 originalScale;
    private float currentScaleOffset = 0f;

    void Start()
    {
        playerPosition = transform.position;
        originalScale = transform.localScale;
        
        // Set initial scale
        transform.localScale = new Vector3(baseScaleX, originalScale.y, originalScale.z);
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

    public void ExpandPlatform(float amount)
    {
        currentScaleOffset += amount;
        float newScaleX = baseScaleX + currentScaleOffset;
        transform.localScale = new Vector3(Mathf.Max(newScaleX, 0.5f), originalScale.y, originalScale.z);
    }

    public void ShrinkPlatform(float amount)
    {
        currentScaleOffset -= amount;
        float newScaleX = baseScaleX + currentScaleOffset;
        transform.localScale = new Vector3(Mathf.Max(newScaleX, 0.5f), originalScale.y, originalScale.z);
    }

    public void ResetPlatform()
    {
        currentScaleOffset = 0f;
        transform.localScale = new Vector3(baseScaleX, originalScale.y, originalScale.z);
    }
}
