namespace Domain.Entities.Interfaces
{
  public interface IVersioning
  {
    long RowVersion { get; set; }
  }
}
