using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace Rainbow.Models
{
    public class CartItemDto
    {
        [JsonProperty("productId")]
        public string ProductId { get; set; }
        [JsonProperty("quantity")]
        public int Quantity { get; set; }
        [JsonProperty("size")]
        public string Size { get; set; }
        [JsonProperty("color")]
        public string Color { get; set; }
        //thêm vào 
        [JsonPropertyName("productName")]
        [JsonProperty("productName")] 
        public string ProductName { get; set; }

        [JsonProperty("image")] 
        public string Image { get; set; }
        [JsonPropertyName("price")]
        [JsonProperty("price")] 
        public decimal Price { get; set; }
    }
}
