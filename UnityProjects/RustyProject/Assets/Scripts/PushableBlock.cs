using UnityEngine;

/// <summary>
/// Толкаемый блок: игрок подходит, зажимает "E" и толкает блок.
/// Вешается на блок с Rigidbody2D и BoxCollider2D.
/// Если блок выталкивается на пустое пространство — он падает.
/// При столкновении со стеной после падения — блок респаунится на начальной позиции.
/// </summary>
public class PushableBlock : MonoBehaviour
{
    [Header("Block Settings")]
    [Tooltip("Скорость толкания")]
    public float pushSpeed = 3f;

    [Header("Knockback from Saw")]
    [Tooltip("Множитель силы отталкивания от пилы (0-1)")]
    [Range(0f, 1f)]
    public float sawKnockbackMultiplier = 0.5f;

    [Header("Ground Check")]
    [Tooltip("Радиус проверки земли под блоком")]
    public float groundCheckRadius = 0.3f;

    [Tooltip("Смещение точки проверки земли вниз от центра блока")]
    public float groundCheckDistance = 0.6f;

    [Tooltip("Слой земли для проверки")]
    public LayerMask groundLayer;

    [Header("Wall Check")]
    [Tooltip("Радиус проверки стены впереди блока")]
    public float wallCheckRadius = 0.3f;

    [Tooltip("Расстояние проверки стены впереди блока")]
    public float wallCheckDistance = 0.6f;

    [Tooltip("Слой стены для проверки")]
    public LayerMask wallLayer;

    [Header("Respawn Settings")]
    [Tooltip("Время до респауна после остановки блока (секунды)")]
    public float respawnDelay = 1f;

    private Rigidbody2D rb;
    private PlayerMovement currentPusher;
    private bool isBeingPushed;
    private bool isKnockedBack = false;
    private float knockbackEndTime;

    // Сохранённые начальные координаты
    private Vector3 initialPosition;
    private Quaternion initialRotation;

    // Состояние падения
    private bool isFalling = false;
    private bool hasHitWallAfterFall = false;
    private float respawnTimer = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogWarning("PushableBlock: На объекте отсутствует Rigidbody2D!");
        }
        else
        {
            rb.bodyType = RigidbodyType2D.Kinematic;
        }

        // Сохраняем начальные координаты
        initialPosition = transform.position;
        initialRotation = transform.rotation;
    }

    void Update()
    {
        // Если блок отброшен — ждём окончания кулдауна и возвращаем кинематику
        if (isKnockedBack)
        {
            if (Time.time > knockbackEndTime)
            {
                isKnockedBack = false;
                rb.bodyType = RigidbodyType2D.Kinematic;
                rb.gravityScale = 0f;
                rb.linearVelocity = Vector2.zero;
            }
            return;
        }

        // Обработка падения и респауна
        if (isFalling)
        {
            HandleFalling();
            return;
        }

        // Если игрок в зоне и ещё не толкаем — проверяем E
        if (!isBeingPushed && currentPusher != null && Input.GetKey(KeyCode.E))
        {
            isBeingPushed = true;
            currentPusher.StartPushing(this);
            return;
        }

        if (!isBeingPushed || currentPusher == null) return;

        // Проверяем, что игрок всё ещё держит E
        if (!Input.GetKey(KeyCode.E))
        {
            StopPush();
            return;
        }

        // Определяем направление толкания по тому, куда смотрит игрок
        float pushDirX = currentPusher.transform.localScale.x > 0 ? 1f : -1f;

        // Проверяем, есть ли стена впереди — если да, не толкаем
        if (IsWallAhead(pushDirX))
        {
            Debug.Log("PushableBlock: Впереди стена, толкание заблокировано");
            StopPush();
            return;
        }

        // Двигаем блок
        rb.linearVelocity = new Vector2(pushDirX * pushSpeed, 0f);

        // Двигаем игрока вместе с блоком
        Rigidbody2D playerRb = currentPusher.GetComponent<Rigidbody2D>();
        if (playerRb != null)
        {
            playerRb.linearVelocity = new Vector2(pushDirX * pushSpeed, playerRb.linearVelocity.y);
        }

        // Проверяем, есть ли опора под блоком во время толкания
        if (!IsGroundedBelow())
        {
            StartFalling();
        }
    }

    /// <summary>
    /// Проверка наличия опоры под блоком
    /// </summary>
    private bool IsGroundedBelow()
    {
        Vector2 checkPos = new Vector2(transform.position.x, transform.position.y - groundCheckDistance);
        return Physics2D.OverlapCircle(checkPos, groundCheckRadius, groundLayer);
    }

    /// <summary>
    /// Проверка наличия стены впереди блока
    /// </summary>
    /// <param name="pushDirection">Направление толкания (1 или -1)</param>
    private bool IsWallAhead(float pushDirection)
    {
        Vector2 checkPos = new Vector2(transform.position.x + (pushDirection * wallCheckDistance), transform.position.y);
        return Physics2D.OverlapCircle(checkPos, wallCheckRadius, wallLayer);
    }

    /// <summary>
    /// Начало падения блока (переключение на Dynamic с гравитацией)
    /// </summary>
    private void StartFalling()
    {
        isFalling = true;
        hasHitWallAfterFall = false;
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 1f;
        Debug.Log($"PushableBlock: Блок начал падать с позиции {transform.position}");
    }

    /// <summary>
    /// Обработка состояния падения
    /// </summary>
    private void HandleFalling()
    {
        // Проверяем, остановился ли блок (скорость близка к нулю)
        if (rb.linearVelocity.magnitude < 0.1f)
        {
            respawnTimer += Time.deltaTime;
            if (respawnTimer >= respawnDelay)
            {
                RespawnBlock();
            }
        }
        else
        {
            respawnTimer = 0f;
        }
    }

    /// <summary>
    /// Обработка столкновений
    /// </summary>
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isFalling) return;

        // Проверяем столкновение со стеной (боковое столкновение)
        foreach (var contact in collision.contacts)
        {
            // Нормаль контакта, направленная от стены
            Vector2 normal = contact.normal;

            // Если нормаль в основном горизонтальная (стена сбоку)
            if (Mathf.Abs(normal.x) > 0.5f)
            {
                hasHitWallAfterFall = true;
                Debug.Log($"PushableBlock: Блок столкнулся со стеной после падения");
                RespawnBlock();
                return;
            }
        }
    }

    /// <summary>
    /// Респаун блока на начальной позиции
    /// </summary>
    private void RespawnBlock()
    {
        Debug.Log($"PushableBlock: Респаун блока на позицию {initialPosition}");
        transform.position = initialPosition;
        transform.rotation = initialRotation;
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0f;
        isFalling = false;
        hasHitWallAfterFall = false;
        respawnTimer = 0f;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            currentPusher = other.GetComponent<PlayerMovement>();
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            StopPush();
            currentPusher = null;
        }
    }

    public void StopPush()
    {
        isBeingPushed = false;
        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic;

        if (currentPusher != null)
        {
            currentPusher.StopPushing();
        }
    }

    public void ApplyKnockback(Vector2 direction, float force)
    {
        // Переключаем на Dynamic для физического воздействия
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 1f;

        isKnockedBack = true;
        knockbackEndTime = Time.time + 2f;
        rb.linearVelocity = new Vector2(direction.x * force * sawKnockbackMultiplier, 5f * sawKnockbackMultiplier);
    }
}
