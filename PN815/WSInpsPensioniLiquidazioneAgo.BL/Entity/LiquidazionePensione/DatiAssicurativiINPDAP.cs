using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.Pensioni.Liquidazione.BLCommon;

namespace INPS.Pensioni.LiquidazioneAgo.Entity
{
    public class DatiAssicurativiINPDAP
    {
        #region public properties

        #region Pensione

        public DateTime? InizioAssicurazione { get; set; }
        public DateTime? FineAssicurazione { get; set; }

        #endregion Pensione

        #region PensioniDatiGenerici

        public byte? CodiceSpecifico { get; set; }
        public string AttivitaSvolta { get; set; }
        public long? CausaCessazione { get; set; }
        public bool? TitolareAltraPensione { get; set; }
        public bool? PagamentoIndennitaIntegrativaSpeciale { get; set; }
        public bool? TrediciMensilita { get; set; }
        public DateTime? DecorrenzaCalcolo { get; set; }
        public bool? DirittoIndennitaIntegrativaSpeciale { get; set; }
        public bool? IntegrazioneMinimo { get; set; }
        public bool? RiduzioneL537 { get; set; }
        public bool? IISAbbattimentoAnni { get; set; }

        #endregion PensioniDatiGenerici

        #endregion public properties

        #region public methods
        public bool IsDatiAssicurativiINPDAPPensioneNull()
        {
            if (!this.InizioAssicurazione.HasValue && !this.FineAssicurazione.HasValue)
                return true;

            return false;
        }

        public bool IsDatiAssicurativiINPDAPPensioniDatiGenericiNull()
        {
            if (!this.CodiceSpecifico.HasValue && string.IsNullOrEmpty(AttivitaSvolta) && !this.CausaCessazione.HasValue && !this.TitolareAltraPensione.HasValue && !this.PagamentoIndennitaIntegrativaSpeciale.HasValue &&
                !this.TrediciMensilita.HasValue && !this.DecorrenzaCalcolo.HasValue && !this.DirittoIndennitaIntegrativaSpeciale.HasValue && !this.IntegrazioneMinimo.HasValue &&
                !this.RiduzioneL537.HasValue && !this.IISAbbattimentoAnni.HasValue)
                return true;

            return false;
        }
        #endregion public methods
    }
}
