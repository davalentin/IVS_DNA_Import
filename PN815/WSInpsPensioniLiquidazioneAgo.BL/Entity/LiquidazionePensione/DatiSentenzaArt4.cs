using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace INPS.Pensioni.LiquidazioneAgo.Entity
{
    public class DatiSentenzaArt4
    {
        public class SentenzaArt4
        {
            public DateTime? DecorrenzaSentenza { get; set; }
            public decimal? ImportoSentenza { get; set; }
            public bool IsFromGP { get; set; }
        }

        #region private properties
        private List<SentenzaArt4> _lDatiSentenzaArt4;

        #endregion private properties

        #region public properties
        public List<SentenzaArt4> lDatiSentenzaArt4 { get { return _lDatiSentenzaArt4; } set { _lDatiSentenzaArt4 = value; } }
        #endregion public properties
    }
}
