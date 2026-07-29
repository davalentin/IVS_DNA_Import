using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace INPS.Pensioni.LiquidazioneCi.Entity
{
    public class CodiceCi21
    {
        #region public properties

        public char Codice { get { return _Codice; } set { _Codice = value; } }
        public string Descrizione { get { return _Descrizione; } set { _Descrizione = value; } }

        #endregion public properties

        #region private properties

        private char _Codice;
        private string _Descrizione;

        #endregion private properties
    }
}
