using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace A_Gde_Si_Ti_Pub.Models
{
    public class Proizvod
    {
        public int ProizvodId { get; set; }
        [Required]
        [StringLength(100)]
        public string Naziv { get; set; }
        [StringLength(500)]
        public string Opis { get; set; } // Optional description
        [StringLength(200)]
        public string Slika { get; set; } // e.g., "~/images/mug.jpg"
        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Cena { get; set; }
        public bool Status { get; set; } = true; // true = Available/In Stock
    }
}