using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;

namespace TheGymWebsite.Middlewares
{
    public class EndpointLoggingMiddleware
    {
		private readonly RequestDelegate next;
		private readonly ILogger<EndpointLoggingMiddleware> logger;

		public EndpointLoggingMiddleware(RequestDelegate next, ILogger<EndpointLoggingMiddleware> logger)
		{
			this.next = next;
			this.logger = logger;
		}

		public Task InvokeAsync(HttpContext context)
		{
			var endpoint = context.GetEndpoint();
			switch (endpoint)
			{
				case RouteEndpoint routeEndpoint:
					logger.LogInformation($"Endpoint Display Name: {routeEndpoint.DisplayName}");
					logger.LogInformation($"Endpoint Pattern: {routeEndpoint.RoutePattern}");
					
					foreach (var type in routeEndpoint.Metadata.Select(md => md.GetType()))
					{
						logger.LogInformation($"{type}");
					}
					break;

				// When no endpoint or when the middleware is endpoint unaware
				// i.e. when placed before app.UseRoute().
				case null:
					logger.LogInformation("Endpoint is null");
					break;
			}

			return next(context);
		}
	}
}
