namespace NINJA.EShop.Basket.API.Models;
// Aggregate root
public class ShoppingCart
{
    public string UserName { get; set; } = default!;
    public Guid CustomerId { get; set; }
    public decimal TotalPrice => Items.Sum(i => i.Price * i.Quantity);
    public ICollection<ShoppingCartItem> Items { get; set; } = new List<ShoppingCartItem>();
    public ShoppingCart(string userName,Guid customerId)
    {
        UserName = userName;
        CustomerId = customerId;
    }
    public ShoppingCart()
    {
    }
}