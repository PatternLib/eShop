using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using EShopOnContainers.Identity.Domain;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace EShopOnContainers.Identity.Models;

public class AccountRegisterViewModel
{
    public string ErrorMessage { get; set; }

    public string ReturnUrl { get; set; }

    public IList<AuthenticationScheme> ExternalLogins { get; set; }

    [BindProperty]
    public InputAccountRegister Input {  get; set; }

    public class InputAccountRegister
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(maximumLength: 100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(dataType: DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare(otherProperty: nameof(Password), ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public ApplicationUser User { get; set; }
    }
}
