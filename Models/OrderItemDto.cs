using Newtonsoft.Json;

namespace Rainbow.Models
{
    public class OrderItemDto
    {
        [JsonProperty("_id")]
        [System.Text.Json.Serialization.JsonPropertyName("_id")]
        public string ProductId { get; set; }

        [JsonProperty("productName")]
        public string ProductName { get; set; }

        [JsonProperty("image")]
        public string Image { get; set; } 

        [JsonProperty("size")]
        public string Size { get; set; } 

        [JsonProperty("color")]
        public string Color { get; set; } // Màu sắc đã chọn

        [JsonProperty("quantity")]
        public int Quantity { get; set; }

        [JsonProperty("price")]
        public decimal Price { get; set; } // Giá tại thời điểm mua

        [JsonProperty("totalPrice")]
        public decimal TotalPrice => Quantity * Price; // Tổng tiền của món hàng này
    }
}
