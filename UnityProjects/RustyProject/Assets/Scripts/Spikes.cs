using UnityEngine;

/// <summary>
/// Шипы: наносят урон игроку при контакте.
/// Вешается на объект с Collider2D (isTrigger = true).
/// </summary>
public class Spikes : MonoBehaviour
{
    [Header("Damage Settings")]
    [Tooltip("Урон, наносимый игроку")]
    public int damage = 1;

    [Tooltip("Задержка между ударами (секунды)")]
    public float damageCooldown = 1f;

    [Header("Knockback")]
    [Tooltip("Сила отбрасывания")]
    public float knockbackForce = 5f;

    [Header("Visual Settings")]
    [Tooltip("Цвет вспышки при активации")]
    public Color flashColor = Color.red;

    [Tooltip("Время вспышки (секунды)")]
    public float flashDuration = 0.2f;

    private float lastDamageTime = -999f;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            ApplyDamage(other);
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            ApplyDamage(other);
        }
    }

    private void ApplyDamage(Collider2D playerCollider)
    {
        if (Time.time < lastDamageTime + damageCooldown) return;

        lastDamageTime = Time.time;

        // Наносим урон через GameManager
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RemoveLife(damage);
            Debug.Log($"Spikes: Игрок получил {damage} урона от шипов");
        }

        // Отталкивание игрока
        Rigidbody2D playerRb = playerCollider.GetComponent<Rigidbody2D>();
        PlayerMovement playerMovement = playerCollider.GetComponent<PlayerMovement>();
        if (playerMovement != null && playerRb != null)
        {
            Vector2 knockbackDir = playerCollider.transform.position - transform.position;
            knockbackDir.y = 0f;
            knockbackDir.Normalize();
            playerMovement.ApplyKnockback(knockbackDir, knockbackForce);
        }

        // Визуальная вспышка
        FlashVisual();
    }

    private void FlashVisual()
    {
        if (spriteRenderer == null) return;

        // Вспышка
        spriteRenderer.color = flashColor;
        Invoke(nameof(ResetColor), flashDuration);
    }

    private void ResetColor()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = originalColor;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, Vector3.one * 0.5f);
    }
}