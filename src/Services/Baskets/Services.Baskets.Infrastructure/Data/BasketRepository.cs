using System.Text.Json;
using Services.Baskets.Application.Abstractions.Data;
using Services.Baskets.Domain.Entities;
using StackExchange.Redis;

namespace Services.Baskets.Infrastructure.Data;
public class BasketRepository(IConnectionMultiplexer redisMul) : IBasketRepository
{  
    private readonly IDatabase _database = redisMul.GetDatabase();
    public Task<IReadOnlyList<Basket>> GetAllBasketsAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<Basket> GetBasketByIdAsync(string basketId)
    {
        RedisValue data = await _database.StringGetAsync(basketId);
        #pragma warning disable CS8604 // Possible null reference argument.
        return data.IsNullOrEmpty ? null : JsonSerializer.Deserialize<Basket>(data);
        #pragma warning restore CS8604 // Possible null reference argument.
    }

    public async Task<Basket> UpdateBasketAsync(Basket basket)
    {
        bool created = await _database.StringSetAsync(basket.Id, JsonSerializer.Serialize(basket), TimeSpan.FromHours(5));

        if (!created)
        {
            return null;
        }

        return await GetBasketByIdAsync(basket.Id);
    }
    public async Task<bool> DeleteBasketAsync(string basketId)
    {
        return await _database.KeyDeleteAsync(basketId);
    }

}
