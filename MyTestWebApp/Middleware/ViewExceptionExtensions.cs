using Microsoft.AspNetCore.Builder;

namespace MyTestWebApp.Middleware
{
    public static class ViewExceptionExtensions
    {
        public static IApplicationBuilder UseViewException(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ViewExceptionMiddleware>();
        }
    }
}
