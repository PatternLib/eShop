using EShopOnContainers.Catalog.Domain;

namespace EShopOnContainers.Catalog.Infrastructure.SeedData;

public static class CatalogTypeSeedData
{
    public static async Task Initialize(CatalogContext context)
    {
        if (context.CatalogTypes.Any())
        {
            return;
        }

        List<CatalogType> catalogTypes = new List<CatalogType>()
        {
            new CatalogType() { Type = "Mug"},
            new CatalogType() { Type = "T-Shirt" },
            new CatalogType() { Type = "Sheet" },
            new CatalogType() { Type = "USB Memory Stick" }
        };

        context.CatalogTypes.AddRange(catalogTypes);
        await context.SaveChangesAsync();
    }
}

