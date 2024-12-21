namespace EShopOnContainers.WebMvc.Infrastructure;

public static class API
{
    public static class Catalog
    {
        public static string GetAllCatalogItems(string baseUri, int page, int take, int? brand, int? type)
        {
            var filterQueryString = "";

            if (type.HasValue)
            {
                filterQueryString = brand.HasValue
                    ? $"/type/{type.Value}/brand/{brand.Value}" // List items from a specific type and brand
                    : $"/type/{type.Value}/brand/"; // List items from a specific type
            }
            else if (brand.HasValue)
            {
                filterQueryString = brand.HasValue
                    ? $"/type/all/brand/{brand.Value}" // List items from a specific brand
                    : $"/type/all/brand/";
            }

            return $"{baseUri}items{filterQueryString}?pageIndex={page}&pageSize={take}";
        }

        public static string GetAllBrands(string baseUri)
        {
            return $"{baseUri}catalogBrands";
        }

        public static string GetAllTypes(string baseUri)
        {
            return $"{baseUri}catalogTypes";
        }
    }
}
