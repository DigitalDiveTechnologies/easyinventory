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
    public class ProductsController : Controller
    {
        private readonly EasyInventoryDbContext _db;

        public ProductsController(EasyInventoryDbContext db) { _db = db; }

        // GET: Products
        public IActionResult Index()
        {
            ViewBag.Suppliers = _db.Suppliers;
            return View(_db.Products.ToList());
        }

        public IActionResult SearchData(string suppId)
        {
            if (suppId.Trim() == string.Empty)
            {

                return PartialView("_SelectedProducts", _db.Products.OrderBy(i => i.Id).ToList());

            }
            else
            {
                int intSuppId = Int32.Parse(suppId.Trim());

                IQueryable<Product> selectedProducts = null;
                selectedProducts = _db.Products.Where(p => p.SupplierId == intSuppId);
                return PartialView("_SelectedProducts", selectedProducts.OrderBy(i => i.Id).ToList());

            }

        }

        //// GET: Products/Details/5
        //public IActionResult Details(decimal id)
        //{
        //    if (id == null)
        //    {
        //        return BadRequest();
        //    }
        //    Product product = _db.Products.Find(id);
        //    if (product == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(product);
        //}

        // GET: Products/Create
        public IActionResult Create()
        {
            //int maxId = _db.Products.Max(p => p.Id);
            int maxId = _db.Products.DefaultIfEmpty().Max(p => p == null ? 0 : p.Id);
            maxId += 1;
            ViewBag.SuggestedId = maxId;
            ViewBag.Suppliers = _db.Suppliers;
            Product prod = new Product();

            prod.PurchasePrice = 0;
            prod.SalePrice = 0;
            prod.Stock = 0;

            prod.Saleable = true;
            return View(prod);
        }

        // POST: Products/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("Id,Name,PurchasePrice,SalePrice,Stock,SupplierId,Saleable,PerPack")] Product product)
        {
            if (product.Stock == null)
            {
                product.Stock = 0;
            }

            if (product.PerPack == null || product.PerPack == 0)
            {
                product.PerPack = 1;
            }

            product.Stock = product.Stock * product.PerPack;

            if (ModelState.IsValid)
            {
                _db.Products.Add(product);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Suppliers = _db.Suppliers;
            return View(product);
        }

        // GET: Products/Edit/5
        public IActionResult Edit(decimal id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            Product product = _db.Products.Find(id);
            product.Stock = product.Stock / product.PerPack;
            ViewBag.SuppName = product.Supplier.Name;
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit([Bind("Id,Name,PurchasePrice,SalePrice,Stock,Saleable,SupplierId,PerPack")] Product product)
        {
            //Product prd = _db.Products.Where(x => x.Id == product.Id).FirstOrDefault();
            //product.SuppId = prd.SuppId;
            if (product.Stock == null)
            {
                product.Stock = 0;
            }

            if (product.PerPack == null || product.PerPack == 0)
            {
                product.PerPack = 1;
            }

            product.Stock = product.Stock * product.PerPack;


            if (ModelState.IsValid)
            {

                _db.Entry(product).State = EntityState.Modified;
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(product);
        }

        //// GET: Products/Delete/5
        //public IActionResult Delete(decimal id)
        //{
        //    if (id == null)
        //    {
        //        return BadRequest();
        //    }
        //    Product product = _db.Products.Find(id);
        //    if (product == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(product);
        //}

        //// POST: Products/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public IActionResult DeleteConfirmed(decimal id)
        //{
        //    Product product = _db.Products.Find(id);
        //    _db.Products.Remove(product);
        //    _db.SaveChanges();
        //    return RedirectToAction("Index");
        //}

    }
}

