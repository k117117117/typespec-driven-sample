namespace GameServer;

public class GlobalExceptionHandlerMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception exception)
        {
            var (statusCode, message) = exception switch
            {
                ArgumentException ex => (StatusCodes.Status400BadRequest, ex.Message),
                InvalidOperationException ex => (StatusCodes.Status409Conflict, ex.Message),
                _ => (StatusCodes.Status500InternalServerError, "予期しないエラーが発生しました。")
            };

            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsJsonAsync(new { code = statusCode, message });
        }
    }
}
