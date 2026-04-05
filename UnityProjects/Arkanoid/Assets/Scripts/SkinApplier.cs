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
    private BoxCollider2D _collider;
    private Sprite _defaultSprite;               // дефолтный спрайт (запомнен в Start)
    private float _defaultSpriteNativeHeight;    // нативная высота дефолтного спрайта
    private Vector2 _defaultColliderSize;       // размер коллайдера при дефолтном масштабе
    private Vector3 _defaultLocalScale;          // localScale при дефолтном спрайте

    void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _collider = GetComponent<BoxCollider2D>();
    }

    void Start()
    {
        // Запомнить состояние ПОСЛЕ всех других Start() (PlatformController, BallController)
        _defaultLocalScale = transform.localScale;
        _defaultSprite = _spriteRenderer != null ? _spriteRenderer.sprite : null;
        if (_defaultSprite != null)
        {
            _defaultSpriteNativeHeight = _defaultSprite.rect.height / _defaultSprite.pixelsPerUnit;
        }
        if (_collider != null)
        {
            _defaultColliderSize = _collider.size;
        }

        ApplyEquippedSkin();
    }

    /// <summary>
    /// Применить спрайт к объекту.
    /// Масштабирует объект так, чтобы новый спрайт визуально совпадал
    /// с размером дефолтного спрайта. Коллайдер сохраняет мировой размер.
    ///
    /// Идемпотентен: повторный вызов с тем же спрайтом ничего не меняет.
    /// </summary>
    public void ApplySprite(Sprite sprite)
    {
        if (sprite == null || _spriteRenderer == null)
        {
            Debug.LogWarning($"[SkinApplier] Cannot apply sprite: sprite={sprite}, renderer={_spriteRenderer}");
            return;
        }

        if (_defaultSpriteNativeHeight <= 0)
        {
            Debug.LogWarning($"[SkinApplier] Default sprite height is 0 on {gameObject.name}");
            _spriteRenderer.sprite = sprite;
            return;
        }

        float newNativeHeight = sprite.rect.height / sprite.pixelsPerUnit;
        if (newNativeHeight <= 0)
        {
            Debug.LogWarning($"[SkinApplier] New sprite height is 0: {sprite.name}");
            _spriteRenderer.sprite = sprite;
            return;
        }

        // Коэффициент масштабирования: во сколько раз новый спрайт меньше/больше дефолтного
        float scaleFactor = _defaultSpriteNativeHeight / newNativeHeight;

        // Всегда считаем от _defaultLocalScale — это гарантирует стабильность
        // независимо от текущего состояния (скин уже применён или нет)
        float newScaleY = _defaultLocalScale.y * scaleFactor;
        float newScaleX = _defaultLocalScale.x * scaleFactor;
        float newScaleZ = _defaultLocalScale.z * scaleFactor;

        transform.localScale = new Vector3(newScaleX, newScaleY, newScaleZ);

        // Применяем спрайт
        _spriteRenderer.sprite = sprite;

        // Корректируем коллайдер: сохраняем мировой размер
        // worldSize = localSize * localScale → localSize = worldSize / localScale
        // worldSize (базовый) = _defaultColliderSize * _defaultLocalScale
        float colliderScaleRatio = scaleFactor; // масштаб относительно дефолта
        if (_collider != null && colliderScaleRatio > 0)
        {
            _collider.size = new Vector2(
                _defaultColliderSize.x / colliderScaleRatio,
                _defaultColliderSize.y / colliderScaleRatio
            );
        }

        Debug.Log($"[SkinApplier] Applied '{sprite.name}' to {gameObject.name}: " +
                  $"defH={_defaultSpriteNativeHeight:F2}, newH={newNativeHeight:F2}, " +
                  $"sf={scaleFactor:F3}, scale=({newScaleX:F2},{newScaleY:F2},{newScaleZ:F2})");
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
