using Microsoft.AspNetCore.Mvc;
using NuGet.Common;
using Rainbow.Services;
using System.Threading.Tasks;

namespace Rainbow.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }
        public async Task<IActionResult> Index(string fullName)
        {
            var token = HttpContext.Session.GetString("JWToken");

           
            var allUsers = await _userService.GetAllUsersAsync(token);

          
            if (!string.IsNullOrEmpty(fullName))
            {
                var searchKey = RemoveDiacritics(fullName);
                allUsers = allUsers.Where(u =>
            // Bỏ dấu Họ tên để so sánh
            (u.FullName != null && RemoveDiacritics(u.FullName).Contains(searchKey)) ||
            // So sánh với Tài khoản (thường là không dấu sẵn)
            (u.UserName != null && u.UserName.ToLower().Contains(searchKey))
        ).ToList();
            }

            ViewBag.SearchTerm = fullName;
            return View(allUsers);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();
            var token = HttpContext.Session.GetString("JWToken");

            var result = await _userService.DeleteUserAsync(id, token);
            if (result)
            {
                TempData["Success"] = "Xóa người dùng thành công!";
            }
            else
            {
                TempData["Error"] = "Không thể xóa người dùng này.";
            }

            return RedirectToAction(nameof(Index));
        }
        public static string RemoveDiacritics(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return text;

            // Chuẩn hóa chuỗi về dạng FormD (tách các dấu ra khỏi chữ cái gốc)
            var normalizedString = text.Normalize(System.Text.NormalizationForm.FormD);
            var stringBuilder = new System.Text.StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c);
                // Nếu không phải là dấu (NonSpacingMark) thì giữ lại chữ cái gốc
                if (unicodeCategory != System.Globalization.UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(System.Text.NormalizationForm.FormC).ToLower();
        }
    }
}
