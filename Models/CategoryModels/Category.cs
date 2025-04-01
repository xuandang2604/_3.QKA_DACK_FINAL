using System.ComponentModel.DataAnnotations;
using _3.QKA_DACK.Models.ProductModels;

namespace _3.QKA_DACK.Models.CategoryModels
{
    public class Category
    {
        public int Id { get; set; }

        public string Name { get; set; }
        public int? ParentCategoryId { get; set; }
        public Category? ParentCategory { get; set; }
        public List<Category> SubCategories { get; set; } = new List<Category>(); // Thêm thuộc tính này
        public List<Product>? Products { get; set; }



    }
}
