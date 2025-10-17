using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using A_Gde_Si_Ti_Pub.Models;
using BCrypt.Net;

namespace A_Gde_Si_Ti_Pub.Controllers
{
    public class NaloziController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Registracija()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Registracija(Korisnik korisnik, string potvrdaLozinke)
        {
            if (ModelState.IsValid)
            {
                if (!string.IsNullOrEmpty(korisnik.Password) && !korisnik.Password.Equals(potvrdaLozinke))
                {
                    ModelState.AddModelError("potvrdaLozinke", "Lozinke se ne poklapaju.");
                    return View(korisnik);
                }
                //provera da li je korisnicko ime zauzeto
                if (db.Korisnici.Any(k => k.Username == korisnik.Username))
                {
                    ModelState.AddModelError("Username", "Korisnicko ime je zauzeto.");
                    return View(korisnik);
                }

                korisnik.PasswordHash = BCrypt.Net.BCrypt.HashPassword(korisnik.Password);
                korisnik.Uloga = "Korisnik"; //default uloga
                korisnik.IsActive = true; //nalog je aktivan
                korisnik.Email = korisnik.Email?.Trim();//uklanjanje belina oko email-a
                try
                {
                    db.Korisnici.Add(korisnik);
                    db.SaveChanges();

                    //automatsko logovanje nakon registracije
                    AutentifikacijaKorisnika(korisnik.Username, korisnik.Uloga);
                    return RedirectToAction("Profil", "Home");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Greška pri čuvanju korisnika u bazu: " + ex.Message);
                    return View(korisnik);
                }

            }
            return View(korisnik);
        }

        public ActionResult Login(string returnUrl)
        {
            if (User.Identity.IsAuthenticated) // kolacic
            {
                var customUser = User as CustomPrincipal;
                if (customUser?.IsInRole("Korisnik") == true)
                {
                    return RedirectToLocal(returnUrl ?? Url.Action("Index", "Home"));
                }
                else
                {
                    TempData["InfoMessage"] = "Već ste ulogovani kao Admin. Za kupovinu, odjavite se i ulogujte sa Korisnik nalogom.";
                    return RedirectToAction("Profil", "Home");
                }
            }
            ViewBag.ReturnUrl = returnUrl;
            return View();

        }
        //POST: Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string username, string password, bool? rememberMe, string returnUrl)
        {
            var korisnik = db.Korisnici.FirstOrDefault(k => k.Username == username && k.IsActive);
            if (korisnik != null && BCrypt.Net.BCrypt.Verify(password, korisnik.PasswordHash))
            {
                bool ostaniUlogovan = rememberMe ?? false;
                AutentifikacijaKorisnika(korisnik.Username, korisnik.Uloga, ostaniUlogovan);
                return RedirectToLocal(returnUrl);
            }
            ModelState.AddModelError("", "Pogresno korisnicko ime ili lozinka.");
            return View(new { username, returnUrl });
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
                1,
                username,
                DateTime.Now,
                DateTime.Now.AddMinutes(30),
                rememberMe,
                uloga
    );
            var encryptedTicket = FormsAuthentication.Encrypt(ticket);
            var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket)
            {
                HttpOnly = true,
                Secure = Request.IsSecureConnection
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