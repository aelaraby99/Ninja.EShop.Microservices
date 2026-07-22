using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace NINJA.EShop.Basket.API.Data
{
    public class CacheBasketRepository
        (IBasketRepository repository,IDistributedCache cache)
        : IBasketRepository
    {
        public async Task<bool> DeleteBasketAsync(string userName,CancellationToken cancellationToken = default)
        {
            var result = await repository.DeleteBasketAsync(userName,cancellationToken);
            if (!result)
                return false;
            await cache.RemoveAsync(userName,cancellationToken);
            return true;
        }

        public async Task<ShoppingCart> GetBasketAsync(string userName,CancellationToken cancellationToken = default)
        {
            var cachedBasket = await cache.GetStringAsync(userName,cancellationToken);
            if (!string.IsNullOrEmpty(cachedBasket))
                return JsonSerializer.Deserialize<ShoppingCart>(cachedBasket)!;

            var basket = await repository.GetBasketAsync(userName,cancellationToken);

            if (basket is null)
                throw new BasketNotFoundException(userName);

            await cache.SetStringAsync(userName,JsonSerializer.Serialize(basket),cancellationToken);
            return basket;
        }

        public async Task<ShoppingCart> StoreBasketAsync(ShoppingCart basket,CancellationToken cancellationToken = default)
        {
            var result = await repository.StoreBasketAsync(basket,cancellationToken);

            if (result is null)
                throw new BadHttpRequestException("Failed to store basket");

            await cache.SetStringAsync(basket.UserName,JsonSerializer.Serialize(basket),cancellationToken);
            return basket;
        }
    }
}