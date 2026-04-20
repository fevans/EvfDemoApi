using System.ComponentModel.DataAnnotations;

namespace EvfDemoApi.Contracts.Products;

public sealed class UpdateProductRequest
{
    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string Name { get; init; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; init; }

    [Range(typeof(decimal), "0.01", "1000000")]
    public decimal Price { get; init; }
}
