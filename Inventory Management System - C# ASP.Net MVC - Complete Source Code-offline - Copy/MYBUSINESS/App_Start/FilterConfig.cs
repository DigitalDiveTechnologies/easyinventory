using System.Web.Mvc;
using LMSMYBUSINESS.Models;

namespace MYBUSINESS
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new LMSFilter());
        }
    }
}
