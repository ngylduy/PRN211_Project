namespace dStore.Models
{
    public class DashboardViewModel
    {
        public int TotalProducts { get; set; }
        public int TotalPendingOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal TotalRevenueByMonth { get; set; }
    }
}
