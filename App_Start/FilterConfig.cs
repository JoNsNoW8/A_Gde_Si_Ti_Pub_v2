using System.Web;
using System.Web.Mvc;

namespace A_Gde_Si_Ti_Pub
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
