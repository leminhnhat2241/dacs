using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace E_Sport.Models
{
    public class FieldType
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên loại sân")]
        public string Name { get; set; }

        public ICollection<Product>? Products { get; set; } // liên kết ngược nếu dùng
    }
}
