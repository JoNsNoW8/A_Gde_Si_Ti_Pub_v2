using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using A_Gde_Si_Ti_Pub.Models;
using BCrypt.Net;

namespace A_Gde_Si_Ti_Pub.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public ActionResult Index()
        {
            // Load latest 5 approved reviews (general or product-based)
            var ocene = db.Ocene
                .Include(o => o.Korisnik)
                .Include(o => o.Proizvod) // Optional load
                .OrderByDescending(o => o.Datum)
                .Take(5)
                .ToList();
            ViewBag.Ocene = ocene; // Pass to view
            ViewBag.IsAuthenticated = User.Identity.IsAuthenticated; // For conditional rendering
            ViewBag.CurrentUserId = GetCurrentUserId(); // For form pre-fill if needed
            ViewBag.Proizvodi = db.Proizvodi.Where(p => p.Status).ToList(); // For dropdown
            return View();
        }
        

        // POST: Add Review (only for logged-in users)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize] // Ensures logged in
        public ActionResult AddReview(Ocena ocena)
        {
            if (!ModelState.IsValid)
            {
                // Reload reviews and return partial error (for AJAX if needed)
                var ocene = LoadReviews(); // Helper below
                ViewBag.Ocene = ocene;
                return PartialView("_ReviewsSection", this); // Or full Index
            }
            var customUser = User as CustomPrincipal;
            if (customUser?.IsInRole("Korisnik") != true)
            {
                ModelState.AddModelError("", "Samo registrovani korisnici mogu ostaviti ocenu.");
                return RedirectToAction("Index");
            }
            ocena.KorisnikId = GetCurrentUserId();
            ocena.Datum = DateTime.Now;
            // ProizvodId: If null, it's general coffeeshop review; else from form
            try
            {
                db.Ocene.Add(ocena);
                db.SaveChanges();
                TempData["SuccessMessage"] = "Ocena je poslata na odobrenje! Hvala!";
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Greška pri slanju ocene: " + ex.Message);
            }
            // Reload and redirect to show updated list
            return RedirectToAction("Index");
        }
        // GET: Partial for Registration Modal (for non-logged users)
        public ActionResult RegisterModal()
        {
            return PartialView("_RegisterModal", new Korisnik());
        }
        // POST: AJAX Registration for Modal (non-logged users)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult RegisterModal(Korisnik korisnik, string potvrdaLozinke)
        {
            if (!ModelState.IsValid || !korisnik.Password.Equals(potvrdaLozinke))
            {
                return Json(new { success = false, errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
            }
            if (db.Korisnici.Any(k => k.Username == korisnik.Username))
            {
                return Json(new { success = false, errors = new[] { "Korisničko ime je zauzeto." } });
            }
            korisnik.PasswordHash = BCrypt.Net.BCrypt.HashPassword(korisnik.Password);
            korisnik.Uloga = "Korisnik";
            korisnik.IsActive = true;
            korisnik.Email = korisnik.Email?.Trim();
            try
            {
                db.Korisnici.Add(korisnik);
                db.SaveChanges();
                // Auto-login after register (set cookie)
                AutentifikacijaKorisnika(korisnik.Username, korisnik.Uloga); // Reuse from NaloziController (make shared or copy)
                return Json(new { success = true, message = "Registracija uspešna! Automatski ste ulogovani.", username = korisnik.Username });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, errors = new[] { "Greška: " + ex.Message } });
            }
        }

        // Helper: Get current user ID (reuse from ProdavnicaController)
        private int GetCurrentUserId()
        {
            var customUser = User as CustomPrincipal;
            if (customUser == null) return 0;
            var korisnickoIme = customUser.Identity.Name;
            var korisnik = db.Korisnici.FirstOrDefault(k => k.Username == korisnickoIme);
            return korisnik?.KorisnikId ?? 0;
        }

        // Helper: Load reviews (reusable)
        private List<Ocena> LoadReviews()
        {
            return db.Ocene
                .Include(o => o.Korisnik)
                .Include(o => o.Proizvod)
                .OrderByDescending(o => o.Datum)
                .Take(5)
                .ToList();
        }
        // Helper: Auto-login (copy from NaloziController or make a base/shared method)
        private void AutentifikacijaKorisnika(string username, string uloga)
        {
            var ticket = new FormsAuthenticationTicket(1, username, DateTime.Now, DateTime.Now.AddMinutes(30), true, uloga);
            var encryptedTicket = FormsAuthentication.Encrypt(ticket);
            var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket)
            {
                HttpOnly = true,
                Secure = Request.IsSecureConnection
            };
            Response.Cookies.Add(cookie);
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
            var customUser = User as CustomPrincipal;
            if(customUser == null)
            {
                return RedirectToAction("Login", "Nalozi");
            }

            var korisnickoIme = customUser.Identity.Name;
            var korisnik = db.Korisnici.FirstOrDefault(k => k.Username == korisnickoIme);
            if(korisnik == null)
            {
                return HttpNotFound("Korisnik nije pronađen");
            }
            ViewBag.IsAdmin = (korisnik.Uloga == "Admin");
            ViewBag.CurrentUserId = korisnik.KorisnikId;
            return View(korisnik);
        }
    }
}