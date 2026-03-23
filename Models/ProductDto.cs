using System.Text.Json;
using System.Text.Json.Serialization;

namespace Rainbow.Models
{
    public class ProductDto
    {
        [JsonPropertyName("_id")]
        public string Id { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("price")]
        public decimal Price { get; set; }
        [JsonPropertyName("description")]
        public string Description { get; set; }
        [JsonPropertyName("imagePath")]
        public string ImagePath { get; set; }
        [JsonPropertyName("images")]
        public List<string> Images { get; set; } = new List<string>();
        public bool IsActive { get; set; }
        [JsonPropertyName("categoryId")]
        // 1. Thuộc tính này để C# làm việc với View
        [JsonIgnore] // Bỏ qua khi giải mã JSON tự động
        public CategoryDto Category { get; set; }
        // 2. Thuộc tính này để nhận dữ liệu "thô" từ Node.js
        [JsonPropertyName("categoryId")]
        public object RawCategoryId
        {
            set
            {
                // Nếu Node.js trả về Object { "_id": "...", "name": "..." }
                if (value is JsonElement element && element.ValueKind == JsonValueKind.Object)
                {
                    Category = element.Deserialize<CategoryDto>(new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                }
                // Nếu Node.js chỉ trả về chuỗi "65f..."
                else if (value != null)
                {
                    Category = new CategoryDto { Id = value.ToString() };
                }
            }
        }
        // --- CÁC THUỘC TÍNH HỨNG FILE (Chỉ dùng ở C#, không gửi sang API) ---
        [JsonIgnore]
        public IFormFile? MainImageFile { get; set; }

        [JsonIgnore]
        public IFormFile? SubImageFile1 { get; set; }

        [JsonIgnore]
        public IFormFile? SubImageFile2 { get; set; }

        [JsonIgnore]
        public IFormFile? SubImageFile3 { get; set; }
    }
}
