using Microsoft.EntityFrameworkCore;
using OpenIddict.Abstractions;

namespace EShopOnContainers.Identity.Extensions;

public static class IWebHostExtensions
{
    public static IWebHost MigrateDbContext<TContext>(this IWebHost webHost, Action<TContext, IOpenIddictApplicationManager, IConfiguration> seeder)
        where TContext : DbContext
    {
        using (var scope = webHost.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            var config = services.GetRequiredService<IConfiguration>();
            var context = services.GetRequiredService<TContext>();
            var manager = services.GetRequiredService<IOpenIddictApplicationManager>();

            try
            {
                // Apply any pending migrations
                context.Database.Migrate();

                // Call the seeder to initialize or seed the database with data
                seeder(context, manager, config);

            }
            catch (Exception ex)
            {
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<TContext>>();
                logger.LogError(exception: ex, message: "An error occurred while migrating the database.");
            }
        }
        
        return webHost;
    }
}
