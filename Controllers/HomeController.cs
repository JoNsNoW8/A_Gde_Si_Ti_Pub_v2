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
            var ocene = db.Ocene
                .Include(o => o.Korisnik)
                .Include(o => o.Proizvod)
                .OrderByDescending(o => o.Datum)
                .Take(5)
                .ToList();
            ViewBag.Ocene = ocene; // dodeljuje se pogledu
            ViewBag.IsAuthenticated = User.Identity.IsAuthenticated;
            ViewBag.Proizvodi = db.Proizvodi.Where(p => p.Status).ToList(); // dropdown proizvodi
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult OstaviOcenu(Ocena ocena)
        {
            var customUser = User as CustomPrincipal;
            if (customUser?.IsInRole("Korisnik") != true)
            {
                ModelState.AddModelError("", "Samo registrovani korisnici mogu ostaviti ocenu.");
                return RedirectToAction("Index");
            }
            ocena.KorisnikId = GetCurrentUserId();
            ocena.Datum = DateTime.Now;
            try
            {
                db.Ocene.Add(ocena);
                db.SaveChanges();
                TempData["SuccessMessage"] = "Ocena je uspešno obrađena! Hvala!";
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Greška pri slanju ocene: " + ex.Message);
            }
            return RedirectToAction("Index");
        }
        private int GetCurrentUserId()
        {
            var customUser = User as CustomPrincipal;
            if (customUser == null) return 0;
            var korisnickoIme = customUser.Identity.Name;
            var korisnik = db.Korisnici.FirstOrDefault(k => k.Username == korisnickoIme);
            return korisnik?.KorisnikId ?? 0;
        }
        private List<Ocena> UcitajOcene()
        {
            return db.Ocene
                .Include(o => o.Korisnik)
                .Include(o => o.Proizvod)
                .OrderByDescending(o => o.Datum)
                .Take(5)
                .ToList();
        }
        public ActionResult About()
        {

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
        public ActionResult Profil()
        {
            var customUser = User as CustomPrincipal;
            if (customUser == null)
            {
                return RedirectToAction("Login", "Nalozi");
            }

            var korisnickoIme = customUser.Identity.Name;
            var korisnik = db.Korisnici.FirstOrDefault(k => k.Username == korisnickoIme);
            if (korisnik == null)
            {
                return HttpNotFound("Korisnik nije pronađen");
            }
            ViewBag.IsAdmin = (korisnik.Uloga == "Admin");
            ViewBag.CurrentUserId = korisnik.KorisnikId;
            return View(korisnik);
        }
        private int TrenutniKupacId()
        {
            var customUser = User as CustomPrincipal;
            if (customUser != null)
            {
                var username = customUser.Identity.Name; //Identity implementira IIdentity - osnovni podaci o korisniku
                var korisnik = db.Korisnici.FirstOrDefault(k => k.Username == username); //pronadji korisnika u bazi po korisnickom imenu
                if (korisnik != null)
                {
                    return korisnik.KorisnikId;
                }
                throw new Exception("Korisnik nije pronađen u bazi. Proverite korisnički nalog.");
            }
            throw new Exception("Korisnik nije autentifikovan. Proverite prijavu.");
        }

        [Authorize] 
        public ActionResult IzmeniProfil()
        {
            var korisnikId = TrenutniKupacId();
            var korisnik = db.Korisnici.Find(korisnikId);
            if (korisnik == null)
            {
                return HttpNotFound("Korisnik nije pronađen.");
            }
            return View(korisnik);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult IzmeniProfil(Korisnik formKorisnik)
        {
            if (!ModelState.IsValid)
            {
                return View(formKorisnik);
            }

            var customUser = User as CustomPrincipal;
            var korisnikId = TrenutniKupacId();
            var originalKorisnik = db.Korisnici.Find(korisnikId);
            if (originalKorisnik == null)
            {
                return HttpNotFound("Korisnik nije pronađen.");
            }

            //moze da promeni samo ime i email
            originalKorisnik.Ime = formKorisnik.Ime;
            originalKorisnik.Email = formKorisnik.Email;

            try
            {
                db.SaveChanges();
                TempData["SuccessMessage"] = "Profil uspešno ažuriran!";
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Greška pri ažuriranju profila: " + ex.Message);
                return View(formKorisnik);
            }

            return RedirectToAction("Profil");
        }
    }
}