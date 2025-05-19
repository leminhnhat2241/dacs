using System.ComponentModel.DataAnnotations;

namespace E_Sport.Models
{
    public class ProductSize
    {
        public int Id { get; set; }

        [Required]
        public string Size { get; set; }

        public bool IsAvailable { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }
    }


}
