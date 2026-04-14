using UnityEngine;

/// <summary>
/// Враг: ходит от точки до точки с анимацией. Можно убить, прыгнув сверху.
/// Вешается на врага с Animator и Collider2D (isTrigger = true).
/// </summary>
public class Enemy : MonoBehaviour
{
    [Header("Waypoints")]
    [Tooltip("Точки, по которым ходит враг")]
    public Transform[] waypoints;

    [Header("Movement Settings")]
    [Tooltip("Скорость движения")]
    public float moveSpeed = 2f;

    [Tooltip("Ждать ли в каждой точке")]
    public bool waitAtWaypoints = true;

    [Tooltip("Время ожидания в точке (секунды)")]
    public float waitTime = 1f;

    [Header("Animation")]
    [Tooltip("Имя параметра скорости в Animator")]
    public string speedParamName = "Speed";

    [Tooltip("Имя параметра смерти в Animator")]
    public string deathParamName = "IsDead";

    [Header("Health")]
    [Tooltip("Количество жизней врага")]
    public int health = 1;

    [Header("Damage")]
    [Tooltip("Урон, наносимый игроку")]
    public int damageToPlayer = 1;

    [Tooltip("Задержка между ударами (секунды)")]
    public float damageCooldown = 1f;

    [Tooltip("Сила отталкивания игрока при ударе")]
    public float knockbackForce = 5f;

    [Tooltip("Сила отскока при прыжке на врага")]
    public float jumpBounceForce = 7f;

    private int currentWaypointIndex = 0;
    private int direction = 1;
    private bool isWaiting = false;
    private float waitTimer;
    private bool isDead = false;
    private float lastDamageTime = -999f;

    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();

        if (waypoints == null || waypoints.Length < 2)
        {
            Debug.LogWarning("Enemy: Нужно минимум 2 точки для движения!");
        }
    }

    void Update()
    {
        if (isDead) return;
        if (waypoints == null || waypoints.Length < 2) return;

        if (isWaiting)
        {
            waitTimer -= Time.deltaTime;
            if (waitTimer <= 0f)
            {
                isWaiting = false;
            }
            else
            {
                if (animator != null) animator.SetFloat(speedParamName, 0f);
                return;
            }
        }

        Transform target = waypoints[currentWaypointIndex];
        transform.position = Vector3.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);

        // Разворот по направлению движения
        float dirX = target.position.x - transform.position.x;
        if (dirX > 0.01f)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else if (dirX < -0.01f)
        {
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }

        // Анимация движения
        if (animator != null)
        {
            animator.SetFloat(speedParamName, 1f);
        }

        // Проверка достижения точки
        if (Vector3.Distance(transform.position, target.position) < 0.01f)
        {
            if (waitAtWaypoints)
            {
                isWaiting = true;
                waitTimer = waitTime;
            }

            // Выбираем следующую точку
            currentWaypointIndex += direction;

            if (currentWaypointIndex >= waypoints.Length)
            {
                currentWaypointIndex = waypoints.Length - 2;
                direction = -1;
            }
            else if (currentWaypointIndex < 0)
            {
                currentWaypointIndex = 1;
                direction = 1;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (isDead) return;
        if (!other.CompareTag("Player")) return;
        if (!gameObject.CompareTag("Enemy")) return;

        PlayerMovement player = other.GetComponent<PlayerMovement>();
        Rigidbody2D playerRb = other.GetComponent<Rigidbody2D>();
        if (player == null || playerRb == null)
        {
            Debug.LogWarning("[Enemy] Не удалось получить PlayerMovement или Rigidbody2D!");
            return;
        }

        // Проверяем, прыгнул ли игрок сверху
        bool isAbove = other.transform.position.y > transform.position.y;
        bool isFalling = playerRb.linearVelocity.y < 0;

        if (isAbove && isFalling)
        {
            // Убили врага прыжком
            TakeDamage();

            // Подбрасываем игрока
            playerRb.linearVelocity = new Vector2(playerRb.linearVelocity.x, jumpBounceForce);
        }
        else if (Time.time > lastDamageTime + damageCooldown)
        {
            // Игрок получил урон от врага
            lastDamageTime = Time.time;
            GameManager.Instance.RemoveLife(damageToPlayer);

            // Отталкивание игрока
            Vector2 knockbackDir = other.transform.position - transform.position;
            knockbackDir.y = 0f;
            knockbackDir.Normalize();
            player.ApplyKnockback(knockbackDir, knockbackForce);
        }
    }

    public void TakeDamage()
    {
        health--;
        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;
        if (animator != null)
        {
            animator.SetBool(deathParamName, true);
        }

        // Отключаем коллайдер и удаляем сразу
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        Destroy(gameObject);
    }
}
