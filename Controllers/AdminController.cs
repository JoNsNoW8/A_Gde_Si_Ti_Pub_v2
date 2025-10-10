using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using A_Gde_Si_Ti_Pub.Models;

namespace A_Gde_Si_Ti_Pub.Controllers
{
    [Authorize(Roles ="Admin")]
    public class AdminController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Admin
        public ActionResult AdminPocetna()
        {
            var porudzbine = db.Porudzbine
             .Include(p => p.Korisnik)
             .Include(p => p.DeloviPorudzbine.Select(dp => dp.Proizvod))
             .ToList();
            var korisnici = db.Korisnici.ToList();
            var ocene = db.Ocene
                .Include(o => o.Korisnik)
                .Include(o => o.Proizvod)
                .ToList();
            var adminModel = new Admin
            {
                Porudzbine = porudzbine,
                Korisnici = korisnici,
                Ocene = ocene
            };
            return View(adminModel);
        }

        public ActionResult UpravljajPorudzbinama(int id)
        {
            var porudzbina = db.Porudzbine
              .Include(p => p.DeloviPorudzbine.Select(dp => dp.Proizvod))
              .FirstOrDefault(p => p.PorudzbinaId == id);
            if (porudzbina == null) return HttpNotFound();
            return View(porudzbina);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpravljajPorudzbinom(Porudzbina porudzbina)
        {
            db.Entry(porudzbina).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("AdminPocetna");
        }

        public ActionResult ObrisiPorudzbinu(int porudzbinaId)
        {
            var porudzbina = db.Porudzbine.Find(porudzbinaId);
            if (porudzbina != null)
            {
                db.Porudzbine.Remove(porudzbina);
                db.SaveChanges();
            }
            return RedirectToAction("AdminPocetna");
        }

        public ActionResult ObrisiOcenu(int ocenaId)
        {
            var ocena = db.Ocene.Find(ocenaId);
            if (ocena != null)
            {
                db.Ocene.Remove(ocena);
                db.SaveChanges();
            }
            return RedirectToAction("AdminPocetna");
        }

        public ActionResult UpravljajKkorisnicima()
        {
            var korisnici = db.Korisnici.ToList();
            return View(korisnici);
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