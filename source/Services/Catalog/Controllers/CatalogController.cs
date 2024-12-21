using System.Net;
using EShopOnContainers.Catalog.Domain;
using EShopOnContainers.Catalog.Extensions;
using EShopOnContainers.Catalog.Infrastructure;
using EShopOnContainers.Catalog.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace EShopOnContainers.Catalog.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class CatalogController : ControllerBase
{
    private readonly CatalogContext _catalogContext;
    private readonly AppSettingsJson _settings;

    public CatalogController(CatalogContext context, IOptionsSnapshot<AppSettingsJson> settings)
    {
        _catalogContext = context ?? throw new ArgumentNullException(nameof(context)); ;
        _settings = settings.Value;

        ((DbContext)context).ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
    }

    // List items without filtering by type or brand
    // GET api/v1/[controller]/items/[?pageSize=3&pageIndex=10]
    [HttpGet]
    [Route("items")]
    [ProducesResponseType(typeof(PaginatedItemsViewModel<CatalogItem>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(IEnumerable<CatalogItem>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> Items([FromQuery] int pageSize = 10, [FromQuery] int pageIndex = 0)
    {
        var totalItems = await _catalogContext.CatalogItems
            .LongCountAsync();

        var itemsOnPage = await _catalogContext.CatalogItems
            .OrderBy(c => c.Name)
            .Skip(pageSize * pageIndex)
            .Take(pageSize)
            .ToListAsync();

        itemsOnPage = itemsOnPage.ChangeUriPlaceholder(settings: _settings);

        var model = new PaginatedItemsViewModel<CatalogItem>(
            pageIndex, pageSize, totalItems, itemsOnPage);

        return Ok(model);
    }

    // List items from a specific brand
    // GET api/v1/[controller]/items/type/all/brand/1[?pageSize=3&pageIndex=10]
    [HttpGet]
    [Route("[action]/type/all/brand/{catalogBrandId:int?}")]
    [ProducesResponseType(typeof(PaginatedItemsViewModel<CatalogItem>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> Items(int? catalogBrandId, [FromQuery] int pageSize = 10, [FromQuery] int pageIndex = 0)
    {
        var root = (IQueryable<CatalogItem>)_catalogContext.CatalogItems;

        if (catalogBrandId.HasValue)
        {
            root = root.Where(ci => ci.CatalogBrandId == catalogBrandId);
        }

        var totalItems = await root
            .LongCountAsync();

        var itemsOnPage = await root
            .Skip(pageSize * pageIndex)
            .Take(pageSize)
            .ToListAsync();

        itemsOnPage = itemsOnPage.ChangeUriPlaceholder(settings: _settings);

        var model = new PaginatedItemsViewModel<CatalogItem>(
            pageIndex, pageSize, totalItems, itemsOnPage);

        return Ok(model);
    }

    // List items from a specific type and brand
    // GET api/v1/[controller]/items/type/1/brand/1[?pageSize=3&pageIndex=10]
    [HttpGet]
    [Route("[action]/type/{catalogTypeId}/brand/{catalogBrandId:int?}")]
    [ProducesResponseType(typeof(PaginatedItemsViewModel<CatalogItem>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> Items(int catalogTypeId, int? catalogBrandId, [FromQuery] int pageSize = 10, [FromQuery] int pageIndex = 0)
    {
        var root = (IQueryable<CatalogItem>)_catalogContext.CatalogItems;

        root = root.Where(ci => ci.CatalogTypeId == catalogTypeId);

        if (catalogBrandId.HasValue)
        {
            root = root.Where(ci => ci.CatalogBrandId == catalogBrandId);
        }

        var totalItems = await root
            .LongCountAsync();

        var itemsOnPage = await root
            .Skip(pageSize * pageIndex)
            .Take(pageSize)
            .ToListAsync();

        itemsOnPage = itemsOnPage.ChangeUriPlaceholder(settings: _settings);

        var model = new PaginatedItemsViewModel<CatalogItem>(
            pageIndex, pageSize, totalItems, itemsOnPage);

        return Ok(model);
    }

    // List items whose name is exactly or partially
    // GET api/v1/[controller]/items/withname/samplename[?pageSize=3&pageIndex=10]
    [HttpGet]
    [Route("[action]/withname/{name:minlength(1)}")]
    [ProducesResponseType(typeof(PaginatedItemsViewModel<CatalogItem>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> Items(string name, [FromQuery] int pageSize = 10, [FromQuery] int pageIndex = 0)
    {
        var totalItems = await _catalogContext.CatalogItems
                .Where(c => c.Name.StartsWith(name))
                .LongCountAsync();

        var itemsOnPage = await _catalogContext.CatalogItems
            .Where(c => c.Name.StartsWith(name))
            .Skip(pageSize * pageIndex)
            .Take(pageSize)
            .ToListAsync();

        itemsOnPage = itemsOnPage.ChangeUriPlaceholder(settings: _settings);

        var model = new PaginatedItemsViewModel<CatalogItem>(
            pageIndex, pageSize, totalItems, itemsOnPage);

        return Ok(model);
    }



    // Get Item By Id
    // GET api/v1/[controller]/items/1
    [HttpGet]
    [Route("items/{id:int}")]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(CatalogItem), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetItemById(int id)
    {
        if (id <= 0)
        {
            return BadRequest();
        }

        var item = await _catalogContext.CatalogItems.SingleOrDefaultAsync(ci => ci.Id == id);
        
        if (item != null)
        {
            var baseUri = _settings.PicBaseUrl;
            var remoteStorage = _settings.PicRemoteStorageEnabled;

            item.PictureUri = remoteStorage
                    ? baseUri + item.PictureFileName
                    : baseUri.Replace(oldValue: "[0]", newValue: item.Id.ToString());

            return Ok(item);
        }

        return NotFound();
    }



    // List types
    // GET api/v1/[controller]/CatalogTypes
    [HttpGet]
    [Route("[action]")]
    [ProducesResponseType(typeof(List<CatalogItem>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> CatalogTypes()
    {
        var items = await _catalogContext.CatalogTypes
            .ToListAsync();

        return Ok(items);
    }

    // List brands
    // GET api/v1/[controller]/CatalogBrands
    [HttpGet]
    [Route("[action]")]
    [ProducesResponseType(typeof(List<CatalogItem>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> CatalogBrands()
    {
        var items = await _catalogContext.CatalogBrands
            .ToListAsync();

        return Ok(items);
    }
}
