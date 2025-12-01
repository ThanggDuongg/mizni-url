namespace WebAPI.Extensions
{
  public static class AppSettingsExtension
  {
    public static IServiceCollection AddAppSettingsConfiguration(
      this IServiceCollection services,
      ConfigurationManager configurationManager
    )
    {
      services.Configure<MongoDbSettings>(configurationManager.GetSection(nameof(MongoDbSettings)));
      services.Configure<CodeGeneratorOptions>(
        configurationManager.GetSection(nameof(CodeGeneratorOptions))
      );
      return services;
    }
  }
}
