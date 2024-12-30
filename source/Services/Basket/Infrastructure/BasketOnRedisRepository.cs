using System.Text.Json;
using EShopOnContainers.Basket.Domain;
using StackExchange.Redis;

namespace EShopOnContainers.Basket.Infrastructure;

public class BasketOnRedisRepository : IBasketRepository
{
    private readonly ILogger<BasketOnRedisRepository> _logger;
    private readonly ConnectionMultiplexer _redis;
    private readonly IDatabase _database;

    public BasketOnRedisRepository(ILogger<BasketOnRedisRepository> logger, ConnectionMultiplexer redis)
    {
        _logger = logger;
        _redis = redis;
        _database = _redis.GetDatabase();
    }

    public async Task<CustomerBasket> GetBasketAsync(string customerId)
    {
        var data = await _database.StringGetAsync(key: customerId);

        if (data.IsNullOrEmpty)
        {
            return null;
        }

        return JsonSerializer.Deserialize<CustomerBasket>(json: data)!;
    }

    public async Task<CustomerBasket> UpdateBasketAsync(CustomerBasket basket)
    {
        var created = await _database.StringSetAsync(
            key: basket.BuyerId,
            value: JsonSerializer.Serialize(value: basket));

        if (!created)
        {
            _logger.LogInformation(message: "Problem occur persisting the item.");

            return null;
        }

        _logger.LogInformation(message: "Basket item persisted succesfully.");

        return await GetBasketAsync(customerId: basket.BuyerId);
    }
}
