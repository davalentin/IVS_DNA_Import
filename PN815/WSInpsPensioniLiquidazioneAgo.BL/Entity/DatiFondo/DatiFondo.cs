using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace INPS.Pensioni.LiquidazioneAgo.Entity
{
    public class DatiFondo
    {
        public byte? Semaforo { get; set; }

        public string TipoPensione { get; set; }
        public DateTime? DecorrenzaCalcolo { get; set; }
        public DateTime? DecorrenzaValidita { get; set; }
        public bool? TrediciMensilita { get; set; }
        public bool? IntegrazioneMinimo { get; set; }
        public bool? IndennitaIntegrativaSpecialeConglobata { get; set; }

        public bool IsNull()
        {
            return !DecorrenzaValidita.HasValue && !TrediciMensilita.HasValue && !IntegrazioneMinimo.HasValue && !IndennitaIntegrativaSpecialeConglobata.HasValue;
        }
    }
}
