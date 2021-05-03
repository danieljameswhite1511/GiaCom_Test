using System;

namespace Order.Model
{
    public class OrderMonthlyProfitSummary
    {
        public DateTime MonthEnding { get; set; }
        public int ItemCount { get; set; }
        public decimal TotalCost { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal Profit { get; set; }
    }
}