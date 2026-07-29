using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace INPS.Pensioni.LiquidazioneFs.Entity
{
    public class CodiceEsodo
    {
        #region public properties

        public bool Codice { get { return _Codice; } set { _Codice = value; } }
        public string Descrizione { get { return _Descrizione; } set { _Descrizione = value; } }

        #endregion public properties

        #region private properties

        private bool _Codice;
        private string _Descrizione;

        #endregion private properties
    }
}
