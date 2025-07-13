using QuizPlatform.Core.Common;
using System.Net;
using System.Text.Json;

namespace QuizPlatform.Web.MiddleWares
{
    namespace Talabat.APIs.MiddleWares
    {
        public class ExceptionMiddleWare
        {
            private readonly RequestDelegate _next;
            private readonly ILogger<ExceptionMiddleWare> _logger;
            private readonly IHostEnvironment _env;

            public ExceptionMiddleWare(RequestDelegate next, ILogger<ExceptionMiddleWare> logger, IHostEnvironment env)
            {
                _next = next;
                _logger = logger;
                _env = env;
            }
            public async Task InvokeAsync(HttpContext context)
            {
                try
                {
                    await _next(context); // ينفذ باقي الطلبات
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message);
                    context.Response.StatusCode = 500;

                    var acceptHeader = context.Request.Headers["Accept"].ToString();

                    if (acceptHeader.Contains("application/json"))
                    {
                        context.Response.ContentType = "application/json";
                        var response = _env.IsDevelopment()
                            ? new ApiResponse(500, ex.Message, ex.StackTrace?.ToString())
                            : new ApiResponse(500, "An unexpected error occurred.");

                        var json = JsonSerializer.Serialize(response);
                        await context.Response.WriteAsync(json);
                    }
                    else
                    {
                        context.Response.Redirect("/Home/Error");
                    }
                }
            }

        }
    }
}
