namespace EShopOnContainers.Basket.Domain;

public class CostumerBasket
{
    public string BuyerId { get; set; }
    public List<BasketItem> Items { get; set; }

    public CostumerBasket(string buyerId)
    {
        BuyerId = buyerId;
        Items = new List<BasketItem>();
    }
}
