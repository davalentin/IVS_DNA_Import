using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace INPS.Pensioni.LiquidazioneAgo.Entity
{
    public class DatiInail
    {
        #region private properties

        private DateTime? _DecorrenzaAssegnoAccompangamento;
        private DateTime? _CessazioneAssegnoAccompangamento;
        private bool? _DirittoAssegnoAccompagnamento;

        private List<PensioniInail> _LpensioniInail;

        #endregion private properties

        #region public properties

        public DateTime? DecorrenzaAssegnoAccompangamento { get { return _DecorrenzaAssegnoAccompangamento; } set { _DecorrenzaAssegnoAccompangamento = value; } }
        public DateTime? CessazioneAssegnoAccompangamento { get { return _CessazioneAssegnoAccompangamento; } set { _CessazioneAssegnoAccompangamento = value; } }
        public bool? DirittoAssegnoAccompagnamento { get { return _DirittoAssegnoAccompagnamento; } set { _DirittoAssegnoAccompagnamento = value; } }

        public List<PensioniInail> LpensioniInail { get { return _LpensioniInail; } set { _LpensioniInail = value; } }

        #endregion public properties

        #region nested class

        public class PensioniInail
        {
            #region public properties

            public long? IdPensione { get { return _IdPensione; } set { _IdPensione = value; } }
            public DateTime? DecorrenzaRenditaInail { get { return _DecorrenzaRenditaInail; } set { _DecorrenzaRenditaInail = value; } }
            public decimal? ImportoMensileInail { get { return _ImportoMensileInail; } set { _ImportoMensileInail = value; } }
            public bool? Evento { get { return _Evento; } set { _Evento = value; } }

            #endregion public properties

            #region private properties

            private long? _IdPensione;
            private DateTime? _DecorrenzaRenditaInail;
            private decimal? _ImportoMensileInail;
            private bool? _Evento;

            #endregion private properties
        }

        #endregion nested class
    }
}
