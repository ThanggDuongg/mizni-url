using Microsoft.Extensions.FileProviders;

namespace WebAPI.Extensions
{
  public static class MiniLogExtension
  {
    public static IApplicationBuilder UseMiniLog(
      this IApplicationBuilder applicationBuilder,
      string rootUrl
    )
    {
      var path = Path.Combine(rootUrl, "logs");
      Directory.CreateDirectory(path);
      applicationBuilder.UseFileServer(
        new FileServerOptions
        {
          FileProvider = new PhysicalFileProvider(path),
          RequestPath = "/logs",
          EnableDirectoryBrowsing = true,
          StaticFileOptions =
          {
            FileProvider = new PhysicalFileProvider(path),
            ServeUnknownFileTypes = true,
            OnPrepareResponse = (context) =>
            {
              context.Context.Response.Headers.CacheControl = "no-cache, no-store";
              context.Context.Response.Headers.Pragma = "no-cache";
              context.Context.Response.Headers.Expires = "-1";
            },
          },
        }
      );

      return applicationBuilder;
    }
  }
}
