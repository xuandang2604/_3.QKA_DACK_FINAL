using _3.QKA_DACK.Models.ProductModels;

namespace _3.QKA_DACK.Repositories.ProductRepo
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetAllAsync();
        Task<Product> GetByIdAsync(int id);
        Task AddAsync(Product product);
        Task UpdateAsync(Product product);
        Task DeleteAsync(int id);
    }
}
