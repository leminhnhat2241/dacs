namespace E_Sport.Models
{
    public class OrderDetail //Chi Tiết Đơn Hàng: Lưu thông tin chi tiết cho mỗi mặt hàng trong đơn.
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public Order Order { get; set; }
        public Product Product { get; set; }
    }
}
