using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using A_Gde_Si_Ti_Pub.Models;

namespace A_Gde_Si_Ti_Pub.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "O nama, cekamo Ogija da napise";

            return View();
        }
        public ActionResult Meni()
        {
            ViewBag.Message = "Proizvodi koje nudimo u kaficu";

            return View();
        }
        public ActionResult Contact()
        {
            ViewBag.Message = "Kako da nas kontaktirate";

            return View();
        }
        [Authorize]
        public ActionResult Profil() //profilna stranica kada se korisnik uloguje
        {
            var korisnik = ((CustomPrincipal)User ).Identity.Name;
            ViewBag.Korisnik = korisnik;
            return View();
        }
    }
}