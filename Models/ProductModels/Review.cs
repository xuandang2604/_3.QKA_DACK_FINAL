using _3.QKA_DACK.Models.Another;

namespace _3.QKA_DACK.Models.ProductModels
{
    public class Review
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string UserId { get; set; }
        public int Rating { get; set; } // Rating từ 1 đến 5
        public string Comment { get; set; }
        public DateTime CreatedAt { get; set; }

        public Product Product { get; set; }
        public ApplicationUser User { get; set; }
    }
}
