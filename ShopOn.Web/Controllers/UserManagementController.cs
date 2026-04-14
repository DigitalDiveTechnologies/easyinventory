using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using EasyInventory.PgData;
using ShopOn.Web.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using ShopOn.Web.Infrastructure;
using ShopOn.Web.Models;
using EasyInventory.PgData.Entities;

namespace ShopOn.Web.Controllers
{
    [AllowAnonymous]
    public class UserManagementController : Controller
    {
        private readonly EasyInventoryDbContext _db;

        public UserManagementController(EasyInventoryDbContext db) { _db = db; }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(string Login, string Password, string? returnUrl = null)
        {
            if (string.IsNullOrWhiteSpace(Login) || string.IsNullOrWhiteSpace(Password))
            {
                ViewBag.Error = "Login and password are required.";
                ViewBag.ReturnUrl = returnUrl;
                return View();
            }

            var encrypted = Encryption.Encrypt(Password, "d3A#");
            var employee = _db.Employees.FirstOrDefault(e => e.Login == Login && e.Password == encrypted);
            if (employee == null)
            {
                ViewBag.Error = "Invalid login or password.";
                ViewBag.ReturnUrl = returnUrl;
                return View();
            }

            HttpContext.Session.SetCurrentUser(employee);
            if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Index", "Dashboard");
        }

        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.SetCurrentUser(null);
            return RedirectToAction("Login", "UserManagement");
        }
    }
}

