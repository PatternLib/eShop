using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace EShopOnContainers.WebMvc.Extensions;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddOIDCAuthExtensions(this IServiceCollection services, IConfiguration Configuration)
    {
        services.AddAuthentication(configureOptions: options =>
        {
            options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
        })
            .AddCookie()
            .AddOpenIdConnect(configureOptions: options =>
            {
                /// Responsible of persisting user's identity after a successful authentication.
                options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;

                /// Setup the OpenID Connect client
                options.ClientId = "webmvc";
                options.ClientSecret = "secret";

                /// Both the sign-in and sign-out paths MUST be registered as redirect URIs.
                /// * The default values are /signin-oidc and /signout-callback-oidc.
                /// * When using the ASP.NET Core OpenID Connect middleware,
                /// * the /signin-oidc endpoint is automatically handled for you.
                /// * You don’t need to write custom logic unless you want to extend or customize the behavior.
                // options.CallbackPath = "/signin-oidc";
                // options.SignedOutCallbackPath = "/signout-callback-oidc";
                // options.RemoteSignOutPath = "/signout-oidc";
                
                options.ResponseType = OpenIdConnectResponseType.Code;

                /// Make sure the ASP.NET Core authentication middleware is correctly registered
                /// * InvalidOperationException: IDX20803: Unable to obtain configuration from: /.well-known/openid-configuration'.
                options.Authority = Configuration["IdentityUrl"];

                /// This property is set to false to reduce the size of the final authentication cookie
                options.SaveTokens = true;
                
                /// InvalidOperationException: The MetadataAddress or Authority must use HTTPS
                /// unless disabled for development by setting RequireHttpsMetadata=false.
                options.RequireHttpsMetadata = false;
                
                /// Mapping claims using OpenID Connect authentication
                /// The profile claims can be returned in the id_token, which is returned after a successful authentication.
                /// * The ASP.NET Core client app only requires the profile scope.
                /// * When using the id_token for claims, no extra claims mapping is required.
                /// * MapInboundClaims MUST be set to false for most OIDC providers, which prevents renaming claims.
                options.MapInboundClaims = false;
                options.TokenValidationParameters.NameClaimType = "name";
                options.TokenValidationParameters.RoleClaimType = "roles";
                
                /// Another way to get the user claims is to use the OpenID Connect User Info API. 
                // options.GetClaimsFromUserInfoEndpoint = true;

                options.Scope.Clear();
                options.Scope.Add("openid");
                options.Scope.Add("profile");
                options.Scope.Add("basket");
            });

        return services;
    }
}
