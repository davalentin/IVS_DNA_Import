using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using System.Linq;
using System.Web;
using System.Data;
using INPS.Pensioni.LiquidazioneAgo.Entity;
using INPS.Pensioni.Liquidazione.BLCommon;

namespace INPS.Pensioni.LiquidazioneAgo.Service.Contracts.DataContracts
{
    [DataContract]
    public class AreaLiquidazionePensione
    {
        #region private properties
        private DatiGenerici _DatiGenerici;
        private DatiAssicurativi _DatiAssicurativi;
        private DatiOpzione _DatiOpzione;
        private DatiIstruttoria _DatiIstruttoria;
        private DatiProvenienza _DatiProvenienza;
        private DatiInail _DatiInail;
        private DatiLiquidazionePensioneStorico _DatiLiquidazionePensioneStorico;
        private DatiSentenzaArt4 _DatiSentenzaArt4;
        private DatiSentenze _DatiSentenze;
        private List<Mobilita> _lMobilita;
        private List<CDCMMR> _lCDCMMR;
        private List<CodiceParticolare> _lCodiceParticolare;
        private List<DecodificaLegge44997> _lDecodificaLegge44997;
        private List<DomandaRicorso> _lDomandaRicorso;
        private List<CodiciNatura> _lCodiciNatura;
        private List<DecModalitaLiquidazione> _lModalitaLiquidazione;
        private List<DecodificaAzienda> _lAziendaEditoria;
        private List<DecodificaRiconoscimentiInvalidita> _lDecodificaRiconoscimentiInvalidita;
        private List<DecodificaDerogaENPALS> _lDecodificaDerogaENPALS;
        private List<DecodificaEnteCassaProfessionale> _lDecodificaEnteCassaProfessionale;
        private List<DecBancaFideiussione> _lDecodificaBancaFideiussione;

        private GestioneLiquidazionePensione.TipoSalvaguardia? _TipologiaSalvaguardia;
        private bool? _IsRiduzioneRetribVisible;
        private bool? _IsEsenzioneFiscaleEstero;
        private bool? _IsAliquotaTfrEsodati;
        private bool? _IsGestioneCOM;
        private bool? _IsCodiceNatura2Enabled;
        private bool? _IsSperimentaleDonna;
        private bool? _IsUsuranti;
        private bool? _IsRimpatriatiAlbania;
        private bool? _IsVecchiaiaInvaliditaSupplementare;
        private bool? _IsDatiExCombattenteENPALSPresenti;
        private bool? _IsDatiBeneficiENPALSPresenti;
        private bool? _IsTabPrepensionamentoVisible;
        private bool? _IsFlagProvvisoriaCheckedAndEnabled;
        private bool? _IsRipristino;
        private bool? _IsRiduzioneRetributivaEnabled;
        private DateTime? _DataAssunzioneCarico;
        #endregion private properties

        #region public data member
        [DataMember]
        public DatiGenerici DatiGenerici { get { return _DatiGenerici; } set { _DatiGenerici = value; } }
        [DataMember]
        public DatiAssicurativi DatiAssicurativi { get { return _DatiAssicurativi; } set { _DatiAssicurativi = value; } }
        [DataMember]
        public DatiOpzione DatiOpzione { get { return _DatiOpzione; } set { _DatiOpzione = value; } }
        [DataMember]
        public DatiIstruttoria DatiIstruttoria { get { return _DatiIstruttoria; } set { _DatiIstruttoria = value; } }
        [DataMember]
        public DatiProvenienza DatiProvenienza { get { return _DatiProvenienza; } set { _DatiProvenienza = value; } }
        [DataMember]
        public DatiInail DatiInail { get { return _DatiInail; } set { _DatiInail = value; } }
        [DataMember]
        public DatiLiquidazionePensioneStorico DatiLiquidazionePensioneStorico { get { return _DatiLiquidazionePensioneStorico; } set { _DatiLiquidazionePensioneStorico = value; } }
        [DataMember]
        public DatiSentenzaArt4 DatiSentenzaArt4 { get { return _DatiSentenzaArt4; } set { _DatiSentenzaArt4 = value; } }
        [DataMember]
        public DatiSentenze DatiSentenze { get { return _DatiSentenze; } set { _DatiSentenze = value; } }
        [DataMember]
        public List<Mobilita> listaMobilita { get { return _lMobilita; } set { _lMobilita = value; } }
        [DataMember]
        public List<CDCMMR> listaCDCMMR { get { return _lCDCMMR; } set { _lCDCMMR = value; } }
        [DataMember]
        public List<CodiceParticolare> listaCodiceParticolare { get { return _lCodiceParticolare; } set { _lCodiceParticolare = value; } }
        [DataMember]
        public List<DecodificaLegge44997> listaDecodificaLegge44997 { get { return _lDecodificaLegge44997; } set { _lDecodificaLegge44997 = value; } }
        [DataMember]
        public List<DomandaRicorso> listaDomandaRicorso { get { return _lDomandaRicorso; } set { _lDomandaRicorso = value; } }
        [DataMember]
        public List<CodiciNatura> listaCodiciNatura { get { return _lCodiciNatura; } set { _lCodiciNatura = value; } }
        [DataMember]
        public List<DecModalitaLiquidazione> listaModalitaLiquidazione { get { return _lModalitaLiquidazione; } set { _lModalitaLiquidazione = value; } }
        [DataMember]
        public List<DecodificaAzienda> listaAziendaEditoria { get { return _lAziendaEditoria; } set { _lAziendaEditoria = value; } }
        [DataMember]
        public List<DecodificaRiconoscimentiInvalidita> listaRiconoscimentiInvalidita { get { return _lDecodificaRiconoscimentiInvalidita; } set { _lDecodificaRiconoscimentiInvalidita = value; } }
        [DataMember]
        public List<DecodificaDerogaENPALS> listaDecodificaDerogaENPALS { get { return _lDecodificaDerogaENPALS; } set { _lDecodificaDerogaENPALS = value; } }
        [DataMember]
        public List<DecodificaEnteCassaProfessionale> listaDecodificaEnteCassaProfessionale { get { return _lDecodificaEnteCassaProfessionale; } set { _lDecodificaEnteCassaProfessionale = value; } }
        [DataMember]
        public List<DecBancaFideiussione> listaDecodificaBancaFideiussione { get { return _lDecodificaBancaFideiussione; } set { _lDecodificaBancaFideiussione = value; } }
        [DataMember]
        public INPS.Pensioni.Liquidazione.BLCommon.Entity.DatiContribuzioneEnpals DatiContribuzioneEnpals { get; set; }
        [DataMember]
        public List<Entity.DecAziendeScadenzaAssegnoGGmmAAAA> ListaAziendeScadenzaAssegnoGGMMAAAA { get; set; }
        [DataMember]
        public List<Entity.DecAnagraficaAccordi> ListaDecAnagraficaAccordi { get; set; }
        [DataMember]
        public List<Entity.DecAnagraficaAziende> ListaDecAnagraficaAziende { get; set; }
        [DataMember]
        public List<Entity.DecAnagraficaAccordiPerTipo0171> ListaDecAnagraficaAccordiPerTipo0171 { get; set; }
        [DataMember]
        public List<Entity.DecAnagraficaAziendePerTipo0171> ListaDecAnagraficaAziendePerTipo0171 { get; set; }
        [DataMember]
        public List<Entity.DecAnagraficaAccordiPerTipo0179> ListaDecAnagraficaAccordiPerTipo0179 { get; set; }
        [DataMember]
        public List<Entity.DecAnagraficaAziendePerTipo0179> ListaDecAnagraficaAziendePerTipo0179 { get; set; }
        [DataMember]
        public List<Entity.DecAnagraficaAccordiLetteraB> ListaDecAnagraficaAccordiLetteraB { get; set; }
        [DataMember]
        public List<Entity.DecAnagraficaAziendeLetteraB> ListaDecAnagraficaAziendeLetteraB { get; set; }
        [DataMember]
        public List<Entity.DecodificaBanchePerSede> ListaDecodificaBanchePerSede { get; set; }
        [DataMember]
        public List<CtrlScadenzaIndennizzoINDCOM> ListaCtrlScadenzaIndennizzoINDCOM { get; set; }

        [DataMember]
        public GestioneLiquidazionePensione.TipoSalvaguardia? TipologiaSalvaguardia { get { return _TipologiaSalvaguardia; } set { _TipologiaSalvaguardia = value; } }
        [DataMember]
        public bool? IsRiduzioneRetribVisible { get { return _IsRiduzioneRetribVisible; } set { _IsRiduzioneRetribVisible = value; } }
        [DataMember]
        public bool? IsEsenzioneFiscaleEstero { get { return _IsEsenzioneFiscaleEstero; } set { _IsEsenzioneFiscaleEstero = value; } }
        [DataMember]
        public bool? IsAliquotaTfrEsodati { get { return _IsAliquotaTfrEsodati; } set { _IsAliquotaTfrEsodati = value; } }
        [DataMember]
        public bool? IsGestioneCOM { get { return _IsGestioneCOM; } set { _IsGestioneCOM = value; } }
        [DataMember]
        public bool? IsCodiceNatura2Enabled { get { return _IsCodiceNatura2Enabled; } set { _IsCodiceNatura2Enabled = value; } }
        [DataMember]
        public bool? IsSperimentaleDonna { get { return _IsSperimentaleDonna; } set { _IsSperimentaleDonna = value; } }
        [DataMember]
        public bool? IsUsuranti { get { return _IsUsuranti; } set { _IsUsuranti = value; } }
        [DataMember]
        public bool? IsRimpatriatiAlbania { get { return _IsRimpatriatiAlbania; } set { _IsRimpatriatiAlbania = value; } }
        [DataMember]
        public bool? IsVecchiaiaInvaliditaSupplementare { get { return _IsVecchiaiaInvaliditaSupplementare; } set { _IsVecchiaiaInvaliditaSupplementare = value; } }
        [DataMember]
        public bool? IsDatiExCombattenteENPALSPresenti { get { return _IsDatiExCombattenteENPALSPresenti; } set { _IsDatiExCombattenteENPALSPresenti = value; } }
        [DataMember]
        public bool? IsDatiBeneficiENPALSPresenti { get { return _IsDatiBeneficiENPALSPresenti; } set { _IsDatiBeneficiENPALSPresenti = value; } }
        [DataMember]
        public bool? IsTabPrepensionamentoVisible { get { return _IsTabPrepensionamentoVisible; } set { _IsTabPrepensionamentoVisible = value; } }
        [DataMember]
        public bool? IsFlagProvvisoriaCheckedAndEnabled { get { return _IsFlagProvvisoriaCheckedAndEnabled; } set { _IsFlagProvvisoriaCheckedAndEnabled = value; } }
        [DataMember]
        public bool? IsRipristino { get { return _IsRipristino; } set { _IsRipristino = value; } }
        [DataMember]
        public bool? IsRiduzioneRetributivaEnabled { get { return _IsRiduzioneRetributivaEnabled; } set { _IsRiduzioneRetributivaEnabled = value; } }
        [DataMember]
        public bool? IsDomandaTrasformazioneInvalidita { get; set; }
        [DataMember]
        public bool? IsDomandaAmianto181FromUnicarpe { get; set; }
        [DataMember]
        public bool? IsDatiBeneficiSalvati { get; set; }
        [DataMember]
        public bool? IsDomandaVESO92WithFiltroL92 { get; set; }
        [DataMember]
        public string CodiceAziendaFromPatronato { get; set; }
        [DataMember]
        public bool? IsDatiCalcoloDAIAltraGestionePresent { get; set; }
        [DataMember]
        public bool IsContribuzioneEnpalsRetributivaVisible { get; set; }
        [DataMember]
        public bool IsContribuzioneEnpalsContributivaVisible { get; set; }
        [DataMember]
        public bool IsEsenzioneFiscaleVittima { get; set; }
        [DataMember]
        public bool? IsRequisitiL247_L243Enable { get; set; }
        [DataMember]
        public bool? IsCodiceComunicazione3Visible { get; set; }
        [DataMember]
        public bool? IsProvvisoriaVisible { get; set; }
        [DataMember]
        public DateTime? DecorrenzaPensioneDirettaDC { get; set; }
        [DataMember]
        public Dictionary<string, byte?> TipoPensione { get; set; }
        [DataMember]
        public bool? IsDecPensAnteAgosto95 { get; set; }
        [DataMember]
        public bool? IsBeneficioArt24Comma15BisFromFELPE { get; set; }
        [DataMember]
        public bool? IsPensioneTipoContributivo { get; set; }
        [DataMember]
        public bool? IsPensioneTipoContributivoConOpzione { get; set; }
        [DataMember]
        public bool? IsPrepensionamentoEditoriaFiltroEAA { get; set; }
        [DataMember]
        public bool? IsPrepensionamentoEditoriaArt1c154L205_2017 { get; set; }
        [DataMember]
        public bool? IsPrepensionamentoEditoriaArt1c500L160_2019 { get; set; }
        [DataMember]
        public bool? IsBeneficioApePrecociFromFELPE { get; set; }
        [DataMember]
        public bool? IsDomandaCasellario { get; set; }
        [DataMember]
        public bool? IsEsenzioneFiscaleEsteroFromDetrazioni { get; set; }
        [DataMember]
        public bool? IsDomandaInabilitaSpecificaENPALS { get; set; }
        [DataMember]
        public bool? IsPensioneInvaliditaInabilitaENPALSOrCasellario { get; set; }
        [DataMember]
        public bool? IsBeneficioInabilitaByPrimoCodiceNatura { get; set; }
        [DataMember]
        public bool? IsRichiestaBonusBookingAbilitata { get; set; }
        [DataMember]
        public bool? IsRiaperturaPerCausaPersa { get; set; }
        [DataMember]
        public bool? IsScadenzaStoricoValorizzata { get; set; }
        [DataMember]
        public bool? IsRicEnpalsMotiviContributivi { get; set; }
        [DataMember]
        public bool? IsBeneficioNonVedente { get; set; }
        [DataMember]
        public bool? IsDataRinunciaTrattenutaInpdapStorico { get; set; }
        [DataMember]
        public bool? IsBeneficioNonVedenteFromStorico { get; set; }
        [DataMember]
        public Utility.TipoAnte96? IsAnte96 { get; set; }
        [DataMember]
        public bool? IsRichiestaBonus154Abilitata { get; set; }
        [DataMember]
        public bool? IsDomandaESPAFiltroL26 { get; set; }
        [DataMember]
        public bool? IsDomandaVESO33FiltroDAP { get; set; }
        [DataMember]
        public bool? IsDomandaRicTrfCred27GestioneL { get; set; }
        [DataMember]
        public bool? IsEliminataPerCauseVarie { get; set; }
        [DataMember]
        public bool? IsOpzioneDonna_Legge197_2022_Art1_Comma292OrRicostituzione { get; set; }
        [DataMember]
        public bool? IsPrepensionamentoEditoriaFiltroEBA { get; set; }
        [DataMember]
        public bool? IsRicConTerzoCodNaturaZAttEconomica67ProfIndividuale11 { get; set; }
        [DataMember]
        public DateTime? DataAssunzioneCarico { get; set; }

        [DataMember]
        public bool? IsDatiContributiviPresenti { get; set; }

        [DataMember]
        public bool? IsDatiRetributiviPresenti { get; set; }

        //ENG - Aggiornamento Memo
        [DataMember]
        public bool? IsPresenteTrattenutaFondoCreditoDaPrelievo { get; set; }
        [DataMember]
        public DateTime? DataPrelievoDomanda { get; set; }
        [DataMember]
        public string TipoSettimaneBeneficio { get; set; }

        //ENG - RIC/TRF (NO ENPALS): rendere non obbligatori i campi "Attivita Economica" e "Professione Individuale" se dal prelievo arrivano vuoti
        [DataMember]
        public bool? IsAttivitaEconomicaDaPrelievo { get; set; }
        [DataMember]
        public bool? IsProfessioneIndividualeDaPrelievo { get; set; }

        [DataMember]
        public bool? IsMemo74_2023Abilitato { get; set; }

        //ENG - Memo 108_2024
        [DataMember]
        public bool? IsFlagProvvisoriaFromCumulo { get; set; }

        [DataMember]
        public bool? IsBypassCompartoScuolaAttivo { get; set; }

        [DataMember]
        public bool? IsDomandaCOOP28FiltroDAP { get; set; }

        #endregion public data member

        #region nested class

        #endregion nested class
    }
}
