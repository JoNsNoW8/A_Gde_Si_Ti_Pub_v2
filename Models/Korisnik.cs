using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace A_Gde_Si_Ti_Pub.Models
{
    public class Korisnik
    {
        public int KorisnikId { get; set; }
        [Required]
        [StringLength(50)]
        public string Username { get; set; }
        [Required]
        public string PasswordHash { get; set; }
        [NotMapped]
        [Required]
        [StringLength(100, MinimumLength =6)]
        public string Password { get; set; }
        [Required]
        [StringLength(20)]
        public string Uloga { get; set; } // "Admin" ili "Korisnik"

        [StringLength(100)]
        [EmailAddress(ErrorMessage = "Nevažeća email adresa.")]
        public string Email { get; set; }
        public bool IsActive { get; set; } = true; // Da li je nalog aktivan
    }
}