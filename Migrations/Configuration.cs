namespace A_Gde_Si_Ti_Pub.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using A_Gde_Si_Ti_Pub.Models;

    internal sealed class Configuration : DbMigrationsConfiguration<A_Gde_Si_Ti_Pub.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }
        
        protected override void Seed(A_Gde_Si_Ti_Pub.Models.ApplicationDbContext context)
        {
            if (!context.Korisnici.Any(u => u.Username == "admin"))
            {
                var admin = new Korisnik
                {
                    Username = "admin",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("AdminPass123"),
                    Uloga = "Admin",
                    Email = "admin@coffeeshop.com"
                };
                context.Korisnici.Add(admin);
            }
            if (!context.Proizvodi.Any())
            {
                // T-Shirt (Majica)
                context.Proizvodi.Add(new Proizvod
                {
                    Naziv = "Majica (T-Shirt)",
                    Opis = "Udobna pamučna majica sa logom našeg kafića. Savršena za svakodnevno nošenje.",
                    Slika = "~/Content/images/tshirt.jpg", // Add image file here
                    Cena = 2500m, // e.g., 25.00 RSD (adjust currency)
                    Status = true // Available
                });
                // Mug (Šolja) - your existing one, enhanced
                context.Proizvodi.Add(new Proizvod
                {
                    Naziv = "Šolja (Mug)",
                    Opis = "Keramička šolja za kafu sa zabavnim dizajnom. Idealna za jutarnju kafu.",
                    Slika = "~/Content/images/mug.jpg",
                    Cena = 1500m, // e.g., 15.00 RSD
                    Status = true
                });
                // Tote Bag (Torba)
                context.Proizvodi.Add(new Proizvod
                {
                    Naziv = "Pamuk Torba (Tote Bag)",
                    Opis = "Ekološka pamuk torba za kupovinu sa printom kafića. Praktična i stilska.",
                    Slika = "~/Content/images/totebag.jpg",
                    Cena = 1800m, // e.g., 18.00 RSD
                    Status = true
                });
            }
            context.SaveChanges(); // Apply all seeds
        }
    }
}
