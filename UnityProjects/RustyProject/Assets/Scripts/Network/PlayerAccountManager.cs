using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace RustyProject.Network
{
    public class PlayerAccountManager : MonoBehaviour
    {
        private const string BaseApiUrl = "http://localhost:5268/api";
        private const string AuthTokenPrefsKey = "RustyApi.AuthToken";
        private const string AuthUsernamePrefsKey = "RustyApi.Username";
        private static readonly string[] KnownLevelKeys = { "FirstLevelData", "SecondLevel", "ThirdLevel" };

        private Task restoreSessionTask;
        private string authToken = string.Empty;
        private string lastErrorMessage = string.Empty;

        public static PlayerAccountManager Instance { get; private set; }

        public event Action OnProfileChanged;
        public event Action<string> OnStatusMessage;

        public UserProfileDto CurrentProfile { get; private set; }
        public bool IsAuthenticated => !string.IsNullOrEmpty(authToken) && CurrentProfile != null;

        // ? Bootstrap : создаёт глобальный менеджер аккаунта до загрузки сцен
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Bootstrap()
        {
            if (Instance != null) return;

            GameObject managerObject = new GameObject("PlayerAccountManager");
            managerObject.AddComponent<PlayerAccountManager>();
        }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        // ? Start : запускает восстановление сессии из сохранённого токена
        private void Start()
        {
            _ = EnsureSessionRestoredAsync();
        }

        // ? EnsureSessionRestoredAsync : один раз восстанавливает сессию игрока при старте
        public Task EnsureSessionRestoredAsync()
        {
            restoreSessionTask ??= RestoreSessionInternalAsync();
            return restoreSessionTask;
        }

        // ? LoginAsync : выполняет вход пользователя через API
        public async Task<bool> LoginAsync(string username, string password)
        {
            return await AuthenticateAsync("login", username, password);
        }

        // ? RegisterAsync : выполняет регистрацию пользователя через API
        public async Task<bool> RegisterAsync(string username, string password)
        {
            return await AuthenticateAsync("register", username, password);
        }

        // ? RefreshProfileAsync : запрашивает профиль текущего пользователя с сервера
        public async Task<bool> RefreshProfileAsync(bool silent = false)
        {
            if (string.IsNullOrEmpty(authToken))
            {
                SetAnonymousState(false);
                return false;
            }

            ApiResult<UserProfileDto> profileResult = await SendAuthorizedRequestAsync<UserProfileDto>("GET", "/users/me", null);
            if (!profileResult.Success || profileResult.Data == null)
            {
                if (!silent)
                {
                    EmitStatus(string.IsNullOrEmpty(profileResult.ErrorMessage) ? "Не удалось загрузить профиль." : profileResult.ErrorMessage);
                }

                return false;
            }

            ApplyProfile(profileResult.Data);
            return true;
        }

        // ? AddCoinsAsync : сохраняет добавленные монеты в удалённый профиль
        public async Task<bool> AddCoinsAsync(int coinsDelta)
        {
            if (coinsDelta <= 0 || string.IsNullOrEmpty(authToken))
            {
                return false;
            }

            UpdateCoinsDto payload = new UpdateCoinsDto { coinsDelta = coinsDelta };
            ApiResult<UserProfileDto> profileResult = await SendAuthorizedRequestAsync<UserProfileDto>("PUT", "/users/me/coins", payload);
            if (!profileResult.Success || profileResult.Data == null)
            {
                EmitStatus(string.IsNullOrEmpty(profileResult.ErrorMessage) ? "Не удалось сохранить монеты." : profileResult.ErrorMessage);
                return false;
            }

            ApplyProfile(profileResult.Data);
            return true;
        }

        // ? SaveProgressAsync : отправляет на сервер прогресс уровней и звёзд
        public async Task<bool> SaveProgressAsync(int lastCompletedLevelIndex, LevelProgressDto[] levelProgresses)
        {
            if (string.IsNullOrEmpty(authToken))
            {
                return false;
            }

            UpdateProgressDto payload = new UpdateProgressDto
            {
                lastCompletedLevelIndex = Mathf.Max(0, lastCompletedLevelIndex),
                levelProgresses = levelProgresses ?? Array.Empty<LevelProgressDto>()
            };

            ApiResult<UserProfileDto> profileResult = await SendAuthorizedRequestAsync<UserProfileDto>("PUT", "/users/me/progress", payload);
            if (!profileResult.Success || profileResult.Data == null)
            {
                EmitStatus(string.IsNullOrEmpty(profileResult.ErrorMessage) ? "Не удалось сохранить прогресс." : profileResult.ErrorMessage);
                return false;
            }

            ApplyProfile(profileResult.Data);
            return true;
        }

        // ? GetLeaderboardAsync : получает таблицу результатов по монетам
        public async Task<LeaderboardEntryDto[]> GetLeaderboardAsync(int limit)
        {
            ApiResponse response = await SendRequestAsync("GET", $"/leaderboard?limit={Mathf.Clamp(limit, 1, 100)}", null, false);
            if (!response.Success || string.IsNullOrEmpty(response.Json))
            {
                return Array.Empty<LeaderboardEntryDto>();
            }

            LeaderboardResponseWrapper wrapper = JsonUtility.FromJson<LeaderboardResponseWrapper>("{\"items\":" + response.Json + "}");
            return wrapper?.items ?? Array.Empty<LeaderboardEntryDto>();
        }

        // ? GetKnownCompletedLevelIndex : возвращает последний сохранённый индекс завершённого уровня
        public int GetKnownCompletedLevelIndex()
        {
            return CurrentProfile != null ? CurrentProfile.lastCompletedLevelIndex : 0;
        }

        // ? Logout : очищает локальную сессию и переводит пользователя в гостевой режим
        public void Logout()
        {
            PlayerPrefs.DeleteKey(AuthTokenPrefsKey);
            PlayerPrefs.DeleteKey(AuthUsernamePrefsKey);
            PlayerPrefs.Save();
            authToken = string.Empty;
            SetAnonymousState(true);
            EmitStatus("Вы вышли из аккаунта.");
        }

        // ? AuthenticateAsync : общий метод для логина и регистрации
        private async Task<bool> AuthenticateAsync(string authPath, string username, string password)
        {
            AuthRequestDto payload = new AuthRequestDto
            {
                username = username.Trim(),
                password = password
            };

            ApiResult<AuthResponseDto> response = await SendAnonymousRequestAsync<AuthResponseDto>("POST", $"/auth/{authPath}", payload);
            if (!response.Success || response.Data == null || string.IsNullOrEmpty(response.Data.token))
            {
                EmitStatus(string.IsNullOrEmpty(response.ErrorMessage)
                    ? authPath == "register" ? "Регистрация не удалась." : "Вход не удался."
                    : response.ErrorMessage);
                return false;
            }

            authToken = response.Data.token;
            PlayerPrefs.SetString(AuthTokenPrefsKey, authToken);
            PlayerPrefs.SetString(AuthUsernamePrefsKey, response.Data.username ?? username);
            PlayerPrefs.Save();

            bool refreshed = await RefreshProfileAsync();
            if (!refreshed)
            {
                EmitStatus("Авторизация прошла, но профиль не загрузился.");
            }
            else
            {
                EmitStatus(authPath == "register" ? "Регистрация успешна." : "Вход выполнен.");
            }

            return refreshed;
        }

        // ? RestoreSessionInternalAsync : пробует автоматически восстановить сохранённую авторизацию
        private async Task RestoreSessionInternalAsync()
        {
            authToken = PlayerPrefs.GetString(AuthTokenPrefsKey, string.Empty);
            if (string.IsNullOrEmpty(authToken))
            {
                SetAnonymousState(false);
                return;
            }

            bool refreshed = await RefreshProfileAsync(true);
            if (!refreshed)
            {
                Logout();
            }
        }

        // ? ApplyProfile : сохраняет полученный профиль и синхронизирует локальный прогресс
        private void ApplyProfile(UserProfileDto profile)
        {
            CurrentProfile = profile;
            SyncLocalProgress(profile);
            OnProfileChanged?.Invoke();
        }

        // ? SetAnonymousState : переводит клиента в гостевой режим и по желанию очищает локальный прогресс
        private void SetAnonymousState(bool clearLocalProgress)
        {
            CurrentProfile = null;
            if (clearLocalProgress)
            {
                ClearLocalProgress();
            }

            OnProfileChanged?.Invoke();
        }

        // ? SyncLocalProgress : копирует серверный прогресс в локальные PlayerPrefs для существующей логики игры
        private void SyncLocalProgress(UserProfileDto profile)
        {
            ClearLocalProgress();
            if (profile?.levelProgresses == null)
            {
                PlayerPrefs.Save();
                return;
            }

            for (int i = 0; i < profile.levelProgresses.Length; i++)
            {
                LevelProgressDto progress = profile.levelProgresses[i];
                if (progress == null || string.IsNullOrEmpty(progress.levelKey))
                {
                    continue;
                }

                PlayerPrefs.SetInt($"LevelStars_{progress.levelKey}", Mathf.Clamp(progress.starsCollected, 0, LevelManager.StarsPerLevel));
            }

            PlayerPrefs.Save();
        }

        // ? ClearLocalProgress : удаляет локально сохранённые звёзды известных уровней
        private void ClearLocalProgress()
        {
            for (int i = 0; i < KnownLevelKeys.Length; i++)
            {
                PlayerPrefs.DeleteKey($"LevelStars_{KnownLevelKeys[i]}");
            }

            PlayerPrefs.Save();
        }

        // ? EmitStatus : отправляет строку статуса в UI формы
        private void EmitStatus(string message)
        {
            OnStatusMessage?.Invoke(message);
        }

        // ? SendAnonymousRequestAsync : отправляет запрос без JWT и возвращает данные или ошибку
        private Task<ApiResult<T>> SendAnonymousRequestAsync<T>(string method, string path, object payload) where T : class
        {
            return SendJsonRequestAsync<T>(method, path, payload, false);
        }

        // ? SendAuthorizedRequestAsync : отправляет авторизованный запрос с Bearer-токеном
        private Task<ApiResult<T>> SendAuthorizedRequestAsync<T>(string method, string path, object payload) where T : class
        {
            return SendJsonRequestAsync<T>(method, path, payload, true);
        }

        // ? SendJsonRequestAsync : преобразует JSON-ответ API в DTO и пробрасывает текст ошибки
        private async Task<ApiResult<T>> SendJsonRequestAsync<T>(string method, string path, object payload, bool authorized) where T : class
        {
            ApiResponse response = await SendRequestAsync(method, path, payload, authorized);
            if (!response.Success || string.IsNullOrEmpty(response.Json))
            {
                return new ApiResult<T>(null, response.ErrorMessage);
            }

            return new ApiResult<T>(JsonUtility.FromJson<T>(response.Json), response.ErrorMessage);
        }

        // ? SendRequestAsync : выполняет HTTP-запрос и извлекает текст ошибки из ответа сервера
        private async Task<ApiResponse> SendRequestAsync(string method, string path, object payload, bool authorized)
        {
            using UnityWebRequest request = new UnityWebRequest(BaseApiUrl + path, method);
            request.downloadHandler = new DownloadHandlerBuffer();

            if (payload != null)
            {
                byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(JsonUtility.ToJson(payload));
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                request.SetRequestHeader("Content-Type", "application/json");
            }

            if (authorized && !string.IsNullOrEmpty(authToken))
            {
                request.SetRequestHeader("Authorization", $"Bearer {authToken}");
            }

            UnityWebRequestAsyncOperation operation = request.SendWebRequest();
            while (!operation.isDone)
            {
                await Task.Yield();
            }

            if (request.result != UnityWebRequest.Result.Success)
            {
                lastErrorMessage = ExtractErrorMessage(request.downloadHandler.text, request.error);
                Debug.LogWarning($"[RustyAPI] {method} {path} failed: {request.responseCode} {request.error} {request.downloadHandler.text}");
                return new ApiResponse(false, null, lastErrorMessage);
            }

            lastErrorMessage = string.Empty;
            return new ApiResponse(true, request.downloadHandler.text, string.Empty);
        }

        // ? ExtractErrorMessage : превращает JSON ошибки API в понятный текст для формы
        private string ExtractErrorMessage(string responseText, string fallbackMessage)
        {
            if (string.IsNullOrWhiteSpace(responseText))
            {
                return string.IsNullOrWhiteSpace(fallbackMessage) ? "Ошибка соединения с API." : fallbackMessage;
            }

            string directMessage = ExtractJsonValue(responseText, "message");
            if (!string.IsNullOrWhiteSpace(directMessage))
            {
                return directMessage;
            }

            MatchCollection validationMessages = Regex.Matches(responseText, "\\[(.*?)\\]");
            if (validationMessages.Count > 0)
            {
                string combinedMessages = string.Empty;
                for (int i = 0; i < validationMessages.Count; i++)
                {
                    Match textMatch = Regex.Match(validationMessages[i].Value, "\"([^\"]+)\"");
                    if (!textMatch.Success) continue;

                    string message = Regex.Unescape(textMatch.Groups[1].Value);
                    if (string.IsNullOrWhiteSpace(message)) continue;

                    if (!string.IsNullOrEmpty(combinedMessages))
                    {
                        combinedMessages += "\n";
                    }

                    combinedMessages += message;
                }

                if (!string.IsNullOrWhiteSpace(combinedMessages))
                {
                    return combinedMessages;
                }
            }

            return string.IsNullOrWhiteSpace(fallbackMessage) ? "Произошла ошибка запроса." : fallbackMessage;
        }

        // ? ExtractJsonValue : достаёт строковое поле из простого JSON-объекта
        private static string ExtractJsonValue(string responseText, string key)
        {
            Match match = Regex.Match(responseText, $"\"{Regex.Escape(key)}\"\\s*:\\s*\"(?<value>.*?)\"");
            return match.Success ? Regex.Unescape(match.Groups["value"].Value) : string.Empty;
        }

        private readonly struct ApiResponse
        {
            public ApiResponse(bool success, string json, string errorMessage)
            {
                Success = success;
                Json = json;
                ErrorMessage = errorMessage;
            }

            public bool Success { get; }
            public string Json { get; }
            public string ErrorMessage { get; }
        }

        private readonly struct ApiResult<T> where T : class
        {
            public ApiResult(T data, string errorMessage)
            {
                Data = data;
                ErrorMessage = errorMessage;
            }

            public T Data { get; }
            public string ErrorMessage { get; }
            public bool Success => Data != null;
        }
    }
}
