using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace INPS.Pensioni.LiquidazioneFs.Entity
{
    public class CodicePensioniPrivilegiate
    {
        #region public properties

        public int Id { get { return _Id; } set { _Id = value; } }
        public System.Nullable<char> TraduzioneSuGP { get { return _TraduzioneSuGP; } set { _TraduzioneSuGP = value; } }
        public System.Nullable<byte> Posizione { get { return _Posizione; } set { _Posizione = value; } }
        public string Descrizione { get { return _Descrizione; } set { _Descrizione = value; } }
        public string Fondo { get { return _Fondo; } set { _Fondo = value; } }

        #endregion public properties

        #region private properties
        private int _Id;
        private System.Nullable<char> _TraduzioneSuGP;
        private System.Nullable<byte> _Posizione;
        private string _Descrizione;
        private string _Fondo;
        #endregion private properties
    }
}
