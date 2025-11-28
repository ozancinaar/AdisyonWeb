using AdisyonWeb.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AdisyonWeb.Controllers
{
    public class TablesController : Controller
    {
        private readonly AppDbContext _context;

        public TablesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /Tables
        public async Task<IActionResult> Index()
        {
            var tables = await _context.Tables
                .OrderBy(t => t.TableNumber)
                .ToListAsync();

            return View(tables);
        }
    }
}
