using Domain.Entities.Interfaces;
using Microsoft.EntityFrameworkCore;
using MongoDB.EntityFrameworkCore.Extensions;

namespace Infrastructure.Extensions
{
  public static class ModelBuilderExtension
  {
    public static void ConfigureMongoConventions(this ModelBuilder modelBuilder)
    {
      var clrTypes = GetClrTypes(modelBuilder);

      ConfigIdentity(modelBuilder, clrTypes);
      ConfigRowVersion(modelBuilder);
      ConfigAudit(modelBuilder, clrTypes);
      ConfigCollectionNames(modelBuilder, clrTypes);
    }

    public static void ConfigCollectionNames(
      this ModelBuilder modelBuilder,
      Type[]? clrTypes = null
    )
    {
      clrTypes ??= GetClrTypes(modelBuilder);

      var baseTypes = clrTypes.Where(x => !clrTypes.Any(t => x.IsSubclassOf(t))).ToArray();

      foreach (var clrType in baseTypes)
      {
        modelBuilder.Entity(clrType).ToCollection(clrType.Name);
      }
    }

    public static void ConfigAudit(this ModelBuilder modelBuilder, Type[]? clrTypes = null)
    {
      clrTypes ??= GetClrTypes(modelBuilder);

      var appliedOnClrTypes = clrTypes
        .Where(x => typeof(IAuditing).IsAssignableFrom(x))
        .Where(x => !clrTypes.Any(t => x.IsSubclassOf(t) && typeof(IAuditing).IsAssignableFrom(t)))
        .ToArray();

      foreach (var clrType in appliedOnClrTypes)
      {
        var entity = modelBuilder.Entity(clrType);

        entity.Property(nameof(IAuditing.CreatedBy)).IsRequired();
        entity.Property(nameof(IAuditing.CreatedOn)).IsRequired();
        entity.Property(nameof(IAuditing.ModifiedBy)).IsRequired();
        entity.Property(nameof(IAuditing.ModifiedOn)).IsRequired();

        entity.HasIndex(nameof(IAuditing.CreatedOn));
      }
    }

    public static void ConfigIdentity(this ModelBuilder modelBuilder, Type[]? clrTypes = null)
    {
      clrTypes ??= GetClrTypes(modelBuilder);

      var appliedOnClrTypes = clrTypes
        .Where(x => typeof(IIdentity).IsAssignableFrom(x))
        .Where(x => !clrTypes.Any(t => x.IsSubclassOf(t) && typeof(IIdentity).IsAssignableFrom(t)))
        .ToArray();

      foreach (var clrType in appliedOnClrTypes)
      {
        var entity = modelBuilder.Entity(clrType);
        entity.HasKey(nameof(IIdentity.Id));
      }
    }

    public static void ConfigRowVersion(this ModelBuilder modelBuilder)
    {
      var clrTypes = GetClrTypes(modelBuilder);
      var appliedOnClrTypes = clrTypes
        .Where(x => x.IsAssignableTo(typeof(IVersioning)))
        .Where(x => !clrTypes.Any(t => x.IsSubclassOf(t) && t.IsAssignableTo(typeof(IVersioning))))
        .ToArray();
      foreach (var clrType in appliedOnClrTypes)
      {
        modelBuilder.Entity(clrType).Property(nameof(IVersioning.Version)).IsRowVersion();
      }
    }

    private static Type[] GetClrTypes(ModelBuilder builder)
    {
      return
      [
        .. builder
          .Model.GetEntityTypes()
          .Select(e => e.ClrType)
          .Where(t => t is { IsAbstract: false, IsInterface: false })
          .Distinct(),
      ];
    }
  }
}
