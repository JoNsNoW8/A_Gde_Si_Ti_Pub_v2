using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using A_Gde_Si_Ti_Pub.Models;

namespace A_Gde_Si_Ti_Pub.Controllers
{
    public class NaloziController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Nalozi/Registracija
        public ActionResult Registracija()
        {
            return View();
        }

        //POST: Registracija
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Registracija(Korisnik korisnik, string potvrdaLozinke)
        {
            if (ModelState.IsValid)
            {
                if (!korisnik.Password.Equals(potvrdaLozinke))
                {
                    ModelState.AddModelError("confirmPassword", "Lozinke se ne poklapaju.");
                    return View(korisnik);
                }
                //provera da li je korisnicko ime zauzeto
                if (db.Korisnici.Any(k => k.Username == korisnik.Username))
                {
                    ModelState.AddModelError("Username", "Korisnicko ime je zauzeto.");
                    return View(korisnik);
                }

                korisnik.PasswordHash = BCrypt.Net.BCrypt.HashPassword(korisnik.PasswordHash);
                korisnik.Uloga = "Korisnik"; //default uloga
                db.Korisnici.Add(korisnik);
                db.SaveChanges();

                //automatsko logovanje nakon registracije
                AutentifikacijaKorisnika(korisnik.Username, korisnik.Uloga);
                return RedirectToAction("Index", "Home");
            }
            return View(korisnik);
        }

        //GET: Nalozi/Login
        public ActionResult Login(string returnUrl)
        {
            ViewBag.Returnurl = returnUrl;
            return View();
        }

        //POST: Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string username, string password, bool rememberMe, string returnUrl)
        {
            var korisnik = db.Korisnici.FirstOrDefault(k => k.Username == username && k.IsActive);
            if(korisnik != null && BCrypt.Net.BCrypt.Verify(password, korisnik.PasswordHash))
            {
                AutentifikacijaKorisnika(korisnik.Username, korisnik.Uloga, rememberMe);
                return RedirectToLocal(returnUrl);
            }
            ModelState.AddModelError("", "Pogresno korisnicko ime ili lozinka.");
            return View();
        }

        //POST: Logout
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session.Clear();
            return RedirectToAction("Index", "Home");
        }
        //pomocna metoda za kreiranje kolacica za korisnika koji se ulogovao
        private void AutentifikacijaKorisnika(string username, string uloga, bool rememberMe = false)
        {
            var ticket = new FormsAuthenticationTicket(
                1, // version
                username,
                DateTime.Now,
                DateTime.Now.AddMinutes(30),
                true,
                uloga // UserData (role)
    );
            var encryptedTicket = FormsAuthentication.Encrypt(ticket);
            var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket)
            {
                HttpOnly = true,
                Secure = Request.IsSecureConnection // Use secure in production
            };
            if (rememberMe) cookie.Expires = DateTime.Now.AddDays(14);
            Response.Cookies.Add(cookie);
        }

        //pomocna metoda
        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);
               
            return RedirectToAction("Index", "Home");
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}