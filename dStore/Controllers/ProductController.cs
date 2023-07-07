using BusinessObject;
using DataAccess.Repository.ProductRepo;
using dStore.Models;
using Microsoft.AspNetCore.Mvc;

namespace dStore.Controllers
{

    public class ProductController : Controller
    {
        IProductRepository productRepository;

        public ProductController()
        {
            productRepository = new ProductRepository();
        }

        FStoreContext context = new FStoreContext();

        // GET: ProductController
        public async Task<IActionResult> Index(string search, string? sort, decimal? from, decimal? to, int? page, int category = 0)
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
                return View("Index", await PaginatedList<Product>.CreateAsync(products.AsQueryable(), page ?? 1, pageSize));
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View("Index");
            }

        }

        // GET: ProductController/Details/5
        public ActionResult Details(int? id)
        {
            ViewBag.Product = true;
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

                var relatedProductViewModels = productRepository.GetRelatedProduct(id.Value);

                var viewModel = new ProductDetailViewModel
                {
                    Product = product,
                    RelatedProducts = relatedProductViewModels
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View();
            }
        }

    }
}
