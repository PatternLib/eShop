using Microsoft.EntityFrameworkCore;

namespace EShopOnContainers.Catalog.Extensions;

public static class WebHostExtensions
{
    public static IWebHost MigrateDbContext<TContext>(this IWebHost webHost, Action<TContext,IServiceProvider> seeder)
        where TContext : DbContext
    {
        using(var scope = webHost.Services.CreateScope())
        {
            var services = scope.ServiceProvider;

            var logger = services.GetRequiredService<ILogger<TContext>>();

            var context = services.GetRequiredService<TContext>();

            try
            {
                logger.LogInformation(message: $"Migrating database associated with context {typeof(TContext).Name}");

                // If the MySQL server container is not creted on run docker-compose this
                // migration can't fail for network related exception.
                context.Database.Migrate();
                seeder(context, services);
            }
            catch (Exception ex)
            {
                logger.LogError(exception: ex, message: $"An error ocurred while migrating the database used on context {typeof(TContext).Name}");
            }
        }

        return webHost;
    }
}
