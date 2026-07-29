using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace INPS.Pensioni.LiquidazioneAgo.Entity
{
    public class DecAnagraficaAziende
    {
        public long Id { get; set; }
        public string DenominazioneAzienda { get; set; }
        public byte? Oneri { get; set; }
        public string OneriTxt { get; set; }
    }
}
