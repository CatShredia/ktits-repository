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
    public Image[] heartImages = new Image[3];

    [Header("Appearance")]
    [Tooltip("Спрайт сердца")]
    public Sprite heartSprite;

    void Start()
    {
        ValidateReferences();

        GameManager.Instance.OnLivesChanged += UpdateHeartUI;

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
        for (int i = 0; i < heartImages.Length; i++)
        {
            if (heartImages[i] == null)
                Debug.LogWarning($"HeartUI: Сердце {i} не назначено!");
        }

        if (heartSprite == null)
            Debug.LogWarning("HeartUI: Спрайт сердца не назначен!");
    }

    private void UpdateHeartUI(int lives)
    {
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
            }
            else
            {
                // Скрываем сердце
                heartImages[i].gameObject.SetActive(false);
            }
        }
    }
}
