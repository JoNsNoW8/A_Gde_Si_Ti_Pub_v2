using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace A_Gde_Si_Ti_Pub.Models
{
    public class Proizvod
    {
        public int ProizvodId { get; set; }
        public string Naziv { get; set; }

        public string Slika { get; set; }

        public bool Status { get; set; }
        public int Cena { get; set; }
    }
}