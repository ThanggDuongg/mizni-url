namespace WebAPI.Extensions
{
  public static class CorsExtension
  {
    public static IServiceCollection AddCorsPolicy(this IServiceCollection services)
    {
      services.AddCors(options =>
      {
        options.AddDefaultPolicy(policy =>
        {
          policy
            .WithOrigins("TBD")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()
            .WithExposedHeaders("Content-Disposition");
        });
      });

      return services;
    }
  }
}
