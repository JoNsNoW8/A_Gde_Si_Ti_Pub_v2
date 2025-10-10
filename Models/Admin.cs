using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace A_Gde_Si_Ti_Pub.Models
{
    public class Admin
    {
        public List<Porudzbina> Porudzbine { get; set; } = new List<Porudzbina>();
        public List<Korisnik> Korisnici { get; set; } = new List<Korisnik>();
        public List<Ocena> Ocene { get; set; } = new List<Ocena>(); 

        public int BrojPorudzbina => Porudzbine.Count;
        public int BrojKorisnika => Korisnici.Count;
        public int BrojOcena => Ocene.Count;
    }
}