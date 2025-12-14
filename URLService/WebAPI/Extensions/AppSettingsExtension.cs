namespace WebAPI.Extensions
{
  public static class AppSettingsExtension
  {
    public static IServiceCollection AddAppSettingsConfiguration(
      this IServiceCollection services,
      IConfiguration configuration
    )
    {
      services.Configure<MongoDbSettings>(configuration.GetSection(nameof(MongoDbSettings)));
      services.Configure<CodeGeneratorOptions>(
        configuration.GetSection(nameof(CodeGeneratorOptions))
      );
      services.Configure<CacheTtlSettings>(configuration.GetSection(nameof(CacheTtlSettings)));
      services.Configure<RedisSettings>(configuration.GetSection(nameof(RedisSettings)));
      services.Configure<CachePrewarmSettings>(
        configuration.GetSection(nameof(CachePrewarmSettings))
      );
      return services;
    }
  }
}
