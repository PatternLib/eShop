using System.ComponentModel.DataAnnotations;

namespace EShopOnContainers.Basket.Domain;

public class BasketItem : IValidatableObject
{
    public string Id { get; set; }
    public string ProductId { get; set; }
    public string ProductName { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal OldUnitPrice { get; set; }
    public int Quantity { get; set; }
    public string PictureUrl { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Quantity < 1)
        {
            yield return new ValidationResult( 
                errorMessage: "Invalid number of unitis", 
                memberNames: [nameof(Quantity)]);
        }
    }
}
