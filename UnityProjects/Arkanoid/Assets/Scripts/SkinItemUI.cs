using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkinItemUI : MonoBehaviour
{
    [Header("=== Visual Elements ===")]
    [SerializeField] private Image background;
    [SerializeField] private Image skinIcon;
    [SerializeField] private Image rarityGlow;

    [Header("=== Text Elements ===")]
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI typeText;
    [SerializeField] private TextMeshProUGUI priceText;

    [Header("=== Indicators ===")]
    [SerializeField] private GameObject ownedBadge;
    [SerializeField] private GameObject equipIndicator;
    [SerializeField] private Button selectButton;

    [Header("=== Rarity Colors ===")]
    [SerializeField] private Color commonColor = new Color(0.7f, 0.7f, 0.7f, 1f);
    [SerializeField] private Color uncommonColor = new Color(0.2f, 0.8f, 0.2f, 1f);
    [SerializeField] private Color rareColor = new Color(0.2f, 0.4f, 0.9f, 1f);
    [SerializeField] private Color epicColor = new Color(0.63f, 0.13f, 0.94f, 1f);
    [SerializeField] private Color legendaryColor = new Color(1f, 0.84f, 0f, 1f);

    private ShopController.SkinDto _skinData;
    private bool _isOwned;
    private bool _isEquipped;
    private System.Action<int> _onActionClicked;

    void Awake()
    {
        if (selectButton == null)
            selectButton = GetComponent<Button>();

        if (selectButton != null)
            selectButton.onClick.AddListener(OnClicked);
    }

    public void Initialize(ShopController.SkinDto skin, bool isOwned, bool isEquipped, System.Action<int> onActionClicked)
    {
        _skinData = skin;
        _isOwned = isOwned;
        _isEquipped = isEquipped;
        _onActionClicked = onActionClicked;

        UpdateVisuals();
        LoadSkinIcon();
    }

    private void UpdateVisuals()
    {
        if (nameText != null)
            nameText.text = _skinData.Name;

        if (typeText != null)
        {
            typeText.text = _skinData.SkinType switch
            {
                "Platform" => "Платформа",
                "Ball" => "Мяч",
                _ => _skinData.SkinType
            };
        }

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

        SetRarityColor(_skinData.Rarity);

        if (ownedBadge != null)
            ownedBadge.SetActive(_isOwned);

        if (equipIndicator != null)
            equipIndicator.SetActive(_isEquipped);
    }

    private void LoadSkinIcon()
    {
        if (skinIcon == null || string.IsNullOrEmpty(_skinData.TexturePath))
            return;

        // Загружаем спрайт по пути из БД (путь от Assets/)
        Sprite sprite = Resources.Load<Sprite>(_skinData.TexturePath);

        if (sprite == null)
        {
            // Пробуем загрузить через AssetDatabase (для редактора)
#if UNITY_EDITOR
            sprite = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>(_skinData.TexturePath);
#endif
        }

        if (sprite != null)
            skinIcon.sprite = sprite;
        else
            Debug.LogWarning($"[SkinItemUI] Не удалось загрузить спрайт: {_skinData.TexturePath}");
    }

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
            rarityGlow.color = color;

        if (background != null)
        {
            Color bgColor = background.color;
            bgColor.a = 0.3f;
            background.color = bgColor;
        }
    }

    private void OnClicked()
    {
        _onActionClicked?.Invoke(_skinData.Id);
    }

    public void UpdateState(bool isOwned, bool isEquipped)
    {
        _isOwned = isOwned;
        _isEquipped = isEquipped;
        UpdateVisuals();
    }
}
