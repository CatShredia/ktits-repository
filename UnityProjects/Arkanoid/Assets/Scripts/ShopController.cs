using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

/// <summary>
/// Основной контроллер магазина.
/// Управляет UI элементами, навигацией и состояниями магазина.
/// </summary>
public class ShopController : MonoBehaviour
{
    #region Singleton

    public static ShopController Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    #endregion

    #region UI References

    [Header("=== Panels ===")]
    [Tooltip("Основная панель магазина")]
    [SerializeField] private GameObject shopPanel;

    [Tooltip("Панель главного меню")]
    [SerializeField] private GameObject mainMenuPanel;

    [Tooltip("Панель паузы")]
    [SerializeField] private GameObject pausePanel;

    [Tooltip("Панель игрового процесса")]
    [SerializeField] private GameObject gameplayUI;

    [Header("=== Shop Header ===")]
    [Tooltip("Отображение монет (текст)")]
    [SerializeField] private TextMeshProUGUI coinsDisplayText;

    [Tooltip("Контейнер для отображения монет")]
    [SerializeField] private GameObject coinsDisplay;

    [Tooltip("Кнопка назад")]
    [SerializeField] private Button backButton;

    [Header("=== Shop Content ===")]
    [Tooltip("Panel для сетки скинов")]
    [SerializeField] private RectTransform skinsGridPanel;

    [Tooltip("Префаб карточки скина")]
    [SerializeField] private GameObject skinItemPrefab;

    [Header("=== Shop Preview ===")]
    [Tooltip("Панель превью выбранного скина")]
    [SerializeField] private GameObject skinPreviewPanel;

    [Tooltip("Изображение превью скина")]
    [SerializeField] private Image skinPreviewImage;

    [Tooltip("Название выбранного скина")]
    [SerializeField] private TextMeshProUGUI skinPreviewName;

    [Tooltip("Описание выбранного скина")]
    [SerializeField] private TextMeshProUGUI skinPreviewDescription;

    [Tooltip("Цена выбранного скина")]
    [SerializeField] private TextMeshProUGUI skinPreviewPrice;

    [Tooltip("Кнопка покупки/экипировки")]
    [SerializeField] private Button actionButton;

    [Tooltip("Текст на кнопке действия")]
    [SerializeField] private TextMeshProUGUI actionButtonText;

    [Header("=== Category Tabs ===")]
    [Tooltip("Кнопка: Все скины")]
    [SerializeField] private Button tabAllButton;

    [Tooltip("Кнопка: Платформы")]
    [SerializeField] private Button tabPlatformButton;

    [Tooltip("Кнопка: Мячи")]
    [SerializeField] private Button tabBallButton;

    [Header("=== Animations ===")]
    [Tooltip("Аниматор панели магазина")]
    [SerializeField] private Animator shopAnimator;

    [Tooltip("Параметр анимации открытия/закрытия")]
    [SerializeField] private string openParameterName = "Open";

    [Tooltip("Длительность анимации открытия")]
    [SerializeField] private float showAnimationDuration = 0.3f;

    [Tooltip("Длительность анимации закрытия")]
    [SerializeField] private float hideAnimationDuration = 0.3f;

    #endregion

    #region State

    private bool isShopOpen = false;
    private bool isAnimating = false;

    // Текущий выбранный скин
    private int selectedSkinId = -1;

    // Кэшированные данные
    private System.Collections.Generic.List<SkinDto> allSkins = new();
    private UserInventoryDto? userInventory;

    #endregion

    #region Unity Lifecycle

    void Start()
    {
        InitializeUI();
    }

    void Update()
    {
        HandleEscapeKey();
    }

    #endregion

    #region Initialization

    /// <summary>
    /// Инициализация UI элементов и подписка на события
    /// </summary>
    private void InitializeUI()
    {
        // Скрываем магазин при старте
        if (shopPanel != null)
            shopPanel.SetActive(false);

        // Подписка на кнопки
        if (backButton != null)
            backButton.onClick.AddListener(OnBackButtonClicked);

        if (actionButton != null)
            actionButton.onClick.AddListener(OnActionButtonClicked);

        if (tabAllButton != null)
            tabAllButton.onClick.AddListener(() => OnCategoryTabClicked(SkinTypeFilter.All));

        if (tabPlatformButton != null)
            tabPlatformButton.onClick.AddListener(() => OnCategoryTabClicked(SkinTypeFilter.Platform));

        if (tabBallButton != null)
            tabBallButton.onClick.AddListener(() => OnCategoryTabClicked(SkinTypeFilter.Ball));

        Debug.Log("[ShopController] UI initialized");
    }

    #endregion

    #region Navigation

    /// <summary>
    /// Открыть магазин из главного меню
    /// </summary>
    public void OpenShopFromMenu()
    {
        if (isShopOpen || isAnimating) return;

        StartCoroutine(OpenShopSequence(true));
    }

    /// <summary>
    /// Открыть магазин из меню паузы
    /// </summary>
    public void OpenShopFromPause()
    {
        if (isShopOpen || isAnimating) return;

        // Скрываем паузу
        if (pausePanel != null)
            pausePanel.SetActive(false);

        StartCoroutine(OpenShopSequence(false));
    }

    /// <summary>
    /// Закрыть магазин
    /// </summary>
    public void CloseShop()
    {
        Debug.Log("[ShopController] Closing shop...");
        if (!isShopOpen || isAnimating) return;

        StartCoroutine(HideShopSequence());
    }

    private IEnumerator OpenShopSequence(bool fromMainMenu)
    {
        isAnimating = true;

        // Скрываем другие панели
        if (fromMainMenu && mainMenuPanel != null)
            mainMenuPanel.SetActive(false);

        if (gameplayUI != null)
            gameplayUI.SetActive(false);

        // Показываем панель магазина
        if (shopPanel != null)
            shopPanel.SetActive(true);

        // Запускаем анимацию открытия
        if (shopAnimator != null)
            shopAnimator.SetBool(openParameterName, true);

        yield return new WaitForSecondsRealtime(showAnimationDuration);

        isShopOpen = true;
        isAnimating = false;

        // Загружаем данные (без await, так как это coroutine)
        _ = LoadShopData();

        Debug.Log("[ShopController] Shop opened");
    }

    private System.Collections.IEnumerator HideShopSequence()
    {
        isAnimating = true;

        // Запускаем анимацию закрытия
        if (shopAnimator != null)
            shopAnimator.SetBool(openParameterName, false);

        yield return new WaitForSecondsRealtime(hideAnimationDuration);

        // Скрываем панель
        if (shopPanel != null)
            shopPanel.SetActive(false);

        // Возвращаемся в меню
        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(true);

        isShopOpen = false;
        isAnimating = false;

        Debug.Log("[ShopController] Shop closed");
    }

    private void HandleEscapeKey()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && isShopOpen && !isAnimating)
        {
            CloseShop();
        }
    }

    #endregion

    #region Data Loading

    /// <summary>
    /// Загрузка данных магазина (скины + инвентарь)
    /// </summary>
    private async System.Threading.Tasks.Task LoadShopData()
    {
        // TODO: Интеграция с API
        // await Task.WhenAll(
        //     LoadAllSkins(),
        //     LoadUserInventory()
        // );

        // Временно: тестовые данные
        LoadTestData();

        UpdateCoinsDisplay();
        RenderSkinsGrid();
    }

    private void LoadTestData()
    {
        allSkins = new System.Collections.Generic.List<SkinDto>
        {
            new SkinDto { Id = 1, Name = "Неоновая платформа", SkinType = "Platform", Price = 100, Rarity = "Common" },
            new SkinDto { Id = 2, Name = "Золотой мяч", SkinType = "Ball", Price = 150, Rarity = "Rare" },
            new SkinDto { Id = 3, Name = "Огненная платформа", SkinType = "Platform", Price = 200, Rarity = "Epic" },
        };

        userInventory = new UserInventoryDto
        {
            Coins = 500,
            Skins = new System.Collections.Generic.List<UserSkinDto>
            {
                new UserSkinDto { SkinId = 1, SkinName = "Неоновая платформа", IsEquipped = false }
            }
        };
    }

    private void UpdateCoinsDisplay()
    {
        if (coinsDisplayText == null) return;

        int coins = userInventory?.Coins ?? 0;
        coinsDisplayText.text = $"{coins}";
    }

    #endregion

    #region UI Rendering

    /// <summary>
    /// Отрисовка сетки скинов
    /// </summary>
    private void RenderSkinsGrid(SkinTypeFilter filter = SkinTypeFilter.All)
    {
        if (skinsGridPanel == null || skinItemPrefab == null)
        {
            Debug.LogError("[ShopController] skinsGridPanel или skinItemPrefab не назначены!");
            return;
        }

        // Очистка сетки
        foreach (Transform child in skinsGridPanel)
        {
            Destroy(child.gameObject);
        }

        // Фильтрация
        var filteredSkins = filter switch
        {
            SkinTypeFilter.Platform => allSkins.FindAll(s => s.SkinType == "Platform"),
            SkinTypeFilter.Ball => allSkins.FindAll(s => s.SkinType == "Ball"),
            _ => allSkins
        };

        // Создание карточек
        foreach (var skin in filteredSkins)
        {
            var item = Instantiate(skinItemPrefab, skinsGridPanel);
            var skinItemUI = item.GetComponent<SkinItemUI>();

            if (skinItemUI != null)
            {
                bool isOwned = userInventory?.Skins?.Any(s => s.SkinId == skin.Id) ?? false;
                bool isEquipped = userInventory?.Skins?.Any(s => s.SkinId == skin.Id && s.IsEquipped) ?? false;

                skinItemUI.Initialize(skin, isOwned, isEquipped, OnSkinSelected);
            }
        }
    }

    /// <summary>
    /// Выбор скина в магазине
    /// </summary>
    private void OnSkinSelected(int skinId)
    {
        selectedSkinId = skinId;
        var skin = allSkins.Find(s => s.Id == skinId);

        if (skin == null || skinPreviewPanel == null) return;

        // Обновление превью
        if (skinPreviewName != null)
            skinPreviewName.text = skin.Name;

        if (skinPreviewDescription != null)
            skinPreviewDescription.text = skin.Description ?? "Нет описания";

        if (skinPreviewPrice != null)
            skinPreviewPrice.text = $"{skin.Price}";

        // Обновление кнопки
        UpdateActionButton(skin);

        Debug.Log($"[ShopController] Skin selected: {skin.Name}");
    }

    private void UpdateActionButton(SkinDto skin)
    {
        if (actionButton == null || actionButtonText == null) return;

        bool isOwned = userInventory?.Skins?.Any(s => s.SkinId == skin.Id) ?? false;
        bool isEquipped = userInventory?.Skins?.Any(s => s.SkinId == skin.Id && s.IsEquipped) ?? false;
        int coins = userInventory?.Coins ?? 0;

        if (isEquipped)
        {
            actionButtonText.text = "ЭКИПИРОВАНО";
            actionButton.interactable = false;
        }
        else if (isOwned)
        {
            actionButtonText.text = "ЭКИПИРОВАТЬ";
            actionButton.interactable = true;
        }
        else if (coins >= skin.Price)
        {
            actionButtonText.text = $"КУПИТЬ ({skin.Price})";
            actionButton.interactable = true;
        }
        else
        {
            actionButtonText.text = $"НЕДОСТАТОЧНО СРЕДСТВ";
            actionButton.interactable = false;
        }
    }

    #endregion

    #region Event Handlers

    private void OnBackButtonClicked()
    {
        CloseShop();
    }

    private void OnActionButtonClicked()
    {
        if (selectedSkinId == -1) return;

        var skin = allSkins.Find(s => s.Id == selectedSkinId);
        if (skin == null) return;

        bool isOwned = userInventory?.Skins?.Any(s => s.SkinId == skin.Id) ?? false;

        if (isOwned)
        {
            // Экипировать
            EquipSkin(skin.Id);
        }
        else
        {
            // Купить
            PurchaseSkin(skin.Id);
        }
    }

    private void OnCategoryTabClicked(SkinTypeFilter filter)
    {
        RenderSkinsGrid(filter);
    }

    #endregion

    #region Actions

    /// <summary>
    /// Покупка скина
    /// </summary>
    private async void PurchaseSkin(int skinId)
    {
        // TODO: API вызов
        // var response = await ShopAPIClient.Instance.PurchaseSkin(skinId);
        // if (response.Success) { ... }

        Debug.Log($"[ShopController] Purchase skin: {skinId}");
    }

    /// <summary>
    /// Экипировка скина
    /// </summary>
    private async void EquipSkin(int skinId)
    {
        // TODO: API вызов
        // var response = await ShopAPIClient.Instance.EquipSkin(skinId);
        // if (response.Success) { ... }

        Debug.Log($"[ShopController] Equip skin: {skinId}");
    }

    #endregion

    #region Enums & Classes

    public enum SkinTypeFilter
    {
        All,
        Platform,
        Ball
    }

    // Временные DTO для тестирования
    [System.Serializable]
    public class SkinDto
    {
        public int Id;
        public string Name;
        public string SkinType;
        public int Price;
        public string Rarity;
        public string Description;
    }

    [System.Serializable]
    public class UserSkinDto
    {
        public int SkinId;
        public string SkinName;
        public bool IsEquipped;
    }

    [System.Serializable]
    public class UserInventoryDto
    {
        public int Coins;
        public System.Collections.Generic.List<UserSkinDto> Skins;
    }

    #endregion
}
