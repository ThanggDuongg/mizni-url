using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.EntityConfigurations
{
  public class UrlEntryEntityConfig : IEntityTypeConfiguration<UrlEntry>
  {
    public void Configure(EntityTypeBuilder<UrlEntry> builder)
    {
      builder.Property(x => x.OriginalUrl).IsRequired();
      builder.Property(x => x.Code).IsRequired();
      builder.HasIndex(x => x.Code).IsUnique();
    }
  }
}
