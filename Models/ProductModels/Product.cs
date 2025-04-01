using System.ComponentModel.DataAnnotations;
using _3.QKA_DACK.Models.BrandModels;
using _3.QKA_DACK.Models.CategoryModels;

namespace _3.QKA_DACK.Models.ProductModels
{
    public class Product
    {
        public int Id { get; set; }
        [Required, StringLength(100)]
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }

        public int Stock { get; set; }
        public string Size { get; set; }
        public string Color { get; set; }
        public string? ImageUrl { get; set; }
        public int? CategoryId { get; set; }
        public Category? Category { get; set; }
        public int? BrandId { get; set; }
        public Brand? Brand { get; set; }

        public List<Review> Reviews { get; set; } = new List<Review>();

    }
}

