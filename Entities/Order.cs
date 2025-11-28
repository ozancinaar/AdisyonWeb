using System;
using System.Collections.Generic;

namespace AdisyonWeb.Entities
{
    public class Order
    {
        public int OrderId { get; set; }
        public int TableId { get; set; }

        public byte Status { get; set; }          // 0: Açýk, 1: Ödendi, 2: Ýptal
        public DateTime OpenedAt { get; set; }
        public DateTime? ClosedAt { get; set; }
        public decimal TotalAmount { get; set; }
        public string? Note { get; set; }

        public RestaurantTable Table { get; set; } = null!;
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    }
}
