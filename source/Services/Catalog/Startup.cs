using EShopOnContainers.Catalog.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

namespace EShopOnContainers.Catalog;

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
        services.AddMvc(setupAction: options => options.EnableEndpointRouting = false)
            .AddControllersAsServices();

        services.Configure<AppSettingsJson>(config: Configuration);

        services.AddDbContext<CatalogContext>(optionsAction: options => {
            options.UseMySQL(
                connectionString: Configuration["ConnectionString"]!,
                mySqlOptionsAction: sqlOptions =>
                {
                    // Configuring Connection Resilency
                    sqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 10,
                        maxRetryDelay: TimeSpan.FromSeconds(30),
                        errorNumbersToAdd: null);
                });
        });

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(setupAction: options =>
        {
            options.SwaggerDoc(name: "v1", info: new OpenApiInfo
            {
                Title = "EShopOnContainners - Catalog HTTP API",
                Version = "v1",
                Description = "The Catalog Microservices HTTP API. This is a Data-Drive/CRUD microservice sample",
                // TermsOfService = "Terms Of Services"
            });
        });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        // Configure the HTTP request pipeline.
        if (env.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseMvcWithDefaultRoute();
    }
}
