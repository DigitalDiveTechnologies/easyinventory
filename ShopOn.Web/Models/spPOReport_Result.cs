namespace ShopOn.Web.Models;

public partial class spPOReport_Result
{
    public string? Id { get; set; }
    public int? POSerial { get; set; }
    public decimal? PurchaseOrderQty { get; set; }
    public decimal? PurchaseReturnQty { get; set; }
    public string? Name { get; set; }
    public string? ProductName { get; set; }
    public decimal? PurchasePrice { get; set; }
    public int? Quantity { get; set; }
    public decimal? PerPack { get; set; }
    public bool? IsPack { get; set; }
    public string? BizName { get; set; }
    public string? BizAddress { get; set; }
    public string? Mobile { get; set; }
    public string? Email { get; set; }
    public string? Website { get; set; }
    public string? Tagline { get; set; }
    public decimal? ItemTotal { get; set; }
    public string? SaleType { get; set; }
    public decimal? PurchaseOrderAmount { get; set; }
    public decimal? PurchaseReturnAmount { get; set; }
    public decimal BillPaid { get; set; }
    public decimal? Discount { get; set; }
    public decimal? Balance { get; set; }
    public decimal? PrevBalance { get; set; }
    public DateTime? Date { get; set; }
    public string? Remarks { get; set; }
}
