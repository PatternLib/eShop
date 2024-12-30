namespace EShopOnContainers.WebMvc.Models;

public class BasketCounterViewComponentModel
{
    public int ItemsCount { get; set; }
    public string Disabled => (ItemsCount == 0) ? "disabled": "";
}
