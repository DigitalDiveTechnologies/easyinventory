using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ShopOn.Web.Controllers;

[AllowAnonymous]
public class NotAllowedController : Controller
{
    public IActionResult UnAuthorized()
    {
        return View();
    }
}
