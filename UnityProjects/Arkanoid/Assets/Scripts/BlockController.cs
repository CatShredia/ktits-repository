using UnityEngine;

// Blocks
// Collider2D
public class BlockController : MonoBehaviour
{
    // normal - обычный блок
    // красный - увеличивает жизни
    // синий - спавнит дополнительный шарик
    // серый - неразрушаемый блок
    // multiHit - блок с несколькими HP (2-4)
    public enum BlockType { Normal, Red, Blue, Invulnerable, MultiHit }

    [SerializeField] private BlockType blockType = BlockType.Normal;
    [SerializeField] private float bonusDropChance = 0.2f;
    [SerializeField] private GameObject[] bonusPrefabs;

    // MultiHit block fields
    [SerializeField] private int currentHP = 2;
    [SerializeField] private int maxHP = 2;

    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    public bool IsInvulnerable => blockType == BlockType.Invulnerable;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        if (spriteRenderer != null)
            originalColor = spriteRenderer.color;
    }

    void Start()
    {
        if (blockType == BlockType.MultiHit)
        {
            SetRandomHP();
            UpdateTransparency();
        }
    }

    void SetRandomHP()
    {
        // Шансы: 2 HP - 50%, 3 HP - 35%, 4 HP - 15%
        float roll = Random.value;
        if (roll < 0.50f)
            maxHP = 2;
        else if (roll < 0.85f)
            maxHP = 3;
        else
            maxHP = 4;

        currentHP = maxHP;
    }

    void UpdateTransparency()
    {
        if (spriteRenderer == null) return;

        // Чем меньше HP, тем прозрачнее
        // 4 HP = 1.0, 3 HP = 0.75, 2 HP = 0.5
        float alpha = maxHP == 0 ? 1f : (float)currentHP / maxHP;
        alpha = Mathf.Clamp01(alpha);

        Color newColor = originalColor;
        newColor.a = alpha;
        spriteRenderer.color = newColor;
    }

    void Hit()
    {
        currentHP--;
        SoundManager.Instance?.PlayBlockHit();
        UpdateTransparency();

        if (currentHP <= 0)
        {
            DestroyBlock();
        }
    }

    void DestroyBlock()
    {
        SoundManager.Instance?.PlayBlockDestroyed();
        Destroy(gameObject);
        LevelController.Instance?.BlockDestroyed();

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

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Ball")) return;

        // MultiHit block logic
        if (blockType == BlockType.MultiHit)
        {
            Hit();
            return;
        }

        // Invulnerable blocks cannot be destroyed
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
