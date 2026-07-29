using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace INPS.Pensioni.LiquidazioneCi.Entity
{
    public class DecodificaCodeGestione
    {
        #region private properties

        private long _Id;
        private string _Descrizione;
        private short? _TraduzioneSuGP;
        private string _Legge;

        #endregion private properties

        #region public properties


        public long Id { get { return _Id; } set { _Id = value; } }
        
        public string Descrizione { get { return _Descrizione; } set { _Descrizione = value; } }
        
        public short? TraduzioneSuGP { get { return _TraduzioneSuGP; } set { _TraduzioneSuGP = value; } }
       
        public string Legge { get { return _Legge; } set { _Legge = value; } }

        #endregion public properties
    }

}
