namespace EShopOnContainers.WebMvc.Infrastructure;

/// <summary>
/// Proporciona las URIs utilizadas en la comunicación HTTP con los microservicios.
/// </summary>
public static class URI
{
    /// <summary>
    /// Proporcionar las URIs relacionados con el servicio de catálogo.
    /// </summary>
    public static class Catalog
    {
        /// <summary>
        /// Construye la URI para obtener una lista paginada de elementos del catálogo.
        /// </summary>
        /// <param name="baseUri">La URI base del servicio de catálogo.</param>
        /// <param name="page">El número de página que se desea obtener.</param>
        /// <param name="take">La cantidad de elementos a recuperar por página.</param>
        /// <param name="brand">El identificador opcional de la marca para filtrar los resultados.</param>
        /// <param name="type">El identificador opcional del tipo de producto para filtrar los resultados.</param>
        /// <returns>Una cadena que representa la URI completa con los parámetros de consulta incluidos.</returns>
        public static string GetCatalogItemsUri(string baseUri, int page, int take, int? brand, int? type)
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

        /// <summary>
        /// Construye la URI para obtener un producto del catalogo según el <paramref name="productId"/>.
        /// </summary>
        /// <param name="baseUri">La URI base del servicio de catalog.</param>
        /// <param name="productId">El identificador del producto a recuperar.</param>
        /// <returns>Una cadena que representa la URI completa.</returns>
        public static string GetCatalogItemByIdUri(string baseUri, int productId)
        {
            return $"{baseUri}items/{productId}";
        }

        /// <summary>
        /// Construye la URI para obtner la lista de marcas en el catalogo.
        /// </summary>
        /// <param name="baseUri">La URI base del servicio de catalogo.</param>
        /// <returns>Una cadena que representa la URI completa.</returns>
        public static string GetBrandsUri(string baseUri)
        {
            return $"{baseUri}catalogBrands";
        }

        /// <summary>
        /// Contruye la URI para obtener la lista de los tipos de producto en el catalogo.
        /// </summary>
        /// <param name="baseUri">La URI base del servicio de catalogo.</param>
        /// <returns>Una cadena con la URI completa.</returns>
        public static string GetTypesUri(string baseUri)
        {
            return $"{baseUri}catalogTypes";
        }
    }

    /// <summary>
    /// Proporcionar las URIs relacionadas con el servicio de cesta de productos.
    /// </summary>
    public static class Basket
    {
        /// <summary>
        /// Construye la URI para obtener la cesta del usuario
        /// </summary>
        /// <param name="baseUri">La URI base del servicio de cesta de productos.</param>
        /// <param name="buyerId">El identificador del usuario del cual se desea obtener la cesta.</param>
        /// <returns>Una cadena con la URI completa.</returns>
        public static string GetBasketUri(string baseUri, string buyerId)
        {
            return $"{baseUri}/{buyerId}";
        }

        /// <summary>
        /// Construye la URI para actualizar la cesta de productos
        /// </summary>
        /// <param name="baseUri">La URI base del servicio de cesta de productos.</param>
        /// <returns>Una cade con la URI completa.</returns>
        public static string UpdateBasketUri(string baseUri)
        {
            return baseUri;
        }
    }
}
