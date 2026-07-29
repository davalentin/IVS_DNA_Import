using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace INPS.Pensioni.LiquidazioneAgo.Entity
{
    public class DecEnteGestioneFondo
    {
        #region public properties

        public long Id { get; set; }
        public string Codice { get; set; }
        public short? CodiceFondo { get; set; }
        public string Ente { get; set; }
        public string Tipologia { get; set; }
        public bool? IsTrattenuteAmmesse { get; set; }

        #endregion public properties
    }
}
