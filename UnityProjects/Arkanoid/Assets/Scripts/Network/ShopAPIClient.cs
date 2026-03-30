using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Arkanoid.Network
{
    /// <summary>
    /// API клиент для магазина скинов.
    /// </summary>
    public class ShopAPIClient : MonoBehaviour
    {
        private static ShopAPIClient _instance;
        public static ShopAPIClient Instance
        {
            get
            {
                if (_instance == null)
                {
                    var go = new GameObject("ShopAPIClient");
                    _instance = go.AddComponent<ShopAPIClient>();
                    DontDestroyOnLoad(go);
                }
                return _instance;
            }
        }

        private const string BASE_URL = "http://localhost:5250/api/shop";
        private string _authToken;

        public void SetAuthToken(string token)
        {
            _authToken = token;
        }

        /// <summary>
        /// Получить все доступные скины
        /// GET /api/shop
        /// </summary>
        public async Task<List<ShopSkinDto>> GetAllSkins()
        {
            try
            {
                var json = await GetRequest(BASE_URL);
                if (string.IsNullOrEmpty(json))
                    return new List<ShopSkinDto>();

                var skins = JsonConvert.DeserializeObject<List<ShopSkinDto>>(json);
                return skins ?? new List<ShopSkinDto>();
            }
            catch (Exception e)
            {
                Debug.LogError($"[ShopAPI] Error getting skins: {e.Message}");
                return new List<ShopSkinDto>();
            }
        }

        /// <summary>
        /// Получить инвентарь пользователя
        /// GET /api/shop/inventory
        /// </summary>
        public async Task<ShopInventoryDto> GetInventory()
        {
            try
            {
                var json = await GetRequest($"{BASE_URL}/inventory", true);
                if (string.IsNullOrEmpty(json))
                    return null;

                var inventory = JsonConvert.DeserializeObject<ShopInventoryDto>(json);
                return inventory;
            }
            catch (Exception e)
            {
                Debug.LogError($"[ShopAPI] Error getting inventory: {e.Message}");
                return null;
            }
        }

        /// <summary>
        /// Купить скин
        /// POST /api/shop/purchase
        /// </summary>
        public async Task<ShopPurchaseResponse> PurchaseSkin(int skinId)
        {
            try
            {
                var requestBody = new { skinId };
                var json = JsonConvert.SerializeObject(requestBody);

                var responseJson = await PostRequest($"{BASE_URL}/purchase", json, true);
                if (string.IsNullOrEmpty(responseJson))
                    return null;

                var response = JsonConvert.DeserializeObject<ShopPurchaseResponse>(responseJson);
                return response;
            }
            catch (Exception e)
            {
                Debug.LogError($"[ShopAPI] Error purchasing skin: {e.Message}");
                return new ShopPurchaseResponse
                {
                    Success = false,
                    Message = $"Ошибка сети: {e.Message}"
                };
            }
        }

        /// <summary>
        /// Экипировать скин
        /// POST /api/shop/equip
        /// </summary>
        public async Task<ShopEquipResponse> EquipSkin(int userSkinId)
        {
            try
            {
                var requestBody = new { userSkinId };
                var json = JsonConvert.SerializeObject(requestBody);

                var responseJson = await PostRequest($"{BASE_URL}/equip", json, true);
                if (string.IsNullOrEmpty(responseJson))
                    return null;

                var response = JsonConvert.DeserializeObject<ShopEquipResponse>(responseJson);
                return response;
            }
            catch (Exception e)
            {
                Debug.LogError($"[ShopAPI] Error equipping skin: {e.Message}");
                return new ShopEquipResponse
                {
                    Success = false,
                    Message = $"Ошибка сети: {e.Message}"
                };
            }
        }

        private async Task<string> GetRequest(string url, bool auth = false)
        {
            using var request = new UnityWebRequest(url, "GET");
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            if (auth && !string.IsNullOrEmpty(_authToken))
            {
                request.SetRequestHeader("Authorization", $"Bearer {_authToken}");
            }

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
                Debug.LogError($"[ShopAPI] GET error: {request.error} - {request.downloadHandler.text}");
                return null;
            }
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
            }

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
                Debug.LogError($"[ShopAPI] POST error: {request.error} - {request.downloadHandler.text}");
                return null;
            }
        }
    }

    /// <summary>
    /// DTO скина (ответ от API)
    /// </summary>
    [Serializable]
    public class ShopSkinDto
    {
        [JsonProperty("id")]
        public int Id;

        [JsonProperty("name")]
        public string Name;

        [JsonProperty("skinType")]
        public string SkinType;

        [JsonProperty("rarity")]
        public string Rarity;

        [JsonProperty("price")]
        public int Price;

        [JsonProperty("description")]
        public string Description;

        [JsonProperty("texturePath")]
        public string TexturePath;

        [JsonProperty("isActive")]
        public bool IsActive;

        [JsonProperty("isStarter")]
        public bool IsStarter;
    }

    /// <summary>
    /// DTO скина пользователя (ответ от API)
    /// </summary>
    [Serializable]
    public class ShopUserSkinDto
    {
        [JsonProperty("id")]
        public int Id;

        [JsonProperty("skinId")]
        public int SkinId;

        [JsonProperty("skinName")]
        public string SkinName;

        [JsonProperty("skinType")]
        public string SkinType;

        [JsonProperty("isEquipped")]
        public bool IsEquipped;

        [JsonProperty("acquiredAt")]
        public string AcquiredAt;

        [JsonProperty("acquisitionMethod")]
        public string AcquisitionMethod;
    }

    /// <summary>
    /// DTO инвентаря пользователя (ответ от API)
    /// </summary>
    [Serializable]
    public class ShopInventoryDto
    {
        [JsonProperty("userId")]
        public int UserId;

        [JsonProperty("username")]
        public string Username;

        [JsonProperty("coins")]
        public int Coins;

        [JsonProperty("skins")]
        public List<ShopUserSkinDto> Skins;

        [JsonProperty("equippedPlatformSkin")]
        public ShopUserSkinDto EquippedPlatformSkin;

        [JsonProperty("equippedBallSkin")]
        public ShopUserSkinDto EquippedBallSkin;
    }

    /// <summary>
    /// Ответ на покупку скина
    /// </summary>
    [Serializable]
    public class ShopPurchaseResponse
    {
        [JsonProperty("success")]
        public bool Success;

        [JsonProperty("message")]
        public string Message;

        [JsonProperty("remainingCoins")]
        public int RemainingCoins;

        [JsonProperty("purchasedSkin")]
        public ShopUserSkinDto PurchasedSkin;
    }

    /// <summary>
    /// Ответ на экипировку скина
    /// </summary>
    [Serializable]
    public class ShopEquipResponse
    {
        [JsonProperty("success")]
        public bool Success;

        [JsonProperty("message")]
        public string Message;

        [JsonProperty("equippedSkinId")]
        public int EquippedSkinId;
    }
}
