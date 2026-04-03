using System.Web.Mvc;

namespace MYBUSINESS.Controllers
{
    [Authorize]
    public class ManageController : Controller
    {
        public ActionResult Index()
        {
            return RedirectToAction("Index", "Home");
        }
    }
}
