using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using TestBlazorAssembly.ApiRequest.Models;

namespace TestBlazorAssembly.ApiRequest
{
    public class ApiRequestService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ApiRequestService> _logger;

        public ApiRequestService(HttpClient httpClient, ILogger<ApiRequestService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            var url = "api/auth/login";

            try
            {
                var response = await _httpClient.PostAsJsonAsync(url, request);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<AuthResponse>();
                    return result ?? new AuthResponse { Success = false, Message = "Login failed." };
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                return new AuthResponse
                {
                    Success = false,
                    Message = $"Login failed: {response.StatusCode}. {errorContent}"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при логине");
                return new AuthResponse
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<UserData> GetAllUsersAsync()
        {
            var url = "api/UsersLogins/getAllUsers";

            try
            {
                var response = await _httpClient.GetAsync(url).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                if (string.IsNullOrEmpty(responseContent))
                {
                    _logger.LogWarning("Ответ от сервера пуст.");
                    return new UserData();
                }

                var usersData = JsonSerializer.Deserialize<UserData>(responseContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return usersData ?? new UserData();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при запросе");
                return new UserData();
            }
        }

        public async Task<UserAddData> AddNewUser(ReqDataUser user)
        {
            var url = "api/UsersLogins/createNewUserAndLogin";

            try
            {
                var response = await _httpClient.PostAsJsonAsync(url, user);

                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();

                var userAddData = JsonSerializer.Deserialize<UserAddData>(responseContent);

                return userAddData ?? new UserAddData();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при запросе: {ex.Message}");
                return new UserAddData();
            }
        }

        public async Task<UserOperationResponse> UpdateUser(EditDataUser user)
        {
            var url = "api/UsersLogins/putUserAndLogin";

            try
            {
                var response = await _httpClient.PutAsJsonAsync(url, user);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<UserOperationResponse>(responseContent);
                    return result ?? new UserOperationResponse { success = true };
                }

                return new UserOperationResponse
                {
                    success = false,
                    message = $"Failed to update user. Status: {response.StatusCode}"
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при запросе: {ex.Message}");
                return new UserOperationResponse
                {
                    success = false,
                    message = ex.Message
                };
            }
        }

        public async Task<UserOperationResponse> DeleteUser(int userId)
        {
            var url = $"api/UsersLogins/deleteUser/{userId}";

            try
            {
                var response = await _httpClient.DeleteAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<UserOperationResponse>(responseContent);
                    return result ?? new UserOperationResponse { success = true };
                }

                return new UserOperationResponse
                {
                    success = false,
                    message = $"Failed to delete user. Status: {response.StatusCode}"
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при запросе: {ex.Message}");
                return new UserOperationResponse
                {
                    success = false,
                    message = ex.Message
                };
            }
        }

        public async Task<UserOperationResponse> RegisterAsync(string login, string password, string userName)
        {
            var url = "api/Auth/register";

            var request = new
            {
                Name = userName,
                Description = string.Empty,
                Login = login,
                Password = password,
                id_Role = 2
            };

            try
            {
                var response = await _httpClient.PostAsJsonAsync(url, request);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<UserOperationResponse>(responseContent);
                    return result ?? new UserOperationResponse { success = true };
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                return new UserOperationResponse
                {
                    success = false,
                    message = $"Registration failed: {response.StatusCode}. {errorContent}"
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при запросе: {ex.Message}");
                return new UserOperationResponse
                {
                    success = false,
                    message = ex.Message
                };
            }
        }
    }
}
