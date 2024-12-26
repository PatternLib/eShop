using EShopOnContainers.Identity.Domain;
using EShopOnContainers.Identity.Extensions;
using EShopOnContainers.Identity.Infrastructure;
using EShopOnContainers.Identity.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace EShopOnContainers.Identity;

public class Startup
{
    public IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        // Add services to the container
        services.AddMvc(setupAction: options => options.EnableEndpointRouting = false);
        services.AddMySqlContext(Configuration);
        services.AddIdentity<ApplicationUser, IdentityRole>(setupAction: o => o.SignIn.RequireConfirmedAccount = true)
            .AddEntityFrameworkStores<OpenIDContext>()
            .AddDefaultTokenProviders();
        services.AddTransient<IEmailSender, EmailSender>();
        services.AddOpenIDServerProvider();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler(errorHandlingPath: "/Error");
            app.UseHsts();
        }

        app.UseStaticFiles();
        app.UseForwardedHeaders();
        /// Make sure the ASP.NET Core authentication middleware is correctly registered
        /// * No web page found for web address: /.well-known/openid-configuration
        /// * InvalidOperationException: IDX20803: Unable to obtain configuration from: /.well-known/openid-configuration.
        app.UseAuthentication();
        app.UseAuthorization();

        app.UseMvc(configureRoutes: routes =>
        {
            routes.MapRoute(
                name: "default",
                template: "{controller=Account}/{Action=Login}/{returnUrl?}");
        });
    }
}
