using System.Collections;
using System.Text.Json.Nodes;
using EShopOnContainers.WebMvc.Domain;
using EShopOnContainers.WebMvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;

namespace EShopOnContainers.WebMvc.Services;

public class CatalogServices : ICatalogServices
{
    private readonly AppSettingsJson _settings;
    private readonly HttpClient _httpClient;
    private readonly ILogger<CatalogServices> _logger;

    private readonly string _remoteServiceBaseUrl;

    public CatalogServices(IOptionsSnapshot<AppSettingsJson> settings, HttpClient httpClient, ILogger<CatalogServices> logger)
    {
        _settings = settings.Value;
        _httpClient = httpClient;
        _logger = logger;

        _remoteServiceBaseUrl = $"{_settings.CatalogUrl}/api/v1/catalog/";
    }

    public async Task<Catalog> GetCatalogItemsAsync(int page, int take, int? brand, int? type)
    {
        var uri = API.Catalog.GetAllCatalogItems(_remoteServiceBaseUrl, page, take, brand, type);

        var responseString = await _httpClient.GetStringAsync(requestUri: uri);

        JsonNode catalogNode = JsonNode.Parse(responseString)!;
        JsonArray data = catalogNode["data"]!.AsArray();

        var catalog = new Catalog
        {
            PageIndex = catalogNode["pageIndex"]!.GetValue<int>(),
            PageSize = catalogNode["pageSize"]!.GetValue<int>(),
            Count = catalogNode["count"]!.GetValue<int>(),
            Data = data.Select(selector: jsonNode =>
            {
                return new CatalogItem
                {
                    Id = jsonNode["id"]!.GetValue<int>(),
                    Name = jsonNode["name"]!.GetValue<string>(),
                    Description = jsonNode["description"]!.GetValue<string>(),
                    Price = jsonNode["price"]!.GetValue<decimal>(),
                    PictureUri = jsonNode["pictureUri"]!.GetValue<string>(),
                    CatalogBrandId = jsonNode["catalogBrandId"]!.GetValue<int>(),
                    CatalogTypeId = jsonNode["catalogTypeId"]!.GetValue<int>()
                };
            })
            .ToList()
        };

        return catalog;
    }

    public async Task<IEnumerable<SelectListItem>> GetAllBrandsAsync()
    {
        var uri = API.Catalog.GetAllBrands(_remoteServiceBaseUrl);

        var responseString = await _httpClient.GetStringAsync(requestUri: uri);

        var items = new List<SelectListItem>();

        items.Add(item: new SelectListItem { Value = null, Text = "All", Selected = true });

        JsonArray brandNode = JsonNode
            .Parse(json: responseString)!
            .AsArray();

        items.AddRange(collection: brandNode.Select(selector: jsonNode =>
        {
            return new SelectListItem
            {
                Value = jsonNode["id"]!.GetValue<int>().ToString(),
                Text = jsonNode["brand"]!.GetValue<string>()
            };
        }));

        return items;
    }

    public async Task<IEnumerable<SelectListItem>> GetAllTypesAsync()
    {
        var uri = API.Catalog.GetAllTypes(_remoteServiceBaseUrl);

        var responseString = await _httpClient.GetStringAsync(requestUri: uri);

        var items = new List<SelectListItem>();

        items.Add(item: new SelectListItem { Value = null, Text = "All", Selected = true });

        JsonArray typeNode = JsonNode
            .Parse(json: responseString)!
            .AsArray();

        items.AddRange(collection: typeNode.Select(selector: jsonNode =>
        {
            return new SelectListItem
            {
                Value = jsonNode["id"]!.GetValue<int>().ToString(),
                Text = jsonNode["type"]!.GetValue<string>()
            };
        }));

        return items;
    }
}
