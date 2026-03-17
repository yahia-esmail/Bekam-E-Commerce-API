using System.Net;
using System.Text.Json;

namespace Bekam.API.Middleware;

public sealed class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(
               ex,
               "Unhandled exception occurred. Path: {Path} Method: {Method}",
               context.Request.Path,
               context.Request.Method
           );

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;

            var message = context.RequestServices
                .GetRequiredService<IHostEnvironment>()
                .IsDevelopment()
                ? ex.Message
                : "Something went wrong";

            var response = new
            {
                success = false,
                error = new
                {
                    code = "INTERNAL_ERROR",
                    message = message
                }
            };

            await context.Response.WriteAsync(
                JsonSerializer.Serialize(response)
            );
        }

    }
}