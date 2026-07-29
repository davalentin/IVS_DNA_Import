using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace INPS.Pensioni.Liquidazione.Entity
{
    public class TipologiaFAQ
    {
        #region public properties
        public string Codice { get; set; }
        public string Descrizione { get; set; }
        public int? Contatore { get; set; }
        #endregion public properties
    }
}
