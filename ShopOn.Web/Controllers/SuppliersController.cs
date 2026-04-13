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
    public class SuppliersController : Controller
    {
        private readonly EasyInventoryDbContext _db;

        public SuppliersController(EasyInventoryDbContext db) { _db = db; }

        // GET: Suppliers
        public IActionResult Index(string id)
        {
           
            return View(_db.Suppliers.ToList());
        }

        // GET: Suppliers/Details/5
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            Supplier supplier = _db.Suppliers.Find(id);
            if (supplier == null)
            {
                return NotFound();
            }
            return View(supplier);
        }

        // GET: Suppliers/Create
        public IActionResult Create()
        {
            int maxId = _db.Suppliers.DefaultIfEmpty().Max(p => p == null ? 0 : p.Id);
            maxId += 1;
            ViewBag.SuggestedNewSuppId = maxId;
            return View();
        }

        // POST: Suppliers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("Id,Name,Address,Balance")] Supplier supplier)
        {
            if (ModelState.IsValid)
            {
                if (supplier.Balance == null)
                {
                    supplier.Balance = 0;
                }
                _db.Suppliers.Add(supplier);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(supplier);
        }

        // GET: Suppliers/Edit/5
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            Supplier supplier = _db.Suppliers.Find(id);
            if (supplier == null)
            {
                return NotFound();
            }
            return View(supplier);
        }

        // POST: Suppliers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit([Bind("Id,Name,Address,Balance")] Supplier supplier)
        {
            if (ModelState.IsValid)
            {
                _db.Entry(supplier).State = EntityState.Modified;
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(supplier);
        }

        //// GET: Suppliers/Delete/5
        //public IActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return BadRequest();
        //    }
        //    Supplier supplier = _db.Suppliers.Find(id);
        //    if (supplier == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(supplier);
        //}

        //// POST: Suppliers/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public IActionResult DeleteConfirmed(int id)
        //{
        //    Supplier supplier = _db.Suppliers.Find(id);
        //    _db.Suppliers.Remove(supplier);
        //    _db.SaveChanges();
        //    return RedirectToAction("Index");
        //}

    }
}

