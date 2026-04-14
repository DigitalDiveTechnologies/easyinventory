using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using EasyInventory.PgData;
using ShopOn.Web.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Reporting.NETCore;
using ShopOn.Web.Infrastructure;
using ShopOn.Web.Models;
using EasyInventory.PgData.Entities;

namespace ShopOn.Web.Controllers
{
    public class DashboardController : Controller
    {
        private readonly EasyInventoryDbContext _db;

        public DashboardController(EasyInventoryDbContext db) { _db = db; }
        //private IQueryable<Product> _dbFilteredProducts;
        //private IQueryable<Supplier> _dbFilteredSuppliers;
        //private IQueryable<Account> _dbFilteredAccounts;
        //private IQueryable<PO> _dbFilteredPO;
        //private IQueryable<SO> _dbFilteredSO;

        //private IQueryable<Employee> _dbFilteredEmployees;
        //private IQueryable<Department> _dbFilteredDepartments;
        //private IQueryable<Customer> _dbFilteredCustomers;


        //protected override void Initialize(RequestContext requestContext)
        //{
        //    //Employee employee1 = TempData["mydata"] as Employee;
        //    //Employee employee = ViewBag.data;
        //    base.Initialize(requestContext);
        //    //decimal BusinessId = decimal.Parse(this.ControllerContext.RouteData.Values["CurrentBusiness"].ToString());
        //    Business CurrentBusiness = (Business)Session["CurrentBusiness"];
        //    _dbFilteredSuppliers = _db.Suppliers.AsQueryable().Where(x => x.bizId == CurrentBusiness.Id);
        //    _dbFilteredProducts = _db.Products.AsQueryable().Where(x => x.Supplier.bizId == CurrentBusiness.Id);
        //    _dbFilteredPO = _db.POes.AsQueryable().Where(x => x.Supplier.bizId == CurrentBusiness.Id);
        //    _dbFilteredAccounts = _db.Accounts.AsQueryable().Where(x => x.bizId == CurrentBusiness.Id);
        //    _dbFilteredSO = _db.SOes.AsQueryable().Where(x => x.Employee.Department.bizId == CurrentBusiness.Id);


        //    _dbFilteredEmployees = _db.Employees.AsQueryable().Where(x => x.Department.bizId == CurrentBusiness.Id);
        //    _dbFilteredDepartments = _db.Departments.AsQueryable().Where(x => x.bizId == CurrentBusiness.Id);
        //    _dbFilteredCustomers = _db.Customers.AsQueryable().Where(x => x.bizId == CurrentBusiness.Id);
        //    _dbFilteredProducts = _db.Products.AsQueryable().Where(x => x.Supplier.bizId == CurrentBusiness.Id);

        //}

        // GET: Dashboard
        public IActionResult Index()
        {
            decimal SaleOrderCount  = _db.SOes.Count();
            ViewBag.SOCount = SaleOrderCount;

            decimal SaleOrderAmount = 0;
            SaleOrderAmount = _db.SOes.Sum(x => x.BillAmount);
            //ViewBag.Sales = SaleOrderAmount;
            ViewBag.SOAmount = SaleOrderAmount;

            decimal PurchaseOrderCount = _db.POes.Count();
            ViewBag.POCount = PurchaseOrderCount;

             decimal PurchaseOrderAmount = 0;

            PurchaseOrderAmount = _db.POes.Sum(x => x.BillAmount);
             ViewBag.POAmount = PurchaseOrderAmount;

            decimal Profit = (decimal)(_db.SOes.Sum(x => x.Profit) ?? 0);
            //ViewBag.Profit = (decimal)(SaleOrderCount - PurchaseOrderCount);

            ViewBag.Profit = Profit;


            ViewBag.Products = _db.Products.Count();
            
            ViewBag.Suppliers = _db.Suppliers.Count();
            
            ViewBag.Customers = _db.Customers.Count();
            
            ViewBag.Employees = _db.Employees.Count();

            var recentSales = _db.SOes
                .Include(x => x.Customer)
                .Where(x => x.SaleReturn == false || x.SaleReturn == null)
                .OrderByDescending(x => x.Date ?? DateTime.MinValue)
                .Take(5)
                .ToList();
            ViewBag.RecentSales = recentSales;

            var lowStockQuery = _db.Products.Where(p =>
                p.Stock.HasValue &&
                (p.ReOrder.HasValue ? p.Stock <= p.ReOrder.Value : p.Stock <= 0));
            ViewBag.LowStockCount = lowStockQuery.Count();
            ViewBag.LowStockProducts = lowStockQuery
                .OrderBy(p => p.Stock)
                .Take(5)
                .ToList();

            return View();
        }
    }
}
