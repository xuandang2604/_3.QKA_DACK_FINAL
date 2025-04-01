using _3.QKA_DACK.Models;
using _3.QKA_DACK.Models.BrandModels;
using Microsoft.EntityFrameworkCore;

namespace _3.QKA_DACK.Repositories.BrandRepo
{
    public class EFBrandRepository : IBrandRepository
    {
        private readonly ApplicationDbContext _context;

        public EFBrandRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Brand>> GetAll()
        {
            var brands = await _context.Brands.ToListAsync();
            Console.WriteLine($"Fetched brands: {brands.Count()}");
            return brands;
        }


        public async Task<Brand> GetById(int id)
        {
            return await _context.Brands.FindAsync(id);
        }
    }
}
