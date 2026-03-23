using Microsoft.EntityFrameworkCore;
using Rainbow.Models;      // Để sử dụng CategoryDto
using System.Net.Http.Json; // Thư viện để đọc JSON nhanh
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
        // Hàm 2: Chỉ lấy những cái đang HOẠT ĐỘNG (Dùng cho Trang chủ)
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
    }
}
