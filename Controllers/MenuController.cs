using System.Linq;
using System.Threading.Tasks;
using AdisyonWeb.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AdisyonWeb.Controllers
{
    public class MenuController : Controller
    {
        private readonly AppDbContext _context;

        public MenuController(AppDbContext context)
        {
            _context = context;
        }

        // /Menu
        public async Task<IActionResult> Index()
        {
            var categories = await _context.MenuCategories
                .Include(c => c.MenuItems)
                .OrderBy(c => c.Name)
                .ToListAsync();

            return View(categories);
        }
    }
}
