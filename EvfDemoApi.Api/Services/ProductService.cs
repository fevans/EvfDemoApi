using EvfDemoApi.Api.Contracts.Products;
using EvfDemoApi.Api.Domain;
using EvfDemoApi.Api.Repositories;

namespace EvfDemoApi.Api.Services;

public sealed class ProductService(IProductRepository repository) : IProductService
{
    public async Task<IReadOnlyCollection<ProductResponse>> GetAllAsync(CancellationToken cancellationToken)
    {
        var products = await repository.GetAllAsync(cancellationToken);
        return products.Select(ToResponse).ToArray();
    }

    public async Task<ProductResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var product = await repository.GetByIdAsync(id, cancellationToken);
        return product is null ? null : ToResponse(product);
    }

    public async Task<ProductResponse> CreateAsync(CreateProductRequest request, CancellationToken cancellationToken)
    {
        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = request.Name.Trim(),
            Description = request.Description?.Trim(),
            Price = request.Price,
            CreatedUtc = DateTimeOffset.UtcNow
        };

        await repository.AddAsync(product, cancellationToken);
        return ToResponse(product);
    }

    public async Task<ProductResponse?> UpdateAsync(Guid id, UpdateProductRequest request, CancellationToken cancellationToken)
    {
        var existing = await repository.GetByIdAsync(id, cancellationToken);
        if (existing is null)
        {
            return null;
        }

        existing.Name = request.Name.Trim();
        existing.Description = request.Description?.Trim();
        existing.Price = request.Price;
        existing.UpdatedUtc = DateTimeOffset.UtcNow;

        var updated = await repository.UpdateAsync(existing, cancellationToken);
        return updated is null ? null : ToResponse(updated);
    }

    public Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken) =>
        repository.DeleteAsync(id, cancellationToken);

    private static ProductResponse ToResponse(Product product) =>
        new(
            product.Id,
            product.Name,
            product.Description,
            product.Price,
            product.CreatedUtc,
            product.UpdatedUtc);
}
