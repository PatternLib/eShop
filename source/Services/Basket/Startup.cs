using EShopOnContainers.Basket.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace EShopOnContainers.Basket;

public class Startup
{
    public IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddMvc(setupAction: o => o.EnableEndpointRouting = false)
            .AddControllersAsServices();

        /// ASP.NET Core adds default namespaces to some known claims, which might not be required in the app.
        /// Optionally, disable these added namespaces and use the exact claims that the OpenID Connect server created.
        // JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
        // JwtSecurityTokenHandler.DefaultOutboundClaimTypeMap.Clear();

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options => {

                options.Authority = Configuration["IdentityUrl"];

                options.Audience = "basket";

                options.RequireHttpsMetadata = false;

                options.IncludeErrorDetails = true;

                options.MapInboundClaims = false;

                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        Console.WriteLine($"Error en la autenticación: {context.Exception.Message}");
                        return Task.CompletedTask;
                    }
                };
            });

        services.AddOptions();
        services.Configure<AppSettingsJson>(config: Configuration);

        services.AddSingleton<ConnectionMultiplexer>(implementationFactory: serviceProvider =>
        {
            var option = serviceProvider.GetRequiredService<IOptions<AppSettingsJson>>();
            var appSettingsJson = option.Value;

            var configuration = ConfigurationOptions.Parse(
                configuration: appSettingsJson.ConnectionString,
                ignoreUnknown: true);

            configuration.ResolveDns = true;

            return ConnectionMultiplexer.Connect(configuration: configuration);
        });

        services.AddTransient<IBasketRepository, BasketOnRedisRepository>();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        // Configure the HTTP request pipeline.
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler(errorHandlingPath: "/Error");
            app.UseHsts();
        }

        app.UseAuthentication();

        app.UseMvcWithDefaultRoute();
    }
}
