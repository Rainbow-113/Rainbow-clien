using Newtonsoft.Json;

namespace Rainbow.Models
{
    public class OrderDto
    {
        [JsonProperty("_id")] 
        public string? Id { get; set; }

        // --- Thông tin khách hàng ---
        [JsonProperty("userId")]
        public string? UserId { get; set; }
        [JsonProperty("fullName")]
        public string FullName { get; set; }

        [JsonProperty("address")]
        public string Address { get; set; }

        [JsonProperty("phoneNumber")]
        public string PhoneNumber { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        // --- Thông tin đơn hàng ---
        [JsonProperty("orderDate")]
        public DateTime OrderDate { get; set; } = DateTime.Now;

        [JsonProperty("status")]
        public string Status { get; set; } = "Pending"; // Trạng thái: Chờ xác nhận, Đang giao...

        [JsonProperty("paymentMethod")]
        public string PaymentMethod { get; set; } // Ví dụ: "COD" hoặc "Bank Transfer"

        // --- Danh sách sản phẩm ---
        [JsonProperty("items")]
        public List<OrderItemDto> Items { get; set; } = new List<OrderItemDto>();

        [JsonProperty("totalAmount")]
        public decimal TotalAmount { get; set; }
    }
}
