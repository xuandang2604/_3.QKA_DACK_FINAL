using _3.QKA_DACK.Models.CategoryModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using _3.QKA_DACK.Models;
namespace _3.QKA_DACK.Areas.ViewComponents
{
    public class CategoryMenuViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;

        public CategoryMenuViewComponent(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            // Lấy tất cả danh mục từ cơ sở dữ liệu
            var categories = await _context.Categories.ToListAsync();

            // Lọc danh mục cha và gắn danh mục con
            var parentCategories = categories
                .Where(c => c.ParentCategory == null)
                .Select(parent => new Category
                {
                    Id = parent.Id,
                    Name = parent.Name,
                    SubCategories = categories.Where(child => child.ParentCategoryId == parent.Id).ToList() // Đảm bảo SubCategories là List<Category>
                }).ToList();

            return View(parentCategories);
        }
    }
}
