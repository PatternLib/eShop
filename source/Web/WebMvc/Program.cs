using Microsoft.AspNetCore;

namespace EShopOnContainers.WebMvc;

public class Program
{
    public static void Main(string[] args)
    {
        BuildWebHost(args)
            .Run();
    }

    public static IWebHost BuildWebHost(string[] args)
    {
        return WebHost.CreateDefaultBuilder(args: args)
            .UseStartup<Startup>()
            .Build();
    }
}
