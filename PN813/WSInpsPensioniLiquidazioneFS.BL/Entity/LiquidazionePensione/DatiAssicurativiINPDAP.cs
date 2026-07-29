using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace INPS.Pensioni.LiquidazioneFs.Entity
{
    public class DatiAssicurativiINPDAP
    {
        #region public properties

        #region Pensione

        public DateTime? InizioAssicurazione { get; set; }
        public DateTime? FineAssicurazione { get; set; }

        #endregion Pensione

        #region PensioniFondoDatiGenerici

        public byte? CodiceSpecifico { get; set; }

        #endregion PensioniFondoDatiGenerici

        #region PensioneINPDAP
        public long? CausaCessazione { get; set; }
        public bool? DirittoIndennitaIntegrativaSpeciale { get; set; }
        public bool? RiduzioneL537 { get; set; }
        public bool? IISAbbattimentoAnni { get; set; }
        public short? VVUtiliDirittoAA { get; set; }
        public byte? VVUtiliDirittoMM { get; set; }
        public byte? VVUtiliDirittoGG { get; set; }
        public short? VVUtiliMisuraAA { get; set; }
        public byte? VVUtiliMisuraMM { get; set; }
        public byte? VVUtiliMisuraGG { get; set; }
        public int? AttivitaEconomica { get; set; }
        public int? ProfessioneIndividuale { get; set; }
        public long? Microqualifica { get; set; }
        public byte? AnniMax { get; set; }
        public byte? AnniUtili { get; set; }

        public int? Comparto { get; set; }
        public int? Settore { get; set; }
        public int? Ruolo { get; set; }
        public string CfAmministrazione { get; set; }
        public string ProgAmministrazione { get; set; }
        #endregion PensioneINPDAP

        #endregion public properties

        #region public methods
        public bool IsDatiAssicurativiINPDAPPensioneNull()
        {
            if (!this.InizioAssicurazione.HasValue && !this.FineAssicurazione.HasValue)
                return true;

            //**Revisione Campi INPDAP**
            //if (!this.InizioAssicurazione.HasValue && !this.FineAssicurazione.HasValue && !this.AttivitaEconomica.HasValue && !this.ProfessioneIndividuale.HasValue)
            //    return true;

            return false;
        }

        public bool IsDatiAssicurativiINPDAPPensioniFondoDatiGenericiNull()
        {
            if (!this.CodiceSpecifico.HasValue)
                return true;

            return false;
        }

        public bool IsDatiAssicurativiINPDAPPensioneINPDAPNull()
        {
            //**Revisione Campi INPDAP**
            //if (!this.CausaCessazione.HasValue && !this.DirittoIndennitaIntegrativaSpeciale.HasValue && !this.RiduzioneL537.HasValue &&
            //    !this.IISAbbattimentoAnni.HasValue && !this.VVUtiliDirittoAA.HasValue && !this.VVUtiliDirittoMM.HasValue && !this.VVUtiliDirittoGG.HasValue &&
            //    !this.VVUtiliMisuraAA.HasValue && !this.VVUtiliMisuraMM.HasValue && !this.VVUtiliMisuraGG.HasValue && !this.Microqualifica.HasValue &&
            //    !this.AnniMax.HasValue && !this.AnniUtili.HasValue)
            //    return true;

            if (!this.CausaCessazione.HasValue && !this.DirittoIndennitaIntegrativaSpeciale.HasValue && !this.RiduzioneL537.HasValue &&
                !this.IISAbbattimentoAnni.HasValue && !this.VVUtiliDirittoAA.HasValue && !this.VVUtiliDirittoMM.HasValue && !this.VVUtiliDirittoGG.HasValue &&
                !this.VVUtiliMisuraAA.HasValue && !this.VVUtiliMisuraMM.HasValue && !this.VVUtiliMisuraGG.HasValue && !this.Microqualifica.HasValue)
                return true;

            return false;
        }
        #endregion public methods
    }
}
