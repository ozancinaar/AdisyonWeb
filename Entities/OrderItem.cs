namespace AdisyonWeb.Entities
{
    public class OrderItem
    {
        public int OrderItemId { get; set; }
        public int OrderId { get; set; }
        public int MenuItemId { get; set; }

        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }

        // Sadece okunur property, EF'e map etmiyoruz (Ignore ediyoruz)
        public decimal LineTotal => Quantity * UnitPrice;

        public Order Order { get; set; } = null!;
        public MenuItem MenuItem { get; set; } = null!;
    }
}
