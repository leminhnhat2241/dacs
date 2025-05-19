using E_Sport.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_Sport.Controllers
{
    [Area("Admin")]
    //[Authorize(Roles = SD.Role_Admin)]
    [Authorize(Roles = "Admin")]
    public class FieldTypeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FieldTypeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: FieldType
        public async Task<IActionResult> Index()
        {
            return View(await _context.FieldTypes.ToListAsync());
        }

        // GET: FieldType/Add
        public IActionResult Add()
        {
            return View();
        }

        // POST: FieldType/Add
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Add(FieldType fieldType)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(fieldType);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(fieldType);
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(FieldType fieldType)
        {
            if (ModelState.IsValid)
            {
                _context.Add(fieldType);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // 👇 THÊM ĐOẠN NÀY để debug
            Console.WriteLine("❌ Không hợp lệ. Giá trị Name: " + fieldType.Name);
            foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
            {
                Console.WriteLine("🔴 Lỗi: " + error.ErrorMessage);
            }

            return View(fieldType);
        }


        // GET: FieldType/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var fieldType = await _context.FieldTypes.FindAsync(id);
            if (fieldType == null) return NotFound();
            return View(fieldType);
        }

        // POST: FieldType/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, FieldType fieldType)
        {
            if (id != fieldType.Id) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(fieldType);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(fieldType);
        }

        // GET: FieldType/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var fieldType = await _context.FieldTypes.FindAsync(id);
            if (fieldType == null) 
                return NotFound();
            return View(fieldType);
        }

        // POST: FieldType/DeleteConfirmed
        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var fieldType = await _context.FieldTypes.FindAsync(id);
            if (fieldType != null)
            {
                _context.FieldTypes.Remove(fieldType);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
