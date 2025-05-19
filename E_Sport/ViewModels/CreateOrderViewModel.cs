using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace E_Sport.ViewModels
{
    public class CreateOrderViewModel
    {
        [Required(ErrorMessage = "Chọn khách hàng")]
        public string UserId { get; set; }

        [Required(ErrorMessage = "Địa chỉ giao hàng không được bỏ trống")]
        public string ShippingAddress { get; set; }

        public string Notes { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn trạng thái")]
        public string Status { get; set; } = "Pending"; // mặc định là "Pending"

        public List<OrderDetailViewModel> OrderDetails { get; set; } = new List<OrderDetailViewModel>();
    }

    public class OrderDetailViewModel
    {
        [Required(ErrorMessage = "Chọn sản phẩm")]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Nhập số lượng")]
        [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn 0")]
        public int Quantity { get; set; }
    }
}
