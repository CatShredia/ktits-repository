using UnityEngine;
using Arkanoid.Network;

namespace Arkanoid.Auth
{
    /// <summary>
    /// Менеджер аутентификации.
    /// Хранит токен пользователя и предоставляет методы для входа/выхода.
    /// </summary>
    public class AuthService : MonoBehaviour
    {
        #region Singleton

        private static AuthService _instance;
        public static AuthService Instance
        {
            get
            {
                if (_instance == null)
                {
                    var go = GameObject.Find("AuthService");
                    if (go == null)
                    {
                        go = new GameObject("AuthService");
                        DontDestroyOnLoad(go);
                    }
                    _instance = go.GetComponent<AuthService>();
                    if (_instance == null)
                    {
                        _instance = go.AddComponent<AuthService>();
                    }
                }
                return _instance;
            }
        }

        #endregion

        #region Properties

        private string _authToken;
        private string _username;
        private string _userId;
        private string _userGuid;

        public string AuthToken
        {
            get => _authToken;
            set
            {
                _authToken = value;
                if (!string.IsNullOrEmpty(value))
                {
                    PlayerPrefs.SetString("AuthToken", value);
                    PlayerPrefs.Save();
                }
                else
                {
                    PlayerPrefs.DeleteKey("AuthToken");
                }
            }
        }

        public string Username
        {
            get => _username;
            set
            {
                _username = value;
                if (!string.IsNullOrEmpty(value))
                {
                    PlayerPrefs.SetString("Username", value);
                    PlayerPrefs.Save();
                }
                else
                {
                    PlayerPrefs.DeleteKey("Username");
                }
            }
        }

        public string UserId
        {
            get => _userId;
            set
            {
                _userId = value;
                if (!string.IsNullOrEmpty(value))
                {
                    PlayerPrefs.SetString("UserId", value);
                    PlayerPrefs.Save();
                }
                else
                {
                    PlayerPrefs.DeleteKey("UserId");
                }
            }
        }

        public string UserGuid
        {
            get => _userGuid;
            set
            {
                _userGuid = value;
                if (!string.IsNullOrEmpty(value))
                {
                    PlayerPrefs.SetString("UserGuid", value);
                    PlayerPrefs.Save();
                }
                else
                {
                    PlayerPrefs.DeleteKey("UserGuid");
                }
            }
        }

        #endregion

        #region Unity Lifecycle

        void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(gameObject);

            // Загружаем сохранённые данные
            _authToken = PlayerPrefs.GetString("AuthToken", null);
            _username = PlayerPrefs.GetString("Username", null);
            _userId = PlayerPrefs.GetString("UserId", null);
            _userGuid = PlayerPrefs.GetString("UserGuid", null);

            Debug.Log($"[AuthService] Initialized. Authenticated: {IsAuthenticated()}");
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Вход в систему
        /// </summary>
        public async System.Threading.Tasks.Task<bool> LoginAsync(string username, string password)
        {
            try
            {
                var response = await AuthAPIClient.Instance.LoginAsync(username, password);

                if (response != null && !string.IsNullOrEmpty(response.Token))
                {
                    AuthToken = response.Token;
                    Username = response.Username;
                    UserId = response.UserId;
                    UserGuid = response.UserGuid;

                    Debug.Log($"[AuthService] Login successful: {username}");
                    return true;
                }
                else
                {
                    Debug.LogError("[AuthService] Login failed: Invalid credentials");
                    return false;
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[AuthService] Login error: {e.Message}");
                return false;
            }
        }

        /// <summary>
        /// Регистрация пользователя
        /// </summary>
        public async System.Threading.Tasks.Task<bool> RegisterAsync(string username, string password)
        {
            try
            {
                var response = await AuthAPIClient.Instance.RegisterAsync(username, password);

                if (response != null && !string.IsNullOrEmpty(response.Token))
                {
                    AuthToken = response.Token;
                    Username = response.Username;
                    UserId = response.UserId;
                    UserGuid = response.UserGuid;

                    Debug.Log($"[AuthService] Registration successful: {username}");
                    return true;
                }
                else
                {
                    Debug.LogError("[AuthService] Registration failed: User already exists");
                    return false;
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[AuthService] Registration error: {e.Message}");
                return false;
            }
        }

        /// <summary>
        /// Выход из системы
        /// </summary>
        public void Logout()
        {
            AuthToken = null;
            Username = null;
            UserId = null;
            UserGuid = null;

            Debug.Log("[AuthService] Logged out");
        }

        /// <summary>
        /// Проверка: авторизован ли пользователь
        /// </summary>
        public bool IsAuthenticated()
        {
            return !string.IsNullOrEmpty(_authToken);
        }

        /// <summary>
        /// Получить заголовок авторизации для API запросов
        /// </summary>
        public string GetAuthorizationHeader()
        {
            return IsAuthenticated() ? $"Bearer {_authToken}" : null;
        }

        #endregion
    }
}
