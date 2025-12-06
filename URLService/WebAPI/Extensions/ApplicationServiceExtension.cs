using Application.Services;
using Application.Services.Interfaces;

namespace WebAPI.Extensions
{
  public static class ApplicationServiceExtension
  {
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
      services.AddScoped<ISequenceService, SequenceService>();
      services.AddSingleton<ICodeGenerator, CodeGenerator>();

      return services;
    }
  }
}
