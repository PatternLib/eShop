namespace EShopOnContainers.Catalog.Domain;

public class CatalogItem
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public string PictureFileName { get; set; }
    public string PictureUri { get; set; }
    public int CatalogTypeId { get; set; }
    public CatalogType CatalogType { get; set; }
    public int CatalogBrandId { get; set; }
    public CatalogBrand CatalogBrand { get; set; }
    // Quantity in stock
    public int AvailableStock { get; set; }
    // This represent the minimum number of items that should be in stock
    public int RestockThreshold { get; set; }
    // Maximum number of units that can be in-stock at any time
    public int MaxStockThreshold { get; set; }
    /// <summary>
    /// Represent a restocking action. When the stock falls to or below <see cref="RestockThreshold"/>.
    /// </summary>
    public bool OnReorder { get; set; }

    public CatalogItem() { }

    /// <summary>
    /// Decrements the quantity of a particular item in inventory
    /// </summary>
    /// <remarks>
    /// If there is sufficient stock of an item, then the integer returned at the end of this call should be the same as <paramref name="quantityDesired"/>
    /// </remarks>
    /// <param name="quantityDesired"></param>
    /// <returns>Return the number actually removed from stock.</returns>
    /// <exception cref="InvalidOperationException"></exception>
    public int RemoveStock(int quantityDesired)
    {
        if (AvailableStock == 0)
        {
            throw new InvalidOperationException($"Empty stock, product item {Name} is sold out");
        }

        if(quantityDesired <= 0)
        {
            throw new InvalidOperationException($"Item units desired should be greather than cero");
        }

        int removed = Math.Min(val1: quantityDesired, val2: AvailableStock);

        AvailableStock -= removed;

        return removed;
    }

    /// <summary>
    /// Increments the quantity of a particular item in inventory.
    /// </summary>
    /// <param name="quantity">Numbers of items to add to the stock</param>
    /// <returns>Return the quantity that has been added to stock</returns>
    public int AddStock(int quantity)
    {
        int original = AvailableStock;

        if((AvailableStock + quantity) > MaxStockThreshold)
        {
            // If quantity is 20. The code adds only 10 ( 100 - 90 )
            AvailableStock += MaxStockThreshold - AvailableStock;
        }
        else
        {
            AvailableStock += quantity;
        }

        OnReorder = false;

        return AvailableStock - original;
    }
}
