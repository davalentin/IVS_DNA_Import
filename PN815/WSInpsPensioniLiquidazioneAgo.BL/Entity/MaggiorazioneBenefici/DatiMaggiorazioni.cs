using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace INPS.Pensioni.LiquidazioneAgo.Entity
{
    public class DatiMaggiorazioni
    {
        #region private properties

        private short? _AnniRiduzioneBeneficiArt38Legge02;
        private DateTime? _DecorrenzaMaggiorazioneSociale;
        private DateTime? _CessazioneMaggiorazioneSociale;

        #endregion private properties

        #region public properties

        public short? AnniRiduzioneBeneficiArt38Legge02 { get { return _AnniRiduzioneBeneficiArt38Legge02; } set { _AnniRiduzioneBeneficiArt38Legge02 = value; } }
        public DateTime? DecorrenzaMaggiorazioneSociale { get { return _DecorrenzaMaggiorazioneSociale; } set { _DecorrenzaMaggiorazioneSociale = value; } }
        public DateTime? CessazioneMaggiorazioneSociale { get { return _CessazioneMaggiorazioneSociale; } set { _CessazioneMaggiorazioneSociale = value; } }

        #endregion public properties

        public bool IsDatiMaggiorazioniNull()
        {
            if (this._AnniRiduzioneBeneficiArt38Legge02.HasValue || this._DecorrenzaMaggiorazioneSociale.HasValue || this._CessazioneMaggiorazioneSociale.HasValue)
                return false;
            else
                return true;
        }
    }
}
