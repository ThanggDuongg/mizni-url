using Serilog.Context;

namespace WebAPI.Middlewares
{
  public class CorrelationIdMiddleware(RequestDelegate next)
  {
    public async Task Invoke(HttpContext context)
    {
      var correlationId =
        context.Request.Headers[Common.CORRELATION_ID_HEADER].FirstOrDefault()
        ?? Guid.NewGuid().ToString();

      context.Items[Common.CORRELATION_ID_ITEM] = correlationId;
      context.Response.Headers[Common.CORRELATION_ID_HEADER] = correlationId;

      using (LogContext.PushProperty(Common.CORRELATION_ID_ITEM, correlationId))
      {
        await next(context);
      }
    }
  }
}
