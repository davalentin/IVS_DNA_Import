using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace INPS.Pensioni.LiquidazioneAgo.Entity
{
    public class DecodificaGestioneCalcoloContributivo
    {
        #region private properties
        private long _Id;
        private string _Descrizione;
        private string _TraduzioneSuGP;
        private bool _IsFondo;
        #endregion private properties

        #region public properties
        public long Id { get { return _Id; } set { _Id = value; } }
        public string Descrizione { get { return _Descrizione; } set { _Descrizione = value; } }
        public string TraduzioneSuGP { get { return _TraduzioneSuGP; } set { _TraduzioneSuGP = value; } }
        public bool IsFondo { get { return _IsFondo; } set { _IsFondo = value; } }
        #endregion public properties
    }
}
