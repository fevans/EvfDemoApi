using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace EvfDemoApi.Api.Infrastructure.ErrorHandling;

public static class GlobalExceptionHandlerExtensions
{
    public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder app)
    {
        app.UseExceptionHandler(errorApp =>
        {
            errorApp.Run(async context =>
            {
                var exceptionFeature = context.Features.Get<IExceptionHandlerFeature>();
                var environment = context.RequestServices.GetRequiredService<IHostEnvironment>();
                var logger = context.RequestServices
                    .GetRequiredService<ILoggerFactory>()
                    .CreateLogger("GlobalExceptionHandler");
                var problemDetailsService = context.RequestServices.GetRequiredService<IProblemDetailsService>();

                if (exceptionFeature?.Error is not null)
                {
                    logger.LogError(
                        exceptionFeature.Error,
                        "Unhandled exception while processing {Method} {Path}.",
                        context.Request.Method,
                        context.Request.Path);
                }

                context.Response.StatusCode = StatusCodes.Status500InternalServerError;

                await problemDetailsService.WriteAsync(new ProblemDetailsContext
                {
                    HttpContext = context,
                    ProblemDetails = new ProblemDetails
                    {
                        Status = StatusCodes.Status500InternalServerError,
                        Title = "An unexpected error occurred.",
                        Detail = environment.IsDevelopment() ? exceptionFeature?.Error.Message : null,
                        Instance = context.Request.Path
                    }
                });
            });
        });

        return app;
    }
}
