using UnityEngine;

/// <summary>
/// Движущаяся платформа: перемещается по заданным точкам.
/// Вешается на платформу с Rigidbody2D (Kinematic) и Collider2D.
/// </summary>
public class MovingPlatform : MonoBehaviour
{
    [Header("Waypoints")]
    [Tooltip("Точки, по которым движется платформа")]
    public Transform[] waypoints;

    [Header("Movement Settings")]
    [Tooltip("Скорость движения")]
    public float moveSpeed = 3f;

    [Tooltip("Ждать ли в каждой точке")]
    public bool waitAtWaypoints = true;

    [Tooltip("Время ожидания в точке (секунды)")]
    public float waitTime = 1f;

    [Tooltip("Зацикленное движение")]
    public bool loop = true;

    private int currentWaypointIndex = 0;
    private int direction = 1; // 1 = вперёд, -1 = назад
    private bool isWaiting = false;
    private float waitTimer;

    void Update()
    {
        if (waypoints == null || waypoints.Length < 2) return;
        if (isWaiting)
        {
            waitTimer -= Time.deltaTime;
            if (waitTimer <= 0f)
            {
                isWaiting = false;
            }
            return;
        }

        Transform target = waypoints[currentWaypointIndex];
        transform.position = Vector3.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, target.position) < 0.01f)
        {
            if (waitAtWaypoints)
            {
                isWaiting = true;
                waitTimer = waitTime;
            }

            // Выбираем следующую точку
            currentWaypointIndex += direction;

            // Если дошли до конца
            if (currentWaypointIndex >= waypoints.Length)
            {
                if (loop)
                {
                    currentWaypointIndex = 0;
                }
                else
                {
                    direction = -1;
                    currentWaypointIndex = waypoints.Length - 2;
                }
            }
            else if (currentWaypointIndex < 0)
            {
                currentWaypointIndex = 0;
                direction = 1;
            }
        }
    }

    void OnDrawGizmos()
    {
        if (waypoints == null || waypoints.Length < 2) return;

        Gizmos.color = Color.cyan;
        for (int i = 0; i < waypoints.Length; i++)
        {
            if (waypoints[i] == null) continue;

            // Рисуем точку
            Gizmos.DrawWireSphere(waypoints[i].position, 0.2f);

            // Рисуем линию к следующей точке
            if (i < waypoints.Length - 1 && waypoints[i + 1] != null)
            {
                Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position);
            }
            else if (loop && i == waypoints.Length - 1 && waypoints[0] != null)
            {
                Gizmos.DrawLine(waypoints[i].position, waypoints[0].position);
            }
        }
    }
}
