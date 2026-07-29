using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace INPS.Pensioni.LiquidazioneAgo.Entity
{
    public class DecodificaAzienda
    {
        #region public properties

        public short Id { get { return _Id; } set { _Id = value; } }
        public string TraduzioneSuGP { get { return _TraduzioneSuGP; } set { _TraduzioneSuGP = value; } }
        public string Descrizione { get { return _Descrizione; } set { _Descrizione = value; } }

        #endregion public properties

        #region private properties

        private short _Id;
        private string _TraduzioneSuGP;
        private string _Descrizione;

        #endregion private properties
    }
}
