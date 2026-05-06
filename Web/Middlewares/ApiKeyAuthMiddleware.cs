using Repository;

namespace Web.Middlewares;

public class ApiKeyAuthMiddleware
{
    private readonly RequestDelegate _next;

    public ApiKeyAuthMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, ApplicationDbContext dbContext)
    {
        if (!context.Request.Path.StartsWithSegments("/api/external"))
        {
            await _next(context);
            return;
        }

        if (!context.Request.Headers.TryGetValue("X-Api-Key", out var authHeader))
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsJsonAsync(new
            {
                Error = "Api Key is Required"
            });
            return;
        }

        Console.WriteLine(authHeader);

        var client = dbContext.ApiClients.FirstOrDefault(x => x.ApiKey == authHeader.ToString() && x.IsActive);

        if (client == null)
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsJsonAsync(new
            {
                Error = "Api Key is not valid"
            });
            return;
        }

        context.Items["ApiClient"] = client;

        await _next(context);
    }
}