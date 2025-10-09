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
            // Seed sample products if needed
            if (!context.Proizvodi.Any())
            {
                context.Proizvodi.Add(new Proizvod { Naziv = "Šolja", Cena = 2000, Slika = "~/images/mug.jpg", Status = true });
                // Add more...
            }
            context.SaveChanges();
        }
    }
}
