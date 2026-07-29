using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace INPS.Pensioni.Liquidazione.Entity
{
    public class DomandeCollegate
    {
        public long NumeroDomanda { get; set; }
        public string Prodotto { get; set; }
        public string StatoLiqPens { get; set; }
        public string StatoWebDom { get; set; }
        public ChiavePensione PensioneAventeDiritto { get; set; }
        public ChiavePensione PensioneRiferimentoDC { get; set; }

        public class ChiavePensione
        {
            public string SiglaCategoria { get; set; }
            public string Sede { get; set; }
            public string Certificato { get; set; }
        }
    }
}
