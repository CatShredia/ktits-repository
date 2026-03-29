using UnityEngine;

namespace Arkanoid.Auth
{
    /// <summary>
    /// Менеджер аутентификации.
    /// Хранит токен пользователя.
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

        public string AuthToken
        {
            get => _authToken;
            set
            {
                _authToken = value;
                // Сохраняем токен в PlayerPrefs для сохранения между сессиями
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

            // Загружаем сохранённый токен
            _authToken = PlayerPrefs.GetString("AuthToken", null);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Очистить токен (выход из системы)
        /// </summary>
        public void Logout()
        {
            _authToken = null;
            PlayerPrefs.DeleteKey("AuthToken");
        }

        /// <summary>
        /// Проверка: авторизован ли пользователь
        /// </summary>
        public bool IsAuthenticated()
        {
            return !string.IsNullOrEmpty(_authToken);
        }

        #endregion
    }
}
