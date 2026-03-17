using Microsoft.Extensions.Configuration;
using StackExchange.Redis;
using System.Text.Json;
using Bekam.Application.Abstraction.Contracts.Persistence;
using Bekam.Domain.Entities.Cart;

namespace Bekam.Infrastructure.Redis;
internal class CartRepository : ICartRepository
{
    private readonly IDatabase _redis;
    private readonly TimeSpan _ttl;
    public CartRepository(IConnectionMultiplexer redis, IConfiguration configuration)
    {
        _redis = redis.GetDatabase();
        _ttl = TimeSpan.FromDays(double.Parse(configuration["Redis:TimeToLiveInDays"]!));
    }
    public async Task<Cart?> GetAsync(string cartId)
    {
        var data = await _redis.StringGetAsync(cartId);

        if (data.IsNullOrEmpty)
            return null;

        return JsonSerializer.Deserialize<Cart>(data!, _jsonOptions);
    }

    public async Task<Cart?> SaveAsync(Cart cart)
    {
        await _redis.StringSetAsync(
           cart.Id!,
            JsonSerializer.Serialize(cart, _jsonOptions),
            _ttl
        );

        return cart;
    }

    public async Task<bool> DeleteAsync(string cartId)
    {
        return await _redis.KeyDeleteAsync(cartId);
    }

    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };
}
