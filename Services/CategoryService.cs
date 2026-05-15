using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Rainbow.Models;      // Để sử dụng CategoryDto
using System.Net.Http.Json; // Thư viện để đọc JSON nhanh
using System.Text;
using System.Text.Json;
namespace Rainbow.Services
{
    public class CategoryService
    {
        private readonly HttpClient _httpClient;
        public CategoryService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<List<CategoryDto>> GetAllCategories()
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<List<CategoryDto>>("api/categories");
                return response ?? new List<CategoryDto>();
            }
            catch (Exception ex)
            {
                // Xử lý khi Node.js chưa chạy hoặc đường dẫn sai
                Console.WriteLine($"Lỗi gọi API Category: {ex.Message}");
                return new List<CategoryDto>();
            }
        }
        public async Task<List<CategoryDto>> GetActiveCategories()
        {
            // Gọi lại hàm GetAllCategories rồi lọc bằng LINQ
            var allCategories = await GetAllCategories();
            return allCategories.Where(c => c.IsActive == true).ToList();
        }
        //
        public async Task<bool> CreateCategory( CategoryDto category)
        {
            try
            {
                //  đổi Name -> name 
                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };

                var response = await _httpClient.PostAsJsonAsync("api/categories", category, options);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"API Error: {errorContent}");
                }
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                // Dòng này cực kỳ quan trọng để biết tại sao không gọi được API
                Console.WriteLine($"THÔNG BÁO LỖI KẾT NỐI: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Lỗi chi tiết: {ex.InnerException.Message}");
                }
                return false;
            }
        }

        public async Task<bool> DeleteCategoryAsync(string id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/categories/{id}");
                return response.IsSuccessStatusCode;
            }
            catch { return false; }
        }
        // 1. Lấy chi tiết Category theo ID để hiện lên trang Edit
        public async Task<CategoryDto> GetCategoryByIdAsync(string id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/categories/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<CategoryDto>(content);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi lấy Category: {ex.Message}");
            }
            return null;
        }
        // 2. Cập nhật Category
        public async Task<bool> UpdateCategoryAsync(string id, CategoryDto category)
        {
            try
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase // Để 'Name' thành 'name'
                };
                var response = await _httpClient.PutAsJsonAsync($"api/categories/{id}", category, options);
                return response.IsSuccessStatusCode;
            }
            catch { return false; }
        }
    }
}
