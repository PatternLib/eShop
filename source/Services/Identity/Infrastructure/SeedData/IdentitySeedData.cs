using EShopOnContainers.Identity.Domain;
using Microsoft.AspNetCore.Identity;

namespace EShopOnContainers.Identity.Infrastructure.SeedData;

public static class IdentitySeedData
{
    public static async Task Initialize(OpenIDContext context)
    {
        if (context.Users.Any())
        {
            return;
        }

        var passwordHasher = new PasswordHasher<ApplicationUser>();

        ApplicationUser user = new ApplicationUser()
        {
            CardHolderName = "DemoUser",
            CardNumber = "4012888888881881",
            CardType = 1,
            City = "Redmond",
            Country = "U.S.",
            Email = "demouser@microsoft.com",
            Expiration = "12/20",

            Id = Guid.NewGuid().ToString(),
            LastName = "DemoLastName",
            Name = "DemoUser",
            PhoneNumber = "1234567890",
            UserName = "demouser@microsoft.com",
            ZipCode = "98052",
            State = "WA",
            Street = "15703 NE 61st Ct",
            SecurityNumber = "535",

            NormalizedEmail = "DEMOUSER@MICROSOFT.COM",
            NormalizedUserName = "DEMOUSER@MICROSOFT.COM",
            SecurityStamp = Guid.NewGuid().ToString("D"),
        };

        user.PasswordHash = passwordHasher.HashPassword(user: user, password: "Pass@word1");

        context.Users.Add(user);

        await context.SaveChangesAsync();
    }
}

