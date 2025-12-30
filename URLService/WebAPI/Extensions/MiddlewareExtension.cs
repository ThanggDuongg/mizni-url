using Microsoft.AspNetCore.HttpOverrides;
using WebAPI.Middlewares;

namespace WebAPI.Extensions
{
  public static class MiddlewareExtension
  {
    public static IApplicationBuilder UseApplicationMiddlewares(
      this IApplicationBuilder app,
      IWebHostEnvironment env
    )
    {
      app.UseMiddleware<CorrelationIdMiddleware>()
        .UseForwardedHeaders(
          new ForwardedHeadersOptions
          {
            ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto,
            KnownIPNetworks = { },
            KnownProxies = { },
          }
        )
        .UseSecurityHeadersMiddleware()
        .UseMiddleware<ExceptionMiddleware>()
        .UseSerilogRequestLogging()
        .UseHttpsRedirection()
        .UseCors()
        .UseAuthentication()
        .UseRateLimiter()
        .UseMiddleware<AntiforgeryMiddleware>()
        .UseAuthorization()
        .UseHangfire()
        .UseMiniProfiler()
        .UseMiniLog(env.ContentRootPath)
        .UseCacheControlHeaderMiddleware();

      return app;
    }
  }
}
