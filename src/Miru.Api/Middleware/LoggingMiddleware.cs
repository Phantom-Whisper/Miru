namespace Miru.Api.Middleware;

public class LoggingMiddleware(RequestDelegate next, ILogger<LoggingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext httpContext)
    {
        logger.LogInformation("Incoming request: {Method} {Path}", httpContext.Request.Method, httpContext.Request.Path);
        
        await next(httpContext);
        
        logger.LogInformation("Response status: {StatusCode}", httpContext.Response.StatusCode);
    }
    
    
}