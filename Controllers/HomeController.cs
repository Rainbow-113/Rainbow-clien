using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Rainbow.Models;
using Rainbow.Services;

namespace Rainbow.Controllers
{

    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly CategoryService _categoryService;
        private readonly ProductService _productService;

        public HomeController(ILogger<HomeController> logger, CategoryService categoryService, ProductService productService)
        {
            _logger = logger;
            _categoryService = categoryService;
            _productService = productService;
        }

        public async Task<IActionResult> Index()
        {
            _logger.LogInformation("Đang truy cập trang chủ và lấy danh mục...");

            var categories = await _categoryService.GetActiveCategories();
            var products= await _productService.GetActiveProducts();

            var viewModel = new ShopViewModel
            {
                CategoryDto = categories,
                ProductsDto = products
            };
            return View(viewModel);
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
