using EvfDemoApi.Api.Endpoints;
using EvfDemoApi.Api.Infrastructure.ErrorHandling;
using EvfDemoApi.Api.Repositories;
using EvfDemoApi.Api.Services;
using Microsoft.AspNetCore.HttpOverrides;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddProblemDetails();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new()
    {
        Title = "Evf Demo API",
        Version = "v1",
        Description = "A demo-ready ASP.NET Core Minimal API using an in-memory repository behind interfaces."
    });
});
builder.Services.AddHealthChecks();
builder.Services.AddSingleton<IProductRepository, InMemoryProductRepository>();
builder.Services.AddScoped<IProductService, ProductService>();

var app = builder.Build();

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

app.UseGlobalExceptionHandler();

if (app.Environment.IsDevelopment() || app.Configuration.GetValue<bool>("Swagger:Enabled"))
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.DocumentTitle = "Evf Demo API";
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Evf Demo API v1");
    });
}

app.MapGet("/", () => TypedResults.Ok(new
{
    name = "EvfDemoApi",
    version = "v1",
    docs = "/swagger",
    health = "/health"
}))
.WithName("GetApiInfo")
.WithSummary("Gets API metadata.")
.WithDescription("Returns basic metadata for the running API.");

app.MapHealthChecks("/health")
    .WithTags("Health");

app.MapProductEndpoints();

app.Run();

public partial class Program;
