using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Rainbow.Models;
using Rainbow.Services;
using System.Threading.Tasks;

namespace Rainbow.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly ProductService _productService;
        private readonly CategoryService _categoryService;
        private readonly dynamic categoryId;
        private dynamic categories;

        public ProductController(ProductService productService, CategoryService categoryService)
        {
            _productService = productService;
            _categoryService = categoryService;
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

        // GET: ProductController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: ProductController/Create
        [HttpGet]
        public async Task<ActionResult> Create()
        {
            var categories = await _categoryService.GetAllCategories();
            ViewBag.Categories = categories;
            return View(new ProductDto());
        }

        // POST: ProductController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(ProductDto productDto)
        {
            // Kiểm tra xem dữ liệu nhập vào có hợp lệ không (ví dụ: Price > 0)
            if (!ModelState.IsValid)
            {
                // Nếu lỗi, phải nạp lại Category cho Dropdown trước khi trả về View
                ViewBag.Categories = await _categoryService.GetAllCategories();
                return View(productDto);
            }
            try
            {
                // 1. Xử lý lưu ảnh chính
                if (productDto.MainImageFile != null)
                {
                    productDto.ImagePath = await SaveFile(productDto.MainImageFile);
                }

                // 2. Xử lý lưu ảnh phụ
                var subFiles = new[] { productDto.SubImageFile1, productDto.SubImageFile2, productDto.SubImageFile3 };
                foreach (var file in subFiles)
                {
                    if (file != null)
                    {
                        string fileName = await SaveFile(file);
                        productDto.Images.Add("/Browser/images/" + fileName);
                    }
                }

                // 3. Gọi Service lưu vào Node.js API
                bool isSaved = await _productService.CreateProductAsync(productDto);

                if (isSaved)
                {
                    return RedirectToAction(nameof(Index));
                }

                // Nếu API trả về false, thêm thông báo lỗi
                ModelState.AddModelError("", "Không thể lưu sản phẩm sang API.");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Có lỗi xảy ra: " + ex.Message);
            }

            // ĐÂY LÀ DÒNG QUAN TRỌNG NHẤT: Trả về View nếu 'isSaved' false HOẶC có Exception
            ViewBag.Categories = await _categoryService.GetAllCategories();
            return View(productDto);
        }
        
        private async Task<string> SaveFile(IFormFile file)
        {
            // Tạo tên file duy nhất để không bị trùng
            string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(file.FileName);

            // Đường dẫn đến thư mục wwwroot/images
            string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Browser","images");

            // Tạo thư mục nếu chưa có
            if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            return uniqueFileName; // Trả về tên file để lưu vào database
        }
        // GET: ProductController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: ProductController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ProductDto productDto)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ProductController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ProductController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
