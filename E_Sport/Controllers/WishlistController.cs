using E_Sport.Models;
using Microsoft.AspNetCore.Mvc;

namespace E_Sport.Controllers
{
    public class WishlistController : Controller
    {
        private readonly ApplicationDbContext _context;

        public WishlistController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult AddToWishlist(int productId)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value;
            if (userId == null)
            {
                return NotFound("Vui lòng đăng nhập hoặc đăng ký!");
            }
            // Kiểm tra nếu sản phẩm đã có trong wishlist
            var existing = _context.Wishlists.FirstOrDefault(w => w.UserId == userId && w.ProductId == productId);
            if (existing == null)
            {
                var wishlist = new Wishlist
                {
                    UserId = userId,
                    ProductId = productId,
                };
                _context.Wishlists.Add(wishlist);
                _context.SaveChanges();
            }

            return RedirectToAction("Index", "Wishlist");
        }

        public IActionResult Index()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value;

            var wishlistItems = _context.Wishlists
                .Where(w => w.UserId == userId)
                .Select(w => w.Product)
                .ToList();

            return View(wishlistItems);
        }

        [HttpPost]
        public IActionResult RemoveFromWishlist(int productId)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var wishlistItem = _context.Wishlists
                .FirstOrDefault(w => w.UserId == userId && w.ProductId == productId);

            if (wishlistItem != null)
            {
                _context.Wishlists.Remove(wishlistItem);
                _context.SaveChanges();
            }

            return RedirectToAction("Index", "Wishlist");
        }

        //Xóa tất cả 
        [HttpPost]
        public IActionResult ClearWishlist()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var wishlistItems = _context.Wishlists.Where(w => w.UserId == userId).ToList();

            if (wishlistItems.Any())
            {
                _context.Wishlists.RemoveRange(wishlistItems);
                _context.SaveChanges();
            }

            return RedirectToAction("Index", "Wishlist");
        }

    }
}
