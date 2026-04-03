using UnityEngine;

/// <summary>
/// Компонент для применения скинов к объекту (Platform / Ball).
/// Вешается на префаб. При старте запрашивает скин у SkinManager
/// и применяет его к SpriteRenderer.
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class SkinApplier : MonoBehaviour
{
    [Header("=== Skin Type ===")]
    [Tooltip("Тип скина: Platform или Ball")]
    [SerializeField] public string SkinType = "Platform";

    private SpriteRenderer _spriteRenderer;
    private Vector2 _originalSpriteSize;  // native world size of original sprite
    private Vector2 _originalColliderSize;
    private float _appliedScaleFactor = 1f; // текущий применённый scaleFactor (для отмены)

    void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();

        // Запомнить исходный размер спрайта и коллайдера ДО замены
        if (_spriteRenderer != null && _spriteRenderer.sprite != null)
        {
            _originalSpriteSize = GetSpriteNativeSize(_spriteRenderer.sprite);
        }

        var collider = GetComponent<BoxCollider2D>();
        if (collider != null)
        {
            _originalColliderSize = collider.size;
        }
    }

    /// <summary>
    /// Получить нативный размер спрайта в мировых единицах.
    /// </summary>
    private Vector2 GetSpriteNativeSize(Sprite sprite)
    {
        if (sprite == null) return Vector2.zero;
        return new Vector2(
            sprite.rect.width / sprite.pixelsPerUnit,
            sprite.rect.height / sprite.pixelsPerUnit
        );
    }

    void Start()
    {
        // Применить сохранённый скин при загрузке
        ApplyEquippedSkin();
    }

    /// <summary>
    /// Применить спрайт к объекту.
    /// Масштабирует объект так, чтобы новый спрайт визуально совпадал
    /// с размером исходного спрайта. Коллайдер остаётся неизменным
    /// в мировом пространстве (localSize компенсируется scaleFactor).
    /// </summary>
    public void ApplySprite(Sprite sprite)
    {
        if (sprite == null || _spriteRenderer == null)
        {
            Debug.LogWarning($"[SkinApplier] Cannot apply sprite: sprite={sprite}, renderer={_spriteRenderer}");
            return;
        }

        // Сначала отменить предыдущий scaleFactor (если был)
        if (_appliedScaleFactor > 0 && _appliedScaleFactor != 1f)
        {
            transform.localScale /= _appliedScaleFactor;
        }

        // Рассчитать новый коэффициент масштабирования по высоте исходного спрайта
        Vector2 newSize = GetSpriteNativeSize(sprite);
        float scaleFactor = 1f;
        if (_originalSpriteSize.y > 0 && newSize.y > 0)
        {
            scaleFactor = _originalSpriteSize.y / newSize.y;
            transform.localScale *= scaleFactor;
        }

        _appliedScaleFactor = scaleFactor;

        // Применить спрайт
        _spriteRenderer.sprite = sprite;

        // Восстановить коллайдер в мировом пространстве:
        // worldSize = localSize × localScale → localSize = worldSize / localScale
        var collider = GetComponent<BoxCollider2D>();
        if (collider != null && scaleFactor > 0)
        {
            collider.size = new Vector2(
                _originalColliderSize.x / scaleFactor,
                _originalColliderSize.y / scaleFactor
            );
        }

        Debug.Log($"[SkinApplier] Applied '{sprite.name}' to {gameObject.name}, scaleFactor={scaleFactor:F3}, collider world-size preserved");
    }

    /// <summary>
    /// Запросить и применить текущий экипированный скин из SkinManager.
    /// </summary>
    public void ApplyEquippedSkin()
    {
        if (SkinManager.Instance == null)
        {
            Debug.LogWarning($"[SkinApplier] SkinManager.Instance is null, cannot apply skin for {gameObject.name}");
            return;
        }

        string skinName = SkinManager.Instance.GetEquippedSkin(SkinType);
        if (string.IsNullOrEmpty(skinName))
        {
            Debug.Log($"[SkinApplier] No equipped skin for type {SkinType} on {gameObject.name}, keeping default sprite");
            return;
        }

        Sprite sprite = SkinManager.Instance.GetSpriteForSkin(skinName);
        ApplySprite(sprite);
    }
}
