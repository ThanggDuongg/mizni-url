using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Infrastructure.Extensions
{
  public static class PropertyBuilderExtension
  {
    public static PropertyBuilder<ICollection<TEnum>?> HasEnumCollectionConversion<TEnum>(
      this PropertyBuilder<ICollection<TEnum>?> builder
    )
      where TEnum : struct, Enum
    {
      var converter = new ValueConverter<ICollection<TEnum>?, string?>(
        v => v == null ? null : string.Join(",", v.Select(e => e.ToString())),
        v =>
          string.IsNullOrWhiteSpace(v)
            ? null
            : v.Split(',', StringSplitOptions.RemoveEmptyEntries)
              .Select(s => Enum.Parse<TEnum>(s.Trim()))
              .ToList()
      );

      return builder.HasConversion(converter);
    }

    public static PropertyBuilder HasEnumToStringConversation<TEnum>(
      this PropertyBuilder<TEnum> propertyBuilder
    )
      where TEnum : struct, Enum
    {
      return propertyBuilder.HasConversion(new EnumToStringConverter<TEnum>());
    }

    public static PropertyBuilder HasEnumToStringConversation<TEnum>(
      this PropertyBuilder<TEnum?> propertyBuilder
    )
      where TEnum : struct, Enum
    {
      return propertyBuilder.HasConversion(new EnumToStringConverter<TEnum>());
    }
  }
}
