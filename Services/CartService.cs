


using Newtonsoft.Json;
using Rainbow.Models;
using System.Net.Http.Headers;
using System.Text;

namespace Rainbow.Services
{
   
    public class CartService : ICartService
    {
        private readonly HttpClient _httpClient;
        public CartService(HttpClient httpClient) { 
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("http://localhost:5000/api/"); // Địa chỉ Node.js của bạn
        }

        public async Task<bool> AddToCartAsync(CartItemDto item, string token)
        {
            // xác thức 
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // đóng gói
            var json = JsonConvert.SerializeObject(item);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // 3. Gửi yêu cầu POST sang Node.js
            var response = await _httpClient.PostAsync("cart/add", content);

            return response.IsSuccessStatusCode;
        }

       


        public async Task<CartDto> GetCartAsync(string token)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                // Gọi đến router.get('/') của Node.js
                var response = await _httpClient.GetAsync("cart");

                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine("DEBUG_JSON: " + jsonString);
                    // Tự động map: productName (JSON) -> ProductName (C#)
                    var cart = JsonConvert.DeserializeObject<CartDto>(jsonString);
                    return cart;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi lấy giỏ hàng: " + ex.Message);
            }

            return new CartDto { Items = new List<CartItemDetailDto>() };
        }


        public async Task<bool> RemoveFromCartAsync(string productId, string size, string color, string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            // Gửi yêu cầu DELETE sang Node.js
            var response = await _httpClient.DeleteAsync($"cart/remove/{productId}?size={size}&color={color}");
            return response.IsSuccessStatusCode;
        }
    }
}
