using Microsoft.EntityFrameworkCore;
using _3.QKA_DACK.Models;
using _3.QKA_DACK.Models.CategoryModels;
using Microsoft.AspNetCore.Mvc;

namespace _3.QKA_DACK.Repositories.CategoryRepo
{
    public class EFCategoryRepository : ICategoryRepository
    {
        private readonly ApplicationDbContext _context;
        public EFCategoryRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Category>> GetAllAsync()
        {
            // return await _context.Products.ToListAsync();
            return await _context.Categories// Include thông tin về category
            .ToListAsync();
        }
        public async Task<Category> GetByIdAsync(int id)
        {
            // return await _context.Products.FindAsync(id);
            // lấy thông tin kèm theo category
            return await _context.Categories.FirstOrDefaultAsync(p => p.Id == id);
        }
        public async Task AddAsync(Category category)
        {
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateAsync(Category category)
        {
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Category>> GetParentCategoriesAsync()
        {
            return await _context.Categories
                .Where(c => c.ParentCategoryId == null)
                .ToListAsync();
        }


        public async Task<IEnumerable<Category>> GetParentCategoriesAsync(int currentCategoryId)
        {
            return await _context.Categories
                .Where(c => c.ParentCategoryId == null && c.Id != currentCategoryId)
                .ToListAsync();
        }


        public IEnumerable<Category> GetChildCategories(int parentId)
        {
            // Truy vấn để lấy các category con theo parentId
            return _context.Categories
                           .Where(c => c.ParentCategoryId == parentId)
                           .ToList();
        }

        //public JsonResult GetChildCategories(int parentId)
        //{
        //    var childCategories = _context.Categories
        //        .Where(c => c.ParentCategoryId == parentId)
        //        .Select(c => new { c.Id, c.Name })
        //        .ToList();

        //    return Json(childCategories);
        //}

    }
}
