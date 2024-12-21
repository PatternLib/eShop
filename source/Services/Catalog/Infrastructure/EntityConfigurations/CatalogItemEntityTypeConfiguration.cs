using EShopOnContainers.Catalog.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EShopOnContainers.Catalog.Infrastructure.EntityConfigurations;

public class CatalogItemEntityTypeConfiguration
    : IEntityTypeConfiguration<CatalogItem>
{
    public void Configure(EntityTypeBuilder<CatalogItem> builder)
    {
        builder.ToTable(name: "Catalog");

        builder.HasKey(keyExpression: x => x.Id);

        builder.Property(propertyExpression: x => x.Id)
            .ValueGeneratedOnAdd();

        builder.Property(propertyExpression: x => x.Name)
            .IsRequired(required: true)
            .HasMaxLength(maxLength: 50);

        builder.Property(propertyExpression: x => x.Price)
            .IsRequired(required: true);

        builder.Property(propertyExpression: x => x.PictureFileName)
            .IsRequired(required: false);

        builder.Ignore(propertyExpression: x => x.PictureUri);

        builder.HasOne(navigationExpression: catalogItem => catalogItem.CatalogBrand)
            .WithMany()
            .HasForeignKey(foreignKeyExpression: catalogItem => catalogItem.CatalogBrandId);

        builder.HasOne(navigationExpression: catalogItem => catalogItem.CatalogType)
            .WithMany()
            .HasForeignKey(foreignKeyExpression: catalogItem => catalogItem.CatalogTypeId);
    }
}
