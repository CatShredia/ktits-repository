using UnityEngine;
using System.Threading.Tasks;
using Arkanoid.Network;

namespace Arkanoid.Auth
{
    public class AuthService : MonoBehaviour
    {
        private static AuthService _instance;
        public static AuthService Instance => _instance;

        // singleton - спавним неразрушаемый объект в сцене для доступа из любого места к токену
        void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
            _instance = this;

            if (transform.parent != null)
                transform.SetParent(null);

            DontDestroyOnLoad(gameObject);

            _authToken = PlayerPrefs.GetString("AuthToken", null);
            _username = PlayerPrefs.GetString("Username", null);
            _userId = PlayerPrefs.GetString("UserId", null);
            _userGuid = PlayerPrefs.GetString("UserGuid", null);
        }

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
                    PlayerPrefs.SetString("AuthToken", value);
                else
                    PlayerPrefs.DeleteKey("AuthToken");
            }
        }

        public string Username
        {
            get => _username;
            set
            {
                _username = value;
                if (!string.IsNullOrEmpty(value))
                    PlayerPrefs.SetString("Username", value);
                else
                    PlayerPrefs.DeleteKey("Username");
            }
        }

        public string UserId
        {
            get => _userId;
            set
            {
                _userId = value;
                if (!string.IsNullOrEmpty(value))
                    PlayerPrefs.SetString("UserId", value);
                else
                    PlayerPrefs.DeleteKey("UserId");
            }
        }

        public string UserGuid
        {
            get => _userGuid;
            set
            {
                _userGuid = value;
                if (!string.IsNullOrEmpty(value))
                    PlayerPrefs.SetString("UserGuid", value);
                else
                    PlayerPrefs.DeleteKey("UserGuid");
            }
        }

        public async Task<bool> LoginAsync(string username, string password)
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

            Debug.LogError("[AuthService] Login failed: Invalid credentials");
            return false;
        }

        public async Task<bool> RegisterAsync(string username, string password)
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

            Debug.LogError("[AuthService] Registration failed: User already exists");
            return false;
        }

        public void Logout()
        {
            AuthToken = null;
            Username = null;
            UserId = null;
            UserGuid = null;
            Debug.Log("[AuthService] Logged out");
        }

        public bool IsAuthenticated() => !string.IsNullOrEmpty(_authToken);

        public string GetAuthorizationHeader() => IsAuthenticated() ? $"Bearer {_authToken}" : null;
    }
}
