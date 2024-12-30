using EShopOnContainers.WebMvc.Extensions;
using EShopOnContainers.WebMvc.Models;
using EShopOnContainers.WebMvc.Services;

namespace EShopOnContainers.WebMvc;

public class Startup
{
    public IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        // Add services to the container.
        services.AddMvc(setupAction: options => options.EnableEndpointRouting = false);
        services.AddOIDCAuthExtensions(Configuration);
        services.Configure<AppSettingsJson>(config: Configuration);

        services.AddTransient<IIdentityParser<ApplicationUser>, IdentityParser>();

        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.AddTransient<HttpClientAuthorizationDelegatingHandler>();

        services.AddHttpClient<ICatalogServices, CatalogServices>();
        services.AddHttpClient<IBasketServices, BasketServices>()
            .AddHttpMessageHandler<HttpClientAuthorizationDelegatingHandler>();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        // Configure the HTTP request pipeline.
        if (!env.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseStaticFiles();

        app.UseAuthentication();

        app.UseMvc(configureRoutes: routes =>
        {
            routes.MapRoute(
                name: "default",
                template: "{controller=Catalog}/{action=Index}/{id?}");
        });
    }
}
