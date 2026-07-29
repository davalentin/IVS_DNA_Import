using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace INPS.Pensioni.LiquidazioneFs.Entity
{
    public class DatiBititolaritaInail
    {
        #region private properties

        private long? _Idpensione;
        private DateTime? _DecorrenzaDirittoIntegrazioneMinimo;
        private DateTime? _CessazioneDirittoIntegrazioneMinimo;

        private DateTime? _SospensionePensioneInvalidita;
        private DateTime? _RipristinoPensioneInvalidita;
        private decimal? _ImportoMensile;

        private DateTime? _DecorrenzaAssegnoAccompangamento;
        bool? _DirittoAssegnoAccompagnamento;

        private List<PensioniInail> _LpensioniInail;

        #endregion private properties

        #region public properties

        public long? Idpensione { get { return _Idpensione; } set { _Idpensione = value; } }
        public DateTime? DecorrenzaDirittoIntegrazioneMinimo { get { return _DecorrenzaDirittoIntegrazioneMinimo; } set { _DecorrenzaDirittoIntegrazioneMinimo = value; } }
        public DateTime? CessazioneDirittoIntegrazioneMinimo { get { return _CessazioneDirittoIntegrazioneMinimo; } set { _CessazioneDirittoIntegrazioneMinimo = value; } }

        public DateTime? SospensionePensioneInvalidita { get { return _SospensionePensioneInvalidita; } set { _SospensionePensioneInvalidita = value; } }
        public DateTime? RipristinoPensioneInvalidita { get { return _RipristinoPensioneInvalidita; } set { _RipristinoPensioneInvalidita = value; } }
        public decimal? ImportoMensile { get { return _ImportoMensile; } set { _ImportoMensile = value; } }

        public DateTime? DecorrenzaAssegnoAccompangamento { get { return _DecorrenzaAssegnoAccompangamento; } set { _DecorrenzaAssegnoAccompangamento = value; } }
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
