using UnityEngine;

// Blocks
// Collider2D
public class BlockController : MonoBehaviour
{
    // normal - обычный блок
    // красный - увеличивает жизни
    // синий - спавнит дополнительный шарик
    public enum BlockType { Normal, Red, Blue }

    [SerializeField] private BlockType blockType = BlockType.Normal;
    [SerializeField] private float bonusDropChance = 0.2f;
    [SerializeField] private GameObject[] bonusPrefabs;

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Ball")) return;

        SoundManager.Instance?.PlayBlockDestroyed();
        Destroy(gameObject);
        LevelController.Instance?.BlockDestroyed();

        if (blockType == BlockType.Red)
            GameController.Instance.IncreaseHearts();
        else if (blockType == BlockType.Blue)
            GameController.Instance.SpawnExtraBall(transform.position);

        // Spawn bonus
        if (bonusDropChance > 0 && bonusPrefabs != null && bonusPrefabs.Length > 0)
        {
            if (Random.value < bonusDropChance)
            {
                int randomIndex = Random.Range(0, bonusPrefabs.Length);
                if (bonusPrefabs[randomIndex] != null)
                {
                    Instantiate(bonusPrefabs[randomIndex], transform.position, Quaternion.identity);
                }
            }
        }
    }
}
