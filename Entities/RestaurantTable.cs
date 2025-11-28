using System;
using System.Collections.Generic;

namespace AdisyonWeb.Entities
{
    public class RestaurantTable
    {
        public int TableId { get; set; }
        public int TableNumber { get; set; }
        public string? Description { get; set; }
        public byte Status { get; set; }          // 0: Boþ, 1: Dolu
        public DateTime CreatedAt { get; set; }

        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
