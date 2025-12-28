using Domain.Entities.Abstracts;
using Domain.Entities.Interfaces;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.Entities
{
  public class UrlEntry : BaseEntity, ISoftDelete
  {
    [BsonElement(nameof(OriginalUrl))]
    public string OriginalUrl { get; set; } = default!;

    [BsonElement(nameof(Code))]
    public string Code { get; set; } = default!;

    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    [BsonElement(nameof(ExpiredAt))]
    public DateTime? ExpiredAt { get; set; }

    [BsonElement(nameof(IsDeleted))]
    public bool IsDeleted { get; set; }

    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    [BsonElement(nameof(MarkedDeletedAt))]
    public DateTime? MarkedDeletedAt { get; set; }
  }
}
