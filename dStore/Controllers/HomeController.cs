using BusinessObject;
using DataAccess.Repository.ProductRepo;
using dStore.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace dStore.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        IProductRepository productRepository = new ProductRepository();
        FStoreContext context = new FStoreContext();

        public async Task<IActionResult> Index(string search, decimal? from, decimal? to, int? page, int category = 0)
        {
            ViewBag.Home = true;
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
                int pageSize = 8;
                return View("Index", await PaginatedList<Product>.CreateAsync(products.AsQueryable(), page ?? 1, pageSize));
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View("Index");
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}