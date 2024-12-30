using System.Security.Principal;

namespace EShopOnContainers.WebMvc.Services;

/// <summary>
/// Proporciona una abstracción para extraer la informacion del usuario autenticado en la aplicación
/// </summary>
public interface IIdentityParser<T>
{
    /// <summary>
    /// Extrae la informacion del usuario del <see cref="IPrincipal"/> y la convierte en <see cref="T"/>.
    /// </summary>
    /// <param name="principal">La identidad del usuario autenticado en la aplicación.</param>
    /// <returns>Un objeto de tipo <see cref="T"/> que representa al usuario autenticado.</returns>
    T Parse(IPrincipal principal);
}