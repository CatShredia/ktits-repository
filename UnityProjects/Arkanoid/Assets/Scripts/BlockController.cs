using UnityEngine;

// Blocks
// Collider2D
public class BlockController : MonoBehaviour
{
    // normal - обычный блок
    // красный - увеличивает жизни
    // синий - спавнит дополнительный шарик
    // серый - неразрушаемый блок
    public enum BlockType { Normal, Red, Blue, Invulnerable }

    [SerializeField] private BlockType blockType = BlockType.Normal;
    [SerializeField] private float bonusDropChance = 1f;
    [SerializeField] private GameObject[] bonusPrefabs;

    public bool IsInvulnerable => blockType == BlockType.Invulnerable;

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Ball")) return;

        if (blockType == BlockType.Invulnerable)
        {
            SoundManager.Instance?.PlayBlockHit();
            return;
        }

        SoundManager.Instance?.PlayBlockDestroyed();
        Destroy(gameObject);
        LevelController.Instance?.BlockDestroyed();

        if (blockType == BlockType.Red)
            GameController.Instance.IncreaseHearts();
        else if (blockType == BlockType.Blue)
            GameController.Instance.SpawnExtraBall(transform.position);

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
