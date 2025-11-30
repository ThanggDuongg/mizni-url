using Domain.Entities.Abstracts;

namespace Domain.Entities
{
  public class Sequence : BaseEntity
  {
    public string Name { get; set; } = default!;
    public long CurrentValue { get; set; } = 1;
  }
}
