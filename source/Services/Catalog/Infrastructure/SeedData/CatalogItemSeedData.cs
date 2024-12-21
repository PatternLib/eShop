using EShopOnContainers.Catalog.Domain;

namespace EShopOnContainers.Catalog.Infrastructure.SeedData;

public static class CatalogItemSeedData
{
    public static async Task Initialize(CatalogContext context)
    {
        if (context.CatalogItems.Any())
        {
            return;
        }

        List<CatalogItem> catalogItems = new List<CatalogItem>()
        {
            new CatalogItem { CatalogTypeId = 2, CatalogBrandId = 2, AvailableStock = 100, Description = ".NET Bot Black Hoodie",   Name = ".NET Bot Black Hoodie",     Price = 19.50M, PictureFileName = "1.png" },
            new CatalogItem { CatalogTypeId = 1, CatalogBrandId = 2, AvailableStock = 100, Description = ".NET Black & White Mug",  Name = ".NET Black & White Mug",    Price = 08.50M, PictureFileName = "2.png" },
            new CatalogItem { CatalogTypeId = 2, CatalogBrandId = 5, AvailableStock = 100, Description = "Prism White T-Shirt",     Name = "Prism White T-Shirt",       Price = 12.00M, PictureFileName = "3.png" },
            new CatalogItem { CatalogTypeId = 2, CatalogBrandId = 2, AvailableStock = 100, Description = ".NET Foundation T-shirt", Name = ".NET Foundation T-shirt",   Price = 12.00M, PictureFileName = "4.png" },
            new CatalogItem { CatalogTypeId = 3, CatalogBrandId = 5, AvailableStock = 100, Description = "Roslyn Red Sheet",        Name = "Roslyn Red Sheet",          Price = 08.50M, PictureFileName = "5.png" },
            new CatalogItem { CatalogTypeId = 2, CatalogBrandId = 2, AvailableStock = 100, Description = ".NET Blue Hoodie",        Name = ".NET Blue Hoodie",          Price = 12.00M, PictureFileName = "6.png" },
            new CatalogItem { CatalogTypeId = 2, CatalogBrandId = 5, AvailableStock = 100, Description = "Roslyn Red T-Shirt",      Name = "Roslyn Red T-Shirt",        Price = 12.00M, PictureFileName = "7.png" },
            new CatalogItem { CatalogTypeId = 2, CatalogBrandId = 5, AvailableStock = 100, Description = "Kudu Purple Hoodie",      Name = "Kudu Purple Hoodie",        Price = 08.50M, PictureFileName = "8.png" },
            new CatalogItem { CatalogTypeId = 1, CatalogBrandId = 5, AvailableStock = 100, Description = "Cup<T> White Mug",        Name = "Cup<T> White Mug",          Price = 12.00M, PictureFileName = "9.png" },
            new CatalogItem { CatalogTypeId = 3, CatalogBrandId = 2, AvailableStock = 100, Description = ".NET Foundation Sheet",   Name = ".NET Foundation Sheet",     Price = 12.00M, PictureFileName = "10.png" },
            new CatalogItem { CatalogTypeId = 3, CatalogBrandId = 2, AvailableStock = 100, Description = "Cup<T> Sheet",            Name = "Cup<T> Sheet",              Price = 08.50M, PictureFileName = "11.png" },
            new CatalogItem { CatalogTypeId = 2, CatalogBrandId = 5, AvailableStock = 100, Description = "Prism White TShirt",      Name = "Prism White TShirt",        Price = 12.00M, PictureFileName = "12.png" },
        };

        context.CatalogItems.AddRange(catalogItems);
        await context.SaveChangesAsync();
    }
}
