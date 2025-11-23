namespace Domain.Entities.Interfaces
{
  public interface IAuditing
  {
    string CreatedBy { get; set; }

    DateTime CreatedOn { get; set; }

    string ModifiedBy { get; set; }

    DateTime ModifiedOn { get; set; }
  }
}
