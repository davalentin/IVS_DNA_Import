using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace INPS.Pensioni.LiquidazioneCi.Entity
{
    public class SoggettoBeneficiario
    {
        public long Id { get; set; }
        public string Descrizione { get; set; }
        public string TraduzioneSuGP { get; set; }
    }
}
