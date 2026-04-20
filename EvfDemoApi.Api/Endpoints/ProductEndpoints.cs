using EvfDemoApi.Api.Contracts.Products;
using EvfDemoApi.Api.Infrastructure.Validation;
using EvfDemoApi.Api.Services;

namespace EvfDemoApi.Api.Endpoints;

public static class ProductEndpoints
{
    public static IEndpointRouteBuilder MapProductEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/products")
            .WithTags("Products");

        group.MapGet("/", async (IProductService service, CancellationToken cancellationToken) =>
            TypedResults.Ok(await service.GetAllAsync(cancellationToken)))
            .WithName("GetProducts")
            .WithSummary("Gets all products.")
            .Produces<IReadOnlyCollection<ProductResponse>>(StatusCodes.Status200OK);

        group.MapGet("/{id:guid}", async Task<IResult> (Guid id, IProductService service, CancellationToken cancellationToken) =>
        {
            var product = await service.GetByIdAsync(id, cancellationToken);

            return product is null
                ? TypedResults.NotFound()
                : TypedResults.Ok(product);
        })
        .WithName("GetProductById")
        .WithSummary("Gets a product by id.")
        .Produces<ProductResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);

        group.MapPost("/", async Task<IResult> (CreateProductRequest request, IProductService service, CancellationToken cancellationToken) =>
        {
            var errors = request.Validate();
            if (errors is not null)
            {
                return TypedResults.ValidationProblem(errors);
            }

            var created = await service.CreateAsync(request, cancellationToken);
            return TypedResults.Created($"/api/products/{created.Id}", created);
        })
        .WithName("CreateProduct")
        .WithSummary("Creates a product.")
        .Accepts<CreateProductRequest>("application/json")
        .Produces<ProductResponse>(StatusCodes.Status201Created)
        .ProducesValidationProblem();

        group.MapPut("/{id:guid}", async Task<IResult> (Guid id, UpdateProductRequest request, IProductService service, CancellationToken cancellationToken) =>
        {
            var errors = request.Validate();
            if (errors is not null)
            {
                return TypedResults.ValidationProblem(errors);
            }

            var updated = await service.UpdateAsync(id, request, cancellationToken);

            return updated is null
                ? TypedResults.NotFound()
                : TypedResults.Ok(updated);
        })
        .WithName("UpdateProduct")
        .WithSummary("Updates an existing product.")
        .Accepts<UpdateProductRequest>("application/json")
        .Produces<ProductResponse>(StatusCodes.Status200OK)
        .ProducesValidationProblem()
        .Produces(StatusCodes.Status404NotFound);

        group.MapDelete("/{id:guid}", async Task<IResult> (Guid id, IProductService service, CancellationToken cancellationToken) =>
        {
            var deleted = await service.DeleteAsync(id, cancellationToken);
            return deleted
                ? TypedResults.NoContent()
                : TypedResults.NotFound();
        })
        .WithName("DeleteProduct")
        .WithSummary("Deletes a product.")
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound);

        return app;
    }
}
