using System.Threading.Tasks;
using RustyProject.Network;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainMenuApiUI : MonoBehaviour
{
    private Canvas targetCanvas;
    private GameObject uiRoot;
    private GameObject authPanel;
    private GameObject leaderboardPanel;
    private Text coinsText;
    private Text accountText;
    private Text authStatusText;
    private Text leaderboardText;
    private InputField usernameInput;
    private InputField passwordInput;
    private Toggle registerToggle;
    private Button authSubmitButton;
    private Text authSubmitButtonText;

    // ? Start : создаёт runtime-UI и подписывает форму на события аккаунта
    private void Start()
    {
        EnsureUi();

        if (PlayerAccountManager.Instance != null)
        {
            PlayerAccountManager.Instance.OnProfileChanged += RefreshHeader;
            PlayerAccountManager.Instance.OnStatusMessage += SetAuthStatus;
            _ = PlayerAccountManager.Instance.EnsureSessionRestoredAsync();
        }

        RefreshHeader();
    }

    // ? OnDestroy : снимает подписки с менеджера аккаунта
    private void OnDestroy()
    {
        if (PlayerAccountManager.Instance != null)
        {
            PlayerAccountManager.Instance.OnProfileChanged -= RefreshHeader;
            PlayerAccountManager.Instance.OnStatusMessage -= SetAuthStatus;
        }
    }

    // ? OpenAccountPanel : открывает полноэкранную форму входа и регистрации
    public void OpenAccountPanel()
    {
        if (!EnsureUi()) return;
        authPanel.SetActive(true);
        leaderboardPanel.SetActive(false);
        SetAuthStatus(PlayerAccountManager.Instance != null && PlayerAccountManager.Instance.IsAuthenticated
            ? $"Вы вошли как {PlayerAccountManager.Instance.CurrentProfile.username}"
            : "Введите логин и пароль.");
        RefreshAuthButtonLabel();
    }

    // ? OpenLeaderboardPanel : открывает полноэкранную таблицу результатов и загружает данные
    public async void OpenLeaderboardPanel()
    {
        if (!EnsureUi()) return;
        authPanel.SetActive(false);
        leaderboardPanel.SetActive(true);
        leaderboardText.text = "Загрузка...";

        LeaderboardEntryDto[] entries = PlayerAccountManager.Instance != null
            ? await PlayerAccountManager.Instance.GetLeaderboardAsync(10)
            : new LeaderboardEntryDto[0];

        if (entries.Length == 0)
        {
            leaderboardText.text = "Нет данных.";
            return;
        }

        System.Text.StringBuilder builder = new System.Text.StringBuilder();
        for (int i = 0; i < entries.Length; i++)
        {
            LeaderboardEntryDto entry = entries[i];
            builder.AppendLine($"{entry.rank}. {entry.username} - {entry.coins} мон.");
        }

        leaderboardText.text = builder.ToString();
    }

    // ? SubmitAuth : отправляет логин или регистрацию и показывает ошибки прямо на форме
    private async void SubmitAuth()
    {
        if (PlayerAccountManager.Instance == null)
        {
            SetAuthStatus("Менеджер аккаунта не найден.");
            return;
        }

        string username = usernameInput.text.Trim();
        string password = passwordInput.text;
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            SetAuthStatus("Заполните логин и пароль.");
            return;
        }

        authSubmitButton.interactable = false;
        SetAuthStatus(registerToggle.isOn ? "Регистрация..." : "Вход...");

        bool success = registerToggle.isOn
            ? await PlayerAccountManager.Instance.RegisterAsync(username, password)
            : await PlayerAccountManager.Instance.LoginAsync(username, password);

        authSubmitButton.interactable = true;
        RefreshHeader();

        if (success)
        {
            authPanel.SetActive(false);
            passwordInput.text = string.Empty;
        }
    }

    // ? Logout : выполняет выход из аккаунта из формы
    private void Logout()
    {
        PlayerAccountManager.Instance?.Logout();
        RefreshHeader();
    }

    // ? EnsureUi : лениво создаёт весь runtime-интерфейс меню поверх канваса
    private bool EnsureUi()
    {
        if (uiRoot != null && authPanel != null && leaderboardPanel != null)
        {
            return true;
        }

        if (uiRoot != null)
        {
            Destroy(uiRoot);
            uiRoot = null;
        }

        EnsureEventSystemExists();
        targetCanvas = GetComponentInParent<Canvas>();
        if (targetCanvas == null)
        {
            targetCanvas = FindFirstObjectByType<Canvas>();
        }

        if (targetCanvas == null)
        {
            targetCanvas = CreateFallbackCanvas();
        }

        if (targetCanvas == null)
        {
            Debug.LogWarning("MainMenuApiUI: Canvas не найден и не создан.");
            return false;
        }

        uiRoot = CreateElement("RuntimeApiUI", targetCanvas.transform);
        RectTransform rootRect = uiRoot.AddComponent<RectTransform>();
        rootRect.anchorMin = Vector2.zero;
        rootRect.anchorMax = Vector2.one;
        rootRect.offsetMin = Vector2.zero;
        rootRect.offsetMax = Vector2.zero;

        coinsText = CreateText("CoinsText", uiRoot.transform, new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(-180f, -36f), new Vector2(320f, 42f), TextAnchor.MiddleRight, "Coins: 0", 26);
        accountText = CreateText("AccountText", uiRoot.transform, new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(-180f, -76f), new Vector2(320f, 32f), TextAnchor.MiddleRight, "Гость", 22);

        authPanel = CreatePanel("AuthPanel", uiRoot.transform, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero, new Color(0f, 0f, 0f, 0.86f));
        GameObject authContent = CreatePanel("AuthContent", authPanel.transform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(760f, 560f), new Color(0.08f, 0.08f, 0.08f, 0.98f));
        CreateText("AuthTitle", authContent.transform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -44f), new Vector2(500f, 44f), TextAnchor.MiddleCenter, "Аккаунт", 34);
        authStatusText = CreateText("AuthStatus", authContent.transform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -94f), new Vector2(560f, 56f), TextAnchor.MiddleCenter, "", 22);
        usernameInput = CreateInputField("UsernameInput", authContent.transform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -180f), "Логин", false);
        passwordInput = CreateInputField("PasswordInput", authContent.transform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -270f), "Пароль", true);
        registerToggle = CreateToggle("RegisterToggle", authContent.transform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(-80f, -355f), "Регистрация?");
        registerToggle.onValueChanged.AddListener(_ => RefreshAuthButtonLabel());
        authSubmitButton = CreateButton("AuthSubmitButton", authContent.transform, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(-120f, 52f), new Vector2(180f, 52f), "Войти");
        authSubmitButton.onClick.AddListener(SubmitAuth);
        authSubmitButtonText = authSubmitButton.GetComponentInChildren<Text>();
        Button closeAuthButton = CreateButton("AuthCloseButton", authContent.transform, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(120f, 52f), new Vector2(180f, 52f), "Закрыть");
        closeAuthButton.onClick.AddListener(() => authPanel.SetActive(false));
        Button logoutButton = CreateButton("LogoutButton", authContent.transform, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 120f), new Vector2(240f, 42f), "Выйти из аккаунта");
        logoutButton.onClick.AddListener(Logout);

        leaderboardPanel = CreatePanel("LeaderboardPanel", uiRoot.transform, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero, new Color(0f, 0f, 0f, 0.86f));
        GameObject leaderboardContent = CreatePanel("LeaderboardContent", leaderboardPanel.transform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(920f, 640f), new Color(0.08f, 0.08f, 0.08f, 0.98f));
        CreateText("LeaderboardTitle", leaderboardContent.transform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -44f), new Vector2(600f, 44f), TextAnchor.MiddleCenter, "Таблица результатов", 34);
        leaderboardText = CreateText("LeaderboardText", leaderboardContent.transform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, -10f), new Vector2(700f, 420f), TextAnchor.UpperLeft, "", 28);
        Button closeLeaderboardButton = CreateButton("LeaderboardCloseButton", leaderboardContent.transform, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 42f), new Vector2(180f, 52f), "Закрыть");
        closeLeaderboardButton.onClick.AddListener(() => leaderboardPanel.SetActive(false));

        authPanel.SetActive(false);
        leaderboardPanel.SetActive(false);
        RefreshAuthButtonLabel();
        return true;
    }

    // ? RefreshHeader : обновляет отображение имени игрока и монет в шапке меню
    private void RefreshHeader()
    {
        EnsureUi();
        if (coinsText == null || accountText == null) return;

        if (PlayerAccountManager.Instance != null && PlayerAccountManager.Instance.IsAuthenticated)
        {
            coinsText.text = $"Coins: {PlayerAccountManager.Instance.CurrentProfile.coins}";
            accountText.text = PlayerAccountManager.Instance.CurrentProfile.username;
        }
        else
        {
            coinsText.text = "Coins: -";
            accountText.text = "Гость";
        }
    }

    // ? RefreshAuthButtonLabel : меняет текст кнопки под режим входа или регистрации
    private void RefreshAuthButtonLabel()
    {
        if (authSubmitButtonText != null)
        {
            authSubmitButtonText.text = registerToggle != null && registerToggle.isOn ? "Зарегистр." : "Войти";
        }
    }

    // ? SetAuthStatus : выводит статус и ошибки валидации на форму аккаунта
    private void SetAuthStatus(string message)
    {
        if (authStatusText != null)
        {
            authStatusText.text = message;
        }
    }

    // ? EnsureEventSystemExists : гарантирует наличие EventSystem для runtime-кнопок и input field
    private static void EnsureEventSystemExists()
    {
        if (FindFirstObjectByType<EventSystem>() != null) return;

        GameObject eventSystem = new GameObject("EventSystem");
        eventSystem.AddComponent<EventSystem>();
        eventSystem.AddComponent<StandaloneInputModule>();
    }

    // ? CreateFallbackCanvas : создаёт резервный Canvas, если на сцене его не удалось найти
    private Canvas CreateFallbackCanvas()
    {
        GameObject canvasObject = new GameObject("RuntimeApiCanvas");
        Canvas canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObject.AddComponent<CanvasScaler>();
        canvasObject.AddComponent<GraphicRaycaster>();
        return canvas;
    }

    // ? CreateElement : создаёт пустой UI-объект и добавляет его в иерархию
    private static GameObject CreateElement(string name, Transform parent)
    {
        GameObject obj = new GameObject(name);
        obj.transform.SetParent(parent, false);
        return obj;
    }

    // ? CreatePanel : создаёт UI-панель с RectTransform и фоном
    private static GameObject CreatePanel(string name, Transform parent, Vector2 anchorMin, Vector2 anchorMax, Vector2 anchoredPos, Vector2 size, Color color)
    {
        GameObject panel = CreateElement(name, parent);
        RectTransform rect = panel.AddComponent<RectTransform>();
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.sizeDelta = size;
        rect.anchoredPosition = anchoredPos;
        Image image = panel.AddComponent<Image>();
        image.color = color;
        return panel;
    }

    // ? CreateText : создаёт текстовый элемент нужного размера и выравнивания
    private static Text CreateText(string name, Transform parent, Vector2 anchorMin, Vector2 anchorMax, Vector2 anchoredPos, Vector2 size, TextAnchor alignment, string textValue, int fontSize = 18)
    {
        GameObject textObject = CreateElement(name, parent);
        RectTransform rect = textObject.AddComponent<RectTransform>();
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.sizeDelta = size;
        rect.anchoredPosition = anchoredPos;

        Text text = textObject.AddComponent<Text>();
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = fontSize;
        text.color = Color.white;
        text.alignment = alignment;
        text.horizontalOverflow = HorizontalWrapMode.Wrap;
        text.verticalOverflow = VerticalWrapMode.Overflow;
        text.text = textValue;
        return text;
    }

    // ? CreateButton : создаёт runtime-кнопку с подписью
    private static Button CreateButton(string name, Transform parent, Vector2 anchorMin, Vector2 anchorMax, Vector2 anchoredPos, Vector2 size, string label)
    {
        GameObject buttonObject = CreatePanel(name, parent, anchorMin, anchorMax, anchoredPos, size, new Color(0.22f, 0.22f, 0.22f, 0.95f));
        Button button = buttonObject.AddComponent<Button>();
        ColorBlock colors = button.colors;
        colors.normalColor = new Color(0.22f, 0.22f, 0.22f, 0.95f);
        colors.highlightedColor = new Color(0.32f, 0.32f, 0.32f, 0.95f);
        colors.pressedColor = new Color(0.15f, 0.15f, 0.15f, 0.95f);
        button.colors = colors;

        CreateText("Label", buttonObject.transform, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero, TextAnchor.MiddleCenter, label, 22);
        return button;
    }

    // ? CreateInputField : создаёт поле ввода для логина и пароля
    private static InputField CreateInputField(string name, Transform parent, Vector2 anchorMin, Vector2 anchorMax, Vector2 anchoredPos, string placeholderText, bool isPassword)
    {
        GameObject inputObject = CreatePanel(name, parent, anchorMin, anchorMax, anchoredPos, new Vector2(420f, 56f), new Color(1f, 1f, 1f, 0.95f));
        InputField input = inputObject.AddComponent<InputField>();

        Text text = CreateText("Text", inputObject.transform, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero, TextAnchor.MiddleLeft, string.Empty, 24);
        text.color = Color.black;

        Text placeholder = CreateText("Placeholder", inputObject.transform, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero, TextAnchor.MiddleLeft, placeholderText, 24);
        placeholder.color = new Color(0.45f, 0.45f, 0.45f, 0.9f);

        RectTransform textRect = text.rectTransform;
        textRect.offsetMin = new Vector2(16f, 8f);
        textRect.offsetMax = new Vector2(-16f, -8f);

        RectTransform placeholderRect = placeholder.rectTransform;
        placeholderRect.offsetMin = new Vector2(16f, 8f);
        placeholderRect.offsetMax = new Vector2(-16f, -8f);

        input.textComponent = text;
        input.placeholder = placeholder;
        input.contentType = isPassword ? InputField.ContentType.Password : InputField.ContentType.Standard;
        return input;
    }

    // ? CreateToggle : создаёт checkbox для режима регистрации
    private static Toggle CreateToggle(string name, Transform parent, Vector2 anchorMin, Vector2 anchorMax, Vector2 anchoredPos, string label)
    {
        GameObject toggleObject = CreateElement(name, parent);
        RectTransform rect = toggleObject.AddComponent<RectTransform>();
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.sizeDelta = new Vector2(320f, 36f);
        rect.anchoredPosition = anchoredPos;

        Toggle toggle = toggleObject.AddComponent<Toggle>();

        GameObject background = CreatePanel("Background", toggleObject.transform, new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), new Vector2(16f, 0f), new Vector2(28f, 28f), Color.white);
        GameObject checkmark = CreatePanel("Checkmark", background.transform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(18f, 18f), new Color(0.2f, 0.7f, 0.2f, 1f));
        CreateText("Label", toggleObject.transform, new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), new Vector2(132f, 0f), new Vector2(220f, 30f), TextAnchor.MiddleLeft, label, 22);

        toggle.targetGraphic = background.GetComponent<Image>();
        toggle.graphic = checkmark.GetComponent<Image>();
        toggle.isOn = false;
        return toggle;
    }
}
