using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Debug = UnityEngine.Debug;
using Arkanoid.Network;
using Arkanoid.Auth;

public class ShopController : MonoBehaviour
{
    public static ShopController Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    [Header("=== Panels ===")]
    [SerializeField] private GameObject shopPanel;
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject gameplayUI;

    [Header("=== Game Objects ===")]
    [SerializeField] private GameObject playerPlatform;
    [SerializeField] private GameObject ball;

    [Header("=== Shop Header ===")]
    [SerializeField] private TextMeshProUGUI coinsDisplayText;
    [SerializeField] private Button backButton;

    [Header("=== Carousel ===")]
    [SerializeField] private RectTransform skinContainer;
    [SerializeField] private Button prevButton;
    [SerializeField] private Button nextButton;
    [SerializeField] private GameObject skinItemPrefab;

    [Header("=== Animations ===")]
    [SerializeField] private Animator shopAnimator;
    [SerializeField] private string openParameterName = "Open";
    [SerializeField] private float showAnimationDuration = 0.3f;
    [SerializeField] private float hideAnimationDuration = 0.3f;

    private bool isShopOpen;
    private bool isAnimating;
    private int currentIndex;
    private float lastClickTime;
    private const float CLICK_COOLDOWN = 0.2f; // Защита от повторных нажатий
    private List<SkinDto> allSkins = new();
    private UserInventoryDto? userInventory;
    private SkinDto? currentSkin;
    private bool isCurrentSkinOwned;
    private bool isCurrentSkinEquipped;

    void Start() => InitializeUI();
    void Update()
    {
        HandleEscapeKey();
        HandleArrowKeys();
    }

    private void InitializeUI()
    {
        if (shopPanel != null) shopPanel.SetActive(false);

        if (backButton != null) backButton.onClick.AddListener(OnBackButtonClicked);
        if (prevButton != null) prevButton.onClick.AddListener(OnPrevButtonClicked);
        if (nextButton != null) nextButton.onClick.AddListener(OnNextButtonClicked);

        Debug.Log("[ShopController] UI initialized (Carousel mode)");
    }

    public void OpenShopFromMenu()
    {
        if (isShopOpen || isAnimating) return;
        StartCoroutine(OpenShopSequence(true));
    }

    public void OpenShopFromPause()
    {
        if (isShopOpen || isAnimating) return;
        if (pausePanel != null) pausePanel.SetActive(false);
        StartCoroutine(OpenShopSequence(false));
    }

    public void CloseShop()
    {
        if (!isShopOpen || isAnimating) return;
        StartCoroutine(HideShopSequence());
    }

    private IEnumerator OpenShopSequence(bool fromMainMenu)
    {
        isAnimating = true;

        if (fromMainMenu && mainMenuPanel != null) mainMenuPanel.SetActive(false);
        if (gameplayUI != null) gameplayUI.SetActive(false);
        if (playerPlatform != null) playerPlatform.SetActive(false);
        if (ball != null) ball.SetActive(false);
        if (shopPanel != null) shopPanel.SetActive(true);
        if (shopAnimator != null) shopAnimator.SetBool(openParameterName, true);

        yield return new WaitForSecondsRealtime(showAnimationDuration);

        isShopOpen = true;
        isAnimating = false;
        _ = LoadShopData();
        Debug.Log("[ShopController] Shop opened");
    }

    private IEnumerator HideShopSequence()
    {
        isAnimating = true;

        if (shopAnimator != null) shopAnimator.SetBool(openParameterName, false);

        yield return new WaitForSecondsRealtime(hideAnimationDuration);

        if (shopPanel != null) shopPanel.SetActive(false);
        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
        if (playerPlatform != null) playerPlatform.SetActive(true);
        if (ball != null) ball.SetActive(true);

        isShopOpen = false;
        isAnimating = false;
        Debug.Log("[ShopController] Shop closed");
    }

    private void HandleEscapeKey()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && isShopOpen && !isAnimating)
            CloseShop();
    }

    private void HandleArrowKeys()
    {
        if (!isShopOpen || isAnimating) return;

        if (Input.GetKeyDown(KeyCode.LeftArrow))
            OnPrevButtonClicked();
        else if (Input.GetKeyDown(KeyCode.RightArrow))
            OnNextButtonClicked();
    }

    private async Task LoadShopData()
    {
        if (ShopAPIClient.Instance == null)
        {
            Debug.LogError("[ShopController] ShopAPIClient.Instance is null.");
            LoadTestData();
            UpdateCoinsDisplay();
            ShowCurrentSkin();
            return;
        }

        string token = AuthService.Instance?.AuthToken;
        if (!string.IsNullOrEmpty(token))
            ShopAPIClient.Instance.SetAuthToken(token);

        var skins = await ShopAPIClient.Instance.GetAllSkins();

        if (skins != null && skins.Count > 0)
        {
            allSkins = skins.Select(s => new SkinDto
            {
                Id = s.Id,
                Name = s.Name,
                SkinType = s.SkinType,
                Price = s.Price,
                Rarity = s.Rarity,
                Description = s.Description ?? "Нет описания",
                TexturePath = s.TexturePath
            }).ToList();

            if (!string.IsNullOrEmpty(token))
            {
                var inventory = await ShopAPIClient.Instance.GetInventory();
                if (inventory != null)
                {
                    userInventory = new UserInventoryDto
                    {
                        Coins = inventory.Coins,
                        Skins = inventory.Skins?.Select(s => new UserSkinDto
                        {
                            Id = s.Id,
                            SkinId = s.SkinId,
                            SkinName = s.SkinName,
                            IsEquipped = s.IsEquipped
                        }).ToList() ?? new List<UserSkinDto>()
                    };
                    Debug.Log($"[ShopController] Loaded inventory: {userInventory.Coins} coins, {userInventory.Skins.Count} skins");
                }
            }
            else
            {
                Debug.LogWarning("[ShopController] No auth token, using demo inventory");
                userInventory = new UserInventoryDto
                {
                    Coins = 1000,
                    Skins = new List<UserSkinDto>
                    {
                        new UserSkinDto { Id = 1, SkinId = 1, SkinName = "Неоновая платформа", IsEquipped = false }
                    }
                };
            }

            Debug.Log($"[ShopController] Loaded {allSkins.Count} skins from API");
        }
        else
        {
            Debug.LogWarning("[ShopController] API returned no skins, using test data");
            LoadTestData();
        }

        UpdateCoinsDisplay();
        ShowCurrentSkin();
    }

    private void LoadTestData()
    {
        allSkins = new List<SkinDto>
        {
            new SkinDto { Id = 1, Name = "Неоновая платформа", SkinType = "Platform", Price = 100, Rarity = "Common", TexturePath = "Assets/Sprites/Skins/neon_platform" },
            new SkinDto { Id = 2, Name = "Золотой мяч", SkinType = "Ball", Price = 150, Rarity = "Rare", TexturePath = "Assets/Sprites/Skins/gold_ball" },
            new SkinDto { Id = 3, Name = "Огненная платформа", SkinType = "Platform", Price = 200, Rarity = "Epic", TexturePath = "Assets/Sprites/Skins/fire_platform" },
        };

        userInventory = new UserInventoryDto
        {
            Coins = 500,
            Skins = new List<UserSkinDto>
            {
                new UserSkinDto { Id = 1, SkinId = 1, SkinName = "Неоновая платформа", IsEquipped = false }
            }
        };
    }

    private void UpdateCoinsDisplay()
    {
        if (coinsDisplayText == null) return;
        coinsDisplayText.text = $"{userInventory?.Coins ?? 0}";
    }

    private void ShowCurrentSkin()
    {
        if (allSkins.Count == 0 || skinContainer == null || skinItemPrefab == null)
        {
            Debug.LogError("[ShopController] Cannot show skin: missing data or UI references");
            return;
        }

        foreach (Transform child in skinContainer)
            Destroy(child.gameObject);

        currentSkin = allSkins[currentIndex];
        isCurrentSkinOwned = userInventory?.Skins?.Any(s => s.SkinId == currentSkin.Id) ?? false;
        isCurrentSkinEquipped = userInventory?.Skins?.Any(s => s.SkinId == currentSkin.Id && s.IsEquipped) ?? false;

        var item = Instantiate(skinItemPrefab, skinContainer, false);

        var rectTransform = item.GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            rectTransform.localPosition = Vector3.zero;
            rectTransform.localScale = Vector3.one;
        }

        var skinItemUI = item.GetComponent<SkinItemUI>();
        if (skinItemUI != null)
        {
            skinItemUI.Initialize(currentSkin, isCurrentSkinOwned, isCurrentSkinEquipped, OnSkinActionClicked);
        }
        else
        {
            Debug.LogError("[ShopController] SkinItemUI component not found on instantiated prefab!");
        }

        UpdateCarouselButtons();
    }

    private void OnSkinActionClicked(int skinId)
    {
        if (skinId != currentSkin?.Id) return;
        if (isCurrentSkinOwned)
            EquipSkin(skinId);
        else
            PurchaseSkin(skinId);
    }

    public void OnPrevButtonClicked()
    {
        // Защита от повторных нажатий
        if (Time.time - lastClickTime < CLICK_COOLDOWN)
        {
            Debug.Log($"[ShopController] Prev click ignored (cooldown)");
            return;
        }
        lastClickTime = Time.time;

        Debug.Log($"[ShopController] OnPrevButtonClicked вызван! allSkins.Count={allSkins.Count}, currentIndex={currentIndex}");

        if (allSkins.Count == 0)
        {
            Debug.LogWarning("[ShopController] Нет скинов для навигации");
            return;
        }

        currentIndex--;
        if (currentIndex < 0)
            currentIndex = allSkins.Count - 1; // Циклический переход на последний элемент

        Debug.Log($"[ShopController] Prev clicked: newIndex={currentIndex}");
        ShowCurrentSkin();
    }

    public void OnNextButtonClicked()
    {
        // Защита от повторных нажатий
        if (Time.time - lastClickTime < CLICK_COOLDOWN)
        {
            Debug.Log($"[ShopController] Next click ignored (cooldown)");
            return;
        }
        lastClickTime = Time.time;

        if (allSkins.Count == 0) return;

        currentIndex++;
        if (currentIndex >= allSkins.Count)
            currentIndex = 0; // Циклический переход на первый элемент

        Debug.Log($"[ShopController] Next clicked: newIndex={currentIndex}");
        ShowCurrentSkin();
    }

    private void UpdateCarouselButtons()
    {
        // При циклической навигации кнопки всегда активны, если есть скины
        bool hasSkins = allSkins.Count > 0;

        if (prevButton != null)
            prevButton.interactable = hasSkins;

        if (nextButton != null)
            nextButton.interactable = hasSkins;

        Debug.Log($"[ShopController] Buttons updated: prev={prevButton?.interactable}, next={nextButton?.interactable}, total={allSkins.Count}");
    }

    private void OnBackButtonClicked() => CloseShop();

    private async void PurchaseSkin(int skinId)
    {
        var skin = allSkins.Find(s => s.Id == skinId);
        if (skin == null) return;

        int coins = userInventory?.Coins ?? 0;
        if (coins < skin.Price)
        {
            Debug.LogWarning($"[ShopController] Недостаточно монет: {coins} < {skin.Price}");
            return;
        }

        try
        {
            var response = await ShopAPIClient.Instance.PurchaseSkin(skinId);

            if (response != null && response.Success)
            {
                Debug.Log($"[ShopController] Skin purchased: {skin.Name}");
                userInventory.Coins = response.RemainingCoins;

                if (response.PurchasedSkin != null)
                {
                    userInventory.Skins.Add(new UserSkinDto
                    {
                        Id = response.PurchasedSkin.Id,
                        SkinId = skinId,
                        SkinName = skin.Name,
                        IsEquipped = false
                    });
                }

                UpdateCoinsDisplay();
                ShowCurrentSkin();
            }
            else
            {
                Debug.LogError($"[ShopController] Purchase failed: {response?.Message ?? "Неизвестная ошибка"}");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[ShopController] Purchase error: {e.Message}");
        }
    }

    private async void EquipSkin(int skinId)
    {
        var userSkin = userInventory?.Skins?.FirstOrDefault(s => s.SkinId == skinId);
        if (userSkin == null)
        {
            Debug.LogError($"[ShopController] Skin not owned: {skinId}");
            return;
        }

        try
        {
            var response = await ShopAPIClient.Instance.EquipSkin(userSkin.Id);

            if (response != null && response.Success)
            {
                Debug.Log($"[ShopController] Skin equipped: {userSkin.SkinName}");

                if (userInventory?.Skins != null)
                {
                    var skin = allSkins.Find(s => s.Id == skinId);
                    if (skin != null)
                    {
                        foreach (var us in userInventory.Skins)
                        {
                            var s = allSkins.Find(sk => sk.Id == us.SkinId);
                            if (s?.SkinType == skin.SkinType)
                                us.IsEquipped = false;
                        }
                    }
                    userSkin.IsEquipped = true;
                }

                ShowCurrentSkin();
            }
            else
            {
                Debug.LogError($"[ShopController] Equip failed: {response?.Message ?? "Неизвестная ошибка"}");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[ShopController] Equip error: {e.Message}");
        }
    }

    [System.Serializable]
    public class SkinDto
    {
        public int Id;
        public string Name;
        public string SkinType;
        public int Price;
        public string Rarity;
        public string Description;
        public string TexturePath;
    }

    [System.Serializable]
    public class UserSkinDto
    {
        public int Id;
        public int SkinId;
        public string SkinName;
        public bool IsEquipped;
    }

    [System.Serializable]
    public class UserInventoryDto
    {
        public int Coins;
        public List<UserSkinDto> Skins;
    }
}
