using EShopOnContainers.WebMvc.Domain;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EShopOnContainers.WebMvc.Services
{
    public interface ICatalogServices
    {
        Task<IEnumerable<SelectListItem>> GetCatalogBrandsAsync();
        Task<IEnumerable<SelectListItem>> GetCatalogTypesAsync();
        Task<Catalog> GetCatalogItemsAsync(int page, int take, int? brand, int? type);
        Task<CatalogItem> GetCatalogItemByIdAsync(int productId);
    }
}