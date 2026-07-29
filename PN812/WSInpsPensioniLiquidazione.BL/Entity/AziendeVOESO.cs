using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace INPS.Pensioni.Liquidazione.Entity
{
    public class AziendeVOESO
    {
        public string CodiceAziendaTraduzioneSuGP { get; set; }
        public string Descrizione { get; set; }
        public DateTime? UltimaDecorrenzaAmmessa { get; set; }
        public string Tipo { get; set; }
    }
}
