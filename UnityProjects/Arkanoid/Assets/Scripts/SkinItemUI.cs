using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// UI компонент карточки скина в магазине (Carousel версия).
/// Клик по карточке = покупка или экипировка скина.
/// </summary>
public class SkinItemUI : MonoBehaviour
{
    #region UI References

    [Header("=== Visual Elements ===")]
    [Tooltip("Фон карточки")]
    [SerializeField] private Image background;

    [Tooltip("Изображение скина")]
    [SerializeField] private Image skinIcon;

    [Tooltip("Свечение по редкости")]
    [SerializeField] private Image rarityGlow;

    [Header("=== Text Elements ===")]
    [Tooltip("Название скина")]
    [SerializeField] private TextMeshProUGUI nameText;

    [Tooltip("Тип скина (Платформа/Мяч)")]
    [SerializeField] private TextMeshProUGUI typeText;

    [Tooltip("Цена скина")]
    [SerializeField] private TextMeshProUGUI priceText;

    [Header("=== Indicators ===")]
    [Tooltip("Индикатор: куплено")]
    [SerializeField] private GameObject ownedBadge;

    [Tooltip("Индикатор: экипировано")]
    [SerializeField] private GameObject equipIndicator;

    [Tooltip("Кнопка (для клика)")]
    [SerializeField] private Button selectButton;

    [Header("=== Rarity Colors ===")]
    [SerializeField] private Color commonColor = new Color(0.7f, 0.7f, 0.7f, 1f);
    [SerializeField] private Color uncommonColor = new Color(0.2f, 0.8f, 0.2f, 1f);
    [SerializeField] private Color rareColor = new Color(0.2f, 0.4f, 0.9f, 1f);
    [SerializeField] private Color epicColor = new Color(0.63f, 0.13f, 0.94f, 1f);
    [SerializeField] private Color legendaryColor = new Color(1f, 0.84f, 0f, 1f);

    #endregion

    #region Data

    private ShopController.SkinDto _skinData;
    private bool _isOwned;
    private bool _isEquipped;
    private System.Action<int> _onActionClicked;

    #endregion

    #region Unity Lifecycle

    void Awake()
    {
        if (selectButton == null)
        {
            selectButton = GetComponent<Button>();
        }

        if (selectButton != null)
        {
            selectButton.onClick.AddListener(OnClicked);
        }
    }

    #endregion

    #region Initialization

    /// <summary>
    /// Инициализация карточки данными скина
    /// </summary>
    public void Initialize(ShopController.SkinDto skin, bool isOwned, bool isEquipped, System.Action<int> onActionClicked)
    {
        _skinData = skin;
        _isOwned = isOwned;
        _isEquipped = isEquipped;
        _onActionClicked = onActionClicked;

        UpdateVisuals();
    }

    /// <summary>
    /// Обновление визуальных элементов
    /// </summary>
    private void UpdateVisuals()
    {
        // Название
        if (nameText != null)
            nameText.text = _skinData.Name;

        // Тип
        if (typeText != null)
        {
            typeText.text = _skinData.SkinType switch
            {
                "Platform" => "Платформа",
                "Ball" => "Мяч",
                _ => _skinData.SkinType
            };
        }

        // Цена / Статус
        if (priceText != null)
        {
            if (_isEquipped)
            {
                priceText.text = "ЭКИПИРОВАНО";
                priceText.color = Color.green;
            }
            else if (_isOwned)
            {
                priceText.text = "КУПЛЕНО";
                priceText.color = Color.green;
            }
            else
            {
                priceText.text = $"{_skinData.Price}";
                priceText.color = Color.yellow;
            }
        }

        // Цвет редкости
        SetRarityColor(_skinData.Rarity);

        // Индикаторы
        if (ownedBadge != null)
            ownedBadge.SetActive(_isOwned);

        if (equipIndicator != null)
            equipIndicator.SetActive(_isEquipped);
    }

    /// <summary>
    /// Установка цвета по редкости
    /// </summary>
    private void SetRarityColor(string rarity)
    {
        Color color = rarity switch
        {
            "Common" => commonColor,
            "Uncommon" => uncommonColor,
            "Rare" => rareColor,
            "Epic" => epicColor,
            "Legendary" => legendaryColor,
            _ => Color.white
        };

        if (rarityGlow != null)
        {
            rarityGlow.color = color;
        }

        if (background != null)
        {
            Color bgColor = background.color;
            bgColor.a = 0.3f;
            background.color = bgColor;
        }
    }

    #endregion

    #region Events

    private void OnClicked()
    {
        _onActionClicked?.Invoke(_skinData.Id);
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Обновить состояние (после покупки/экипировки)
    /// </summary>
    public void UpdateState(bool isOwned, bool isEquipped)
    {
        _isOwned = isOwned;
        _isEquipped = isEquipped;
        UpdateVisuals();
    }

    #endregion
}
