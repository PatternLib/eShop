using EShopOnContainers.Catalog.Domain;
using EShopOnContainers.Catalog.Infrastructure.EntityConfigurations;
using Microsoft.EntityFrameworkCore;

namespace EShopOnContainers.Catalog.Infrastructure;

public class CatalogContext : DbContext
{
    public DbSet<CatalogBrand> CatalogBrands { get; set; }
    public DbSet<CatalogType> CatalogTypes { get; set; }
    public DbSet<CatalogItem> CatalogItems { get; set; }

    public CatalogContext(DbContextOptions<CatalogContext> options) : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new CatalogBrandEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new CatalogTypeEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new CatalogItemEntityTypeConfiguration());
    }
}
