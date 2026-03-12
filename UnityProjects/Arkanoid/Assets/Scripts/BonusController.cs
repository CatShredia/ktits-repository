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


        switch (bonusType)
        {
            case BonusType.BallSpeedUp:
                foreach (var ball in balls)
                    ball?.ChangeSpeed(speedMultiplier);
                BonusUIManager.Instance?.ShowEffect("Ускорение мяча", effectDuration);
                break;

            case BonusType.BallSpeedDown:
                foreach (var ball in balls)
                    ball?.ChangeSpeed(1f / speedMultiplier);
                BonusUIManager.Instance?.ShowEffect("Замедление мяча", effectDuration);
                break;

            case BonusType.PlatformExpand:
                platform?.ExpandPlatform(platformExpandAmount);
                BonusUIManager.Instance?.ShowEffect("Увеличение платформы", effectDuration);
                break;

            case BonusType.PlatformShrink:
                platform?.ShrinkPlatform(platformExpandAmount);
                BonusUIManager.Instance?.ShowEffect("Уменьшение платформы", effectDuration);
                break;
        }
    }
}
