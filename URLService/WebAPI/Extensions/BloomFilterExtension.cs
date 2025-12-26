namespace WebAPI.Extensions
{
  public static class BloomFilterExtension
  {
    public static IServiceCollection AddBloomFilter(
      this IServiceCollection services,
      IConfiguration configuration
    )
    {
      var bloomSettings = configuration
        .GetRequiredSection(nameof(BloomFilterSettings))
        .Get<BloomFilterSettings>()!;

      services.AddBloomFilter(setup =>
      {
        setup.UseInMemory(
          bloomSettings.Name,
          options =>
          {
            options.ExpectedElements = bloomSettings.ExpectedElements;
            options.ErrorRate = bloomSettings.ErrorRate;
          }
        );
      });

      return services;
    }
  }
}
