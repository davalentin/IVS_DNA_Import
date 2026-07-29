using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace INPS.Pensioni.Liquidazione.Entity
{
    public class CodiciNatura
    {
        #region public properties

        public System.Nullable<char> TraduzioneSuGP { get { return _TraduzioneSuGP; } set { _TraduzioneSuGP = value; } }

        public System.Nullable<byte> Posizione { get { return _Posizione; } set { _Posizione = value; } }

        public string Descrizione { get { return _Descrizione; } set { _Descrizione = value; } }

        public string Tipologia { get { return _Tipologia; } set { _Tipologia = value; } }

        public string Fondo { get { return _Fondo; } set { _Fondo = value; } }
        #endregion public properties

        #region private properties
        private System.Nullable<char> _TraduzioneSuGP;

        private System.Nullable<byte> _Posizione;

        private string _Descrizione;

        private string _Tipologia;

        private string _Fondo;
        #endregion private properties
    }
}
