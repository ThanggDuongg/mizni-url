namespace Domain.Entities.Interfaces
{
  public interface ISoftDelete
  {
    bool IsDeleted { get; set; }

    DateTime? MarkedDeletedAt { get; set; }
  }
}
