namespace NINJA.EShop.Basket.API.Models;
// Aggregate root
public class ShoppingCart
{
    public string UserName { get; set; } = default!;
    public decimal TotalPrice => Items.Sum(i => i.Price * i.Quantity);
    public ICollection<ShoppingCartItem> Items { get; set; } = new List<ShoppingCartItem>();
    public ShoppingCart(string userName)
    {
        UserName = userName;
    }
    public ShoppingCart()
    {
    }
}