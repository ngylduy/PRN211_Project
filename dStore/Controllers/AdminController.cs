using BusinessObject;
using DataAccess.Repository.CategoryRepo;
using DataAccess.Repository.DashboardRepo;
using DataAccess.Repository.MemberRepo;
using DataAccess.Repository.OrderDetailRepo;
using DataAccess.Repository.OrderRepo;
using DataAccess.Repository.ProductRepo;
using dStore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace dStore.Controllers
{
    public class AdminController : Controller
    {

        private IMemberRepository memberRepository;
        private IProductRepository productRepository;
        private FStoreContext context;
        private IOrderRepository orderRepository;
        private IOrderDetailRepository orderDetailRepository;
        private IWebHostEnvironment _webHostEnvironment;
        private ICategoryRepository categoryRepository;
        private IDashboardRepository dashboardRepository;

        public AdminController(IWebHostEnvironment webHostEnvironment)
        {
            memberRepository = new MemberRepository();
            productRepository = new ProductRepository();
            context = new FStoreContext();
            orderRepository = new OrderRepository();
            orderDetailRepository = new OrderDetailRepository();
            _webHostEnvironment = webHostEnvironment;
            categoryRepository = new CategoryRepository();
            dashboardRepository = new DashboardRepository();
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Index()
        {
            var viewModel = new DashboardViewModel
            {
                TotalProducts = dashboardRepository.getTotalProduct(),
                TotalRevenue = dashboardRepository.getTotalRevenue(),
                TotalRevenueByMonth = dashboardRepository.getTotalRevenueByMonth(),
                TotalPendingOrders = dashboardRepository.getTotalPendingOrder()
            };

            return View(viewModel);
        }

        #region Order Management
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
        #endregion

        #region Member Management
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
        [HttpPost]
        public IActionResult UpdateStatusMember(int id, int status)
        {
            Member member = memberRepository.GetMember(id);
            if (member == null)
            {
                throw new Exception("Member ID is not exit! Please try again");
            }
            memberRepository.UpdateMemberStatus(id, status);
            if (status == 1)
            {
                TempData["Update"] = "Unban member with ID <strong>" + id + "</strong> successfully!!";
            }
            else
            {
                TempData["Update"] = "Baned member with ID <strong>" + id + "</strong> successfully!!";
            }
            return RedirectToAction("Member");
        }

        #endregion

        #region Product Management
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


        public ActionResult CreateProduct()
        {
            try
            {
                IEnumerable<Category> categories = categoryRepository.GetCategoryList();
                var categoriesList = new SelectList(categories.ToDictionary(cate => cate.CategoryId, cate => cate.CategoryName), "Key", "Value");
                ViewBag.Category = categoriesList;
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View();
            }
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateProduct(Product product, IFormFile fileAnh)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (fileAnh != null && fileAnh.Length > 0)
                    {
                        string uniqueFileName = Guid.NewGuid().ToString() + "_" + System.IO.Path.GetFileName(fileAnh.FileName);
                        string imagePath = System.IO.Path.Combine(_webHostEnvironment.WebRootPath, "images", uniqueFileName);
                        using (var fileStream = new FileStream(imagePath, FileMode.Create))
                        {
                            fileAnh.CopyTo(fileStream);
                        }
                        product.Image = "/images/" + uniqueFileName;
                    }
                    else
                    {
                        throw new Exception("Image product is not empty!");
                    }
                    TempData["Create"] = "Create Product successfully!!";
                    productRepository.AddProduct(product);
                }
                return RedirectToAction(nameof(Product));
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.InnerException?.Message ?? ex.Message;
                return View(product);
            }
        }

        [Authorize(Roles = "Admin")]
        public ActionResult EditProduct(int? id)
        {
            try
            {
                if (id == null)
                {
                    throw new Exception("Product ID is not found!!!");
                }
                Product product = productRepository.GetProduct(id.Value);
                if (product == null)
                {
                    throw new Exception("Product ID is not found!!!");
                }

                ICategoryRepository categoryRepository = new CategoryRepository();
                IEnumerable<Category> categories = categoryRepository.GetCategoryList();
                var categoriesList = new SelectList(categories.ToDictionary(cate => cate.CategoryId, cate => cate.CategoryName), "Key", "Value");
                ViewBag.Category = categoriesList;

                return View(product);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View();
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditProduct(int? id, Product product, IFormFile fileAnh)
        {
            try
            {
                Product oProduct = productRepository.GetProduct(id.Value);
                if (oProduct == null)
                {
                    throw new Exception("Product ID is not found!! Please try again");
                }
                if (fileAnh != null && fileAnh.Length > 0)
                {
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + System.IO.Path.GetFileName(fileAnh.FileName);
                    string imagePath = System.IO.Path.Combine(_webHostEnvironment.WebRootPath, "images", uniqueFileName);
                    using (var fileStream = new FileStream(imagePath, FileMode.Create))
                    {
                        fileAnh.CopyTo(fileStream);
                    }
                    product.Image = "/images/" + uniqueFileName;
                }
                else
                {
                    product.Image = oProduct.Image;
                }
                productRepository.Update(product);
                TempData["Update"] = "Update Product with the ID <strong>" + id + "</strong> successfully!!";
                return RedirectToAction(nameof(Product));
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View(product);
            }
        }

        public ActionResult DeleteProductPopup(int? id)
        {
            Product product = productRepository.GetProduct(id.Value);
            if (product == null)
            {
                throw new Exception("Product ID is not found!!!");
            }
            return PartialView("_DeleteProductPopup", product);
        }

        [HttpPost]
        public ActionResult DeleteProduct(int id)
        {
            Product product = productRepository.GetProduct(id);
            if (product == null)
            {
                return NotFound();
            }
            IOrderDetailRepository orderDetailRepository = new OrderDetailRepository();
            orderDetailRepository.DeleteByProduct(id);
            productRepository.Delete(id);
            TempData["Delete"] = "Delete Product with the ID <strong>" + id + "</strong> successfully!!";
            return RedirectToAction("Product");

        }
        #endregion

        #region Category Management
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Category()
        {
            IEnumerable<Category> categories = categoryRepository.GetCategoryList();
            return View(categories);
        }
        [Authorize(Roles = "Admin")]
        public ActionResult CreateCategoryPopup()
        {
            return PartialView("_CreateCategoryPopup");
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult CreateCategory(string categoryName)
        {
            try
            {
                if (!string.IsNullOrEmpty(categoryName))
                {
                    categoryRepository.AddCategory(categoryName);
                }
                else
                {
                    throw new Exception("The Category Name is empty!!");
                }
                ViewBag.OK = "Create Category successfully!";
                return RedirectToAction(nameof(Category));
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Can not create new category!";
                return RedirectToAction(nameof(Category));
            }

        }
        [Authorize(Roles = "Admin")]
        public ActionResult DeleteCategoryPopup(int? id)
        {
            Category category = categoryRepository.GetCategory(id.Value);
            if (category == null)
            {
                throw new Exception("Product ID is not found!!!");
            }
            return PartialView("_DeleteCategoryPopup", category);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult DeleteCategory(int? id)
        {
            Category category = categoryRepository.GetCategory(id.Value);
            if (category == null)
            {
                return NotFound();
            }
            productRepository.DeleteByCategory(id.Value);
            categoryRepository.DeleteCategory(id.Value);
            TempData["Delete"] = "Delete Category with the ID <strong>" + id + "</strong> successfully!!";
            return RedirectToAction("Category");
        }
        [Authorize(Roles = "Admin")]
        public ActionResult EditCategoryPopup(int? id)
        {
            Category category = categoryRepository.GetCategory(id.Value);
            if (category == null)
            {
                throw new Exception("Product ID is not found!!!");
            }
            return PartialView("_EditCategoryPopup", category);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditCategory(int id, Category category)
        {
            if (id != category.CategoryId)
            {
                throw new Exception("Category ID is not matched!! Please try again");
            }
            if (ModelState.IsValid)
            {
                categoryRepository.Update(category);
                TempData["Update"] = "Update Category with the ID <strong>" + id + "</strong> successfully!!";
            }
            return RedirectToAction("Category");
        }

        #endregion
    }
}
