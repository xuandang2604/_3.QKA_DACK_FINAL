using Microsoft.EntityFrameworkCore;
using _3.QKA_DACK.Models;
using _3.QKA_DACK.Models.CartModels;

namespace _3.QKA_DACK.Repositories.CartRepo
{
    public class EFCartRepository : ICartRepository
    {
        private readonly ApplicationDbContext _context;

        public EFCartRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Cart> GetCartByUserIdAsync(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                // Nếu không có userId, trả về null để biết rằng chưa có giỏ hàng lưu trong CSDL
                return null;
            }

            return await _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);
        }


        public async Task AddCartAsync(Cart cart)
        {
            await _context.Carts.AddAsync(cart);
        }

        public async Task UpdateCartAsync(Cart cart)
        {
            _context.Carts.Update(cart);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
