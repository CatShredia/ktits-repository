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

    void Update()
    {
        // Debug hotkeys (using non-conflicting keys)
        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
        {
            if (Input.GetKeyDown(KeyCode.F1))  // F1 pressed (not held)
            {
                Debug.Log("Debug: Ball Speed Up (Ctrl+F1)");
                ApplyBallSpeedChange(1.5f);
            }
            else if (Input.GetKeyDown(KeyCode.F2))  // F2 pressed (not held)
            {
                Debug.Log("Debug: Ball Speed Down (Ctrl+F2)");
                ApplyBallSpeedChange(0.7f);
            }
            else if (Input.GetKeyDown(KeyCode.F3))  // F3 pressed (not held)
            {
                Debug.Log("Debug: Platform Expand (Ctrl+F3)");
                ApplyPlatformChange(true);
            }
            else if (Input.GetKeyDown(KeyCode.F4))  // F4 pressed (not held)
            {
                Debug.Log("Debug: Platform Shrink (Ctrl+F4)");
                ApplyPlatformChange(false);
            }
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
                break;

            case BonusType.BallSpeedDown:
                foreach (var ball in balls)
                    ball?.ChangeSpeed(1f / speedMultiplier);
                break;

            case BonusType.PlatformExpand:
                platform?.ExpandPlatform(platformExpandAmount);
                Invoke(nameof(ResetPlatform), effectDuration);
                break;

            case BonusType.PlatformShrink:
                platform?.ShrinkPlatform(platformExpandAmount);
                Invoke(nameof(ResetPlatform), effectDuration);
                break;
        }
    }

    void ResetPlatform()
    {
        var platform = FindObjectOfType<PlatformController>();
        platform?.ResetPlatform();
    }

    void ApplyBallSpeedChange(float multiplier)
    {
        var balls = FindObjectsOfType<BallController>();
        foreach (var ball in balls)
            ball?.ChangeSpeed(multiplier);
    }

    void ApplyPlatformChange(bool expand)
    {
        var platform = FindObjectOfType<PlatformController>();
        if (platform != null)
        {
            if (expand)
                platform.ExpandPlatform(platformExpandAmount);
            else
                platform.ShrinkPlatform(platformExpandAmount);
        }
    }
}
