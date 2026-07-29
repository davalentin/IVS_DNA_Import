using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using INPS.Pensioni.LiquidazioneFs.Entity;

namespace INPS.Pensioni.LiquidazioneFs.Service.Contracts.DataContracts
{
    [DataContract]
    public class AreaLiquidazionePensione
    {
        #region private properties
        private DatiGenerici _DatiGenerici;
        private DatiAssicurativi _DatiAssicurativi;
        private DatiPrecedentePensione _DatiPrecedentePensione;
        private DatiBititolaritaInail _DatiBititolaritaInail;
        private DatiLegge460 _DatiLegge460;
        private DatiLiquidazionePensioneStorico _DatiLiquidazionePensioneStorico;
        private DatiIstruttoriaINPDAP _DatiIstruttoriaINPDAP;
        private List<RecordFondo> _ListaRecordFondo;
        private List<DatiAttivitaSvolta> _ListaAttivitaSvolte;
        private List<CodiceRequisito1> _ListaCodiceRequisito1;
        private List<CodiceRequisito2> _ListaCodiceRequisito2;
        private List<CodiceSpecifico> _ListaCodiceSpecifico;
        private List<CodiceConvenzioneInternazionale> _ListaCodiceConvenzioneInternazionale;
        private List<CodiceEsodo> _ListaCodiceEsodo;
        private List<CodicePartTime> _ListaCodicePartTime;
        private List<CodiceArt22> _ListaCodiceArt22;
        private List<CodiceCapitalizzazione> _ListaCodiceCapitalizzazione;
        private List<CausaCessazione> _ListaCausaCessazione;
        private List<TipoCalcolo> _ListaTipoCalcolo;
        private List<CodiceEliminazione> _ListaCodiceEliminazione;
        private List<CodiceParticolare> _ListaCodiceParticolare;
        private List<TipoLiquidazionePM> _ListaTipoLiquidazionePM;
        private List<CodiceLegge413> _ListaCodiceLegge413;
        private List<AttivitaSvolta2> _ListaAttivitaSvolta2;
        private List<TipoLiquidazione> _ListaTipoLiquidazione;
        private List<CodiciNatura> _ListaCodiciNatura;
        private List<CodiceTipoLiquidazionePM> _ListaCodiceTipoLiquidazionePM;
        private List<PersonaleViaggiante> _ListaPersonaleViaggiante;
        private List<MicroqualificaINPDAP> _ListaMicroqualificaINPDAP;
        private List<CtrlCompartoSettoreRuolo> _ListaCtrlCompartoSettoreRuolo;

        private INPS.Pensioni.Liquidazione.BLCommon.Utility.CategoriaFondoPI? _categoriaFondoPI;
        private GestioneLiquidazionePensione.TipoSalvaguardia? _TipologiaSalvaguardia;
        private DateTime? _DecorrenzaPensioneDirettaDC;
        private Dictionary<string, char?> _TipoPensione;

        private bool? _IsEsenzioneFiscaleEstero;
        private bool? _IsResidenteEstero;
        private bool? _IsEsenzioneFiscaleVittima;
        private bool? _IsRequisitiL247_L243Enable;
        private bool? _IsCodiceSpecificoVisible;
        private bool? _IsVisibleArt2;
        private bool? _IsDecPensAnteAgosto95;
        private bool? _IsCodiceNatura2Enabled;
        private bool? _IsUsuranti;
        private bool? _IsVecchPerditaTitolo;
        private bool? _IsCodiceSpecificoEnabled;
        private bool? _IsCodiceArt22Enabled;
        private bool? _IsDomandaTrasformazioneAOI;
        private bool? _IsCodDirittoQuoteFisseVisible;
        private bool? _IsIndennitaAggiuntivaVisible;
        private bool? _IsDecorrenzaSuccSett1989;
        private bool? _IsCodiceComunicazione3Visible;
        private bool? _IsProvvisoriaVisible;
        private bool? _IsCodiceNatura2DisabledPerSperDonna;
        private bool? _IsDomandaConNuovaGestioneDatiFondoFSPT;
        private bool? _IsDomandaPersonaleViaggianteSenzaPerditaTitoloAbilitante;
        private bool? _IsDomandaPersonaleViaggianteSenzaPerditaTitoloAbilitanteVOCPDEL;
        private bool? _IsDomandaPersonaleViaggianteConPerditaTitoloAbilitante;
        private bool? _IsDomandaPersonaleViaggianteConPerditaTitoloAbilitanteVOCPDEL;
        private bool? _IsDomandaAnteArmonizzazione;
        private bool? _IsCapitalizzazioneVisible;
        private bool? _IsRiduzioneRetribVisible;
        private bool? _IsRiduzioneRetributivaEnabled;

        #endregion private properties

        #region public data member
        [DataMember]
        public DatiGenerici DatiGenerici { get { return _DatiGenerici; } set { _DatiGenerici = value; } }
        [DataMember]
        public DatiAssicurativi DatiAssicurativi { get { return _DatiAssicurativi; } set { _DatiAssicurativi = value; } }
        [DataMember]
        public DatiGenericiINPDAP DatiGenericiINPDAP { get; set; }
        [DataMember]
        public DatiAssicurativiINPDAP DatiAssicurativiINPDAP { get; set; }
        [DataMember]
        public DatiPrecedentePensione DatiPrecedentePensione { get { return _DatiPrecedentePensione; } set { _DatiPrecedentePensione = value; } }
        [DataMember]
        public DatiBititolaritaInail DatiBititolaritaInail { get { return _DatiBititolaritaInail; } set { _DatiBititolaritaInail = value; } }
        [DataMember]
        public DatiLegge460 DatiLegge460 { get { return _DatiLegge460; } set { _DatiLegge460 = value; } }
        [DataMember]
        public DatiLiquidazionePensioneStorico DatiLiquidazionePensioneStorico { get { return _DatiLiquidazionePensioneStorico; } set { _DatiLiquidazionePensioneStorico = value; } }
        [DataMember]
        public DatiIstruttoriaINPDAP DatiIstruttoriaINPDAP { get { return _DatiIstruttoriaINPDAP; } set { _DatiIstruttoriaINPDAP = value; } }
        [DataMember]
        public List<RecordFondo> ListaRecordFondo { get { return _ListaRecordFondo; } set { _ListaRecordFondo = value; } }
        [DataMember]
        public List<Entity.RipartizioneINPDAP> ListaRipartizioneINPDAP { get; set; }
        [DataMember]
        public List<Entity.DecodificaEnteRipartizioneINPDAP> ListaDecEnteRipartizioneINPDAP { get; set; }
        [DataMember]
        public List<DatiAttivitaSvolta> ListaAttivitaSvolte { get { return _ListaAttivitaSvolte; } set { _ListaAttivitaSvolte = value; } }
        [DataMember]
        public List<CodiceRequisito1> ListaCodiceRequisito1 { get { return _ListaCodiceRequisito1; } set { _ListaCodiceRequisito1 = value; } }
        [DataMember]
        public List<CodiceRequisito2> ListaCodiceRequisito2 { get { return _ListaCodiceRequisito2; } set { _ListaCodiceRequisito2 = value; } }
        [DataMember]
        public List<CodiceSpecifico> ListaCodiceSpecifico { get { return _ListaCodiceSpecifico; } set { _ListaCodiceSpecifico = value; } }
        [DataMember]
        public List<CodiceConvenzioneInternazionale> ListaCodiceConvenzioneInternazionale { get { return _ListaCodiceConvenzioneInternazionale; } set { _ListaCodiceConvenzioneInternazionale = value; } }
        [DataMember]
        public List<CodiceEsodo> ListaCodiceEsodo { get { return _ListaCodiceEsodo; } set { _ListaCodiceEsodo = value; } }
        [DataMember]
        public List<CodicePartTime> ListaCodicePartTime { get { return _ListaCodicePartTime; } set { _ListaCodicePartTime = value; } }
        [DataMember]
        public List<CodiceArt22> ListaCodiceArt22 { get { return _ListaCodiceArt22; } set { _ListaCodiceArt22 = value; } }
        [DataMember]
        public List<CodiceCapitalizzazione> ListaCodiceCapitalizzazione { get { return _ListaCodiceCapitalizzazione; } set { _ListaCodiceCapitalizzazione = value; } }
        [DataMember]
        public List<CausaCessazione> ListaCausaCessazione { get { return _ListaCausaCessazione; } set { _ListaCausaCessazione = value; } }
        [DataMember]
        public List<TipoCalcolo> ListaTipoCalcolo { get { return _ListaTipoCalcolo; } set { _ListaTipoCalcolo = value; } }
        [DataMember]
        public INPS.Pensioni.Liquidazione.BLCommon.Utility.CategoriaFondoPI? CategoriaFondoPI { get { return _categoriaFondoPI; } set { _categoriaFondoPI = value; } }
        [DataMember]
        public GestioneLiquidazionePensione.TipoSalvaguardia? TipologiaSalvaguardia { get { return _TipologiaSalvaguardia; } set { _TipologiaSalvaguardia = value; } }
        [DataMember]
        public List<CodiceEliminazione> ListaCodiceEliminazione { get { return _ListaCodiceEliminazione; } set { _ListaCodiceEliminazione = value; } }
        [DataMember]
        public List<CodiceParticolare> ListaCodiceParticolare { get { return _ListaCodiceParticolare; } set { _ListaCodiceParticolare = value; } }
        [DataMember]
        public List<TipoLiquidazionePM> ListaTipoLiquidazionePM { get { return _ListaTipoLiquidazionePM; } set { _ListaTipoLiquidazionePM = value; } }
        [DataMember]
        public List<CodiceLegge413> ListaCodiceLegge413 { get { return _ListaCodiceLegge413; } set { _ListaCodiceLegge413 = value; } }
        [DataMember]
        public List<AttivitaSvolta2> ListaAttivitaSvolta2 { get { return _ListaAttivitaSvolta2; } set { _ListaAttivitaSvolta2 = value; } }
        [DataMember]
        public List<TipoLiquidazione> ListaTipoLiquidazione { get { return _ListaTipoLiquidazione; } set { _ListaTipoLiquidazione = value; } }
        [DataMember]
        public List<CodiciNatura> ListaCodiciNatura { get { return _ListaCodiciNatura; } set { _ListaCodiciNatura = value; } }
        [DataMember]
        public List<CodiceTipoLiquidazionePM> ListaCodiceTipoLiquidazionePM { get { return _ListaCodiceTipoLiquidazionePM; } set { _ListaCodiceTipoLiquidazionePM = value; } }
        [DataMember]
        public List<PersonaleViaggiante> ListaPersonaleViaggiante { get { return _ListaPersonaleViaggiante; } set { _ListaPersonaleViaggiante = value; } }
        [DataMember]
        public List<MicroqualificaINPDAP> ListaMicroqualificaINPDAP { get { return _ListaMicroqualificaINPDAP; } set { _ListaMicroqualificaINPDAP = value; } }
        [DataMember]
        public DateTime? DecorrenzaPensioneDirettaDC { get { return _DecorrenzaPensioneDirettaDC; } set { _DecorrenzaPensioneDirettaDC = value; } }
        [DataMember]
        public Dictionary<string, char?> TipoPensione { get { return _TipoPensione; } set { _TipoPensione = value; } }

        [DataMember]
        public List<CtrlCompartoSettoreRuolo> ListaCtrlCompartoSettoreRuolo { get { return _ListaCtrlCompartoSettoreRuolo; } set { _ListaCtrlCompartoSettoreRuolo = value; } }


        [DataMember]
        public bool? IsEsenzioneFiscaleEstero { get { return _IsEsenzioneFiscaleEstero; } set { _IsEsenzioneFiscaleEstero = value; } }
        [DataMember]
        public bool? IsResidenteEstero { get { return _IsResidenteEstero; } set { _IsResidenteEstero = value; } }
        [DataMember]
        public bool? IsEsenzioneFiscaleVittima { get { return _IsEsenzioneFiscaleVittima; } set { _IsEsenzioneFiscaleVittima = value; } }
        [DataMember]
        public bool? IsRequisitiL247_L243Enable { get { return _IsRequisitiL247_L243Enable; } set { _IsRequisitiL247_L243Enable = value; } }
        [DataMember]
        public bool? IsCodiceSpecificoVisible { get { return _IsCodiceSpecificoVisible; } set { _IsCodiceSpecificoVisible = value; } }
        [DataMember]
        public bool? IsVisibleArt2 { get { return _IsVisibleArt2; } set { _IsVisibleArt2 = value; } }
        [DataMember]
        public bool? IsDecPensAnteAgosto95 { get { return _IsDecPensAnteAgosto95; } set { _IsDecPensAnteAgosto95 = value; } }
        [DataMember]
        public bool? IsCodiceNatura2Enabled { get { return _IsCodiceNatura2Enabled; } set { _IsCodiceNatura2Enabled = value; } }
        [DataMember]
        public bool? IsUsuranti { get { return _IsUsuranti; } set { _IsUsuranti = value; } }
        [DataMember]
        public bool? IsVecchPerditaTitolo { get { return _IsVecchPerditaTitolo; } set { _IsVecchPerditaTitolo = value; } }
        [DataMember]
        public bool? IsCodiceSpecificoEnabled { get { return _IsCodiceSpecificoEnabled; } set { _IsCodiceSpecificoEnabled = value; } }
        [DataMember]
        public bool? IsCodiceArt22Enabled { get { return _IsCodiceArt22Enabled; } set { _IsCodiceArt22Enabled = value; } }
        [DataMember]
        public bool? IsDomandaTrasformazioneAOI { get { return _IsDomandaTrasformazioneAOI; } set { _IsDomandaTrasformazioneAOI = value; } }
        [DataMember]
        public bool? IsCodDirittoQuoteFisseVisible { get { return _IsCodDirittoQuoteFisseVisible; } set { _IsCodDirittoQuoteFisseVisible = value; } }
        [DataMember]
        public bool? IsIndennitaAggiuntivaVisible { get { return _IsIndennitaAggiuntivaVisible; } set { _IsIndennitaAggiuntivaVisible = value; } }
        [DataMember]
        public bool? IsDecorrenzaSuccSett1989 { get { return _IsDecorrenzaSuccSett1989; } set { _IsDecorrenzaSuccSett1989 = value; } }
        [DataMember]
        public bool? IsCodiceComunicazione3Visible { get { return _IsCodiceComunicazione3Visible; } set { _IsCodiceComunicazione3Visible = value; } }
        [DataMember]
        public bool? IsProvvisoriaVisible { get { return _IsProvvisoriaVisible; } set { _IsProvvisoriaVisible = value; } }
        [DataMember]
        public bool? IsCodiceNatura2DisabledPerSperDonna { get { return _IsCodiceNatura2DisabledPerSperDonna; } set { _IsCodiceNatura2DisabledPerSperDonna = value; } }
        [DataMember]
        public bool? IsDomandaConNuovaGestioneDatiFondoFSPT { get { return _IsDomandaConNuovaGestioneDatiFondoFSPT; } set { _IsDomandaConNuovaGestioneDatiFondoFSPT = value; } }
        [DataMember]
        public bool? IsDomandaPersonaleViaggianteSenzaPerditaTitoloAbilitante { get { return _IsDomandaPersonaleViaggianteSenzaPerditaTitoloAbilitante; } set { _IsDomandaPersonaleViaggianteSenzaPerditaTitoloAbilitante = value; } }
        [DataMember]
        public bool? IsDomandaPersonaleViaggianteSenzaPerditaTitoloAbilitanteVOCPDEL { get { return _IsDomandaPersonaleViaggianteSenzaPerditaTitoloAbilitanteVOCPDEL; } set { _IsDomandaPersonaleViaggianteSenzaPerditaTitoloAbilitanteVOCPDEL = value; } }
        [DataMember]
        public bool? IsDomandaPersonaleViaggianteConPerditaTitoloAbilitante { get { return _IsDomandaPersonaleViaggianteConPerditaTitoloAbilitante; } set { _IsDomandaPersonaleViaggianteConPerditaTitoloAbilitante = value; } }
        [DataMember]
        public bool? IsDomandaPersonaleViaggianteConPerditaTitoloAbilitanteVOCPDEL { get { return _IsDomandaPersonaleViaggianteConPerditaTitoloAbilitanteVOCPDEL; } set { _IsDomandaPersonaleViaggianteConPerditaTitoloAbilitanteVOCPDEL = value; } }
        [DataMember]
        public bool? IsDomandaAnteArmonizzazione { get { return _IsDomandaAnteArmonizzazione; } set { _IsDomandaAnteArmonizzazione = value; } }
        [DataMember]
        public bool? IsCapitalizzazioneVisible { get { return _IsCapitalizzazioneVisible; } set { _IsCapitalizzazioneVisible = value; } }
        [DataMember]
        public bool? IsTrimestreAnzianitaRequisitiNoInvaliditaVisible { get; set; }
        [DataMember]
        public bool? IsBeneficioArt24Comma15BisFromFELPE { get; set; }
        [DataMember]
        public bool? IsPensioneTipoContributivo { get; set; }
        [DataMember]
        public bool? IsPensioneTipoContributivoConOpzione { get; set; }
        [DataMember]
        public bool? IsSperimentaleDonna { get; set; }
        [DataMember]
        public bool? IsRiduzioneRetribVisible { get { return _IsRiduzioneRetribVisible; } set { _IsRiduzioneRetribVisible = value; } }
        [DataMember]
        public bool? IsRiduzioneRetributivaEnabled { get { return _IsRiduzioneRetributivaEnabled; } set { _IsRiduzioneRetributivaEnabled = value; } }
        [DataMember]
        public bool? IsBeneficioApePrecociFromFELPE { get; set; }
        [DataMember]
        public bool? IsEsenzioneFiscaleEsteroFromDetrazioni { get; set; }
        [DataMember]
        public bool? IsReversibilitaOrRicostituzione { get; set; }
        [DataMember]
        public bool? IsRicostituzioneForMemo72 { get; set; }
        [DataMember]
        public bool? IsRichiestaBonusBookingAbilitata { get; set; }
        [DataMember]
        public bool? IsPrimoVersamentoNonObbligatorio { get; set; }
        [DataMember]
        public bool? IsBeneficioNonVedente { get; set; }
        [DataMember]
        public bool? IsDataRinunciaTrattenutaInpdapStorico { get; set; }
        [DataMember]
        public bool? IsBeneficioNonVedenteFromStorico { get; set; }
        [DataMember]
        public bool? IsRichiestaBonus154Abilitata { get; set; }
        [DataMember]
        public bool? IsCodComunicazioniEsenzioneFiscaleVittimaVisibile { get; set; }
        [DataMember]
        public bool? IsOpzioneDonna_Legge197_2022_Art1_Comma292OrRicostituzione { get; set; }
        [DataMember]
        public bool? isSenzaLegge33670 { get; set; }
        //ENG - Aggiornamento Memo86
        [DataMember]
        public bool? IsPresenteTrattenutaFondoCreditoDaPrelievo { get; set; }
        [DataMember]
        public DateTime? DataPrelievoDomanda { get; set; }
        [DataMember]
        public char? TipoReversibilita { get; set; }

        [DataMember]
        public bool? IsMiglioramentiContrattualiAutomatici { get; set; }
        
        #endregion public data member

        #region nested class

        #endregion nested class
    }
}
