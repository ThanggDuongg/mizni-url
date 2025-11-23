namespace Shared.Constants
{
  public static class AppConstants
  {
    public struct Common
    {
      public const string API_VERSION = "v1";
      public const string CORRELATION_ID_HEADER = "X-Correlation-ID";
      public const string CORRELATION_ID_ITEM = "CorrelationId";
    }

    public struct Auth
    {
      public const string ANTIFORGERY_TOKEN_HEADER = "X-XSRF-TOKEN";
      public const string ANTIFORGERY_COOKIE_NAME = "XSRF-TOKEN";
    }

    public struct ApiDocument
    {
      public const string VERSION = "v1";
      public const string TITLE = "MIZNI URL";
      public const string ROUTE_PREFIX = "swagger";
      public const string ENDPOINT_URL = "/swagger/v1/swagger.json";
      public const string ENDPOINT_NAME = "MIZNI v1";
    }

    public struct Profiler
    {
      public const string ROUTE = "/profiler";
    }

    public struct MessageTemplate
    {
      public const string NOT_FOUND = "Entity type={0}, id={1} is not found";
    }
  }
}
