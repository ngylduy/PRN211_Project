using BusinessObject;
using DataAccess.DAO;

namespace DataAccess.Repository.CategoryRepo
{
    public class CategoryRepository : ICategoryRepository
    {
        public void AddCategory(string categoryName) => CategoryDAO.Instance.AddCategory(categoryName);

        public Category GetCategory(int categoryId)
        {
            return CategoryDAO.Instance.GetCategory(categoryId);
        }

        public Category GetCategory(string categoryName)
        {
            return CategoryDAO.Instance.GetCategory(categoryName);
        }

        public IEnumerable<Category> GetCategoryList()
        {
            return CategoryDAO.Instance.GetCategoryList();
        }
    }
}
