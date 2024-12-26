using System.Text;
using System.Text.Encodings.Web;
using EShopOnContainers.Identity.Domain;
using EShopOnContainers.Identity.Extensions;
using EShopOnContainers.Identity.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;

namespace EShopOnContainers.Identity.Controllers;

public class AccountController : Controller
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IUserStore<ApplicationUser> _userStore;
    private readonly IUserEmailStore<ApplicationUser> _userEmailStore;
    private readonly ILogger<AccountController> _logger;
    private readonly IEmailSender _emailSender;

    public AccountController(SignInManager<ApplicationUser> signInManager, ILogger<AccountController> logger, UserManager<ApplicationUser> userManager, IUserStore<ApplicationUser> userStore, IEmailSender emailSender)
    {
        _signInManager = signInManager;
        _logger = logger;
        _userManager = userManager;
        _userStore = userStore;
        _userEmailStore = GetEmailStore();
        _emailSender = emailSender;
    }

    [TempData]
    public string ErrorMessage { get; set; }

    [TempData]
    public string StatusMessage { get; set; }

    public string ReturnUrl { get; set; }

    public IList<AuthenticationScheme> ExternalLogins { get; set; }

    private IUserEmailStore<ApplicationUser> GetEmailStore()
    {
        if (!_userManager.SupportsUserEmail)
        {
            throw new NotSupportedException("The default UI requires a user store with email support.");
        }

        return (IUserEmailStore<ApplicationUser>)_userStore;
    }


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

    [HttpGet]
    public async Task<IActionResult> Register(string returnUrl = null)
    {
        return View(model: new AccountRegisterViewModel {
            ReturnUrl = returnUrl,
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList(),
            ErrorMessage = ErrorMessage,
            Input = new AccountRegisterViewModel.InputAccountRegister()
        });
    }

    [HttpPost]
    public async Task<IActionResult> Register([FromForm] AccountRegisterViewModel.InputAccountRegister Input, string returnUrl = null)
    {
        returnUrl ??= Url.Content(contentPath: "/");

        if (ModelState.IsValid)
        {
            var user = Input.CreateUserExtension();

            await _userStore.SetUserNameAsync(user: user, userName: Input.Email, cancellationToken: CancellationToken.None);
            await _userEmailStore.SetEmailAsync(user: user, email: Input.Email, cancellationToken: CancellationToken.None);
            var result = await _userManager.CreateAsync(user: user, password: Input.Password);

            if (result.Succeeded)
            {
                _logger.LogInformation(message: "User created a new account with email '{0}'.", args: [Input.Email]);

                var userId = await _userManager.GetUserIdAsync(user: user);
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user: user);
                code = WebEncoders.Base64UrlEncode(input: Encoding.UTF8.GetBytes(chars: code.ToArray()));

                var callbackUrl = Url.Action(
                    action: "ConfirmEmail",
                    controller: "Account",
                    values: new { userId = userId, code = code, returnUrl = returnUrl },
                    protocol: Request.Scheme);

                await _emailSender.SendEmailAsync(
                    email: Input.Email, 
                    subject: "Confirm your email",
                    htmlMessage: String.Format(
                        format: "Please confirm your account by <a href='{0}'>clicking here</a>.",
                        args: [HtmlEncoder.Default.Encode(value: callbackUrl)]));

                if (_userManager.Options.SignIn.RequireConfirmedAccount)
                {
                    return RedirectToAction(
                        actionName: "ConfirmEmail",
                        controllerName: "Account",
                        routeValues: new { email = Input.Email, returnUrl = returnUrl });
                }
                else
                {
                    await _signInManager.SignInAsync(user: user, isPersistent: false);
                    return LocalRedirect(localUrl: returnUrl);
                }
            }


            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(key: String.Empty, errorMessage: error.Description);
            }
        }

        // If we got this far, something filed, redisplay form
        return View(model: new AccountRegisterViewModel
        {
            ReturnUrl = returnUrl,
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList(),
            ErrorMessage = ErrorMessage,
            Input = Input
        });
    }

    [HttpGet]
    public async Task<IActionResult> RegisterConfirmation(string email, string returnUrl = null)
    {
        if (email == null)
        {
            return RedirectToAction(actionName: "Login", controllerName: "Account");
        }

        returnUrl = returnUrl ?? Url.Content(contentPath: "~/");

        var user = await _userManager.FindByEmailAsync(email:  email);

        if (user == null)
        {
            return NotFound(value: $"Unable to load user with email '{email}'.");
        }

        bool displayConfirmAccountLink = user != null;
        string emailConfirmationUrl = "";

        if (displayConfirmAccountLink)
        {
            var userId = await _userManager.GetUserIdAsync(user: user);
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user: user);
            code = WebEncoders.Base64UrlEncode(input: Encoding.UTF8.GetBytes(chars: code.ToArray()));
            emailConfirmationUrl = Url.Action(
                action: "ConfirmEmail",
                controller: "Account",
                values: new { userId = userId, code = code, returnUrl = returnUrl },
                protocol: Request.Scheme)!;
        }


        return View(model: (email, displayConfirmAccountLink, emailConfirmationUrl));
    }

    [HttpGet]
    public async Task<IActionResult> ConfirmEmail(string userId, string code, string returnUrl = null)
    {
        returnUrl ??= Url.Content(contentPath: "~/");

        if (userId == null || code == null)
        {
            return RedirectToAction(actionName: "Login", controllerName: "Account", routeValues: new { returnUrl =  returnUrl });
        }

        var user = await _userManager.FindByIdAsync(userId: userId);

        if (user == null)
        {
            return NotFound(value: $"unable to load user with ID '{userId}'.");
        }

        code = Encoding.UTF8.GetString(bytes: WebEncoders.Base64UrlDecode(input: code));

        var result = await _userManager.ConfirmEmailAsync(user: user, token: code);

        StatusMessage = result.Succeeded
            ? "Thank you for confirming your email."
            : "Error confirming your email";

        return View(model: StatusMessage);
    }
}
