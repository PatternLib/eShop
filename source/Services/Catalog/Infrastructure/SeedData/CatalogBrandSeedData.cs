using EShopOnContainers.Catalog.Domain;

namespace EShopOnContainers.Catalog.Infrastructure.SeedData
{
    public static class CatalogBrandSeedData
    {
        public static async Task Initialize(CatalogContext context)
        {
            if(context.CatalogBrands.Any())
            {
                return;
            }

            List<CatalogBrand> catalogBrands = new List<CatalogBrand>()
            {
                new CatalogBrand() { Brand = "Azure"},
                new CatalogBrand() { Brand = ".NET" },
                new CatalogBrand() { Brand = "Visual Studio" },
                new CatalogBrand() { Brand = "SQL Server" },
                new CatalogBrand() { Brand = "Other" }
            };

            context.CatalogBrands.AddRange(catalogBrands);
            await context.SaveChangesAsync();
        }
    }
}
