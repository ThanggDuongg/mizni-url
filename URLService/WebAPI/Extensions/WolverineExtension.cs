using Application.Behaviors;
using Wolverine;
using Wolverine.FluentValidation;

namespace WebAPI.Extensions
{
  public static class WolverineExtension
  {
    public static IHostApplicationBuilder AddWolverineFx(this IHostApplicationBuilder builder)
    {
      builder.UseWolverine(opts =>
      {
        opts.UseFluentValidation();
        opts.Policies.AddMiddleware(typeof(LoggingBehavior));

        opts.Discovery.IncludeAssembly(typeof(LoggingBehavior).Assembly);
      });

      return builder;
    }
  }
}
