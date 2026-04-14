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
using ShopOn.Web.Infrastructure;
using MyPaymentMethod = ShopOn.Web.Infrastructure.MyPaymentMethod;
using ShopOn.Web.Models;
using EasyInventory.PgData.Entities;

namespace ShopOn.Web.Controllers
{
    public class PaymentsController : Controller
    {
        private readonly EasyInventoryDbContext _db;

        public PaymentsController(EasyInventoryDbContext db) { _db = db; }
        private Utilities util = new Utilities();
        // GET: Payments
        //public IActionResult Index()
        //{
        //    var payments = _db.Payments.Include(p => p.SO);
        //    return View(payments.ToList());
        //}
        public IActionResult Index(string orderId)
        {
            if (string.IsNullOrWhiteSpace(orderId))
            {
                return RedirectToAction("Index", "Home");
            }

            orderId = Decode(orderId);
            SO sO = _db.SOes.Where(x => x.Id == orderId).FirstOrDefault();
            if (sO == null)
            {
                return NotFound();
            }

            ViewBag.Customer = sO.Customer.Name;
            ViewBag.OrderNo = sO.SOSerial;
            ViewBag.id = sO.Id;
            //var payments = _db.Payments.Include(p => p.SO);
            var payments = _db.Payments.Where(p => p.SOId == orderId);
            return View(payments.ToList());
        }

        // GET: Payments/Create
        public IActionResult Create(string custName, string orderNo, string id)
        {
            ViewBag.custName = custName;
            ViewBag.orderNo = orderNo;
            id = Decode(id);
            ViewBag.Payments = _db.Payments.Where(p => p.SOId == id).OrderBy(i => i.Id);
            ViewBag.id = string.Join("-", ASCIIEncoding.ASCII.GetBytes(Encryption.Encrypt(id, "BZNS")));//Decode( id);
            //ViewBag.SOId = new SelectList(_db.SOes.OrderByDescending(i => i.Date), "Id", "Remarks");
            ViewBag.TodayDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time")).ToString("dd-MMM-yyy");
            SO? sO = _db.SOes.Include(s => s.Customer).FirstOrDefault(x => x.Id == id);
            if (sO == null)
                return NotFound();

            decimal payable = util.GetPayableAmount((decimal)sO.SaleOrderAmount, (decimal)(sO.SaleReturnAmount ?? 0), (decimal)(sO.Discount ?? 0));
            ViewBag.PayableAmount = payable;
            var LstPymnt = _db.Payments.Where(x => x.SOId == id).ToList();

            decimal billPaidUptilnow = 0;
            LstPymnt.ForEach(x => billPaidUptilnow += x.PaymentAmount);

            ViewBag.Paid = billPaidUptilnow;
            ViewBag.Balance = util.GetBalanceAmount(payable, billPaidUptilnow);
            return View();
        }

        // POST: Payments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("SOId,PaymentMethod,PaymentAmount,Id,ReceivedDate,Remarks")] Payment payment)
        {
            string codedId = payment.SOId;
            
            string SOId = Decode(payment.SOId ?? "");
            SO? sO = _db.SOes.Include(s => s.Customer).FirstOrDefault(x => x.Id == SOId);
            if (sO == null)
                return BadRequest();
            //Customer thisCust= sO.Customer;
            int maxId = _db.Payments.DefaultIfEmpty().Max(p => p == null ? 0 : p.Id);
            maxId += 1;
            payment.Id = maxId;
            if (ModelState.IsValid)
            {

                payment.ReceivedDate = DateTime.UtcNow;
                payment.SOId = SOId;
                _db.Payments.Add(payment);
                ////////////////add to SO table
                //SO newSO = new SO();
                //newSO.Id = System.Guid.NewGuid().ToString().ToUpper();
                //newSO.SOSerial = sO.SOSerial;
                //newSO.BillAmount = 0;
                sO.BillPaid += payment.PaymentAmount;
                sO.PrevBalance = sO.Customer.Balance; //customer last balnce will go to prev balance
                sO.Customer.Balance-= payment.PaymentAmount;//minus this amout from total balance also
                sO.Balance = sO.Customer.Balance;
                sO.Date = DateTime.UtcNow;
                
                //_db.SOes.Add(sO);
                _db.Entry(sO).State = EntityState.Modified;
                /////////////
                _db.SaveChanges();

                string orderId = string.Join("-", ASCIIEncoding.ASCII.GetBytes(Encryption.Encrypt(payment.SOId, "BZNS")));
                //public IActionResult Create(string custName, string orderNo, string id)
                return RedirectToAction("Create", new { custName=sO.Customer.Name, orderNo=sO.SOSerial, id=orderId });
            }
            return RedirectToAction("Create", new { custName=sO.Customer.Name, orderNo=sO.SOSerial, id=codedId });

            //SO sO1 = _db.SOes.Where(x => x.Id == SOId).FirstOrDefault();
            //ViewBag.custName = sO1.Customer.Name;
            //ViewBag.orderNo = sO1.SOSerial;
            //ViewBag.Payments = _db.Payments.Where(p => p.SOId == SOId);
            //ViewBag.id = string.Join("-", ASCIIEncoding.ASCII.GetBytes(Encryption.Encrypt(SOId, "BZNS")));//Decode( id);
            ////ViewBag.SOId = new SelectList(_db.SOes.OrderByDescending(i => i.Date), "Id", "Remarks");
            //ViewBag.TodayDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time")).ToString("dd-MMM-yyy");
            //return View(payment);
        }
        // POST: Payments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit([Bind("SOId,PaymentMethod,PaymentAmount,Id,ReceivedDate,Remarks")] Payment payment)
        {
            Payment oldPymnt = _db.Payments.Where(x => x.Id == payment.Id).FirstOrDefault();
            SO sO = _db.SOes.Where(x => x.Id == payment.SOId).FirstOrDefault();
            if (ModelState.IsValid)
            {
                

                

                ////first recover old payment 
                sO.BillPaid -= oldPymnt.PaymentAmount;
                sO.Customer.Balance += oldPymnt.PaymentAmount;//minus this amout from total balance also
                decimal addedPmnts = 0;
                foreach (Payment pmt in _db.Payments)
                {
                    if (pmt.SOId == payment.SOId && pmt.Id != payment.Id)
                    {
                        addedPmnts += pmt.PaymentAmount;
                    }
                }
                decimal a = (decimal)sO.Discount + (decimal)sO.SaleReturnAmount + (decimal)addedPmnts;
                decimal b = (decimal)sO.BillAmount - a;
                sO.PrevBalance = sO.Customer.Balance - b; //sO.Customer.Balance - (double) sO.SaleOrderAmount ; //customer last balance will go to prev balance
                sO.Balance = sO.Customer.Balance;
                //sO.Date = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time"));


                //_db.SOes.Add(sO);
                

                ////then update new payment 


                sO.BillPaid += payment.PaymentAmount;
                sO.PrevBalance = sO.Customer.Balance; //customer last balnce will go to prev balance
                sO.Customer.Balance -= payment.PaymentAmount;//minus this amout from total balance also
                sO.Balance = sO.Customer.Balance;
                sO.Date = DateTime.UtcNow;


                oldPymnt.PaymentAmount = payment.PaymentAmount;
                oldPymnt.PaymentMethod= payment.PaymentMethod;
                oldPymnt.ReceivedDate = DateTime.UtcNow;
                oldPymnt.Remarks = payment.Remarks;

                _db.Entry(oldPymnt).State = EntityState.Modified;
                _db.Entry(sO).State = EntityState.Modified;






                _db.SaveChanges();
                //return RedirectToAction("Index");
                //SO sO = _db.SOes.Where(x => x.Id == payment.SOId).FirstOrDefault();

                string orderId = string.Join("-", ASCIIEncoding.ASCII.GetBytes(Encryption.Encrypt(payment.SOId, "BZNS")));
                //public IActionResult Create(string custName, string orderNo, string id)
                return RedirectToAction("Create", new { custName = sO.Customer.Name, orderNo = sO.SOSerial, id = orderId });
                //return RedirectToAction("Create", new { orderId });
            }
            ViewBag.SOId = new SelectList(_db.SOes.ToList(), "Id", "Remarks", payment.SOId);
            return View(payment);
        }
        // GET: Payments/Edit/5
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            List<MyPaymentMethod> myOptionLst = new List<MyPaymentMethod> {
                            new MyPaymentMethod {
                                Text = "Cash",
                                Value = "Cash"
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



            ViewBag.OptionLst = myOptionLst;

            Payment payment = _db.Payments.Find(id);
            if (payment == null)
            {
                return NotFound();
            }
            //ViewBag.SOId = new SelectList(_db.SOes, "Id", "Remarks", payment.SOId);
            return View(payment);
        }

       

        // GET: Payments/Delete/5
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            Payment payment = _db.Payments.Find(id);
            if (payment == null)
            {
                return NotFound();
            }
            return View(payment);
        }

        // POST: Payments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            Payment payment = _db.Payments.Find(id);
            _db.Payments.Remove(payment);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }
        // GET: Payments/Details/5
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            Payment payment = _db.Payments.Find(id);
            if (payment == null)
            {
                return NotFound();
            }
            return View(payment);
        }
        private string Decode(string id)
        {
            byte[] BytesArr = id.Split('-').Select(byte.Parse).ToArray();
            id = new string(Encoding.UTF8.GetString(BytesArr).ToCharArray());
            id = Encryption.Decrypt(id, "BZNS");
            return id;
        }
    }
}

