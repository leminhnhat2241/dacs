using E_Sport.Models;
using E_Sport.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace E_Sport.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;  // EF DbContext

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Dashboard
        public async Task<IActionResult> Index()
        {
            // Lấy dữ liệu thống kê cơ bản (ví dụ đếm số sản phẩm, đơn hàng,...)
            var productCount = await Task.FromResult(_context.Products.Count());
            var orderCount = await Task.FromResult(_context.Orders.Count());
            var userCount = await Task.FromResult(_context.Users.Count());

            // Tạo ViewModel chứa dữ liệu để gửi xuống View
            var model = new DashboardViewModel
            {
                ProductCount = productCount,
                OrderCount = orderCount,
                UserCount = userCount
            };

            return View(model);
        }
    }
}
