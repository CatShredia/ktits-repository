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
            string levelName = LevelManager.Instance?.ActiveLevelData?.name ?? "StartZone";
            Debug.Log($"[Damage] Уровень получения урона (DamageWall): '{levelName}'");

            GameManager.Instance.RemoveLife(damageAmount);

            if (LevelManager.Instance != null && LevelManager.Instance.ActiveLevelData != null)
                LevelManager.Instance.RespawnPlayer();
            else
                other.transform.position = respawnPosition;
        }
    }
}
