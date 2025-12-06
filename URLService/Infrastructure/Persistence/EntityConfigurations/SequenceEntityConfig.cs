using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.EntityConfigurations
{
  public class SequenceEntityConfig : IEntityTypeConfiguration<Sequence>
  {
    public void Configure(EntityTypeBuilder<Sequence> builder)
    {
      builder.Property(x => x.Name).IsRequired();
      builder.HasIndex(x => x.Name).IsUnique();
    }
  }
}
