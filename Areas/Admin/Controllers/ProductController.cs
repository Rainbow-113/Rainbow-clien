using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Rainbow.Models;
using Rainbow.Services;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
namespace Rainbow.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly ProductService _productService;
        private readonly CategoryService _categoryService;
        private readonly dynamic categoryId;
        private dynamic categories;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductController(ProductService productService, CategoryService categoryService,IWebHostEnvironment webHostEnvironment)
        {
            _productService = productService;
            _categoryService = categoryService;
            _webHostEnvironment = webHostEnvironment;

        }
        // GET: ProductController
        public async Task<ActionResult> Index(string categoryId , string searchTerm)
        {

            var  products = await _productService.GetAllProducts(categoryId, searchTerm);
            var category = await _categoryService.GetAllCategories();
            var viewBag = new ShopViewModel
            {
                ProductsDto = products,
                CategoryDto = category,
            };
            ViewBag.SelectedCategory = categoryId;
            ViewBag.CurrentSearch = searchTerm;
            return View(viewBag);
        }
        public ActionResult Details(int id)
        {
            return View();
        }
        [HttpGet]
        public async Task<ActionResult> Create()
        {
            var categories = await _categoryService.GetAllCategories();
            ViewBag.Categories = categories;
            return View(new ProductDto());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(ProductDto productDto)
        {
            try
            {  
                ModelState.Remove("Id");
                ModelState.Remove("ImagePath");
                bool isNameUsed = await _productService.IsNameExistsAsync(productDto.Name);
                if (isNameUsed)
                {
                    ModelState.AddModelError("Name", "Tên sản phẩm này đã tồn tại. Vui lòng chọn tên khác!");
                }
                if (ModelState.IsValid)
                {
                        // 1.ảnh chính
                        if (productDto.MainImageFile != null)
                        {
                            productDto.ImagePath = await SaveFile(productDto.MainImageFile);
                        }
                        // 2. ảnh phụ
                        var subFiles = new[] { productDto.SubImageFile1, productDto.SubImageFile2, productDto.SubImageFile3 };
                        foreach (var file in subFiles)
                        {
                            if (file != null)
                            {
                                string fileName = await SaveFile(file);
                                productDto.Images.Add($"/images/" + fileName);
                            }
                        }
                    bool isSaved = await _productService.CreateProductAsync(productDto);
                    Console.WriteLine("4. KẾT QUẢ API: " + isSaved);
                    if (isSaved)
                    {
                        return RedirectToAction(nameof(Index));
                    }
                    ModelState.AddModelError("", "Không thể lưu sản phẩm sang API.");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Có lỗi xảy ra: " + ex.Message);
            }
            ViewBag.Categories = await _categoryService.GetAllCategories();
            return View(productDto);
        }
       
        public async Task<ActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();

            var product = await _productService.GetProductById(id);
            if (product == null) return NotFound();

            // Lấy danh sách Category để đổ vào DropdownList (nếu bạn có CategoryService)
            ViewBag.CategoryList = await _categoryService.GetAllCategories();

            return View(product);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, ProductDto productDto)
        {
            try
            {
                if (productDto.MainImageFile != null)
                {     
                    productDto.ImagePath = await SaveFile(productDto.MainImageFile);
                }
                // 2. Xử lý ảnh phụ
                var subFiles = new[] { productDto.SubImageFile1, productDto.SubImageFile2, productDto.SubImageFile3 };
                productDto.Images = new List<string>();
                foreach (var file in subFiles)
                {
                    if (file != null)
                    {
                        // Lưu file vào thư mục vật lý 'Browser/images' thông qua hàm SaveFile
                        string filePath = await SaveFile(file);
                        productDto.Images.Add("/images/" + filePath);
                    }
                }

                // Gọi Service gửi sang Node.js
                bool isUpdated = await _productService.UpdateProductAsync(id, productDto);
                if (isUpdated) return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                // Ghi log lỗi ra màn hình Console để bạn đọc được
                Console.WriteLine($"LỖI CỤ THỂ: {ex.Message}");
                ModelState.AddModelError("", "Lỗi lưu ảnh: " + ex.Message);
            }

            return View(productDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(string id, IFormCollection collection)
        {
            // Kiểm tra ID có trống không
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest();
            }

            // Gọi Service để xóa (Đảm bảo Service đã sửa lỗi chính tả 'prodcuts' -> 'products')
            bool isDeleted = await _productService.DeleteProductAsync(id);

            if (isDeleted)
            {
                TempData["Success"] = "Xóa sản phẩm thành công!";
            }
            else
            {
                // Nếu vào đây, hãy kiểm tra Log của Node.js xem có nhận được request không
                TempData["Error"] = "Lỗi! Không thể xóa sản phẩm.";
            }

            return RedirectToAction(nameof(Index));
        }
        private async Task<string> SaveFile(IFormFile file)
        {
            if (file == null) return null;

            // 1. Đường dẫn tuyệt đối để GHI file (Dùng cho ổ cứng)
            string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Browser", "images");

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            string fullPath = Path.Combine(folderPath, fileName);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            return $"{fileName}";
        }
    }
}
