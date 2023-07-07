using BusinessObject;

namespace dStore.Models
{
    public class ProductDetailViewModel
    {
        public Product Product { get; set; }
        public List<Product> RelatedProducts { get; set; }
    }
}
