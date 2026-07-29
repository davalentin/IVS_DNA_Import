using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace INPS.Pensioni.LiquidazioneCi.Entity
{
    public class CodiceRequisitiLegge50392
    {
        #region public properties
        public string Id { get { return _Id; } set { _Id = value; } }

        public string Descrizione { get { return _Descrizione; } set { _Descrizione = value; } }

        public char? TraduzioneSuGP { get { return _TraduzioneSuGP; } set { _TraduzioneSuGP = value; } }
        #endregion public properties

        #region private properties
        private string _Id;

        private string _Descrizione;

        private char? _TraduzioneSuGP;
        #endregion private properties
    }
}
