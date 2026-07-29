using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace INPS.Pensioni.LiquidazioneFs.Entity
{
    public class DatiAssicurativi
    {
        #region public properties

        public DateTime? InizioAssicurazione { get; set; }
        public DateTime? FineAssicurazione { get; set; }

        #region fondoDatiGenerici

        public char? TipoPensione { get; set; }
        public DateTime? Decorrenza { get; set; }
        public string AttivitaSvolta { get; set; }
        public byte? CodiceDirittoQuoteFisse { get; set; }
        public byte? CodiceSpecifico { get; set; }
        public char? CodiceRequisiti1 { get; set; }
        public char? CodiceRequisiti2 { get; set; }
        
        public bool IsFondoDatiGenericiNull()
        {
            if (this.TipoPensione == null && this.Decorrenza == null &&
                String.IsNullOrEmpty(this.AttivitaSvolta) &&
                this.CodiceDirittoQuoteFisse == null &&
                this.CodiceSpecifico == null && this.CodiceRequisiti1 == null &&
                this.CodiceRequisiti2 == null)
                return true;
            else
                return false;
        }

        #endregion fondoDatiGenerici

        public int? AttivitaEconomica { get; set; }
        public int? ProfessioneIndividuale { get; set; }

        public FondoEL fondoEL { get; set; }
        public FondoTT fondoTT { get; set; }
        public FondoET fondoET { get; set; }
        public FondoVL fondoVL { get; set; }
        public FondoPT fondoPT { get; set; }
        public FondoFST fondoFST { get; set; }
        public FondoPI fondoPI { get; set; }
        public FondoGAS fondoGAS { get; set; }
        public FondoCL fondoCL { get; set; }
        public FondoDZ fondoDZ { get; set; }
        public FondoES fondoES { get; set; }
        public FondoPM fondoPM { get; set; }

        #endregion public properties

        #region nested class

        public class FondoEL
        {
            public DateTime? DecorrenzaTeorica { get; set; }
            public char? ConvenzioneInternazionale { get; set; }
            public byte? AnnoRiscatti { get; set; }
            public byte? MeseRiscatti { get; set; }
            public byte? AnnoAnzianitaPregressa { get; set; }
            public byte? MeseAnzianitaPregressa { get; set; }
            public byte? AnnoServizioMilitare { get; set; }
            public byte? MeseServizioMilitare { get; set; }
            public byte? AnnoArt3Legge107971 { get; set; }
            public byte? MeseArt3Legge107971 { get; set; }
            public byte? GradoInvalidita { get; set; }
            public byte? PercentualeMaggiorazione { get; set; }
            public byte? ProRataEnel { get; set; }
            public long? CodiceAzienda { get; set; }

            public bool IsFondoNull()
            {
                if (!this.DecorrenzaTeorica.HasValue && !this.ConvenzioneInternazionale.HasValue && !this.AnnoRiscatti.HasValue && !this.MeseRiscatti.HasValue &&
                    !this.AnnoAnzianitaPregressa.HasValue && !this.MeseAnzianitaPregressa.HasValue && !this.AnnoServizioMilitare.HasValue && !this.MeseServizioMilitare.HasValue &&
                    !this.AnnoArt3Legge107971.HasValue && !this.MeseArt3Legge107971.HasValue && !this.GradoInvalidita.HasValue && !this.PercentualeMaggiorazione.HasValue &&
                    !this.ProRataEnel.HasValue && !this.CodiceAzienda.HasValue)
                    return true;
                else
                    return false;
            }
        }

        public class FondoTT
        {
            public bool IsFondoNull()
            {
                if (!this.ConvenzioneInternazionale.HasValue && !this.RiscattiContributiFissiAnni.HasValue && !this.RiscattiContributiFissiMesi.HasValue &&
                    !this.RiscattiContributiFissiGiorni.HasValue && !this.RiscattiRiservaMatematicaAnni.HasValue && !this.RiscattiRiservaMatematicaMesi.HasValue &&
                    !this.RiscattiRiservaMatematicaGiorni.HasValue && !this.PeriodiFigurativiAnni.HasValue && !this.PeriodiFigurativiMesi.HasValue &&
                    !this.PeriodiFigurativiGiorni.HasValue && !this.DecorrenzaTeorica.HasValue && !this.SupplementoLegge58367.HasValue &&
                    !this.Ditta.HasValue && !this.RenditaInailAnnua.HasValue && !this.RetribuzioneMensileInail.HasValue && !this.PensioneDirettaGenitori.HasValue &&
                    !this.CodiceArt5L58.HasValue && !this.DimissioniAnte97.HasValue)
                    return true;
                else
                    return false;
            }

            public char? ConvenzioneInternazionale { get; set; }
            public int? RiscattiContributiFissiAnni { get; set; }
            public int? RiscattiContributiFissiMesi { get; set; }
            public int? RiscattiContributiFissiGiorni { get; set; }
            public int? RiscattiRiservaMatematicaAnni { get; set; }
            public int? RiscattiRiservaMatematicaMesi { get; set; }
            public int? RiscattiRiservaMatematicaGiorni { get; set; }
            public int? PeriodiFigurativiAnni { get; set; }
            public int? PeriodiFigurativiMesi { get; set; }
            public int? PeriodiFigurativiGiorni { get; set; }
            public DateTime? DecorrenzaTeorica { get; set; }
            public decimal? SupplementoLegge58367 { get; set; }
            public long? Ditta { get; set; }
            public decimal? RenditaInailAnnua { get; set; }
            public decimal? RetribuzioneMensileInail { get; set; }
            public decimal? PensioneDirettaGenitori { get; set; }
            public bool? CodiceArt5L58 { get; set; }
            public bool? DimissioniAnte97 { get; set; }
        }

        public class FondoET
        {
            public long IdFondo { get; set; }
            public long? CodAzienda { get; set; }
            public DateTime? DataEsonero { get; set; }
            public DateTime? DecorrenzaTeorica { get; set; }
            public decimal? ContributiAgoLegge140830 { get; set; }
            public decimal? ContributiAgoLegge40245 { get; set; }
            public int? GGInterruzione { get; set; }
            public short? NSettimaneLeva { get; set; }
            public short? NSettimaneRichiamato { get; set; }
            public decimal? Stipendio { get; set; }
            public decimal? Importo13ma { get; set; }
            public decimal? Importo14ma { get; set; }
            public decimal? ElementiAccessori { get; set; }
            public decimal? Competenze40Percento { get; set; }
            public bool? PartTime { get; set; }
            public int? AAInterruzione { get; set; }
            public int? MMInterruzione { get; set; }
            public bool? CodiceServizioMilitare { get; set; }
            public bool? CodiceEsodo { get; set; }
            public decimal? RetribuzioneEsodo { get; set; }
            public byte? GradoInvalidita { get; set; }
            public decimal? ImportoRenditaInail { get; set; }
            public decimal? RetribuzioneEffettiva { get; set; }
            public long? PersonaleViaggiante { get; set; }

            public bool IsFondoNull()
            {
                if (!this.CodAzienda.HasValue && !this.DataEsonero.HasValue && !this.DecorrenzaTeorica.HasValue && !this.ContributiAgoLegge140830.HasValue &&
                    !this.ContributiAgoLegge40245.HasValue && !this.GGInterruzione.HasValue && !this.NSettimaneLeva.HasValue &&
                    !this.NSettimaneRichiamato.HasValue && !this.Stipendio.HasValue && !this.Importo13ma.HasValue &&
                    !this.Importo14ma.HasValue && !this.ElementiAccessori.HasValue && !this.Competenze40Percento.HasValue &&
                    !this.PartTime.HasValue && !this.AAInterruzione.HasValue && !this.MMInterruzione.HasValue &&
                    !this.CodiceServizioMilitare.HasValue && !this.CodiceEsodo.HasValue && !this.RetribuzioneEsodo.HasValue && !this.GradoInvalidita.HasValue && 
                    !this.ImportoRenditaInail.HasValue && !this.RetribuzioneEffettiva.HasValue && !this.PersonaleViaggiante.HasValue)
                    return true;
                else
                    return false;
            }
        }

        public class FondoVL
        {
            public char? ConvenzioneInternazionale { get; set; }
            public byte? CodiceArt22 { get; set; }
            public DateTime? DataInvalidita { get; set; }
            public int? ProsecuzioneVolontariaAA { get; set; }
            public int? ProsecuzioneVolontariaMM { get; set; }
            public int? ProsecuzioneVolontariaGG { get; set; }
            public int? RiscattiRicongiunzioniAA { get; set; }
            public int? RiscattiRicongiunzioniMM { get; set; }
            public int? RiscattiRicongiunzioniGG { get; set; }
            public byte? CodiceCapitalizzazione { get; set; }
            public decimal? ImportoPercentualeCapitalizzazione { get; set; }
            public decimal? AliquotaIrpef { get; set; }
            public decimal? RetribuzioneSettimanaleAgoQuotaA { get; set; }
            public decimal? RetribuzioneSettimanaleAgoQuotaB { get; set; }

            public bool IsFondoNull()
            {
                if (!this.ConvenzioneInternazionale.HasValue && !this.CodiceArt22.HasValue && !this.DataInvalidita.HasValue && !this.ProsecuzioneVolontariaAA.HasValue &&
                    !this.ProsecuzioneVolontariaMM.HasValue && !this.ProsecuzioneVolontariaGG.HasValue && !this.RiscattiRicongiunzioniAA.HasValue &&
                    !this.RiscattiRicongiunzioniMM.HasValue && !this.RiscattiRicongiunzioniGG.HasValue && !this.CodiceCapitalizzazione.HasValue &&
                    !this.ImportoPercentualeCapitalizzazione.HasValue && !this.AliquotaIrpef.HasValue && !this.RetribuzioneSettimanaleAgoQuotaA.HasValue && !this.RetribuzioneSettimanaleAgoQuotaB.HasValue)
                    return true;
                else
                    return false;
            }
        }

        public class FondoPT
        {
            public long? CausaCessazione { get; set; }
            public bool? PagamentoIndennitaIntegrativaSpeciale { get; set; }
            public bool? IndennitaIntegrativaSpecialeConglobata { get; set; }
            public bool? TrediciMensilita { get; set; }
            public DateTime? DecorrenzaCalcolo { get; set; }
            public bool? DirittoIndennitaIntegrativaSpeciale { get; set; }
            public bool? IntegrazioneMinimo { get; set; }
            public bool? RiduzioneL537 { get; set; }
            public bool? IISAbbattimentoAnni { get; set; }
            public bool? OnereMEF { get; set; }
            public decimal? RipartizioneInpdap { get; set; }
            public short? VVUtiliDiritto { get; set; }
            public short? VVUtiliMisura { get; set; }            

            public bool IsFondoNull()
            {
                if (!this.CausaCessazione.HasValue && !this.PagamentoIndennitaIntegrativaSpeciale.HasValue && 
                    !this.IndennitaIntegrativaSpecialeConglobata.HasValue && !this.TrediciMensilita.HasValue && 
                    !this.DecorrenzaCalcolo.HasValue && !this.DirittoIndennitaIntegrativaSpeciale.HasValue &&
                    !this.IntegrazioneMinimo.HasValue && !this.RiduzioneL537.HasValue && !this.IISAbbattimentoAnni.HasValue &&
                    !this.OnereMEF.HasValue && !this.RipartizioneInpdap.HasValue &&
                    !this.VVUtiliDiritto.HasValue && !this.VVUtiliMisura.HasValue)
                    return true;
                else
                    return false;
            }
        }

        public class FondoFST
        {
            public long? CausaCessazione { get; set; }
            public bool? PagamentoIndennitaIntegrativaSpeciale { get; set; }
            public bool? IndennitaIntegrativaSpecialeConglobata { get; set; }
            public bool? TrediciMensilita { get; set; }
            public DateTime? DecorrenzaCalcolo { get; set; }
            public bool? TitolareAltraPensione { get; set; }
            public bool? DirittoIndennitaIntegrativaSpeciale { get; set; }
            public bool? IntegrazioneMinimo { get; set; }
            public bool? RiduzioneL537 { get; set; }
            public bool? IISAbbattimentoAnni { get; set; }
            public short? VVUtiliDiritto { get; set; }
            public short? VVUtiliMisura { get; set; }
            

            public bool IsFondoNull()
            {
                if (!this.CausaCessazione.HasValue && !this.PagamentoIndennitaIntegrativaSpeciale.HasValue && 
                    !this.IndennitaIntegrativaSpecialeConglobata.HasValue && !this.TrediciMensilita.HasValue &&
                    !this.DecorrenzaCalcolo.HasValue && !this.TitolareAltraPensione.HasValue &&
                    !this.DirittoIndennitaIntegrativaSpeciale.HasValue && !this.IntegrazioneMinimo.HasValue &&
                    !this.RiduzioneL537.HasValue && !this.IISAbbattimentoAnni.HasValue &&
                    !this.VVUtiliDiritto.HasValue && !this.VVUtiliMisura.HasValue)
                    return true;
                else
                    return false;
            }
        }

        public class FondoPI
        {
            public string NumeroMatricola { get; set; }
            public string Qualifica { get; set; }
            public bool? NonVedente { get; set; }
            public decimal? ImportoIIS { get; set; }
            public decimal? StipendioAnnuo { get; set; }
            public decimal? PensioneFacoltativaMensile { get; set; }
            public int? ControCodiceRetribuzione { get; set; }
            public short? RiscattiAA { get; set; }
            public short? RiscattiMM { get; set; }
            public short? RiscattiGG { get; set; }
            public DateTime? DecorrenzaPensioneEliminata { get; set; }
            public DateTime? DecorrenzaPrescrizione { get; set; }
            public DatiServizioUtile ServizioUtile { get; set; }
            public short? ServizioNonUtileAA { get; set; }
            public short? ServizioNonUtileMM { get; set; }
            public short? ServizioNonUtileGG { get; set; }
            public byte? Livello { get; set; }
            public short? SettimaneMaggiorazione { get; set; }
            public short? SettimaneEsclusive { get; set; }
            public short? SettimaneINPDAI { get; set; }
            public string CodiceCategoria { get; set; }
            public short? Sede { get; set; }
            public int? Certificato { get; set; }
            
            public bool IsFondoNull()
            {
                if (string.IsNullOrEmpty(this.NumeroMatricola) && string.IsNullOrEmpty(this.Qualifica) &&
                    !this.NonVedente.HasValue && !this.ImportoIIS.HasValue &&
                    !this.StipendioAnnuo.HasValue && !this.PensioneFacoltativaMensile.HasValue && !this.ControCodiceRetribuzione.HasValue &&
                    !this.RiscattiAA.HasValue && !this.RiscattiMM.HasValue && !this.RiscattiGG.HasValue &&
                    !this.DecorrenzaPensioneEliminata.HasValue && !this.DecorrenzaPrescrizione.HasValue
                    && this.ServizioUtile == null)
                    return true;
                else
                    return false;
            }
        }

        public class FondoGAS
        {
            public string Ditta { get; set; }
            public short? MesiUtiliIndennitaAggiuntiva { get; set; }
            public short? MesiNonUtiliIndennitaAggiuntiva { get; set; }
            public short? ServizioUtileIndennitaAggiuntiva { get; set; }
            public decimal? Retribuzione { get; set; }
            public bool? CodicePensioneRidotta { get; set; }
            public decimal? Conguaglio { get; set; }
            public short? MesiAnte46 { get; set; }
            public short? AnzianitaUtileDal46 { get; set; }
            public bool? CodiceDimissioni { get; set; }
            public short? PercentualeRiduzione { get; set; }
            public string Convenzione { get; set; }

            public bool IsFondoNull()
            {
                if (string.IsNullOrEmpty(this.Ditta) && !this.MesiUtiliIndennitaAggiuntiva.HasValue &&
                    !this.MesiNonUtiliIndennitaAggiuntiva.HasValue && !this.ServizioUtileIndennitaAggiuntiva.HasValue &&
                    !this.Retribuzione.HasValue && !this.CodicePensioneRidotta.HasValue && !this.Conguaglio.HasValue &&
                    !this.MesiAnte46.HasValue && !this.AnzianitaUtileDal46.HasValue && !this.CodiceDimissioni.HasValue &&
                    !this.PercentualeRiduzione.HasValue && string.IsNullOrEmpty(this.Convenzione))
                    return true;
                else
                    return false;
            }
        }

        public class FondoCL
        {            
            public decimal? ImportoAltraPensione { get; set; }
            public bool? CodicePensioneSenzaRequisiti { get; set; }
            public short? AnniDifferimento { get; set; }
            public byte? EtaPerfezionamentoRequisiti { get; set; }
            public DateTime? DataPerfezionamentoRequisiti { get; set; }
            public char? ContrProvv { get; set; }
            public short? ServizioUtileAA { get; set; }
            public short? ServizioUtileMM { get; set; }
            
            public bool IsFondoNull()
            {
                if (!this.ImportoAltraPensione.HasValue && !this.CodicePensioneSenzaRequisiti.HasValue &&
                    !this.AnniDifferimento.HasValue && !this.EtaPerfezionamentoRequisiti.HasValue &&
                    !this.DataPerfezionamentoRequisiti.HasValue && !this.ContrProvv.HasValue && !this.ServizioUtileAA.HasValue && !this.ServizioUtileMM.HasValue)
                    return true;
                else
                    return false;
            }
        }

        public class FondoDZ
        {
            public long IdFondo { get; set; }
            public short? RiscattiAA { get; set; }
            public short? RiscattiMM { get; set; }
            public bool? CodiceCaroPane { get; set; }
            public short? CodiceBenefici { get; set; }
            public bool? CodiceDZ { get; set; }
            public short? MaggiorazionePensionePrivilegiataAA { get; set; }
            public short? MaggiorazionePensionePrivilegiataMM { get; set; }
            public bool? CodiceEsodo { get; set; }
            public short? MaggiorazioneAnzianitaEsodoAA { get; set; }
            public short? MaggiorazioneAnzianitaEsodoMM { get; set; }
            public decimal? RetribuzioneAlNettoBeneficiEsodo { get; set; }
            public DateTime? DataCessazioneServizio { get; set; }
            public short? ClasseAnte50 { get; set; }
            public int? PercentualeLiquidazionePensione { get; set; }
            public string Ditta { get; set; }
            public bool? RaggiuntoRequisiti311297 { get; set; }

            public bool IsFondoNull()
            {
                if (!RiscattiAA.HasValue && !RiscattiMM.HasValue && !CodiceCaroPane.HasValue && !CodiceBenefici.HasValue &&
                    !CodiceDZ.HasValue && !MaggiorazionePensionePrivilegiataAA.HasValue && !MaggiorazionePensionePrivilegiataMM.HasValue &&
                    !CodiceEsodo.HasValue && !MaggiorazioneAnzianitaEsodoAA.HasValue && !MaggiorazioneAnzianitaEsodoMM.HasValue &&
                    !RetribuzioneAlNettoBeneficiEsodo.HasValue && !DataCessazioneServizio.HasValue && !ClasseAnte50.HasValue &&
                    !PercentualeLiquidazionePensione.HasValue && string.IsNullOrEmpty(Ditta) && !RaggiuntoRequisiti311297.HasValue)
                    return true;
                else
                    return false;
            }
        }

        public class FondoES
        {
            public char? ConvenzioneInternazionale { get; set; }
            public int? AnniRiscatti { get; set; }
            public int? MesiRiscatti { get; set; }

            public bool IsFondoNull()
            {
                if (this.ConvenzioneInternazionale.HasValue ||
                    this.AnniRiscatti.HasValue ||
                    this.MesiRiscatti.HasValue)
                    return false;

                return true;
            }
        }

        public class FondoPM
        {
            public byte? TipoLiquidazione { get; set; }
            public bool? AnnoUtileUltimoDecennio { get; set; }
            public char? AttivitaSvolta2 { get; set; }
            public char? CL413 { get; set; }

            public bool IsFondoNull()
            {
                if (this.AnnoUtileUltimoDecennio.HasValue ||
                    this.AttivitaSvolta2.HasValue ||
                    this.CL413.HasValue ||
                    this.TipoLiquidazione.HasValue)
                    return false;

                return true;
            }
        }

        public class DatiServizioUtile
        {
            public short? ServizioUtileAA { get; set; }
            public short? ServizioUtileMM { get; set; }
            public short? ServizioUtileGG { get; set; }

            public bool IsDatiServizioUtileNull()
            {
                if (!this.ServizioUtileAA.HasValue && !this.ServizioUtileMM.HasValue && !this.ServizioUtileGG.HasValue)
                    return true;
                else
                    return false;
            }
        }

        #endregion nested class
    }
}