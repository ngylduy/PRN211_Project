using BusinessObject;
using DataAccess.DAO;

namespace DataAccess.Repository.OrderRepo
{
    public class OrderRepository : IOrderRepository
    {
        public Order AddOrder(Order order) => OrderDAO.Instance.AddOrder(order);

        public void DeleteByMember(int memberId) => OrderDAO.Instance.DeleteByMember(memberId);

        public void DeleteOrder(int orderId) => OrderDAO.Instance.DeleteOrder(orderId);

        public Order GetOrder(int orderId) => OrderDAO.Instance.GetOrder(orderId);

        public IEnumerable<Order> GetOrders(int memberId) => OrderDAO.Instance.GetOrders(memberId);

        public IEnumerable<Order> GetOrders(int memberId, DateTime startDate, DateTime endDate) => OrderDAO.Instance.GetOrders(memberId, startDate, endDate);
    }
}
