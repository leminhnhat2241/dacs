namespace E_Sport.Models
{
    public class Wishlist
    {
        public int Id { get; set; }

        public string UserId { get; set; }   // hoặc int nếu UserId là int
        public int ProductId { get; set; }

        public DateTime AddedDate { get; set; } = DateTime.Now;

        // Navigation properties
        public virtual ApplicationUser User { get; set; }
        public virtual Product Product { get; set; }
    }
}
