using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace INPS.Pensioni.Liquidazione.Entity
{
    public class PuliziaDomanda
    {
        #region public properties
        public string Attivita { get; set; }
        public DateTime? DataInizio { get; set; }
        public DateTime? DataFine { get; set; }
        #endregion public properties
    }
}
