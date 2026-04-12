using UnityEngine;

/// <summary>
/// Стена-ловушка: при касании игроком отнимает жизнь и телепортирует его на заданную позицию.
/// Вешается на объект с BoxCollider2D (isTrigger = true).
/// </summary>
public class DamageWall : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("Позиция, на которую телепортируется игрок")]
    public Vector3 respawnPosition = new Vector3(0f, 0f, 1f);

    [Tooltip("Количество отнимаемых жизней")]
    public int damageAmount = 1;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.RemoveLife(damageAmount);
            other.transform.position = respawnPosition;
        }
    }
}
