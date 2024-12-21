using EShopOnContainers.Catalog.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EShopOnContainers.Catalog.Infrastructure.EntityConfigurations;

public class CatalogTypeEntityTypeConfiguration
    : IEntityTypeConfiguration<CatalogType>
{
    public void Configure(EntityTypeBuilder<CatalogType> builder)
    {
        builder.ToTable(name: "CatalogType");

        builder.HasKey(keyExpression: x => x.Id);

        builder.Property(propertyExpression: x => x.Id)
            .ValueGeneratedOnAdd();

        builder.Property(propertyExpression: x => x.Type)
            .IsRequired(required: true)
            .HasMaxLength(maxLength: 100);
    }
}
