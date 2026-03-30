using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Arkanoid.Network
{
    public class AuthAPIClient : MonoBehaviour
    {
        private static AuthAPIClient _instance;
        public static AuthAPIClient Instance => _instance;

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
        }

        private const string BASE_URL = "http://localhost:5250/api/auth";

        public async Task<AuthResponseDto> RegisterAsync(string username, string password)
        {
            var json = JsonConvert.SerializeObject(new { username, password });
            var responseJson = await PostRequest($"{BASE_URL}/register", json);
            return string.IsNullOrEmpty(responseJson) ? null : JsonConvert.DeserializeObject<AuthResponseDto>(responseJson);
        }

        public async Task<AuthResponseDto> LoginAsync(string username, string password)
        {
            var json = JsonConvert.SerializeObject(new { username, password });
            var responseJson = await PostRequest($"{BASE_URL}/login", json);
            return string.IsNullOrEmpty(responseJson) ? null : JsonConvert.DeserializeObject<AuthResponseDto>(responseJson);
        }

        private async Task<string> PostRequest(string url, string json)
        {
            using var request = new UnityWebRequest(url, "POST");
            request.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(json));
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            await request.SendWebRequest();

            Debug.Log($"[AuthAPI] {url} - Status: {request.responseCode}, Result: {request.result}, Response: {request.downloadHandler.text}");

            return request.result == UnityWebRequest.Result.Success ? request.downloadHandler.text : null;
        }

        [Serializable]
        public class AuthResponseDto
        {
            [JsonProperty("id")]
            public int Id;

            [JsonProperty("userId")]
            public string UserId;

            [JsonProperty("userGuid")]
            public string UserGuid;

            [JsonProperty("username")]
            public string Username;

            [JsonProperty("token")]
            public string Token;

            [JsonProperty("expiresAt")]
            public DateTime ExpiresAt;
        }
    }
}
