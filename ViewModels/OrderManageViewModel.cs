using System.Collections.Generic;
using AdisyonWeb.Entities;

namespace AdisyonWeb.ViewModels
{
    public class OrderManageViewModel
    {
        public RestaurantTable Table { get; set; }
        public Order Order { get; set; }
        public List<OrderItem> OrderItems { get; set; } = new();
        public List<MenuItem> MenuItems { get; set; } = new();
    }
}
