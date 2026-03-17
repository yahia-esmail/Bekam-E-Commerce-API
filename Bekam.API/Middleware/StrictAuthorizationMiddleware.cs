namespace Bekam.API.Middleware;
public class StrictAuthorizationMiddleware
{
    private readonly RequestDelegate _next;

    public StrictAuthorizationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var hasAuthHeader = context.Request.Headers.ContainsKey("Authorization");

        if (hasAuthHeader && context.User?.Identity?.IsAuthenticated == false)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return;
        }

        await _next(context);
    }
}

