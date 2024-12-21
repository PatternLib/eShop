
using EShopOnContainers.Catalog.Extensions;
using EShopOnContainers.Catalog.Infrastructure;
using EShopOnContainers.Catalog.Infrastructure.SeedData;
using Microsoft.AspNetCore;

namespace EShopOnContainers.Catalog;

public class Program
{
    public static void Main(string[] args)
    {
        BuildWebHost(args)
            .MigrateDbContext<CatalogContext>(seeder: (context, services) =>
            {
                CatalogBrandSeedData.Initialize(context).Wait();
                CatalogTypeSeedData.Initialize(context).Wait();
                CatalogItemSeedData.Initialize(context).Wait();
            })
            .Run();
    }

    public static IWebHost BuildWebHost(string[] args)
    {
        return WebHost.CreateDefaultBuilder(args)
            .UseStartup<Startup>()
            .UseContentRoot(Directory.GetCurrentDirectory())
            .UseWebRoot(webRoot: "Pics") // If the images are stored locally
            .Build();
    }
}
