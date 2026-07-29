using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace INPS.Pensioni.LiquidazioneAgo.Entity
{
    public class DecodificaGestioneQuotaFondoIntegrativo
    {
        #region private properties
        private long _Id;
        private string _Descrizione;
        private string _TraduzioneSuGP;
        #endregion private properties

        #region public properties
        public long Id { get { return _Id; } set { _Id = value; } }
        public string Descrizione { get { return _Descrizione; } set { _Descrizione = value; } }
        public string TraduzioneSuGP { get { return _TraduzioneSuGP; } set { _TraduzioneSuGP = value; } }
        #endregion public properties
    }
}
