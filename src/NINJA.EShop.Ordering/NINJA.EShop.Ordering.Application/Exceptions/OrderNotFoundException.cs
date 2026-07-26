using NINJA.EShop.Shared.Exceptions;
namespace NINJA.EShop.Ordering.Application.Exceptions;

public class OrderNotFoundException: NotFoundException
{
    public OrderNotFoundException(Guid id) : base("Order",id)
    {
    }
}