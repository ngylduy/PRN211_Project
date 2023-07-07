using BusinessObject;

namespace DataAccess.Repository.OrderRepo
{
    public interface IOrderRepository
    {
        public Order AddOrder(Order order);
        public IEnumerable<Order> GetOrders(int memberId);
        public IEnumerable<Order> GetOrders(int memberId, DateTime startDate, DateTime endDate);
        public void UpdateOrderStatus(int id, int status);
        public Order GetOrder(int orderId);
        public void DeleteOrder(int orderId);
        public void DeleteByMember(int memberId);
    }
}
