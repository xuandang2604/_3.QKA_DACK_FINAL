using _3.QKA_DACK.Models.CategoryModels;

namespace _3.QKA_DACK.Repositories.CategoryRepo
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<Category>> GetAllAsync();
        Task<Category> GetByIdAsync(int id);
        Task AddAsync(Category category);
        Task UpdateAsync(Category category);
        Task DeleteAsync(int id);
        Task<IEnumerable<Category>> GetParentCategoriesAsync();
        Task<IEnumerable<Category>> GetParentCategoriesAsync(int currentCategoryId);


        IEnumerable<Category> GetChildCategories(int parentId);
       // Task<IEnumerable<Category>> GetChildCategoriesAsync(int parentId);
    }
}
