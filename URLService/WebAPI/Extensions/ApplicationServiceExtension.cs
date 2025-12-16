using Application.Background;
using Application.Services;
using Application.Services.Interfaces;

namespace WebAPI.Extensions
{
  public static class ApplicationServiceExtension
  {
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
      // Background
      services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();
      services.AddHostedService<QueuedHostedService>();
      services.AddHostedService<HotKeyPrewarmWorker>();

      services.AddScoped<ISequenceService, SequenceService>();
      services.AddSingleton<ICodeGenerator, CodeGenerator>();
      services.AddScoped<IUrlEntryService, UrlEntryService>();

      return services;
    }
  }
}
