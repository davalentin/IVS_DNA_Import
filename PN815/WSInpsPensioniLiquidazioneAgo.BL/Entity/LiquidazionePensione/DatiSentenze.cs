using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace INPS.Pensioni.LiquidazioneAgo.Entity
{
    public class DatiSentenze
    {
        public class Sentenze
        {
            public string CodSentenzaMerito { get; set; }
            public string CodSentenza { get; set; }
            public DateTime? DecorrenzaDal { get; set; }
            public DateTime? DecorrenzaAl { get; set; }
        }

        #region private properties
        private List<Sentenze> _lDatiSentenze;
        private bool? _IsSentenza49593;
        private bool? _IsSentenza2401994;
        private bool? _IsSentenze49593_2401994;
        #endregion

        #region public properties
        public List<Sentenze> lDatiSentenze { get { return _lDatiSentenze; } set { _lDatiSentenze = value; } }
        public bool? IsSentenza49593 { get { return _IsSentenza49593; } set { _IsSentenza49593 = value; } }
        public bool? IsSentenza2401994 { get { return _IsSentenza2401994; } set { _IsSentenza2401994 = value; } }
        public bool? IsSentenze49593_2401994 { get { return _IsSentenze49593_2401994; } set { _IsSentenze49593_2401994 = value; } }
        #endregion public properties

        #region public methods
        public bool IsNull()
        {
            if (_IsSentenza49593.HasValue || _IsSentenza2401994.HasValue || _IsSentenze49593_2401994.HasValue ||
                (_lDatiSentenze != null && _lDatiSentenze.Count > 0))
                return false;

            return true;
        }
        #endregion public methods
    }
}
