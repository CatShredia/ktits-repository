using UnityEngine;

// Attach to: Bonus prefabs
// Required: Collider2D (Is Trigger), Rigidbody2D
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
                    ball?.ChangeSpeed(1.1f);
                break;

            case BonusType.BallSpeedDown:
                foreach (var ball in balls)
                    ball?.ChangeSpeed(0.7f);
                break;

            case BonusType.PlatformExpand:
                platform?.ExpandPlatform();
                Invoke(nameof(ResetPlatform), effectDuration);
                break;

            case BonusType.PlatformShrink:
                platform?.ShrinkPlatform();
                Invoke(nameof(ResetPlatform), effectDuration);
                break;
        }
    }

    void ResetPlatform()
    {
        var platform = FindObjectOfType<PlatformController>();
        platform?.ResetPlatform();
    }
}
