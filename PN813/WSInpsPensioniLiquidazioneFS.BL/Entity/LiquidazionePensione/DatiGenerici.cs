using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace INPS.Pensioni.LiquidazioneFs.Entity
{
    public class DatiGenerici
    {
        #region pensione
        public DateTime? DecorrenzaCalcoloArretrati { get; set; }
        public byte? CodiceArretrati { get; set; }
        public DateTime? DataCompletezza { get; set; }
        public DateTime? DataInteressiLegali { get; set; }
        public byte? TipoCalcolo { get; set; }
        public string NaturaPensione { get; set; }
        public byte? CausaCarico { get; set; }
        public bool? TrasformazioneAOI { get; set; }
        public bool? AgevolazioniLegge { get; set; }
        public bool? ExCombattente { get; set; }
        public bool? RequisitiVecchiaiaAl1294 { get; set; }
        public bool? RequisitiAl1294 { get; set; }
        public bool? RequisitiAl996 { get; set; }
        public bool? Benefici { get; set; }
        public bool? IsRichiestaBonus { get; set; }
        public string AnnoDecorrenzaBonus { get; set; }
        
        public bool IsPensioneNull()
        {
            if (this.DecorrenzaCalcoloArretrati == null &&
                this.CodiceArretrati == null && this.DataCompletezza == null &&
                this.DataInteressiLegali == null && this.TipoCalcolo == null &&
                String.IsNullOrEmpty(this.NaturaPensione) && this.CausaCarico == null && 
                this.TrasformazioneAOI == null && this.AgevolazioniLegge == null && 
                this.ExCombattente == null && this.RequisitiVecchiaiaAl1294 == null &&
                this.RequisitiAl1294 == null && this.RequisitiAl996 == null && this.Benefici == null &&
                this.IsRichiestaBonus == null && String.IsNullOrEmpty(this.AnnoDecorrenzaBonus))
                return true;
            else
                return false;
        }
        #endregion pensione

        #region istruttoria
        public DateTime? ScadenzaRevisioneSanitaria { get; set; }
        public byte? CodiceComunicazioneCampo1 { get; set; }
        public char? CodiceComunicazioneCampo2 { get; set; }
        public char? CodiceComunicazioneCampo3 { get; set; }
        public byte? CodiceComunicazioneCampo4 { get; set; }
        public long? CodiceParticolareSoggettoDerogato { get; set; }
        public bool? TrattamentoDisagi { get; set; }
        
        public bool IsIstruttoriaNull()
        {
            if (this.ScadenzaRevisioneSanitaria == null && this.CodiceComunicazioneCampo1 == null &&
                this.CodiceComunicazioneCampo2 == null && this.CodiceComunicazioneCampo3 == null &&
                this.CodiceComunicazioneCampo4 == null && !this.CodiceParticolareSoggettoDerogato.HasValue &&
                !this.TrattamentoDisagi.HasValue)
                return true;
            else
                return false;
        }
        #endregion istruttoria

        #region eliminazione
        public byte? CodiceMotivo { get; set; }
        public DateTime? DecorrenzaEliminazione { get; set; }
        public DateTime? DataEvento { get; set; }
        
        public bool IsEliminazioneNull()
        {
            if (this.CodiceMotivo == null && this.DecorrenzaEliminazione == null && this.DataEvento == null)
                return true;
            else
                return false;
        }
        #endregion eliminazione

        #region fondoDatiGenerici

        public bool? AttribuzioneBonus { get; set; }
        public DateTime? InizioBonus { get; set; }
        public DateTime? FineBonus { get; set; }
        public bool? ChkDL407 { get; set; }
        public bool? Articolo2 { get; set; }
        public bool? Privilegiate { get; set; }

        public bool IsFondoDatiGenericiNull()
        {
            if (!this.AttribuzioneBonus.HasValue && !this.InizioBonus.HasValue && !this.FineBonus.HasValue && (!this.ChkDL407.HasValue || !this.ChkDL407.Value) &&
                (!this.Articolo2.HasValue || !this.Articolo2.Value) && (!this.Privilegiate.HasValue || !this.Privilegiate.Value))
                return true;
            else
                return false;
        }

        #endregion fondoDatiGenerici

        #region FondiSpecifici
        public FondoEL fondoEL { get; set; }
        public FondoTT fondoTT { get; set; }
        public FondoET fondoET { get; set; }
        public FondoVL fondoVL { get; set; }
        public FondoPT fondoPT { get; set; }
        public FondoFST fondoFST { get; set; }
        public FondoPI fondoPI { get; set; }
        public FondoGAS fondoGAS { get; set; }
        public FondoDZ fondoDZ { get; set; }
        public FondoES fondoES { get; set; }
        public FondoPM fondoPM { get; set; }
        #endregion FondiSpecifici

        #region ControlloFelpe
        public bool? IsProvvisoria { get; set; }
        #endregion ControlloFelpe

        #region Pagamento
        public bool? TrattenutaInpdap { get; set; }
        public DateTime? DataRinunciaTrattenutaInpdap { get; set; }

        public bool IsDatiGenericiPagamentoNull()
        {
            if (!this.TrattenutaInpdap.HasValue && !this.DataRinunciaTrattenutaInpdap.HasValue)
                return true;
            else
                return false;
        }
        #endregion Pagamento

        #region nested class

        public class FondoEL
        {            
            public bool? Requisiti247_243 { get; set; }
            public byte? NumeroTriSemRequisiti { get; set; }
            public short? AnnoRequisiti { get; set; }
            public int? AnzianitaAnni { get; set; }

            public bool IsFondoNull()
            {
                if (!this.Requisiti247_243.HasValue && !this.NumeroTriSemRequisiti.HasValue && !this.AnnoRequisiti.HasValue && !this.AnzianitaAnni.HasValue)
                    return true;
                else
                    return false;
            }
        }

        public class FondoTT
        {
            public bool IsFondoNull()
            {
                if (!this.Requisiti247_243.HasValue && !this.NumeroTriSemRequisiti.HasValue && !this.AnnoRequisiti.HasValue && !this.AnzianitaAnni.HasValue)
                    return true;
                else
                    return false;
            }

            public bool? Requisiti247_243 { get; set; }
            public byte? NumeroTriSemRequisiti { get; set; }
            public short? AnnoRequisiti { get; set; }
            public int? AnzianitaAnni { get; set; }
        }

        public class FondoET
        {
            public bool IsFondoNull()
            {
                if (!this.Requisiti247_243.HasValue && !this.NumeroTriSemRequisiti.HasValue && !this.AnnoRequisiti.HasValue && !this.AnzianitaAnni.HasValue)
                    return true;
                else
                    return false;
            }

            public bool? Requisiti247_243 { get; set; }
            public byte? NumeroTriSemRequisiti { get; set; }
            public short? AnnoRequisiti { get; set; }
            public int? AnzianitaAnni { get; set; }
        }

        public class FondoVL
        {
            public bool? Requisiti247_243 { get; set; }
            public byte? NumeroTriSemRequisiti { get; set; }
            public short? AnnoRequisiti { get; set; }
            public int? AnzianitaAnni { get; set; }

            public bool IsFondoNull()
            {
                if (!this.Requisiti247_243.HasValue && !this.NumeroTriSemRequisiti.HasValue && !this.AnnoRequisiti.HasValue && !this.AnzianitaAnni.HasValue)
                    return true;
                else
                    return false;
            }
        }

        public class FondoPT
        {
            public DateTime? DecorrenzaEconomica { get; set; }
            public DateTime? FinestraMobile { get; set; }
            public bool? RequisitiAnte247 { get; set; }
            public byte? TrimesteRequisiti { get; set; }
            public int? AnzianitaAnni { get; set; }
            public short? AnnoRequisiti { get; set; }

            public bool IsFondoNull()
            {
                if (!this.DecorrenzaEconomica.HasValue && !this.FinestraMobile.HasValue && !this.RequisitiAnte247.HasValue && 
                    !this.TrimesteRequisiti.HasValue && !this.AnzianitaAnni.HasValue)
                    return true;
                else
                    return false;
            }
        }

        public class FondoFST
        {
            public DateTime? DecorrenzaEconomica { get; set; }
            public bool? RequisitiAnte247 { get; set; }
            public byte? TrimesteRequisiti { get; set; }
            public int? AnzianitaAnni { get; set; }
            public short? AnnoRequisiti { get; set; }

            public bool IsFondoNull()
            {
                if (!this.DecorrenzaEconomica.HasValue && !this.RequisitiAnte247.HasValue && !this.TrimesteRequisiti.HasValue && !this.AnzianitaAnni.HasValue)
                    return true;
                else
                    return false;
            }
        }

        public class FondoPI
        {
            public bool? Requisiti247_243 { get; set; }
            public byte? NumeroTriSemRequisiti { get; set; }
            public short? AnnoRequisiti { get; set; }
            public int? AnzianitaAnni { get; set; }

            public bool IsFondoNull()
            {
                if (!this.Requisiti247_243.HasValue && !this.NumeroTriSemRequisiti.HasValue && !this.AnnoRequisiti.HasValue && !this.AnzianitaAnni.HasValue)
                    return true;
                else
                    return false;
            }
        }

        public class FondoGAS
        {
            public bool? Requisiti247_243 { get; set; }
            public byte? NumeroTriSemRequisiti { get; set; }
            public short? AnnoRequisiti { get; set; }
            public int? AnzianitaAnni { get; set; }

            public bool IsFondoNull()
            {
                if (!this.Requisiti247_243.HasValue && !this.NumeroTriSemRequisiti.HasValue && !this.AnnoRequisiti.HasValue && !this.AnzianitaAnni.HasValue)
                    return true;
                else
                    return false;
            }
        }

        public class FondoDZ
        {
            public bool? Requisiti247_243 { get; set; }
            public byte? NumeroTriSemRequisiti { get; set; }
            public short? AnnoRequisiti { get; set; }
            public int? AnzianitaAnni { get; set; }

            public bool IsFondoNull()
            {
                if (!this.Requisiti247_243.HasValue && !this.NumeroTriSemRequisiti.HasValue && !this.AnnoRequisiti.HasValue && !this.AnzianitaAnni.HasValue)
                    return true;
                else
                    return false;
            }
        }

        public class FondoES
        {
            public bool? Requisiti247_243 { get; set; }
            public byte? NumeroTriSemRequisiti { get; set; }
            public short? AnnoRequisiti { get; set; }
            public int? AnzianitaAnni { get; set; }

            public bool IsFondoNull()
            {
                if (!this.Requisiti247_243.HasValue && !this.NumeroTriSemRequisiti.HasValue && !this.AnnoRequisiti.HasValue && !this.AnzianitaAnni.HasValue)
                    return true;
                else
                    return false;
            }
        }

        public class FondoPM
        {
            public byte? CodiceTipoLiquidazione { get; set; }
            public byte? NumeroTriSemRequisiti { get; set; }
            public short? AnnoRequisiti { get; set; }
            public int? AnzianitaAnni { get; set; }

            public bool IsFondoNull()
            {
                if (!this.CodiceTipoLiquidazione.HasValue && !this.NumeroTriSemRequisiti.HasValue && !this.AnnoRequisiti.HasValue && !this.AnzianitaAnni.HasValue)
                    return true;

                return false;
            }
        }

        #endregion nested class
    }
}

