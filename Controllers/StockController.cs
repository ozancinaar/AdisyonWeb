using AdisyonWeb.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AdisyonWeb.Controllers
{
    public class StockController : Controller
    {
        private readonly AppDbContext _context;

        public StockController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _context.MenuItems
                .Include(m => m.Category)
                .OrderBy(m => m.Category.Name)
                .ThenBy(m => m.Name)
                .ToListAsync();

            return View(products);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStock(int id, int stock)
        {
            var item = await _context.MenuItems.FirstOrDefaultAsync(m => m.MenuItemId == id);
            if (item == null)
                return NotFound();

            if (stock < 0)
                stock = 0;

            item.StockQuantity = stock;
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
    }
}
