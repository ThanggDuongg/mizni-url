using MongoDB.Bson;

namespace Domain.Entities.Interfaces
{
  public interface IIdentity
  {
    ObjectId Id { get; set; }
  }
}
