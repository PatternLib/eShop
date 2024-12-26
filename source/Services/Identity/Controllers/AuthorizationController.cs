using System.Security.Claims;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace EShopOnContainers.Identity.Controllers;

public class AuthorizationController : Controller
{
    [HttpGet(template: "~/connect/authorize")]
    [HttpPost(template: "~/connect/authorize")]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> Authorize()
    {
        /** 
         * The GetOpenIddictServerRequest() method is part of the OpenIddict library and
         * is used to access the OIDC protocol details that were sent to the server.
         * * Details included in the request
         *   client_id=00001111-aaaa-2222-bbbb-3333cccc4444
         *   &response_type=code id_token
         *   &redirect_uri=http://localhost
         *   &response_mode=fragment
         *   &scope=openid offline_access
         */
        var request = HttpContext.GetOpenIddictServerRequest() ??
            throw new InvalidOperationException(message: "The OpenID Connect request cannot be retrived.");

        /**
         * The AuthenticateAsync method checks the current HTTP request to determine 
         * if the user is authenticated. It evaluates the authentication information 
         * (e.g., cookies, tokens) provided in the request using a specific authentication scheme.
         */
        var authResult = await HttpContext.AuthenticateAsync(scheme: IdentityConstants.ApplicationScheme);

        if (!authResult.Succeeded)
        {
            /**
             * The endpoint to which the challenge redirects depends 
             * on the specific configuration of the authentication scheme,
             * particularly in the IdentityConstants.ApplicationScheme case, 
             * which is typically associated with ASP.NET Core Identity's application cookie.
             * * The default login endpoint is /Account/Login
             */
            return Challenge(
                authenticationSchemes: IdentityConstants.ApplicationScheme,
                properties: new AuthenticationProperties
                {
                    RedirectUri = Request.PathBase + Request.Path + QueryString.Create(
                        parameters: Request.HasFormContentType
                            ? Request.Form.ToList()
                            : Request.Query.ToList())
                });
        }

        // Create a new claims principal
        var claims = new List<Claim>
            {
            // 'subject' claim which is required
                new Claim(Claims.Subject, authResult.Principal.Identity.Name),
                new Claim(Claims.Name, authResult.Principal.Identity.Name),
                new Claim(Claims.Username, authResult.Principal.Identity.Name),
                new Claim(Claims.Audience, "test"),
        };

        var email = authResult.Principal.Claims.FirstOrDefault(q => q.Type == ClaimTypes.Email);
        if (email is not null)
        {
            claims.Add(new Claim(Claims.Email, email.Value));
        }

        /**
         * Create a new ClaimsIdentity containing the claims that
         * will be used to create an id_token, a token or a code.
         */
        var claimsIdentity = new ClaimsIdentity(claims, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        // Set requested scopes (this is not done automatically)
        claimsPrincipal.SetScopes(request.GetScopes());

        foreach (var claim in claimsPrincipal.Claims)
        {
            claim.SetDestinations(claim.Type switch
            {
                // If the "profile" scope was granted, allow the "name" claim to be
                // added to the access and identity tokens derived from the principal.
                Claims.Name when claimsPrincipal.HasScope(Scopes.Profile) => new[]
                {
                    Destinations.AccessToken,
                    Destinations.IdentityToken
                },
                Claims.Email when claimsPrincipal.HasScope(Scopes.Profile) => new[]
                {
                    Destinations.AccessToken,
                    Destinations.IdentityToken
                },

                // Never add the "secret_value" claim to access or identity tokens.
                // In this case, it will only be added to authorization codes,
                // refresh tokens and user/device codes, that are always encrypted.
                "secret_value" => Array.Empty<string>(),

                // Otherwise, add the claim to the access tokens only.
                _ => new[]
                {
                    Destinations.AccessToken
                }
            });
        }

        // Signing in with the OpenIddict authentiction scheme trigger OpenIddict to issue a code (which can be exchanged for an access token)
        return SignIn(claimsPrincipal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
    }

    [HttpPost(template: "~/connect/token")]
    public async Task<IActionResult> Exchange()
    {
        /** 
         * The GetOpenIddictServerRequest() method is part of the OpenIddict library and
         * is used to access the OIDC protocol details that were sent to the server.
         * * Details included in the request
         *   client_id = 
         *   &client_secret = 
         *   &scope = 
         *   &code = ...
         *   &redirect_uri = 
         *   &grant_type =
         *   &code_verifier =  
         */
        var request = HttpContext.GetOpenIddictServerRequest() ??
            throw new InvalidOperationException(message: "The OpenID Connect request cannot be retrived.");

        ClaimsPrincipal claimsPrincipal;

        // &grant_type = client_credentials // OAuth 2.0 client credentials flow
        if (request.IsClientCredentialsGrantType())
        {
            // Note: the client credentials are automatically validated by OpenIddict:
            // if client_id or client_secret are invalid, this action won't be invoked.

            /**
             * Create a new ClaimsIdentity containing the claims that
             * will be used to create an id_token, a token or a code.
             */
            var identity = new ClaimsIdentity(
                authenticationType: TokenValidationParameters.DefaultAuthenticationType,
                nameType: Claims.Name,
                roleType: Claims.Role);

            // Subject (sub) is a required field, Use the client_id as the subject identifier.
            identity.SetClaim(Claims.Subject, request.ClientId ?? throw new InvalidOperationException());

            claimsPrincipal = new ClaimsPrincipal(identity);

            claimsPrincipal.SetScopes(request.GetScopes());
        }
        // &grant_type = authorization_code // OAuth 2.0 authorization code flow
        else if (request.IsAuthorizationCodeGrantType())
        {
            // Retrive the claims principal stored in the authorization code
            claimsPrincipal = (await HttpContext.AuthenticateAsync(
                scheme: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme))
                .Principal!;
        }
        // &grant_type=refresh_token // Any flow
        else if (request.IsRefreshTokenGrantType())
        {
            // Retrive the claims principal stored in the refresh token
            claimsPrincipal = (await HttpContext.AuthenticateAsync(
                scheme: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme))
                .Principal!;
        }
        else
        {
            throw new InvalidOperationException(message: "The specified grand type is not support.");
        }

        // Returning a SignInResult will ask OpenIddict to issue the appropriate access/identity token.
        return SignIn(principal: claimsPrincipal, authenticationScheme: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
    }

    [Authorize(AuthenticationSchemes = OpenIddictServerAspNetCoreDefaults.AuthenticationScheme)]
    [HttpGet(template: "~/connect/userinfo")]
    public async Task<IActionResult> Userinfo()
    {
        var claimsPrincipal = (await HttpContext.AuthenticateAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme)).Principal;

        return Ok(new
        {
            Sub = claimsPrincipal.GetClaim(Claims.Subject),
            Name = claimsPrincipal.GetClaim(Claims.Subject),
            Occupation = "Developer",
            Age = 31
        });
    }
}
