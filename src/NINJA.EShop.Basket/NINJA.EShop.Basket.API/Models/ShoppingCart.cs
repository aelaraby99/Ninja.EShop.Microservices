namespace NINJA.EShop.Basket.API.Models;

// Aggregate root
public class ShoppingCart
{
    public string UserName { get; set; } = default!;
    public ICollection<ShoppingCartItem> Items { get; set; } = new List<ShoppingCartItem>();
    public decimal TotalPrice => Items.Sum(i => i.Price * i.Quantity);
    public ShoppingCart(string userName)
    {
        UserName = userName;
    }
    public ShoppingCart()
    {
    }
}