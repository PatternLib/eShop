using EShopOnContainers.Identity.Domain;
using EShopOnContainers.Identity.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EShopOnContainers.Identity.Controllers;

public class AccountController : Controller
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ILogger<AccountController> _logger;

    public AccountController(SignInManager<ApplicationUser> signInManager, ILogger<AccountController> logger)
    {
        _signInManager = signInManager;
        _logger = logger;
    }

    [TempData]
    public string ErrorMessage { get; set; }

    public string ReturnUrl { get; set; }

    public IList<AuthenticationScheme> ExternalLogins { get; set; }

    [HttpGet]
    public async Task<IActionResult> Login(string returnUrl)
    {
        if (!String.IsNullOrEmpty(ErrorMessage))
        {
            ModelState.AddModelError(key: String.Empty, errorMessage:  ErrorMessage);
        }

        returnUrl = returnUrl ?? Url.Content(contentPath: "/");

        // Clear the existing external cookie to ensure a clean login process
        await HttpContext.SignOutAsync(scheme: IdentityConstants.ExternalScheme);

        ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

        ReturnUrl = returnUrl;

        return View(model: new AccountLoginViewModel
        {
            ReturnUrl = ReturnUrl,
            ExternalLogins = ExternalLogins,
            ErrorMessage = ErrorMessage,
            Input = new AccountLoginViewModel.InputAccountLogin()
        });
    }

    [HttpPost]
    public async Task<IActionResult> Login([FromForm] AccountLoginViewModel.InputAccountLogin Input, string returnUrl)
    {
        returnUrl ??= Url.Content(contentPath: "/");

        ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

        if (ModelState.IsValid)
        {
            // var Input = model.Input

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, set lockoutOnFailure: true
            var result = await _signInManager.PasswordSignInAsync(
                userName: Input.Email,
                password: Input.Password,
                isPersistent: Input.RememberMe,
                lockoutOnFailure: true);
            if (result.Succeeded)
            {
                _logger.LogInformation(
                    message: "The user with associated with account {0} has successfully logged in", 
                    args: [Input.Email]);

                return LocalRedirect(localUrl: returnUrl);
            }
            if (result.RequiresTwoFactor)
            {
                return RedirectToPage(
                    pageName: "./LoginWith2gfa", 
                    routeValues: new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
            }
            if (result.IsLockedOut)
            {
                _logger.LogWarning(
                    message: "The user for account {0} is now locked out.",
                    args: [Input.Email]);
                return RedirectToPage(pageName: "./Lockout");
            }
        }

        // If we got this far, something failed, redisplay form.
        return View(model: new AccountLoginViewModel
        {
            ReturnUrl = returnUrl,
            ExternalLogins = ExternalLogins,
            ErrorMessage = ErrorMessage,
            Input = Input
        });
    }
}
