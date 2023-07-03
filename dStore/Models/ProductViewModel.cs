using System.ComponentModel.DataAnnotations;

namespace dStore.Models
{
    public class ProductViewModel
    {
        [Display(Name = "Product ID")]
        public int ProductID { get; set; }

        [Display(Name = "Product Name")]
        public string ProductName { get; set; }

        [Display(Name = "Category")]
        public string Category { get; set; }

        [Display(Name = "Weight")]
        public string Weight { get; set; }

        [Display(Name = "Unit Price")]
        public decimal UnitPrice { get; set; }

        [Display(Name = "Units In Stock")]
        public int UnitsInStock { get; set; }
    }
}
