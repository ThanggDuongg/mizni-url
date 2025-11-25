using Domain.Entities.Interfaces;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.Entities.Abstracts
{
  [BsonIgnoreExtraElements]
  public class BaseEntity : IIdentity, IAuditing, IVersioning
  {
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public ObjectId Id { get; set; }

    public string CreatedBy { get; set; } = default!;

    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime CreatedOn { get; set; }

    public string ModifiedBy { get; set; } = default!;

    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime ModifiedOn { get; set; }

    public long Version { get; set; }
  }
}
