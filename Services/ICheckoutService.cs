using Rainbow.Models;

namespace Rainbow.Services
{
    public interface ICheckoutService
    {
        // Gửi toàn bộ thông tin OrderDto sang API Node.js
        Task<bool> PlaceOrderAsync(OrderDto order, string token);
        Task<HttpResponseMessage> PlaceOrderResponseAsync(OrderDto order, string token);
    }
}
