using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using A_Gde_Si_Ti_Pub.Models;
using BCrypt.Net;

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

        //CRUD za proizvode
        public ActionResult UpravljajProizvodima()
        {
            var proizvodi = db.Proizvodi.ToList(); // Load all for management
            return View(proizvodi);
        }
        public ActionResult DodajProizvod()
        {
            return View(new Proizvod());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DodajProizvod(Proizvod proizvod)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (db.Proizvodi.Any(p => p.Naziv.ToLower() == proizvod.Naziv.ToLower().Trim()))
                    {
                        ModelState.AddModelError("Naziv", "Proizvod sa sličnim nazivom već postoji. Izaberite jedinstveni naziv.");
                        return View(proizvod);
                    }
                    db.Proizvodi.Add(proizvod);
                    db.SaveChanges();
                    TempData["SuccessMessage"] = $"Proizvod '{proizvod.Naziv}' uspešno dodat!";
                    return RedirectToAction("UpravljajProizvodima");
                }

                catch (Exception ex)
                {
                    if (ex.Message.Contains("unique") || ex.Message.Contains("duplicate"))
                    {
                        ModelState.AddModelError("Naziv", "Naziv je već zauzet. Pokušajte drugi.");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Greška pri dodavanju: " + ex.Message);
                    }
                    return View(proizvod);
                }
            }
            return View(proizvod);
        }

        public ActionResult IzmeniProizvod(int id)
        {
            var proizvod = db.Proizvodi.Find(id);
            if (proizvod == null) return HttpNotFound("Proizvod nije pronadjen");
            return View(proizvod);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IzmeniProizvod(Proizvod formProizvod)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // NEW: Load the original entity from DB by ID (ensures it's attached)
                    var originalProizvod = db.Proizvodi.Find(formProizvod.ProizvodId);
                    if (originalProizvod == null)
                    {
                        return HttpNotFound("Proizvod nije pronađen. Možda je obrisan.");
                    }
                    // NEW: Copy properties from form to original (safe merge)
                    originalProizvod.Naziv = formProizvod.Naziv;
                    originalProizvod.Opis = formProizvod.Opis;
                    originalProizvod.Cena = formProizvod.Cena;
                    originalProizvod.Slika = formProizvod.Slika;
                    originalProizvod.Status = formProizvod.Status;
                    // Save the attached entity (EF knows it's modified)
                    db.SaveChanges();
                    TempData["SuccessMessage"] = $"Proizvod '{originalProizvod.Naziv}' uspešno izmenjen!";
                    return RedirectToAction("UpravljajProizvodima");
                }
                catch (Exception ex)
                {
                    // NEW: More specific error handling
                    ModelState.AddModelError("", $"Greška pri izmeni: {ex.Message}. Ako se ponavlja, proverite da li je proizvod još u bazi.");
                    // Re-load for error display
                    var errorProizvod = db.Proizvodi.Find(formProizvod.ProizvodId);
                    return View(errorProizvod ?? formProizvod);
                }
            }
            return View(formProizvod);
        }

        public ActionResult ObrisiProizvod(int id)
        {
            var proizvod = db.Proizvodi.Find(id);
            if (proizvod == null)
            {
                TempData["ErrorMessage"] = "Proizvod nije pronađen.";
                return RedirectToAction("UpravljajProizvodima");
            }

            try
            {
                db.Proizvodi.Remove(proizvod);
                db.SaveChanges(); // Removes from DB
                TempData["SuccessMessage"] = $"Proizvod '{proizvod.Naziv}' obrisan!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Greška pri brisanju: " + ex.Message;
            }
            return RedirectToAction("UpravljajProizvodima");
        }

        //CRUD za porudzbine
        public ActionResult UpravljajPorudzbinama()
        {
            var porudzbina = db.Porudzbine
              .Include(p => p.Korisnik)
              .Include(p => p.DeloviPorudzbine.Select(dp => dp.Proizvod))
              .OrderByDescending(p => p.Datum)
              .ToList();
            return View(porudzbina);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpravljajPorudzbinom(Porudzbina porudzbina)
        {
            db.Entry(porudzbina).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Profil");
        }

        public ActionResult ObrisiPorudzbinu(int porudzbinaId)
        {
            var porudzbina = db.Porudzbine.Find(porudzbinaId);
            if (porudzbina != null)
            {
                db.Porudzbine.Remove(porudzbina);
                db.SaveChanges();
            }
            return RedirectToAction("Profil", "Home");
        }

        //CRUD za ocene
        public ActionResult UpravljajOcenama()
        {
            var ocene = db.Ocene
                .Include(o => o.Korisnik)
                .Include(o => o.Proizvod)
                .ToList();
            return View(ocene);
        }
        public ActionResult ObrisiOcenu(int ocenaId)
        {
            var ocena = db.Ocene.Find(ocenaId);
            if (ocena != null)
            {
                db.Ocene.Remove(ocena);
                db.SaveChanges();
                TempData["SuccessMessage"] = $"Ocena ID {ocenaId} obrisana!";
            }
            else
            {
                TempData["ErrorMessage"] = "Ocena nije pronađena.";
            }
            return RedirectToAction("UpravljajOcenama");
        }

        //CRUD za korisnike

        public ActionResult UpravljajKorisnicima()
        {
            var korisnici = db.Korisnici.ToList();
            return View(korisnici);
        }
        public ActionResult ToggleKorisnik(int id)
        {
            var korisnik = db.Korisnici.Find(id);
            if (korisnik != null)
            {
                korisnik.IsActive = !korisnik.IsActive;
                db.SaveChanges();
                TempData["SuccessMessage"] = $"Korisnik '{korisnik.Username}' {(korisnik.IsActive ? "aktivan" : "deaktiviran")}.";
            }
            return RedirectToAction("UpravljajKorisnicima");
        }
        public ActionResult KreirajKorisnika()
        {
            return View(new Korisnik());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult KreirajKorisnika(Korisnik noviKorisnik, string potvrdaLozinke)
        {
            // Server-side sanity: ensure current user is admin (controller already [Authorize(Roles="Admin")])
            if (!ModelState.IsValid)
            {
                // If model validation fails, return the form with errors (client will see messages)
                // Optional: log validation errors for easier debugging (remove in production)
                foreach (var kv in ModelState)
                {
                    foreach (var err in kv.Value.Errors)
                    {
                        System.Diagnostics.Debug.WriteLine($"ModelState error - {kv.Key}: {err.ErrorMessage}");
                    }
                }
                return View(noviKorisnik);
            }

            // Validate password confirmation
            if (!string.IsNullOrEmpty(noviKorisnik.Password) && !noviKorisnik.Password.Equals(potvrdaLozinke))
            {
                ModelState.AddModelError("potvrdaLozinke", "Lozinke se ne poklapaju.");
                return View(noviKorisnik);
            }

            // Duplicate username check
            if (db.Korisnici.Any(k => k.Username == noviKorisnik.Username))
            {
                ModelState.AddModelError("Username", "Korisničko ime je zauzeto.");
                return View(noviKorisnik);
            }

            // Prepare and save
            noviKorisnik.PasswordHash = BCrypt.Net.BCrypt.HashPassword(noviKorisnik.Password);
            noviKorisnik.Uloga = "Korisnik"; // Force regular user
            noviKorisnik.IsActive = true;
            noviKorisnik.Email = noviKorisnik.Email?.Trim();

            try
            {
                db.Korisnici.Add(noviKorisnik);
                db.SaveChanges();
                TempData["SuccessMessage"] = $"Korisnički nalog '{noviKorisnik.Username}' uspešno kreiran!";
                return RedirectToAction("UpravljajKorisnicima");
            }
            catch (Exception ex)
            {
                // Log exception and show a user-friendly error
                System.Diagnostics.Debug.WriteLine("Error creating user: " + ex);
                ModelState.AddModelError("", "Greška pri kreiranju korisnika: " + ex.Message);
            }

            return View(noviKorisnik);
        }

        //pravljenje admin naloga

        public ActionResult KreirajAdmina()
        {
            return View(new Korisnik());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult KreirajAdmina(Korisnik noviAdmin, string potvrdaLozinke)
        {
            if (!ModelState.IsValid)
            {
                return View(noviAdmin); // Block if invalid
            }
            if (!string.IsNullOrEmpty(noviAdmin.Password) && !noviAdmin.Password.Equals(potvrdaLozinke))
            {
                ModelState.AddModelError("potvrdaLozinke", "Lozinke se ne poklapaju.");
                return View(noviAdmin);
            }
            // Check for duplicate username
            if (db.Korisnici.Any(k => k.Username == noviAdmin.Username))
            {
                ModelState.AddModelError("Username", "Korisničko ime je zauzeto.");
                return View(noviAdmin);
            }
            noviAdmin.PasswordHash = BCrypt.Net.BCrypt.HashPassword(noviAdmin.Password);
            noviAdmin.Uloga = "Admin"; // Force Admin (secure)
            noviAdmin.IsActive = true;
            noviAdmin.Email = noviAdmin.Email?.Trim();

            try
            {
                db.Korisnici.Add(noviAdmin);
                db.SaveChanges(); // Saves to DB
                TempData["SuccessMessage"] = $"Admin nalog '{noviAdmin.Username}' uspešno kreiran! Lozinka: {noviAdmin.Password} (promenite nakon prvog logovanja).";
                return RedirectToAction("UpravljajKorisnicima");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Greška pri kreiranju admina: " + ex.Message);
            }

            return View(noviAdmin);
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