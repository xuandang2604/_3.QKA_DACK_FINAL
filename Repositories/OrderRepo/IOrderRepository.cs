using _3.QKA_DACK.Models.OrderModels;

namespace _3.QKA_DACK.Repositories.OrderRepo
{
    public interface IOrderRepository
    {
        Task<Order> GetOrderByIdAsync(int orderId);
        Task AddOrderAsync(Order order);
        Task SaveChangesAsync();
        void RemoveOrder(Order order);
        Task<IEnumerable<Order>> GetAllOrdersAsync();
        Task<IEnumerable<Order>> GetOrdersByUserIdAsync(string userId);
        Task<List<MonthlyRevenueDto>> GetMonthlyRevenueAsync();
    }
}
