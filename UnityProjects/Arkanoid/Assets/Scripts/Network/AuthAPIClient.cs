using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Arkanoid.Network
{
    /// <summary>
    /// API клиент для аутентификации.
    /// </summary>
    public class AuthAPIClient : MonoBehaviour
    {
        #region Singleton

        private static AuthAPIClient _instance;
        public static AuthAPIClient Instance
        {
            get
            {
                if (_instance == null)
                {
                    var go = GameObject.Find("AuthAPIClient");
                    if (go == null)
                    {
                        go = new GameObject("AuthAPIClient");
                        DontDestroyOnLoad(go);
                    }
                    _instance = go.GetComponent<AuthAPIClient>();
                    if (_instance == null)
                    {
                        _instance = go.AddComponent<AuthAPIClient>();
                    }
                }
                return _instance;
            }
        }

        #endregion

        #region Configuration

        private const string BASE_URL = "http://localhost:5250/api/auth";

        #endregion

        #region API Methods

        /// <summary>
        /// Регистрация пользователя
        /// POST /api/auth/register
        /// </summary>
        public async Task<AuthResponseDto> RegisterAsync(string username, string password)
        {
            try
            {
                var requestBody = new { username, password };
                var json = JsonConvert.SerializeObject(requestBody);

                var responseJson = await PostRequest($"{BASE_URL}/register", json);
                if (string.IsNullOrEmpty(responseJson))
                    return null;

                var response = JsonConvert.DeserializeObject<AuthResponseDto>(responseJson);
                return response;
            }
            catch (Exception e)
            {
                Debug.LogError($"[AuthAPI] Register error: {e.Message}");
                return new AuthResponseDto
                {
                    Success = false,
                    Message = $"Ошибка сети: {e.Message}"
                };
            }
        }

        /// <summary>
        /// Вход пользователя
        /// POST /api/auth/login
        /// </summary>
        public async Task<AuthResponseDto> LoginAsync(string username, string password)
        {
            try
            {
                var requestBody = new { username, password };
                var json = JsonConvert.SerializeObject(requestBody);

                var responseJson = await PostRequest($"{BASE_URL}/login", json);
                if (string.IsNullOrEmpty(responseJson))
                    return null;

                var response = JsonConvert.DeserializeObject<AuthResponseDto>(responseJson);
                return response;
            }
            catch (Exception e)
            {
                Debug.LogError($"[AuthAPI] Login error: {e.Message}");
                return new AuthResponseDto
                {
                    Success = false,
                    Message = $"Ошибка сети: {e.Message}"
                };
            }
        }

        #endregion

        #region HTTP Helpers

        private async Task<string> PostRequest(string url, string json)
        {
            using var request = new UnityWebRequest(url, "POST");
            request.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(json));
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            var operation = request.SendWebRequest();
            while (!operation.isDone)
            {
                await Task.Yield();
            }

            if (request.result == UnityWebRequest.Result.Success)
            {
                return request.downloadHandler.text;
            }
            else
            {
                Debug.LogError($"[AuthAPI] POST error: {request.error} - {request.downloadHandler.text}");

                // Попытка распарсить ошибку от сервера
                if (!string.IsNullOrEmpty(request.downloadHandler.text))
                {
                    try
                    {
                        var errorResponse = JsonConvert.DeserializeObject<ErrorResponseDto>(request.downloadHandler.text);
                        if (errorResponse != null && !string.IsNullOrEmpty(errorResponse.Message))
                        {
                            return JsonConvert.SerializeObject(errorResponse);
                        }
                    }
                    catch { }
                }

                return null;
            }
        }

        #endregion

        #region DTO Classes

        [Serializable]
        public class AuthResponseDto
        {
            [JsonProperty("success")]
            public bool Success;

            [JsonProperty("message")]
            public string Message;

            [JsonProperty("token")]
            public string Token;

            [JsonProperty("userId")]
            public string UserId;

            [JsonProperty("userGuid")]
            public string UserGuid;

            [JsonProperty("username")]
            public string Username;

            [JsonProperty("expiresAt")]
            public DateTime ExpiresAt;
        }

        [Serializable]
        public class ErrorResponseDto
        {
            [JsonProperty("message")]
            public string Message;
        }

        #endregion
    }
}
