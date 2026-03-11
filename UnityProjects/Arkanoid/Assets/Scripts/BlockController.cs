using UnityEngine;

public class BlockController : MonoBehaviour
{
    public enum BlockType { Normal, Red, Blue }

    [SerializeField] private BlockType blockType = BlockType.Normal;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Block collision with: " + collision.gameObject.name + ", Tag: " + collision.gameObject.tag + ", BlockType: " + blockType);

        if (collision.gameObject.CompareTag("Ball"))
        {
            switch (blockType)
            {
                case BlockType.Normal:
                    Debug.Log("Destroying Normal block");
                    Destroy(gameObject);
                    break;
                case BlockType.Red:
                    Debug.Log("Red block - restoring heart");
                    GameController.Instance.IncreaseHearts();
                    Destroy(gameObject);
                    break;
                case BlockType.Blue:
                    Debug.Log("Blue block - spawning extra ball at: " + transform.position);
                    GameController.Instance.SpawnExtraBall(transform.position);
                    Destroy(gameObject);
                    break;
            }
        }
    }
}
