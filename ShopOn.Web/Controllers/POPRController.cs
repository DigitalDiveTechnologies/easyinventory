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
using EasyInventory.PgData.Entities;
using System.IO;

namespace ShopOn.Web.Controllers
{
    public class POPRController : Controller
    {
        private readonly EasyInventoryDbContext _db;
        private readonly IWebHostEnvironment _env;

        public POPRController(EasyInventoryDbContext db, IWebHostEnvironment env) { _db = db; _env = env; }

        // GET: POes
        public IActionResult Index()
        {
            var (dtStartDate, dtEndtDate, startDisplay, endDisplay) = Utilities.GetCurrentMonthRange();

            //IQueryable<PO> pOes = _db.POes.Include(s => s.Supplier);
            IQueryable<PO> pOes = _db.POes.Where(x => x.Date >= dtStartDate && x.Date <= dtEndtDate).Include(s => s.Supplier);
            //pOes.ForEachAsync(m => m.Id = Encryption.Encrypt(m.Id, "BZNS"));
            //var pOes = _db.POes.Where(s => s.SaleReturn == false);
            GetTotalBalance(ref pOes);
            Dictionary<int, int> LstMaxSerialNo = new Dictionary<int, int>();
            int thisSerial = 0;
            foreach (PO itm in pOes)
            {
                thisSerial = (int)itm.Supplier.POes.Max(x => x.POSerial);

                if (!LstMaxSerialNo.ContainsKey((int)itm.SupplierId))
                {
                    LstMaxSerialNo.Add(itm.Supplier.Id, thisSerial);
                }

                //itm.Id = Encryption.Encrypt(itm.Id, "BZNS");
                itm.Id = string.Join("-", ASCIIEncoding.ASCII.GetBytes(Encryption.Encrypt(itm.Id, "BZNS")));
            }
            ViewBag.LstMaxSerialno = LstMaxSerialNo;
            ViewBag.Suppliers = _db.Suppliers;
            ViewBag.StartDate = startDisplay;
            ViewBag.EndDate = endDisplay;
            return View(pOes.OrderByDescending(i => i.Date).ToList());
        }
        //public IActionResult SearchData(string custName, DateTime startDate, DateTime endDate)

        //public IActionResult SearchData(string custName, string startDate, string endDate)
        public IActionResult SearchData(string suppId, string startDate, string endDate)
        {
            var hasSupplierId = !string.IsNullOrWhiteSpace(suppId);
            var dtStartDate = Utilities.ParseStartDateOrDefaultUtc(startDate);
            var dtEndtDate = Utilities.ParseEndDateOrDefaultUtc(endDate);
            IQueryable<PO> selectedPOes = _db.POes;

            if (hasSupplierId)
            {
                var intSuppId = Int32.Parse(suppId.Trim());
                selectedPOes = selectedPOes.Where(so => so.SupplierId == intSuppId);
            }

            selectedPOes = selectedPOes.Where(so => so.Date >= dtStartDate && so.Date <= dtEndtDate);

            GetTotalBalance(ref selectedPOes);
            Dictionary<int, int> LstMaxSerialNo = new Dictionary<int, int>();
            int thisSerial = 0;
            foreach (PO itm in selectedPOes)
            {
                thisSerial = (int)itm.Supplier.POes.Max(x => x.POSerial);
                if (!LstMaxSerialNo.ContainsKey((int)itm.SupplierId))
                {
                    LstMaxSerialNo.Add(itm.Supplier.Id, thisSerial);
                }
                //itm.Id = Encryption.Encrypt(itm.Id, "BZNS");
                itm.Id = string.Join("-", ASCIIEncoding.ASCII.GetBytes(Encryption.Encrypt(itm.Id, "BZNS")));
            }

            return PartialView("_SelectedPOPR", selectedPOes.OrderByDescending(i => i.Date).ToList());

            //return View("Some thing went wrong");


        }
        public IActionResult PerMonthPurchase(int productId)
        {
            IQueryable<PO> pOes = _db.POes.Include(s => s.Supplier);

            //sOes = _db.SOes.Where(x => x.SODs.Where(y => y.ProductId == productId));

            List<POD> lstPODs = _db.PODs.Where(x => x.ProductId == productId && x.SaleType == false).ToList();
            List<PO> lstSlectedPO = new List<PO>();
            foreach (POD lpod in lstPODs)
            {
                if (lstSlectedPO.Where(x => x.Id == lpod.POId).FirstOrDefault() == null)
                {
                    lstSlectedPO.Add(lpod.PO);
                }
            }

            pOes = lstSlectedPO.ToList().AsQueryable();
            foreach (PO itm in pOes)
            {
                //itm.Id = Encryption.Encrypt(itm.Id, "BZNS");
                itm.Id = string.Join("-", ASCIIEncoding.ASCII.GetBytes(Encryption.Encrypt(itm.Id, "BZNS")));
            }
            ViewBag.ProductName = _db.Products.FirstOrDefault(x => x.Id == productId).Name;
            return View("PerMonthPurchase", pOes.OrderBy(i => i.Date).ToList());
        }

        public IActionResult SearchProduct(int productId)
        {
            IQueryable<PO> pOes = _db.POes.Include(s => s.Supplier);

            List<POD> lstPODs = _db.PODs.Where(x => x.ProductId == productId).ToList();
            List<PO> lstSlectedPO = new List<PO>();
            foreach (POD lpod in lstPODs)
            {
                if (lstSlectedPO.Where(x => x.Id == lpod.POId).FirstOrDefault() == null)
                {
                    lstSlectedPO.Add(lpod.PO);
                }


            }

            pOes = lstSlectedPO.ToList().AsQueryable();

            //sOes.ForEachAsync(m => m.Id = Encryption.Encrypt(m.Id, "BZNS"));
            //var sOes = _db.SOes.Where(s => s.SaleReturn == false);
            GetTotalBalance(ref pOes);
            foreach (PO itm in pOes)
            {

                //itm.Id = Encryption.Encrypt(itm.Id, "BZNS");
                itm.Id = string.Join("-", ASCIIEncoding.ASCII.GetBytes(Encryption.Encrypt(itm.Id, "BZNS")));
            }
            ViewBag.Suppliers = _db.Suppliers;
            return View("Index", pOes.OrderByDescending(i => i.Date).ToList());
        }

        private void GetTotalBalance(ref IQueryable<PO> POes)
        {
            var supplierIds = POes
                .Where(x => x.SupplierId.HasValue)
                .Select(x => x.SupplierId!.Value)
                .Distinct()
                .ToList();

            var totalBalance = _db.Suppliers
                .Where(x => supplierIds.Contains(x.Id))
                .Sum(x => (decimal?)x.Balance) ?? 0;

            ViewBag.TotalBalance = totalBalance;

        }
        //[ChildActionOnly]
        //public PartialViewResult _SelectedPOPR()
        //{

        //    return PartialView(_db.POes);
        //}
        
        // GET: POes/Details/5
        public IActionResult Details(string id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            PO? pO = _db.POes.Find(id);
            if (pO == null)
            {
                return NotFound();
            }
            return View(pO);
        }

        // GET: POes/Create
        public IActionResult Create(string IsReturn)
        {
            //ViewBag.SupplierId = new SelectList(_db.Suppliers, "Id", "Name");
            //ViewBag.Products = _db.Products;

            //int maxId = _db.Suppliers.Max(p => p.Id);
            int maxId = _db.Suppliers.DefaultIfEmpty().Max(p => p == null ? 0 : p.Id);
            maxId += 1;
            ViewBag.SuggestedNewSuppId = maxId;


            PurchaseOrderViewModel purchaseOrderViewModel = new PurchaseOrderViewModel();
            purchaseOrderViewModel.Suppliers = _db.Suppliers;
            purchaseOrderViewModel.Products = _db.Products.Where(x => x.Saleable == true);
            ViewBag.IsReturn = IsReturn;
            return View(purchaseOrderViewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("Name,Address", Prefix = "Supplier")] Supplier Supplier, [Bind("BillAmount,Balance,PrevBalance,BillPaid,Discount,SupplierId,Remarks,Remarks2,PaymentMethod,PaymentDetail,PurchaseReturn", Prefix = "PurchaseOrder")] PO pO, [Bind("ProductId,Quantity,SaleType,PerPack,IsPack,PurchasePrice", Prefix = "PurchaseOrderDetail")] List<POD> pOD)

        {
            //PO pO = new PO();
            if (ModelState.IsValid)
            {
                var purchaseOrderDetails = (pOD ?? new List<POD>())
                    .Where(pod => pod.ProductId.HasValue && pod.ProductId.Value > 0 && pod.Quantity.HasValue && pod.Quantity.Value > 0)
                    .ToList();

                if (purchaseOrderDetails.Count == 0)
                {
                    ModelState.AddModelError("", "Please select at least one valid product.");
                    return View(BuildCreatePurchaseOrderViewModel(pO, Supplier));
                }

                Supplier supp = _db.Suppliers.FirstOrDefault(x => x.Id == pO.SupplierId);
                if (supp == null)
                {//its means new customer
                    //pO.SupplierId = 10;
                    //int maxId = _db.Suppliers.Max(p => p.Id);
                    int maxId = _db.Suppliers.DefaultIfEmpty().Max(p => p == null ? 0 : p.Id);
                    maxId += 1;
                    Supplier.Id = maxId;
                    Supplier.Balance = pO.Balance;
                    _db.Suppliers.Add(Supplier);
                    //_db.SaveChanges();
                }
                else
                {//its means old customer. old customer balance should be updated.
                    //Supplier.Id = (int)pO.SupplierId;
                    supp.Balance = pO.Balance;
                    _db.Entry(supp).State = EntityState.Modified;
                    //_db.SaveChanges();

                    //Payment payment = new Payment();
                    //payment = _db.Payments.Find(orderId);
                    //payment.Status = true;
                    //_db.Entry(payment).State = EntityState.Modified;
                    //_db.SaveChanges();

                }

                ////////////////////////////////////////
                //int maxId = _db.POes.Max(p => p.Auto);
                int maxId1 = (int)_db.POes.DefaultIfEmpty().Max(p => p == null ? 0 : p.POSerial);
                maxId1 += 1;
                pO.POSerial = maxId1;
                //pO.Date = DateTime.Now;
                pO.Date = DateTime.UtcNow;
                //pO.SaleReturn = false;
                pO.Id = System.Guid.NewGuid().ToString().ToUpper();
                pO.PurchaseOrderAmount = 0;
                pO.PurchaseReturnAmount = 0;
                pO.PurchaseOrderQty = 0;
                pO.PurchaseReturnQty = 0;
                var emp = HttpContext.Session.GetCurrentUser();
                if (emp == null)
                    return RedirectToAction("Login", "UserManagement");
                pO.EmployeeId = emp.Id;
                _db.POes.Add(pO);
                //_db.SaveChanges();
                int sno = 0;
                if (purchaseOrderDetails.Count > 0)
                {
                    //pOD.RemoveAll(so => so.ProductId == null);
                    foreach (POD pod in purchaseOrderDetails)
                    {
                        sno += 1;
                        pod.PODId = sno;
                        pod.PO = pO;
                        pod.POId = pO.Id;

                        Product product = _db.Products.FirstOrDefault(x => x.Id == pod.ProductId);
                        if (product == null)
                        {
                            ModelState.AddModelError("", $"The selected product for line item {sno} could not be found. Please choose it again from the list.");
                            return View(BuildCreatePurchaseOrderViewModel(pO, Supplier));
                        }

                        //dont do this. when user made a bill and chnage sale price. it does not reflect in bill and calculations geting wrong
                        //pod.PurchasePrice = product.PurchasePrice;
                        if (pod.Quantity == null) { pod.Quantity = 0; }
                        pod.OpeningStock = product.Stock;
                        if (pod.SaleType == true)//return
                        {
                            
                            if (pod.IsPack == false)
                            {
                                pO.PurchaseReturnAmount += (decimal)(pod.Quantity * pod.PurchasePrice);
                                decimal qty = (decimal)pod.Quantity;// / (decimal)product.PerPack;
                                product.Stock -= qty;
                                pO.PurchaseReturnQty += qty;//(int)sod.Quantity;
                                
                            }
                            else
                            {
                                pO.PurchaseReturnAmount += (decimal)(pod.Quantity * pod.PurchasePrice * pod.PerPack);
                                product.Stock -= (int)pod.Quantity * pod.PerPack;

                                pO.PurchaseReturnQty += (int)pod.Quantity * pod.PerPack;
                                
                            }
                        }
                        else//purchase
                        {
                            
                            if (pod.IsPack == false)
                            {//piece
                                pO.PurchaseOrderAmount += (decimal)(pod.Quantity * pod.PurchasePrice);
                                //int pieceSold = (int)(sod.Quantity * product.Stock);
                                decimal qty = (decimal)pod.Quantity;// / (decimal)product.PerPack;
                                product.Stock += qty;

                                pO.PurchaseOrderQty += qty;//(int)sod.Quantity;
                                
                            }
                            else
                            {//pack

                                pO.PurchaseOrderAmount += (decimal)(pod.Quantity * pod.PurchasePrice * pod.PerPack);
                                product.Stock += (int)pod.Quantity * pod.PerPack;

                                pO.PurchaseOrderQty += (int)pod.Quantity * pod.PerPack;

                            }

                        }

                    }
                    _db.PODs.AddRange(purchaseOrderDetails);
                }
                _db.SaveChanges();


                //SqlParameter param1 = new SqlParameter("@PurchaseOrderID", pO.Id);
                ////var result = _db.Database.ExecuteSqlCommand("spSOReceipt @PurchaseOrderID", param1);
                //var result = _db.Database.SqlQuery<Object>("spSOReceipt @PurchaseOrderID", param1);


                //var cr = new ReportDocument();
                //cr.Load(@"E:\PROJECTS\MYBUSINESS - v.4.6\MYBUSINESS\Reports\SOReceipt.rpt");
                //cr.DataDefinition.RecordSelectionFormula = "{PurchaseOrderID} = '" + pO.Id + "'";
                //cr.PrintToPrinter(1, true, 0, 0);


                ////////////////////////finalized
                //string pathh = HttpRuntime.AppDomainAppPath;
                //ReportDocument reportDocument = new ReportDocument();
                //reportDocument.Load(pathh + @"Reports\SOReceipt.rpt");
                //reportDocument.SetParameterValue("@PurchaseOrderID", pO.Id);
                //System.Drawing.Printing.PrinterSettings printerSettings = new System.Drawing.Printing.PrinterSettings();
                ////printerSettings.PrinterName = PrinterName;
                //reportDocument.PrintToPrinter(printerSettings, new PageSettings(), false);
                /////////////////////////////////////


                string POId = string.Join("-", ASCIIEncoding.ASCII.GetBytes(Encryption.Encrypt(pO.Id, "BZNS")));
                //return PrintSO(POId);
                //return PrintSO3(POId);
                return RedirectToAction("PrintSO3", new { id = POId });
                //return RedirectToAction("Index");
            }

            //ViewBag.SupplierId = new SelectList(_db.Suppliers, "Id", "Name", pO.SupplierId);
            //return View(pO);
            return View(BuildCreatePurchaseOrderViewModel(pO, Supplier));
            //return View();

        }

        private PurchaseOrderViewModel BuildCreatePurchaseOrderViewModel(PO? purchaseOrder = null, Supplier? supplier = null)
        {
            ViewBag.SuggestedNewSuppId = _db.Suppliers.DefaultIfEmpty().Max(p => p == null ? 0 : p.Id) + 1;
            ViewBag.IsReturn = (purchaseOrder?.PurchaseReturn ?? false).ToString().ToLower();

            return new PurchaseOrderViewModel
            {
                PurchaseOrder = purchaseOrder,
                Supplier = supplier,
                Suppliers = _db.Suppliers,
                Products = _db.Products.Where(x => x.Saleable == true)
            };
        }
        //public void PrintSO(string POId)
        //{
        //    POId = Encryption.Decrypt(POId, "BZNS");
        //    string pathh = HttpRuntime.AppDomainAppPath;
        //    ReportDocument reportDocument = new ReportDocument();
        //    reportDocument.Load(pathh + @"Reports\POPRReceipt2.rpt");
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

        //    reportDocument.SetParameterValue("@PurchaseOrderID", POId);
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

            var po = _db.POes.FirstOrDefault(y => y.Id == id);
            if (po == null)
                return NotFound();

            var emp = _db.Employees.FirstOrDefault(x => x.Id == po.EmployeeId);
            var reportFile = string.Equals(emp?.Login, "LahoreKarachi", StringComparison.Ordinal)
                ? "Purchase_LahoreKarachi.rdlc"
                : "Purchase_GoldPanel.rdlc";
            var reportPath = Path.Combine(_env.WebRootPath, "Reports", reportFile);

            using var lr = new LocalReport();
            lr.ReportPath = reportPath;
            lr.DataSources.Add(new ReportDataSource("DataSet1", _db.spPOReport(id).ToList()));
            lr.SetParameters(new[] { new ReportParameter("PurchaseOrderID", id) });

            byte[] bytes = lr.Render("PDF");

            Response.Headers.Append("Content-Disposition", $"inline; filename=\"Purchase.{extension}\"");
            return File(bytes, mimeType);
        }

        public decimal GetPreviousBalance(int id)
        {
            IQueryable lstSO = _db.POes.Where(x => x.SupplierId == id);

            //lstSO.ForEachAsync(c => { c. = 0; c.GroupID = 0; c.CompanyID = 0; });
            decimal POAmount = 0;
            decimal PRAmount = 0;
            foreach (PO itm in lstSO)
            {
                POAmount += (decimal)itm.PurchaseOrderAmount;
                PRAmount += (decimal)itm.PurchaseReturnAmount;

            }

            return (POAmount - PRAmount);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        //public IActionResult Edit([Bind("BillAmount,Balance,BillPaid,Discount", Prefix = "PurchaseOrder")] PO pO, [Bind("ProductId,Quantity", Prefix = "PurchaseOrderDetail")] List<POD> pOD)
        public IActionResult Edit(PurchaseOrderViewModel purchaseOrderViewModel1)
        {
            PO newPO = purchaseOrderViewModel1.PurchaseOrder;
            List<POD> newPODs = purchaseOrderViewModel1.PurchaseOrderDetail;
            if (ModelState.IsValid)
            {
                newPO.Id = Encryption.Decrypt(purchaseOrderViewModel1.PurchaseOrder.Id, "BZNS");//
                PO PO = _db.POes.Where(x => x.Id == newPO.Id).FirstOrDefault();
                PO.Date = DateTime.UtcNow;//
                //PO.PurchaseReturn = false;//
                PO.BillAmount = newPO.BillAmount;//
                PO.Discount = newPO.Discount;//
                PO.BillPaid = newPO.BillPaid;//
                PO.Balance = newPO.Balance;//
                PO.Remarks = newPO.Remarks;//
                PO.Remarks2 = newPO.Remarks;//
                PO.PaymentMethod = newPO.PaymentMethod;
                PO.PaymentDetail = newPO.PaymentDetail;

                //PO.POSerial = newPO.POSerial;//should be unchanged

                ///////////////////////////////////////////

                //Supplier cust = _db.Suppliers.FirstOrDefault(x => x.Id == newPO.SupplierId);
                Supplier supplier = _db.Suppliers.Where(x => x.Id == newPO.SupplierId).FirstOrDefault();
                if (supplier == null)
                {//its means new supplier(not in db)
                 //PO.SupplierId = 10;
                 //int maxId = _db.Suppliers.Max(p => p.Id);
                    supplier = purchaseOrderViewModel1.Supplier;
                    int maxId = _db.Suppliers.DefaultIfEmpty().Max(p => p == null ? 0 : p.Id);
                    maxId += 1;

                    supplier.Id = maxId;
                    //supplier.Balance = newPO.Balance;
                    _db.Suppliers.Add(supplier);
                }
                else
                {
                    _db.Entry(supplier).State = EntityState.Modified;
                }

                if (PO.SupplierId != newPO.SupplierId)
                {//POme other db supplier
                 //first revert the previous supplier balance 
                    Supplier oldSupplier = _db.Suppliers.Where(x => x.Id == PO.SupplierId).FirstOrDefault();
                    oldSupplier.Balance = _db.POes.Where(x => x.Id == PO.Id).FirstOrDefault().PrevBalance;
                    _db.Entry(oldSupplier).State = EntityState.Modified;
                }

                PO.PrevBalance = newPO.PrevBalance;//
                // assign balance of this supplier
                //Supplier supplier = _db.Suppliers.Where(x => x.Id == newPO.SupplierId).FirstOrDefault();
                supplier.Balance = newPO.Balance;
                //assign supplier and supplierId in PO
                PO.SupplierId = newPO.SupplierId;
                PO.Supplier = supplier;

                /////////////////////////////////////////////////////////////////////////////



                List<POD> oldPODs = _db.PODs.Where(x => x.POId == newPO.Id).ToList();

                //handling old prodcts quantity. add old quantites back to the stock, then in next loop product quantity will be minus. this is simple and stateforward.
                foreach (POD pod in oldPODs)
                {
                    Product product = _db.Products.FirstOrDefault(x => x.Id == pod.ProductId);
                    if (pod.SaleType == false)//purchase
                    {
                        //product.Stock -= pod.Quantity;

                        if (pod.IsPack == false)
                        {
                            decimal qty = (decimal)pod.Quantity;// / (decimal)product.PerPack;
                            product.Stock -= qty;
                        }
                        else
                        {
                            product.Stock -= (int)pod.Quantity * pod.PerPack;
                        }

                    }
                    else//return
                    {
                        //product.Stock += pod.Quantity;

                        if (pod.IsPack == false)
                        {
                            decimal qty = (decimal)pod.Quantity;// / (decimal)product.PerPack;
                            product.Stock += qty;
                        }
                        else
                        {
                            product.Stock += (int)pod.Quantity * pod.PerPack;
                        }


                    }
                    _db.Entry(product).State = EntityState.Modified;
                }

                _db.PODs.RemoveRange(oldPODs);
                //////////////////////////////////////////////////////////////////////////////

                PO.PurchaseOrderAmount = 0;
                PO.PurchaseReturnAmount = 0;
                PO.PurchaseOrderQty = 0;
                PO.PurchaseReturnQty = 0;
                //PO.Profit = 0;
                int sno = 0;

                if (newPODs != null)
                {

                    foreach (POD pod in newPODs)
                    {
                        sno += 1;
                        pod.PODId = sno;
                        pod.PO = PO;
                        pod.POId = PO.Id;

                        Product product = _db.Products.FirstOrDefault(x => x.Id == pod.ProductId);
                        //POd.purchasePrice is now from view
                        //POd.PurchasePrice = product.PurchasePrice;
                        //dont do this. calculation are geting wrong. when user open an old bill and just press save. all calculations distrubs
                        //pod.PurchasePrice = product.PurchasePrice;
                        if (pod.Quantity == null) { pod.Quantity = 0; }
                        pod.OpeningStock = product.Stock;
                        if (pod.SaleType == true)//return
                        {
                            if (pod.IsPack == false)
                            {
                                PO.PurchaseReturnAmount += (decimal)(pod.Quantity * pod.PurchasePrice);
                                decimal qty = (decimal)pod.Quantity;// / (decimal)product.PerPack;
                                product.Stock -= qty;
                                PO.PurchaseReturnQty += qty;//(int)sod.Quantity;

                            }
                            else
                            {
                                PO.PurchaseReturnAmount += (decimal)(pod.Quantity * pod.PurchasePrice * pod.PerPack);
                                product.Stock -= (int)pod.Quantity * pod.PerPack;

                                PO.PurchaseReturnQty += (int)pod.Quantity *pod.PerPack;

                            }
                        }
                        else//purchase
                        {
                            if (pod.IsPack == false)
                            {//piece
                                PO.PurchaseOrderAmount += (decimal)(pod.Quantity * pod.PurchasePrice);
                                //int pieceSold = (int)(sod.Quantity * product.Stock);
                                decimal qty = (decimal)pod.Quantity;// / (decimal)product.PerPack;
                                product.Stock += qty;

                                PO.PurchaseOrderQty += qty;//(int)sod.Quantity;

                            }
                            else
                            {//pack

                                PO.PurchaseOrderAmount += (decimal)(pod.Quantity * pod.PurchasePrice * pod.PerPack);
                                product.Stock += (int)pod.Quantity * pod.PerPack;

                                PO.PurchaseOrderQty += (int)pod.Quantity * pod.PerPack;

                            }
                        }

                    }
                    //PO.Profit -= (decimal)PO.Discount;
                    _db.Entry(PO).State = EntityState.Modified;
                    _db.Entry(PO).Property(x => x.POSerial).IsModified = false;
                    _db.PODs.AddRange(newPODs);

                }
                _db.SaveChanges();

                return RedirectToAction("Index");
            }

            //ViewBag.SupplierId = new SelectList(_db.Suppliers, "Id", "Name", PO.SupplierId);
            //return View(PO);
            PurchaseOrderViewModel purchaseOrderViewModel = new PurchaseOrderViewModel();

            purchaseOrderViewModel.Products = _db.Products;
            return View(purchaseOrderViewModel);
            //return View();
        }
        private string Decode(string id)
        {
            byte[] BytesArr = id.Split('-').Select(byte.Parse).ToArray();
            id = new string(Encoding.UTF8.GetString(BytesArr).ToCharArray());
            id = Encryption.Decrypt(id, "BZNS");
            return id;
        }




        // GET: POes/Edit/5
        public IActionResult Edit(string id, bool update)
        {

            if (id == null)
            {

                return BadRequest();
            }

            //byte[] BytesArr = id.Split('-').Select(byte.Parse).ToArray();
            //id = new string( Encoding.UTF8.GetString(BytesArr).ToCharArray());
            //id = Encryption.Decrypt(id,"BZNS");

            int maxId = _db.Suppliers.DefaultIfEmpty().Max(p => p == null ? 0 : p.Id);
            maxId += 1;
            ViewBag.SuggestedNewSuppId = maxId;

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

            PO pO = _db.POes.Find(id);
            if (pO == null)
            {
                return NotFound();
            }
            PurchaseOrderViewModel purchaseOrderViewModel = new PurchaseOrderViewModel();
            List<POD> pod = _db.PODs.Where(x => x.POId == id).ToList();
            purchaseOrderViewModel.Products = _db.Products;
            purchaseOrderViewModel.Suppliers = _db.Suppliers;
            purchaseOrderViewModel.PurchaseOrderDetail = pod;
            pO.Id = Encryption.Encrypt(pO.Id, "BZNS");
            purchaseOrderViewModel.PurchaseOrder = pO;
            int orderQty = 0;
            int orderQtyPiece = 0;//orderQtyPiece 'P for piece' 
            int returnQty = 0;
            int returnQtyPiece = 0;//orderQtyPiece 'P for piece' 
            foreach (var item in pod)
            {
                if (pO.PurchaseReturn == false)
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
            //ViewBag.SupplierId = new SelectList(_db.Suppliers, "Id", "Name", sO.SupplierId);
            ViewBag.SupplierName = pO.Supplier.Name;
            ViewBag.SupplierAddress = pO.Supplier.Address;
            decimal subTotal = (decimal)(pO.PurchaseOrderAmount - pO.PurchaseReturnAmount - pO.Discount);
            ViewBag.SubTotal = subTotal;
            ViewBag.Total = subTotal + (decimal)pO.PrevBalance;
            ViewBag.IsUpdate = update;
            ViewBag.IsReturn = pO.PurchaseReturn.ToString().ToLower();
            return View(purchaseOrderViewModel);
        }


        // GET: POes/Delete/5
        public IActionResult Delete(string id)
        {
            return null;
            if (id == null)
            {
                return BadRequest();
            }
            id = Decode(id);
            PO pO = _db.POes.Find(id);
            if (pO == null)
            {
                return NotFound();
            }
            return View(pO);
        }

        // POST: POes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(string id)
        {
            return null;
            id = Decode(id);

            List<POD> oldSODs = _db.PODs.Where(x => x.POId == id).ToList();
            //handling old prodcts quantity. add old quantites back to the stock, then in next loop product quantity will be minus. this si simple and stateforward.
            foreach (POD pod in oldSODs)
            {
                Product product = _db.Products.FirstOrDefault(x => x.Id == pod.ProductId);
                product.Stock += pod.Quantity;
            }
            _db.PODs.RemoveRange(oldSODs);

            PO pO = _db.POes.Find(id);
            _db.POes.Remove(pO);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }

    }

}

