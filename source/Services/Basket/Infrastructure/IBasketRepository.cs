using EShopOnContainers.Basket.Domain;

namespace EShopOnContainers.Basket.Infrastructure;

public interface IBasketRepository
{
    Task<CustomerBasket> GetBasketAsync(string customerId);
    Task<CustomerBasket> UpdateBasketAsync(CustomerBasket basket);
}
