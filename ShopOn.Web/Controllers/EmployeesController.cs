using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EasyInventory.PgData;
using EasyInventory.PgData.Entities;
using ShopOn.Web.Infrastructure;

namespace ShopOn.Web.Controllers;

public class EmployeesController : Controller
{
    private readonly EasyInventoryDbContext _db;

    public EmployeesController(EasyInventoryDbContext db) => _db = db;

    public IActionResult Edit(int id)
    {
        var currentUser = HttpContext.Session.GetCurrentUser();
        if (currentUser == null)
            return RedirectToAction("Login", "UserManagement");

        id = currentUser.Id;
        var employee = _db.Employees.Find(id);
        if (employee == null)
            return NotFound();
        return View(employee);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit([Bind("Login,Password")] Employee userChanges, IFormCollection fc)
    {
        string? oldPass = fc["OldPassword"];
        string? pass1 = fc["Password1"];
        string? pass2 = fc["Password2"];

        var currentUser = HttpContext.Session.GetCurrentUser();
        if (currentUser == null)
            return RedirectToAction("Login", "UserManagement");

        if (userChanges != null &&
            currentUser.Login == userChanges.Login &&
            currentUser.Password == Encryption.Encrypt(oldPass ?? "", "d3A#") &&
            pass1 == pass2)
        {
            userChanges.Id = currentUser.Id;
            userChanges.Password = Encryption.Encrypt(pass2 ?? "", "d3A#");

            if (ModelState.IsValid)
            {
                _db.Entry(userChanges).State = EntityState.Modified;
                _db.SaveChanges();
                var updated = _db.Employees.Find(currentUser.Id);
                if (updated != null)
                    HttpContext.Session.SetCurrentUser(updated);

                return RedirectToAction("Create", "SOSR");
            }
        }

        ViewBag.Error = "Password does not match";
        return View(userChanges);
    }
}
