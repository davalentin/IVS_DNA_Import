using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace INPS.Pensioni.LiquidazioneAgo.Entity
{
    public class DatiExCombattente
    {
        #region private properties

        private DateTime? _DecorrenzaMaggiorazioneArt6;
        private byte? _CodiceCieco;
        // Campo ENPALS
        private int? _NumeroContributiNLNonVedenti;

        #endregion private properties

        #region public properties

        public DateTime? DecorrenzaMaggiorazioneArt6 { get { return _DecorrenzaMaggiorazioneArt6; } set { _DecorrenzaMaggiorazioneArt6 = value; } }
        public byte? CodiceCieco { get { return _CodiceCieco; } set { _CodiceCieco = value; } }
        // Campo ENPALS
        public int? NumeroContributiNLNonVedenti { get { return _NumeroContributiNLNonVedenti; } set { _NumeroContributiNLNonVedenti = value; } }

        #endregion public properties

        public bool IsDatiExCombattenteNull()
        {
            if (this._DecorrenzaMaggiorazioneArt6.HasValue || this.CodiceCieco.HasValue)
                return false;
            else
                return true;
        }

        public bool IsDatiExCombattenteENPALSNull()
        {
            if (this._NumeroContributiNLNonVedenti.HasValue)
                return false;
            else
                return true;
        }
    }
}
