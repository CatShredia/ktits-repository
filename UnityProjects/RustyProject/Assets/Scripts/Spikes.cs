using UnityEngine;

/// <summary>
/// Тип поведения шипов/пил при контакте с игроком
/// </summary>
public enum SpikeBehavior
{
    Knockback,  // Отбрасывание игрока
    Teleport    // Перенос игрока на заданные координаты
}

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

    [Header("Behavior Type")]
    [Tooltip("Тип поведения при контакте")]
    public SpikeBehavior behaviorType = SpikeBehavior.Knockback;

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
            Debug.Log($"Spikes: Игрок получил {damage} урона от шипов (поведение: {behaviorType})");
        }

        // Обработка поведения в зависимости от типа
        switch (behaviorType)
        {
            case SpikeBehavior.Knockback:
                ApplyKnockback(playerCollider);
                break;

            case SpikeBehavior.Teleport:
                ApplyTeleport(playerCollider);
                break;
        }

        // Визуальная вспышка
        FlashVisual();
    }

    /// <summary>
    /// Отталкивание игрока
    /// </summary>
    private void ApplyKnockback(Collider2D playerCollider)
    {
        Rigidbody2D playerRb = playerCollider.GetComponent<Rigidbody2D>();
        PlayerMovement playerMovement = playerCollider.GetComponent<PlayerMovement>();
        if (playerMovement != null && playerRb != null)
        {
            Vector2 knockbackDir = playerCollider.transform.position - transform.position;
            knockbackDir.y = 0f;
            knockbackDir.Normalize();
            playerMovement.ApplyKnockback(knockbackDir, knockbackForce);
        }
    }

    /// <summary>
    /// Телепортация игрока на координаты спавна текущего уровня из LevelData
    /// </summary>
    private void ApplyTeleport(Collider2D playerCollider)
    {
        Vector3 targetPosition = Vector3.zero;

        // Получаем координаты спавна из LevelData текущего уровня
        if (LevelManager.Instance != null)
        {
            LevelData currentLevel = LevelManager.Instance.GetCurrentLevelData();
            if (currentLevel != null)
            {
                // Используем spawnPlayerPoint, если задан, иначе playerSpawnPosition
                if (currentLevel.useSpawnPlayerPoint)
                {
                    targetPosition = currentLevel.spawnPlayerPoint;
                }
                else
                {
                    targetPosition = currentLevel.playerSpawnPosition;
                }
                Debug.Log($"Spikes: Телепортация на spawn point уровня '{currentLevel.name}': {targetPosition}");
            }
            else
            {
                Debug.LogWarning("Spikes: Текущий уровень не найден, телепортация невозможна");
                return;
            }
        }
        else
        {
            Debug.LogWarning("Spikes: LevelManager не найден, телепортация невозможна");
            return;
        }

        // Перемещаем игрока
        playerCollider.transform.position = targetPosition;

        // Сбрасываем скорость игрока
        Rigidbody2D playerRb = playerCollider.GetComponent<Rigidbody2D>();
        if (playerRb != null)
        {
            playerRb.linearVelocity = Vector2.zero;
        }
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