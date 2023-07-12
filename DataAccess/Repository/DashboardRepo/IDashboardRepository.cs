namespace DataAccess.Repository.DashboardRepo
{
    public interface IDashboardRepository
    {
        public int getTotalProduct();
        public int getTotalPendingOrder();
        public decimal getTotalRevenue();
        public decimal getTotalRevenueByMonth();
    }
}
