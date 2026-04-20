namespace EvfDemoApi.Contracts.Products;

public sealed record ProductResponse(
    Guid Id,
    string Name,
    string? Description,
    decimal Price,
    DateTimeOffset CreatedUtc,
    DateTimeOffset? UpdatedUtc);
