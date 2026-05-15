using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using Microsoft.EntityFrameworkCore;
using Rainbow.Models;
using Rainbow.Services;
using System.Threading.Tasks;
namespace Rainbow.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly CategoryService _categoryService;
        public CategoryController(CategoryService categoryService)
        {
            _categoryService = categoryService;
        }
         // GET: CategoryController
        public async Task<ActionResult> Index()
        {
            var categories = await _categoryService.GetAllCategories();
            return View(categories);
        }

        // GET: CategoryController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: CategoryController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CategoryController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryDto categoryDto)
        {
            if (ModelState.IsValid)
            {
                bool isCreated = await _categoryService.CreateCategory(categoryDto);
                if (isCreated)
                {
                    return RedirectToAction(nameof(Index));
                }
                ModelState.AddModelError("", "Không thể tạo danh mục. Vui lòng kiểm tra lại API.");
            }
            return View(categoryDto);
        }

        // GET: CategoryController/Edit/5
        public async Task<ActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id)) return BadRequest();

            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category == null) return NotFound();

            return View(category);
        }

        // POST: CategoryController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(string id, CategoryDto categoryDto)
        {
            if (id != categoryDto.Id) return BadRequest();

            if (ModelState.IsValid)
            {
                bool isUpdated = await _categoryService.UpdateCategoryAsync(id, categoryDto);
                if (isUpdated)
                {
                    TempData["Success"] = "Cập nhật danh mục thành công!";
                    return RedirectToAction(nameof(Index));
                }
                ModelState.AddModelError("", "Lỗi khi cập nhật danh mục.");
            }
            return View(categoryDto);
        }

        // GET: CategoryController/Delete/5
        public ActionResult Delete(string id)
        {
            return View();
        }
        // POST: CategoryController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(string id, IFormCollection collection)
        {
            try
            {
                if (string.IsNullOrEmpty(id)) return BadRequest();
                bool isDeleted = await _categoryService.DeleteCategoryAsync(id);
                if (isDeleted)
                {
                    TempData["Success"] = "Xóa danh mục thành công!";
                }
                else
                {
                    TempData["Error"] = "Không thể xóa. Có thể danh mục này đang chứa sản phẩm!";
                }
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                TempData["Error"] = "Đã xảy ra lỗi hệ thống khi xóa.";
                return RedirectToAction(nameof(Index));
            }
        }




    }
}
