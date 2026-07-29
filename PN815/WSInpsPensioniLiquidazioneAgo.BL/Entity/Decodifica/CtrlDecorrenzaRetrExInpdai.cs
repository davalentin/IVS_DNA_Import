using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace INPS.Pensioni.LiquidazioneAgo.Entity
{
    public class CtrlDecorrenzaRetrExINPDAI
    {
        public string Gestione { get; set; }

        public System.Nullable<char> Quota { get; set; }

        public string TipoQuota { get; set; }

        public System.Nullable<byte> CodiceDecorrenza { get; set; }

        public string Periodi { get; set; }

    }
}
