namespace Application.Features.Example.GetWeathers
{
  public sealed record WeatherDataDto(int Id, DateTime Date, int TemperatureC, string Summary);
}
