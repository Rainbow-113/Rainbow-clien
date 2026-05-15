using Microsoft.AspNetCore.Mvc;
using Rainbow.Models;
using Rainbow.Services;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace Rainbow.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserService _userService;

        public AccountController(IUserService userService)
        {
            _userService = userService;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Register() {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            var registerDto = new RegisterDto
            {
                Username = model.Username,
                Email = model.Email,
                Password = model.Password,
                FullName = model.FullName,
                Number = model.Number,
                Address = model.Address

            };
            var isSuccess = await _userService.RegisterAsync(registerDto);

            if (isSuccess)
            {
                TempData["Success"] = "Đăng ký thành công! Hãy đăng nhập.";
                return RedirectToAction("Login");
            }

            ModelState.AddModelError("", "Đăng ký thất bại. Email hoặc tên tài khoản có thể đã tồn tại.");
            return View(model);

        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            var loginDto = new LoginDto { Email = model.Email, Password = model.Password };

            var response = await _userService.LoginAsync(loginDto);
            if (response != null && response.User != null)
            { 
                string debugRole = response.User.Role ?? "NULL (Không có role)";
                ModelState.AddModelError("", "Hệ thống nhận được Role là: " + debugRole);
            }
            if (response != null && !string.IsNullOrEmpty(response.Token))
            {
                // Lưu Token 
                HttpContext.Session.SetString("JWToken", response.Token);
                // LƯU THÊM USERNAME (Lấy từ đối tượng User trong AuthResponse)
                HttpContext.Session.SetString("UserName", response.User.Username);
                HttpContext.Session.SetString("UserRole", response.User.Role);
                if (response.User.Role == "admin")
                {
                    return RedirectToAction("Home", "Admin");
                }

                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", "Email hoặc mật khẩu không chính xác.");
            return View(model);
        }
        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
        [HttpGet]
        public async Task<IActionResult> DetailAccount(string id)
        {
            var token = HttpContext.Session.GetString("JWToken");
            if (string.IsNullOrEmpty(token)) {
                return RedirectToAction("Login");
            }

            var user = await _userService.GetProfileAsync(token);

            if (user == null)
            {
                return Content("Không thể lấy thông tin người dùng. Hãy kiểm tra API Node.js!");
            }
            return View(user);
        }


        [HttpGet]
       
        public async Task<IActionResult> EditAccount()
        {
            var token = HttpContext.Session.GetString("JWToken");
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login");

            var userProfile = await _userService.GetProfileAsync(token);
            return View(userProfile);
        }

        [HttpPost]
        public async Task<IActionResult> EditAccount(UserProfileDto model)
        {
            var token = HttpContext.Session.GetString("JWToken");

            var isUpdated = await _userService.UpdateProfileAsync(model, token);

            if (isUpdated)
            {
                return RedirectToAction("DetailAccount"); 
            }

            ModelState.AddModelError("", "Cập nhật không thành công!");
            return View(model);
        }

        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordDto model)
        {

            if (!ModelState.IsValid) return View(model);

            var token = HttpContext.Session.GetString("JWToken");

            
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login");
            }
            var result = await _userService.ChangePasswordAsync(model, token);
            if (result)
            {
                TempData["Success"] = "Đổi mật khẩu thành công!";
                return RedirectToAction("DetailAccount");
            }

            ModelState.AddModelError("", "Mật khẩu cũ không đúng hãy kiểm tra lại  .");
            return View(model);
        }
    }
}
