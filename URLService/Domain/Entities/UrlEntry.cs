using Domain.Entities.Abstracts;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.Entities
{
  public class UrlEntry : BaseEntity
  {
    [BsonElement(nameof(OriginalUrl))]
    public string OriginalUrl { get; set; } = default!;

    [BsonElement(nameof(Code))]
    public string Code { get; set; } = default!;

    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    [BsonElement(nameof(Expires))]
    public DateTime? Expires { get; set; }
  }
}
