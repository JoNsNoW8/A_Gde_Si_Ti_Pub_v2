using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace A_Gde_Si_Ti_Pub.Models
{
    public class Ocena
    {
        public int OcenaId { get; set; }
        public int Vrednost { get; set; } // Vrednost ocene, npr. od 1 do 5
        public int KorisnikId { get; set; } // ID korisnika koji je dao ocenu
        public DateTime Datum { get; set; } = DateTime.Now; // Datum kada je ocena data
        public int? ProizvodId { get; set; } // Moze se ocenjivati i proizvod i kafic
        [StringLength(500)]
        public string Komentar { get; set; } // Opcioni komentar uz ocenu

        public virtual Korisnik Korisnik { get; set; } // Navigaciono svojstvo ka korisniku
        public virtual Proizvod Proizvod { get; set; } // Navigaciono svojstvo ka proizvodu
    }
}