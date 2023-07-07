using BusinessObject;
using DataAccess.Repository.MemberRepo;
using DataAccess.Repository.OrderDetailRepo;
using DataAccess.Repository.OrderRepo;
using DataAccess.Repository.ProductRepo;
using dStore.Models;
using eStore.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace dStore.Controllers
{
    public class AdminController : Controller
    {

        private IMemberRepository memberRepository;
        private IProductRepository productRepository;
        private FStoreContext context;
        private IOrderRepository orderRepository;
        private IOrderDetailRepository orderDetailRepository;

        public AdminController()
        {
            memberRepository = new MemberRepository();
            productRepository = new ProductRepository();
            context = new FStoreContext();
            orderRepository = new OrderRepository();
            orderDetailRepository = new OrderDetailRepository();
        }

        public async Task<IActionResult> Order(DateTime? start, DateTime? end, int? page)
        {
            try
            {
                ViewBag.Start = (start == null) ? "" : start.Value.Date.ToString("yyyy-MM-dd");
                ViewBag.End = (end == null) ? "" : end.Value.Date.ToString("yyyy-MM-dd");

                if (page == null)
                {
                    page = 1;
                }

                int memberId = int.Parse(User.Claims.First(c => c.Type.Equals("MemberId")).Value);
                IEnumerable<Order> orders = orderRepository.GetOrders(memberId);

                if (start != null && end != null)
                {
                    if (start > end)
                    {
                        DateTime? temp = start;
                        start = end;
                        end = temp;
                    }
                    orders = orderRepository.GetOrders(memberId, start.Value, end.Value);
                }
                else if ((start != null && end == null) || (start == null && end != null))
                {
                    throw new Exception("Please fill both of the Start and End Date inputs to filter or leave them blank!");
                }
                else if (start == null && end == null)
                {
                    ViewBag.Start = orders.Min(or => or.OrderDate).Date.ToString("yyyy-MM-dd");
                    ViewBag.End = orders.Max(or => or.OrderDate).Date.ToString("yyyy-MM-dd");
                }

                List<OrderExportData> orderExport = new List<OrderExportData>();

                foreach (var order in orders)
                {
                    decimal total = orderDetailRepository.GetOrderTotal(order.OrderId);
                    ViewData[order.OrderId.ToString()] = Math.Round(total, 2);
                    orderExport.Add(new OrderExportData
                    {
                        OrderID = order.OrderId,
                        MemberName = order.Member.Fullname,
                        OrderDate = order.OrderDate,
                        OrderTotal = total
                    });
                }
                HttpContext.Session.SetComplexData("OrderData", orderExport);
                int pageSize = 10;

                return View(await PaginatedList<Order>.CreateAsync(orders.AsQueryable(), page ?? 1, pageSize));
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View();
            }
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        // GET: MemberController
        public async Task<IActionResult> Member(string search, int? page)
        {
            if (page == null)
            {
                page = 1;
            }

            IEnumerable<Member> members = memberRepository.GetMembersList().OrderByDescending(mem => mem.Fullname);
            if (!string.IsNullOrEmpty(search))
            {
                members = members.Where(mem => mem.Fullname.ToLower().Contains(search.ToLower()));
                ViewBag.Search = search;
            }

            int pageSize = 10;

            return View(await PaginatedList<Member>.CreateAsync(members.AsQueryable(), page ?? 1, pageSize));
        }

        [Authorize(Roles = "Admin")]
        // GET: ProductController
        public async Task<IActionResult> Product(string search, string? sort, decimal? from, decimal? to, int? page, int category = 0)
        {
            ViewBag.Product = true;
            ViewBag.Categories = context.Categories.ToList();
            try
            {
                ViewBag.Search = search;
                ViewBag.From = from;
                ViewBag.To = to;

                if (page == null)
                {
                    page = 1;
                }
                IEnumerable<Product> products;
                ViewBag.SelectedCate = category;

                if (category != 0)
                {
                    products = productRepository.GetProductsList().Where(p => p.CategoryId == category).OrderByDescending(pro => pro.ProductName);
                }
                else
                {

                }
                {
                    products = productRepository.GetProductsList().OrderByDescending(pro => pro.ProductName);
                }
                if (!string.IsNullOrEmpty(search))
                {
                    products = productRepository.SearchProduct(search, products);
                }
                if (from != null && to != null)
                {
                    if (from > to)
                    {
                        decimal? temp = from;
                        from = to;
                        to = temp;
                    }
                    products = productRepository.SearchProduct(from.Value, to.Value, products);
                }
                else if ((from != null && to == null) || (from == null && to != null))
                {
                    throw new Exception("Please fill both of the Unit Price inputs to filter or leave them blank!");
                }

                if (!string.IsNullOrEmpty(sort))
                {
                    products = productRepository.SortProduct(sort, products);
                }

                int pageSize = 6;
                return View("Product", await PaginatedList<Product>.CreateAsync(products.AsQueryable(), page ?? 1, pageSize));
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View("Product");
            }

        }

        public ActionResult OrderDetailsPopup(int id)
        {
            Order order = orderRepository.GetOrder(id);
            if (order == null)
            {
                return NotFound();
            }
            order.OrderDetails = orderDetailRepository.GetOrderDetails(order.OrderId).ToList();
            decimal orderTotal = orderDetailRepository.GetOrderTotal(order.OrderId);
            ViewBag.OrderTotal = orderTotal;

            return PartialView("_OrderDetailsPopup", order);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult UpdateOrderStatus(int id, int status)
        {
            Order order = orderRepository.GetOrder(id);
            if (order == null)
            {
                return NotFound();
            }
            orderRepository.UpdateOrderStatus(id, status);
            return RedirectToAction("Order");
        }

    }
}
