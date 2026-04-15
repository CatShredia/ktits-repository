using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI-индикатор жизней (сердечки). Обновляет отображение сердец в зависимости от количества жизней.
/// Вешается на родительский объект, содержащий Image-компоненты для каждого сердца.
/// </summary>
public class HeartUI : MonoBehaviour
{
    [Header("Heart Images")]
    [Tooltip("Image-компоненты сердец (по порядку)")]
    public Image[] heartImages = new Image[5];

    [Header("Appearance")]
    [Tooltip("Спрайт сердца")]
    public Sprite heartSprite;

    [Header("Auto Setup")]
    [Tooltip("Автоматически создавать недостающие сердца")]
    public bool autoCreateHearts = true;

    private Transform heartsContainer;

    void Start()
    {
        ValidateReferences();

        GameManager.Instance.OnLivesChanged += UpdateHeartUI;

        // Автоматическое создание сердец при необходимости
        if (autoCreateHearts)
        {
            CreateMissingHearts();
        }

        // Инициализация при старте
        UpdateHeartUI(GameManager.Instance.Lives);
    }

    void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnLivesChanged -= UpdateHeartUI;
        }
    }

    private void ValidateReferences()
    {
        // Находим контейнер для сердец (родительский объект)
        heartsContainer = transform;

        for (int i = 0; i < heartImages.Length; i++)
        {
            if (heartImages[i] == null)
                Debug.LogWarning($"HeartUI: Сердце {i} не назначено!");
        }

        if (heartSprite == null)
            Debug.LogWarning("HeartUI: Спрайт сердца не назначен!");
    }

    private void CreateMissingHearts()
    {
        // Используем количество жизней для определения количества сердец
        int maxHearts = GameManager.Instance.Lives;
        int currentHearts = heartImages.Length;

        if (maxHearts <= currentHearts) return;

        Debug.Log($"HeartUI: Автоматическое создание {maxHearts - currentHearts} сердец");

        // Создаём недостающие сердца
        System.Array.Resize(ref heartImages, maxHearts);

        for (int i = currentHearts; i < maxHearts; i++)
        {
            // Создаём новый объект для сердца
            GameObject heartObj = new GameObject($"Heart_{i + 1}");
            heartObj.transform.SetParent(heartsContainer, false);

            // Добавляем Image компонент
            Image heartImage = heartObj.AddComponent<Image>();
            heartImage.sprite = heartSprite;
            heartImage.color = Color.white;

            // Настраиваем RectTransform для равномерного распределения
            RectTransform rectTransform = heartImage.rectTransform;
            rectTransform.anchorMin = new Vector2(0, 0.5f);
            rectTransform.anchorMax = new Vector2(0, 0.5f);
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
            rectTransform.sizeDelta = new Vector2(50, 50); // Размер сердца
            rectTransform.anchoredPosition = new Vector2(i * 60, 0); // Расстояние между сердцами

            heartImages[i] = heartImage;
        }
    }

    private void UpdateHeartUI(int lives)
    {
        Debug.Log($"HeartUI: Обновление отображения жизней: {lives}");

        for (int i = 0; i < heartImages.Length; i++)
        {
            if (heartImages[i] == null) continue;

            if (i < lives)
            {
                // Активное сердце
                heartImages[i].gameObject.SetActive(true);
                if (heartSprite != null)
                {
                    heartImages[i].sprite = heartSprite;
                }
                Debug.Log($"HeartUI: Сердце {i + 1} активировано");
            }
            else
            {
                // Скрываем сердце
                heartImages[i].gameObject.SetActive(false);
                Debug.Log($"HeartUI: Сердце {i + 1} скрыто");
            }
        }
    }
}
