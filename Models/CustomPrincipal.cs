using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Security;

namespace A_Gde_Si_Ti_Pub.Models
{
    //trenutno ulogovan korisnik
    public class CustomPrincipal : IPrincipal
    {
        public CustomPrincipal(string username, string role)
        {
            Identity = new FormsIdentity(new FormsAuthenticationTicket(username, true, 30)); // 30 minuta trajanje
            Role = role;
        }

        public IIdentity Identity { get; private set; }
        public string Role { get; private set; }

        public bool IsInRole(string role)
        {
            return Role == role;
        }

    }
}