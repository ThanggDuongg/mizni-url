namespace Application.Features.Example.GetWeathers
{
  public static class GetWeathersMapping
  {
    public static GetWeathersDataDto ToDto(WeatherEntity entity) =>
      new(entity.Id, entity.Date, entity.TemperatureC, entity.Summary);

    public static GetWeathersQuery ToQuery(this GetWeathersRequestDto request) => new(request.Days);
  }
}
