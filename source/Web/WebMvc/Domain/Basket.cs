using static EShopOnContainers.WebMvc.Infrastructure.URI;

namespace EShopOnContainers.WebMvc.Domain;

public class Basket
{
    public string BuyerId { get; set; }

    public List<BasketItem> Items { get; set; }

    public Basket(string buyerId)
    {
        BuyerId = buyerId;
        Items = new List<BasketItem>();
    }

    public void AddBasketItem(CatalogItem item, int productId, int quantity)
    {
        var curItem = Items.Find(match: itm => itm.ProductId.Equals(value: productId.ToString()));

        if (curItem == null)
        {
            Items.Add(item: new BasketItem
            {
                Id = Guid.NewGuid().ToString(),
                ProductId = item.Id.ToString(),
                ProductName = item.Name,
                UnitPrice = item.Price,
                Quantity = quantity,
                PictureUrl = item.PictureUri,
            });
        }
        else
        {
            curItem.Quantity = quantity;
        }
    }

    public decimal Total()
    {
        return Math.Round(
            d: Items.Sum(
                selector: s => s.UnitPrice *  s.Quantity), 
            decimals: 2);
    }
}
