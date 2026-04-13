namespace ShopOn.Web.Infrastructure;

public class Utilities
{
    public decimal GetPayableAmount(decimal orderTotal, decimal returnTotal, decimal discount)
    {
        return orderTotal - returnTotal - discount;
    }

    public decimal GetBalanceAmount(decimal payableAmount, decimal amountPaid)
    {
        return payableAmount - amountPaid;
    }
}
