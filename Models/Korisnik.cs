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

        [Required(ErrorMessage = "Korisničko ime je obavezno.")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Korisničko ime mora imati 3-50 karaktera.")]
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        [NotMapped]
        [StringLength(100, MinimumLength =6, ErrorMessage ="Lozinka mora imati najmanje 6 karaktera.")]
        public string Password { get; set; }
        [StringLength(20)]
        public string Uloga { get; set; } // "Admin" ili "Korisnik"

        [StringLength(100)]
        [EmailAddress(ErrorMessage = "Nevažeća email adresa.")]
        public string Email { get; set; }
        public bool IsActive { get; set; } = true; // Da li je nalog aktivan
    }
}