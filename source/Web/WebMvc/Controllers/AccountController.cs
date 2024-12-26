using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EShopOnContainers.WebMvc.Controllers;

[Authorize]
public class AccountController : Controller
{
    private readonly ILogger<AccountController> _logger;

    public AccountController(ILogger<AccountController> logger)
    {
        _logger = logger;
    }

    public async Task<IActionResult> Login(string returnUrl)
    {
        var token = await HttpContext.GetTokenAsync(tokenName: "access_token");

        if (token != null)
        {
            ViewData["access_token"] = token;
        }

        return RedirectToAction(actionName: "Index", controllerName: "Catalog");
    }

    public IActionResult Logout()
    {
        return SignOut(
            authenticationSchemes: 
            [
                // Clear auth cookie
                CookieAuthenticationDefaults.AuthenticationScheme,
                // Redirect to OIDC provider signout endpoint
                OpenIdConnectDefaults.AuthenticationScheme
            ],
            properties: new AuthenticationProperties
            {
                RedirectUri = "/Account/Logout"
            });
    }
}
