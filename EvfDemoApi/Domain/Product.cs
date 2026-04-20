namespace EvfDemoApi.Domain;

public sealed class Product : IEntity
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public DateTimeOffset CreatedUtc { get; set; }

    public DateTimeOffset? UpdatedUtc { get; set; }
}