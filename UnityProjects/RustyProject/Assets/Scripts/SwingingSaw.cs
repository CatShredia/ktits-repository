using UnityEngine;

/// <summary>
/// Качающаяся пила: раскачивается по оси Z и наносит урон при касании.
/// Вешается на объект с Collider2D (isTrigger = true или обычный коллайдер).
/// </summary>
public class SwingingSaw : MonoBehaviour
{
    [Header("Swing Settings")]
    [Tooltip("Максимальный угол отклонения (градусы)")]
    public float maxAngle = 100f;

    [Tooltip("Скорость раскачивания")]
    public float swingSpeed = 2f;

    [Tooltip("Начальный угол (для якоря)")]
    public float startAngle = 0f;

    [Header("Damage")]
    [Tooltip("Количество отнимаемых жизней")]
    public int damageAmount = 1;

    [Header("Behavior Type")]
    [Tooltip("Тип поведения при контакте")]
    public SpikeBehavior behaviorType = SpikeBehavior.Knockback;

    [Header("Knockback")]
    [Tooltip("Сила отбрасывания")]
    public float knockbackForce = 8f;


    [Header("Cooldown")]
    [Tooltip("Задержка между срабатываниями (секунды)")]
    public float damageCooldown = 1f;

    private float baseZRotation;
    private float lastDamageTime = -999f;

    void Start()
    {
        baseZRotation = startAngle;
    }

    void Update()
    {
        float angle = Mathf.Sin(Time.time * swingSpeed) * maxAngle;
        transform.rotation = Quaternion.Euler(0f, 0f, baseZRotation + angle);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && Time.time > lastDamageTime + damageCooldown)
        {
            lastDamageTime = Time.time;
            string levelName = LevelManager.Instance?.ActiveLevelData?.name ?? "StartZone";
            Debug.Log($"[Damage] Уровень получения урона (SwingingSaw): '{levelName}'");
            GameManager.Instance.RemoveLife(damageAmount);

            switch (behaviorType)
            {
                case SpikeBehavior.Knockback:
                    ApplyKnockbackToPlayer(other);
                    break;
                case SpikeBehavior.Teleport:
                    ApplyTeleportToPlayer(other);
                    break;
            }
        }

        // Отталкивание блока (только для типа Knockback)
        PushableBlock pushableBlock = other.GetComponent<PushableBlock>();
        if (pushableBlock != null && Time.time > lastDamageTime + damageCooldown)
        {
            lastDamageTime = Time.time;

            Rigidbody2D blockRb = other.GetComponent<Rigidbody2D>();
            if (blockRb != null)
            {
                Vector2 knockbackDir = other.transform.position - transform.position;
                knockbackDir.y = 0f;
                knockbackDir.Normalize();
                pushableBlock.ApplyKnockback(knockbackDir, knockbackForce);
            }
        }
    }

    /// <summary>
    /// Отталкивание игрока
    /// </summary>
    private void ApplyKnockbackToPlayer(Collider2D playerCollider)
    {
        Rigidbody2D playerRb = playerCollider.GetComponent<Rigidbody2D>();
        PlayerMovement playerMovement = playerCollider.GetComponent<PlayerMovement>();
        if (playerMovement != null)
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
    private void ApplyTeleportToPlayer(Collider2D playerCollider)
    {
        if (LevelManager.Instance != null && LevelManager.Instance.ActiveLevelData != null)
            LevelManager.Instance.RespawnPlayer();
        else
            Debug.LogWarning("SwingingSaw: LevelManager или ActiveLevelData не найдены, телепортация невозможна");
    }
}
