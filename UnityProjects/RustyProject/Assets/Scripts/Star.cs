using UnityEngine;

/// <summary>
/// Звезда-коллекционер. При касании игрока сообщает LevelManager о сборе.
/// </summary>
public class Star : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("Уникальный индекс звезды на уровне (0, 1 или 2)")]
    public int starIndex = 0;

    private LevelData ownerLevelData;

    public void AssignLevel(LevelData levelData)
    {
        ownerLevelData = levelData;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            LevelData levelData = ownerLevelData;
            if (levelData == null && LevelManager.Instance != null)
            {
                levelData = LevelManager.Instance.ActiveLevelData;
            }

            LevelManager.Instance?.CollectStar(levelData, starIndex);
            Destroy(gameObject);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 0.3f);
    }
}
