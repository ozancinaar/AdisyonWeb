using System;

namespace AdisyonWeb.Entities
{
    public class Payment
    {
        public int PaymentId { get; set; }
        public int OrderId { get; set; }

        public byte PaymentType { get; set; }  // 0: Cash, 1: Card
        public decimal Amount { get; set; }
        public decimal? CashGiven { get; set; }
        public decimal? ChangeAmount { get; set; }
        public DateTime PaidAt { get; set; }

        public Order Order { get; set; } = null!;
    }
}
