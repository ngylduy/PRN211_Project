using BusinessObject;

namespace DataAccess.DAO
{
    public class CategoryDAO
    {
        // Using singleton Pattern
        private static CategoryDAO instance = null;
        private static object instanceLock = new object();

        public static CategoryDAO Instance
        {
            get
            {
                lock (instanceLock)
                {
                    if (instance == null)
                    {
                        instance = new CategoryDAO();
                    }
                    return instance;
                }
            }
        }

        public IEnumerable<Category> GetCategoryList()
        {
            IEnumerable<Category> categories = null;

            try
            {
                var context = new FStoreContext();
                categories = context.Categories;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return categories;
        }

        public Category GetCategory(int categoryId)
        {
            Category category = null;

            try
            {
                var context = new FStoreContext();
                category = context.Categories.SingleOrDefault(cate => cate.CategoryId == categoryId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return category;
        }

        public Category GetCategory(string categoryName)
        {
            Category category = null;

            try
            {
                var context = new FStoreContext();
                category = context.Categories.SingleOrDefault(cate => cate.CategoryName.Equals(categoryName.Trim()));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return category;
        }

        public void AddCategory(string categoryName)
        {
            try
            {
                if (GetCategory(categoryName) != null)
                {
                    throw new Exception("Category name is existed!!!");
                }
                else
                {
                    var context = new FStoreContext();
                    context.Categories.Add(new Category
                    {
                        CategoryName = categoryName
                    });
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
