using System.Net;
using System.Net.Http.Json;
using EvfDemoApi.Contracts.Products;
using EvfDemoApi.Tests.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace EvfDemoApi.Tests;

public sealed class ProductApiTests
{
    [Fact]
    public async Task CreateUpdateDeleteProduct_RoundTripsSuccessfully()
    {
        await using var factory = new TestWebApplicationFactory();
        using var client = factory.CreateClient();

        var createRequest = new CreateProductRequest
        {
            Name = "Demo Webcam",
            Description = "A USB webcam for the demo suite.",
            Price = 79.95m
        };

        client.BaseAddress = new Uri("https://evfdemoapi-d0dxcnbxfcfdgmbx.ukwest-01.azurewebsites.net");
        var createResponse = await client.PostAsJsonAsync("/api/products", createRequest);

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var created = await createResponse.Content.ReadFromJsonAsync<ProductResponse>();

        Assert.NotNull(created);
        Assert.False(created.Id == Guid.Empty);
        Assert.Equal(createRequest.Name, created.Name);

        var getResponse = await client.GetAsync($"/api/products/{created.Id}");
        Console.WriteLine($"GET /api/products/{created.Id} - Status: {getResponse.StatusCode}");

        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        var fetched = await getResponse.Content.ReadFromJsonAsync<ProductResponse>();

        Assert.NotNull(fetched);
        Assert.Equal(created.Id, fetched.Id);
        Assert.Equal(createRequest.Price, fetched.Price);

        var updateRequest = new UpdateProductRequest
        {
            Name = "Demo Webcam Pro",
            Description = "An updated product description.",
            Price = 99.95m
        };

        var updateResponse = await client.PutAsJsonAsync($"/api/products/{created.Id}", updateRequest);

        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);

        var updated = await updateResponse.Content.ReadFromJsonAsync<ProductResponse>();

        Assert.NotNull(updated);
        Assert.Equal(updateRequest.Name, updated.Name);
        Assert.Equal(updateRequest.Price, updated.Price);
        Assert.NotNull(updated.UpdatedUtc);

        var deleteResponse = await client.DeleteAsync($"/api/products/{created.Id}");

        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        var missingResponse = await client.GetAsync($"/api/products/{created.Id}");

        Assert.Equal(HttpStatusCode.NotFound, missingResponse.StatusCode);
    }

    [Fact]
    public async Task CreateProduct_ReturnsBadRequest_WhenPayloadIsInvalid()
    {
        await using var factory = new TestWebApplicationFactory();
        using var client = factory.CreateClient();

        var invalidRequest = new CreateProductRequest
        {
            Name = "A",
            Price = 0
        };

        var response = await client.PostAsJsonAsync("/api/products", invalidRequest);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var validationProblem = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();

        Assert.NotNull(validationProblem);
        Assert.Contains("Name", validationProblem.Errors.Keys);
        Assert.Contains("Price", validationProblem.Errors.Keys);
    }

    [Fact]
    public async Task GetProduct_ReturnsNotFound_WhenProductDoesNotExist()
    {
        await using var factory = new TestWebApplicationFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync($"/api/products/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UpdateProduct_ReturnsNotFound_WhenProductDoesNotExist()
    {
        await using var factory = new TestWebApplicationFactory();
        using var client = factory.CreateClient();

        var request = new UpdateProductRequest
        {
            Name = "Unknown Product",
            Description = "Should not exist.",
            Price = 42.00m
        };

        var response = await client.PutAsJsonAsync($"/api/products/{Guid.NewGuid()}", request);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
