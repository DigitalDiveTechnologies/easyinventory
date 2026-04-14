using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using EasyInventory.PgData;
using ShopOn.Web.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using ShopOn.Web.Models;
using EasyInventory.PgData.Entities;

namespace ShopOn.Web.Controllers
{
    public class CustomersController : Controller
    {
        private readonly EasyInventoryDbContext _db;

        public CustomersController(EasyInventoryDbContext db) { _db = db; }

        // GET: Customers
        public IActionResult Index(string id)
        {

            return View(_db.Customers.ToList());
        }

        // GET: Customers/Details/5
        public IActionResult Details(decimal id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            Customer customer = _db.Customers.Find(id);
            if (customer == null)
            {
                return NotFound();
            }
            return View(customer);
        }

        // GET: Customers/Create
        public IActionResult Create()
        {
            return View(new Customer
            {
                Id = GetNextCustomerId(),
                Balance = 0
            });
        }

        // POST: Customers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("Id,Name,Address,Balance")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                if (customer.Balance==null)
                {
                    customer.Balance = 0;
                }
                _db.Customers.Add(customer);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }

            //if ((TempData["Controller"]).ToString() == "SOSR" && (TempData["Action"]).ToString() == "Create")
            //{
            //    return RedirectToAction("Create", "SOSR");

            //}
            //else
            //{
            //    return View(customer);
            //}
            if (customer.Id == 0)
            {
                customer.Id = GetNextCustomerId();
            }

            if (customer.Balance == null)
            {
                customer.Balance = 0;
            }

            return View(customer);
        }

        private int GetNextCustomerId()
        {
            return _db.Customers.DefaultIfEmpty().Max(p => p == null ? 0 : p.Id) + 1;
        }

        // GET: Customers/Edit/5
        public IActionResult Edit(decimal id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            Customer customer = _db.Customers.Find(id);
            if (customer == null)
            {
                return NotFound();
            }
            return View(customer);
        }

        // POST: Customers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit([Bind("Id,Name,Address,Balance")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                
                _db.Entry(customer).State = EntityState.Modified;
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(customer);
        }

        //// GET: Customers/Delete/5
        //public IActionResult Delete(decimal id)
        //{
        //    if (id == null)
        //    {
        //        return BadRequest();
        //    }
        //    Customer customer = _db.Customers.Find(id);
        //    if (customer == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(customer);
        //}

        //// POST: Customers/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public IActionResult DeleteConfirmed(decimal id)
        //{
        //    Customer customer = _db.Customers.Find(id);
        //    _db.Customers.Remove(customer);
        //    _db.SaveChanges();
        //    return RedirectToAction("Index");
        //}

    }
}

