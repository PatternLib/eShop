using EShopOnContainers.WebMvc.Domain;
using EShopOnContainers.WebMvc.Models;
using EShopOnContainers.WebMvc.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EShopOnContainers.WebMvc.Controllers;

/// <summary>
/// Controlador que maneja las operaciones relacionadas con <see cref="IBasketServices"/>.
/// </summary>
[Authorize]
public class BasketController : Controller
{
    private readonly IIdentityParser<ApplicationUser> _identityParser;
    private readonly IBasketServices _basketSvc;

    /// <summary>
    /// Inicializa una nueva instancia de la clase <see cref="BasketController"/>.
    /// </summary>
    /// <param name="identityParser">El extrae la información del usuario en un objeto <see cref="ApplicationUser"/>.</param>
    /// <param name="basketSvc">El servicio que maneja la lógica de negocio relacionada con <see cref="Basket"/>.</param>
    public BasketController(IIdentityParser<ApplicationUser> identityParser, IBasketServices basketSvc)
    {
        _identityParser = identityParser;
        _basketSvc = basketSvc;
    }

    /// <summary>
    /// Muestra la cesta de compras del usuario.
    /// </summary>
    /// <returns>Una vista que muestra el contenido de la cesta de compras del usuario.</returns>
    public async Task<IActionResult> Index()
    {
        var user = _identityParser.Parse(principal: HttpContext.User);
        var vm = await _basketSvc.GetBasketAsync(user:  user);

        return View(model: vm);
    }

    /// <summary>
    /// Actualiza las cantidades de los artículos en la <see cref="Basket"/>.
    /// </summary>
    /// <param name="quantities">La lista de los productos y sus cantidades.</param>
    /// <param name="action">La acción que se desea realizar.</param>
    /// <returns>Una vista actualizada con el contenido de la cesta.</returns>
    [HttpPost]
    public async Task<IActionResult> Index(Dictionary<string, int> quantities, string action)
    {
        if (!quantities.Any())
        {
            return BadRequest(error: "No updates sent");
        }

        var user = _identityParser.Parse(principal: HttpContext.User);
        var basket = await _basketSvc.SetQuantitiesAsync(user: user, quantities: quantities);

        if (basket == null)
        {
            return BadRequest(error: $"Basket with ID {user.Id} not found.");
        }

        if (action == "[ CHECKOUT ]")
        {
            return RedirectToAction(actionName: "Create", controllerName: "Order");
        }

        return View (model: basket);
    }

    /// <summary>
    /// Agrega un artículo a la cesta de compras del usuario.
    /// </summary>
    /// <param name="productDetails">El objeto que contiene los detalles del producto que se va a agregar a la cesta.</param>
    /// <param name="quantity">La cantidad del producto a agregar a la cesta (valor por defecto es 1).</param>
    /// <returns>Una vista con la cesta de compras actualizada.</returns>
    public async Task<IActionResult> AddToBasket(CatalogItem productDetails, string action, int quantity = 1)
    {
        if (productDetails?.Id != null)
        {
            var user = _identityParser.Parse(principal: HttpContext.User);

            if (action == "[ ADD TO CART ]")
            {
                await _basketSvc.AddItemToBasketAsync(user: user, productId: productDetails.Id, quantity: quantity);
            }

            var basket = await _basketSvc.GetBasketAsync(user: user);

            var curItem = basket.Items.Find(match: itm => itm.ProductId.Equals(value: productDetails.Id.ToString()));

            ViewData["COUNT_ITEM"] = curItem.Quantity;

            return View(model: productDetails);
        }

        return RedirectToAction(
            actionName: "Index",
            controllerName: "Catalog");
    }
}
