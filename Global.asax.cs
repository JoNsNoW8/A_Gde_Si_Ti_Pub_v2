using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using A_Gde_Si_Ti_Pub.Models;

namespace A_Gde_Si_Ti_Pub
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            if (User != null && User.Identity.IsAuthenticated)
            {
                var authCookie = HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName];
                if (authCookie != null)
                {
                    var ticket = FormsAuthentication.Decrypt(authCookie.Value);
                    var principal = new CustomPrincipal(ticket.Name, ticket.UserData); // UserData will hold role
                    HttpContext.Current.User = principal;
                }
            }
        }
        protected void Application_PostAuthenticateRequest(Object sender, EventArgs e)
        {
            HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            if (authCookie != null)
            {
                try
                {
                    var authTicket = FormsAuthentication.Decrypt(authCookie.Value);
                    string role = authTicket.UserData;

                    // Create your custom principal
                    var newUser = new CustomPrincipal(authTicket.Name, role);
                    HttpContext.Current.User = newUser;
                }
                catch
                {
                    // corrupted cookie or invalid ticket
                    FormsAuthentication.SignOut();
                }
            }
        }
    }
}
