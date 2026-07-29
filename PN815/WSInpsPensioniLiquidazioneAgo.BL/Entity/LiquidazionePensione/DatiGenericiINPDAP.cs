using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace INPS.Pensioni.LiquidazioneAgo.Entity
{
    public class DatiGenericiINPDAP
    {
        public DatiGenericiINPDAP()
        { }

        #region public properties

        #region Pensione
        public DateTime? DataInteressiLegali { get; set; }
        public DateTime? DecorrenzaCalcoloArretrati { get; set; }
        public byte? CodiceArretrati { get; set; }
        public DateTime? DataCompletezza { get; set; }
        public byte? TipoCalcolo { get; set; }
        public string NaturaPensione { get; set; }
        public byte? CausaCarico { get; set; }
        public bool? ExCombattente { get; set; }
        public bool? Benefici { get; set; }
        #endregion Pensione

        #region Istruttoria
        public DateTime? ScadenzaRevisioneSanitaria { get; set; }
        public byte? CodiceComunicazioneCampo1 { get; set; }
        public char? CodiceComunicazioneCampo2 { get; set; }
        public char? CodiceComunicazioneCampo3 { get; set; }
        public byte? CodiceComunicazioneCampo4 { get; set; }
        #endregion Istruttoria

        #region PensioniDatiGenerici
        public DateTime? DecorrenzaEconomica { get; set; }
        public bool? AttribuzioneBonus { get; set; }
        public bool? RequisitiAnte247 { get; set; }
        public byte? TrimesteRequisiti { get; set; }
        public short? AnnoRequisiti { get; set; }
        public int? AnzianitaAnni { get; set; }
        public decimal? AliquotaMediaINPDAP { get; set; }
        public DateTime? DataRivalsaINPDAP { get; set; }
        #endregion PensioniDatiGenerici

        #region DatiControlloFelpe
        public DateTime? InizioBonus { get; set; }
        public DateTime? FineBonus { get; set; }
        public bool? IsProvvisoria { get; set; }
        #endregion DatiControlloFelpe

        #endregion public properties

        #region public methods

        public bool IsDatiGenericiINPDAPPensioneNull()
        {
            if (!this.DecorrenzaCalcoloArretrati.HasValue && !this.CodiceArretrati.HasValue &&
                !this.DataCompletezza.HasValue && !this.TipoCalcolo.HasValue &&
                String.IsNullOrEmpty(this.NaturaPensione) && !this.CausaCarico.HasValue &&
                !this.ExCombattente.HasValue && !this.Benefici.HasValue && !this.DataInteressiLegali.HasValue)
                return true;
            else
                return false;
        }

        public bool IsDatiGenericiINPDAPIstruttoriaNull()
        {
            if (!this.ScadenzaRevisioneSanitaria.HasValue && !this.CodiceComunicazioneCampo1.HasValue && !this.CodiceComunicazioneCampo2.HasValue &&
                !this.CodiceComunicazioneCampo3.HasValue && !this.CodiceComunicazioneCampo4.HasValue)
                return true;

            return false;
        }

        public bool IsDatiGenericiINPDAPPensioniDatiGenericiNull()
        {
            if (!this.DecorrenzaEconomica.HasValue && !this.AttribuzioneBonus.HasValue && !this.RequisitiAnte247.HasValue && !this.TrimesteRequisiti.HasValue && !this.AnnoRequisiti.HasValue && !this.AnzianitaAnni.HasValue &&
                !this.AliquotaMediaINPDAP.HasValue && !this.DataRivalsaINPDAP.HasValue)
                return true;

            return false;
        }

        public bool IsDatiGenericiINPDAPControlloFelpeNull()
        {
            if (!this.InizioBonus.HasValue && !this.FineBonus.HasValue && !this.IsProvvisoria.HasValue)
                return true;

            return false;
        }

        public char GetCodNatura1()
        {
            char codNat1 = ' ';
            char codNat2 = ' ';
            char codNat3 = ' ';
            if (this.NaturaPensione != null)
                GestioneControlli.GetCodiciNatura(this.NaturaPensione, out codNat1, out codNat2, out codNat3);
            return codNat1;
        }
        public char GetCodNatura2()
        {
            char codNat1 = ' ';
            char codNat2 = ' ';
            char codNat3 = ' ';
            if (this.NaturaPensione != null)
                GestioneControlli.GetCodiciNatura(this.NaturaPensione, out codNat1, out codNat2, out codNat3);
            return codNat2;
        }
        public char GetCodNatura3()
        {
            char codNat1 = ' ';
            char codNat2 = ' ';
            char codNat3 = ' ';
            if (this.NaturaPensione != null)
                GestioneControlli.GetCodiciNatura(this.NaturaPensione, out codNat1, out codNat2, out codNat3);
            return codNat3;
        }
        #endregion public methods
    }
}
