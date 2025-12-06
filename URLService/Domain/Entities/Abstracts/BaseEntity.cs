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

    [BsonElement(nameof(CreatedBy))]
    public string CreatedBy { get; set; } = default!;

    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    [BsonElement(nameof(CreatedOn))]
    public DateTime CreatedOn { get; set; }

    [BsonElement(nameof(ModifiedBy))]
    public string ModifiedBy { get; set; } = default!;

    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    [BsonElement(nameof(ModifiedOn))]
    public DateTime ModifiedOn { get; set; }

    [BsonElement("_version")]
    public long Version { get; set; }
  }
}
