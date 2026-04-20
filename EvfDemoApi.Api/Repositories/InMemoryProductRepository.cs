using System.Collections.Concurrent;
using EvfDemoApi.Api.Domain;

namespace EvfDemoApi.Api.Repositories;

public sealed class InMemoryProductRepository : IProductRepository
{
    private readonly ConcurrentDictionary<Guid, Product> _products = new();

    public InMemoryProductRepository()
    {
        var seededProducts = new[]
        {
            new Product
            {
                Id = Guid.Parse("2f968a18-e764-4adb-bd76-102067c1ac6f"),
                Name = "Demo Keyboard",
                Description = "A preloaded sample product.",
                Price = 49.99m,
                CreatedUtc = DateTimeOffset.UtcNow.AddDays(-2)
            },
            new Product
            {
                Id = Guid.Parse("21427c6c-81f5-4f14-af51-1534232055b2"),
                Name = "Demo Headset",
                Description = "Another sample product for list responses.",
                Price = 89.50m,
                CreatedUtc = DateTimeOffset.UtcNow.AddDays(-1)
            }
        };

        foreach (var product in seededProducts)
        {
            _products[product.Id] = product;
        }
    }

    public Task<IReadOnlyCollection<Product>> GetAllAsync(CancellationToken cancellationToken)
    {
        var products = _products.Values
            .OrderBy(product => product.Name)
            .ToArray();

        return Task.FromResult<IReadOnlyCollection<Product>>(products);
    }

    public Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        _products.TryGetValue(id, out var product);
        return Task.FromResult(product);
    }

    public Task<Product> AddAsync(Product product, CancellationToken cancellationToken)
    {
        _products[product.Id] = product;
        return Task.FromResult(product);
    }

    public Task<Product?> UpdateAsync(Product product, CancellationToken cancellationToken)
    {
        if (!_products.ContainsKey(product.Id))
        {
            return Task.FromResult<Product?>(null);
        }

        _products[product.Id] = product;
        return Task.FromResult<Product?>(product);
    }

    public Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var deleted = _products.TryRemove(id, out _);
        return Task.FromResult(deleted);
    }
}
