using _3.QKA_DACK.Models.Another;

namespace _3.QKA_DACK.Models.OrderModels
{
    public class Order
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public DateTime InvoiceDate { get; set; }
        public string? Note { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; }
        public decimal TotalAmount { get; set; }
        public string? Status { get; set; }
    }
}
