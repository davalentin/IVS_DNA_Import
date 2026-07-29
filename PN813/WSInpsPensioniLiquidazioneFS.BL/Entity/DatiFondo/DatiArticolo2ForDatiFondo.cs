using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.Pensioni.Liquidazione.BLCommon;

namespace INPS.Pensioni.LiquidazioneFs.Entity
{
    public class DatiArticolo2ForDatiFondo
    {
        public byte? Semaforo { get; set; }
        public DateTime? ScadenzaBenefici { get; set; }
        public decimal? PALConBenefici { get; set; }
        public bool? ScadenzaIllimitata { get; set; }

        
        public bool IsNull()
        {
            return !ScadenzaBenefici.HasValue && !PALConBenefici.HasValue && ScadenzaIllimitata!=true;
        }

        public bool IsNullInabilitaLegge335()
        {
            return !ScadenzaBenefici.HasValue && ScadenzaIllimitata != true;
        }
    }
}
