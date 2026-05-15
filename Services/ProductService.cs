using Rainbow.Models;
using System.Collections.Generic; 
using System.Text.Json;
using System.Threading.Tasks;    
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
                var allProducts = await response.Content.ReadFromJsonAsync<List<ProductDto>>();

                // --- THÊM DÒNG NÀY ĐỂ LỌC ---
                // Chỉ trả về những sản phẩm có IsActive là true
                return allProducts?.Where(p => p.IsActive).ToList() ?? new List<ProductDto>();
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
        //checkname
        public async Task<bool> IsNameExistsAsync(string name)
        {
            try
            {
                var url = $"api/products/check-name?name={Uri.EscapeDataString(name.Trim())}";
                var response = await _httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                   
                    return content.Trim().ToLower().Replace("\"", "") == "true";
                }
                return false;
            }
            catch { return false; }
        }
        public async Task<bool> CreateProductAsync(ProductDto productDto)
        {
            try
            {
                using var content = new MultipartFormDataContent(); 

               
                content.Add(new StringContent(productDto.Name ?? ""), "name");
                content.Add(new StringContent(productDto.Price.ToString()), "price");
                content.Add(new StringContent(productDto.Description ?? ""), "description");
                content.Add(new StringContent(productDto.IsActive.ToString().ToLower()), "isActive");

                if (!string.IsNullOrEmpty(productDto.CategoryId))
                {
                    content.Add(new StringContent(productDto.CategoryId), "categoryId");
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
                // GỬI REQUEST - Dùng URL tuyệt đối để test
                var response = await _httpClient.PostAsync("http://localhost:5000/api/products", content);

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                
                System.Diagnostics.Debug.WriteLine("CRITICAL ERROR: " + ex.Message);
                if (ex.InnerException != null)
                    System.Diagnostics.Debug.WriteLine("INNER ERROR: " + ex.InnerException.Message);

                return false;
            }
        }
        
        public async Task<bool> DeleteProductAsync(string id)
        {
            try {
                //gửi yêu cầu dele qua api
                var response = await _httpClient.DeleteAsync($"api/products/{id}");


                return response.IsSuccessStatusCode;

            }
            catch(Exception ex)
            {
                // Ghi log lỗi nếu cần thiết
                Console.WriteLine($"Lỗi khi gọi API xóa Product: {ex.Message}");
                return false;
            } 
        }

        public async Task<bool> UpdateProductAsync(string id, ProductDto productDto)
        {
            try
            {
                // Khi gửi sang Node.js, ta cần chuyển Id của Category về dạng chuỗi đơn giản
                // Nếu Category không null, lấy Id của nó gán vào CategoryId
                if (productDto.Category != null && string.IsNullOrEmpty(productDto.CategoryId))
                {
                    productDto.CategoryId = productDto.Category.Id;
                }

                var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
                var response = await _httpClient.PutAsJsonAsync($"api/products/{id}", productDto, options);

                return response.IsSuccessStatusCode;
            }
            catch { return false; }
        }

    }
}
