using OpenIddict.Abstractions;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace EShopOnContainers.Identity.Infrastructure.SeedData;

public static class OpenIDSeedData
{
    public static async Task Initialize(IOpenIddictApplicationManager manager, IConfiguration configuration)
    {
        if (await manager.CountAsync() > 0)
        {
            return ;
        }

        //callbacks urls from config:
        var clientsUrl = new Dictionary<string, string>()
        {
            { "Mvc", configuration["MvcClient"]! },
        };

        var clientApps = new OpenIddictApplicationDescriptor[]
        {
            // Confidential Client
            new OpenIddictApplicationDescriptor
            {
                ClientId = "webmvc",
                DisplayName = "Web MVC Client",
                ApplicationType = ApplicationTypes.Web,
                ClientType = ClientTypes.Confidential,
                ClientSecret = "secret", // Confidential apps require a secret
                
                /// Both the sign-in and sign-out paths MUST be registered as redirect URIs.
                /// * The default values are /signin-oidc and /signout-callback-oidc.
                /// * When using the ASP.NET Core OpenID Connect middleware,
                /// * the /signin-oidc endpoint is automatically handled for you.
                /// * You don’t need to write custom logic unless you want to extend or customize the behavior.
                RedirectUris =
                {
                    new Uri($"{clientsUrl["Mvc"]}/signin-oidc")
                },
                PostLogoutRedirectUris =
                {
                    new Uri($"{clientsUrl["Mvc"]}/signout-callback-oidc")
                },
                Permissions =
                {
                    Permissions.ResponseTypes.Code,

                    Permissions.Endpoints.Authorization,
                    Permissions.Endpoints.Token,
                    Permissions.GrantTypes.AuthorizationCode, // Autentica al usuario
                    Permissions.GrantTypes.ClientCredentials, // Autentica la applicacion (cliente)

                    Permissions.GrantTypes.RefreshToken,
                    Permissions.Prefixes.Scope + Scopes.OfflineAccess,

                    Permissions.Prefixes.Scope + Scopes.OpenId, // Requerido para generar ID token, access_token
                    Permissions.Prefixes.Scope + Scopes.Profile, // Ensure 'profile' scope is listed
                    //Permissions.Prefixes.Scope + "orders", // Recursos a los que puede acceder
                    //Permissions.Prefixes.Scope + "basket",
                    //Permissions.Prefixes.Scope + "locations",
                    //Permissions.Prefixes.Scope + "marketing",
                    //Permissions.Prefixes.Scope + "webshoppingagg",
                    //Permissions.Prefixes.Scope + "orders.signalrhub"
                }
            }
        };

        foreach(var client in clientApps)
        {
            await manager.CreateAsync(descriptor:  client);
        }
    }
}
