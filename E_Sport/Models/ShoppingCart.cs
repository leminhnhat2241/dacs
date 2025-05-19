using System.Drawing;

namespace E_Sport.Models
{
    public class ShoppingCart //Giỏ hàng là nơi chứa các add cartitem
    {
        public List<CartItem> Items { get; set; } = new List<CartItem>();
        public void AddItem(CartItem item)
        {
            var existingItem = Items.FirstOrDefault(i => i.ProductId ==
            item.ProductId);
            if (existingItem != null)
            {
                existingItem.Quantity += item.Quantity;
            }
            else
            {
                Items.Add(item);
            }
        }
        public void RemoveItem(int productId, string size)   //Xóa hàng
        {
            var item = Items.FirstOrDefault(i => i.ProductId == productId && i.Size == size);
            if (item != null)
            {
                Items.Remove(item);
            }
        }
        // Các phương thức khác...
    }
}
