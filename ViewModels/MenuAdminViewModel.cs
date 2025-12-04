using System.Collections.Generic;
using AdisyonWeb.Entities;

namespace AdisyonWeb.ViewModels
{
    public class MenuAdminViewModel
    {
        public List<MenuItem> MenuItems { get; set; } = new();
        public List<MenuCategory> Categories { get; set; } = new();

        // Yeni ürün ekleme alanlarý
        public int NewCategoryId { get; set; }
        public string NewName { get; set; }
        public string? NewDescription { get; set; }
        public decimal NewUnitPrice { get; set; }
        public int NewStockQuantity { get; set; }
        public bool NewIsActive { get; set; } = true;
    }
}
