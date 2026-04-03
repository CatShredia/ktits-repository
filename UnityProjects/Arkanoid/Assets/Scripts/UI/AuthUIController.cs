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
    [SerializeField] private Button registerButton;
    [SerializeField] private Button authButton;
    [SerializeField] private Button logOutButton;

    [Header("=== Text ===")]
    [SerializeField] private TextMeshProUGUI errorText;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI userNameText;

    [Header("=== Panels ===")]
    [SerializeField] private GameObject authPanel;
    [SerializeField] private GameObject shopPanel;

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
        if (authPanel != null) authPanel.SetActive(false);

        if (loginButton != null) loginButton.onClick.AddListener(OnLoginButtonClicked);
        if (registerButton != null) registerButton.onClick.AddListener(OnRegisterButtonClicked);
        if (authButton != null) authButton.onClick.AddListener(OnAuthButtonClicked);
        if (logOutButton != null) logOutButton.onClick.AddListener(OnLogOutButtonClicked);
        if (isRememberedToggle != null) isRememberedToggle.onValueChanged.AddListener(OnRememberedToggleChanged);
        if (isRegisterToggle != null) isRegisterToggle.onValueChanged.AddListener(OnRegisterToggleChanged);

        LoadRememberedCredentials();
        UpdateTitle();
        UpdateUserNameText();
        UpdateLogOutButton();

        if (AuthService.Instance != null && AuthService.Instance.IsAuthenticated())
        {
            HideAuthButton();
            HideAuthPanel();
        }
        else
        {
            ShowAuthButton();
            HideAuthPanel();
        }
    }

    public void ShowAuthPanel()
    {
        if (authPanel != null) authPanel.SetActive(true);
        ClearError();
        UpdateTitle();
    }

    private void ShowShop()
    {
        if (authPanel != null) authPanel.SetActive(false);
        if (shopPanel != null) shopPanel.SetActive(true);
        HideAuthButton();
        UpdateUserNameText();
        UpdateLogOutButton();

        // Если магазин уже открыт, обновляем состояние авторизации и данные
        if (ShopController.Instance != null)
        {
            ShopController.Instance.RefreshAuthState();

            // Если магазин уже открыт (isShopOpen = true), нужно перезагрузить инвентарь
            // OpenShopFromMenu вернётся сразу, поэтому вызываем LoadShopData напрямую
            if (ShopController.Instance.IsShopOpen)
            {
                ShopController.Instance.ReloadShopData();
            }
            else
            {
                ShopController.Instance?.OpenShopFromMenu();
            }
        }
    }

    private void UpdateTitle()
    {
        if (titleText != null)
            titleText.text = isRegisterMode ? "Регистрация" : "Вход";
    }

    private void UpdateUserNameText()
    {
        if (userNameText == null) return;

        if (AuthService.Instance != null && AuthService.Instance.IsAuthenticated())
        {
            userNameText.text = AuthService.Instance.Username;
            userNameText.gameObject.SetActive(true);
        }
        else
        {
            userNameText.gameObject.SetActive(false);
        }
    }

    private void UpdateLogOutButton()
    {
        if (logOutButton == null) return;
        logOutButton.gameObject.SetActive(AuthService.Instance != null && AuthService.Instance.IsAuthenticated());
    }

    private void ShowAuthButton() { if (authButton != null) authButton.gameObject.SetActive(true); }
    private void HideAuthButton() { if (authButton != null) authButton.gameObject.SetActive(false); }
    private void HideAuthPanel() { if (authPanel != null) authPanel.SetActive(false); }
    private void HideShopPanel() { if (shopPanel != null) shopPanel.SetActive(false); }

    private void OnRememberedToggleChanged(bool value)
    {
        isRemembered = value;
        Debug.Log($"[AuthUI] Remember me: {value}");
    }

    private void OnRegisterToggleChanged(bool value)
    {
        isRegisterMode = value;
        Debug.Log($"[AuthUI] Register mode: {value}");
        UpdateTitle();
    }

    private void LoadRememberedCredentials()
    {
        if (PlayerPrefs.HasKey(RememberedUsernameKey))
        {
            usernameInput.text = PlayerPrefs.GetString(RememberedUsernameKey);
            passwordInput.text = PlayerPrefs.GetString(RememberedPasswordKey);
            if (isRememberedToggle != null) isRememberedToggle.isOn = true;
            Debug.Log("[AuthUI] Loaded remembered credentials");
        }
    }

    private void SaveRememberedCredentials(string username, string password)
    {
        if (isRemembered)
        {
            PlayerPrefs.SetString(RememberedUsernameKey, username);
            PlayerPrefs.SetString(RememberedPasswordKey, password);
            Debug.Log("[AuthUI] Saved remembered credentials");
        }
        else
        {
            ClearRememberedCredentials();
        }
    }

    private void ClearRememberedCredentials()
    {
        PlayerPrefs.DeleteKey(RememberedUsernameKey);
        PlayerPrefs.DeleteKey(RememberedPasswordKey);
        Debug.Log("[AuthUI] Cleared remembered credentials");
    }

    private void ClearError() { if (errorText != null) errorText.text = ""; }

    private void ShowError(string message)
    {
        if (errorText != null)
        {
            errorText.text = message;
            errorText.color = Color.red;
        }
        Debug.LogError($"[AuthUI] Error: {message}");
    }

    private void SetProcessingState(bool processing)
    {
        isProcessing = processing;
        if (loginButton != null) loginButton.interactable = !processing;
        if (registerButton != null) registerButton.interactable = !processing;
        if (usernameInput != null) usernameInput.interactable = !processing;
        if (passwordInput != null) passwordInput.interactable = !processing;
    }

    private void OnAuthButtonClicked() => ShowAuthPanel();

    private async void OnLogOutButtonClicked()
    {
        Debug.Log("[AuthUI] Logout button clicked");

        AuthService.Instance?.Logout();
        ClearRememberedCredentials();
        HideShopPanel();
        ShowAuthButton();

        if (usernameInput != null) usernameInput.text = "";
        if (passwordInput != null) passwordInput.text = "";

        UpdateUserNameText();
        UpdateLogOutButton();

        // Обновить состояние монет в магазине перед закрытием
        if (ShopController.Instance != null)
        {
            ShopController.Instance.RefreshAuthState();
        }

        ShopController.Instance?.CloseShop();

        Debug.Log("[AuthUI] Logout completed");
    }

    private async void OnLoginButtonClicked()
    {
        if (isProcessing) return;

        if (AuthService.Instance == null)
        {
            ShowError("AuthService не найден на сцене!");
            Debug.LogError("[AuthUI] AuthService.Instance is null. Make sure AuthService is on the scene.");
            return;
        }

        string username = usernameInput?.text.Trim();
        string password = passwordInput?.text;

        if (!ValidateInput(username, password)) return;

        SetProcessingState(true);
        ClearError();

        bool success = isRegisterMode
            ? await AuthService.Instance.RegisterAsync(username, password)
            : await AuthService.Instance.LoginAsync(username, password);

        SetProcessingState(false);

        if (success)
        {
            Debug.Log(isRegisterMode ? "[AuthUI] Registration successful" : "[AuthUI] Login successful");
            SaveRememberedCredentials(username, password);
            ShowShop();
        }
        else
        {
            ShowError(isRegisterMode ? "Пользователь с таким именем уже существует" : "Неверное имя пользователя или пароль");
        }
    }

    private void OnRegisterButtonClicked() => OnLoginButtonClicked();

    private bool ValidateInput(string username, string password)
    {
        ClearError();

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

    private void HandleEnterKey()
    {
        if ((Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            && !isProcessing && authPanel != null && authPanel.activeSelf)
        {
            OnLoginButtonClicked();
        }
    }
}
