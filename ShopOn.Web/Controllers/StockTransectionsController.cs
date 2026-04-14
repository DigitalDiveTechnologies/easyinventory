using ShopOn.Web.Infrastructure;
using ShopOn.Web.Models;
using EasyInventory.PgData.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using EasyInventory.PgData;
using ShopOn.Web.Data;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ShopOn.Web.Controllers
{
    public class StockTransectionsController : Controller
    {
        private readonly EasyInventoryDbContext _db;

        public StockTransectionsController(EasyInventoryDbContext db) { _db = db; }
        //[NoCache]
        public IActionResult Summary()
        {
            var (dtStartDate, dtEndtDate, startDisplay, endDisplay) = Utilities.GetCurrentMonthRange();

            
            ViewBag.Customers = _db.Customers;
           

            ViewBag.StartDate = startDisplay;
            ViewBag.EndDate = endDisplay;

            
            DashboardViewModel dbVM = new DashboardViewModel();
            
            List<SOD> FilteredSaleOrderDetails;// = _db.Customers;
            List<POD> FilteredPurchaseOrderDetails;// = _db.Customers;
            List<Product> FilteredProducts = new List<Product>();
            foreach (Product prod in _db.Products)
            {
                
                FilteredSaleOrderDetails = new List<SOD>();

                foreach (SOD sod in prod.SODs)
                {
                    if (sod.SO.Date >= dtStartDate && sod.SO.Date <= dtEndtDate)
                    {
                        FilteredSaleOrderDetails.Add(sod);
                    }
                }

                //if (FilteredSaleOrderDetails.Count > 0)
                //{
                    //prod.SODs = FilteredSaleOrderDetails;
                    

                //}
                


                /////////////////////////////

                FilteredPurchaseOrderDetails = new List<POD>();

                foreach (POD pod in prod.PODs)
                {
                    if (pod.PO.Date >= dtStartDate && pod.PO.Date <= dtEndtDate)
                    {
                        FilteredPurchaseOrderDetails.Add(pod);
                    }
                }

                //if (FilteredPurchaseOrderDetails.Count > 0)
                //{
                    //prod.PODs = FilteredPurchaseOrderDetails;
                    

                //}
                


                prod.SODs = FilteredSaleOrderDetails;
                prod.PODs = FilteredPurchaseOrderDetails;
                FilteredProducts.Add(prod);



            }

            dbVM.Products = FilteredProducts.OrderBy(x => x.Name).AsQueryable();



            ////////////////////////
           

            //dbVM.SOes = sOes;
            //dbVM.POes = _db.POes;
            
            return View("Summary", dbVM);
            //return View("Dashboard", sOes);

        }
        public IActionResult FilterIndex(string custId, string suppId, string startDate, string endDate)
        {
            var dtStartDate = Utilities.ParseStartDateOrDefaultUtc(startDate);
            var dtEndtDate = Utilities.ParseEndDateOrDefaultUtc(endDate);

            ViewBag.Customers = _db.Customers;

            ViewBag.StartDate = Utilities.FormatBusinessDate(dtStartDate);
            ViewBag.EndDate = Utilities.FormatBusinessDate(dtEndtDate);

            DashboardViewModel dbVM = new DashboardViewModel();
          
          
          
            List<SOD> FilteredSaleOrderDetails;// = _db.Customers;
            List<POD> FilteredPurchaseOrderDetails;// = _db.Customers;
            List<Product> FilteredProducts = new List<Product>();
            foreach (Product prod in _db.Products)
            {

                FilteredSaleOrderDetails = new List<SOD>();

                foreach (SOD sod in prod.SODs)
                {
                    if (sod.SO.Date >= dtStartDate && sod.SO.Date <= dtEndtDate)
                    {
                        FilteredSaleOrderDetails.Add(sod);
                    }
                }

                //if (FilteredSaleOrderDetails.Count > 0)
                //{
                    //prod.SODs = FilteredSaleOrderDetails;


                //}
                


                /////////////////////////////

                FilteredPurchaseOrderDetails = new List<POD>();

                foreach (POD pod in prod.PODs)
                {
                    if (pod.PO.Date >= dtStartDate && pod.PO.Date <= dtEndtDate)
                    {
                        FilteredPurchaseOrderDetails.Add(pod);
                    }
                }

                //if (FilteredPurchaseOrderDetails.Count > 0)
                //{
                    //prod.PODs = FilteredPurchaseOrderDetails;


                //}
               



                prod.SODs = FilteredSaleOrderDetails;
                prod.PODs = FilteredPurchaseOrderDetails;
                FilteredProducts.Add(prod);



            }

            dbVM.Products = FilteredProducts.OrderBy(x => x.Name).AsQueryable();





            return PartialView("_Summary", dbVM);
            //return View("Some thing went wrong");
        }
       
      
    }
}
