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
            var myBusinessInfo = _db.MyBusinessInfos.OrderBy(x => x.Id).FirstOrDefault();
            if (myBusinessInfo == null)
            {
                return RedirectToAction("Create");
            }

            return RedirectToAction("Edit", new { id = myBusinessInfo.Id });
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
            var existingBusinessInfo = _db.MyBusinessInfos.OrderBy(x => x.Id).FirstOrDefault();
            if (existingBusinessInfo != null)
            {
                return RedirectToAction("Edit", new { id = existingBusinessInfo.Id });
            }

            return View(new MyBusinessInfo
            {
                Id = GetNextBusinessInfoId()
            });
        }

        // POST: MyBusinessInfoes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("Id,BizName,BizAddress,Mobile,Email,Website,Tagline")] MyBusinessInfo myBusinessInfo)
        {
            if (_db.MyBusinessInfos.Any())
            {
                var existingBusinessInfo = _db.MyBusinessInfos.OrderBy(x => x.Id).First();
                return RedirectToAction("Edit", new { id = existingBusinessInfo.Id });
            }

            if (ModelState.IsValid)
            {
                _db.MyBusinessInfos.Add(myBusinessInfo);
                _db.SaveChanges();
                return RedirectToAction("Edit", new { id = myBusinessInfo.Id });
            }

            return View(myBusinessInfo);
        }

        // GET: MyBusinessInfoes/Edit/5
        public IActionResult Edit(decimal? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }

            MyBusinessInfo myBusinessInfo = _db.MyBusinessInfos.Find(id);
            if (myBusinessInfo == null)
            {
                myBusinessInfo = _db.MyBusinessInfos.OrderBy(x => x.Id).FirstOrDefault();
            }

            if (myBusinessInfo == null)
            {
                return RedirectToAction("Create");
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
            if (!_db.MyBusinessInfos.Any(x => x.Id == myBusinessInfo.Id))
            {
                return RedirectToAction("Create");
            }

            if (ModelState.IsValid)
            {
                _db.Entry(myBusinessInfo).State = EntityState.Modified;
                _db.SaveChanges();
                //return RedirectToAction("Create","SOSR");
                return RedirectToAction("Create", "SOSR", new { IsReturn = "false" });
            }
            return View(myBusinessInfo);
        }

        private decimal GetNextBusinessInfoId()
        {
            return _db.MyBusinessInfos.DefaultIfEmpty().Max(p => p == null ? 0 : p.Id) + 1;
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

