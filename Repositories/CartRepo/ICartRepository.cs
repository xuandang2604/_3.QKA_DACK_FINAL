using _3.QKA_DACK.Models.CartModels;

namespace _3.QKA_DACK.Repositories.CartRepo
{
    public interface ICartRepository
    {
        Task<Cart> GetCartByUserIdAsync(string userId);
        Task AddCartAsync(Cart cart);
        Task UpdateCartAsync(Cart cart);
        Task SaveChangesAsync();
    }
}
