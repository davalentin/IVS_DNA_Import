using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace INPS.Pensioni.LiquidazioneAgo.Entity
{
    public class DecodificaBanchePerSede
    {
        public long Id { get; set; }
        public string TraduzioneSuGP { get; set; }
        public string Descrizione { get; set; }
        public string CodiceSede { get; set; }

    }
}
