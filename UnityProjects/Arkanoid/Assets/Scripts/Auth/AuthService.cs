using UnityEngine;
using Arkanoid.Network;

namespace Arkanoid.Auth
{
    // ! Хранит токен пользователя и предоставляет методы для входа/выхода.
    public class AuthService : MonoBehaviour
    {
        // singleton - спавним неразрушаемый объект в сцене для доступа из любого места к токену
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

        private string _authToken;
        private string _username;
        private string _userId;
        private string _userGuid;

        // токен хранится в PlayerPrefs для сохранения между сессиями. 
        // При загрузке игры он восстанавливается, и если токен валиден, пользователь остаётся авторизованным.
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

        void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(gameObject);

            _authToken = PlayerPrefs.GetString("AuthToken", null);
            _username = PlayerPrefs.GetString("Username", null);
            _userId = PlayerPrefs.GetString("UserId", null);
            _userGuid = PlayerPrefs.GetString("UserGuid", null);
        }

        // ! Вход в систему
        public async Task<bool> LoginAsync(string username, string password)
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

        // ! Регистрация пользователя
        public async Task<bool> RegisterAsync(string username, string password)
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

        // ! Выход из системы
        public void Logout()
        {
            AuthToken = null;
            Username = null;
            UserId = null;
            UserGuid = null;

            Debug.Log("[AuthService] Logged out");
        }

        // ! Проверка: авторизован ли пользователь
        public bool IsAuthenticated()
        {
            return !string.IsNullOrEmpty(_authToken);
        }

        public string GetAuthorizationHeader()
        {
            return IsAuthenticated() ? $"Bearer {_authToken}" : null;
        }
    }
}
