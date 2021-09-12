using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Server.WebSocketManager
{
    public static class WebSocketExtension
    {
        public static IServiceCollection AddWebSocketManager(this IServiceCollection services)
        {
            services.AddTransient<ConnectionManager>();

            foreach (var item in Assembly.GetEntryAssembly().ExportedTypes)
            {
                if (item.GetTypeInfo().BaseType == typeof(WebSocketHandler))
                {
                    services.AddSingleton(item);
                }
            }

            return services;
        }

        public static IApplicationBuilder MapWebSockets(this IApplicationBuilder app, 
            PathString path, 
            WebSocketHandler webSocketHandler)
        {
            return app.Map(path, (x) => x.UseMiddleware<WebSocketMiddleware>(webSocketHandler));
        }
    }
}
