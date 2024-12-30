using EShopOnContainers.WebMvc.Domain;
using EShopOnContainers.WebMvc.Models;

namespace EShopOnContainers.WebMvc.Services;

/// <summary>
/// Proporciona una abstracción para gestionar la cesta de compra de un usuario en la tienda.
/// </summary>
public interface IBasketServices
{
    /// <summary>
    /// Agrega un producto con el identificador <paramref name="productId"/> a la cesta del <paramref name="user"/>.
    /// </summary>
    /// <param name="user">El usuario al que pertenece la cesta.</param>
    /// <param name="productId">El identificador del producto que se desea agregar a la cesta.</param>
    /// <param name="quantity">La cantidad del producto que se va a agregar.</param>
    /// <returns>
    /// Una <see cref="Task"/> que representa la operación asincrónica y contiene la <see cref="Basket"/> actualizada del usuario.
    /// </returns>
    Task<Basket> AddItemToBasketAsync(ApplicationUser user, int productId, int quantity = 1);

    /// <summary>
    /// Recupera la <see cref="Basket"/> asociada al <paramref name="user"/> especificado.
    /// </summary>
    /// <param name="user">El usuario cuya cesta de compras se desea obtener.</param>
    /// <returns>
    /// Una <see cref="Task"/> que representa la operación asincrónica y contiene la <see cref="Basket"/> asociada al usuario.
    /// </returns>
    Task<Basket> GetBasketAsync(ApplicationUser user);

    /// <summary>
    /// Actualiza las cantidades de productos en la <see cref="Basket"/> del <paramref name="user"/> especificado.
    /// </summary>
    /// <param name="user">El usuario cuya cesta se desea actualizar.</param>
    /// <param name="quantities">Las nuevas cantidades de los productos.</param>
    /// <returns>
    /// Una <see cref="Task"/> que representa la operación asincrónica y contiene la <see cref="Basket"/> actualizada del usuario.
    /// </returns>
    Task<Basket> SetQuantitiesAsync(ApplicationUser user, Dictionary<string, int> quantities);
}
