using EShopOnContainers.Identity.Infrastructure;
using Microsoft.EntityFrameworkCore;
using OpenIddict.Abstractions;

namespace EShopOnContainers.Identity.Extensions;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddMySqlContext(this IServiceCollection services, IConfiguration Configuration)
    {
        services.AddDbContext<OpenIDContext>(optionsAction: options =>
        {
            options.UseMySQL(
                connectionString: Configuration["ConnectionString"] ??
                    throw new InvalidOperationException(message: $"Connection string not found."),
                mySqlOptionsAction: actions =>
                {
                    // Configuring Connection Resiliency
                    actions.EnableRetryOnFailure(maxRetryCount: 10, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                });
        });

        return services;
    }

    public static IServiceCollection AddOpenIDServerProvider(this IServiceCollection services)
    {
        services.AddOpenIddict()

            // Register the OpenIddict core components.
            .AddCore(options =>
            {
                // Configure OpenIddict to use the Entity Framework Core stores and models.
                // Note: call ReplaceDefaultEntities() to replace the default entities.
                options.UseEntityFrameworkCore()
                       .UseDbContext<OpenIDContext>();
            });

        services.AddOpenIddict()

            // Register the OpenIddict server components.
            .AddServer(options =>
            {
                // Enable the token endpoint.
                options.SetAuthorizationEndpointUris(uris: "/connect/authorize")
                    .SetTokenEndpointUris(uris: "/connect/token")
                    .SetUserinfoEndpointUris(uris: "/connect/userinfo");

                // Enable the client credentials flow.
                options.AllowClientCredentialsFlow()
                    // ERROR: The specified 'response_type' is not supported.
                    .AllowAuthorizationCodeFlow()
                    .RequireProofKeyForCodeExchange()
                    .AllowRefreshTokenFlow();

                // Register scopes (permissions)
                options.RegisterScopes(scopes: new[]
                {
                    OpenIddictConstants.Scopes.OpenId,
                    OpenIddictConstants.Scopes.Profile,
                    OpenIddictConstants.Scopes.OfflineAccess
                });

                // Register the signing and encryption credentials.
                options.AddDevelopmentEncryptionCertificate()
                       .AddDevelopmentSigningCertificate();

                // Register the ASP.NET Core host and configure the ASP.NET Core options.
                options.UseAspNetCore()
                    .EnableTokenEndpointPassthrough()
                    // ERROR: The authorization request was not handled
                    .EnableAuthorizationEndpointPassthrough()
                    // ERROR: Openiddict with dotnet core 5 giving the errors as "this server only accepts HTTPS requests."
                    // InvalidOperationException: IDX20803: Unable to obtain configuration from: /.well-known/openid-configuration.
                    .DisableTransportSecurityRequirement();
            });
        return services;
    }
}
