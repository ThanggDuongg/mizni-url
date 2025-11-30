namespace Application.Features.Example.GetWeathers
{
  public class WeatherEntity
  {
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public int TemperatureC { get; set; }
    public string Summary { get; set; } = "";
  }

  public sealed record GetWeathersQuery(int Days);

  public static class GetWeathersQueryHandler
  {
    public static async Task<IEnumerable<WeatherDataDto>> Handle(GetWeathersQuery query)
    {
      var weathers = Enumerable
        .Range(1, query.Days)
        .Select(i => new WeatherEntity
        {
          Id = i,
          Date = DateTime.UtcNow.AddDays(i),
          TemperatureC = Random.Shared.Next(-5, 35),
          Summary = "Sunny",
        })
        .ToList();

      var dtoList = weathers.Select(GetWeathersMapping.ToDto).ToList();
      return await Task.FromResult(dtoList);
    }
  }
}
