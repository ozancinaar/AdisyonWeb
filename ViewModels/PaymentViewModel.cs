using AdisyonWeb.Entities;

namespace AdisyonWeb.ViewModels
{
    public class PaymentViewModel
    {
        public int OrderId { get; set; }
        public int TableId { get; set; }

        public decimal TotalAmount { get; set; }

        public byte PaymentType { get; set; } // 0: Cash, 1: Card
        public decimal? CashGiven { get; set; }
        public decimal? ChangeAmount { get; set; }

        public RestaurantTable Table { get; set; }
        public Order Order { get; set; }

        public string? ErrorMessage { get; set; } // 🔴 Hata mesajı için
    }
}
