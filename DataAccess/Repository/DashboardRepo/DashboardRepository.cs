using DataAccess.DAO;

namespace DataAccess.Repository.DashboardRepo
{
    public class DashboardRepository : IDashboardRepository
    {
        public int getTotalPendingOrder() => DashboardDAO.Instance.GetTotalPendingOrder();
        public int getTotalProduct() => DashboardDAO.Instance.GetTotalProductsFromDatabase();
        public decimal getTotalRevenue() => DashboardDAO.Instance.CalculateTotalRevenue();

        public decimal getTotalRevenueByMonth() => DashboardDAO.Instance.CalculateTotalRevenueByMonth();
    }
}
