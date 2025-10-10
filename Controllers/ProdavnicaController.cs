using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Web.Mvc;
using A_Gde_Si_Ti_Pub.Models;

namespace A_Gde_Si_Ti_Pub.Controllers
{
    [Authorize]
    public class ProdavnicaController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Prodavnica
        public ActionResult Index()
        {
            var customUser = User as CustomPrincipal;
            if (customUser == null || !customUser.IsInRole("Korisnik"))
            {
                return RedirectToAction("Login", "Nalozi");
            }

            var proizvodi = db.Proizvodi.Where(p => p.Status).ToList();
            return View(proizvodi);
        }

        //get : DodajUKorpu
        [HttpGet]
        public ActionResult KreirajPorudzbinu()
        {
            // Load cart from session
            var korpa = Session["Korpa"] as List<DeloviPorudzbine> ?? new List<DeloviPorudzbine>();
            if (!korpa.Any()) return RedirectToAction("Index"); // No items
            var porudzbina = new Porudzbina { DeloviPorudzbine = korpa };
            return View(porudzbina);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult KreirajPorudzbinu(Porudzbina porudzbina)
        {
            if (!ModelState.IsValid || !porudzbina.DeloviPorudzbine.Any())
            {
                // Reload cart if invalid
                porudzbina.DeloviPorudzbine = Session["Korpa"] as List<DeloviPorudzbine> ?? new List<DeloviPorudzbine>();
                return View(porudzbina);
            }

            porudzbina.KorisnikId = TrenutniKupacId();
            porudzbina.Status = "Obrada";
            porudzbina.Datum = DateTime.Now;
            porudzbina.UkupnaCena = porudzbina.DeloviPorudzbine.Sum(dp => dp.Cena * dp.Kolicina);

            try
            {
                db.Porudzbine.Add(porudzbina);
                db.SaveChanges(); // Save order to get PorudzbinaId
                foreach (var deo in porudzbina.DeloviPorudzbine)
                {
                    var proizvod = db.Proizvodi.Find(deo.ProizvodId);
                    if (proizvod == null || !proizvod.Status)
                    {
                        ModelState.AddModelError("", "Proizvod nije dostupan.");
                        db.Entry(porudzbina).State = EntityState.Deleted; // Rollback if needed
                        db.SaveChanges();
                        return View(porudzbina);
                    }
                    deo.PorudzbinaId = porudzbina.PorudzbinaId; // Now valid
                    db.DeloviPorudzbine.Add(deo);
                }
                db.SaveChanges();
                Session["Korpa"] = null; // Clear cart
                return RedirectToAction("MojeKupovine");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Greška pri kreiranju porudžbine: " + ex.Message);
                return View(porudzbina);
            }
        }

        public ActionResult MojeKupovine()
        {
            var korisnikId = TrenutniKupacId();
            var porudzbine = db.Porudzbine.Where(p => p.KorisnikId == korisnikId)
                .Include(p => p.DeloviPorudzbine.Select(dp => dp.Proizvod))
                .ToList();
            return View(porudzbine);
        }

        private int TrenutniKupacId() //trenutni kupac iz baze
        {
            var customUser = User as CustomPrincipal;
            if (customUser == null)
                throw new Exception("Nije pronađen korisnik u sesiji.");

            var korisnickoIme = customUser.Identity.Name;
            var korisnik = db.Korisnici.FirstOrDefault(k => k.Username == korisnickoIme);
            if (korisnik == null)
                throw new Exception("Korisnik nije pronađen u bazi.");

            return korisnik.KorisnikId;
        }
        [HttpPost]
        public ActionResult DodajUKorpu(int proizvodId, int kolicina = 1)
        {
            var korpa = Session["Korpa"] as List<DeloviPorudzbine> ?? new List<DeloviPorudzbine>();
            var proizvod = db.Proizvodi.Find(proizvodId);
            if (proizvod != null && proizvod.Status)
            {
                var item = korpa.FirstOrDefault(i => i.ProizvodId == proizvodId);
                if (item != null) item.Kolicina += kolicina;
                else korpa.Add(new DeloviPorudzbine { ProizvodId = proizvodId, Kolicina = kolicina, Cena = proizvod.Cena });
                Session["Korpa"] = korpa;
            }
            return RedirectToAction("Index");
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

    }
}