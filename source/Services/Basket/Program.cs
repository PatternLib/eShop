using Microsoft.AspNetCore;

namespace EShopOnContainers.Basket;

public class Program
{
    public static void Main(string[] args)
    {
        BuildWebHost(args: args)
            .Run();
    }

    public static IWebHost BuildWebHost(string[] args)
    {
        return WebHost.CreateDefaultBuilder(args)
            .UseStartup<Startup>()
            .Build();
    }
}
