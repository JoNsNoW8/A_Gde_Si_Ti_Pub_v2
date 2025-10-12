using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace A_Gde_Si_Ti_Pub.Models
{
    public class Porudzbina
    {
        public int PorudzbinaId { get; set; }
        public int KorisnikId { get; set; } // ID korisnika koji je narucio

        [Required(ErrorMessage = "Ime je obavezno.")]
        [StringLength(50)]
        public string Ime { get; set; }

        [Required(ErrorMessage = "PRezime je obavezno.")]
        [StringLength(50)]
        public string Prezime { get; set; }

        [Required(ErrorMessage = "Adresa je obavezna.")]
        [StringLength(200)]
        public string Adresa { get; set; }

        [Required(ErrorMessage = "Email je obavezan.")]
        [EmailAddress(ErrorMessage = "Nevažeća email adresa.")]
        [StringLength(100)]
        public string Email { get; set; }
        public DateTime Datum { get; set; }
        public decimal UkupnaCena { get; set; } // Ukupna cena porudzbine
        public string Status { get; set; } // Status porudzbine (npr. "Obrada", "Poslato", "Isporuceno")
        public virtual Korisnik Korisnik { get; set; } // Navigaciona svojstva
        public virtual ICollection<DeloviPorudzbine> DeloviPorudzbine { get; set; } = new List<DeloviPorudzbine>(); // Stavke porudzbine
    }
}