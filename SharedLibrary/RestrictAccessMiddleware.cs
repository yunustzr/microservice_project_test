using Microsoft.AspNetCore.Http;

namespace SharedLibrary
{
    public class RestrictAccessMiddleware
    {
        private readonly RequestDelegate  _next;
        public RestrictAccessMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context) {
            var referrer = context.Request.Headers["Referrer"].FirstOrDefault();
            if (String.IsNullOrEmpty(referrer))
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsync("Bu sayfa bulunamadı.");
                return;
            }
            else
            {
                 await _next(context);
            }
        }

    }
}
