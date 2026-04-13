using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ShopOn.Web.Infrastructure;

namespace ShopOn.Web.Filters;

public class ViewBagUserFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        if (context.Controller is Controller controller)
            controller.ViewBag.CurrentUser = context.HttpContext.Session.GetCurrentUser();
    }

    public void OnActionExecuted(ActionExecutedContext context) { }
}
