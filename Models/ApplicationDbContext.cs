using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace A_Gde_Si_Ti_Pub.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext() 
            : base("PubKonekcija")
        {
        }

        public DbSet<Korisnik> Korisnici { get; set; }
        public DbSet<Proizvod> Proizvodi { get; set; }
        public DbSet<Porudzbina> Porudzbine { get; set; }
        public DbSet<DeloviPorudzbine> DeloviPorudzbine { get; set; }
        public DbSet<Ocena> Ocene { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Porudzbina>()
                 .HasMany(o => o.DeloviPorudzbine)  // svaka porudzbina ima vise proiyzvoda
                 .WithRequired(d => d.Porudzbina)        //
                 .HasForeignKey(d => d.PorudzbinaId);    //

            modelBuilder.Entity<Porudzbina>()
                .HasRequired(p => p.Korisnik) // Svaka porudzbina ima jednog korisnika
                .WithMany() // Jedan korisnik moze imati vise porudzbina
                .HasForeignKey(p => p.KorisnikId); // Strani kljuc u tabeli Porudzbine

            base.OnModelCreating(modelBuilder);
        }
    }

    //EF konfigurise kako ce klase biti mapirane u tabele - kako ce klase biti povezane
    //kada override-ujemo OnModelCreating tada sami konfigurisemo mapiranje


}