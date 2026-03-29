using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Arkanoid.Auth;

/// <summary>
/// Контроллер панели авторизации.
/// Управляет входом и регистрацией пользователя.
/// </summary>
public class AuthUIController : MonoBehaviour
{
    #region UI References

    [Header("=== Input Fields ===")]
    [Tooltip("Поле ввода имени пользователя")]
    [SerializeField] private TMP_InputField usernameInput;

    [Tooltip("Поле ввода пароля")]
    [SerializeField] private TMP_InputField passwordInput;

    [Header("=== Buttons ===")]
    [Tooltip("Кнопка входа")]
    [SerializeField] private Button loginButton;

    [Tooltip("Кнопка регистрации")]
    [SerializeField] private Button registerButton;

    [Header("=== Text ===")]
    [Tooltip("Текст ошибок")]
    [SerializeField] private TextMeshProUGUI errorText;

    [Tooltip("Заголовок панели")]
    [SerializeField] private TextMeshProUGUI titleText;

    [Header("=== Panels ===")]
    [Tooltip("Основная панель авторизации")]
    [SerializeField] private GameObject authPanel;

    [Tooltip("Панель магазина (показывается после входа)")]
    [SerializeField] private GameObject shopPanel;

    [Header("=== Settings ===")]
    [Tooltip("Минимальная длина имени")]
    [SerializeField] private int minUsernameLength = 3;

    [Tooltip("Минимальная длина пароля")]
    [SerializeField] private int minPasswordLength = 6;

    #endregion

    #region State

    private bool isProcessing = false;

    #endregion

    #region Unity Lifecycle

    void Start()
    {
        InitializeUI();
    }

    void Update()
    {
        HandleEnterKey();
    }

    #endregion

    #region Initialization

    private void InitializeUI()
    {
        // Скрываем панель авторизации по умолчанию
        if (authPanel != null)
            authPanel.SetActive(false);

        // Подписка на кнопки
        if (loginButton != null)
            loginButton.onClick.AddListener(OnLoginButtonClicked);

        if (registerButton != null)
            registerButton.onClick.AddListener(OnRegisterButtonClicked);

        // Проверка сохранённой авторизации
        if (AuthService.Instance != null && AuthService.Instance.IsAuthenticated())
        {
            Debug.Log("[AuthUI] User already authenticated, showing shop");
            ShowShop();
        }
    }

    #endregion

    #region UI Methods

    public void ShowAuthPanel()
    {
        if (authPanel != null)
            authPanel.SetActive(true);

        if (shopPanel != null)
            shopPanel.SetActive(false);

        ClearError();
    }

    private void ShowShop()
    {
        if (authPanel != null)
            authPanel.SetActive(false);

        if (shopPanel != null)
            shopPanel.SetActive(true);

        // Открываем магазин
        if (ShopController.Instance != null)
        {
            ShopController.Instance.OpenShopFromMenu();
        }
    }

    private void ClearError()
    {
        if (errorText != null)
            errorText.text = "";
    }

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

        if (loginButton != null)
            loginButton.interactable = !processing;

        if (registerButton != null)
            registerButton.interactable = !processing;

        if (usernameInput != null)
            usernameInput.interactable = !processing;

        if (passwordInput != null)
            passwordInput.interactable = !processing;
    }

    #endregion

    #region Event Handlers

    private void OnLoginButtonClicked()
    {
        if (isProcessing) return;

        string username = usernameInput?.text.Trim();
        string password = passwordInput?.text;

        if (!ValidateInput(username, password))
            return;

        SetProcessingState(true);
        ClearError();

        StartCoroutine(LoginCoroutine(username, password));
    }

    private void OnRegisterButtonClicked()
    {
        if (isProcessing) return;

        string username = usernameInput?.text.Trim();
        string password = passwordInput?.text;

        if (!ValidateInput(username, password))
            return;

        SetProcessingState(true);
        ClearError();

        StartCoroutine(RegisterCoroutine(username, password));
    }

    #endregion

    #region Coroutines

    private System.Collections.IEnumerator LoginCoroutine(string username, string password)
    {
        var task = AuthService.Instance.LoginAsync(username, password);

        while (!task.IsCompleted)
        {
            yield return null;
        }

        SetProcessingState(false);

        if (task.Result)
        {
            Debug.Log("[AuthUI] Login successful");
            ShowShop();
        }
        else
        {
            ShowError("Неверное имя пользователя или пароль");
        }
    }

    private System.Collections.IEnumerator RegisterCoroutine(string username, string password)
    {
        var task = AuthService.Instance.RegisterAsync(username, password);

        while (!task.IsCompleted)
        {
            yield return null;
        }

        SetProcessingState(false);

        if (task.Result)
        {
            Debug.Log("[AuthUI] Registration successful");
            ShowShop();
        }
        else
        {
            ShowError("Пользователь с таким именем уже существует");
        }
    }

    #endregion

    #region Validation

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

    #endregion

    #region Helpers

    private void HandleEnterKey()
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            if (!isProcessing && authPanel != null && authPanel.activeSelf)
            {
                OnLoginButtonClicked();
            }
        }
    }

    #endregion
}
