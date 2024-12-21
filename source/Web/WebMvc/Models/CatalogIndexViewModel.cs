using EShopOnContainers.WebMvc.Domain;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EShopOnContainers.WebMvc.Models;

public class CatalogIndexViewModel
{
    public IEnumerable<CatalogItem> CatalogItems { get; set; }
    public IEnumerable<SelectListItem> Brands { get; set; }
    public IEnumerable<SelectListItem> Types { get; set; }
    public int? BrandFilterApplied { get; set; }
    public int? TypeFilterApplied { get; set; }
    public PaginationInfo PaginationInfo { get; set; }
}
