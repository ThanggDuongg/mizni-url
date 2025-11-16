using Serilog;
using Serilog.Events;

namespace WebAPI.Extensions
{
  public static class SerilogExtension
  {
    public static IHostApplicationBuilder AddSerilogLogging(this WebApplicationBuilder builder)
    {
      builder.Host.UseSerilog(
        (context, services, configuration) =>
          configuration
            .ReadFrom.Configuration(context.Configuration)
            .ReadFrom.Services(services)
            .Enrich.FromLogContext()
            .Enrich.WithProperty("ServiceName", "URLService")
            .Enrich.WithProperty("Version", "1.0.0")
            .Enrich.WithProperty("MachineName", Environment.MachineName)
            .Enrich.WithProperty("Environment", context.HostingEnvironment.EnvironmentName)
      );
      return builder;
    }

    public static IApplicationBuilder UseSerilogRequestLogging(this IApplicationBuilder app)
    {
      app.UseSerilogRequestLogging(options =>
      {
        options.MessageTemplate =
          "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms | CorrelationId: {CorrelationId}";
        options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
        {
          diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
          diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
          diagnosticContext.Set("UserAgent", httpContext.Request.Headers.UserAgent.ToString());

          var remoteIpAddress = httpContext.Connection.RemoteIpAddress;
          if (remoteIpAddress is not null)
          {
            diagnosticContext.Set("ClientIP", remoteIpAddress.ToString());
          }

          if (httpContext.Items.TryGetValue(Common.CORRELATION_ID_ITEM, out var correlationId))
          {
            diagnosticContext.Set(
              Common.CORRELATION_ID_ITEM,
              correlationId?.ToString() ?? string.Empty
            );
          }
        };

        options.GetLevel = (httpContext, elapsed, ex) =>
          ex switch
          {
            not null => LogEventLevel.Error,
            _ when httpContext.Response.StatusCode >= 500 => LogEventLevel.Error,
            _ when httpContext.Response.StatusCode >= 400 => LogEventLevel.Warning,
            _ => LogEventLevel.Information,
          };
      });

      return app;
    }
  }
}
