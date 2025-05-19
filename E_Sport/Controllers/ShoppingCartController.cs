//using E_Sport.Extensions;
//using E_Sport.Models;
//using E_Sport.Repositories;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Mvc;

//namespace E_Sport.Controllers
//{
//    public class ShoppingCartController : Controller
//    {
//        private readonly IProductRepository _productRepository;
//        private readonly ApplicationDbContext _context;
//        private readonly UserManager<ApplicationUser> _userManager;
//        public ShoppingCartController(ApplicationDbContext context,
//        UserManager<ApplicationUser> userManager, IProductRepository
//        productRepository)
//        {
//            _productRepository = productRepository;
//            _context = context;
//            _userManager = userManager;
//        }
//        public async Task<IActionResult> AddToCart(int productId, int quantity, string size)
//        {
//            // Giả sử bạn có phương thức lấy thông tin sản phẩm từ productId
//            var product = await GetProductFromDatabase(productId);
//            if (product == null || string.IsNullOrEmpty(size))
//                return NotFound();
//            var cartItem = new CartItem
//            {
//                ProductId = productId,
//                Name = product.Name,
//                Price = product.Price,
//                Quantity = quantity
//            };
//            var cart =
//            HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart") ?? new
//            ShoppingCart();
//            cart.AddItem(cartItem);
//            HttpContext.Session.SetObjectAsJson("Cart", cart);
//            return RedirectToAction("Index");
//        }
//        public IActionResult Index()
//        {
//            var cart =
//            HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart") ?? new
//            ShoppingCart();
//            return View(cart);
//        }
//        // Các actions khác...
//        private async Task<Product> GetProductFromDatabase(int productId)
//        {
//            // Truy vấn cơ sở dữ liệu để lấy thông tin sản phẩm
//            var product = await _productRepository.GetByIdAsync(productId);
//            return product;
//        }
//        public IActionResult RemoveFromCart(int productId)
//        {
//            var cart =
//            HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart");
//            if (cart is not null)
//            {
//                cart.RemoveItem(productId);// Lưu lại giỏ hàng vào Session sau khi đã xóa mục
//                HttpContext.Session.SetObjectAsJson("Cart", cart);
//            }
//            return RedirectToAction("Index");
//        }

//        public IActionResult Checkout()
//        {
//            return View(new Order());
//        }
//        [
//        HttpPost]
//        public async Task<IActionResult> Checkout(Order order)
//        {
//            var cart =
//            HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart");
//            if (cart == null || !cart.Items.Any())
//            {
//                // Xử lý giỏ hàng trống...
//                return RedirectToAction("Index");
//            }

//            var user = await _userManager.GetUserAsync(User);
//            if (user == null) 
//            {
//                return NotFound("Vui lòng đăng nhập");
//            }
//            order.UserId = user.Id;
//            order.OrderDate = DateTime.UtcNow;
//            order.TotalPrice = cart.Items.Sum(i => i.Price * i.Quantity);
//            order.OrderDetails = cart.Items.Select(i => new OrderDetail
//            {
//                ProductId = i.ProductId,
//                Quantity = i.Quantity,
//                Price = i.Price
//            }).ToList();
//            _context.Orders.Add(order);
//            await _context.SaveChangesAsync();
//            HttpContext.Session.Remove("Cart");
//            return View("OrderCompleted", order.Id); // Trang xác nhận hoàn thành đơn hàng
//        }
//    }
//}

using E_Sport.Extensions;
using E_Sport.Models;
using E_Sport.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace E_Sport.Controllers
{
    public class ShoppingCartController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ShoppingCartController(ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            IProductRepository productRepository)
        {
            _productRepository = productRepository;
            _context = context;
            _userManager = userManager;
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId, int quantity, string size)
        {
            var product = await GetProductFromDatabase(productId);
            if (product == null || string.IsNullOrEmpty(size))
                return NotFound();

            var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart") ?? new ShoppingCart();

            // Check theo productId + size
            var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == productId && i.Size == size);
            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                var cartItem = new CartItem
                {
                    ProductId = productId,
                    Name = product.Name,
                    Price = product.Price,
                    Quantity = quantity,
                    Size = size
                };
                cart.AddItem(cartItem);
            }

            HttpContext.Session.SetObjectAsJson("Cart", cart);
            return RedirectToAction("Index");
        }

        public IActionResult Index()
        {
            var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart") ?? new ShoppingCart();
            return View(cart);
        }

        public IActionResult RemoveFromCart(int productId, string size)
        {
            var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart");
            if (cart is not null)
            {
                cart.RemoveItem(productId, size); // Sửa RemoveItem nhận thêm size
                HttpContext.Session.SetObjectAsJson("Cart", cart);
            }
            return RedirectToAction("Index");
        }

        public IActionResult Checkout()
        {
            return View(new Order());
        }

        [HttpPost]
        public async Task<IActionResult> Checkout(Order order)
        {
            var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart");
            if (cart == null || !cart.Items.Any())
                return RedirectToAction("Index");

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return NotFound("Vui lòng đăng nhập");

            order.UserId = user.Id;
            order.OrderDate = DateTime.UtcNow;
            order.TotalPrice = cart.Items.Sum(i => i.Price * i.Quantity);
            order.OrderDetails = cart.Items.Select(i => new OrderDetail
            {
                ProductId = i.ProductId,
                Quantity = i.Quantity,
                Price = i.Price,
                // Nếu bạn thêm Size trong OrderDetail thì gán thêm ở đây
            }).ToList();

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            HttpContext.Session.Remove("Cart");

            return View("OrderCompleted", order.Id);
        }

        private async Task<Product> GetProductFromDatabase(int productId)
        {
            return await _productRepository.GetByIdAsync(productId);
        }
    }
}
