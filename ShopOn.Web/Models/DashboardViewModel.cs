using EasyInventory.PgData.Entities;

namespace ShopOn.Web.Models;

public class DashboardViewModel
{
    public IQueryable<SO>? SOes { get; set; }
    public IQueryable<Customer>? Customers { get; set; }
    public IQueryable<Product>? Products { get; set; }
    public IQueryable<PO>? POes { get; set; }
}
