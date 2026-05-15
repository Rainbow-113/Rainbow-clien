using Newtonsoft.Json;

namespace Rainbow.Models
{

    public class CartDto
    {
        public string UserId { get; set; }
        public List<CartItemDetailDto> Items { get; set; } = new List<CartItemDetailDto>();
        // Thuộc tính tự tính tổng tiền
        public decimal TotalCartPrice => Items.Sum(x => x.Price * x.Quantity);
    }
    public class CartItemDetailDto
    {
        [JsonProperty("productId")]
        public string ProductId { get; set; }
        [JsonProperty("productName")] 
        public string ProductName { get; set; }

        [JsonProperty("productImage")]
        public string ProductImage { get; set; }
        [JsonProperty("price")]
        public decimal Price { get; set; }
        [JsonProperty("quantity")]
        public int Quantity { get; set; }
        [JsonProperty("size")]
        public string Size { get; set; }
        [JsonProperty("color")]
        public string Color { get; set; }
    }
}
