﻿using BusinessObject;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.DAO
{
    public class OrderDAO
    {
        // Using Singleton Pattern
        private static OrderDAO instance = null;
        private static object instanceLock = new object();

        public static OrderDAO Instance
        {
            get
            {
                lock (instanceLock)
                {
                    if (instance == null)
                    {
                        instance = new OrderDAO();
                    }
                    return instance;
                }
            }
        }

        public Order AddOrder(Order order)
        {
            Order returnOrder = null;
            try
            {
                var context = new FStoreContext();
                context.Orders.Add(order);
                context.SaveChanges();
                returnOrder = order;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return order;
        }
        public IEnumerable<Order> GetOrders(int memberId)
        {
            IEnumerable<Order> orders = null;

            try
            {
                var context = new FStoreContext();
                if (memberId > 0)
                {
                    orders = context.Orders.Where(or => or.MemberId == memberId)
                        .Include(or => or.Member)
                        .Include(or => or.OrderDetails);
                }
                else if (memberId <= 0)
                {
                    orders = context.Orders.Include(or => or.Member)
                        .Include(or => or.OrderDetails);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return orders;
        }
        public IEnumerable<Order> GetOrders(int memberId, DateTime startDate, DateTime endDate)
        {
            IEnumerable<Order> orders = null;

            try
            {
                var context = new FStoreContext();
                if (memberId <= 0)
                {
                    orders = context.Orders.Where(or =>
                            DateTime.Compare(or.OrderDate, startDate) >= 0 &&
                            DateTime.Compare(or.OrderDate, endDate) <= 0)
                    .Include(or => or.Member)
                    .Include(or => or.OrderDetails);
                }
                else
                {
                    orders = context.Orders.Where(or => or.MemberId == memberId &&
                            DateTime.Compare(or.OrderDate, startDate) >= 0 &&
                            DateTime.Compare(or.OrderDate, endDate) <= 0)
                    .Include(or => or.Member)
                    .Include(or => or.OrderDetails);
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return orders;
        }
        public Order GetOrder(int orderId)
        {
            Order order = null;

            try
            {
                var context = new FStoreContext();
                order = context.Orders.Where(or => or.OrderId == orderId).Include(or => or.Member).Include(or => or.OrderDetails).First();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return order;
        }
        public void DeleteOrder(int orderId)
        {
            try
            {
                var context = new FStoreContext();
                context.Orders.Remove(GetOrder(orderId));
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public void DeleteByMember(int memberId)
        {
            try
            {
                IEnumerable<Order> orders = GetOrders(memberId);
                var context = new FStoreContext();
                foreach (var order in orders)
                {
                    context.Remove(order);
                }
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void Update(int id, int status)
        {
            try
            {
                Order _order = GetOrder(id);
                if (_order != null)
                {
                    var context = new FStoreContext();
                    _order.Status = status;
                    context.Orders.Update(_order);
                    context.SaveChanges();
                }
                else
                {
                    throw new Exception("Order does not exist!!");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }


    }
}
