using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace INPS.Pensioni.LiquidazioneAgo.Entity
{
    public class DatiArticolo2
    {
        public byte? Semaforo { get; set; }
        public DateTime? ScadenzaBenefici { get; set; }
        public decimal? PALConBenefici { get; set; }
        public bool? ScadenzaIllimitata { get; set; }


        public bool IsNull()
        {
            return !ScadenzaBenefici.HasValue && !PALConBenefici.HasValue && ScadenzaIllimitata != true;
        }
    }
}
