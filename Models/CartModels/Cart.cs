using System.Text.Json.Serialization;
using _3.QKA_DACK.Models.Another;

namespace _3.QKA_DACK.Models.CartModels
{
    public class Cart
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        [JsonIgnore]
        public ApplicationUser User { get; set; }
        public ICollection<CartItem> CartItems { get; set; }
    }
}
