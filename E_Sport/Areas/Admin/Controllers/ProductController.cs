using E_Sport.Models;
using E_Sport.Models.ViewModel;
using E_Sport.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SQLitePCL;

namespace E_Sport.Areas.Admin.Controllers       //quản lý sản phẩm
{
    [Area("Admin")]
    //[Authorize(Roles = SD.Role_Admin)]
    [Authorize(Roles = "Admin,Product_Manager")]
    public class ProductController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ApplicationDbContext _context;
        public ProductController(IProductRepository productRepository,
        ICategoryRepository categoryRepository, ApplicationDbContext context)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _context = context;
        }

        //Hiển thị danh sách sản phẩm
        public async Task<IActionResult> Index()
        {
            var products = await _productRepository.GetAllAsync();
            return View(products);
        }
        // GET: Thêm sản phẩm trực tiếp (không dùng ViewModel)
        public async Task<IActionResult> Add()
        {
            var categories = await _categoryRepository.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");
            ViewBag.FieldTypes = new SelectList(await _context.FieldTypes.ToListAsync(), "Id", "Name");

            return View(new Product());
        }

        //[HttpPost]
        //public async Task<IActionResult> Add(Product product, IFormFile imageUrl)
        //{
        //    // 🐞 Debug lỗi ModelState nếu có
        //    foreach (var key in ModelState.Keys)
        //    {
        //        var errors = ModelState[key].Errors;
        //        foreach (var error in errors)
        //        {
        //            Console.WriteLine($"❌ Lỗi ModelState tại {key}: {error.ErrorMessage}");
        //        }
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        if (imageUrl != null)
        //        {
        //            product.ImageUrl = await SaveImage(imageUrl);
        //        }

        //        await _productRepository.AddAsync(product);
        //        await _context.SaveChangesAsync();

        //        return RedirectToAction(nameof(Index));
        //    }

        //    var categories = await _categoryRepository.GetAllAsync();
        //    ViewBag.Categories = new SelectList(categories, "Id", "Name");
        //    ViewBag.FieldTypes = new SelectList(await _context.FieldTypes.ToListAsync(), "Id", "Name");

        //    return View(product);
        //}

        [HttpPost]
        public async Task<IActionResult> Add(Product product, IFormFile imageUrl, string SizeInput)
        {
            if (ModelState.IsValid)
            {
                if (imageUrl != null)
                {
                    product.ImageUrl = await SaveImage(imageUrl);
                }

                // Thêm sản phẩm
                await _productRepository.AddAsync(product);
                await _context.SaveChangesAsync(); // đảm bảo product.Id được sinh

                // ✅ Xử lý size nếu có
                if (!string.IsNullOrWhiteSpace(SizeInput))
                {
                    var sizes = SizeInput.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                         .Select(s => s.Trim())
                                         .Where(s => !string.IsNullOrWhiteSpace(s))
                                         .Distinct()
                                         .ToList();

                    foreach (var size in sizes)
                    {
                        _context.ProductSizes.Add(new ProductSize
                        {
                            ProductId = product.Id,
                            Size = size,
                            IsAvailable = true
                        });
                    }

                    await _context.SaveChangesAsync();
                }

                return RedirectToAction(nameof(Index));
            }

            // Nếu lỗi, load lại dropdown
            var categories = await _categoryRepository.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");
            ViewBag.FieldTypes = new SelectList(await _context.FieldTypes.ToListAsync(), "Id", "Name");

            return View(product);
        }




        // Viết thêm hàm SaveImage (tham khảo bài 02)
        private async Task<string> SaveImage(IFormFile image)
        {
            var savePath = Path.Combine("wwwroot/images", image.FileName); // Thay đổi đường dẫn theo cấu hình của bạn
            using (var fileStream = new FileStream(savePath, FileMode.Create))
            {
                await image.CopyToAsync(fileStream);
            }
            return "/images/" + image.FileName; // Trả về đường dẫn tương đối
        }

        // Hiển thị thông tin chi tiết sản phẩm
        public async Task<IActionResult> Display(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // Hiển thị form cập nhật sản phẩm
        public async Task<IActionResult> Update(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            var categories = await _categoryRepository.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name", product.CategoryId);

            var fieldTypes = await _context.FieldTypes.ToListAsync();
            ViewBag.FieldTypes = new SelectList(fieldTypes, "Id", "Name", product.FieldTypeId);
            return View(product);
        }

        // Xử lý cập nhật sản phẩm
        [HttpPost]
        public async Task<IActionResult> Update(int id, Product product,
        IFormFile imageUrl)
        {
            ModelState.Remove("ImageUrl"); // Loại bỏ xác thực ModelState cho ImageUrl
            if (id != product.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                var existingProduct = await
                _productRepository.GetByIdAsync(id); // Giả định có phương thức GetByIdAsync
                                                     // Giữ nguyên thông tin hình ảnh nếu không có hình mới được tải lên
                if (imageUrl == null)
                {
                    product.ImageUrl = existingProduct.ImageUrl;
                }
                else
                {
                    // Lưu hình ảnh mới
                    product.ImageUrl = await SaveImage(imageUrl);
                }
                // Cập nhật các thông tin khác của sản phẩm
                existingProduct.Name = product.Name;
                existingProduct.Price = product.Price;
                existingProduct.Description = product.Description;
                existingProduct.CategoryId = product.CategoryId;
                existingProduct.ImageUrl = product.ImageUrl;
                await _productRepository.UpdateAsync(existingProduct);
                return RedirectToAction(nameof(Index));
            }
            var categories = await _categoryRepository.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");

            var fieldTypes = await _context.FieldTypes.ToListAsync();
            ViewBag.FieldTypes = new SelectList(fieldTypes, "Id", "Name", product.FieldTypeId); // ✅ chọn đúng loại sân

            return View(product);
        }

        // Hiển thị form xác nhận xóa sản phẩm
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // Xử lý xóa sản phẩm
        [HttpPost, ActionName("DeleteConfirmed")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _productRepository.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
