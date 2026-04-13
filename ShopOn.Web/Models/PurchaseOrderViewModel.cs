using EasyInventory.PgData.Entities;

namespace ShopOn.Web.Models;

public class PurchaseOrderViewModel
{
    public IQueryable<Supplier>? Suppliers { get; set; }
    public Supplier? Supplier { get; set; }
    public PO? PurchaseOrder { get; set; }
    public List<POD>? PurchaseOrderDetail { get; set; }
    public IQueryable<Product>? Products { get; set; }
    public Product? Product { get; set; }
}
