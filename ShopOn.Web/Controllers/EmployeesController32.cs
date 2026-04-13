using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EasyInventory.PgData;
using EasyInventory.PgData.Entities;

namespace ShopOn.Web.Controllers;

[Route("Employees 32/[action]/{id?}")]
public class Employees32Controller : Controller
{
    private readonly EasyInventoryDbContext _db;

    public Employees32Controller(EasyInventoryDbContext db) => _db = db;

    private const string V = "~/Views/Employees 32";

    public IActionResult Index()
    {
        var employees = _db.Employees.Include(e => e.Department);
        return View($"{V}/Index.cshtml", employees.ToList());
    }

    public IActionResult Details(int? id)
    {
        if (id == null)
            return BadRequest();
        var employee = _db.Employees.Find(id);
        if (employee == null)
            return NotFound();
        return View($"{V}/Details.cshtml", employee);
    }

    public IActionResult Create()
    {
        ViewBag.DepartmentId = new SelectList(_db.Departments, "Id", "Name");
        return View($"{V}/Create.cshtml");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create([Bind("Id,FirstName,LastName,Gender,Login,Password,Email,EmployeeTypeId,RightId,RankId,DepartmentId,Designation,Probation,RegistrationDate,Casual,Earned,IsActive,CreateDate,UpdateDate")] Employee employee)
    {
        if (ModelState.IsValid)
        {
            _db.Employees.Add(employee);
            _db.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        ViewBag.DepartmentId = new SelectList(_db.Departments, "Id", "Name", employee.DepartmentId);
        return View($"{V}/Create.cshtml", employee);
    }

    public IActionResult Edit(int? id)
    {
        if (id == null)
            return BadRequest();
        var employee = _db.Employees.Find(id);
        if (employee == null)
            return NotFound();
        ViewBag.DepartmentId = new SelectList(_db.Departments, "Id", "Name", employee.DepartmentId);
        return View($"{V}/Edit.cshtml", employee);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit([Bind("Id,FirstName,LastName,Gender,Login,Password,Email,EmployeeTypeId,RightId,RankId,DepartmentId,Designation,Probation,RegistrationDate,Casual,Earned,IsActive,CreateDate,UpdateDate")] Employee employee)
    {
        if (ModelState.IsValid)
        {
            _db.Entry(employee).State = EntityState.Modified;
            _db.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        ViewBag.DepartmentId = new SelectList(_db.Departments, "Id", "Name", employee.DepartmentId);
        return View($"{V}/Edit.cshtml", employee);
    }

    public IActionResult Delete(int? id)
    {
        if (id == null)
            return BadRequest();
        var employee = _db.Employees.Find(id);
        if (employee == null)
            return NotFound();
        return View($"{V}/Delete.cshtml", employee);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(int id)
    {
        var employee = _db.Employees.Find(id);
        if (employee != null)
        {
            _db.Employees.Remove(employee);
            _db.SaveChanges();
        }
        return RedirectToAction(nameof(Index));
    }
}
