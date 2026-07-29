using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace INPS.Pensioni.LiquidazioneAgo.Entity
{
    public class DecBancaFideiussione
    {
        public string CodiceAzienda { get; set; }
        public string Matricola { get; set; }
        public string BancaFideiussione { get; set; }
        public byte? Progressivo { get; set; }
        public short? Anno { get; set; }
        public DateTime? InizioEsodo { get; set; }
        public DateTime? FineEsodo { get; set; }
        public int? ABI { get; set; }
        public int? CAB { get; set; }
    }
}
