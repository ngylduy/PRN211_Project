using BusinessObject;
using DataAccess.Repository.CategoryRepo;
using DataAccess.Repository.OrderDetailRepo;
using DataAccess.Repository.ProductRepo;
using dStore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

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
        public async Task<IActionResult> Index(string search, decimal? from, decimal? to, int? page, int category = 0)
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
                int pageSize = 8;
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
                return View(product);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View();
            }
        }

        // GET: ProductController/Create
        public ActionResult Create()
        {
            try
            {
                ICategoryRepository categoryRepository = new CategoryRepository();
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

        // POST: ProductController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Product product)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    productRepository.AddProduct(product);
                }
                Product newProduct = productRepository.GetProduct(product.ProductName);
                TempData["Create"] = "Create Product with the ID <strong>" + newProduct.ProductId + "</strong> successfully!!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View(product);
            }
        }

        // GET: ProductController/Edit/5
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int? id)
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

        // POST: ProductController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Product product)
        {
            try
            {
                if (id != product.ProductId)
                {
                    throw new Exception("Product ID is not matched!! Please try again");
                }
                if (ModelState.IsValid)
                {
                    productRepository.Update(product);
                    TempData["Update"] = "Update Product with the ID <strong>" + id + "</strong> successfully!!";
                }
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View(product);
            }
        }

        // GET: ProductController/Delete/5
        public ActionResult Delete(int? id)
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
                return View(product);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View();
            }
        }

        // POST: ProductController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            try
            {
                IOrderDetailRepository orderDetailRepository = new OrderDetailRepository();
                orderDetailRepository.DeleteByProduct(id);
                productRepository.Delete(id);
                TempData["Delete"] = "Delete Product with the ID <strong>" + id + "</strong> successfully!!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View();
            }
        }


    }
}
