namespace Application.Background
{
  public interface IRecurringJob : IBackgroundJob
  {
    JobConfiguration Configuration { get; }
  }
}
