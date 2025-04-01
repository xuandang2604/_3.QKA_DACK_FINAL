using Microsoft.EntityFrameworkCore;
using _3.QKA_DACK.Models;
using _3.QKA_DACK.Models.OrderModels;

namespace _3.QKA_DACK.Repositories.OrderRepo
{
    public class EFOrderRepository : IOrderRepository
    {
        private readonly ApplicationDbContext _context;

        public EFOrderRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Order> GetOrderByIdAsync(int orderId)
        {
            return await _context.Orders
                //.Include(o => o.User) // <-- thêm dòng này
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == orderId);
        }

        public async Task AddOrderAsync(Order order)
        {
            await _context.Orders.AddAsync(order);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public void RemoveOrder(Order order)
        {
            _context.Orders.Remove(order);
        }

        public async Task<IEnumerable<Order>> GetAllOrdersAsync()
        {

            return await _context.Orders.ToListAsync();
        }

        public async Task<IEnumerable<Order>> GetOrdersByUserIdAsync(string userId)
        {
            return await _context.Orders
                        .Include(o => o.User) // <-- thêm dòng này
                        .Where(o => o.UserId == userId)
                        .ToListAsync();
        }

        //public async Task<Dictionary<int, decimal>> GetMonthlyRevenueAsync()
        //{
        //    return await _context.Orders
        //        .Where(o => o.Status == "Paid") // Lọc đơn hàng đã hoàn thành
        //        .GroupBy(o => o.InvoiceDate.Month)
        //        .Select(g => new { Month = g.Key, TotalRevenue = g.Sum(o => o.TotalAmount) })
        //        .ToDictionaryAsync(g => g.Month, g => g.TotalRevenue);
        //}

        public async Task<List<MonthlyRevenueDto>> GetMonthlyRevenueAsync()
        {
            return await _context.Orders
                .Where(o => o.Status == "Paid")
                .GroupBy(o => o.InvoiceDate.Month)
                .Select(g => new MonthlyRevenueDto
                {
                    Month = g.Key,
                    TotalRevenue = g.Sum(o => o.TotalAmount),
                    OrderCount = g.Count(),
                    AvgOrderValue = g.Average(o => o.TotalAmount)
                })
                .OrderBy(m => m.Month)
                .ToListAsync();
        }




    }
}
