using AdisyonWeb.Entities;
using Microsoft.EntityFrameworkCore;

namespace AdisyonWeb.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<RestaurantTable> Tables { get; set; }
        public DbSet<MenuCategory> MenuCategories { get; set; }
        public DbSet<MenuItem> MenuItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Payment> Payments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 🔑 Primary key'leri açıkça tanımlıyoruz
            modelBuilder.Entity<RestaurantTable>()
                .HasKey(t => t.TableId);

            modelBuilder.Entity<MenuCategory>()
                .HasKey(c => c.CategoryId);

            modelBuilder.Entity<MenuItem>()
                .HasKey(mi => mi.MenuItemId);

            modelBuilder.Entity<Order>()
                .HasKey(o => o.OrderId);

            modelBuilder.Entity<OrderItem>()
                .HasKey(oi => oi.OrderItemId);

            modelBuilder.Entity<Payment>()
                .HasKey(p => p.PaymentId);

            // İlişki örnekleri (konvansiyonla da çalışır ama açık yazmak iyi olur)
            modelBuilder.Entity<MenuItem>()
                .HasOne(mi => mi.Category)
                .WithMany(c => c.MenuItems)
                .HasForeignKey(mi => mi.CategoryId);

            modelBuilder.Entity<Order>()
                .HasOne(o => o.Table)
                .WithMany(t => t.Orders)
                .HasForeignKey(o => o.TableId);

            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(oi => oi.OrderId);

            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.MenuItem)
                .WithMany(mi => mi.OrderItems)
                .HasForeignKey(oi => oi.MenuItemId);

            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Order)
                .WithMany(o => o.Payments)
                .HasForeignKey(p => p.OrderId);

            // OrderItem.LineTotal sadece C# tarafında hesaplanacak, DB'deki computed kolona güvenmiyoruz
            modelBuilder.Entity<OrderItem>()
                .Ignore(oi => oi.LineTotal);
        }
    }
}
