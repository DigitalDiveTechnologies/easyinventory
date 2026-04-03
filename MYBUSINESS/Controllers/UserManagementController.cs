using System.Linq;
using System.Web.Mvc;
using MYBUSINESS.CustomClasses;
using MYBUSINESS.Models;

namespace MYBUSINESS.Controllers
{
    [AllowAnonymous]
    public class UserManagementController : Controller
    {
        private readonly BusinessContext _db = new BusinessContext();

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string Login, string Password)
        {
            if (string.IsNullOrWhiteSpace(Login) || string.IsNullOrWhiteSpace(Password))
            {
                ViewBag.Error = "Login and password are required.";
                return View();
            }

            var encrypted = Encryption.Encrypt(Password, "d3A#");
            var employee = _db.Employees.FirstOrDefault(e => e.Login == Login && e.Password == encrypted);
            if (employee == null)
            {
                ViewBag.Error = "Invalid login or password.";
                return View();
            }

            Session["CurrentUser"] = employee;
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public ActionResult Logout()
        {
            Session["CurrentUser"] = null;
            return RedirectToAction("Login", "UserManagement");
        }
    }
}
