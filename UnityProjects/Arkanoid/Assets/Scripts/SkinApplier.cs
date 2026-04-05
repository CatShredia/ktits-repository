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
    private Vector3 _baseLocalScale; // localScale после инициализации контроллера

    void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        // Если ApplySprite уже вызывался (через ApplySkinToAll из ShopController),
        // то transform.localScale уже содержит scaleFactor. Нужно восстановить базовый scale.
        if (_appliedScaleFactor > 0 && _appliedScaleFactor != 1f)
        {
            _baseLocalScale = transform.localScale / _appliedScaleFactor;
            var collider = GetComponent<BoxCollider2D>();
            if (collider != null)
            {
                _originalColliderSize = collider.size * _appliedScaleFactor;
            }
            // _originalSpriteSize уже установлен в ApplySprite, НЕ перезаписываем
        }
        else
        {
            _baseLocalScale = transform.localScale;
            var collider = GetComponent<BoxCollider2D>();
            if (collider != null)
            {
                _originalColliderSize = collider.size;
            }
            // Только если ещё не установлен (ApplySprite не вызывался)
            if (_originalSpriteSize == Vector2.zero && _spriteRenderer != null && _spriteRenderer.sprite != null)
            {
                _originalSpriteSize = GetSpriteNativeSize(_spriteRenderer.sprite);
            }
        }

        // Применить экипированный скин.
        // Если скин уже был применён через ApplySkinToAll, ApplySprite — идемпотентен
        // и пересчитает те же значения (refSize == newSize → scaleFactor=1, scale не меняется).
        ApplyEquippedSkin();
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

        // Если _originalSpriteSize ещё не установлен (вызов до Start()),
        // запоминаем нативный размер ТЕКУЩЕГО спрайта как референс
        if (_originalSpriteSize == Vector2.zero && _spriteRenderer.sprite != null)
        {
            _originalSpriteSize = GetSpriteNativeSize(_spriteRenderer.sprite);
        }

        Vector2 refSize = _originalSpriteSize;
        Vector2 newSize = GetSpriteNativeSize(sprite);
        float scaleFactor = 1f;
        if (refSize.y > 0 && newSize.y > 0)
        {
            scaleFactor = refSize.y / newSize.y;
        }

        _appliedScaleFactor = scaleFactor;

        // Определяем базовый scale
        Vector3 baseScale = _baseLocalScale != Vector3.zero ? _baseLocalScale : transform.localScale;

        // Если это первый вызов ДО Start(), запоминаем базу
        if (_baseLocalScale == Vector3.zero)
        {
            _baseLocalScale = baseScale;

            // Также запоминаем размер коллайдера
            var collider = GetComponent<BoxCollider2D>();
            if (collider != null && _originalColliderSize == Vector2.zero)
            {
                _originalColliderSize = collider.size;
            }
        }

        // Применяем новый scale
        transform.localScale = new Vector3(
            baseScale.x * scaleFactor,
            baseScale.y * scaleFactor,
            baseScale.z * scaleFactor
        );

        // Применяем спрайт
        _spriteRenderer.sprite = sprite;

        // Корректируем коллайдер
        var col = GetComponent<BoxCollider2D>();
        if (col != null && scaleFactor > 0)
        {
            col.size = new Vector2(
                _originalColliderSize.x / scaleFactor,
                _originalColliderSize.y / scaleFactor
            );
        }

        Debug.Log($"[SkinApplier] Applied '{sprite.name}' to {gameObject.name}, scaleFactor={scaleFactor:F3}");
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
