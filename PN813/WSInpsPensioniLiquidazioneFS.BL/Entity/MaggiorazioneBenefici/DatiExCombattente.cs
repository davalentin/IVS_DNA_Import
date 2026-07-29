using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace INPS.Pensioni.LiquidazioneFs.Entity
{
    public class DatiExCombattente
    {
        #region private properties

        private long? _ExCombattente;
        private DateTime? _DecorrenzaMaggiorazioneArt6;
        private byte? _CodiceCieco;
        private decimal? _RMSSenzaLegge33670QA;
        private decimal? _RMSSenzaLegge33670QB;
        private byte? _PercentualeMaggiorazioneSenzaLegge33670;
        private int? _DirittoScattiLegge336;
        private DateTime? _DecorrenzaMaggiorazioneLegge140;

        #endregion private properties

        #region public properties

        public long? ExCombattente { get { return _ExCombattente; } set { _ExCombattente = value; } }
        public DateTime? DecorrenzaMaggiorazioneArt6 { get { return _DecorrenzaMaggiorazioneArt6; } set { _DecorrenzaMaggiorazioneArt6 = value; } }
        public byte? CodiceCieco { get { return _CodiceCieco; } set { _CodiceCieco = value; } }
        public decimal? RMSSenzaLegge33670QA { get { return _RMSSenzaLegge33670QA; } set { _RMSSenzaLegge33670QA = value; } }
        public decimal? RMSSenzaLegge33670QB { get { return _RMSSenzaLegge33670QB; } set { _RMSSenzaLegge33670QB = value; } }
        public byte? PercentualeMaggiorazioneSenzaLegge33670 { get { return _PercentualeMaggiorazioneSenzaLegge33670; } set { _PercentualeMaggiorazioneSenzaLegge33670 = value; } }
        public int? DirittoScattiLegge336 { get { return _DirittoScattiLegge336; } set { _DirittoScattiLegge336 = value; } }
        public DateTime? DecorrenzaMaggiorazioneLegge140 { get { return _DecorrenzaMaggiorazioneLegge140; } set { _DecorrenzaMaggiorazioneLegge140 = value; } }

        #endregion public properties

        public bool IsDatiExCombattenteNull()
        {
            if (this._ExCombattente.HasValue || this._DecorrenzaMaggiorazioneArt6.HasValue || this.CodiceCieco.HasValue ||  
                this._RMSSenzaLegge33670QB.HasValue || this._PercentualeMaggiorazioneSenzaLegge33670.HasValue || this._DirittoScattiLegge336.HasValue || this._DecorrenzaMaggiorazioneLegge140.HasValue)
                return false;
            else
                return true;
        }
    }
}
