using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI-индикатор звёзд. Обновляет прозрачность 3 звёзд: 25% (не собраны) → 100% (собраны).
/// Вешается на родительский объект, содержащий 3 Image-компонента (звёзды).
/// </summary>
public class StarUI : MonoBehaviour
{
    [Header("Star Images")]
    [Tooltip("Image-компоненты 3 звёзд (по порядку: 0, 1, 2)")]
    public Image[] starImages = new Image[3];

    [Header("Appearance")]
    [Range(0f, 1f)]
    [Tooltip("Прозрачность несобранной звезды")]
    public float uncollectedAlpha = 0.25f;

    [Range(0f, 1f)]
    [Tooltip("Прозрачность собранной звезды")]
    public float collectedAlpha = 1f;

    void Start()
    {
        ValidateReferences();

        LevelManager.Instance.OnStarCollected += UpdateStarUI;
        LevelManager.Instance.OnLevelChanged += ResetStarUI;

        // Инициализация при старте
        ResetStarUI();
    }

    void OnDestroy()
    {
        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.OnStarCollected -= UpdateStarUI;
            LevelManager.Instance.OnLevelChanged -= ResetStarUI;
        }
    }

    private void ValidateReferences()
    {
        for (int i = 0; i < starImages.Length; i++)
        {
            if (starImages[i] == null)
                Debug.LogWarning($"StarUI: Звезда {i} не назначена!");
        }
    }

    private void UpdateStarUI(int starIndex)
    {
        if (starIndex < 0 || starIndex >= starImages.Length) return;
        SetStarVisual(starIndex, true);
    }

    private void ResetStarUI()
    {
        for (int i = 0; i < starImages.Length; i++)
        {
            SetStarVisual(i, false);
        }
    }

    private void SetStarVisual(int index, bool collected)
    {
        if (starImages[index] == null) return;

        Color color = starImages[index].color;
        color.a = collected ? collectedAlpha : uncollectedAlpha;
        starImages[index].color = color;
    }
}
