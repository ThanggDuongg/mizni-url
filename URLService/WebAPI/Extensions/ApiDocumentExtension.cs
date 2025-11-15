using Microsoft.OpenApi.Models;

namespace WebAPI.Extensions
{
  public static class ApiDocumentExtension
  {
    public static IServiceCollection AddApiDocumentSupport(this IServiceCollection services)
    {
      services.AddSwaggerGen(options =>
      {
        options.SwaggerDoc(
          ApiDocument.VERSION,
          new OpenApiInfo { Title = ApiDocument.TITLE, Version = ApiDocument.VERSION }
        );
      });

      return services;
    }

    public static IApplicationBuilder UseApiDocumentSupport(
      this IApplicationBuilder app,
      IWebHostEnvironment env
    )
    {
      if (env.IsDevelopment())
      {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
          options.RoutePrefix = ApiDocument.ROUTE_PREFIX;
          options.SwaggerEndpoint(ApiDocument.ENDPOINT_URL, ApiDocument.ENDPOINT_NAME);
        });
      }

      return app;
    }
  }
}
