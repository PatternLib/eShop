using EShopOnContainers.Catalog.Domain;

namespace EShopOnContainers.Catalog.Extensions;

public static class CatalogItemExtensions
{
    public static List<CatalogItem> ChangeUriPlaceholder(this List<CatalogItem> items, AppSettingsJson settings)
    {
        var baseUri = settings.PicBaseUrl;
        var remoteStorage = settings.PicRemoteStorageEnabled;

        foreach (var item in items)
        {
             item.PictureUri = remoteStorage 
                ? baseUri + item.PictureFileName
                : baseUri.Replace(oldValue: "[0]", newValue: item.Id.ToString());
        }

        return items;
    }
}
