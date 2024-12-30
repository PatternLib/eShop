using EShopOnContainers.WebMvc.Models;
using EShopOnContainers.WebMvc.Services;
using Microsoft.AspNetCore.Mvc;

namespace EShopOnContainers.WebMvc.ViewComponents;

/// <summary>
/// Componente de vista que muestra el número de artículos en la cesta de compras del usuario.
/// </summary>
public class BasketCounter : ViewComponent
{
    private readonly IBasketServices _basketServices;

    /// <summary>
    /// Inicializa una nueva instancia de la clase <see cref="BasketCounter"/>.
    /// </summary>
    /// <param name="basketServices">Servicio que maneja la lógica de la cesta de compras.</param>
    public BasketCounter(IBasketServices basketServices)
    {
        _basketServices = basketServices;
    }

    /// <summary>
    /// Ejecuta la lógica del componente para recuperar la cantidad de artículos en la cesta del usuario.
    /// </summary>
    /// <param name="User">Usuario cuya cesta se va acontar</param>
    /// <returns>Un resultado de vista con el número de artículos en la cesta.</returns>
    public async Task<IViewComponentResult> InvokeAsync(ApplicationUser User)
    {
        var vcm = new BasketCounterViewComponentModel();

        var basket = await _basketServices.GetBasketAsync(user: User);

        vcm.ItemsCount = basket.Items.Sum(selector: x => x.Quantity);

        return View(model: vcm);
    }
}
