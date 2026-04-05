using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using System.Threading.Tasks;
using Arkanoid.Network;
using Debug = UnityEngine.Debug;

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
    [SerializeField] private TextMeshProUGUI skinStatusText;

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

    void Awake()
    {
        if (selectButton == null)
            selectButton = GetComponent<Button>();

        if (selectButton != null)
            selectButton.onClick.AddListener(OnClicked);

        if (equipButton != null)
            equipButton.onClick.AddListener(OnEquipClicked);
    }

    public void Initialize(ShopController.SkinDto skin, bool isOwned, bool isEquipped, Sprite sprite)
    {
        _skinData = skin;
        _isOwned = isOwned;
        _isEquipped = isEquipped;

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
            priceText.text = $"{_skinData.Price}";
            priceText.color = Color.yellow;
        }

        if (skinStatusText != null)
        {
            if (_isEquipped)
            {
                skinStatusText.text = "ЭКИПИРОВАНО";
                skinStatusText.color = Color.green;
                skinStatusText.gameObject.SetActive(true);
            }
            else if (_isOwned)
            {
                skinStatusText.text = "КУПЛЕНО";
                skinStatusText.color = Color.green;
                skinStatusText.gameObject.SetActive(true);
            }
            else
            {
                skinStatusText.gameObject.SetActive(false);
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
        if (_isOwned)
            EquipSkin();
        else
            PurchaseSkin();
    }

    private void OnEquipClicked()
    {
        EquipSkin();
    }

    // ! Купить скин через API
    // Вызывается из OnClicked когда скин не принадлежит пользователю
    public async void PurchaseSkin()
    {
        if (_skinData == null) return;

        try
        {
            var response = await ShopAPIClient.Instance.PurchaseSkin(_skinData.Id);

            if (response != null && response.Success)
            {
                Debug.Log($"[SkinItemUI] Skin purchased: {_skinData.Name}");

                if (ShopController.Instance != null)
                {
                    ShopController.Instance.UpdateCoins(response.RemainingCoins);

                    if (response.PurchasedSkin != null)
                    {
                        ShopController.Instance.AddOwnedSkin(new ShopController.UserSkinDto
                        {
                            Id = response.PurchasedSkin.Id,
                            SkinId = _skinData.Id,
                            SkinName = _skinData.Name,
                            IsEquipped = false
                        });
                    }

                    ShopController.Instance.RefreshShopUI();
                }

                _isOwned = true;
                UpdateVisuals();
            }
            else
            {
                var errorCode = (PurchaseErrorCode)(response?.ErrorCode ?? 0);

                if (SkinsError.Instance != null)
                {
                    SkinsError.Instance.ShowPurchaseError(errorCode, response?.Message);
                }
                else
                {
                    Debug.LogError($"[SkinItemUI] Purchase failed: {response?.Message ?? GetPurchaseErrorMessage(errorCode)}");
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[SkinItemUI] Purchase error: {e.Message}");
        }
    }

    // ! Экипировать скин через API
    // Вызывается из OnClicked, OnEquipClicked когда скин принадлежит пользователю
    public async void EquipSkin()
    {
        if (_skinData == null) return;

        var userSkin = ShopController.Instance?.GetOwnedSkin(_skinData.Id);
        if (userSkin == null)
        {
            Debug.LogError($"[SkinItemUI] Skin not owned: {_skinData.Id}");
            return;
        }

        try
        {
            var response = await ShopAPIClient.Instance.EquipSkin(userSkin.Id);

            if (response != null && response.Success)
            {
                Debug.Log($"[SkinItemUI] Skin equipped: {userSkin.SkinName}");

                var skin = ShopController.Instance?.GetSkinById(_skinData.Id);

                if (skin != null)
                {
                    ShopController.Instance.UnequipAllOfType(skin.SkinType, _skinData.Id);
                    ShopController.Instance.SetSkinEquipped(_skinData.Id, true);
                }

                if (skin != null && SkinManager.Instance != null)
                {
                    string spriteKey = skin.PrefabPath;
                    SkinManager.Instance.EquipSkin(skin.SkinType, spriteKey);
                    Debug.Log($"[SkinItemUI] Applied skin '{skin.Name}' ({skin.SkinType}), spriteKey='{spriteKey}' via SkinManager");
                }

                _isEquipped = true;
                UpdateVisuals();
                ShopController.Instance.RefreshShopUI();
            }
            else
            {
                var errorCode = (EquipErrorCode)(response?.ErrorCode ?? 0);

                if (SkinsError.Instance != null)
                {
                    SkinsError.Instance.ShowEquipError(errorCode, response?.Message);
                }
                else
                {
                    Debug.LogError($"[SkinItemUI] Equip failed: {response?.Message ?? GetEquipErrorMessage(errorCode)}");
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[SkinItemUI] Equip error: {e.Message}");
        }
    }

    private string GetPurchaseErrorMessage(PurchaseErrorCode code)
    {
        return code switch
        {
            PurchaseErrorCode.AlreadyOwned => "Этот скин уже есть у вас",
            PurchaseErrorCode.InsufficientCoins => "Недостаточно монет",
            PurchaseErrorCode.SkinNotFound => "Скин не найден",
            PurchaseErrorCode.SkinNotAvailable => "Скин недоступен для покупки",
            PurchaseErrorCode.UserNotFound => "Пользователь не найден",
            _ => "Ошибка покупки"
        };
    }

    private string GetEquipErrorMessage(EquipErrorCode code)
    {
        return code switch
        {
            EquipErrorCode.AlreadyEquipped => "Этот скин уже экипирован",
            EquipErrorCode.SkinNotFound => "Скин не найден или не принадлежит вам",
            EquipErrorCode.SkinNotOwned => "Скин не принадлежит вам",
            EquipErrorCode.SkinDataNotFound => "Данные скина не найдены",
            _ => "Ошибка экипировки"
        };
    }

    public void UpdateState(bool isOwned, bool isEquipped)
    {
        _isOwned = isOwned;
        _isEquipped = isEquipped;
        UpdateVisuals();
    }
}
