using System.Security.Claims;
using EShopOnContainers.Identity.Domain;
using Microsoft.AspNetCore.Identity;
using OpenIddict.Abstractions;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace EShopOnContainers.Identity.Extensions;

public static class ClaimsIdentityExtensions
{
    /// <summary>
    /// Agregar claims a la informacion del login del usuario
    /// </summary>
    /// <param name="User"></param>
    /// <param name="_userManager"></param>
    /// <returns></returns>
    public static IEnumerable<Claim> SignInClaimsExtension(this ApplicationUser User, UserManager<ApplicationUser> _userManager)
    {
        var claims = new List<Claim>
            {
                new Claim(type: Claims.Subject, value: User.Id),
                new Claim(type: Claims.PreferredUsername, value: User.UserName),
                new Claim(type: Claims.Username, value: User.UserName),
                new Claim(type: Claims.Name, value: User.UserName),
                new Claim(type: "unique_name", value: User.UserName)
            };

        if (!string.IsNullOrWhiteSpace(User.Name))
            claims.Add(item: new Claim(type: "first_name", value: User.Name));

        if (!string.IsNullOrWhiteSpace(User.LastName))
            claims.Add(item: new Claim(type: "last_name", value: User.LastName));

        if (!string.IsNullOrWhiteSpace(User.CardNumber))
            claims.Add(item: new Claim(type: "card_number", value: User.CardNumber));

        if (!string.IsNullOrWhiteSpace(User.CardHolderName))
            claims.Add(item: new Claim(type: "card_holder", value: User.CardHolderName));

        if (!string.IsNullOrWhiteSpace(User.SecurityNumber))
            claims.Add(item: new Claim(type: "card_security_number", value: User.SecurityNumber));

        if (!string.IsNullOrWhiteSpace(User.Expiration))
            claims.Add(item: new Claim(type: "card_expiration", value: User.Expiration));

        if (!string.IsNullOrWhiteSpace(User.City))
            claims.Add(item: new Claim(type: "address_city", value: User.City));

        if (!string.IsNullOrWhiteSpace(User.Country))
            claims.Add(item: new Claim(type: "address_country", value: User.Country));

        if (!string.IsNullOrWhiteSpace(User.State))
            claims.Add(item: new Claim(type: "address_state", value: User.State));

        if (!string.IsNullOrWhiteSpace(User.Street))
            claims.Add(item: new Claim(type: "address_street", value: User.Street));

        if (!string.IsNullOrWhiteSpace(User.ZipCode))
            claims.Add(item: new Claim(type: "address_zip_code", value: User.ZipCode));

        if (_userManager.SupportsUserEmail)
        {
            claims.AddRange(new[]
            {
                    new Claim(type: Claims.Email, value: User.Email),
                    new Claim(type: Claims.EmailVerified, value: User.EmailConfirmed ? "true" : "false", valueType: ClaimValueTypes.Boolean)
            });
        }

        if (_userManager.SupportsUserPhoneNumber && !string.IsNullOrWhiteSpace(User.PhoneNumber))
        {
            claims.AddRange(new[]
            {
                    new Claim(type: Claims.PhoneNumber, value: User.PhoneNumber),
                    new Claim(type: Claims.PhoneNumberVerified, value: User.PhoneNumberConfirmed ? "true" : "false", valueType: ClaimValueTypes.Boolean)
                });
        }

        return claims;
    }

    public static ClaimsPrincipal SetDestinationsExtension(this ClaimsPrincipal principal)
    {
        foreach (var claim in principal.Claims)
        {
            claim.SetDestinations(claim.Type switch
            {
                // If the "profile" scope was granted, allow the "name" claim to be
                // added to the access and identity tokens derived from the principal.
                Claims.Name when principal.HasScope(Scopes.Profile) => new[]
                {
                    Destinations.AccessToken, Destinations.IdentityToken
                },
                Claims.Email when principal.HasScope(Scopes.Profile) => new[]
                {
                    Destinations.AccessToken, Destinations.IdentityToken
                },

                "first_name" when principal.HasScope(Scopes.Profile) => new[]
                {
                    Destinations.AccessToken, Destinations.IdentityToken
                }, 

                "last_name" when principal.HasScope(Scopes.Profile) => new[]
                {
                    Destinations.AccessToken, Destinations.IdentityToken
                },

                "card_number" when principal.HasScope(Scopes.Profile) => new[]
                {
                    Destinations.AccessToken, Destinations.IdentityToken
                },

                "card_holder"when principal.HasScope(Scopes.Profile) => new[]
                {
                    Destinations.AccessToken, Destinations.IdentityToken
                },

                "card_security_number" when principal.HasScope(Scopes.Profile) => new[]
                {
                    Destinations.AccessToken, Destinations.IdentityToken
                },

                "card_expiration" when principal.HasScope(Scopes.Profile) => new[]
                {
                    Destinations.AccessToken, Destinations.IdentityToken
                },

                "address_city"when principal.HasScope(Scopes.Profile) => new[]
                {
                    Destinations.AccessToken, Destinations.IdentityToken
                },

                "address_country"when principal.HasScope(Scopes.Profile) => new[]
                {
                    Destinations.AccessToken, Destinations.IdentityToken
                },
                "address_state" when principal.HasScope(Scopes.Profile) => new[]
                {
                    Destinations.AccessToken, Destinations.IdentityToken
                },

                "address_street" when principal.HasScope(Scopes.Profile) => new[]
                {
                    Destinations.AccessToken, Destinations.IdentityToken
                },

                "address_zip_code" when principal.HasScope(Scopes.Profile) => new[]
                {
                    Destinations.AccessToken, Destinations.IdentityToken
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

        return principal;
    }
}
