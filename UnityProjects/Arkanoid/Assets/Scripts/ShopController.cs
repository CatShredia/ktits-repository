using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Debug = UnityEngine.Debug;

/// <summary>
/// Контроллер магазина (Carousel версия).
/// Переключение скинов стрелками, клик по карточке = покупка/экипировка.
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

    [Tooltip("Кнопка назад")]
    [SerializeField] private Button backButton;

    [Header("=== Carousel ===")]
    [Tooltip("Контейнер для одного скина")]
    [SerializeField] private RectTransform skinContainer;

    [Tooltip("Кнопка: Назад (стрелка влево)")]
    [SerializeField] private Button prevButton;

    [Tooltip("Кнопка: Вперёд (стрелка вправо)")]
    [SerializeField] private Button nextButton;

    [Tooltip("Префаб карточки скина")]
    [SerializeField] private GameObject skinItemPrefab;

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

    // Текущий индекс скина в карусели
    private int currentIndex = 0;

    // Кэшированные данные
    private List<SkinDto> allSkins = new();
    private UserInventoryDto? userInventory;

    // Текущий показываемый скин
    private SkinDto? currentSkin;
    private bool isCurrentSkinOwned;
    private bool isCurrentSkinEquipped;

    #endregion

    #region Unity Lifecycle

    void Start()
    {
        InitializeUI();
    }

    void Update()
    {
        HandleEscapeKey();
        HandleArrowKeys();
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

        if (prevButton != null)
            prevButton.onClick.AddListener(OnPrevButtonClicked);

        if (nextButton != null)
            nextButton.onClick.AddListener(OnNextButtonClicked);

        Debug.Log("[ShopController] UI initialized (Carousel mode)");
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

        if (fromMainMenu && mainMenuPanel != null)
            mainMenuPanel.SetActive(false);

        if (gameplayUI != null)
            gameplayUI.SetActive(false);

        if (shopPanel != null)
            shopPanel.SetActive(true);

        if (shopAnimator != null)
            shopAnimator.SetBool(openParameterName, true);

        yield return new WaitForSecondsRealtime(showAnimationDuration);

        isShopOpen = true;
        isAnimating = false;

        _ = LoadShopData();

        Debug.Log("[ShopController] Shop opened");
    }

    private IEnumerator HideShopSequence()
    {
        isAnimating = true;

        if (shopAnimator != null)
            shopAnimator.SetBool(openParameterName, false);

        yield return new WaitForSecondsRealtime(hideAnimationDuration);

        if (shopPanel != null)
            shopPanel.SetActive(false);

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

    private void HandleArrowKeys()
    {
        if (!isShopOpen || isAnimating) return;

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            OnPrevButtonClicked();
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            OnNextButtonClicked();
        }
    }

    #endregion

    #region Data Loading

    /// <summary>
    /// Загрузка данных магазина
    /// </summary>
    private async System.Threading.Tasks.Task LoadShopData()
    {
        // TODO: Интеграция с API
        LoadTestData();

        UpdateCoinsDisplay();
        ShowCurrentSkin();
    }

    private void LoadTestData()
    {
        allSkins = new List<SkinDto>
        {
            new SkinDto { Id = 1, Name = "Неоновая платформа", SkinType = "Platform", Price = 100, Rarity = "Common" },
            new SkinDto { Id = 2, Name = "Золотой мяч", SkinType = "Ball", Price = 150, Rarity = "Rare" },
            new SkinDto { Id = 3, Name = "Огненная платформа", SkinType = "Platform", Price = 200, Rarity = "Epic" },
        };

        userInventory = new UserInventoryDto
        {
            Coins = 500,
            Skins = new List<UserSkinDto>
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

    #region Carousel Logic

    /// <summary>
    /// Показать текущий скин в карусели
    /// </summary>
    private void ShowCurrentSkin()
    {
        if (allSkins.Count == 0 || skinContainer == null || skinItemPrefab == null)
        {
            Debug.LogError("[ShopController] Cannot show skin: missing data or UI references");
            return;
        }

        // Очистка контейнера
        foreach (Transform child in skinContainer)
        {
            Destroy(child.gameObject);
        }

        // Получаем текущий скин
        currentSkin = allSkins[currentIndex];

        // Проверяем владение
        isCurrentSkinOwned = userInventory?.Skins?.Any(s => s.SkinId == currentSkin.Id) ?? false;
        isCurrentSkinEquipped = userInventory?.Skins?.Any(s => s.SkinId == currentSkin.Id && s.IsEquipped) ?? false;

        // Спавн карточки
        var item = Instantiate(skinItemPrefab, skinContainer, false);

        // Сброс позиции и масштаба
        var rectTransform = item.GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            rectTransform.localPosition = Vector3.zero;
            rectTransform.localScale = Vector3.one;
        }

        // Инициализация карточки (клик = покупка/экипировка)
        var skinItemUI = item.GetComponent<SkinItemUI>();
        if (skinItemUI != null)
        {
            skinItemUI.Initialize(currentSkin, isCurrentSkinOwned, isCurrentSkinEquipped, OnSkinActionClicked);
        }

        // Обновление кнопок навигации
        UpdateCarouselButtons();

        Debug.Log($"[ShopController] Showing skin: {currentSkin.Name} ({currentIndex + 1}/{allSkins.Count})");
    }

    /// <summary>
    /// Клик по скину в карусели = покупка или экипировка
    /// </summary>
    private void OnSkinActionClicked(int skinId)
    {
        if (skinId != currentSkin?.Id) return;

        if (isCurrentSkinOwned)
        {
            EquipSkin(skinId);
        }
        else
        {
            PurchaseSkin(skinId);
        }
    }

    /// <summary>
    /// Кнопка: Назад
    /// </summary>
    private void OnPrevButtonClicked()
    {
        if (currentIndex > 0)
        {
            currentIndex--;
            ShowCurrentSkin();
        }
    }

    /// <summary>
    /// Кнопка: Вперёд
    /// </summary>
    private void OnNextButtonClicked()
    {
        if (currentIndex < allSkins.Count - 1)
        {
            currentIndex++;
            ShowCurrentSkin();
        }
    }

    /// <summary>
    /// Обновление состояния кнопок карусели
    /// </summary>
    private void UpdateCarouselButtons()
    {
        if (prevButton != null)
            prevButton.interactable = currentIndex > 0;

        if (nextButton != null)
            nextButton.interactable = currentIndex < allSkins.Count - 1;
    }

    #endregion

    #region Event Handlers

    private void OnBackButtonClicked()
    {
        CloseShop();
    }

    #endregion

    #region Actions

    /// <summary>
    /// Покупка скина
    /// </summary>
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

        // TODO: API вызов
        // var response = await ShopAPIClient.Instance.PurchaseSkin(skinId);

        // Временно: локальное обновление
        Debug.Log($"[ShopController] Purchase skin: {skinId} ({skin.Name}) - {skin.Price} coins");

        // Обновляем данные
        userInventory.Coins -= skin.Price;
        userInventory.Skins.Add(new UserSkinDto
        {
            SkinId = skinId,
            SkinName = skin.Name,
            IsEquipped = false
        });

        // Обновляем UI
        UpdateCoinsDisplay();
        ShowCurrentSkin();
    }

    /// <summary>
    /// Экипировка скина
    /// </summary>
    private async void EquipSkin(int skinId)
    {
        // TODO: API вызов
        // var response = await ShopAPIClient.Instance.EquipSkin(skinId);

        // Временно: локальное обновление
        Debug.Log($"[ShopController] Equip skin: {skinId}");

        // Обновляем данные
        if (userInventory?.Skins != null)
        {
            // Снять все экипированные скины того же типа
            var skin = allSkins.Find(s => s.Id == skinId);
            if (skin != null)
            {
                foreach (var userSkin in userInventory.Skins)
                {
                    var s = allSkins.Find(sk => sk.Id == userSkin.SkinId);
                    if (s?.SkinType == skin.SkinType)
                    {
                        userSkin.IsEquipped = false;
                    }
                }
            }

            // Экипировать выбранный
            var equippedSkin = userInventory.Skins.FirstOrDefault(s => s.SkinId == skinId);
            if (equippedSkin != null)
            {
                equippedSkin.IsEquipped = true;
            }
        }

        // Обновляем UI
        ShowCurrentSkin();
    }

    #endregion

    #region Enums & Classes

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
        public List<UserSkinDto> Skins;
    }

    #endregion
}
