using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace INPS.Pensioni.LiquidazioneFs.Entity
{
    public class TipoCalcolo
    {
        #region public properties
        public string Id { get { return _Id; } set { _Id = value; } }

        public string Descrizione { get { return _Descrizione; } set { _Descrizione = value; } }

        public System.Nullable<byte> TraduzioneSuGP { get { return _TraduzioneSuGP; } set { _TraduzioneSuGP = value; } }

        public string Tipo { get { return _Tipo; } set { _Tipo = value; } }

        public string Tipologia { get { return _Tipologia; } set { _Tipologia = value; } }
        #endregion public properties

        #region private properties
        private string _Id;

        private string _Descrizione;

        private System.Nullable<byte> _TraduzioneSuGP;

        private string _Tipo;

        private string _Tipologia;
        #endregion private properties
    }
}
