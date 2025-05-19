namespace E_Sport.Models
{
    public class CartItem //Nút thêm vào giỏ hàng
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string Size { get; set; }  // thêm size giày
    }
}
