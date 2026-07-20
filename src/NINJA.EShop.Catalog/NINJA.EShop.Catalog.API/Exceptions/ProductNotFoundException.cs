using NINJA.EShop.Shared.Exceptions;
namespace NINJA.EShop.Catalog.API.Exceptions
{
    public class ProductNotFoundException: NotFoundException
    {
        public ProductNotFoundException(Guid Id) : base("Product",Id)
        {
        }
    }
}