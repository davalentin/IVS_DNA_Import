using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace INPS.Pensioni.LiquidazioneCi.Entity
{
    public class DatiExCombattente
    {
        #region private properties

        private DateTime? _DecorrenzaMaggiorazioneArt6;
        private byte? _CodiceCieco;

        #endregion private properties

        #region public properties

        public DateTime? DecorrenzaMaggiorazioneArt6 { get { return _DecorrenzaMaggiorazioneArt6; } set { _DecorrenzaMaggiorazioneArt6 = value; } }
        public byte? CodiceCieco { get { return _CodiceCieco; } set { _CodiceCieco = value; } }

        #endregion public properties

        public bool IsDatiExCombattenteNull()
        {
            if (this._DecorrenzaMaggiorazioneArt6.HasValue || this.CodiceCieco.HasValue)
                return false;
            else
                return true;
        }
    }
}
