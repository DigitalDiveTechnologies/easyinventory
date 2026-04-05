using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace MYBUSINESS.Models
{
    public partial class BusinessContext
    {
        /// <summary>LINQ replacement for SQL Server stored procedure spSOReport (not ported to PostgreSQL).</summary>
        public IEnumerable<spSOReport_Result> spSOReport(string saleOrderID)
        {
            if (string.IsNullOrWhiteSpace(saleOrderID))
                return Enumerable.Empty<spSOReport_Result>();

            var so = SOes
                .Include(s => s.SODs)
                .Include(s => s.Customer)
                .FirstOrDefault(s => s.Id == saleOrderID);

            if (so == null)
                return Enumerable.Empty<spSOReport_Result>();

            var biz = MyBusinessInfoes.AsNoTracking().FirstOrDefault();
            var list = new List<spSOReport_Result>();

            foreach (var sod in so.SODs)
            {
                Product prod = null;
                if (sod.ProductId.HasValue)
                    prod = Products.AsNoTracking().FirstOrDefault(p => p.Id == sod.ProductId.Value);

                var qty = sod.Quantity ?? 0;
                var price = sod.SalePrice ?? 0m;
                var itemTotal = qty * price;
                var saleType = sod.SaleType == true ? "Return" : "Sale";

                list.Add(new spSOReport_Result
                {
                    Id = so.Id,
                    SOSerial = so.SOSerial,
                    SaleOrderQty = so.SaleOrderQty,
                    SaleReturnQty = so.SaleReturnQty,
                    Name = so.Customer != null ? so.Customer.Name : null,
                    ProductName = prod != null ? prod.Name : null,
                    SalePrice = sod.SalePrice,
                    Quantity = sod.Quantity,
                    PerPack = sod.PerPack,
                    IsPack = sod.IsPack,
                    BizName = biz != null ? biz.BizName : null,
                    BizAddress = biz != null ? biz.BizAddress : null,
                    Mobile = biz != null ? biz.Mobile : null,
                    Email = biz != null ? biz.Email : null,
                    Website = biz != null ? biz.Website : null,
                    Tagline = biz != null ? biz.Tagline : null,
                    ItemTotal = itemTotal,
                    SaleType = saleType,
                    SaleOrderAmount = so.SaleOrderAmount,
                    SaleReturnAmount = so.SaleReturnAmount,
                    BillPaid = so.BillPaid,
                    Discount = so.Discount,
                    Balance = so.Balance,
                    PrevBalance = so.PrevBalance,
                    Date = so.Date,
                    Remarks = sod.Remarks ?? so.Remarks
                });
            }

            return list;
        }

        /// <summary>LINQ replacement for SQL Server stored procedure spPOReport.</summary>
        public IEnumerable<spPOReport_Result> spPOReport(string purchaseOrderID)
        {
            if (string.IsNullOrWhiteSpace(purchaseOrderID))
                return Enumerable.Empty<spPOReport_Result>();

            var po = POes
                .Include(p => p.PODs)
                .Include(p => p.Supplier)
                .FirstOrDefault(p => p.Id == purchaseOrderID);

            if (po == null)
                return Enumerable.Empty<spPOReport_Result>();

            var biz = MyBusinessInfoes.AsNoTracking().FirstOrDefault();
            var list = new List<spPOReport_Result>();

            foreach (var pod in po.PODs)
            {
                Product prod = null;
                if (pod.ProductId.HasValue)
                    prod = Products.AsNoTracking().FirstOrDefault(p => p.Id == pod.ProductId.Value);

                var qty = pod.Quantity ?? 0;
                var price = pod.PurchasePrice ?? 0m;
                var itemTotal = qty * price;
                var saleType = pod.SaleType == true ? "Return" : "Purchase";

                list.Add(new spPOReport_Result
                {
                    Id = po.Id,
                    POSerial = po.POSerial,
                    PurchaseOrderQty = po.PurchaseOrderQty,
                    PurchaseReturnQty = po.PurchaseReturnQty,
                    Name = po.Supplier != null ? po.Supplier.Name : null,
                    ProductName = prod != null ? prod.Name : null,
                    PurchasePrice = pod.PurchasePrice,
                    Quantity = pod.Quantity,
                    PerPack = pod.PerPack,
                    IsPack = pod.IsPack,
                    BizName = biz != null ? biz.BizName : null,
                    BizAddress = biz != null ? biz.BizAddress : null,
                    Mobile = biz != null ? biz.Mobile : null,
                    Email = biz != null ? biz.Email : null,
                    Website = biz != null ? biz.Website : null,
                    Tagline = biz != null ? biz.Tagline : null,
                    ItemTotal = itemTotal,
                    SaleType = saleType,
                    PurchaseOrderAmount = po.PurchaseOrderAmount,
                    PurchaseReturnAmount = po.PurchaseReturnAmount,
                    BillPaid = po.BillPaid,
                    Discount = po.Discount,
                    Balance = po.Balance,
                    PrevBalance = po.PrevBalance,
                    Date = po.Date,
                    Remarks = pod.Remarks ?? po.Remarks
                });
            }

            return list;
        }
    }
}
