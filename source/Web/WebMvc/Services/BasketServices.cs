using System.Text.Json;
using System.Text.Json.Nodes;
using EShopOnContainers.WebMvc.Domain;
using EShopOnContainers.WebMvc.Infrastructure;
using EShopOnContainers.WebMvc.Models;
using Microsoft.Extensions.Options;

namespace EShopOnContainers.WebMvc.Services;

/// <summary>
/// Proporciona el servicio para administrar el <see cref="Basket"/> de un usuario
/// </summary>
public class BasketServices : IBasketServices
{
    private readonly IOptions<AppSettingsJson> _settings;
    private readonly ILogger<BasketServices> _logger;
    private readonly ICatalogServices _catalogServices;
    private readonly HttpClient _httpClient;
    private readonly string _basketUrlApi;

    /// <summary>
    /// Inicializa una nueva instancia de la clase <see cref="BasketServices"/>.
    /// </summary>
    /// <param name="settings">Las opciones de configuración, representadas por un objeto <see cref="AppSettingsJson"/>.</param>
    /// <param name="httpClient">El cliente HTTP utilizado para realizar solicitudes.</param>
    /// <param name="catalogServices">El servicio que proporciona acceso a los productos y el catálogo de la tienda.</param>
    /// <param name="logger">El logger utilizado para registrar mensajes y errores durante la ejecución de los métodos en esta clase.</param>
    public BasketServices(IOptions<AppSettingsJson> settings, HttpClient httpClient, ICatalogServices catalogServices, ILogger<BasketServices> logger)
    {
        _logger = logger;
        _httpClient = httpClient;
        _catalogServices = catalogServices;
        _settings = settings;
        _basketUrlApi = String.Format(
            format: "{0}/api/v1/basket",
            args: [_settings.Value.BasketUrl]);
    }

    /// <summary>
    /// Agrega un producto con el identificador <paramref name="productId"/> a la cesta del <paramref name="user"/>.
    /// </summary>
    /// <param name="user">El usuario al que pertenece la cesta.</param>
    /// <param name="productId">El identificador del producto que se desea agregar a la cesta.</param>
    /// <param name="quantity">La cantidad del producto que se va a agregar.</param>
    /// <returns>
    /// Una <see cref="Task"/> que representa la operación asincrónica y contiene la <see cref="Basket"/> actualizada del usuario.
    /// </returns>
    public async Task<Basket> AddItemToBasketAsync(ApplicationUser user, int productId, int quantity = 1)
    {
        // WebMvc le manda un POST a ApiGw
        // [+] newItem
        // var newItem = new { CatalogItemId = productId, BasketId = user.Id, Quantity = quantity };

        // Web.BFF envia un solicitud GET al catalog para obtener el product que se desea agregar a la cesta.
        // Step 1: Get the item from catalog
        var item = await _catalogServices.GetCatalogItemByIdAsync(productId: productId);

        // Web.BFF envia una solictud GET al basket para obtener la cesta del usuario, si existe.
        // Step 2: Get current basket status
        var basket = await GetBasketAsync(user: user);

        // Step 3: Merge current status with new product
        basket.AddBasketItem(item: item, productId: productId, quantity: quantity);

        // Web.BFF envia una solicitud POST al basket para actualizar la cesta del usuario.
        // Step 4: Update basket
        var updateUri = URI.Basket.UpdateBasketUri(baseUri: _basketUrlApi);

        var basketContent = new StringContent(
            content: JsonSerializer.Serialize(value: basket),
            encoding: System.Text.Encoding.UTF8,
            mediaType: "application/json");
        
        var data = await _httpClient.PostAsync(requestUri: updateUri, content: basketContent);
        
        return await DeserializeBasketAsync(content: data.Content);
    }

    /// <summary>
    /// Recupera la <see cref="Basket"/> asociada al <paramref name="user"/> especificado.
    /// </summary>
    /// <param name="user">El usuario cuya cesta de compras se desea obtener.</param>
    /// <returns>
    /// Una <see cref="Task"/> que representa la operación asincrónica y contiene la <see cref="Basket"/> asociada al usuario.
    /// </returns>
    public async Task<Basket> GetBasketAsync(ApplicationUser user)
    {
        var uri = URI.Basket.GetBasketUri(baseUri: _basketUrlApi, buyerId: user.Id);

        var responseString = await _httpClient.GetStringAsync(requestUri: uri);

        return String.IsNullOrEmpty(value: responseString)
            ? new Basket(buyerId: user.Id)
            : DeserializeBasket(responseJson: responseString);
    }

    /// <summary>
    /// Actualiza las cantidades de productos en la <see cref="Basket"/> del <paramref name="user"/> especificado.
    /// </summary>
    /// <param name="user">El usuario cuya cesta se desea actualizar.</param>
    /// <param name="quantities">Las nuevas cantidades de los productos.</param>
    /// <returns>
    /// Una <see cref="Task"/> que representa la operación asincrónica y contiene la <see cref="Basket"/> actualizada del usuario.
    /// </returns>
    public async Task<Basket> SetQuantitiesAsync(ApplicationUser user, Dictionary<string, int> quantities)
    {
        // Step 1: Retrieve the current basket
        var curBasket = await GetBasketAsync(user: user);
        
        if (curBasket == null)
        {
            return null;
        }

        // Step 2: Update with new quantities
        foreach(var update  in quantities)
        {
            var item = curBasket.Items.Find(match: x => x.Id == update.Key);
            item.Quantity = update.Value;
        }

        // Web.BFF envia una solicitud POST al basket para actualizar la cesta del usuario.
        // Save the update basket
        var updateUri = URI.Basket.UpdateBasketUri(baseUri: _basketUrlApi);

        var basketContent = new StringContent(
            content: JsonSerializer.Serialize(value: curBasket),
            encoding: System.Text.Encoding.UTF8,
            mediaType: "application/json");

        var data = await _httpClient.PostAsync(requestUri: updateUri, content: basketContent);

        return await DeserializeBasketAsync(content: data.Content);
    }

    /// <summary>
    /// Deserializa el contenido HTTP en un objeto de tipo <see cref="Basket"/>.
    /// </summary>
    /// <param name="content">El contenido HTTP que contiene los datos JSON para deserializar.</param>
    /// <returns>
    /// Una <see cref="Task"/> que representa el proceso asincrónico y contiene <see cref="Basket"/> deserializado.
    /// </returns>
    public async Task<Basket> DeserializeBasketAsync(HttpContent content)
    {
        return DeserializeBasket(
            responseJson: await content.ReadAsStringAsync());
    }

    /// <summary>
    /// Deserializa una cadena JSON en un objeto de tipo <see cref="Basket"/>.
    /// </summary>
    /// <param name="responseJson">La cadena JSON que representa un objeto <see cref="Basket"/>.</param>
    /// <returns>Un objeto deserializado de tipo <see cref="Basket"/>.</returns>
    public Basket DeserializeBasket(string responseJson)
    {
        JsonNode basketJson = JsonNode.Parse(json: responseJson)!;
        JsonArray jsonItems = basketJson["items"]!.AsArray();

        return new Basket(buyerId: basketJson["buyerId"]!.GetValue<String>())
        {
            Items = jsonItems.Select(selector: static jsonNode =>
            {
                return new BasketItem
                {
                    Id = jsonNode["id"]!.GetValue<String>(),
                    ProductId = jsonNode["productId"]!.GetValue<String>(),
                    ProductName = jsonNode["productName"]!.GetValue<String>(),
                    UnitPrice = jsonNode["unitPrice"]!.GetValue<Decimal>(),
                    OldUnitPrice = jsonNode["oldUnitPrice"]!.GetValue<Decimal>(),
                    Quantity = jsonNode["quantity"]!.GetValue<Int32>(),
                    PictureUrl = jsonNode["pictureUrl"]!.GetValue<String>()
                };
            })
            .ToList()
        };
    }
}
