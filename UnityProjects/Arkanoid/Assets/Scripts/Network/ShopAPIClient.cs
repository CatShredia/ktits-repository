using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Arkanoid.Network
{
    public class ShopAPIClient : MonoBehaviour
    {
        private static ShopAPIClient _instance;
        public static ShopAPIClient Instance => _instance;

        void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private const string BASE_URL = "http://localhost:5250/api/shop";
        private string _authToken;

        public void SetAuthToken(string token) => _authToken = token;

        public async Task<List<ShopSkinDto>> GetAllSkins()
        {
            Debug.Log($"[ShopAPIClient] GET {BASE_URL}");
            var json = await GetRequest(BASE_URL);
            Debug.Log($"[ShopAPIClient] GetAllSkins response: {(string.IsNullOrEmpty(json) ? "EMPTY" : json.Substring(0, Math.Min(200, json.Length)))}");
            return string.IsNullOrEmpty(json) ? new List<ShopSkinDto>() : JsonConvert.DeserializeObject<List<ShopSkinDto>>(json);
        }

        public async Task<ShopInventoryDto> GetInventory()
        {
            Debug.Log($"[ShopAPIClient] GET {BASE_URL}/inventory (auth: true)");
            var json = await GetRequest($"{BASE_URL}/inventory", true);
            if (string.IsNullOrEmpty(json))
            {
                Debug.LogWarning("[ShopAPIClient] GetInventory response: EMPTY or NULL");
                return null;
            }
            Debug.Log($"[ShopAPIClient] GetInventory response: {json.Substring(0, Math.Min(500, json.Length))}");
            return JsonConvert.DeserializeObject<ShopInventoryDto>(json);
        }

        public async Task<ShopPurchaseResponse> PurchaseSkin(int skinId)
        {
            var json = JsonConvert.SerializeObject(new { skinId });
            Debug.Log($"[ShopAPIClient] POST {BASE_URL}/purchase (auth: true) body: {json}");
            var responseJson = await PostRequest($"{BASE_URL}/purchase", json, true);
            Debug.Log($"[ShopAPIClient] PurchaseSkin response: {(string.IsNullOrEmpty(responseJson) ? "EMPTY" : responseJson)}");
            return string.IsNullOrEmpty(responseJson) ? null : JsonConvert.DeserializeObject<ShopPurchaseResponse>(responseJson);
        }

        public async Task<ShopEquipResponse> EquipSkin(int userSkinId)
        {
            var json = JsonConvert.SerializeObject(new { userSkinId });
            Debug.Log($"[ShopAPIClient] POST {BASE_URL}/equip (auth: true) body: {json}");
            var responseJson = await PostRequest($"{BASE_URL}/equip", json, true);
            Debug.Log($"[ShopAPIClient] EquipSkin response: {(string.IsNullOrEmpty(responseJson) ? "EMPTY" : responseJson)}");
            return string.IsNullOrEmpty(responseJson) ? null : JsonConvert.DeserializeObject<ShopEquipResponse>(responseJson);
        }

        private async Task<string> GetRequest(string url, bool auth = false)
        {
            using var request = new UnityWebRequest(url, "GET");
            request.downloadHandler = new DownloadHandlerBuffer();

            if (auth && !string.IsNullOrEmpty(_authToken))
            {
                request.SetRequestHeader("Authorization", $"Bearer {_authToken}");
                Debug.Log($"[ShopAPIClient] GET auth header set (token starts with: {_authToken.Substring(0, Math.Min(20, _authToken.Length))}...)");
            }
            else if (auth)
            {
                Debug.LogWarning("[ShopAPIClient] GET auth requested but token is empty/null");
            }

            Debug.Log($"[ShopAPIClient] Sending GET {url}");
            await request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log($"[ShopAPIClient] GET {url} SUCCESS ({request.responseCode})");
                return request.downloadHandler.text;
            }

            Debug.LogWarning($"[ShopAPIClient] GET {url} FAILED: result={request.result}, error={request.error}");
            if (request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogWarning($"[ShopAPIClient] GET HTTP Status: {request.responseCode}");
            }

            var errorText = request.downloadHandler?.text;
            if (!string.IsNullOrEmpty(errorText))
            {
                Debug.LogWarning($"[ShopAPIClient] GET error body: {errorText.Substring(0, Math.Min(500, errorText.Length))}");
            }

            return null;
        }

        private async Task<string> PostRequest(string url, string json, bool auth = false)
        {
            using var request = new UnityWebRequest(url, "POST");
            request.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(json));
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            if (auth && !string.IsNullOrEmpty(_authToken))
            {
                request.SetRequestHeader("Authorization", $"Bearer {_authToken}");
                Debug.Log($"[ShopAPIClient] POST auth header set (token starts with: {_authToken.Substring(0, Math.Min(20, _authToken.Length))}...)");
            }
            else if (auth)
            {
                Debug.LogWarning("[ShopAPIClient] POST auth requested but token is empty/null");
            }

            Debug.Log($"[ShopAPIClient] Sending POST {url}");
            await request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success ||
                request.result == UnityWebRequest.Result.ProtocolError ||
                request.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.Log($"[ShopAPIClient] POST {url} result={request.result}, HTTP={(request.result != UnityWebRequest.Result.Success ? request.responseCode.ToString() : "200 OK")}");
                var responseText = request.downloadHandler.text;
                if (!string.IsNullOrEmpty(responseText))
                {
                    Debug.Log($"[ShopAPIClient] POST response: {responseText.Substring(0, Math.Min(500, responseText.Length))}");
                }
                return responseText;
            }

            Debug.LogWarning($"[ShopAPIClient] POST {url} FAILED: result={request.result}, error={request.error}");
            return null;
        }
    }

    [Serializable]
    public class ShopSkinDto
    {
        [JsonProperty("id")] public int Id;
        [JsonProperty("name")] public string Name;
        [JsonProperty("skinType")] public string SkinType;
        [JsonProperty("rarity")] public string Rarity;
        [JsonProperty("price")] public int Price;
        [JsonProperty("description")] public string Description;
        [JsonProperty("texturePath")] public string TexturePath;
        [JsonProperty("prefabPath")] public string PrefabPath;
        [JsonProperty("isActive")] public bool IsActive;
        [JsonProperty("isStarter")] public bool IsStarter;
    }

    [Serializable]
    public class ShopUserSkinDto
    {
        [JsonProperty("id")] public int Id;
        [JsonProperty("skinId")] public int SkinId;
        [JsonProperty("skinName")] public string SkinName;
        [JsonProperty("skinType")] public string SkinType;
        [JsonProperty("isEquipped")] public bool IsEquipped;
        [JsonProperty("acquiredAt")] public string AcquiredAt;
        [JsonProperty("acquisitionMethod")] public string AcquisitionMethod;
    }

    [Serializable]
    public class ShopInventoryDto
    {
        [JsonProperty("userId")] public int UserId;
        [JsonProperty("username")] public string Username;
        [JsonProperty("coins")] public int Coins;
        [JsonProperty("skins")] public List<ShopUserSkinDto> Skins;
        [JsonProperty("equippedPlatformSkin")] public ShopUserSkinDto EquippedPlatformSkin;
        [JsonProperty("equippedBallSkin")] public ShopUserSkinDto EquippedBallSkin;
    }

    [Serializable]
    public class ShopPurchaseResponse
    {
        [JsonProperty("success")] public bool Success;
        [JsonProperty("message")] public string Message;
        [JsonProperty("remainingCoins")] public int RemainingCoins;
        [JsonProperty("purchasedSkin")] public ShopUserSkinDto PurchasedSkin;
        [JsonProperty("errorCode")] public int? ErrorCode;
    }

    [Serializable]
    public class ShopEquipResponse
    {
        [JsonProperty("success")] public bool Success;
        [JsonProperty("message")] public string Message;
        [JsonProperty("equippedSkinId")] public int EquippedSkinId;
        [JsonProperty("errorCode")] public int? ErrorCode;
    }
}
