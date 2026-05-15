using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Rainbow.Models;
using Rainbow.Services;
namespace Rainbow.Controllers
{
    public class ShopController : Controller
    {
        private readonly ILogger<ShopController> _logger;
        private readonly CategoryService _categoryService;
        private readonly ProductService _productService;
        private readonly ICartService _cartService;
        public ShopController(ILogger<ShopController> logger, CategoryService categoryService, ProductService productService,ICartService cartService ) 
        {
            _logger = logger;
            _categoryService = categoryService;         
            _productService = productService;
            _cartService = cartService;

        }
        public async Task<IActionResult> Index()
        {
            _logger.LogInformation("Đang truy cập trang chủ và lấy danh mục...");
            var category= await _categoryService.GetActiveCategories();
            var product= await _productService.GetActiveProducts();
            var viewModel = new ShopViewModel
            {
                CategoryDto = category,
                ProductsDto = product
            };
            return View(viewModel);
        }
        public async Task<IActionResult> Detail(string id)
        {
            var product = await _productService.GetProductById(id);
            if (product == null)
            {
                // Thay vì Redirect, hãy trả về trang lỗi để biết tại sao null
                return Content("Không tìm thấy sản phẩm với ID: " + id);
            }
            return View(product);
        }


        public async Task<IActionResult> SearchProductsPartial(string searchTerm)
        {
            IEnumerable<ProductDto> products;

            if (string.IsNullOrEmpty(searchTerm))
            {
                products = await _productService.GetActiveProducts();
            }
            else
            {
                products = await _productService.SearchProducts(searchTerm);
            }

            // Trả về file chung trong thư mục Shared
            return PartialView("_ProductList", products);
        }
        public async Task<IActionResult> Cart()
        {
            var token = HttpContext.Session.GetString("JWToken");
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Account");

            var cart = await _cartService.GetCartAsync(token);

            if (cart != null && cart.Items != null)
            {
                // Thử in ra Debug để xem có dữ liệu không
                foreach (var item in cart.Items)
                {
                    System.Diagnostics.Debug.WriteLine($"SP: {item.ProductName}, Giá: {item.Price}");
                }

                var cartJson = JsonConvert.SerializeObject(cart.Items);
                HttpContext.Session.SetString("Cart", cartJson);
            }

            return View(cart);
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart([FromBody] CartItemDto model)
        {

            var token = HttpContext.Session.GetString("JWToken");

            if (string.IsNullOrEmpty(token))
            {
                return Json(new { success = false, message = "Vui lòng đăng nhập để mua hàng!" });
            }
            var result = await _cartService.AddToCartAsync(model, token);

            if (result)
            {
                return Json(new { success = true, redirectUrl = Url.Action("Cart", "Shop"), message = "Đã thêm vào giỏ hàng thành công!" });
            }
            return Json(new { success = false, message = "Không thể thêm vào giỏ hàng. Vui lòng thử lại!" });
        }
        [HttpPost]
        public async Task<IActionResult> RemoveFromCart(string productId, string size, string color)
        {
            var token = HttpContext.Session.GetString("JWToken");
            var result = await _cartService.RemoveFromCartAsync(productId, size, color,token);
            return Json(new { success = result });
        }

    }
}
