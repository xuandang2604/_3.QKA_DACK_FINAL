using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using _3.QKA_DACK.Models.CategoryModels;
using _3.QKA_DACK.Repositories.CategoryRepo;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace _3.QKA_DACK.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("admin/categories")]
    public class CategoriesController : Controller
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoriesController(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        // Hiển thị danh sách danh mục
        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var categories = await _categoryRepository.GetAllAsync();
            var parentCategories = await _categoryRepository.GetParentCategoriesAsync();

            return View(categories);
        }
        [HttpGet("/admin/categories/child/{parentId}")]
        public JsonResult GetChildCategories(int parentId)
        {
            var childCategories = _categoryRepository.GetChildCategories(parentId);
            var result = childCategories.Select(c => new { c.Id, c.Name }).ToList();
            return Json(result);
        }

        // Hiển thị chi tiết danh mục

        [HttpGet("display/{id}")]
        public async Task<IActionResult> Display(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        // Hiển thị form thêm danh mục mới
        [Authorize(Roles = "Admin,Employee")]
        [HttpGet("add")]
        public async Task<IActionResult> Add()
        {
            var parentCategories = await _categoryRepository.GetParentCategoriesAsync();
            ViewBag.ParentCategories = new SelectList(parentCategories, "Id", "Name");

            return View();
        }

        // Xử lý thêm danh mục mới
        [HttpPost("add")]
        public async Task<IActionResult> Add(Category category)
        {
            if (ModelState.IsValid)
            {
                await _categoryRepository.AddAsync(category);
                return RedirectToAction(nameof(Index));
            }
            var parentCategories = await _categoryRepository.GetParentCategoriesAsync();
            ViewBag.ParentCategories = new SelectList(parentCategories, "Id", "Name");


            return View(category);
        }

        // Hiển thị form cập nhật danh mục
        [Authorize(Roles = "Admin,Employee")]
        [HttpGet("update/{id}")]
        public async Task<IActionResult> Update(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            var parentCategories = await _categoryRepository.GetParentCategoriesAsync(id);
            ViewBag.ParentCategories = new SelectList(parentCategories, "Id", "Name", category.ParentCategoryId);
            return View(category);
        }


        
        // Xử lý cập nhật danh mục
        [HttpPost("update/{id}")]
        public async Task<IActionResult> Update(int id, Category category)
        {
            if (id != category.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                await _categoryRepository.UpdateAsync(category);
                return RedirectToAction(nameof(Index));
            }

            var parentCategories = await _categoryRepository.GetParentCategoriesAsync(category.Id);
            ViewBag.ParentCategories = new SelectList(parentCategories, "Id", "Name", category.ParentCategoryId);
            return View(category);
        }
        [Authorize(Roles = "Admin")]
        // Hiển thị form xác nhận xóa danh mục
        [HttpGet("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        // Xử lý xóa danh mục
        [HttpPost("delete/{id}")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category != null)
            {
                await _categoryRepository.DeleteAsync(id);
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
