using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace E_Sport.Models.ViewModel
{
    public class ProductWithSizesViewModel
    {
        public Product Product { get; set; }

        //[Required]
        //public string? Sizes { get; set; } // từ List<string> → string
    }
}
