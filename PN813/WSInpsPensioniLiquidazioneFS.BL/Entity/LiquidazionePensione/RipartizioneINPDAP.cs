using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace INPS.Pensioni.LiquidazioneFs.Entity
{
    public class RipartizioneINPDAP
    {
        #region public properties

        public long CodiceEnte { get; set; }
        public decimal? Importo { get; set; }

        #endregion public properties
    }
}
