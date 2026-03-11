using UnityEngine;

// Block sprites
// Collider2D
public class BlockController : MonoBehaviour
{
    public enum BlockType { Normal, Red, Blue }

    [SerializeField] private BlockType blockType = BlockType.Normal;

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Ball")) return;

        switch (blockType)
        {
            case BlockType.Normal:
                Destroy(gameObject);
                break;
            case BlockType.Red:
                GameController.Instance.IncreaseHearts();
                Destroy(gameObject);
                break;
            case BlockType.Blue:
                GameController.Instance.SpawnExtraBall(transform.position);
                Destroy(gameObject);
                break;
        }
    }
}
