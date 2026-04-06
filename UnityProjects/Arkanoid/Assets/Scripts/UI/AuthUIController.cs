using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Arkanoid.Auth;

public class AuthUIController : MonoBehaviour
{
    [Header("=== Input Fields ===")]
    [SerializeField] private TMP_InputField usernameInput;
    [SerializeField] private TMP_InputField passwordInput;

    [Header("=== Buttons ===")]
    [SerializeField] private Button loginButton;
    [SerializeField] private Button authButton;
    [SerializeField] private Button logOutButton;

    [Header("=== Text ===")]
    [SerializeField] private TextMeshProUGUI errorText;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI userNameText;

    [Header("=== Panels ===")]
    [SerializeField] private GameObject startMenuPanel;
    [SerializeField] private GameObject shopPanel;
    [SerializeField] private GameObject authPanel;

    [Header("=== Toggles ===")]
    [SerializeField] private Toggle isRememberedToggle;
    [SerializeField] private Toggle isRegisterToggle;

    [Header("=== Settings ===")]
    [SerializeField] private int minUsernameLength = 3;
    [SerializeField] private int minPasswordLength = 6;

    private bool isProcessing;
    private bool isRemembered;
    private bool isRegisterMode;

    private const string RememberedUsernameKey = "Auth_RememberedUsername";
    private const string RememberedPasswordKey = "Auth_RememberedPassword";

    void Start() => InitializeUI();
    void Update() => HandleEnterKey();

    private void InitializeUI()
    {
        authPanel?.SetActive(false);

        loginButton?.onClick.AddListener(OnLoginButtonClicked);
        authButton?.onClick.AddListener(OnAuthButtonClicked);
        logOutButton?.onClick.AddListener(OnLogOutButtonClicked);
        isRememberedToggle?.onValueChanged.AddListener(v => isRemembered = v);
        isRegisterToggle?.onValueChanged.AddListener(v => { isRegisterMode = v; UpdateTitle(); });

        LoadRememberedCredentials();
        UpdateTitle();
        UpdateUserNameText();
        UpdateLogOutButton();

        if (AuthService.Instance?.IsAuthenticated() ?? false) HideAuthButton(); else ShowAuthButton();
    }

    // ! Показать панель авторизации, скрыть остальные панели
    // Вызывается из MainMenu при нажатии кнопки Auth
    public void ShowAuthPanel()
    {
        ShopController.Instance?.CloseShop(showMainMenu: false);
        startMenuPanel?.SetActive(false);
        shopPanel?.SetActive(false);
        authPanel?.SetActive(true);
        if (errorText != null) errorText.text = "";
        UpdateTitle();
        VideoBackgroundController.Instance?.ShowVideo();
    }

    // ! Войти/зарегистрироваться и открыть магазин
    // Вызывается после успешного логина/регистрации
    private void ShowShop()
    {
        authPanel?.SetActive(false);
        shopPanel?.SetActive(true);
        HideAuthButton();
        UpdateUserNameText();
        UpdateLogOutButton();

        if (ShopController.Instance == null) return;
        ShopController.Instance.RefreshAuthState();

        if (ShopController.Instance.IsShopOpen)
            ShopController.Instance.ReloadShopData();
        else
            ShopController.Instance.OpenShopFromMenu();
    }

    private void UpdateTitle()
    {
        if (titleText != null) titleText.text = isRegisterMode ? "Регистрация" : "Вход";
    }

    private void UpdateUserNameText()
    {
        if (userNameText == null) return;
        bool auth = AuthService.Instance?.IsAuthenticated() ?? false;
        userNameText.text = auth ? AuthService.Instance.Username : "";
        userNameText.gameObject.SetActive(auth);
    }

    private void UpdateLogOutButton()
    {
        if (logOutButton != null) logOutButton.gameObject.SetActive(AuthService.Instance?.IsAuthenticated() ?? false);
    }

    private void ShowAuthButton() { authButton?.gameObject.SetActive(true); }
    private void HideAuthButton() { authButton?.gameObject.SetActive(false); }

    private void LoadRememberedCredentials()
    {
        if (!PlayerPrefs.HasKey(RememberedUsernameKey)) return;
        usernameInput.text = PlayerPrefs.GetString(RememberedUsernameKey);
        passwordInput.text = PlayerPrefs.GetString(RememberedPasswordKey);
        if (isRememberedToggle != null) isRememberedToggle.isOn = true;
    }

    private void SaveRememberedCredentials(string username, string password)
    {
        if (isRemembered)
        {
            PlayerPrefs.SetString(RememberedUsernameKey, username);
            PlayerPrefs.SetString(RememberedPasswordKey, password);
        }
        else
        {
            PlayerPrefs.DeleteKey(RememberedUsernameKey);
            PlayerPrefs.DeleteKey(RememberedPasswordKey);
        }
    }

    private void SetProcessingState(bool processing)
    {
        isProcessing = processing;
        if (loginButton != null) loginButton.interactable = !processing;
        if (usernameInput != null) usernameInput.interactable = !processing;
        if (passwordInput != null) passwordInput.interactable = !processing;
    }

    private void OnAuthButtonClicked() => ShowAuthPanel();

    // ! Выйти из аккаунта
    // Вызывается из кнопки LogOut
    public async void OnLogOutButtonClicked()
    {
        AuthService.Instance?.Logout();
        SaveRememberedCredentials("", "");
        shopPanel?.SetActive(false);
        ShowAuthButton();

        if (usernameInput != null) usernameInput.text = "";
        if (passwordInput != null) passwordInput.text = "";
        UpdateUserNameText();
        UpdateLogOutButton();
        ShopController.Instance?.RefreshAuthState();
        ShopController.Instance?.CloseShop();
        VideoBackgroundController.Instance?.HideVideo();
    }

    // ! Войти или зарегистрироваться
    // Вызывается из кнопки Login
    private async void OnLoginButtonClicked()
    {
        if (isProcessing || AuthService.Instance == null) return;

        string username = usernameInput?.text.Trim();
        string password = passwordInput?.text;

        if (!ValidateInput(username, password)) return;

        SetProcessingState(true);
        if (errorText != null) errorText.text = "";

        bool success = isRegisterMode
            ? await AuthService.Instance.RegisterAsync(username, password)
            : await AuthService.Instance.LoginAsync(username, password);

        SetProcessingState(false);

        if (success)
        {
            SaveRememberedCredentials(username, password);
            ShowShop();
        }
        else
        {
            ShowError(isRegisterMode
                ? "Пользователь с таким именем уже существует"
                : "Неверное имя пользователя или пароль");
        }
    }

    private bool ValidateInput(string username, string password)
    {
        if (errorText != null) errorText.text = "";

        if (string.IsNullOrEmpty(username))
        {
            ShowError("Введите имя пользователя");
            return false;
        }
        if (username.Length < minUsernameLength)
        {
            ShowError($"Имя должно содержать минимум {minUsernameLength} символа");
            return false;
        }
        if (string.IsNullOrEmpty(password))
        {
            ShowError("Введите пароль");
            return false;
        }
        if (password.Length < minPasswordLength)
        {
            ShowError($"Пароль должен содержать минимум {minPasswordLength} символов");
            return false;
        }
        return true;
    }

    private void ShowError(string message)
    {
        if (errorText != null)
        {
            errorText.text = message;
            errorText.color = Color.red;
        }
    }

    private void HandleEnterKey()
    {
        if ((Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            && !isProcessing && authPanel != null && authPanel.activeSelf)
        {
            OnLoginButtonClicked();
        }
    }
}
