using _3.QKA_DACK.Models.BrandModels;
namespace _3.QKA_DACK.Repositories.BrandRepo
{
    public interface IBrandRepository
    {
        Task<IEnumerable<Brand>> GetAll();
        Task<Brand> GetById(int id);
    }
}
