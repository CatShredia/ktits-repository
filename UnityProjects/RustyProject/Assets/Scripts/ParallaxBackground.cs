using UnityEngine;

/// <summary>
/// Параллакс-фон: бесшовная прокрутка неба/облаков через один спрайт на всю камеру.
/// Вешается на пустой объект.
/// </summary>
public class ParallaxBackground : MonoBehaviour
{
    [Header("Camera")]
    [Tooltip("Камера для отслеживания (null = MainCamera)")]
    public Camera targetCamera;

    [Header("Settings")]
    [Tooltip("Спрайт облаков/неба (бесшовная текстура)")]
    public Sprite cloudSprite;

    [Tooltip("Скорость прокрутки")]
    public float scrollSpeed = 0.5f;

    [Tooltip("Порядок сортировки (rendering layer)")]
    public int sortingOrder = -10;

    [Header("Appearance")]
    [Tooltip("Дополнительный масштаб по горизонтали")]
    public float horizontalScale = 1f;

    [Tooltip("Дополнительный масштаб по вертикали")]
    public float verticalScale = 1f;

    [Tooltip("Вертикальное смещение (в мировых единицах)")]
    public float yOffset = 0f;

    [Tooltip("Фильтр текстуры (Point для пиксель-арта, Bilinear для сглаживания)")]
    public FilterMode textureFilterMode = FilterMode.Bilinear;

    private SpriteRenderer spriteRenderer;
    private Material backgroundMaterial;

    void Start()
    {
        if (cloudSprite == null)
        {
            Debug.LogError("ParallaxBackground: Не назначен спрайт!");
            return;
        }

        if (targetCamera == null)
        {
            targetCamera = Camera.main;
        }

        CreateBackgroundSprite();
    }

    private void CreateBackgroundSprite()
    {
        GameObject bg = new GameObject("BackgroundSprite");
        bg.transform.SetParent(transform);
        bg.transform.position = new Vector3(0, 0, 0f);

        spriteRenderer = bg.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = cloudSprite;
        spriteRenderer.sortingOrder = sortingOrder;

        // Размеры камеры
        float camHeight = targetCamera.orthographicSize * 2f;
        float camWidth = camHeight * targetCamera.aspect;

        // Размеры спрайта в мире
        float spriteWorldWidth = cloudSprite.bounds.size.x;
        float spriteWorldHeight = cloudSprite.bounds.size.y;

        // Масштабируем спрайт чтобы покрыть всю камеру
        float scaleX = (camWidth / spriteWorldWidth) * horizontalScale;
        float scaleY = (camHeight / spriteWorldHeight) * verticalScale;

        bg.transform.localScale = new Vector3(scaleX, scaleY, 1f);

        // Настраиваем текстуру на повторение и фильтр
        Texture2D texture = cloudSprite.texture;
        texture.wrapMode = TextureWrapMode.Repeat;
        texture.filterMode = textureFilterMode;

        // Создаём материал с этой текстурой
        Shader shader = Shader.Find("Unlit/Transparent");
        backgroundMaterial = new Material(shader);
        backgroundMaterial.mainTexture = texture;
        backgroundMaterial.color = Color.white;

        spriteRenderer.material = backgroundMaterial;
    }

    void Update()
    {
        if (backgroundMaterial == null || targetCamera == null) return;

        // Сдвигаем текстуру
        float offset = Time.time * scrollSpeed;
        backgroundMaterial.mainTextureOffset = new Vector2(offset, 0f);

        // Обновляем позицию и размер, если камера изменилась
        float camHeight = targetCamera.orthographicSize * 2f;
        float camWidth = camHeight * targetCamera.aspect;

        float spriteWorldWidth = cloudSprite.bounds.size.x;
        float spriteWorldHeight = cloudSprite.bounds.size.y;

        float scaleX = (camWidth / spriteWorldWidth) * horizontalScale;
        float scaleY = (camHeight / spriteWorldHeight) * verticalScale;

        spriteRenderer.transform.localScale = new Vector3(scaleX, scaleY, 1f);
        spriteRenderer.transform.position = new Vector3(
            targetCamera.transform.position.x,
            targetCamera.transform.position.y + yOffset,
            0f
        );
    }
}
