using BusinessObject;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace DataAccess.DAO
{
    public class DashboardDAO
    {
        private static DashboardDAO instance = null;
        private static object instanceLock = new object();

        public static DashboardDAO Instance
        {
            get
            {
                lock (instanceLock)
                {
                    if (instance == null)
                    {
                        instance = new DashboardDAO();
                    }
                    return instance;
                }
            }
        }

        static string GetConnectionString()
        {
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true, true);
            IConfiguration conn = builder.Build();
            return conn.GetConnectionString("MyStoreDB");
        }

        public int GetTotalProductsFromDatabase()
        {
            int totalProducts;
            try
            {
                var context = new FStoreContext();
                totalProducts = context.Products.Count();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return totalProducts;
        }

        public int GetTotalPendingOrder()
        {
            int total;
            try
            {
                var context = new FStoreContext();
                total = context.Orders.Where(o => o.Status == 0).Count();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return total;
        }

        public decimal CalculateTotalRevenue()
        {
            decimal totalRevenue;
            try
            {
                var context = new FStoreContext();
                totalRevenue = context.OrderDetails.Sum(o => o.UnitPrice * o.Quantity);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return totalRevenue;
        }



        public decimal CalculateTotalRevenueByMonth()
        {
            decimal sum = 0;
            string query = @"
            SELECT 
                SUM(od.UnitPrice * od.Quantity) AS TotalRevenue
            FROM
                [dbo].[Order] o JOIN [dbo].[OrderDetail] od ON o.OrderId = od.OrderId
            WHERE 
                MONTH(o.OrderDate) = MONTH(GETDATE()) AND YEAR(o.OrderDate) = YEAR(GETDATE())";

            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            sum = reader.GetDecimal(0);
                        }
                    }
                }

                connection.Close();
            }

            return sum;
        }
    }
}
