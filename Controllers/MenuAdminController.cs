using System.Linq;
using System.Threading.Tasks;
using AdisyonWeb.Data;
using AdisyonWeb.Entities;
using AdisyonWeb.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AdisyonWeb.Controllers
{
    public class MenuAdminController : Controller
    {
        private readonly AppDbContext _context;

        public MenuAdminController(AppDbContext context)
        {
            _context = context;
        }

        // Menü yönetimi ana ekraný
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var items = await _context.MenuItems
                .Include(m => m.Category)
                .OrderBy(m => m.Category.Name)
                .ThenBy(m => m.Name)
                .ToListAsync();

            var categories = await _context.MenuCategories
                .OrderBy(c => c.Name)
                .ToListAsync();

            var vm = new MenuAdminViewModel
            {
                MenuItems = items,
                Categories = categories,
                NewIsActive = true
            };

            return View(vm);
        }

        // Yeni ürün ekleme
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(MenuAdminViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.NewName) || model.NewCategoryId == 0)
            {
                TempData["Error"] = "Kategori ve ürün adý zorunludur.";
                return RedirectToAction("Index");
            }

            var category = await _context.MenuCategories
                .FirstOrDefaultAsync(c => c.CategoryId == model.NewCategoryId);

            if (category == null)
            {
                TempData["Error"] = "Geçersiz kategori.";
                return RedirectToAction("Index");
            }

            var item = new MenuItem
            {
                CategoryId = model.NewCategoryId,
                Name = model.NewName,
                Description = model.NewDescription,
                UnitPrice = model.NewUnitPrice,
                StockQuantity = model.NewStockQuantity,
                IsActive = model.NewIsActive
            };

            _context.MenuItems.Add(item);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Yeni ürün baþarýyla eklendi.";
            return RedirectToAction("Index");
        }

        // Mevcut ürünü güncelle
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(
            int id,
            string name,
            string description,
            decimal unitPrice,
            int stockQuantity,
            bool isActiveFlag,
            int categoryId)
        {
            var item = await _context.MenuItems.FirstOrDefaultAsync(m => m.MenuItemId == id);
            if (item == null)
                return NotFound();

            if (string.IsNullOrWhiteSpace(name))
            {
                TempData["Error"] = "Ürün adý boþ olamaz.";
                return RedirectToAction("Index");
            }

            item.Name = name;
            item.Description = description;
            item.UnitPrice = unitPrice;
            item.StockQuantity = stockQuantity < 0 ? 0 : stockQuantity;
            item.IsActive = isActiveFlag;
            item.CategoryId = categoryId;

            await _context.SaveChangesAsync();

            TempData["Success"] = "Ürün bilgileri güncellendi.";
            return RedirectToAction("Index");
        }

        // Ürün sil (opsiyonel ama iþe yarar)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _context.MenuItems.FirstOrDefaultAsync(m => m.MenuItemId == id);
            if (item != null)
            {
                _context.MenuItems.Remove(item);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Ürün silindi.";
            }

            return RedirectToAction("Index");
        }
    }
}
