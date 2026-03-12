using UnityEngine;

// Bonus prefabs
// Collider2D, Rigidbody2D
public class BonusController : MonoBehaviour
{
    public enum BonusType
    {
        BallSpeedUp,
        BallSpeedDown,
        PlatformExpand,
        PlatformShrink
    }

    [SerializeField] private BonusType bonusType;
    [SerializeField] private float fallSpeed = 3f;
    [SerializeField] private float effectDuration = 10f;
    [SerializeField] private float speedMultiplier = 1.3f;
    [SerializeField] private float platformExpandAmount = 0.5f;

    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.gravityScale = 0;
            rb.linearVelocity = Vector2.down * fallSpeed;
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            ApplyEffect();
            Destroy(gameObject);
        }
        else if (collision.CompareTag("WallDown"))
        {
            Destroy(gameObject);
        }
    }

    void ApplyEffect()
    {
        var platform = FindObjectOfType<PlatformController>();
        var balls = FindObjectsOfType<BallController>();

        Debug.Log($"[Bonus] Picked up: {bonusType}");

        switch (bonusType)
        {
            case BonusType.BallSpeedUp:
                foreach (var ball in balls)
                    ball?.ChangeSpeed(speedMultiplier);
                Debug.Log($"[Bonus] Ball speed increased by x{speedMultiplier}");
                BonusUIManager.Instance?.ShowEffect("Ball Speed Up", effectDuration);
                break;

            case BonusType.BallSpeedDown:
                foreach (var ball in balls)
                    ball?.ChangeSpeed(1f / speedMultiplier);
                Debug.Log($"[Bonus] Ball speed decreased by x{1f / speedMultiplier:F2}");
                BonusUIManager.Instance?.ShowEffect("Ball Speed Down", effectDuration);
                break;

            case BonusType.PlatformExpand:
                platform?.ExpandPlatform(platformExpandAmount);
                Debug.Log($"[Bonus] Platform expanded (+{platformExpandAmount}) for {effectDuration}s");
                BonusUIManager.Instance?.ShowEffect("Platform Expand", effectDuration);
                break;

            case BonusType.PlatformShrink:
                platform?.ShrinkPlatform(platformExpandAmount);
                Debug.Log($"[Bonus] Platform shrunk (-{platformExpandAmount}) for {effectDuration}s");
                BonusUIManager.Instance?.ShowEffect("Platform Shrink", effectDuration);
                break;
        }
    }
}
