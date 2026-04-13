using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ShopOn.Web.Controllers;

[AllowAnonymous]
public class AccountController : Controller
{
    public IActionResult Login(string? returnUrl = null)
    {
        return RedirectToAction("Index", "Home");
    }

    public IActionResult Register() => RedirectToAction("Index", "Home");

    public IActionResult ForgotPassword() => RedirectToAction("Index", "Home");

    public IActionResult ResetPassword(string? code = null) => RedirectToAction("Index", "Home");

    public IActionResult ExternalLoginFailure() => RedirectToAction("Index", "Home");

    public IActionResult VerifyCode() => RedirectToAction("Index", "Home");

    public IActionResult SendCode() => RedirectToAction("Index", "Home");

    public IActionResult ForgotPasswordConfirmation() => RedirectToAction("Index", "Home");

    public IActionResult ResetPasswordConfirmation() => RedirectToAction("Index", "Home");

    public IActionResult ConfirmEmail() => RedirectToAction("Index", "Home");
}
