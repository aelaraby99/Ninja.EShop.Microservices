namespace NINJA.EShop.Catalog.API.Exceptions
{
    public class ProductNotFoundException: Exception
    {
        public ProductNotFoundException() : base("Product not found.")
        {
        }
    }
}