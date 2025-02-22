using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace GatewayApi.Middlewares
{
    // Bu sınıf, bir middleware oluşturur ve HTTP isteklerini işleme hattında (pipeline) bir ara işlem olarak çalışır
    public class IntercaptionMiddleware
    {
        // Middleware'in işleme hattındaki bir sonraki middleware'e geçiş yapması için gerekli RequestDelegate parametresi
        private readonly RequestDelegate _next;

        // Constructor: Middleware, bir sonraki middleware'i almak için parametre olarak RequestDelegate'i alır
        public IntercaptionMiddleware(RequestDelegate next)
        {
            _next = next; // RequestDelegate'i saklar
        }

        // InvokeAsync: Middleware'in ana çalışma fonksiyonu
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                // Gelen isteğin başlıklarına (headers) "Referrer" adlı bir başlık ekler
                // Bu başlık, isteğin API Gateway'den geldiğini belirtmek için kullanılır
                context.Request.Headers["Referrer"] = "Api-Gateway";

                // İşlemi tamamladıktan sonra işleme hattındaki bir sonraki middleware'e geçiş yapar
                await _next(context);
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = 500;
                await context.Response.WriteAsync($"An error occurred: {ex.Message}");
            }
            
        }
    }
}
