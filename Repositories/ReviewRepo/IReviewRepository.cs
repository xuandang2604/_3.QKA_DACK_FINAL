using _3.QKA_DACK.Models.ProductModels;

namespace _3.QKA_DACK.Repositories.ReviewRepo
{
    public interface IReviewRepository
    {
        Task<IEnumerable<Review>> GetReviewsByProductIdAsync(int productId);
        Task AddAsync(Review review);
    }
}
