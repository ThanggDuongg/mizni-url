using Domain.Entities.Abstracts;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.Entities
{
  public class UrlEntry : BaseEntity
  {
    public string OriginalUrl { get; set; } = default!;

    public string Code { get; set; } = default!;

    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime? Expires { get; set; }
  }
}
