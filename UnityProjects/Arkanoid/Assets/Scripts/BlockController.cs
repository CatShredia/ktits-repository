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

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Ball")) return;

        Destroy(gameObject);
        LevelController.Instance?.BlockDestroyed();

        if (blockType == BlockType.Red)
            GameController.Instance.IncreaseHearts();
        else if (blockType == BlockType.Blue)
            GameController.Instance.SpawnExtraBall(transform.position);
    }
}
