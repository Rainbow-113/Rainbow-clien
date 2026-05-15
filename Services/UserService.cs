using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Rainbow.Models;
using System.Net.Http.Headers;

namespace Rainbow.Services
{
    public class UserService : IUserService
    {
        private readonly HttpClient _httpClient;

        private readonly string _baseUrl = "http://localhost:5000/api/auth";
        public UserService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
       
        public async Task<bool> RegisterAsync(RegisterDto registerDto)
        {
            var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/register", registerDto);
            return response.IsSuccessStatusCode;
        }
        public async Task<AuthResponse> LoginAsync(LoginDto loginDto)
        {

            var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/login", loginDto);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<AuthResponse>();
            }
            return null;
        }

        public async Task<UserProfileDto> GetProfileAsync(string token)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                // Kiểm tra lại URL: Nếu BaseUrl đã có "api/auth" thì ở đây chỉ cần "profile"
                var response = await _httpClient.GetAsync($"{_baseUrl}/profile");

                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<UserProfileDto>(jsonString);
                }
                else
                {
                    // ĐÂY LÀ ĐOẠN QUAN TRỌNG:
                    var errorContent = await response.Content.ReadAsStringAsync();
                    // In ra Console để biết Node.js đang trả về lỗi gì (401, 404, 500...)
                    Console.WriteLine($"API Error: {response.StatusCode} - {errorContent}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
            }
            return null;
        }

        public async Task<bool> UpdateProfileAsync(UserProfileDto model, string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Gửi yêu cầu PUT sang Node.js
            var response = await _httpClient.PutAsJsonAsync($"{_baseUrl}/update-profile", model);

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> ChangePasswordAsync(ChangePasswordDto model, string token)
        {

            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/change-password", model);
            return response.IsSuccessStatusCode;
        }

        public async Task<List<UserProfileDto>> GetAllUsersAsync(string token, string fullName = "")
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var url = $"{_baseUrl}/users";
            if (!string.IsNullOrEmpty(fullName)) url += $"?fullName={fullName}";

            var response = await _httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<UserProfileDto>>();
            }
            return new List<UserProfileDto>();
        }

        public async Task<bool> DeleteUserAsync(string id, string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.DeleteAsync($"{_baseUrl}/users/{id}");
            return response.IsSuccessStatusCode;
        }

    }
}
