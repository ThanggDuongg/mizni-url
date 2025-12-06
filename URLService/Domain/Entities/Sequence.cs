using Domain.Entities.Abstracts;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.Entities
{
  public class Sequence : BaseEntity
  {
    [BsonElement(nameof(Name))]
    public string Name { get; set; } = default!;

    [BsonElement(nameof(CurrentValue))]
    public long CurrentValue { get; set; } = 1;
  }
}
