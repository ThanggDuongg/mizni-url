namespace Application.Features.Example.GetWeathers
{
  public static class GetWeathersMapping
  {
    public static WeatherDataDto ToDto(WeatherEntity entity) =>
      new(entity.Id, entity.Date, entity.TemperatureC, entity.Summary);

    public static GetWeathersQuery ToQuery(this WeatherRequestDto request) => new(request.Days);
  }
}
