using Scalar.AspNetCore;

namespace WebAPI.Extensions
{
  public static class ApiDocumentExtension
  {
    public static IServiceCollection AddApiDocumentSupport(this IServiceCollection services)
    {
      services.AddOpenApi(options =>
      {
        options.AddDocumentTransformer(
          (document, context, _) =>
          {
            document.Info = new()
            {
              Title = ApiDocument.TITLE,
              Version = ApiDocument.VERSION,
              Description = ApiDocument.DESCRIPTION,
            };
            return Task.CompletedTask;
          }
        );
      });

      return services;
    }

    public static IEndpointRouteBuilder MapApiDocumentSupport(this IEndpointRouteBuilder app)
    {
      app.MapOpenApi();
      app.MapScalarApiReference(options =>
      {
        options
          .WithTitle(ApiDocument.TITLE)
          .WithTheme(ScalarTheme.Default)
          .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
      });

      return app;
    }
  }
}
