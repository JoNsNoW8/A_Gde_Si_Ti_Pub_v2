using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace A_Gde_Si_Ti_Pub.Models
{
    public class Porudzbina
    {
        public int PorudzbinaId { get; set; }
        public int KorisnikId { get; set; } // ID korisnika koji je narucio
        public DateTime Datum { get; set; }
        public int UkupnaCena { get; set; } // Ukupna cena porudzbine
        public string Status { get; set; } // Status porudzbine (npr. "Obrada", "Poslato", "Isporuceno")
        public virtual Korisnik Korisnik { get; set; } // Navigaciona svojstva
        public virtual ICollection<DeloviPorudzbine> DeloviPorudzbine { get; set; } = new List<DeloviPorudzbine>(); // Stavke porudzbine
    }
}