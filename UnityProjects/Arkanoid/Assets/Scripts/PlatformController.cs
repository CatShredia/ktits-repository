using UnityEngine;

// Player platform
// Tag: Player
public class PlatformController : MonoBehaviour
{
    [SerializeField] private float playerVelocity = 0.5f;
    [SerializeField] private float leftBoundary = -9.8f;
    [SerializeField] private float rightBoundary = 9.8f;
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

        Debug.Log($"[Platform] Start() - originalScale: {originalScale}, baseScaleX: {baseScaleX}");

        transform.localScale = new Vector3(baseScaleX, originalScale.y, originalScale.z);

        Debug.Log($"[Platform] Start() - new scale: {transform.localScale}");
    }

    void Update()
    {
        if (MenuController.Instance != null && !MenuController.Instance.IsGameStarted) return;

        playerPosition.x += Input.GetAxis("Horizontal") * playerVelocity;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

        float halfPlatformWidth = transform.localScale.x / 2f;
        float effectiveLeftBoundary = leftBoundary + halfPlatformWidth;
        float effectiveRightBoundary = rightBoundary - halfPlatformWidth;

        transform.position = new Vector3(
            Mathf.Clamp(playerPosition.x, effectiveLeftBoundary, effectiveRightBoundary),
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
        float newY = originalScale.y > 0 ? originalScale.y : 1f;
        float newZ = originalScale.z > 0 ? originalScale.z : 1f;
        transform.localScale = new Vector3(baseScaleX, newY, newZ);
        Debug.Log($"[Platform] ResetPlatform() - scale: {transform.localScale}");
    }
}
