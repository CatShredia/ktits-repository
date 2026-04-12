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

    [Header("Knockback")]
    [Tooltip("Сила отбрасывания")]
    public float knockbackForce = 8f;

    private float baseZRotation;

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
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.RemoveLife(damageAmount);

            Rigidbody2D playerRb = other.GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                Vector2 knockbackDirection = (other.transform.position - transform.position).normalized;
                playerRb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);
            }
        }
    }
}
