namespace _3.QKA_DACK.Models.OrderModels
{
    public class MonthlyRevenueDto
    {
        public int Month { get; set; }
        public decimal TotalRevenue { get; set; }
        public int OrderCount { get; set; }
        public decimal AvgOrderValue { get; set; }
    }
}
