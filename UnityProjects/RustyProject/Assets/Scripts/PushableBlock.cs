using UnityEngine;

/// <summary>
/// Толкаемый блок: игрок подходит, зажимает "E" и толкает блок.
/// Вешается на блок с Rigidbody2D и BoxCollider2D.
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

    private Rigidbody2D rb;
    private PlayerMovement currentPusher;
    private bool isBeingPushed;
    private bool isKnockedBack = false;
    private float knockbackEndTime;

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
                rb.linearVelocity = Vector2.zero;
            }
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

        // Двигаем блок
        rb.linearVelocity = new Vector2(pushDirX * pushSpeed, 0f);

        // Двигаем игрока вместе с блоком
        Rigidbody2D playerRb = currentPusher.GetComponent<Rigidbody2D>();
        if (playerRb != null)
        {
            playerRb.linearVelocity = new Vector2(pushDirX * pushSpeed, playerRb.linearVelocity.y);
        }
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
