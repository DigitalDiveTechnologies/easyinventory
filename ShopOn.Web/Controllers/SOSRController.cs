using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Text;
using Microsoft.AspNetCore.Hosting;
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
using System.IO;
using EasyInventory.PgData.Entities;

namespace ShopOn.Web.Controllers
{

    //[OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
    //[NoCache]
    public class SOSRController : Controller
    {
        private readonly EasyInventoryDbContext _db;
        private readonly IWebHostEnvironment _env;

        public SOSRController(EasyInventoryDbContext db, IWebHostEnvironment env) { _db = db; _env = env; }

        // GET: SOes
        public IActionResult Index()
        {
            //EnterProfit();
            DateTime PKDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time"));
            var dtStartDate = new DateTime(PKDate.Year, PKDate.Month, 1);
            var dtEndtDate = dtStartDate.AddMonths(1).AddSeconds(-1);

            IQueryable<SO> sOes = _db.SOes.Where(x => x.Date >= dtStartDate && x.Date <= dtEndtDate).Include(s => s.Customer);
            //sOes = sOes.Where(x => x.Date >= dtStartDate && x.Date <= dtEndtDate);
            //sOes.ForEachAsync(m => m.Id = Encryption.Encrypt(m.Id, "BZNS"));
            //var sOes = _db.SOes.Where(s => s.SaleReturn == false);
            GetTotalBalance(ref sOes);
            Dictionary<int, int> LstMaxSerialNo = new Dictionary<int, int>();
            int thisSerial = 0;
            foreach (SO itm in sOes)
            {
                thisSerial = (int)itm.Customer.SOes.Max(x => x.SOSerial);

                if (!LstMaxSerialNo.ContainsKey((int)itm.CustomerId))
                {
                    LstMaxSerialNo.Add(itm.Customer.Id, thisSerial);
                }


                //itm.Id = Encryption.Encrypt(itm.Id, "BZNS");
                itm.Id = string.Join("-", ASCIIEncoding.ASCII.GetBytes(Encryption.Encrypt(itm.Id, "BZNS")));
            }

            ViewBag.LstMaxSerialno = LstMaxSerialNo;
            ViewBag.Customers = _db.Customers;
            ViewBag.StartDate = dtStartDate.ToString("dd-MMM-yyyy");
            ViewBag.EndDate = dtEndtDate.ToString("dd-MMM-yyyy");
            return View(sOes.OrderByDescending(i => i.Date).ToList());
        }
        //public IActionResult SearchData(string custName, DateTime startDate, DateTime endDate)

        //public IActionResult SearchData(string custName, string startDate, string endDate)
        public IActionResult SearchData(string custId, string startDate, string endDate)
        {

            int intCustId;
            DateTime dtStartDate;
            DateTime dtEndtDate;
            IQueryable<SO> selectedSOes = null;
            if (endDate != string.Empty)
            {
                dtEndtDate = DateTime.Parse(endDate);
                dtEndtDate = dtEndtDate.AddDays(1);
                endDate = dtEndtDate.ToString();

            }

            if (custId != string.Empty && startDate != string.Empty && endDate != string.Empty)
            {
                intCustId = Int32.Parse(custId);
                dtStartDate = DateTime.Parse(startDate);
                dtEndtDate = DateTime.Parse(endDate);

                selectedSOes = _db.SOes.Where(so => so.CustomerId == intCustId && so.Date >= dtStartDate && so.Date <= dtEndtDate);

            }

            if (custId == string.Empty && startDate == string.Empty && endDate == string.Empty)
            {

                dtStartDate = DateTime.Parse("1-1-1800");
                dtEndtDate = DateTime.Today.AddDays(1);

                selectedSOes = _db.SOes;//.Where(so => so.CustomerId == intCustId && so.Date >= dtStartDate && so.Date <= dtEndtDate);

            }

            //get all customers data acornding to start end date
            if (custId == string.Empty && startDate != string.Empty && endDate != string.Empty)
            {

                dtStartDate = DateTime.Parse(startDate);
                dtEndtDate = DateTime.Parse(endDate);

                selectedSOes = _db.SOes.Where(so => so.Date >= dtStartDate && so.Date <= dtEndtDate);

            }

            //get this customer with from undefined startdate to this defined enddate
            if (custId != string.Empty && startDate == string.Empty && endDate != string.Empty)
            {
                intCustId = Int32.Parse(custId);
                dtStartDate = DateTime.Parse("1-1-1800");
                dtEndtDate = DateTime.Parse(endDate);

                selectedSOes = _db.SOes.Where(so => so.CustomerId == intCustId && so.Date >= dtStartDate && so.Date <= dtEndtDate);

            }

            //get this customer with from defined start date to undefined end date
            if (custId != string.Empty && startDate != string.Empty && endDate == string.Empty)
            {
                intCustId = Int32.Parse(custId);
                dtStartDate = DateTime.Parse(startDate);
                dtEndtDate = DateTime.Today.AddDays(1);

                selectedSOes = _db.SOes.Where(so => so.CustomerId == intCustId && so.Date >= dtStartDate && so.Date <= dtEndtDate);

            }

            //get this customer with all dates
            if (custId != string.Empty && startDate == string.Empty && endDate == string.Empty)
            {
                intCustId = Int32.Parse(custId);
                dtStartDate = DateTime.Parse("1-1-1800");
                dtEndtDate = DateTime.Today.AddDays(1);

                selectedSOes = _db.SOes.Where(so => so.CustomerId == intCustId && so.Date >= dtStartDate && so.Date <= dtEndtDate);

            }

            //get all customer with defined startdate and undefined end date
            if (custId == string.Empty && startDate != string.Empty && endDate == string.Empty)
            {

                dtStartDate = DateTime.Parse(startDate);
                dtEndtDate = DateTime.Today.AddDays(1);

                selectedSOes = _db.SOes.Where(so => so.Date >= dtStartDate && so.Date <= dtEndtDate);

            }

            //get all customers with undifined start date with defined enddate
            if (custId == string.Empty && startDate == string.Empty && endDate != string.Empty)
            {

                dtStartDate = DateTime.Parse("1-1-1800");
                dtEndtDate = DateTime.Parse(endDate);

                selectedSOes = _db.SOes.Where(so => so.Date >= dtStartDate && so.Date <= dtEndtDate);

            }
            GetTotalBalance(ref selectedSOes);
            Dictionary<int, int> LstMaxSerialNo = new Dictionary<int, int>();
            int thisSerial = 0;
            foreach (SO itm in selectedSOes)
            {
                thisSerial = (int)itm.Customer.SOes.Max(x => x.SOSerial);

                if (!LstMaxSerialNo.ContainsKey((int)itm.CustomerId))
                {
                    LstMaxSerialNo.Add(itm.Customer.Id, thisSerial);
                }
                //itm.Id = Encryption.Encrypt(itm.Id, "BZNS");
                itm.Id = string.Join("-", ASCIIEncoding.ASCII.GetBytes(Encryption.Encrypt(itm.Id, "BZNS")));
            }
            ViewBag.LstMaxSerialno = LstMaxSerialNo;
            return PartialView("_SelectedSOSR", selectedSOes.OrderByDescending(i => i.Date).ToList());

            //return View("Some thing went wrong");


        }
        public IActionResult CustomerWiseSale(int custId, string custName)
        {

            //DateTime dtEndtDate = DateTime.Today.AddDays(1);
            //DateTime dtStartDate = dtEndtDate.AddDays(-7);
            DateTime PKDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time"));
            var dtStartDate = new DateTime(PKDate.Year, PKDate.Month, 1);
            var dtEndtDate = dtStartDate.AddMonths(1).AddSeconds(-1);

            ViewBag.CustomerId = custId;
            ViewBag.CustName = custName;
            //ViewBag.SupplierName = supplierName;//_db.Products.FirstOrDefault(x => x.Id == productId).Name;
            ViewBag.Customers = _db.Customers;
            //01-Jan-2019

            ViewBag.StartDate = dtStartDate.ToString("dd-MMM-yyyy");
            ViewBag.EndDate = dtEndtDate.ToString("dd-MMM-yyyy");

            IQueryable<SO> sOes = _db.SOes;//.Include(s => s.Customer);
            sOes = sOes.Where(x => x.CustomerId == custId && x.Date >= dtStartDate && x.Date <= dtEndtDate).OrderBy(i => i.SOSerial).AsQueryable();
            //foreach (SO itm in sOes)
            //{
            //    //itm.Id = Encryption.Encrypt(itm.Id, "BZNS");
            //    itm.Id = string.Join("-", ASCIIEncoding.ASCII.GetBytes(Encryption.Encrypt(itm.Id, "BZNS")));
            //}

            return View("CustomerWiseSale", sOes);

            //return View("CustomerWiseSale", sOes.OrderBy(i => i.Date).ToList());
        }
        public IActionResult FilterCustomerWiseSale(string custId, string suppId, string startDate, string endDate)
        {

            /////////////////////////////////////////////////////////////////////////////
            IQueryable<SO> sOes = _db.SOes;//.Include(s => s.Customer);
            //sOes = sOes.Where(x => x.CustomerId == custId && x.Date >= dtStartDate && x.Date <= dtEndtDate).OrderBy(i => i.Date).OrderBy(i => i.SOSerial).AsQueryable();






            int intCustId;
            DateTime dtStartDate;
            DateTime dtEndtDate;
            IQueryable<SO> selectedSOes = null;
            if (endDate != string.Empty)
            {
                dtEndtDate = DateTime.Parse(endDate);
                dtEndtDate = dtEndtDate.AddDays(1);
                endDate = dtEndtDate.ToString();

            }

            if (custId != string.Empty && startDate != string.Empty && endDate != string.Empty)
            {
                intCustId = Int32.Parse(custId);
                dtStartDate = DateTime.Parse(startDate);
                dtEndtDate = DateTime.Parse(endDate);

                selectedSOes = sOes.Where(so => so.CustomerId == intCustId && so.Date >= dtStartDate && so.Date <= dtEndtDate);

            }

            if (custId == string.Empty && startDate == string.Empty && endDate == string.Empty)
            {

                dtStartDate = DateTime.Parse("1-1-1800");
                dtEndtDate = DateTime.Today.AddDays(1);

                selectedSOes = sOes;//.Where(so => so.CustomerId == intCustId && so.Date >= dtStartDate && so.Date <= dtEndtDate);

            }

            //get all customers data acornding to start end date
            if (custId == string.Empty && startDate != string.Empty && endDate != string.Empty)
            {

                dtStartDate = DateTime.Parse(startDate);
                dtEndtDate = DateTime.Parse(endDate);

                selectedSOes = sOes.Where(so => so.Date >= dtStartDate && so.Date <= dtEndtDate);

            }

            //get this customer with from undefined startdate to this defined enddate
            if (custId != string.Empty && startDate == string.Empty && endDate != string.Empty)
            {
                intCustId = Int32.Parse(custId);
                dtStartDate = DateTime.Parse("1-1-1800");
                dtEndtDate = DateTime.Parse(endDate);

                selectedSOes = sOes.Where(so => so.CustomerId == intCustId && so.Date >= dtStartDate && so.Date <= dtEndtDate);

            }

            //get this customer with from defined start date to undefined end date
            if (custId != string.Empty && startDate != string.Empty && endDate == string.Empty)
            {
                intCustId = Int32.Parse(custId);
                dtStartDate = DateTime.Parse(startDate);
                dtEndtDate = DateTime.Today.AddDays(1);

                selectedSOes = sOes.Where(so => so.CustomerId == intCustId && so.Date >= dtStartDate && so.Date <= dtEndtDate);

            }

            //get this customer with all dates
            if (custId != string.Empty && startDate == string.Empty && endDate == string.Empty)
            {
                intCustId = Int32.Parse(custId);
                dtStartDate = DateTime.Parse("1-1-1800");
                dtEndtDate = DateTime.Today.AddDays(1);

                selectedSOes = sOes.Where(so => so.CustomerId == intCustId && so.Date >= dtStartDate && so.Date <= dtEndtDate);

            }

            //get all customer with defined startdate and undefined end date
            if (custId == string.Empty && startDate != string.Empty && endDate == string.Empty)
            {

                dtStartDate = DateTime.Parse(startDate);
                dtEndtDate = DateTime.Today.AddDays(1);

                selectedSOes = sOes.Where(so => so.Date >= dtStartDate && so.Date <= dtEndtDate);

            }

            //get all customers with undifined start date with defined enddate
            if (custId == string.Empty && startDate == string.Empty && endDate != string.Empty)
            {

                dtStartDate = DateTime.Parse("1-1-1800");
                dtEndtDate = DateTime.Parse(endDate);

                selectedSOes = sOes.Where(so => so.Date >= dtStartDate && so.Date <= dtEndtDate);

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
        public IActionResult ProductWiseSale(int productId)
        {
            DateTime PKDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time"));
            var dtStartDate = new DateTime(PKDate.Year, PKDate.Month, 1);
            var dtEndtDate = dtStartDate.AddMonths(1).AddSeconds(-1);
            ViewBag.ProductId = productId;
            ViewBag.ProductName = _db.Products.FirstOrDefault(x => x.Id == productId).Name;
            ViewBag.Customers = _db.Customers;
            ViewBag.StartDate = dtStartDate.ToString("dd-MMM-yyyy");
            ViewBag.EndDate = dtEndtDate.ToString("dd-MMM-yyyy");

            List<SO> sOes = _db.SOes.ToList();//.Include(s => s.Customer);

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

            sOes = lstSlectedSO.Where(x => x.Date >= dtStartDate && x.Date <= dtEndtDate).ToList();
            foreach (SO itm in sOes)
            {
                //itm.Id = Encryption.Encrypt(itm.Id, "BZNS");
                itm.Id = string.Join("-", ASCIIEncoding.ASCII.GetBytes(Encryption.Encrypt(itm.Id, "BZNS")));
            }

            return View("ProductWiseSale", sOes.OrderBy(i => i.SOSerial).ToList());
        }
        public IActionResult FilterProductWiseSale(string prodId, string custId, string suppId, string startDate, string endDate)
        {


            DateTime dtStartDate = DateTime.Today;//just to defer error
            DateTime dtEndtDate = DateTime.Today;//just to defer error


            if (startDate != string.Empty)
            {
                dtStartDate = DateTime.Parse(startDate);
            }

            if (endDate != string.Empty)
            {
                dtEndtDate = DateTime.Parse(endDate);
                //dtEndtDate = dtEndtDate.AddDays(1);
            }

            if (startDate == string.Empty)
            {
                dtStartDate = DateTime.Parse("1-1-1800");
            }

            if (endDate == string.Empty)
            {
                dtEndtDate = DateTime.Today.AddDays(1);
            }


            //List<SO> sOes = _db.SOes.ToList();//.Include(s => s.Customer);
            List<SO> selectedSOes = null;

            selectedSOes = _db.SOes.Where(so => so.Date >= dtStartDate && so.Date <= dtEndtDate).ToList();
            /////////////////////////////////////////////////////////////////////////////////////////////////////////

            int intProdId;
            intProdId = Int32.Parse(prodId);

            //List<SOD> lstSODs = _db.SODs.Where(x => x.ProductId == intProdId).ToList();

            List<SO> newSOes = new List<SO>();
            List<SOD> newSODs;
            foreach (SO thisSO in selectedSOes)
            {

                newSODs = new List<SOD>();
                foreach (SOD thisSOD in thisSO.SODs)
                {
                    if (thisSOD.ProductId == intProdId)
                    {
                        newSODs.Add(thisSOD);
                    }

                }
                if (newSODs.Count > 0)
                {
                    thisSO.SODs = newSODs;
                    newSOes.Add(thisSO);
                }


            }

            //foreach (SO itm in selectedSOes)
            //{
            //    //itm.Id = Encryption.Encrypt(itm.Id, "BZNS");
            //    itm.Id = string.Join("-", ASCIIEncoding.ASCII.GetBytes(Encryption.Encrypt(itm.Id, "BZNS")));
            //}
            //GetTotalBalance(ref selectedSOes);
            //return PartialView("_SelectedSOSR", selectedSOes.OrderByDescending(i => i.Date).ToList());
            //_ProfitGainFromSupplier
            return PartialView("_ProductWiseSale", newSOes.OrderBy(i => i.SOSerial).ToList());
            //return View("Some thing went wrong");
        }
        public IActionResult PerMonthSale(int productId)
        {
            IQueryable<SO> sOes = _db.SOes.Include(s => s.Customer);

            //sOes = _db.SOes.Where(x => x.SODs.Where(y => y.ProductId == productId));

            List<SOD> lstSODs = _db.SODs.Where(x => x.ProductId == productId && x.SaleType == false).ToList();
            List<SO> lstSlectedSO = new List<SO>();
            foreach (SOD lsod in lstSODs)
            {
                if (lstSlectedSO.Where(x => x.Id == lsod.SOId).FirstOrDefault() == null)
                {
                    lstSlectedSO.Add(lsod.SO);
                }
            }

            sOes = lstSlectedSO.ToList().AsQueryable();
            foreach (SO itm in sOes)
            {
                //itm.Id = Encryption.Encrypt(itm.Id, "BZNS");
                itm.Id = string.Join("-", ASCIIEncoding.ASCII.GetBytes(Encryption.Encrypt(itm.Id, "BZNS")));
            }
            ViewBag.ProductName = _db.Products.FirstOrDefault(x => x.Id == productId).Name;
            return View("PerMonthSale", sOes.OrderBy(i => i.Date).ToList());
        }

        public IActionResult SearchProduct(int productId)
        {
            IQueryable<SO> sOes = _db.SOes.Include(s => s.Customer);

            //sOes = _db.SOes.Where(x => x.SODs.Where(y => y.ProductId == productId));

            List<SOD> lstSODs = _db.SODs.Where(x => x.ProductId == productId).ToList();
            List<SO> lstSlectedSO = new List<SO>();
            foreach (SOD lsod in lstSODs)
            {
                if (lstSlectedSO.Where(x => x.Id == lsod.SOId).FirstOrDefault() == null)
                {
                    lstSlectedSO.Add(lsod.SO);
                }


            }

            sOes = lstSlectedSO.ToList().AsQueryable();

            //sOes.ForEachAsync(m => m.Id = Encryption.Encrypt(m.Id, "BZNS"));
            //var sOes = _db.SOes.Where(s => s.SaleReturn == false);
            GetTotalBalance(ref sOes);
            foreach (SO itm in sOes)
            {

                //itm.Id = Encryption.Encrypt(itm.Id, "BZNS");
                itm.Id = string.Join("-", ASCIIEncoding.ASCII.GetBytes(Encryption.Encrypt(itm.Id, "BZNS")));
            }
            ViewBag.Customers = _db.Customers;
            return View("Index", sOes.OrderByDescending(i => i.Date).ToList());
        }

        private void GetTotalBalance(ref IQueryable<SO> SOes)
        {
            //IQueryable<SO> DistSOes = SOes.Select(x => x.CustomerId).Distinct();
            IQueryable<SO> DistSOes = SOes.GroupBy(x => x.CustomerId).Select(y => y.FirstOrDefault());

            decimal TotalBalance = 0;
            foreach (SO itm in DistSOes)
            {
                Customer cust = _db.Customers.Where(x => x.Id == itm.CustomerId).FirstOrDefault();

                TotalBalance += (decimal)cust.Balance;

            }
            ViewBag.TotalBalance = TotalBalance;

        }
        //[ChildActionOnly]
        //public PartialViewResult _SelectedSOSR()
        //{

        //    return PartialView(_db.SOes);
        //}


        // GET: SOes/Details/5
        public IActionResult Details(string id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            SO? sO = _db.SOes.Find(id);
            if (sO == null)
            {
                return NotFound();
            }
            return View(sO);
        }

        // GET: SOes/Create

        public IActionResult Create(string IsReturn)
        {

            //ViewBag.CustomerId = new SelectList(_db.Customers, "Id", "Name");
            //ViewBag.Products = _db.Products;

            //int maxId = _db.Customers.Max(p => p.Id);
            int maxId = _db.Customers.DefaultIfEmpty().Max(p => p == null ? 0 : p.Id);
            maxId += 1;
            ViewBag.SuggestedNewCustId = maxId;


            SaleOrderViewModel saleOrderViewModel = new SaleOrderViewModel();
            saleOrderViewModel.Customers = _db.Customers;
            saleOrderViewModel.Products = _db.Products.Where(x => x.Saleable == true);
            //bool IsReturn1 = true;
            ViewBag.IsReturn = IsReturn;
            //string isReturn1 = "true";
            //ViewBag.isReturn = isReturn1;
            return View(saleOrderViewModel);
        }


        //[OutputCache(NoStore = true, Duration = 0)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("Name,Address", Prefix = "Customer")] Customer Customer, [Bind("BillAmount,Balance,PrevBalance,BillPaid,Discount,CustomerId,Remarks,Remarks2,PaymentMethod,PaymentDetail,SaleReturn", Prefix = "SaleOrder")] SO sO, [Bind("ProductId,SalePrice,Quantity,SaleType,PerPack,IsPack", Prefix = "SaleOrderDetail")] List<SOD> sOD, IFormCollection collection)

        {

            string SOId = string.Empty;
            //SO sO = new SO();
            if (ModelState.IsValid)
            {


                Customer cust = _db.Customers.FirstOrDefault(x => x.Id == sO.CustomerId);
                if (cust == null)
                {//its means new customer
                    //sO.CustomerId = 10;
                    //int maxId = _db.Customers.Max(p => p.Id);
                    int maxId = _db.Customers.DefaultIfEmpty().Max(p => p == null ? 0 : p.Id);
                    maxId += 1;
                    Customer.Id = maxId;
                    Customer.Balance = sO.Balance;
                    _db.Customers.Add(Customer);
                    //_db.SaveChanges();
                }
                else
                {//its means old customer. old customer balance should be updated.
                    //Customer.Id = (int)sO.CustomerId;
                    cust.Balance = sO.Balance;
                    _db.Entry(cust).State = EntityState.Modified;
                    //_db.SaveChanges();




                    //Payment payment = new Payment();
                    //payment = _db.Payments.Find(orderId);
                    //payment.Status = true;
                    //_db.Entry(payment).State = EntityState.Modified;
                    //_db.SaveChanges();

                }

                ////////////////////////////////////////
                //int maxId = _db.SOes.Max(p => p.Auto);
                int maxId1 = (int)_db.SOes.DefaultIfEmpty().Max(p => p == null ? 0 : p.SOSerial);
                maxId1 += 1;
                sO.SOSerial = maxId1;
                
                //sO.Date = DateTime.Now;
                sO.Date = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time"));
                //sO.SaleReturn = false;
                sO.Id = System.Guid.NewGuid().ToString().ToUpper();
                sO.SaleOrderAmount = 0;
                sO.SaleReturnAmount = 0;
                sO.SaleOrderQty = 0;
                sO.SaleReturnQty = 0;
                sO.Profit = 0;
                var emp = HttpContext.Session.GetCurrentUser();
                if (emp == null)
                    return RedirectToAction("Login", "UserManagement");
                sO.EmployeeId = emp.Id;

                _db.SOes.Add(sO);
                //_db.SaveChanges();
                int sno = 0;
                decimal totalPurchaseAmount = 0;
                //sOD.RemoveAll(so => so.ProductId == null);
                if (sOD != null)
                {

                    foreach (SOD sod in sOD)
                    {
                        sno += 1;
                        sod.SODId = sno;
                        sod.SO = sO;
                        sod.SOId = sO.Id;

                        Product product = _db.Products.FirstOrDefault(x => x.Id == sod.ProductId);
                        //sod.Sale Price in now from view
                        //sod.SalePrice = product.SalePrice;
                        //dont do this uneessary and sacary(no we have to do it here but not in update. if we not do it here then purchase price will remain empty. and cause error in productwisesale etc)
                        sod.PurchasePrice = product.PurchasePrice;
                        if (sod.Quantity == null) { sod.Quantity = 0; }
                        sod.OpeningStock = product.Stock;
                        if (sod.SaleType == true)//return
                        {
                            if (sod.IsPack == false)
                            {
                                sO.SaleReturnAmount += (sod.Quantity * sod.SalePrice);
                                decimal qty = (decimal)sod.Quantity;// / (decimal)product.PerPack;
                                product.Stock += qty;
                                sO.SaleReturnQty += qty;//(int)sod.Quantity;
                                sO.Profit -= (qty * sod.SalePrice) - (decimal)(qty * product.PurchasePrice); //- (decimal)(sO.Discount);
                            }
                            else
                            {
                                sO.SaleReturnAmount += (sod.Quantity * sod.SalePrice * sod.PerPack);
                                product.Stock += (int)sod.Quantity * sod.PerPack;

                                sO.SaleReturnQty += (int)sod.Quantity * sod.PerPack;
                                sO.Profit -= (sod.Quantity * sod.SalePrice * sod.PerPack) - (decimal)(sod.Quantity * product.PurchasePrice * sod.PerPack); //- (decimal)(sO.Discount);
                            }
                        }
                        else//sale
                        {


                            if (sod.IsPack == false)
                            {//piece
                                sO.SaleOrderAmount += (sod.Quantity * sod.SalePrice);
                                //int pieceSold = (int)(sod.Quantity * product.Stock);
                                decimal qty = (decimal)sod.Quantity;// / (decimal)product.PerPack;
                                product.Stock -= qty;

                                sO.SaleOrderQty += qty;//(int)sod.Quantity;
                                sO.Profit += (qty * sod.SalePrice) - (decimal)(qty * product.PurchasePrice); //- (decimal)(sO.Discount);
                            }
                            else
                            {//pack

                                sO.SaleOrderAmount += (sod.Quantity * sod.SalePrice * sod.PerPack);
                                product.Stock -= (int)sod.Quantity * sod.PerPack;

                                sO.SaleOrderQty += (int)sod.Quantity * sod.PerPack;
                                sO.Profit += (sod.Quantity * sod.SalePrice * sod.PerPack) - (decimal)(sod.Quantity * product.PurchasePrice * sod.PerPack); //- (decimal)(sO.Discount);
                            }





                        }

                    }

                    sO.Profit -= (decimal)sO.Discount;
                    _db.SODs.AddRange(sOD);
                }

                /////////////////////add values to payment table

                int maxPaymentId = _db.Payments.DefaultIfEmpty().Max(p => p == null ? 0 : p.Id);
                maxPaymentId += 1;
                Payment payment = new Payment();
                payment.PaymentAmount = (decimal)sO.BillPaid;
                payment.Id = maxPaymentId;
                payment.SOId = sO.Id;
                payment.PaymentMethod = collection["SaleOrder.PaymentMethod"].ToString();
                payment.Remarks = collection["SaleOrder.PaymentRemarks"].ToString();
                payment.ReceivedDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time"));

                _db.Payments.Add(payment);

                //////////////////////

                _db.SaveChanges();


                //SqlParameter param1 = new SqlParameter("@SaleOrderID", sO.Id);
                ////var result = _db.Database.ExecuteSqlCommand("spSOReceipt @SaleOrderID", param1);
                //var result = _db.Database.SqlQuery<Object>("spSOReceipt @SaleOrderID", param1);


                //var cr = new ReportDocument();
                //cr.Load(@"E:\PROJECTS\MYBUSINESS - v.4.6\MYBUSINESS\Reports\SOReceipt.rpt");
                //cr.DataDefinition.RecordSelectionFormula = "{SaleOrderID} = '" + sO.Id + "'";
                //cr.PrintToPrinter(1, true, 0, 0);


                ////////////////////////finalized
                //string pathh = HttpRuntime.AppDomainAppPath;
                //ReportDocument reportDocument = new ReportDocument();
                //reportDocument.Load(pathh + @"Reports\SOReceipt.rpt");
                //reportDocument.SetParameterValue("@SaleOrderID", sO.Id);
                //System.Drawing.Printing.PrinterSettings printerSettings = new System.Drawing.Printing.PrinterSettings();
                ////printerSettings.PrinterName = PrinterName;
                //reportDocument.PrintToPrinter(printerSettings, new PageSettings(), false);
                /////////////////////////////////////


                SOId = string.Join("-", ASCIIEncoding.ASCII.GetBytes(Encryption.Encrypt(sO.Id, "BZNS")));

                //RedirectToAction("Create", PrintSO3(SOId));

                //return RedirectToAction("Create");
            }

            //return PrintSO3(SOId);
            return RedirectToAction("PrintSO3", new { id = SOId });
            //SaleOrderViewModel saleOrderViewModel = new SaleOrderViewModel();
            //saleOrderViewModel.Customers = _db.Customers;
            //saleOrderViewModel.Products = _db.Products;

            //return View(saleOrderViewModel);


        }
        //public void PrintSO(string SOId)
        //{
        //    SOId = Encryption.Decrypt(SOId, "BZNS");
        //    string pathh = HttpRuntime.AppDomainAppPath;
        //    ReportDocument reportDocument = new ReportDocument();
        //    reportDocument.Load(pathh + @"Reports\SOSRReceipt2.rpt");
        //    //reportDocument.SetDatabaseLogon("sa", "abc", "LAPTOP-MGR35B58", "Business");


        //    ////
        //    CrystalDecisions.CrystalReports.Engine.Database oCRDb = reportDocument.Database;
        //    CrystalDecisions.CrystalReports.Engine.Tables oCRTables = oCRDb.Tables;
        //    //CrystalDecisions.CrystalReports.Engine.Table oCRTable;
        //    CrystalDecisions.Shared.TableLogOnInfo oCRTableLogonInfo;
        //    CrystalDecisions.Shared.ConnectionInfo oCRConnectionInfo = new CrystalDecisions.Shared.ConnectionInfo();
        //    oCRConnectionInfo.DatabaseName = "Business";
        //    oCRConnectionInfo.ServerName = "(local)";
        //    oCRConnectionInfo.UserID = "sa";
        //    oCRConnectionInfo.Password = "abc";
        //    foreach (CrystalDecisions.CrystalReports.Engine.Table oCRTable in oCRTables)
        //    {
        //        oCRTableLogonInfo = oCRTable.LogOnInfo;
        //        oCRTableLogonInfo.ConnectionInfo = oCRConnectionInfo;
        //        oCRTable.ApplyLogOnInfo(oCRTableLogonInfo);
        //    }
        //    ////

        //    reportDocument.SetParameterValue("@SaleOrderID", SOId);
        //    System.Drawing.Printing.PrinterSettings printerSettings = new System.Drawing.Printing.PrinterSettings();
        //    //printerSettings.PrinterName = "abc";
        //    reportDocument.PrintToPrinter(printerSettings, new PageSettings(), false);

        //}
    

        public IActionResult PrintSO2(string id)
        {
            id = Decode(id);
            using var localreport = new LocalReport();
            localreport.ReportPath = Path.Combine(_env.WebRootPath, "Reports", "Report3.rdlc");

            var reportDataSource = new ReportDataSource
            {
                Name = "ReportDataSet",
                Value = null
            };
            localreport.DataSources.Add(reportDataSource);

            var renderBytes = localreport.Render("PDF");
            Response.Headers.Append("content-disposition", "attachment; filename=Urls.pdf");
            return File(renderBytes, "application/pdf");
        }

        public IActionResult PrintSO3(string id)
        {
            id = Decode(id);
            const string mimeType = "application/pdf";
            const string extension = "pdf";

            var so = _db.SOes.FirstOrDefault(y => y.Id == id);
            if (so == null)
                return NotFound();

            var emp = _db.Employees.FirstOrDefault(x => x.Id == so.EmployeeId);
            var reportFile = string.Equals(emp?.Login, "LahoreKarachi", StringComparison.Ordinal)
                ? "Sale_LahoreKarachi.rdlc"
                : "Sale_GoldPanel.rdlc";
            var reportPath = Path.Combine(_env.WebRootPath, "Reports", reportFile);

            using var lr = new LocalReport();
            lr.ReportPath = reportPath;
            lr.DataSources.Add(new ReportDataSource("DataSet1", _db.spSOReport(id).ToList()));
            lr.SetParameters(new[] { new ReportParameter("SaleOrderID", id) });

            byte[] bytes;
            if (FileTempered(reportPath))
                bytes = Encoding.ASCII.GetBytes("Report not found. Contact +92 300 88 55 011");
            else
                bytes = lr.Render("PDF");

            Response.Headers.Append("Content-Disposition", $"inline; filename=\"Sale Receipt.{extension}\"");
            return File(bytes, mimeType);
        }

        public decimal GetPreviousBalance(int id)
        {
            IQueryable lstSO = _db.SOes.Where(x => x.CustomerId == id);

            //lstSO.ForEachAsync(c => { c. = 0; c.GroupID = 0; c.CompanyID = 0; });
            decimal SOAmount = 0;
            decimal SRAmount = 0;
            foreach (SO itm in lstSO)
            {
                SOAmount += (decimal)itm.SaleOrderAmount;
                SRAmount += (decimal)itm.SaleReturnAmount;

            }

            return (SOAmount - SRAmount);
        }


        // GET: SOes/Edit/5
        public IActionResult Edit(string id, bool update)
        {

            if (id == null)
            {

                return BadRequest();
            }

            //byte[] BytesArr = id.Split('-').Select(byte.Parse).ToArray();
            //id = new string( Encoding.UTF8.GetString(BytesArr).ToCharArray());
            //id = Encryption.Decrypt(id,"BZNS");

            int maxId = _db.Customers.DefaultIfEmpty().Max(p => p == null ? 0 : p.Id);
            maxId += 1;
            ViewBag.SuggestedNewCustId = maxId;

            List<MySaleType> myOptionLst = new List<MySaleType> {
                            new MySaleType {
                                Text = "Order",
                                Value = "false"
                            },
                            new MySaleType {
                                Text = "Return",
                                Value = "true"
                            }
                        };
            ViewBag.OptionLst = myOptionLst;

            ////////////////
            List<MyPaymentMethod> myPaymentOptionLst = new List<MyPaymentMethod> {
                            new MyPaymentMethod {
                                Text = "Cash",
                                Value = "Cash"
                            },
                            new MyPaymentMethod {
                                Text = "Online",
                                Value = "Online"
                            },
                            new MyPaymentMethod {
                                Text = "Cheque",
                                Value = "Cheque"
                            },
                            new MyPaymentMethod {
                                Text = "Other",
                                Value = "Other"
                            }
                        };

            ViewBag.PaymentMethodOptionLst = myPaymentOptionLst;

            List<MyUnitType> myUnitTypeOptionList = new List<MyUnitType> {
                            new MyUnitType {
                                Text = "Piece",
                                Value = "false"
                            },
                            new MyUnitType {
                                Text = "Pack",
                                Value = "true"
                            }
                        };

            ViewBag.UnitTypeOptionList = myUnitTypeOptionList;
            string iid = Decode(id);
            Payment pmnt = _db.Payments.Where(x => x.SOId == iid).FirstOrDefault();
            if (pmnt != null)
            {
                ViewBag.paymentMethod = pmnt.PaymentMethod;
                ViewBag.paymentRemarks = pmnt.Remarks;
            }
            ///////////////////

            id = Decode(id);

            SO sO = _db.SOes.Find(id);
            if (sO == null)
            {
                return NotFound();
            }
            SaleOrderViewModel saleOrderViewModel = new SaleOrderViewModel();
            List<SOD> sod = _db.SODs.Where(x => x.SOId == id).ToList();
            saleOrderViewModel.Products = _db.Products;
            saleOrderViewModel.Customers = _db.Customers;
            saleOrderViewModel.SaleOrderDetail = sod;
            sO.Id = Encryption.Encrypt(sO.Id, "BZNS");
            saleOrderViewModel.SaleOrder = sO;
            int orderQty = 0;
            int orderQtyPiece = 0;//orderQtyPiece 'P for piece' 
            int returnQty = 0;
            int returnQtyPiece = 0;//orderQtyPiece 'P for piece' 
            foreach (var item in sod)
            {
                if (sO.SaleReturn == false)
                {
                    if (item.IsPack == true)
                    {//Pack
                        orderQty += (int)item.Quantity;
                    }
                    else
                    {//Item
                        orderQtyPiece += (int)item.Quantity;
                    }
                }
                else
                {
                    if (item.IsPack == true)
                    {//Pack
                        returnQty += (int)item.Quantity;
                    }
                    else
                    {//Item
                        returnQtyPiece += (int)item.Quantity;
                    }

                }

            }
            ViewBag.orderQty = orderQty;
            ViewBag.orderQtyPiece = orderQtyPiece;
            ViewBag.returnQty = returnQty;
            ViewBag.returnQtyPiece = returnQtyPiece;
            //ViewBag.CustomerId = new SelectList(_db.Customers, "Id", "Name", sO.CustomerId);
            ViewBag.CustomerName = sO.Customer.Name;
            ViewBag.CustomerAddress = sO.Customer.Address;
            decimal subTotal = (decimal)(sO.SaleOrderAmount - sO.SaleReturnAmount - sO.Discount);
            ViewBag.SubTotal = subTotal;
            ViewBag.Total = subTotal + (decimal)sO.PrevBalance;
            ViewBag.IsUpdate = update;
            ViewBag.IsReturn = sO.SaleReturn.ToString().ToLower();
            return View(saleOrderViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        //public IActionResult Edit([Bind("BillAmount,Balance,BillPaid,Discount", Prefix = "SaleOrder")] SO sO, [Bind("ProductId,Quantity", Prefix = "SaleOrderDetail")] List<SOD> sOD)
        public IActionResult Edit(SaleOrderViewModel saleOrderViewModel1)
        {
            SO newSO = saleOrderViewModel1.SaleOrder;
            List<SOD> newSODs = saleOrderViewModel1.SaleOrderDetail;
            if (ModelState.IsValid)
            {
                newSO.Id = Encryption.Decrypt(saleOrderViewModel1.SaleOrder.Id, "BZNS");//
                SO sO = _db.SOes.Where(x => x.Id == newSO.Id).FirstOrDefault();
                sO.Date = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time"));//
                //sO.SaleReturn = false;//
                sO.BillAmount = newSO.BillAmount;//
                sO.Discount = newSO.Discount;//
                sO.BillPaid = newSO.BillPaid;//
                sO.Balance = newSO.Balance;//
                sO.Remarks = newSO.Remarks;//
                sO.Remarks2 = newSO.Remarks;//
                sO.PaymentMethod = newSO.PaymentMethod;
                sO.PaymentDetail = newSO.PaymentDetail;

                //sO.SOSerial = newSO.SOSerial;//should be unchanged

                ///////////////////////////////////////////

                //Customer cust = _db.Customers.FirstOrDefault(x => x.Id == newSO.CustomerId);
                Customer customer = _db.Customers.Where(x => x.Id == newSO.CustomerId).FirstOrDefault();
                if (customer == null)
                {//its means new customer(not in db)
                 //sO.CustomerId = 10;
                 //int maxId = _db.Customers.Max(p => p.Id);
                    customer = saleOrderViewModel1.Customer;
                    int maxId = _db.Customers.DefaultIfEmpty().Max(p => p == null ? 0 : p.Id);
                    maxId += 1;

                    customer.Id = maxId;
                    //customer.Balance = newSO.Balance;
                    _db.Customers.Add(customer);
                }
                else
                {
                    _db.Entry(customer).State = EntityState.Modified;
                }

                if (sO.CustomerId != newSO.CustomerId)
                {//some other db customer
                 //first revert the previous customer balance 
                    Customer oldCustomer = _db.Customers.Where(x => x.Id == sO.CustomerId).FirstOrDefault();
                    oldCustomer.Balance = _db.SOes.Where(x => x.Id == sO.Id).FirstOrDefault().PrevBalance;
                    _db.Entry(oldCustomer).State = EntityState.Modified;
                }

                sO.PrevBalance = newSO.PrevBalance;//
                // assign balance of this customer
                //Customer customer = _db.Customers.Where(x => x.Id == newSO.CustomerId).FirstOrDefault();
                customer.Balance = newSO.Balance;
                //assign customer and customerId in SO
                sO.CustomerId = newSO.CustomerId;
                sO.Customer = customer;

                /////////////////////////////////////////////////////////////////////////////



                List<SOD> oldSODs = _db.SODs.Where(x => x.SOId == newSO.Id).ToList();

                //handling old prodcts quantity. add old quantites back to the stock, then in next loop product quantity will be minus. this is simple and stateforward.
                foreach (SOD sod in oldSODs)
                {
                    Product product = _db.Products.FirstOrDefault(x => x.Id == sod.ProductId);

                    if (sod.SaleType == false)//sale
                    {
                        //product.Stock += sod.Quantity;

                        if (sod.IsPack == false)
                        {
                            decimal qty = (decimal)sod.Quantity;// / (decimal)product.PerPack;
                            product.Stock += qty;
                        }
                        else
                        {
                            product.Stock += (int)sod.Quantity * sod.PerPack;
                        }


                    }
                    else//return
                    {
                        //product.Stock -= sod.Quantity;

                        if (sod.IsPack == false)
                        {
                            decimal qty = (decimal)sod.Quantity;// / (decimal)product.PerPack;
                            product.Stock -= qty;
                        }
                        else
                        {
                            product.Stock -= (int)sod.Quantity * sod.PerPack;
                        }



                    }
                    _db.Entry(product).State = EntityState.Modified;
                }

                _db.SODs.RemoveRange(oldSODs);
                //////////////////////////////////////////////////////////////////////////////

                sO.SaleOrderAmount = 0;
                sO.SaleReturnAmount = 0;
                sO.SaleOrderQty = 0;
                sO.SaleReturnQty = 0;
                sO.Profit = 0;
                int sno = 0;

                if (newSODs != null)
                {

                    foreach (SOD sod in newSODs)
                    {
                        sno += 1;
                        sod.SODId = sno;
                        sod.SO = sO;
                        sod.SOId = sO.Id;

                        Product product = _db.Products.FirstOrDefault(x => x.Id == sod.ProductId);
                        //sod.salePrice is now from view
                        //sod.SalePrice = product.SalePrice;
                        //dont do this. when user even just open a old bill and just press save. and price was updated after that old bill. all calculations gets wrong
                        //if we dont do this then error in product wise sale
                        sod.PurchasePrice = product.PurchasePrice;
                        if (sod.Quantity == null) { sod.Quantity = 0; }
                        sod.OpeningStock = product.Stock;
                        if (sod.SaleType == true)//return
                        {
                            if (sod.IsPack == false)
                            {
                                sO.SaleReturnAmount += (sod.Quantity * sod.SalePrice);
                                decimal qty = (decimal)sod.Quantity;// / (decimal)product.PerPack;
                                product.Stock += qty;
                                sO.SaleReturnQty += qty;//(int)sod.Quantity;
                                sO.Profit -= (qty * sod.SalePrice) - (decimal)(qty * product.PurchasePrice); //- (decimal)(sO.Discount);
                            }
                            else
                            {
                                sO.SaleReturnAmount += (sod.Quantity * sod.SalePrice * sod.PerPack);
                                product.Stock += (int)sod.Quantity * sod.PerPack;

                                sO.SaleReturnQty += (int)sod.Quantity * sod.PerPack;
                                sO.Profit -= (sod.Quantity * sod.SalePrice * sod.PerPack) - (decimal)(sod.Quantity * product.PurchasePrice * sod.PerPack); //- (decimal)(sO.Discount);
                            }
                        }
                        else//sale
                        {
                            if (sod.IsPack == false)
                            {//piece
                                sO.SaleOrderAmount += (sod.Quantity * sod.SalePrice);
                                //int pieceSold = (int)(sod.Quantity * product.Stock);
                                decimal qty = (decimal)sod.Quantity;// / (decimal)product.PerPack;
                                product.Stock -= qty;

                                sO.SaleOrderQty += qty;//(int)sod.Quantity;
                                sO.Profit += (qty * sod.SalePrice) - (decimal)(qty * product.PurchasePrice); //- (decimal)(sO.Discount);
                            }
                            else
                            {//pack

                                sO.SaleOrderAmount += (sod.Quantity * sod.SalePrice * sod.PerPack);
                                product.Stock -= (int)sod.Quantity * sod.PerPack;

                                sO.SaleOrderQty += (int)sod.Quantity * sod.PerPack;
                                sO.Profit += (sod.Quantity * sod.SalePrice * sod.PerPack) - (decimal)(sod.Quantity * product.PurchasePrice * sod.PerPack); //- (decimal)(sO.Discount);
                            }

                        }

                    }
                    sO.Profit -= (decimal)sO.Discount;
                    _db.Entry(sO).State = EntityState.Modified;
                    _db.Entry(sO).Property(x => x.SOSerial).IsModified = false;
                    _db.SODs.AddRange(newSODs);

                }
                _db.SaveChanges();

                return RedirectToAction("Index");
            }

            //ViewBag.CustomerId = new SelectList(_db.Customers, "Id", "Name", sO.CustomerId);
            //return View(sO);
            SaleOrderViewModel saleOrderViewModel = new SaleOrderViewModel();

            saleOrderViewModel.Products = _db.Products;
            return View(saleOrderViewModel);
            //return View();
        }

        private string Decode(string id)
        {
            byte[] BytesArr = id.Split('-').Select(byte.Parse).ToArray();
            id = new string(Encoding.UTF8.GetString(BytesArr).ToCharArray());
            id = Encryption.Decrypt(id, "BZNS");
            return id;
        }





        // GET: SOes/Delete/5
        public IActionResult Delete(string id)
        {
            return null;
            if (id == null)
            {
                return BadRequest();
            }
            id = Decode(id);
            SO sO = _db.SOes.Find(id);
            if (sO == null)
            {
                return NotFound();
            }
            return View(sO);
        }

        // POST: SOes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(string id)
        {
            return null;
            id = Decode(id);

            List<SOD> oldSODs = _db.SODs.Where(x => x.SOId == id).ToList();
            //handling old prodcts quantity. add old quantites back to the stock, then in next loop product quantity will be minus. this si simple and stateforward.
            foreach (SOD sod in oldSODs)
            {
                Product product = _db.Products.FirstOrDefault(x => x.Id == sod.ProductId);
                product.Stock += sod.Quantity;
            }
            _db.SODs.RemoveRange(oldSODs);

            SO sO = _db.SOes.Find(id);
            _db.SOes.Remove(sO);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        bool FileTempered(string filePath)
        {
            return false;
            //DateTime modificationDate = System.IO.File.GetLastWriteTime(filePath);
            //long fileSize = new System.IO.FileInfo(filePath).Length;
            //return (fileSize != 60576) ? true : false;


        }
      
        void EnterProfit()
        {

            foreach (SO so in _db.SOes)
            {
                //decimal totalPurchasePrice = 0;
                List<SOD> lstSODItems = _db.SODs.Where(x => x.SOId == so.Id).ToList();

                decimal soProfit = 0;
                foreach (SOD sod in lstSODItems)

                {
                    Product prod = _db.Products.Where(x => x.Id == sod.ProductId).FirstOrDefault();

                    if (sod.SaleType == true)//return
                    {
                        //totalPurchasePrice += (decimal)(prod.PurchasePrice * sod.Quantity);
                        soProfit -= (decimal)(sod.Quantity * prod.SalePrice) - (decimal)(sod.Quantity * prod.PurchasePrice); //- (decimal)(so.Discount);
                    }
                    else
                    {
                        //totalPurchasePrice += (decimal)(prod.PurchasePrice * sod.Quantity);
                        soProfit += (decimal)(sod.Quantity * prod.SalePrice) - (decimal)(sod.Quantity * prod.PurchasePrice); //- (decimal)(so.Discount);
                    }

                    sod.PurchasePrice = prod.PurchasePrice;
                    _db.Entry(sod).State = EntityState.Modified;
                }
                so.Profit = soProfit - (decimal)so.Discount;

                //so.Profit = (decimal)(so.SaleOrderAmount - so.Discount) - totalPurchasePrice;
                _db.Entry(so).State = EntityState.Modified;
            }
            _db.SaveChanges();
        }

    }

}

