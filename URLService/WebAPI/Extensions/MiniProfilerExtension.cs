using StackExchange.Profiling.SqlFormatters;
using StackExchange.Profiling.Storage;

namespace WebAPI.Extensions
{
  public static class MiniProfilerExtension
  {
    public static IServiceCollection AddMiniProfilerSupport(this IServiceCollection services)
    {
      services
        .AddMiniProfiler(options =>
        {
          options.RouteBasePath = Profiler.ROUTE;
          options.TrackConnectionOpenClose = true;
          options.EnableServerTimingHeader = true;
          options.EnableMvcViewProfiling = true;

          if (options.Storage is MemoryCacheStorage memoryCacheStorage)
          {
            memoryCacheStorage.CacheDuration = TimeSpan.FromMinutes(60);
          }
          options.SqlFormatter = new InlineFormatter();
          options.ColorScheme = StackExchange.Profiling.ColorScheme.Auto;
        })
        .AddEntityFramework();
      return services;
    }
  }
}
