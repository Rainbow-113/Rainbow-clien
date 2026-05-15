using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Rainbow.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập tên tài khoản")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập Email")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập họ và tên")]
        [Display(Name = "Họ và tên")]
        [JsonProperty("fullname")]
        public string FullName { get; set; } // Khớp với fullname trong MongoDB

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        [RegularExpression(@"^\d{10,11}$", ErrorMessage = "Số điện thoại phải từ 10-11 chữ số")]
        public string Number { get; set; } // Khớp với number trong MongoDB

        [Required(ErrorMessage = "Vui lòng nhập địa chỉ")]
        public string Address { get; set; } // Khớp với address trong MongoDB


        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        [MinLength(6, ErrorMessage = "Mật khẩu phải từ 6 ký tự trở lên")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Mật khẩu xác nhận không khớp")]
        public string ConfirmPassword { get; set; }
    }
}
