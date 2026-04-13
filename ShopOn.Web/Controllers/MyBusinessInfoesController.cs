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
    public class MyBusinessInfoesController : Controller
    {
        private readonly EasyInventoryDbContext _db;

        public MyBusinessInfoesController(EasyInventoryDbContext db) { _db = db; }

        // GET: MyBusinessInfoes
        public IActionResult Index()
        {
            return View(_db.MyBusinessInfos.ToList());
        }

        // GET: MyBusinessInfoes/Details/5
        public IActionResult Details(decimal id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            MyBusinessInfo myBusinessInfo = _db.MyBusinessInfos.Find(id);
            if (myBusinessInfo == null)
            {
                return NotFound();
            }
            return View(myBusinessInfo);
        }

        // GET: MyBusinessInfoes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: MyBusinessInfoes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("Id,BizName,BizAddress,Mobile,Email,Website,Tagline")] MyBusinessInfo myBusinessInfo)
        {
            if (ModelState.IsValid)
            {
                _db.MyBusinessInfos.Add(myBusinessInfo);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(myBusinessInfo);
        }

        // GET: MyBusinessInfoes/Edit/5
        public IActionResult Edit(decimal id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            //MyBusinessInfo myBusinessInfo = _db.MyBusinessInfos.Find(id);
            MyBusinessInfo myBusinessInfo = _db.MyBusinessInfos.FirstOrDefault();
            if (myBusinessInfo == null)
            {
                return NotFound();
            }
            return View(myBusinessInfo);
        }

        // POST: MyBusinessInfoes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit([Bind("Id,BizName,BizAddress,Mobile,Email,Website,Tagline")] MyBusinessInfo myBusinessInfo)
        {
            if (ModelState.IsValid)
            {
                _db.Entry(myBusinessInfo).State = EntityState.Modified;
                _db.SaveChanges();
                //return RedirectToAction("Create","SOSR");
                return RedirectToAction("Create", "SOSR", new { IsReturn = "false" });
            }
            return View(myBusinessInfo);
        }

        // GET: MyBusinessInfoes/Delete/5
        public IActionResult Delete(decimal id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            MyBusinessInfo myBusinessInfo = _db.MyBusinessInfos.Find(id);
            if (myBusinessInfo == null)
            {
                return NotFound();
            }
            return View(myBusinessInfo);
        }

        // POST: MyBusinessInfoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(decimal id)
        {
            MyBusinessInfo myBusinessInfo = _db.MyBusinessInfos.Find(id);
            _db.MyBusinessInfos.Remove(myBusinessInfo);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }

    }
}

