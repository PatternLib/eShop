using System.Security.Claims;
using System.Security.Principal;
using EShopOnContainers.WebMvc.Models;

namespace EShopOnContainers.WebMvc.Services;

/// <summary>
/// Proporciona el servicio para extraer la informacion del usuario autenticado en la aplicación
/// </summary>
public class IdentityParser : IIdentityParser<ApplicationUser>
{
    /// <summary>
    /// Extrae la informacion del usuario del <see cref="IPrincipal"/> y la convierte en <see cref="ApplicationUser"/>.
    /// </summary>
    /// <param name="principal">La identidad del usuario autenticado en la aplicación.</param>
    /// <returns>Un objeto de tipo <see cref="ApplicationUser"/> que representa al usuario autenticado.</returns>
    public ApplicationUser Parse(IPrincipal principal)
    {
        // Pattern matching 'is' expression
        // assigns "claims" if "principal" is a "ClaimsPrincipal"
        if (principal is ClaimsPrincipal claims)
        {
            return new ApplicationUser
            {

                CardHolderName = claims.Claims.FirstOrDefault(x => x.Type == "card_holder")?.Value ?? "",
                CardNumber = claims.Claims.FirstOrDefault(x => x.Type == "card_number")?.Value ?? "",
                Expiration = claims.Claims.FirstOrDefault(x => x.Type == "card_expiration")?.Value ?? "",
                CardType = int.Parse(claims.Claims.FirstOrDefault(x => x.Type == "missing")?.Value ?? "0"),
                City = claims.Claims.FirstOrDefault(x => x.Type == "address_city")?.Value ?? "",
                Country = claims.Claims.FirstOrDefault(x => x.Type == "address_country")?.Value ?? "",
                Email = claims.Claims.FirstOrDefault(x => x.Type == "email")?.Value ?? "",
                Id = claims.Claims.FirstOrDefault(x => x.Type == "sub")?.Value ?? "",
                LastName = claims.Claims.FirstOrDefault(x => x.Type == "last_name")?.Value ?? "",
                Name = claims.Claims.FirstOrDefault(x => x.Type == "name")?.Value ?? "",
                PhoneNumber = claims.Claims.FirstOrDefault(x => x.Type == "phone_number")?.Value ?? "",
                SecurityNumber = claims.Claims.FirstOrDefault(x => x.Type == "card_security_number")?.Value ?? "",
                State = claims.Claims.FirstOrDefault(x => x.Type == "address_state")?.Value ?? "",
                Street = claims.Claims.FirstOrDefault(x => x.Type == "address_street")?.Value ?? "",
                ZipCode = claims.Claims.FirstOrDefault(x => x.Type == "address_zip_code")?.Value ?? ""
            };
        }
        throw new ArgumentException(message: "The principal must be a ClaimsPrincipal", paramName: nameof(principal));
    }
}