using EShopOnContainers.WebMvc.Domain;
using EShopOnContainers.WebMvc.Models;
using EShopOnContainers.WebMvc.Services;
using Microsoft.AspNetCore.Mvc;

namespace EShopOnContainers.WebMvc.Controllers;

public class CatalogController : Controller
{
    private readonly ICatalogServices _catalogSvc;

    public CatalogController(ICatalogServices catalogSvc)
    {
        _catalogSvc = catalogSvc;
    }

    public async Task<IActionResult> Index(int? brandFilterApplied, int? typeFilterApplied, int? page)
    {
        var itemsPage = 10;

        var catalog = await _catalogSvc.GetCatalogItemsAsync(page ?? 0, itemsPage, brandFilterApplied, typeFilterApplied);

        var vmodel = new CatalogIndexViewModel
        {
            CatalogItems = catalog.Data,
            Brands = await _catalogSvc.GetAllBrandsAsync(),
            Types = await _catalogSvc.GetAllTypesAsync(),
            BrandFilterApplied = brandFilterApplied,
            TypeFilterApplied = typeFilterApplied,
            PaginationInfo = new PaginationInfo
            {
                ActualPage = page ?? 0,
                ItemsPerPage = catalog.Data.Count,
                TotalItems = catalog.Count,
                TotalPage = (int)Math.Ceiling((decimal)catalog.Count / itemsPage),
            }
        };

        vmodel.PaginationInfo.Next = (vmodel.PaginationInfo.ActualPage == vmodel.PaginationInfo.TotalPage - 1) ? "disabled" : "";
        vmodel.PaginationInfo.Previous = (vmodel.PaginationInfo.ActualPage == 0) ? "disabled" : "";

        return View(vmodel);
    }

}
