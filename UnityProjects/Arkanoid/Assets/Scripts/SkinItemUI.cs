using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkinItemUI : MonoBehaviour
{
    [Header("=== Visual Elements ===")]
    [SerializeField]
    private Image background;

    [SerializeField] private Transform iconContainer;
    [SerializeField] private Image skinImage;
    [SerializeField] private Image rarityGlow;

    [Header("=== Text Elements ===")]
    [SerializeField]
    private TextMeshProUGUI nameText;

    [SerializeField] private TextMeshProUGUI typeText;
    [SerializeField] private TextMeshProUGUI priceText;

    [Header("=== Indicators ===")]
    [SerializeField]
    private GameObject ownedBadge;

    [SerializeField] private GameObject equipIndicator;
    [SerializeField] private Button selectButton;  // BuyButton (для покупки)
    [SerializeField] private Button equipButton;   // EquipButton (для экипировки)

    [Header("=== Rarity Colors ===")]
    [SerializeField]
    private Color commonColor = new Color(0.7f, 0.7f, 0.7f, 1f);

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

        if (equipButton != null)
            equipButton.onClick.AddListener(OnEquipClicked);
    }

    public void Initialize(ShopController.SkinDto skin, bool isOwned, bool isEquipped,
        System.Action<int> onActionClicked, Sprite sprite)
    {
        _skinData = skin;
        _isOwned = isOwned;
        _isEquipped = isEquipped;
        _onActionClicked = onActionClicked;

        UpdateVisuals();
        SetSkinSprite(sprite);
    }

    private void UpdateVisuals()
    {
        if (nameText != null)
        {
            nameText.text = _skinData.Name;
        }

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

        // Показывать/скрывать кнопки в зависимости от состояния
        if (selectButton != null)
            selectButton.gameObject.SetActive(!_isOwned);  // BuyButton видна только для некупленных

        if (equipButton != null)
            equipButton.gameObject.SetActive(_isOwned && !_isEquipped);  // EquipButton для купленных, но не экипированных
    }

    private void SetSkinSprite(Sprite sprite)
    {
        if (skinImage == null)
        {
            // Если skinImage не назначен, создаём Image на iconContainer
            if (iconContainer == null)
            {
                Debug.LogError("[SkinItemUI] iconContainer not assigned and skinImage is null!");
                return;
            }

            GameObject imgObj = new GameObject("SkinImage");
            imgObj.transform.SetParent(iconContainer, false);
            skinImage = imgObj.AddComponent<Image>();

            RectTransform rt = imgObj.GetComponent<RectTransform>();
            if (rt != null)
            {
                rt.anchorMin = Vector2.zero;
                rt.anchorMax = Vector2.one;
                rt.offsetMin = Vector2.zero;
                rt.offsetMax = Vector2.zero;
            }
        }

        if (sprite != null)
        {
            skinImage.sprite = sprite;
            skinImage.preserveAspect = true;
            skinImage.type = Image.Type.Simple;
            skinImage.enabled = true;
            Debug.Log($"[SkinItemUI] Set sprite: {sprite.name}");
        }
        else
        {
            Debug.LogWarning($"[SkinItemUI] No sprite provided for skin: {_skinData?.Name}");
        }
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

    private void OnEquipClicked()
    {
        // EquipButton вызывает ту же логику, что и OnClicked для owned скина
        _onActionClicked?.Invoke(_skinData.Id);
    }

    public void UpdateState(bool isOwned, bool isEquipped)
    {
        _isOwned = isOwned;
        _isEquipped = isEquipped;
        UpdateVisuals();
    }
}
