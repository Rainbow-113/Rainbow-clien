using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
namespace Rainbow.Models
{
    public class UserProfileDto
    {
        [JsonProperty("_id")]
        [System.Text.Json.Serialization.JsonPropertyName("_id")]
        public string Id { get; set; }
        [JsonProperty("username")] 
        public string UserName { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("fullName")]
        public string FullName { get; set; }

        [JsonProperty("number")]
        public string Number { get; set; }
        [JsonProperty("role")]
        public string Role { get; set; }

        [JsonProperty("address")]
        public string Address { get; set; }
    }

    public class ChangePasswordDto
    {
        [Required(ErrorMessage = "Vui lòng nhập mật khẩu cũ")]
        [JsonProperty("oldPassword")]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu mới")]
        [MinLength(6, ErrorMessage = "Mật khẩu mới phải có ít nhất 6 ký tự")]
        [JsonProperty("newPassword")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Vui lòng xác nhận mật khẩu mới")]
        [Compare("NewPassword", ErrorMessage = "Mật khẩu xác nhận không khớp")]
        // Trường này thường không gửi sang Node.js, chỉ dùng để kiểm tra ở giao diện C#
        public string ConfirmPassword { get; set; }
    }

}
