using NINJA.EShop.Shared.Exceptions;

namespace NINJA.EShop.Basket.API.Exceptions
{
    public class BasketNotFoundException(string userName): NotFoundException("Basket",userName)
    {
    }
}
