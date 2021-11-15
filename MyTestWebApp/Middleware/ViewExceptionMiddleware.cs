using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Threading.Tasks;

namespace MyTestWebApp.Middleware
{
    public class ViewExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ViewExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;

            if (exception != null)
            {
                var message = exception.Message;
                var trace = exception.StackTrace;
                var type = exception.GetType().Name;
                await context.Response.WriteAsync(String.Format(
                      "Exception info:\n\n" +
                      "Type={0}\n\n" +
                      "Message={1}\n\n" +
                      "StackTrace={2}", type, message, trace));
            }
            else
            {
                await _next.Invoke(context);
            }
        }
    }
}
