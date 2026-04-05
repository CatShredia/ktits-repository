using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
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
    [SerializeField] private GameObject coinsDisplayContainer;
    [SerializeField] private Button backButton;

    [Header("=== Carousel ===")]
    [SerializeField] private RectTransform skinContainer;
    [SerializeField] private Button prevButton;
    [SerializeField] private Button nextButton;
    [SerializeField] private GameObject skinItemPrefab;

    [Header("=== Tabs ===")]
    [SerializeField] private Button platformTabButton;
    [SerializeField] private Button ballTabButton;

    [Header("=== Animations ===")]
    [SerializeField] private Animator shopAnimator;
    [SerializeField] private string openParameterName = "Open";
    [SerializeField] private float showAnimationDuration = 0.3f;
    [SerializeField] private float hideAnimationDuration = 0.3f;

    [Header("=== Skin Sprites (название из БД → Sprite) ===")]
    [SerializeField] private SkinSpriteEntry[] skinSprites;
    private Dictionary<string, Sprite> _skinSpriteLookup;

    private bool isShopOpen;
    private bool isAnimating;
    private int currentIndex;
    private float lastClickTime;
    private const float CLICK_COOLDOWN = 0.2f;
    private string selectedSkinType = "Platform"; // по умолчанию Platform
    private List<SkinDto> allSkins = new();
    private List<SkinDto> filteredSkins = new();
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

        if (platformTabButton != null) platformTabButton.onClick.AddListener(OnPlatformTabClicked);
        if (ballTabButton != null) ballTabButton.onClick.AddListener(OnBallTabClicked);

        // Построить словарь для быстрого поиска спрайтов по имени
        _skinSpriteLookup = new Dictionary<string, Sprite>();
        if (skinSprites != null)
        {
            foreach (var entry in skinSprites)
            {
                if (!string.IsNullOrEmpty(entry.spriteName) && entry.sprite != null)
                {
                    _skinSpriteLookup[entry.spriteName] = entry.sprite;
                }
            }
        }

        Debug.Log($"[ShopController] UI initialized (Carousel mode). Registered {_skinSpriteLookup.Count} skin sprites.");
        UpdateShopHeaderVisibility();
        UpdateTabAppearance();
    }

    private void UpdateShopHeaderVisibility()
    {
        bool isAuthenticated = AuthService.Instance?.IsAuthenticated() ?? false;

        // Показываем/скрываем контейнер монет
        if (coinsDisplayContainer != null)
            coinsDisplayContainer.SetActive(isAuthenticated);

        Debug.Log($"[ShopController] Shop header: auth={isAuthenticated}, coinsDisplay={(coinsDisplayContainer?.activeSelf == true ? "VISIBLE" : "HIDDEN")}");
    }

    private void UpdateTabAppearance()
    {
        // Активный таб — 100% непрозрачность, неактивный — 50%
        if (platformTabButton != null)
        {
            var image = platformTabButton.GetComponent<Image>();
            if (image != null)
            {
                var color = image.color;
                color.a = selectedSkinType == "Platform" ? 1f : 0.5f;
                image.color = color;
            }
        }

        if (ballTabButton != null)
        {
            var image = ballTabButton.GetComponent<Image>();
            if (image != null)
            {
                var color = image.color;
                color.a = selectedSkinType == "Ball" ? 1f : 0.5f;
                image.color = color;
            }
        }
    }

    private void ApplySkinTypeFilter()
    {
        filteredSkins = allSkins.Where(s => s.SkinType == selectedSkinType).ToList();
        Debug.Log($"[ShopController] Filter applied: type='{selectedSkinType}', {filteredSkins.Count} skins (out of {allSkins.Count} total)");
        for (int i = 0; i < filteredSkins.Count; i++)
        {
            Debug.Log($"[ShopController]   FilteredSkin[{i}]: Id={filteredSkins[i].Id}, Name='{filteredSkins[i].Name}'");
        }
    }

    private void OnPlatformTabClicked()
    {
        if (selectedSkinType == "Platform") return;
        selectedSkinType = "Platform";
        currentIndex = 0;
        UpdateTabAppearance();
        ApplySkinTypeFilter();
        ShowCurrentSkin();
        Debug.Log($"[ShopController] Platform tab selected, showing {filteredSkins.Count} platform skins");
    }

    private void OnBallTabClicked()
    {
        if (selectedSkinType == "Ball") return;
        selectedSkinType = "Ball";
        currentIndex = 0;
        UpdateTabAppearance();
        ApplySkinTypeFilter();
        ShowCurrentSkin();
        Debug.Log($"[ShopController] Ball tab selected, showing {filteredSkins.Count} ball skins");
    }

    // ! Вызывается после изменения состояния авторизации (вход/выход)
    // Вызывается из AuthUIController
    public void RefreshAuthState()
    {
        UpdateShopHeaderVisibility();
        UpdateCoinsDisplay();
    }

    // ! Возвращает, открыт ли сейчас магазин
    public bool IsShopOpen => isShopOpen;

    // ! Перезагружает данные магазина (инвентарь и скины) без перезапуска анимации
    // Вызывается из AuthUIController после входа пользователя
    public void ReloadShopData()
    {
        _ = LoadShopData();
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

    public void CloseShop(bool showMainMenu = true)
    {
        if (!isShopOpen || isAnimating) return;
        StartCoroutine(HideShopSequence(showMainMenu));
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

    private IEnumerator HideShopSequence(bool showMainMenu)
    {
        isAnimating = true;

        if (shopAnimator != null) shopAnimator.SetBool(openParameterName, false);

        yield return new WaitForSecondsRealtime(hideAnimationDuration);

        if (shopPanel != null) shopPanel.SetActive(false);
        if (showMainMenu && mainMenuPanel != null) mainMenuPanel.SetActive(true);
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
        Debug.Log("[ShopController] === LoadShopData START ===");

        if (ShopAPIClient.Instance == null)
        {
            Debug.LogError("[ShopController] ShopAPIClient.Instance is null.");
            LoadTestData();
            UpdateCoinsDisplay();
            ShowCurrentSkin();
            return;
        }

        string token = AuthService.Instance?.AuthToken;
        Debug.Log($"[ShopController] Auth token: {(string.IsNullOrEmpty(token) ? "NULL/EMPTY" : $"present (len={token.Length})")}");

        if (!string.IsNullOrEmpty(token))
            ShopAPIClient.Instance.SetAuthToken(token);
        else
            Debug.LogWarning("[ShopController] No auth token found in AuthService.Instance");

        Debug.Log("[ShopController] Calling GetAllSkins...");
        var skins = await ShopAPIClient.Instance.GetAllSkins();
        Debug.Log($"[ShopController] GetAllSkins returned: {skins?.Count ?? 0} skins");

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
                TexturePath = s.TexturePath,
                PrefabPath = s.PrefabPath
            }).ToList();

            Debug.Log($"[ShopController] Mapped {allSkins.Count} skins to DTOs");
            for (int i = 0; i < allSkins.Count; i++)
            {
                Debug.Log($"[ShopController]   Skin[{i}]: Id={allSkins[i].Id}, Name='{allSkins[i].Name}', PrefabPath='{allSkins[i].PrefabPath}'");
            }

            // Зарегистрировать маппинг Name → PrefabPath в SkinManager для разрешения старых PlayerPrefs
            if (SkinManager.Instance != null)
            {
                foreach (var skin in allSkins)
                {
                    SkinManager.Instance.RegisterSkinNameMapping(skin.Name, skin.PrefabPath);
                }
                Debug.Log($"[ShopController] Registered {allSkins.Count} skin name mappings in SkinManager");
            }

            if (!string.IsNullOrEmpty(token))
            {
                Debug.Log("[ShopController] Calling GetInventory...");
                var inventory = await ShopAPIClient.Instance.GetInventory();
                Debug.Log($"[ShopController] GetInventory returned: {(inventory == null ? "NULL" : $"Coins={inventory.Coins}, Skins={inventory.Skins?.Count ?? 0}")}");

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
                    for (int i = 0; i < userInventory.Skins.Count; i++)
                    {
                        Debug.Log($"[ShopController]   OwnedSkin[{i}]: Id={userInventory.Skins[i].Id}, SkinId={userInventory.Skins[i].SkinId}, Name='{userInventory.Skins[i].SkinName}', Equipped={userInventory.Skins[i].IsEquipped}");
                    }
                }
                else
                {
                    Debug.LogWarning("[ShopController] GetInventory returned NULL, coins will show 0");
                    userInventory = new UserInventoryDto
                    {
                        Coins = 0,
                        Skins = new List<UserSkinDto>()
                    };
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

            // Применить фильтр по типу
            ApplySkinTypeFilter();
        }
        else
        {
            Debug.LogWarning("[ShopController] API returned no skins, using test data");
            LoadTestData();
            ApplySkinTypeFilter();
        }

        Debug.Log($"[ShopController] UpdateCoinsDisplay: coins={userInventory?.Coins ?? 0}");
        UpdateCoinsDisplay();
        UpdateShopHeaderVisibility();
        Debug.Log("[ShopController] ShowCurrentSkin");
        ShowCurrentSkin();
        Debug.Log("[ShopController] === LoadShopData END ===");
    }

    private void LoadTestData()
    {
        allSkins = new List<SkinDto>
        {
            new SkinDto { Id = 1, Name = "Неоновая платформа", SkinType = "Platform", Price = 100, Rarity = "Common", TexturePath = "Assets/Sprites/Skins/neon_platform", PrefabPath = "Assets/Prefabs/Skins/neon_platform" },
            new SkinDto { Id = 2, Name = "Золотой мяч", SkinType = "Ball", Price = 150, Rarity = "Rare", TexturePath = "Assets/Sprites/Skins/gold_ball", PrefabPath = "Assets/Prefabs/Skins/gold_ball" },
            new SkinDto { Id = 3, Name = "Огненная платформа", SkinType = "Platform", Price = 200, Rarity = "Epic", TexturePath = "Assets/Sprites/Skins/fire_platform", PrefabPath = "Assets/Prefabs/Skins/fire_platform" },
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
        if (filteredSkins.Count == 0 || skinContainer == null || skinItemPrefab == null)
        {
            Debug.LogError($"[ShopController] Cannot show skin: filteredSkins.Count={filteredSkins.Count}, missing UI references");
            return;
        }

        foreach (Transform child in skinContainer)
            Destroy(child.gameObject);

        currentSkin = filteredSkins[currentIndex];
        isCurrentSkinOwned = userInventory?.Skins?.Any(s => s.SkinId == currentSkin.Id) ?? false;
        isCurrentSkinEquipped = userInventory?.Skins?.Any(s => s.SkinId == currentSkin.Id && s.IsEquipped) ?? false;

        var item = Instantiate(skinItemPrefab, skinContainer, false);

        var rectTransform = item.GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            rectTransform.localPosition = Vector3.zero;
            rectTransform.localScale = Vector3.one;
        }

        // Найти спрайт по имени из БД (PrefabPath содержит название спрайта)
        Sprite sprite = null;
        string spriteName = currentSkin.PrefabPath;
        if (!string.IsNullOrEmpty(spriteName) && _skinSpriteLookup != null)
        {
            _skinSpriteLookup.TryGetValue(spriteName, out sprite);
        }

        if (sprite == null)
        {
            Debug.LogWarning($"[ShopController] Sprite not found for skin '{currentSkin.Name}', spriteName='{spriteName}'");
        }

        var skinItemUI = item.GetComponent<SkinItemUI>();
        if (skinItemUI != null)
        {
            skinItemUI.Initialize(currentSkin, isCurrentSkinOwned, isCurrentSkinEquipped, sprite);
        }
        else
        {
            Debug.LogError("[ShopController] SkinItemUI component not found on instantiated prefab!");
        }

        UpdateCarouselButtons();
    }

    public void OnPrevButtonClicked()
    {
        if (Time.time - lastClickTime < CLICK_COOLDOWN)
        {
            Debug.Log($"[ShopController] Prev click ignored (cooldown)");
            return;
        }
        lastClickTime = Time.time;

        if (filteredSkins.Count == 0)
        {
            Debug.LogWarning("[ShopController] Нет скинов для навигации");
            return;
        }

        currentIndex--;
        if (currentIndex < 0)
            currentIndex = filteredSkins.Count - 1;

        Debug.Log($"[ShopController] Prev clicked: newIndex={currentIndex}");
        ShowCurrentSkin();
    }

    public void OnNextButtonClicked()
    {
        if (Time.time - lastClickTime < CLICK_COOLDOWN)
        {
            Debug.Log($"[ShopController] Next click ignored (cooldown)");
            return;
        }
        lastClickTime = Time.time;

        if (filteredSkins.Count == 0) return;

        currentIndex++;
        if (currentIndex >= filteredSkins.Count)
            currentIndex = 0;

        Debug.Log($"[ShopController] Next clicked: newIndex={currentIndex}");
        ShowCurrentSkin();
    }

    private void UpdateCarouselButtons()
    {
        bool hasSkins = filteredSkins.Count > 0;

        if (prevButton != null)
            prevButton.interactable = hasSkins;

        if (nextButton != null)
            nextButton.interactable = hasSkins;

        Debug.Log($"[ShopController] Buttons updated: prev={prevButton?.interactable}, next={nextButton?.interactable}, total={filteredSkins.Count}");
    }

    private void OnBackButtonClicked() => CloseShop();

    // ! Обновить монеты в инвентаре
    // Вызывается из SkinItemUI после покупки
    public void UpdateCoins(int coins)
    {
        if (userInventory != null)
        {
            userInventory.Coins = coins;
        }
    }

    // ! Добавить купленный скин в инвентарь
    // Вызывается из SkinItemUI после покупки
    public void AddOwnedSkin(UserSkinDto skin)
    {
        if (userInventory != null)
        {
            userInventory.Skins.Add(skin);
        }
    }

    // ! Получить скин из инвентаря по skinId
    // Вызывается из SkinItemUI.EquipSkin
    public UserSkinDto? GetOwnedSkin(int skinId)
    {
        return userInventory?.Skins?.FirstOrDefault(s => s.SkinId == skinId);
    }

    // ! Получить все данные скина по ID
    // Вызывается из SkinItemUI.EquipSkin
    public SkinDto GetSkinById(int skinId)
    {
        return allSkins.Find(s => s.Id == skinId);
    }

    // ! Снять экипировку со всех скинов указанного типа, кроме указанного
    // Вызывается из SkinItemUI.EquipSkin
    public void UnequipAllOfType(string skinType, int exceptSkinId)
    {
        if (userInventory?.Skins == null) return;

        foreach (var us in userInventory.Skins)
        {
            var s = allSkins.Find(sk => sk.Id == us.SkinId);
            if (s?.SkinType == skinType && us.SkinId != exceptSkinId)
                us.IsEquipped = false;
        }
    }

    // ! Установить флаг экипировки для скина
    // Вызывается из SkinItemUI.EquipSkin
    public void SetSkinEquipped(int skinId, bool equipped)
    {
        var userSkin = userInventory?.Skins?.FirstOrDefault(s => s.SkinId == skinId);
        if (userSkin != null)
            userSkin.IsEquipped = equipped;
    }

    // ! Обновить UI магазина после изменения инвентаря
    // Вызывается из SkinItemUI после покупки/экипировки
    public void RefreshShopUI()
    {
        UpdateCoinsDisplay();
        UpdateShopHeaderVisibility();
        ShowCurrentSkin();
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
        public string PrefabPath;
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

    [System.Serializable]
    public class SkinSpriteEntry
    {
        public string spriteName;
        public Sprite sprite;
    }
}
