using E_Sport.Models;
using E_Sport.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_Sport.Areas.Admin.Controllers       //NV chăm sóc khách hàng
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,Customer_Service")]
    public class Customer_Service_Controller : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public Customer_Service_Controller(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var orders = await _context.Orders
                .Include(o => o.ApplicationUser)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
            return View(orders);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.Products = await _context.Products.ToListAsync();
            ViewBag.Customers = await _userManager.GetUsersInRoleAsync("Customer");
            return View(); // ✅ Không cần new model
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateOrderViewModel model)
        {
            if (ModelState.IsValid)
            {
                decimal total = 0;
                foreach (var item in model.OrderDetails)
                {
                    var product = await _context.Products.FindAsync(item.ProductId);
                    if (product != null)
                    {
                        total += product.Price * item.Quantity;
                    }
                }

                var order = new Order
                {
                    UserId = model.UserId,
                    ShippingAddress = model.ShippingAddress,
                    Notes = model.Notes,
                    Status = model.Status,
                    OrderDate = DateTime.UtcNow.AddHours(7),
                    TotalPrice = total
                };

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                foreach (var item in model.OrderDetails)
                {
                    _context.OrderDetails.Add(new OrderDetail
                    {
                        OrderId = order.Id,
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        Price = (await _context.Products.FindAsync(item.ProductId)).Price
                    });
                }
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            ViewBag.Products = await _context.Products.ToListAsync();
            ViewBag.Customers = await _userManager.GetUsersInRoleAsync("Customer");
            return View(model);
        }
    }
}
