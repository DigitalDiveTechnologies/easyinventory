using ShopOn.Web.Infrastructure;
using ShopOn.Web.Models;
using EasyInventory.PgData.Entities;
using System;
using System.Diagnostics;
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
    public class HomeController : Controller
    {
        private readonly EasyInventoryDbContext _db;

        public HomeController(EasyInventoryDbContext db) { _db = db; }
        //[NoCache]
        public IActionResult Index()
        {
            var (dtStartDate, dtEndtDate, startDisplay, endDisplay) = Utilities.GetCurrentMonthRange();

            
            ViewBag.Customers = _db.Customers;
           

            ViewBag.StartDate = startDisplay;
            ViewBag.EndDate = endDisplay;

            
            DashboardViewModel dbVM = new DashboardViewModel();
            ///////////////////////////////////////
            List<SO> FilteredSaleOrders;// = _db.Customers;
            List<Customer> FilteredCustomers = new List<Customer>();
            foreach (Customer cust in _db.Customers)
            {
                FilteredSaleOrders = new List<SO>();
                foreach (SO so in cust.SOes)
                {
                    if (so.Date >= dtStartDate && so.Date <= dtEndtDate)
                    {
                        FilteredSaleOrders.Add(so);
                    }
                }

                cust.SOes = FilteredSaleOrders;
                FilteredCustomers.Add(cust);
            }

            dbVM.Customers = FilteredCustomers.OrderBy(x => x.Name).AsQueryable();
            /////////////////

            List<SOD> FilteredSaleOrderDetails;// = _db.Customers;
            List<Product> FilteredProducts = new List<Product>();
            foreach (Product prod in _db.Products)
            {
                //FilteredSaleOrders = new List<SO>();
                FilteredSaleOrderDetails = new List<SOD>();

                foreach (SOD sod in prod.SODs)
                {
                    if (sod.SO.Date >= dtStartDate && sod.SO.Date <= dtEndtDate)
                    {
                        FilteredSaleOrderDetails.Add(sod);
                    }
                }

                prod.SODs = FilteredSaleOrderDetails;
                FilteredProducts.Add(prod);
            }

            //IQueryable<Product> Products = _db.Products;
            dbVM.Products = FilteredProducts.OrderBy(x => x.Name).AsQueryable();


            //dbVM.SOes = sOes;
            //dbVM.POes = _db.POes;
            
            return View("Dashboard", dbVM);
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
          
          
            /////////////////////////////////////////////////////////////////////////////
            List<SO> FilteredSaleOrders;// = _db.Customers;
            List<Customer> FilteredCustomers = new List<Customer>();
            foreach (Customer cust in _db.Customers)
            {
                FilteredSaleOrders = new List<SO>();
                foreach (SO so in cust.SOes)
                {
                    if (so.Date >= dtStartDate && so.Date <= dtEndtDate)
                    {
                        FilteredSaleOrders.Add(so);
                    }
                }

                cust.SOes = FilteredSaleOrders;
                FilteredCustomers.Add(cust);
            }

            dbVM.Customers = FilteredCustomers.AsQueryable();

            ///////////////////////////////////////////////////////////////////////////

            List<SOD> FilteredSaleOrderDetails;// = _db.Customers;
            List<Product> FilteredProducts = new List<Product>();
            foreach (Product prod in _db.Products)
            {
                //FilteredSaleOrders = new List<SO>();
                FilteredSaleOrderDetails = new List<SOD>();

                foreach (SOD sod in prod.SODs)
                {
                    if (sod.SO.Date >= dtStartDate && sod.SO.Date <= dtEndtDate)
                    {
                        FilteredSaleOrderDetails.Add(sod);
                    }
                }

                prod.SODs = FilteredSaleOrderDetails;
                FilteredProducts.Add(prod);
            }

            //IQueryable<Product> Products = _db.Products;
            dbVM.Products = FilteredProducts.AsQueryable();




            return PartialView("_Dashboard", dbVM);
            //return View("Some thing went wrong");
        }
        public IActionResult CustomerWiseSale(int custId, string custName)
        {
            var (dtStartDate, dtEndtDate, startDisplay, endDisplay) = Utilities.GetCurrentMonthRange();

            ViewBag.CustomerId = custId;
            ViewBag.CustName = custName;
            //ViewBag.SupplierName = supplierName;//_db.Products.FirstOrDefault(x => x.Id == productId).Name;
            ViewBag.Customers = _db.Customers;
            //01-Jan-2019

            ViewBag.StartDate = startDisplay;
            ViewBag.EndDate = endDisplay;

            IQueryable<SO> sOes = _db.SOes;//.Include(s => s.Customer);
            sOes = sOes.Where(x => x.CustomerId == custId && x.Date >= dtStartDate && x.Date <= dtEndtDate).OrderBy(i => i.SOSerial).AsQueryable();
            //foreach (SO itm in sOes)
            //{
            //    //itm.Id = Encryption.Encrypt(itm.Id, "BZNS");
            //    itm.Id = string.Join("-", ASCIIEncoding.ASCII.GetBytes(Encryption.Encrypt(itm.Id, "BZNS")));
            //}



            //      unitOfWork.deviceInstanceRepository.Get()
            //.GroupBy(w => new
            //{
            //    DeviceId = w.DeviceId,
            //    Device = w.Device.Name,
            //    Manufacturer = w.Device.Manufacturer,
            //})
            //.Select(s => new
            //{
            //    DeviceId = s.Key.DeviceId,
            //    Name = s.Key.Device,
            //    Manufacturer = s.Key.Manufacturer.Name,
            //    Quantity = s.Sum(x => x.Quantity)
            //})






            return View("Dashboard", sOes);

            //return View("CustomerWiseSale", sOes.OrderBy(i => i.Date).ToList());
        }
        public IActionResult FilterCustomerWiseSale(string custId, string suppId, string startDate, string endDate)
        {
            var hasCustomerId = !string.IsNullOrWhiteSpace(custId);
            var dtStartDate = Utilities.ParseStartDateOrDefaultUtc(startDate);
            var dtEndtDate = Utilities.ParseEndDateOrDefaultUtc(endDate);
            IQueryable<SO> selectedSOes = _db.SOes;

            if (hasCustomerId)
            {
                var intCustId = Int32.Parse(custId);
                selectedSOes = selectedSOes.Where(so => so.CustomerId == intCustId);
            }

            selectedSOes = selectedSOes.Where(so => so.Date >= dtStartDate && so.Date <= dtEndtDate);


            //foreach (SO itm in selectedSOes)
            //{
            //    //itm.Id = Encryption.Encrypt(itm.Id, "BZNS");
            //    itm.Id = string.Join("-", ASCIIEncoding.ASCII.GetBytes(Encryption.Encrypt(itm.Id, "BZNS")));
            //}
            //GetTotalBalance(ref selectedSOes);
            //return PartialView("_SelectedSOSR", selectedSOes.OrderByDescending(i => i.Date).ToList());
            //_ProfitGainFromSupplier
            return PartialView("_Dashboard", selectedSOes.OrderBy(i => i.SOSerial).ToList());
            //return View("Some thing went wrong");
        }
        public IActionResult ProductWiseSale(int productId)
        {
            var (dtStartDate, dtEndtDate, startDisplay, endDisplay) = Utilities.GetCurrentMonthRange();
            ViewBag.ProductId = productId;
            ViewBag.ProductName = _db.Products.FirstOrDefault(x => x.Id == productId).Name;
            ViewBag.Customers = _db.Customers;
            ViewBag.StartDate = startDisplay;
            ViewBag.EndDate = endDisplay;

            IQueryable<SO> sOes = _db.SOes;//.Include(s => s.Customer);

            //sOes = _db.SOes.Where(x => x.SODs.Where(y => y.ProductId == productId));

            List<SOD> lstSODs = _db.SODs.Where(x => x.ProductId == productId).ToList();
            List<SO> lstSlectedSO = new List<SO>();
            foreach (SOD lsod in lstSODs)
            {
                //do not add if already added
                if (lstSlectedSO.Where(x => x.Id == lsod.SOId).FirstOrDefault() == null)
                {
                    lstSlectedSO.Add(lsod.SO);
                }
            }

            sOes = lstSlectedSO.Where(x => x.Date >= dtStartDate && x.Date <= dtEndtDate).AsQueryable();
            foreach (SO itm in sOes)
            {
                //itm.Id = Encryption.Encrypt(itm.Id, "BZNS");
                itm.Id = string.Join("-", ASCIIEncoding.ASCII.GetBytes(Encryption.Encrypt(itm.Id, "BZNS")));
            }

            return View("ProductWiseSale", sOes.OrderBy(i => i.SOSerial).ToList());
        }
        public IActionResult FilterProductWiseSale(string prodId, string custId, string suppId, string startDate, string endDate)
        {
            int intProdId;
            intProdId = Int32.Parse(prodId);
            IQueryable<SO> sOes = _db.SOes;//.Include(s => s.Customer);

            //sOes = _db.SOes.Where(x => x.SODs.Where(y => y.ProductId == productId));

            List<SOD> lstSODs = _db.SODs.Where(x => x.ProductId == intProdId).ToList();
            List<SO> lstSlectedSO = new List<SO>();
            foreach (SOD lsod in lstSODs)
            {
                //do not add if already added
                if (lstSlectedSO.Where(x => x.Id == lsod.SOId).FirstOrDefault() == null)
                {
                    lstSlectedSO.Add(lsod.SO);
                }
            }
            sOes = lstSlectedSO.AsQueryable();
            //sOes = lstSlectedSO.Where(x => x.Date >= dtStartDate && x.Date <= dtEndtDate).AsQueryable();

            /////////////////////////////////////////////////////////////////////////////
            //IQueryable<SO> sOes = _db.SOes;//.Include(s => s.Customer);
            //sOes = sOes.Where(x => x.CustomerId == custId && x.Date >= dtStartDate && x.Date <= dtEndtDate).OrderBy(i => i.Date).OrderBy(i => i.SOSerial).AsQueryable();


            var hasCustomerId = !string.IsNullOrWhiteSpace(custId);
            var dtStartDate = Utilities.ParseStartDateOrDefaultUtc(startDate);
            var dtEndtDate = Utilities.ParseEndDateOrDefaultUtc(endDate);
            IQueryable<SO> selectedSOes = sOes.Where(so => so.Date >= dtStartDate && so.Date <= dtEndtDate);

            if (hasCustomerId)
            {
                var intCustId = Int32.Parse(custId);
                selectedSOes = selectedSOes.Where(so => so.CustomerId == intCustId);
            }


            //foreach (SO itm in selectedSOes)
            //{
            //    //itm.Id = Encryption.Encrypt(itm.Id, "BZNS");
            //    itm.Id = string.Join("-", ASCIIEncoding.ASCII.GetBytes(Encryption.Encrypt(itm.Id, "BZNS")));
            //}
            //GetTotalBalance(ref selectedSOes);
            //return PartialView("_SelectedSOSR", selectedSOes.OrderByDescending(i => i.Date).ToList());
            //_ProfitGainFromSupplier
            return PartialView("_CustomerWiseSale", selectedSOes.OrderBy(i => i.SOSerial).ToList());
            //return View("Some thing went wrong");
        }

        public IActionResult About()
        {
            ViewBag.Message = "ShopON";

            return View();
        }

        public IActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
