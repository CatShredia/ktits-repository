using UnityEngine;

public class LevelTrigger : MonoBehaviour
{
    [Header("Level Configuration")]
    [Tooltip("Данные уровня для загрузки")]
    public LevelData levelData;

    [Header("Detection Settings")]
    [Tooltip("Ссылка на трансформ игрока (автоматически ищется по тегу 'Player')")]
    private Transform playerTransform;

    private bool isLevelLoaded = false;
    private bool isPlayerNear = false;

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
        else
        {
            Debug.LogWarning("LevelTrigger: Игрок с тегом 'Player' не найден!");
        }
    }

    void Update()
    {
        if (playerTransform == null || levelData == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        if (distanceToPlayer <= levelData.loadDistance && !isLevelLoaded)
        {
            LoadLevel();
        }
        else if (distanceToPlayer >= levelData.unloadDistance && isLevelLoaded)
        {
            UnloadLevel();
        }
    }

    private void LoadLevel()
    {
        LevelManager.Instance.LoadLevel(levelData, transform.position);
        isLevelLoaded = true;
        isPlayerNear = true;
        Debug.Log($"LevelTrigger: Уровень загружен на расстоянии от триггера");
    }

    private void UnloadLevel()
    {
        LevelManager.Instance.UnloadCurrentLevel();
        isLevelLoaded = false;
        isPlayerNear = false;
        Debug.Log($"LevelTrigger: Уровень выгружен, игрок отошёл");
    }

    void OnDrawGizmosSelected()
    {
        if (levelData != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, levelData.loadDistance);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, levelData.unloadDistance);
        }
    }
}
