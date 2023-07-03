using BusinessObject;

namespace DataAccess.Repository.CartRepo
{
    public interface ICartRepository
    {
        public Dictionary<int, CartProduct> GetCart();
        public void AddToCart(int productId, int quantity, decimal price);
        public void RemoveFromCart(int productId);
        public void UpdateCart(int productId, int quantity, decimal price);
        public void DeleteCart();
    }
}
