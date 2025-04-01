using _3.QKA_DACK.Models.ProductModels;

namespace _3.QKA_DACK.Models.BrandModels
{
    public class Brand
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public List<Product> Products { get; set; }
    }

}
