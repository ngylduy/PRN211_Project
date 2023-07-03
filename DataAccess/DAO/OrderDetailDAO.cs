using BusinessObject;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.DAO
{
    public class OrderDetailDAO
    {
        // Using Singleton Pattern
        private static OrderDetailDAO instance = null;
        private static object instanceLock = new object();

        public static OrderDetailDAO Instance
        {
            get
            {
                lock (instanceLock)
                {
                    if (instance == null)
                    {
                        instance = new OrderDetailDAO();
                    }
                    return instance;
                }
            }
        }

        public void AddOrderDetail(OrderDetail orderDetail)
        {
            try
            {
                var context = new FStoreContext();
                context.OrderDetails.Add(orderDetail);
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public decimal GetOrderTotal(int orderId)
        {
            decimal orderTotal = 0;

            try
            {
                var context = new FStoreContext();
                IEnumerable<OrderDetail> orderDetails = context.OrderDetails.Where(od => od.OrderId == orderId);
                foreach (var orderDetail in orderDetails)
                {
                    orderTotal += orderDetail.UnitPrice * orderDetail.Quantity;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return Math.Round(orderTotal, 2);
        }

        public IEnumerable<OrderDetail> GetOrderDetails(int orderId)
        {
            IEnumerable<OrderDetail> orderDetails = null;

            try
            {
                var context = new FStoreContext();
                orderDetails = context.OrderDetails.Where(od => od.OrderId == orderId)
                    .Include(od => od.Product).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return orderDetails;
        }
        public void DeleteOrderDetails(int orderId)
        {
            try
            {
                var context = new FStoreContext();
                IEnumerable<OrderDetail> orderDetails = context.OrderDetails.Where(od => od.OrderId == orderId);
                foreach (var orderDetail in orderDetails)
                {
                    context.Remove(orderDetail);
                }
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public void DeleteByProduct(int productId)
        {
            try
            {
                var context = new FStoreContext();
                IEnumerable<OrderDetail> orderDetails = context.OrderDetails.Where(od => od.ProductId == productId);

                foreach (var orderDetail in orderDetails)
                {
                    context.Remove(orderDetail);
                }
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
