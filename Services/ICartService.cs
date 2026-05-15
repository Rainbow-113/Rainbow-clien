using Rainbow.Models;

namespace Rainbow.Services
{
    public interface ICartService
    {
        Task<bool> AddToCartAsync(CartItemDto item, string token);
        Task<CartDto> GetCartAsync(string token);
        Task<bool> RemoveFromCartAsync(string productId, string size, string color, string token);
    }
}
