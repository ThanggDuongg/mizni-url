namespace Contracts.Example.GetWeathers
{
  public sealed record GetWeathersDataDto(int Id, DateTime Date, int TemperatureC, string Summary);
}
