using Microsoft.AspNetCore.Mvc;
using Rainbow.Services;
using Rainbow.Models;
namespace Rainbow.Controllers
{
    public class ShopController : Controller
    {
        private readonly ILogger<ShopController> _logger;
        private readonly CategoryService _categoryService;
        private readonly ProductService _productService;
        public ShopController(ILogger<ShopController> logger, CategoryService categoryService, ProductService productService ) 
        {
            _logger = logger;
            _categoryService = categoryService;         
            _productService = productService;
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
    }
}
