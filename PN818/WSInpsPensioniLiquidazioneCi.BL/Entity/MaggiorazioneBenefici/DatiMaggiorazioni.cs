using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace INPS.Pensioni.LiquidazioneCi.Entity
{
    public class DatiMaggiorazioni
    {
        #region private properties

        private DateTime? _DecorrenzaMaggiorazioneSociale;
        private DateTime? _CessazioneMaggiorazioneSociale;
        private DateTime? _DecorrenzaMaggiorazioneLegge140;
        private short? _AnniRiduzioneBeneficiArt38Legge02;
        private byte? _CodiceRequisitiLegge50392Art2;

        #endregion private properties

        #region public properties

        public DateTime? DecorrenzaMaggiorazioneSociale { get { return _DecorrenzaMaggiorazioneSociale; } set { _DecorrenzaMaggiorazioneSociale = value; } }
        public DateTime? CessazioneMaggiorazioneSociale { get { return _CessazioneMaggiorazioneSociale; } set { _CessazioneMaggiorazioneSociale = value; } }
        public DateTime? DecorrenzaMaggiorazioneLegge140 { get { return _DecorrenzaMaggiorazioneLegge140; } set { _DecorrenzaMaggiorazioneLegge140 = value; } }
        public short? AnniRiduzioneBeneficiArt38Legge02 { get { return _AnniRiduzioneBeneficiArt38Legge02; } set { _AnniRiduzioneBeneficiArt38Legge02 = value; } }
        public byte? CodiceRequisitiLegge50392Art2 { get { return _CodiceRequisitiLegge50392Art2; } set { _CodiceRequisitiLegge50392Art2 = value; } }

        #endregion public properties

        public bool IsDatiMaggiorazioniNull()
        {
            if (this._DecorrenzaMaggiorazioneSociale.HasValue || this._CessazioneMaggiorazioneSociale.HasValue || this._DecorrenzaMaggiorazioneLegge140.HasValue || this._AnniRiduzioneBeneficiArt38Legge02.HasValue || this._CodiceRequisitiLegge50392Art2.HasValue)
                return false;
            else
                return true;
        }
    }
}
