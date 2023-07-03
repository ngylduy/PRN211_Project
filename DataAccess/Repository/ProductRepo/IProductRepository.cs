using BusinessObject;

namespace DataAccess.Repository.ProductRepo
{
    public interface IProductRepository
    {
        public IEnumerable<Product> GetProductsList(bool order = false);
        public Product GetProduct(int productId, IEnumerable<Product> searchList = null);
        public Product GetProduct(string productName, IEnumerable<Product> searchList = null);
        public int GetNextProductId();
        public void AddProduct(Product product);
        public void Update(Product product);
        public void Delete(int productId);
        public IEnumerable<Product> SearchProduct(string name, IEnumerable<Product> searchList = null);
        public IEnumerable<Product> SearchProduct(int startUnit, int endUnit, IEnumerable<Product> searchList = null);
        public IEnumerable<Product> SearchProduct(decimal startPrice, decimal endPrice, IEnumerable<Product> searchList = null);
    }
}
