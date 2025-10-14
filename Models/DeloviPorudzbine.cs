using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace A_Gde_Si_Ti_Pub.Models
{
    public class DeloviPorudzbine
    {
        [Key]
        public int DeloviPorudzbineID { get; set; }
        [Required]
        public int PorudzbinaId { get; set; } // Strani kljuc za Porudzbina
        [Required]
        public int ProizvodId { get; set; } // Strani kljuc za Proizvod
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Porudzbina mora sadrzati barem 1 proizvod.")]
        public int Kolicina { get; set; } = 1;
        public decimal Cena { get; set; } // Price at time of order (snapshot)
        public decimal Subtotal => Kolicina * Cena;
        // Navigation Properties (for EF relationships)
        public virtual Porudzbina Porudzbina { get; set; } // Back-reference to parent order
        public virtual Proizvod Proizvod { get; set; } // Reference to the product
    }

}