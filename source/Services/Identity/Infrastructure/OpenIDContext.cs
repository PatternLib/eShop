using EShopOnContainers.Identity.Domain;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EShopOnContainers.Identity.Infrastructure;

public class OpenIDContext : IdentityDbContext<ApplicationUser>
{
    public OpenIDContext(DbContextOptions<OpenIDContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.UseOpenIddict();
    }
}
