using Rainbow.Models;
using System.Collections.Generic; // Để hiểu List<>
using System.Text.Json;
using System.Threading.Tasks;    // Để hiểu Task<>
namespace Rainbow.Services
{
    public class ProductService
    {
        private readonly HttpClient _httpClient;
        public ProductService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<ProductDto>> GetAllProducts(string? categoryId = null, string? searchTerm = null)
        {
            // 1. Khởi tạo URL cơ sở với tham số adminView để lấy toàn bộ dữ liệu
            string url = "api/products?adminView=true";
            // 2. Nếu có categoryId thì nối thêm vào URL bằng dấu &
            if (!string.IsNullOrEmpty(categoryId))
            {
                url += $"&categoryId={categoryId}";
            }
            if (!string.IsNullOrEmpty(searchTerm))
            {
                url += $"&searchTerm={searchTerm}";
            }
            System.Diagnostics.Debug.WriteLine(">>> URL THỰC TẾ GỬI ĐI: " + url);
            // 3. Gọi API và trả về kết quả
            try
            {
                var response = await _httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<List<ProductDto>>()
                           ?? new List<ProductDto>();
                }
            }
            catch (Exception ex)
            {
                // In lỗi ra cửa sổ Output của Visual Studio để debug
                System.Diagnostics.Debug.WriteLine($"Lỗi gọi API: {ex.Message}");
            }

            return new List<ProductDto>();
        }

        public async Task<List<ProductDto>> GetActiveProducts(string? categoryId = null)
        {
            // Tạo URL cơ sở
            string url = "api/products";
            if (!string.IsNullOrEmpty(categoryId))
            {
                url += $"?categoryId={categoryId}";
            }
            var response = await _httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<ProductDto>>();
            }
            return new List<ProductDto>();
        }
        public async Task<ProductDto> GetProductById(string id)
        {
            // Gọi đến bên Node.js
            var response = await _httpClient.GetAsync($"api/products/{id}");

            if (response.IsSuccessStatusCode)
            {
                // "Bóc tem" gói hàng JSON và chuyển thành đối tượng ProductDto
                var rawJson = await response.Content.ReadAsStringAsync();
                if (string.IsNullOrEmpty(rawJson) || !rawJson.Trim().StartsWith("{"))
                {
                    return null;
                }
                return JsonSerializer.Deserialize<ProductDto>(rawJson, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
            return null;
        }
        public async Task<List<ProductDto>> SearchProducts(string keyword)
        {
            return await _httpClient.GetFromJsonAsync<List<ProductDto>>($"api/products/search?name={keyword}");
        }

        public async Task<bool> CreateProductAsync(ProductDto productDto)
        {
            try
            {
                using var content = new MultipartFormDataContent();

                // Thêm dữ liệu (Sử dụng toán tử ?? "" để tránh lỗi Null)
                content.Add(new StringContent(productDto.Name ?? ""), "name");
                content.Add(new StringContent(productDto.Price.ToString()), "price");
                content.Add(new StringContent(productDto.Description ?? ""), "description");
                content.Add(new StringContent(productDto.IsActive.ToString().ToLower()), "isActive");

                if (productDto.Category != null && !string.IsNullOrEmpty(productDto.Category.Id))
                {
                    content.Add(new StringContent(productDto.Category.Id), "categoryId");
                }

                if (!string.IsNullOrEmpty(productDto.ImagePath))
                {
                    content.Add(new StringContent(productDto.ImagePath), "imagePath");
                }

                // Kiểm tra danh sách ảnh phụ
                if (productDto.Images != null)
                {
                    foreach (var img in productDto.Images)
                    {
                        content.Add(new StringContent(img), "images");
                    }
                }
                var debugContent = await content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine(">>> DỮ LIỆU C# CHUẨN BỊ GỬI: " + debugContent);
                // GỬI REQUEST - Dùng URL tuyệt đối để test nhanh nhất
                var response = await _httpClient.PostAsync("http://localhost:5000/api/products", content);

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                // Ghi lại lỗi chính xác vào cửa sổ Output thay vì sập web
                System.Diagnostics.Debug.WriteLine("CRITICAL ERROR: " + ex.Message);
                if (ex.InnerException != null)
                    System.Diagnostics.Debug.WriteLine("INNER ERROR: " + ex.InnerException.Message);

                return false; // Trả về false để Controller xử lý tiếp, không làm sập app
            }
        }
    }
}
