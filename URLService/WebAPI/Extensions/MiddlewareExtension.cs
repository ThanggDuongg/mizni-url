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
        .UseSecurityHeadersMiddleware()
        .UseMiddleware<ExceptionMiddleware>()
        .UseSerilogRequestLogging()
        .UseCors()
        .UseHttpsRedirection()
        .UseAuthentication()
        .UseMiddleware<AntiforgeryMiddleware>()
        .UseAuthorization()
        .UseMiniProfiler()
        .UseMiniLog(env.ContentRootPath)
        .UseApiDocumentSupport(env)
        .UseCacheControlHeaderMiddleware();

      return app;
    }
  }
}
