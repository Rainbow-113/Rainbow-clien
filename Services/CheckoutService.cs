using Newtonsoft.Json;
using Rainbow.Models;
using System.Net.Http.Headers;
using System.Text;

namespace Rainbow.Services
{
    public class CheckoutService : ICheckoutService
    {
        private readonly HttpClient _httpClient;

        public CheckoutService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<bool> PlaceOrderAsync(OrderDto order, string token)
        {
            try
            {
                // Gắn token để chứng thực người dùng
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var json = JsonConvert.SerializeObject(order);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // Gọi đến Endpoint lưu đơn hàng ở Node.js
                var response = await _httpClient.PostAsync("api/orders/create", content);

                return response.IsSuccessStatusCode;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<HttpResponseMessage> PlaceOrderResponseAsync(OrderDto order, string token)
        {
            // Gán Token vào Header để Node.js xác thực
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            // Gửi POST tới Node.js (hãy kiểm tra kỹ URL này khớp với app.js của Node)
            return await _httpClient.PostAsJsonAsync("api/checkout/create", order);
        }
    }
}

