using EShopOnContainers.Catalog.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EShopOnContainers.Catalog.Infrastructure.EntityConfigurations
{
    public class CatalogBrandEntityTypeConfiguration
        : IEntityTypeConfiguration<CatalogBrand>
    {
        public void Configure(EntityTypeBuilder<CatalogBrand> builder)
        {
            builder.ToTable(name: "CatalogBrand");

            builder.HasKey(keyExpression: x => x.Id);

            builder.Property(propertyExpression: x => x.Id)
                .ValueGeneratedOnAdd();

            builder.Property(propertyExpression: x => x.Brand)
                .IsRequired(required: true)
                .HasMaxLength(maxLength: 100);
        }
    }
}
