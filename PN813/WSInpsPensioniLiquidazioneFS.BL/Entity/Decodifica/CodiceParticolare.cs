using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace INPS.Pensioni.LiquidazioneFs.Entity
{
    public class CodiceParticolare
    {
        #region public properties
        public long Id { get { return _Id; } set { _Id = value; } }

        public string Descrizione { get { return _Descrizione; } set { _Descrizione = value; } }

        public char? TraduzioneSuGp { get { return _TraduzioneSuGp; } set { _TraduzioneSuGp = value; } }

        public string CodCategoria { get { return _CodCategoria; } set { _CodCategoria = value; } }
        #endregion public properties

        #region private properties
        private long _Id;

        private string _Descrizione;

        private char? _TraduzioneSuGp;

        private string _CodCategoria;
        #endregion private properties
    }
}
