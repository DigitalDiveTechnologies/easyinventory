namespace ShopOn.Web.Models;

public class CustomersWiseSaleViewModel
{
    public int CustomerId { get; set; }
    public string? CustomerName { get; set; }
    public int SaleOrderQty { get; set; }
    public decimal SaleOrderAmount { get; set; }
    public int SaleReturnQty { get; set; }
    public decimal SaleReturnAmount { get; set; }
    public decimal Discount { get; set; }
}
