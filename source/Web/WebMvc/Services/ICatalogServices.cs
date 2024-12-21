using EShopOnContainers.WebMvc.Domain;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EShopOnContainers.WebMvc.Services
{
    public interface ICatalogServices
    {
        Task<IEnumerable<SelectListItem>> GetAllBrandsAsync();
        Task<IEnumerable<SelectListItem>> GetAllTypesAsync();
        Task<Catalog> GetCatalogItemsAsync(int page, int take, int? brand, int? type);
    }
}