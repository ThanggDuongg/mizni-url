using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Middlewares
{
  public class AntiforgeryMiddleware(
    RequestDelegate next,
    IAntiforgery antiforgery,
    ILogger<AntiforgeryMiddleware> logger
  )
  {
    private static readonly string[] _excludedPrefixes =
    {
      "/scalar",
      "/health",
      "/grid",
      "/search",
    };

    public async Task InvokeAsync(HttpContext context)
    {
      if (ShouldSkipAntiforgeryValidation(context))
      {
        await next(context);
        return;
      }

      try
      {
        await antiforgery.ValidateRequestAsync(context);
        await next(context);
      }
      catch (AntiforgeryValidationException ex)
      {
        if (logger.IsEnabled(LogLevel.Warning))
        {
          logger.LogWarning(ex, "Invalid CSRF token for path: {Path}", context.Request.Path);
        }
        context.Response.StatusCode = StatusCodes.Status400BadRequest;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync("Invalid CSRF token.");
      }
    }

    private static bool ShouldSkipAntiforgeryValidation(HttpContext context)
    {
      var path = context.Request.Path.Value ?? string.Empty;

      if (EnvironmentsHelper.IsDevelopment())
      {
        return true;
      }

      if (
        HttpMethods.IsGet(context.Request.Method)
        || HttpMethods.IsHead(context.Request.Method)
        || HttpMethods.IsOptions(context.Request.Method)
      )
      {
        return true;
      }

      if (
        _excludedPrefixes.Any(prefix => path.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
      )
      {
        return true;
      }

      var endpoint = context.Features.Get<IEndpointFeature>()?.Endpoint;
      if (endpoint?.Metadata.OfType<IgnoreAntiforgeryTokenAttribute>().Any() == true)
      {
        return true;
      }

      return false;
    }
  }
}
