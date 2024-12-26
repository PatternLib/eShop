using EShopOnContainers.Identity.Domain;
using EShopOnContainers.Identity.Models;

namespace EShopOnContainers.Identity.Extensions;

public static class ApplicationUserBuilderExtensions
{
    public static ApplicationUser CreateUserExtension(this AccountRegisterViewModel.InputAccountRegister Input)
    {
        return new ApplicationUser()
        {
            CardHolderName = Input.User.CardHolderName,
            CardNumber = Input.User.CardNumber,
            CardType = Input.User.CardType,
            SecurityNumber = Input.User.SecurityNumber,
            Expiration = Input.User.Expiration,

            Id = Guid.NewGuid().ToString(),
            LastName = Input.User.LastName,
            Name = Input.User.Name,
            UserName = Input.Email,
            Email = Input.Email,
            PhoneNumber = Input.User.PhoneNumber,
            ZipCode = Input.User.ZipCode,
            Country = Input.User.Country,
            State = Input.User.State,
            City = Input.User.City,
            Street = Input.User.Street,

            NormalizedEmail = Input.Email.ToLower().ToUpper(),
            NormalizedUserName = Input.Email.ToLower().ToUpper(),
            SecurityStamp = Guid.NewGuid().ToString("D"),
        };
    }
}
