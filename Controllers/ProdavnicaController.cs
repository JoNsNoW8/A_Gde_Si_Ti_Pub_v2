using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Description;
using System.Web.UI.WebControls.WebParts;
using A_Gde_Si_Ti_Pub.Models;
using Microsoft.Ajax.Utilities;

namespace A_Gde_Si_Ti_Pub.Controllers
{
    public class ProdavnicaController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Prodavnica
        public ActionResult Index()
        {
            var customUser = User as CustomPrincipal;
            ViewBag.MozeKorpa = User.Identity.IsAuthenticated && customUser?.IsInRole("Korisnik") == true;

            var proizvodi = db.Proizvodi
                .Where(p => p.Status)
                .OrderBy(p => p.Naziv) // Optional: Sort by name
                .GroupBy(p => p.Naziv) // Group by name to avoid duplicates
                .Select(g => g.FirstOrDefault()) // Select first from each 
                .ToList();
            return View(proizvodi);
        }

        [HttpGet]
        [Authorize]
        public ActionResult KreirajPorudzbinu() //pregled korpe i potvrda porudzbine
        {
            var customUser = User as CustomPrincipal;
            if (customUser?.IsInRole("Korisnik") != true)
            {
                return RedirectToAction("Login", "Nalozi");
            }
            var korpa = Session["Korpa"] as List<DeloviPorudzbine> ?? new List<DeloviPorudzbine>();
            if (!korpa.Any())
            {
                TempData["ErrorMessage"] = "Korpa je prazna";
                return RedirectToAction("Korpa");
            }
            foreach (var item in korpa)
            {
                item.Proizvod = db.Proizvodi.Find(item.ProizvodId);
            }

            var korisnikId = TrenutniKupacId();
            var korisnik = db.Korisnici.Find(korisnikId);
            var porudzbina = new Porudzbina
            {
                KorisnikId = korisnikId,
                Ime = "", // Empty for form fill
                Prezime = "",
                Adresa = "",
                Email = korisnik?.Email ?? "", // Pre-fill from profile
                DeloviPorudzbine = korpa,
                Datum = DateTime.Now,
                UkupnaCena = korpa.Sum(dp => dp.Cena * dp.Kolicina) // Pre-calculate
            };
            ViewBag.Korpa = korpa;
            return View(porudzbina);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult KreirajPorudzbinu(Porudzbina porudzbina) //pravljenje porudzbine i sacuvaj u bazu
        {
            var customUser = User as CustomPrincipal;
            if (customUser?.IsInRole("Korisnik") != true)
            {
                return RedirectToAction("Login", "Nalozi");
            }

            var korpa = Session["Korpa"] as List<DeloviPorudzbine> ?? new List<DeloviPorudzbine>();
            if (!ModelState.IsValid || !korpa.Any())
            {
                // Re-populate for error display
                porudzbina.DeloviPorudzbine = korpa;
                foreach (var item in korpa)
                {
                    item.Proizvod = db.Proizvodi.Find(item.ProizvodId);
                }
                ViewBag.Korpa = korpa;
                porudzbina.UkupnaCena = korpa.Sum(dp => dp.Cena * dp.Kolicina);
                return View(porudzbina); // Return with errors
            }
            // Bind user details from form
            porudzbina.KorisnikId = TrenutniKupacId();
            porudzbina.Status = "Obrada"; // Default status
            porudzbina.Datum = DateTime.Now;
            porudzbina.UkupnaCena = korpa.Sum(dp => dp.Cena * dp.Kolicina); // Recalculate

                try
                {
                    // Save main order
                    db.Porudzbine.Add(porudzbina);
                    db.SaveChanges(); // Gets PorudzbinaId
                                      // Save order items
                    foreach (var deo in korpa)
                    {
                        var proizvod = db.Proizvodi.Find(deo.ProizvodId);
                        if (proizvod == null || !proizvod.Status)
                        {
                            ModelState.AddModelError("", "Proizvod nije dostupan.");
                            db.Porudzbine.Remove(porudzbina); // Rollback
                            db.SaveChanges();
                            ViewBag.Korpa = korpa;
                            return View(porudzbina);
                        }
                        deo.Proizvod = null; // Avoid EF trying to re-add product
                        deo.PorudzbinaId = porudzbina.PorudzbinaId;
                        db.DeloviPorudzbine.Add(deo);
                    }
                    db.SaveChanges();
                System.Diagnostics.Debug.WriteLine($"Snimljena porudžbina ID: {porudzbina.PorudzbinaId}");

                Session["Korpa"] = null;
                    TempData["SuccessMessage"] = $"Porudžbina #{porudzbina.PorudzbinaId} uspešno kreirana! Hvala na kupovini.";
                    return RedirectToAction("MojeKupovine"); // Or a success page
                }
                catch (Exception ex)
                {
                    var message = ex.Message;
                    var inner = ex.InnerException?.Message;
                    var inner2 = ex.InnerException?.InnerException?.Message;

                    System.Diagnostics.Debug.WriteLine("ERROR while saving order:");
                    System.Diagnostics.Debug.WriteLine(message);
                    System.Diagnostics.Debug.WriteLine(inner);
                    System.Diagnostics.Debug.WriteLine(inner2);

                    ModelState.AddModelError("", "Greška pri kreiranju porudžbine: " +
                        (inner2 ?? inner ?? message));

                    ViewBag.Korpa = korpa;
                    return View(porudzbina);

                }
        }

        public ActionResult Korpa()
        {
            var customUser = User as CustomPrincipal;
            if (customUser?.IsInRole("Korisnik") != true)
            {
                return RedirectToAction("Login", "Nalozi");
            }
            var korpa = Session["Korpa"] as List<DeloviPorudzbine> ?? new List<DeloviPorudzbine>();
            if (!korpa.Any())
            {
                ViewBag.Message = "Vaša korpa je prazna.";
                return View(new List<DeloviPorudzbine>());
            }
            // Load product details for display
            foreach (var item in korpa)
            {
                item.Proizvod = db.Proizvodi.Find(item.ProizvodId); // Eager load name/image
            }
            ViewBag.UkupnaCena = korpa.Sum(item => item.Subtotal); // Total for view
            return View(korpa);
        }
        // NEW: GET - View details of a specific order (for the current user only)
        [Authorize] // Require login
        public ActionResult DetaljiPorudzbine(int id)
        {
            var customUser = User as CustomPrincipal;
            if (customUser?.IsInRole("Korisnik") != true)
            {
                return RedirectToAction("Login", "Nalozi");
            }

            var korisnikId = TrenutniKupacId();
            var porudzbina = db.Porudzbine
                .Include(p => p.Korisnik)
                .Include(p => p.DeloviPorudzbine.Select(dp => dp.Proizvod))
                .FirstOrDefault(p => p.PorudzbinaId == id && p.KorisnikId == korisnikId);  // Ensure it's the user's order

            if (porudzbina == null)
            {
                TempData["ErrorMessage"] = "Porudžbina nije pronađena ili nemate pristup.";
                return RedirectToAction("MojeKupovine");
            }
            // NEW: Debug check for items (log if empty for future debugging)
            if (!porudzbina.DeloviPorudzbine.Any())
            {
                System.Diagnostics.Debug.WriteLine($"Order ID {id} has no items. Possible save issue.");
                TempData["WarningMessage"] = "Ova porudžbina nema stavki (kontaktirajte podršku ako ovo nije tačno).";
            }
            return View(porudzbina);  // Pass the order to the details view
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult AzurirajKorpu(int proizvodId, int kolicina)
        {
            var customUser = User as CustomPrincipal;
            if (customUser?.IsInRole("Korisnik") != true)
            {
                return Json(new { success = false, message = "Niste ovlašćeni." });
            }
            var korpa = Session["Korpa"] as List<DeloviPorudzbine> ?? new List<DeloviPorudzbine>();
            var item = korpa.FirstOrDefault(i => i.ProizvodId == proizvodId);
            if (item != null)
            {
                if(kolicina <= 0)
        {
                    korpa.Remove(item);
                }
                else
                {
                    item.Kolicina = Math.Max(1, kolicina);
                }
                Session["Korpa"] = korpa;
                return Json(new { success = true, totalItems = korpa.Count, subtotal = item?.Subtotal ?? 0 });
            }
            return Json(new { success = false, message = "Stavka nije pronađena." });
        }
        [Authorize]
        public ActionResult MojeKupovine() //istorija porudzbina
        {
            var customUser = User as CustomPrincipal;
            if (customUser?.IsInRole("Korisnik") != true)
            {
                return RedirectToAction("Login", "Nalozi");
            }
            var korisnikId = TrenutniKupacId();
            var porudzbine = db.Porudzbine.Where(p => p.KorisnikId == korisnikId)
        .Include(p => p.Korisnik)
        .Include(p => p.DeloviPorudzbine.Select(dp => dp.Proizvod))
        .OrderByDescending(p => p.Datum) // Newest first
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
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult DodajUKorpu(int proizvodId, int kolicina = 1)
        {

            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Nalozi", new { returnUrl = Url.Action("Index") }); // Redirect if not Korisnik
            }

            var customUser = User as CustomPrincipal;
            if (customUser?.IsInRole("Korisnik") != true)
            {
                // NEW: For Admin: Redirect to profile with message (no login)
                TempData["ErrorMessage"] = "Admin nalog ne može dodavati u korpu. Koristite Korisnik nalog za kupovinu.";
                return RedirectToAction("Profil", "Home");
            }

            var korpa = Session["Korpa"] as List<DeloviPorudzbine> ?? new List<DeloviPorudzbine>();
            var proizvod = db.Proizvodi.Find(proizvodId);
            if (proizvod != null && proizvod.Status)
            {
                var item = korpa.FirstOrDefault(i => i.ProizvodId == proizvodId);
                if (item != null) item.Kolicina += kolicina;
                else korpa.Add(new DeloviPorudzbine { ProizvodId = proizvodId, Kolicina = kolicina, Cena = proizvod.Cena });
                Session["Korpa"] = korpa;
                TempData["SuccessMessage"] = $"Proizvod '{proizvod.Naziv}' dodat u korpu!";
            }
            else
            {
                TempData["ErrorMessage"] = "Proizvod nije dostupan";
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