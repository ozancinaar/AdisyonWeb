using System;
using System.Linq;
using System.Threading.Tasks;
using AdisyonWeb.Data;
using AdisyonWeb.Entities;
using AdisyonWeb.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AdisyonWeb.Controllers
{
    public class OrdersController : Controller
    {
        private readonly AppDbContext _context;

        public OrdersController(AppDbContext context)
        {
            _context = context;
        }

        // /Orders/Manage?tableId=1
        public async Task<IActionResult> Manage(int tableId)
        {
            // Masa var mý?
            var table = await _context.Tables
                .FirstOrDefaultAsync(t => t.TableId == tableId);

            if (table == null)
                return NotFound();

            // Bu masaya ait açýk sipariþ var mý?
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.MenuItem)
                        .ThenInclude(mi => mi.Category)
                .FirstOrDefaultAsync(o => o.TableId == tableId && o.Status == 0);

            // Yoksa yeni sipariþ aç
            if (order == null)
            {
                order = new Order
                {
                    TableId = tableId,
                    Status = 0,            // Açýk
                    OpenedAt = DateTime.Now,
                    TotalAmount = 0
                };

                _context.Orders.Add(order);
                table.Status = 1;        // Masayý dolu yap

                await _context.SaveChangesAsync();

                // Yeniden yükle ki navigation property'ler dolu gelsin
                order = await _context.Orders
                    .Include(o => o.OrderItems)
                        .ThenInclude(oi => oi.MenuItem)
                            .ThenInclude(mi => mi.Category)
                    .FirstOrDefaultAsync(o => o.OrderId == order.OrderId);
            }

            // Menudeki aktif ürünleri getir
            var menuItems = await _context.MenuItems
                .Include(mi => mi.Category)
                .Where(mi => mi.IsActive)
                .OrderBy(mi => mi.Category.Name)
                .ThenBy(mi => mi.Name)
                .ToListAsync();

            var vm = new OrderManageViewModel
            {
                Table = table,
                Order = order,
                OrderItems = order.OrderItems?.ToList() ?? new(),
                MenuItems = menuItems
            };

            return View(vm);
        }

        // Menüden ürünü sipariþe ekle
        [HttpPost]
        public async Task<IActionResult> AddItem(int orderId, int menuItemId)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);

            if (order == null)
                return NotFound();

            var menuItem = await _context.MenuItems
                .FirstOrDefaultAsync(mi => mi.MenuItemId == menuItemId);

            if (menuItem == null)
                return NotFound();

            if (menuItem.StockQuantity <= 0)
            {
                TempData["Error"] = "Bu ürün için yeterli stok yok.";
                return RedirectToAction("Manage", new { tableId = order.TableId });
            }

            // Stok düþ
            menuItem.StockQuantity -= 1;

            // Sipariþte ayný üründen var mý?
            var existingItem = order.OrderItems
                .FirstOrDefault(oi => oi.MenuItemId == menuItemId);

            if (existingItem == null)
            {
                existingItem = new OrderItem
                {
                    OrderId = order.OrderId,
                    MenuItemId = menuItemId,
                    Quantity = 1,
                    UnitPrice = menuItem.UnitPrice
                };

                // ÖNEMLÝ: Hem context'e hem navigasyon koleksiyonuna ekleyelim
                order.OrderItems.Add(existingItem);
                //_context.OrderItems.Add(existingItem); // Bunu yazmasan da EF, navigation'dan anlar
            }
            else
            {
                existingItem.Quantity += 1;
            }

            // Önce deðiþiklikleri kaydedelim
            await _context.SaveChangesAsync();

            // Sonra DB üzerinden tekrar toplamý hesaplayalým (garanti çözüm)
            order.TotalAmount = await _context.OrderItems
                .Where(oi => oi.OrderId == order.OrderId)
                .SumAsync(oi => oi.Quantity * oi.UnitPrice);

            await _context.SaveChangesAsync();

            return RedirectToAction("Manage", new { tableId = order.TableId });
        }

        // Adet artýr
        public async Task<IActionResult> IncreaseItem(int orderItemId)
        {
            var orderItem = await _context.OrderItems
                .Include(oi => oi.Order)
                .Include(oi => oi.MenuItem)
                .FirstOrDefaultAsync(oi => oi.OrderItemId == orderItemId);

            if (orderItem == null)
                return NotFound();

            if (orderItem.MenuItem.StockQuantity <= 0)
            {
                TempData["Error"] = "Bu ürün için yeterli stok yok.";
                return RedirectToAction("Manage", new { tableId = orderItem.Order.TableId });
            }

            orderItem.MenuItem.StockQuantity -= 1;
            orderItem.Quantity += 1;

            orderItem.Order.TotalAmount = await _context.OrderItems
                .Where(oi => oi.OrderId == orderItem.OrderId)
                .SumAsync(oi => oi.Quantity * oi.UnitPrice);

            await _context.SaveChangesAsync();

            return RedirectToAction("Manage", new { tableId = orderItem.Order.TableId });
        }

        // Adet azalt
        public async Task<IActionResult> DecreaseItem(int orderItemId)
        {
            var orderItem = await _context.OrderItems
                .Include(oi => oi.Order)
                .Include(oi => oi.MenuItem)
                .FirstOrDefaultAsync(oi => oi.OrderItemId == orderItemId);

            if (orderItem == null)
                return NotFound();

            orderItem.MenuItem.StockQuantity += 1;
            orderItem.Quantity -= 1;

            if (orderItem.Quantity <= 0)
            {
                _context.OrderItems.Remove(orderItem);
            }

            orderItem.Order.TotalAmount = await _context.OrderItems
                .Where(oi => oi.OrderId == orderItem.OrderId)
                .SumAsync(oi => oi.Quantity * oi.UnitPrice);

            await _context.SaveChangesAsync();

            return RedirectToAction("Manage", new { tableId = orderItem.Order.TableId });
        }

        // Satýrý tamamen sil
        public async Task<IActionResult> RemoveItem(int orderItemId)
        {
            var orderItem = await _context.OrderItems
                .Include(oi => oi.Order)
                .Include(oi => oi.MenuItem)
                .FirstOrDefaultAsync(oi => oi.OrderItemId == orderItemId);

            if (orderItem == null)
                return NotFound();

            // Stoku iade et
            orderItem.MenuItem.StockQuantity += orderItem.Quantity;

            var tableId = orderItem.Order.TableId;
            var orderId = orderItem.OrderId;

            _context.OrderItems.Remove(orderItem);

            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);

            if (order != null)
            {
                order.TotalAmount = order.OrderItems.Sum(oi => oi.Quantity * oi.UnitPrice);
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("Manage", new { tableId });
        }

        // ÖDEME EKRANI (GET)
        [HttpGet]
        public async Task<IActionResult> Payment(int orderId)
        {
            var order = await _context.Orders
                .Include(o => o.Table)
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.OrderId == orderId && o.Status == 0);

            if (order == null)
                return NotFound();

            var vm = new PaymentViewModel
            {
                OrderId = order.OrderId,
                TableId = order.TableId,
                TotalAmount = order.TotalAmount,
                PaymentType = 0, // Varsayýlan cash
                Table = order.Table,
                Order = order
            };

            return View(vm);
        }

        // ÖDEME (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Payment(PaymentViewModel model)
        {
            var order = await _context.Orders
                .Include(o => o.Table)
                .FirstOrDefaultAsync(o => o.OrderId == model.OrderId && o.Status == 0);

            if (order == null)
                return NotFound();

            var total = order.TotalAmount;
            decimal? change = null;

            // Nakit ise verilen para kontrolü
            if (model.PaymentType == 0)
            {
                if (!model.CashGiven.HasValue)
                {
                    model.ErrorMessage = "Nakit ödeme için verilen para zorunludur.";
                }
                else if (model.CashGiven.Value < total)
                {
                    model.ErrorMessage = "Verilen para toplam tutardan az olamaz.";
                }
                else
                {
                    change = model.CashGiven.Value - total;
                }
            }
            else
            {
                // Kart ise nakit giriþi önemli deðil
                model.CashGiven = null;
                change = 0;
            }

            if (!string.IsNullOrEmpty(model.ErrorMessage))
            {
                // Hata varsa ekrana geri dön
                model.TotalAmount = total;
                model.Table = order.Table!;
                model.Order = order;
                model.ChangeAmount = change;
                return View(model);
            }

            // Ödeme kaydý
            var payment = new Payment
            {
                OrderId = order.OrderId,
                PaymentType = model.PaymentType,
                Amount = total,
                CashGiven = model.PaymentType == 0 ? model.CashGiven : null,
                ChangeAmount = model.PaymentType == 0 ? change : null,
                PaidAt = DateTime.Now
            };

            _context.Payments.Add(payment);

            // Sipariþi kapat
            order.Status = 1;
            order.ClosedAt = DateTime.Now;

            // Masayý boþalt
            if (order.Table != null)
            {
                order.Table.Status = 0;
            }

            await _context.SaveChangesAsync();

            // Masalar ekranýna dön
            return RedirectToAction("Index", "Tables");
        }


    }
}
