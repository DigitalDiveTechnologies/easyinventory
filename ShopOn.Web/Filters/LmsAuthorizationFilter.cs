using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ShopOn.Web.Infrastructure;

namespace ShopOn.Web.Filters;

/// <summary>Ports legacy LMSFilter: require session user except Account and UserManagement/Login.</summary>
public class LmsAuthorizationFilter : IAsyncActionFilter
{
    public Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (context.ActionDescriptor.EndpointMetadata?.Any(m => m is AllowAnonymousAttribute) == true)
            return next();

        var session = context.HttpContext.Session;
        var routeData = context.RouteData.Values;
        var currentController = routeData["controller"]?.ToString() ?? "";
        var currentAction = routeData["action"]?.ToString() ?? "";

        if (session.GetCurrentUser() != null)
            return next();

        if (string.Equals(currentController, "Account", StringComparison.OrdinalIgnoreCase))
            return next();

        if (string.Equals(currentController, "UserManagement", StringComparison.OrdinalIgnoreCase) &&
            string.Equals(currentAction, "Login", StringComparison.OrdinalIgnoreCase))
            return next();

        context.Result = new RedirectToActionResult("Login", "UserManagement", null);
        return Task.CompletedTask;
    }
}
