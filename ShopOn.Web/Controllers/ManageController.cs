using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ShopOn.Web.Controllers;

[AllowAnonymous]
public class ManageController : Controller
{
    public IActionResult Index()
    {
        return RedirectToAction("Index", "Home");
    }
}
