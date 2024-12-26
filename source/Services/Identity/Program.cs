using EShopOnContainers.Identity.Extensions;
using EShopOnContainers.Identity.Infrastructure;
using EShopOnContainers.Identity.Infrastructure.SeedData;
using Microsoft.AspNetCore;

namespace EShopOnContainers.Identity;

public class Program
{
    public static void Main(string[] args)
    {
        BuildWebHost(args: args)
            .MigrateDbContext<OpenIDContext>(seeder: (context, manager, config) =>
            {
                IdentitySeedData.Initialize(context).Wait();
                OpenIDSeedData.Initialize(manager, config).Wait();
            })
            .Run();
    }

    public static IWebHost BuildWebHost(string[] args)
    {
        return WebHost.CreateDefaultBuilder(args)
            .UseKestrel()
            .UseContentRoot(contentRoot: Directory.GetCurrentDirectory())
            .UseIISIntegration()
            .UseStartup<Startup>()
            .Build();
    }
}
