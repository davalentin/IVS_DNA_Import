using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using System.Linq;
using System.Web;
using System.Data;
using INPS.Pensioni.Liquidazione.BLCommon;


namespace INPS.Pensioni.Liquidazione.Service.Contracts.DataContracts
{
    [DataContract]
    public class AreaTitolare
    {
        public AreaTitolare()
        {
            this._Esito = new AreaEsito();
            this._ElencoStatiCiviliTitolare = new List<DatiStatoCivileTitolare>();
            this._ElencoResidenzeEstereTitolare = new List<DatiResidenzaEsteroTitolare>();
            this._Anagrafica = new AreaRispostaRiepilogo.DatiRiepilogoAnagrafica();
            this._Patronato = new DatiPatronato();
            this._Sindacato = new DatiSindacato();
            this._Pensione = new DatiPensione();
        }

        #region private properties

        private List<DatiStatoCivileTitolare> _ElencoStatiCiviliTitolare;

        private List<DatiResidenzaEsteroTitolare> _ElencoResidenzeEstereTitolare;

        private AreaRispostaRiepilogo.DatiRiepilogoAnagrafica _Anagrafica;

        private DatiPatronato _Patronato;

        private DatiSindacato _Sindacato;

        private DatiPensione _Pensione;

        private AreaEsito _Esito;

        private List<Liquidazione.BLCommon.Entity.Sindacato> _ElencoSindacati;

        #endregion private properties

        #region public data member

        [DataMember]
        public List<DatiStatoCivileTitolare> ElencoStatiCiviliTitolare { get { return _ElencoStatiCiviliTitolare; } set { _ElencoStatiCiviliTitolare = value; } }

        [DataMember]
        public List<DatiResidenzaEsteroTitolare> ElencoResidenzeEstereTitolare { get { return _ElencoResidenzeEstereTitolare; } set { _ElencoResidenzeEstereTitolare = value; } }

        [DataMember]
        public AreaRispostaRiepilogo.DatiRiepilogoAnagrafica Anagrafica { get { return _Anagrafica; } set { _Anagrafica = value; } }

        [DataMember]
        public DatiPatronato Patronato { get { return _Patronato; } set { _Patronato = value; } }

        [DataMember]
        public DatiSindacato Sindacato { get { return _Sindacato; } set { _Sindacato = value; } }

        [DataMember]
        public DatiPensione Pensione { get { return _Pensione; } set { _Pensione = value; } }

        [DataMember]
        public AreaEsito Esito { get { return _Esito; } set { _Esito = value; } }

        [DataMember]
        public List<Liquidazione.BLCommon.Entity.Sindacato> ElencoSindacati { get { return _ElencoSindacati; } set { _ElencoSindacati = value; } }

        [DataMember]
        public bool IsContitolareConiuge { get; set; }

        [DataMember]
        public bool IsDecorrenzaDisabledPerSuperstiti { get; set; }

        [DataMember]
        public bool IsContitolareAscendente { get; set; }

        [DataMember]
        public bool IsContitolareExConiuge { get; set; }

        [DataMember]
        public bool IsSceltaLavoratriciMadriVisible { get; set; }

        [DataMember]
        public bool IsEnteIstruttoreFondoExINPDAP { get; set; }

        [DataMember]
        public bool IsRicVoautNoFiltroPavAssunzioneCaricoEntro042024 { get; set; }

        #endregion public data member

        #region nested class

        [DataContract]
        public class DatiStatoCivileTitolare
        {
            public DatiStatoCivileTitolare()
            {
            }

            internal DatiStatoCivileTitolare(BLCommon.GestioneAnagrafica.DatiStatoCivile statoCivile)
            {
                this._Codice = statoCivile.Codice;
                this._Decorrenza = statoCivile.Decorrenza;
            }

            #region private properties
            private char _Codice;

            private DateTime? _Decorrenza;
            #endregion private properties

            #region public data member
            [DataMember]
            public char Codice { get { return _Codice; } set { _Codice = value; } }
            [DataMember]
            public DateTime? Decorrenza { get { return _Decorrenza; } set { _Decorrenza = value; } }
            #endregion public data member
        }

        [DataContract]
        public class DatiResidenzaEsteroTitolare
        {
            public DatiResidenzaEsteroTitolare()
            {
            }

            internal DatiResidenzaEsteroTitolare(BLCommon.GestioneAnagrafica.DatiResidenzaEstero residenzaEstero)
            {
                this._CodCatastaleStatoEE = residenzaEstero.CodCatastaleStatoEE;
                this._Decorrenza = residenzaEstero.Decorrenza;
            }

            #region private properties
            private string _CodCatastaleStatoEE;

            private DateTime? _Decorrenza;
            #endregion private properties

            #region public data member
            [DataMember]
            public string CodCatastaleStatoEE { get { return _CodCatastaleStatoEE; } set { _CodCatastaleStatoEE = value; } }
            [DataMember]
            public DateTime? Decorrenza { get { return _Decorrenza; } set { _Decorrenza = value; } }
            #endregion public data member
        }

        [DataContract]
        public class DatiPatronato
        {
            public DatiPatronato()
            {
            }

            internal DatiPatronato(BLCommon.GestionePensione.DatiPatronato patronato)
            {
                this._CodiceEnte = patronato.CodiceEnte;
                this._CodiceUfficio = patronato.CodiceUfficio;
                this.NPratica = patronato.NPratica;
                this.TipoUfficio = patronato.TipoUfficio;
                GestioneDecodifica.Patronato descPatronato = null;
                GestioneDecodifica.GetPatronatoByEnteUfficio(this._CodiceEnte, this._CodiceUfficio, out descPatronato);
                if (descPatronato != null)
                    this._Descrizione = descPatronato.DescPatronato.Trim() + " - " + descPatronato.DescUfficioPatronato;
            }

            #region private properties
            private string _CodiceEnte;

            private string _CodiceUfficio;

            private string _NPratica;

            private string _TipoUfficio;

            private string _Descrizione;
            #endregion private properties

            #region public data member
            [DataMember]
            public string CodiceEnte { get { return _CodiceEnte; } set { _CodiceEnte = value; } }

            [DataMember]
            public string CodiceUfficio { get { return _CodiceUfficio; } set { _CodiceUfficio = value; } }

            [DataMember]
            public string NPratica { get { return _NPratica; } set { _NPratica = value; } }

            [DataMember]
            public string TipoUfficio { get { return _TipoUfficio; } set { _TipoUfficio = value; } }

            [DataMember]
            public string Descrizione { get { return _Descrizione; } set { _Descrizione = value; } }
            #endregion public data member
        }

        [DataContract]
        public class DatiSindacato
        {
            public DatiSindacato()
            {
            }

            internal DatiSindacato(BLCommon.GestionePensione.DatiSindacato sindacato)
            {
                this._CessazioneSindacato = sindacato.CessazioneSindacato;
                this._CodiceSindacato = sindacato.CodiceSindacato;
                this._DescrizioneSindacato = sindacato.DescrizioneSindacato;
                this._DecorrenzaSindacato = sindacato.DecorrenzaSindacato;
                this._IsFromService = sindacato.IsFromService;
            }

            #region private properties
            private string _CodiceSindacato;

            private string _DescrizioneSindacato;

            private System.Nullable<System.DateTime> _DecorrenzaSindacato;

            private System.Nullable<System.DateTime> _CessazioneSindacato;

            private Utility.StatoSindacato? _Stato;

            private bool? _IsFromService;
            #endregion private properties

            #region public data member
            [DataMember]
            public string CodiceSindacato { get { return _CodiceSindacato; } set { _CodiceSindacato = value; } }

            [DataMember]
            public string DescrizioneSindacato { get { return _DescrizioneSindacato; } set { _DescrizioneSindacato = value; } }

            [DataMember]
            public System.Nullable<System.DateTime> DecorrenzaSindacato { get { return _DecorrenzaSindacato; } set { _DecorrenzaSindacato = value; } }

            [DataMember]
            public System.Nullable<System.DateTime> CessazioneSindacato { get { return _CessazioneSindacato; } set { _CessazioneSindacato = value; } }

            [DataMember]
            public Utility.StatoSindacato? Stato { get { return _Stato; } set { _Stato = value; } }

            [DataMember]
            public bool? IsFromService { get { return _IsFromService; } set { _IsFromService = value; } }
            #endregion public data member
        }

        [DataContract]
        public class DatiPensione
        {
            public DatiPensione()
            {
            }

            internal DatiPensione(BLCommon.GestionePensione.DatiPensione pensione)
            {
                this._DecorrenzaOriginaria = pensione.DecorrenzaOriginaria;
                this._DataPresentazioneDomanda = pensione.DataPresentazioneDomanda;
                this._DataPerfezionamentoRequisiti = pensione.DataPerfezionamentoRequisiti;
                this._NDomus = pensione.NDomus;
                this._ProgStorico = pensione.ProgStorico;
                BLCommon.GestioneDecodifica.GetGestioneFondoInChiaro(pensione.Gestione, pensione.Fondo, out this._Gestione, out this._Fondo);
                BLCommon.GestioneDecodifica.GetProdottoInChiaro(pensione.Prodotto, out this._Prodotto);
                BLCommon.GestioneDecodifica.GetTipologiaInChiaro(pensione.Tipo, out this._Tipologia);
                BLCommon.GestioneDecodifica.GetEnteInChiaro(pensione.Ente, out this._Ente);
                BLCommon.GestioneDecodifica.GetFiltroInChiaro(pensione.CodiceTipoRichiesta, out this._Filtro);

                if (pensione.Tipo == "0184")
                    this._Prodotto = this._Tipologia;

                if (pensione.Gruppo == "0002" && pensione.Prodotto == "0012" && pensione.Tipo == "0052" && pensione.Gestione == "202" && pensione.Fondo == "144")
                    this._Fondo = "IPOST/FERROVIE";

                this._FlagUnicarpe = pensione.FlagUnicarpe;
                this._IndConvInt = pensione.IndConvInt;
                Utility.TipoDomanda tipoDomanda = Utility.GetTipoDomanda(pensione.Gruppo, pensione.Prodotto);
                this._CodeProdotto = pensione.Prodotto;
                this._CentroOperativo = pensione.CentroOperativo;
                this._CodeGruppo = pensione.Gruppo;
                this._CodeTipo = pensione.Tipo;
                this._CodiceTipoRichiesta = pensione.CodiceTipoRichiesta;
                this._DataPerfezionamentoRequisitiUnicarpe = pensione.DataPerfezionamentoRequisitiUnicarpe;
                this._TipoLetturaUnicarpe = pensione.TipoLetturaUnicarpe;
                this._IsPLUnicarpe = pensione.IsPLUnicarpe;
                switch (tipoDomanda)
                {
                    case Utility.TipoDomanda.Superstiti:
                        this._Tipo = TipoDomanda.Superstiti;
                        break;
                    case Utility.TipoDomanda.Ricostituzione:
                        this._Tipo = TipoDomanda.Ricostituzione;
                        break;
                    case Utility.TipoDomanda.RiliquidazioneSuperstiti:
                        this._Tipo = TipoDomanda.RiliquidazioneSuperstiti;
                        break;
                    case Utility.TipoDomanda.Riliquidazione:
                        this._Tipo = TipoDomanda.Riliquidazione;
                        break;
                    case Utility.TipoDomanda.Ripristino:
                        this._Tipo = TipoDomanda.Ripristino;
                        break;
                    case Utility.TipoDomanda.RipristinoSuperstiti:
                        this._Tipo = TipoDomanda.RipristinoSuperstiti;
                        break;
                    default:
                        this._Tipo = TipoDomanda.Normale;
                        break;
                }

                BLCommon.Utility.TipoAppartenenza? tipoAppartenenza = BLCommon.Utility.GetTipoAppartenenza(pensione.IndConvInt, pensione.Gestione);
                DateTime? dataValidita = null;
                if (Utility.IsDomandaPensioneSuperstitiOrRicostituzione(pensione))
                {
                    //ENG - Spacchettate SOPGI
                    BLCommon.GestioneDanteCausa.DatiDanteCausa danteCausa = null;
                    BLCommon.GestioneDanteCausa.GetDanteCausabyIdPensione(pensione.Id, out danteCausa);

                    //ENG - RIC REVERSIBILITA 024: implementazione flusso per riconoscere le reversibilità "vecchie" 
                    GestioneLavorazione.DatiLavorazione datiLavorazione = null;
                    GestioneLavorazione.GetLavorazioneByIdPensione(pensione.Id, out datiLavorazione);

                    if (!Utility.IsDomandaENPALS(pensione.Gestione) && !Utility.IsDomandaSpacchettamentoSOPGIPost072022(pensione, danteCausa) && !Utility.IsDomandaSpacchettamentoSO(pensione, Utility.IsRiaperturaDomanda(pensione.Id)) &&
                        !Utility.IsDomandaSpacchettamentoSOART(pensione, Utility.IsRiaperturaDomanda(pensione.Id)) && !Utility.IsDomandaSpacchettamentoSOCOM(pensione, Utility.IsRiaperturaDomanda(pensione.Id)) && !Utility.IsDomandaSpacchettamentoSR(pensione, Utility.IsRiaperturaDomanda(pensione.Id)))
                    {
                        if (danteCausa != null)
                            this._IsDecorrenzaValida = BLCommon.Utility.ControllaDataDecorrenzaInferiore(pensione, Utility.IsDomandaReversibilitaOrRicostituzione(pensione, danteCausa, datiLavorazione), danteCausa.DecorrenzaPensione, out dataValidita);
                    }
                }
                else
                {
                    if ((this._Tipo == TipoDomanda.Normale) || (this._Tipo == TipoDomanda.Ricostituzione && tipoAppartenenza.HasValue && tipoAppartenenza.Value != Utility.TipoAppartenenza.AGO))
                        this._IsDecorrenzaValida = BLCommon.Utility.ControllaDataDecorrenzaInferiore(pensione, false, this.DecorrenzaOriginaria, out dataValidita);
                }

                if (this._Tipo == TipoDomanda.Ricostituzione || Utility.IsRiaperturaDomanda(pensione.Id))
                {
                    BLCommon.GestioneDatiStoricoGP.DatiStoricoGP datiStoricoGP = null;
                    BLCommon.GestioneDatiStoricoGP.GetDatiStoricoGPByIdPensione(pensione.Id, out datiStoricoGP);

                    if (datiStoricoGP != null)
                    {
                        this._DataPerfezionamentoRequisitiStoricoGP = datiStoricoGP.DataPerfezionamentoRequisiti;
                        if ((string.IsNullOrEmpty(datiStoricoGP.TipoSettimaneBeneficio) ||
                           (datiStoricoGP.TipoSettimaneBeneficio != "12" && datiStoricoGP.TipoSettimaneBeneficio != "15")) &&
                            datiStoricoGP.NumeroFigli.GetValueOrDefault() == 0)
                            this._IsSceltaLavoratriciMadriEmpty = true;
                    }
                    this.NaturaPensione = pensione.NaturaPensione;
                    if (tipoAppartenenza == Utility.TipoAppartenenza.CI && this.NaturaPensione.Substring(1, 1).ToUpperInvariant() == "J")
                    {
                        List<BLCommon.GestioneBeneficiParticolari.DatiBeneficiParticolari> datiBeneficiParticolari = null;
                        BLCommon.GestioneBeneficiParticolari.GetBeneficiParticolariByIdPensione(pensione.Id, pensione, out datiBeneficiParticolari);

                        if (datiBeneficiParticolari != null && datiBeneficiParticolari.Count != 0)
                        {
                            if (!datiBeneficiParticolari.Where(x => x.CodiceBenefici == "12" && x.CodiceBenefici == "15").Any())
                            {
                                this._IsSceltaLavoratriciMadriEmpty = false;
                            }
                        }

                    }

                }



                switch (tipoAppartenenza)
                {
                    case Utility.TipoAppartenenza.AGO:
                        this._TipoAppartenenzaDomanda = TipoAppDomanda.AGO;
                        break;
                    case Utility.TipoAppartenenza.CI:
                        this._TipoAppartenenzaDomanda = TipoAppDomanda.CI;
                        break;
                    case Utility.TipoAppartenenza.FS:
                        this._TipoAppartenenzaDomanda = TipoAppDomanda.FS;
                        break;
                    case null:
                    default:
                        this._TipoAppartenenzaDomanda = null;
                        break;
                }

                this._CodiceSedeDestinazione = pensione.CodiceSedeDestinazione;
                this._CentroOperativoDestinazione = pensione.CentroOperativoDestinazione;
                this._TipoCalcolo = Utility.GetTipoCalcolo(pensione);

                this.IsDomandaCumuloAutomatica = pensione.IsCumuloAutomatica.GetValueOrDefault();
                this.IsDomandaTotAutomatica = pensione.IsTotAutomatica.GetValueOrDefault();
                this._IsDomandaAPEPrecociOrRicostituzione = Utility.IsDomandaAPEPrecoci(pensione);
                this._IsDomandaSperimentaleDonna_DL_4_2019OrRicostituzione = Utility.IsDomandaSperimentaleDonna_DL_4_2019OrRicostituzione(pensione);
                this._IsDomandaAnzianitaPerLeggeBilancio2019OrRicostituzione = Utility.IsDomandaAnzianitaPerLeggeBilancio2019OrRicostituzione(pensione);
                this._IsDomandaQuota100OrRicostituzione = Utility.IsDomandaQuota100(pensione);
                this._IsDomandaInabilitaAmiantoOrRicostituzione = Utility.IsDomandaInabilitaAmianto(pensione);
                this._IsDatiENPALSRecuperati = pensione.IsDatiENPALSRecuperati;
                this.LavoratorePubblico = pensione.LavoratorePubblico;
                this.NumeroFigli = pensione.NumeroFigli;
                this.SceltaLavoratriciMadri = pensione.SceltaLavMadri;
                this.DataOpzione = pensione.DataOpzione;
                this.DataRaggiungimentoOpzione = pensione.DataRaggiungimentoOpzione;
                this.TipoFelpe = pensione.TipoFelpe;
                this.NaturaPensione = pensione.NaturaPensione;
                this.IsRichiestaBonus = pensione.IsRichiestaBonus;
                this.IsDomandaVecchiaiaOrAnzianitaMaggiorazioneAmianto = Utility.IsDomandaVecchiaiaMaggiorazioneAmiantoLegge208_2015(pensione) || Utility.IsDomandaAnzianitaMaggiorazioneAmiantoLegge208_2015(pensione);
                this.IsDatiAggiuntiviFromJSON = pensione.IsDatiAggiuntiviFromJSON;
                this._IsDomandaQuota102OrRicostituzione = Utility.IsDomandaQuota102(pensione);
                this._DataCondizioniPerComputo = pensione.DataCondizioniPerComputo;
                this._IsPLInvalidita = pensione.IsPLInvalidita;
                this._IsRicRinnovata = pensione.IsRicRinnovata;
                this._IsRicExtracalcolo = pensione.IsRicExtracalcolo;
                this._CodeGestione = pensione.Gestione;
                this._CodeFondo = pensione.Fondo;
                this.TipoAutomazione = pensione.TipoAutomazione;
                this._IsDomandaAnticipataFlessibileOrRicostituzione = Utility.IsDomandaAnticipataFlessibile(pensione);
                this._IsDomandaOpzioneDonna_Legge197_2022_Art1_Comma292_LetteraA = Utility.IsOpzioneDonna_Legge197_2022_Art1_Comma292_LetteraA(pensione, true, true);
                this._IsDomandaOpzioneDonna_Legge197_2022_Art1_Comma292_LetteraB = Utility.IsOpzioneDonna_Legge197_2022_Art1_Comma292_LetteraB(pensione, true, true);
                this._IsRicostituzioneOpzioneDonna_Legge197_2022_Art1_Comma292_LetteraA = Utility.IsRicOpzioneDonna_Legge197_2022_Art1_Comma292_LetteraA(pensione);
                this._IsRicostituzioneOpzioneDonna_Legge197_2022_Art1_Comma292_LetteraB = Utility.IsRicOpzioneDonna_Legge197_2022_Art1_Comma292_LetteraB(pensione);
                this._CodiceSedeGP1ALZ6 = pensione.CodiceSedeGP1ALZ6.GetValueOrDefault().ToString();
                this._CodiceSede = pensione.CodiceSede.ToString();
                this._Caratterizzazione = pensione.Caratterizzazione == null ? null : pensione.Caratterizzazione.ToString();
                this._IdTipoPLPerRIC = pensione.IdTipoPLPerRIC;
                this._IsDomandaAnticipataFlessibileOpzioneContributivoOrRicostituzione = Utility.IsDomandaAnticipataFlessibileOpzioneContributivo(pensione);
                //ENG - Memo 166/2023
                this._IsDomandaVecchiaiaAOICalcoloContributivo = Utility.isDomandaVecchiaiaTrasformazioneAOICalcoloContributivo(pensione);
                //ENG - Memo 06_2024
                this._CodProPe = pensione.CodProPE;
                this._IsDomandaTipoContributivo = Utility.IsDomandaTipoContributivo(pensione, null, null);
                this._GP1AV91B = pensione.GP1AV91B;
                this.IsDomandaAnticipataFlessibileLeggeBilancio2024OrRicostituzione = Utility.IsDomandaAnticipataFlessibileLeggeBilancio2024(pensione);
                this.IsDomandaAnticipataFlessibileLeggeBilancio2024OpzioneContributivoOrRicostituzione = Utility.IsDomandaAnticipataFlessibileOpzioneContributivoLeggeBilancio2024(pensione);
                this.GP1AJ11 = pensione.GP1AJ11;
                this._IsDomandaVOAUTAnticipataFlessibileLeggeBilancio2024FiltroGSEOrRicostituzione = Utility.IsDomandaVOAUTAnticipataFlessibileLeggeBilancio2024FiltroGSE(pensione);
                this._IsDomandaVOAUTAnticipataTipoContributivoFiltroGSEOrRicostituzione = Utility.IsDomandaVOAUTAnticipataTipoContributivoFiltroGSE(pensione);
                this._IsDomandaVOAUTVecchiaiaTipoContributivoFiltroGSEOrRicostituzione = Utility.IsDomandaVOAUTVecchiaiaTipoContributivoFiltroGSE(pensione);
                this._GP1AJSP = pensione.GP1AJSP;
                //ENG - Implementazione Meta Processo
                this._CentroOperativoGP1ALZ6 = pensione.CentroOperativoGP1ALZ6;
                this._FineAssicurazione = pensione.FineAssicurazione;
            }

            #region private properties
            private System.Nullable<System.DateTime> _DecorrenzaOriginaria;

            private System.Nullable<System.DateTime> _DataPresentazioneDomanda;
            private long _NDomus;

            private byte? _ProgStorico;

            private string _Prodotto;

            private string _Tipologia;

            private string _Gestione;

            private string _Fondo;

            private string _Ente;

            private System.Nullable<bool> _FlagUnicarpe;

            private System.Nullable<bool> _IndConvInt;

            private TipoDomanda _Tipo;

            private TipoAppDomanda? _TipoAppartenenzaDomanda;

            private System.Nullable<bool> _IsDecorrenzaValida;

            private System.Nullable<System.DateTime> _DataPerfezionamentoRequisiti;

            private string _CodeProdotto;

            private System.Nullable<short> _CodiceSedeDestinazione;

            private System.Nullable<byte> _CentroOperativo;

            private System.Nullable<byte> _CentroOperativoDestinazione;

            private string _CodeGruppo;

            private string _CodeTipo;

            private string _CodiceTipoRichiesta;

            private DateTime? _DataPerfezionamentoRequisitiUnicarpe;

            private char? _TipoLetturaUnicarpe;

            private Utility.TipoCalcolo _TipoCalcolo;

            private string _Filtro;

            private DateTime? _DataPerfezionamentoRequisitiStoricoGP;

            private bool _IsDomandaAPEPrecociOrRicostituzione;

            private bool _IsDomandaSperimentaleDonna_DL_4_2019OrRicostituzione;

            private bool _IsDomandaAnzianitaPerLeggeBilancio2019OrRicostituzione;

            private bool _IsDomandaQuota100OrRicostituzione;

            private bool _IsDomandaInabilitaAmiantoOrRicostituzione;

            private bool? _IsDatiENPALSRecuperati;

            private bool? _IsPLUnicarpe;

            private bool? _IsRichiestaBonus;

            private bool _IsDomandaVecchiaiaOrAnzianitaMaggiorazioneAmianto;

            private bool _IsSceltaLavoratriciMadriEmpty;

            private System.Nullable<bool> _IsDatiAggiuntiviFromJSON;

            private bool _IsDomandaQuota102OrRicostituzione;

            private System.Nullable<System.DateTime> _DataCondizioniPerComputo;

            private bool? _IsPLInvalidita;

            private bool? _IsRicRinnovata;

            private bool? _IsRicExtracalcolo;

            private string _CodeGestione;

            private string _CodeFondo;

            private bool _IsDomandaAnticipataFlessibileOrRicostituzione;

            private bool _IsDomandaOpzioneDonna_Legge197_2022_Art1_Comma292_LetteraA;

            private bool _IsDomandaOpzioneDonna_Legge197_2022_Art1_Comma292_LetteraB;

            private bool _IsRicostituzioneOpzioneDonna_Legge197_2022_Art1_Comma292_LetteraA;

            private bool _IsRicostituzioneOpzioneDonna_Legge197_2022_Art1_Comma292_LetteraB;

            private string _CodiceSedeGP1ALZ6;

            private string _CodiceSede;

            private string _Caratterizzazione;

            private byte? _IdTipoPLPerRIC;

            private bool _IsDomandaAnticipataFlessibileOpzioneContributivoOrRicostituzione;

            //ENG - Memo 166/2023
            private bool _IsDomandaVecchiaiaAOICalcoloContributivo;

            //ENG - Memo 06_2024
            private int? _CodProPe;
            private bool _IsDomandaTipoContributivo;
            private string _GP1AV91B;

            private bool _IsDomandaAnticipataFlessibileLeggeBilancio2024OrRicostituzione;

            private bool _IsDomandaAnticipataFlessibileLeggeBilancio2024OpzioneContributivoOrRicostituzione;

            //ENG - Aggiornamento Memo INPGI
            private string _GP1AJ11;

            //ENG - Memo 116/2025
            private bool _IsDomandaVOAUTAnticipataFlessibileLeggeBilancio2024FiltroGSEOrRicostituzione;
            private bool _IsDomandaVOAUTAnticipataTipoContributivoFiltroGSEOrRicostituzione;
            private bool _IsDomandaVOAUTVecchiaiaTipoContributivoFiltroGSEOrRicostituzione;
            //
            //ENG - RIC/TRF Spacchettate AGO
            private char? _GP1AJSP;

            //ENG - Implementazione Meta Processo
            private byte? _CentroOperativoGP1ALZ6;

            private System.Nullable<System.DateTime> _FineAssicurazione;

            #endregion private properties

            #region public data member
            [DataMember]
            public System.Nullable<System.DateTime> DecorrenzaOriginaria { get { return _DecorrenzaOriginaria; } set { _DecorrenzaOriginaria = value; } }
            [DataMember]
            public System.Nullable<System.DateTime> DataPresentazioneDomanda { get { return _DataPresentazioneDomanda; } set { _DataPresentazioneDomanda = value; } }
            [DataMember]
            public long NDomus { get { return _NDomus; } set { _NDomus = value; } }

            [DataMember]
            public byte? ProgStorico { get { return _ProgStorico; } set { _ProgStorico = value; } }

            [DataMember]
            public string Prodotto { get { return _Prodotto; } set { _Prodotto = value; } }

            [DataMember]
            public string Tipologia { get { return _Tipologia; } set { _Tipologia = value; } }

            [DataMember]
            public string Gestione { get { return _Gestione; } set { _Gestione = value; } }

            [DataMember]
            public string Fondo { get { return _Fondo; } set { _Fondo = value; } }

            [DataMember]
            public string Ente { get { return _Ente; } set { _Ente = value; } }

            [DataMember]
            public System.Nullable<bool> FlagUnicarpe { get { return _FlagUnicarpe; } set { _FlagUnicarpe = value; } }

            [DataMember]
            public System.Nullable<bool> IndConvInt { get { return _IndConvInt; } set { _IndConvInt = value; } }

            [DataMember]
            public TipoDomanda Tipo { get { return _Tipo; } set { _Tipo = value; } }

            [DataMember]
            public TipoAppDomanda? TipoAppartenenzaDomanda { get { return _TipoAppartenenzaDomanda; } set { _TipoAppartenenzaDomanda = value; } }

            [DataMember]
            public System.Nullable<bool> IsDecorrenzaValida { get { return _IsDecorrenzaValida; } set { _IsDecorrenzaValida = value; } }

            [DataMember]
            public System.Nullable<System.DateTime> DataPerfezionamentoRequisiti { get { return _DataPerfezionamentoRequisiti; } set { _DataPerfezionamentoRequisiti = value; } }

            [DataMember]
            public string CodeProdotto { get { return _CodeProdotto; } set { _CodeProdotto = value; } }

            [DataMember]
            public System.Nullable<short> CodiceSedeDestinazione { get { return _CodiceSedeDestinazione; } set { _CodiceSedeDestinazione = value; } }

            [DataMember]
            public System.Nullable<byte> CentroOperativo { get { return _CentroOperativo; } set { _CentroOperativo = value; } }

            [DataMember]
            public System.Nullable<byte> CentroOperativoDestinazione { get { return _CentroOperativoDestinazione; } set { _CentroOperativoDestinazione = value; } }

            [DataMember]
            public string CodeGruppo { get { return _CodeGruppo; } set { _CodeGruppo = value; } }

            [DataMember]
            public string CodeTipo { get { return _CodeTipo; } set { _CodeTipo = value; } }

            [DataMember]
            public string CodiceTipoRichiesta { get { return _CodiceTipoRichiesta; } set { _CodiceTipoRichiesta = value; } }

            [DataMember]
            public DateTime? DataPerfezionamentoRequisitiUnicarpe { get { return _DataPerfezionamentoRequisitiUnicarpe; } set { _DataPerfezionamentoRequisitiUnicarpe = value; } }

            [DataMember]
            public char? TipoLetturaUnicarpe { get { return _TipoLetturaUnicarpe; } set { _TipoLetturaUnicarpe = value; } }

            [DataMember]
            public Utility.TipoCalcolo TipoCalcolo { get { return _TipoCalcolo; } set { _TipoCalcolo = value; } }

            [DataMember]
            public string Filtro { get { return _Filtro; } set { _Filtro = value; } }

            [DataMember]
            public DateTime? DataPerfezionamentoRequisitiStoricoGP { get { return _DataPerfezionamentoRequisitiStoricoGP; } set { _DataPerfezionamentoRequisitiStoricoGP = value; } }

            [DataMember]
            public bool IsDomandaCumuloAutomatica { get; set; }

            [DataMember]
            public bool IsDomandaTotAutomatica { get; set; }

            [DataMember]
            public bool IsDomandaAPEPrecociOrRicostituzione { get { return _IsDomandaAPEPrecociOrRicostituzione; } set { _IsDomandaAPEPrecociOrRicostituzione = value; } }

            [DataMember]
            public bool IsDomandaSperimentaleDonna_DL_4_2019OrRicostituzione { get { return _IsDomandaSperimentaleDonna_DL_4_2019OrRicostituzione; } set { _IsDomandaSperimentaleDonna_DL_4_2019OrRicostituzione = value; } }

            [DataMember]
            public bool IsDomandaAnzianitaPerLeggeBilancio2019OrRicostituzione { get { return _IsDomandaAnzianitaPerLeggeBilancio2019OrRicostituzione; } set { _IsDomandaAnzianitaPerLeggeBilancio2019OrRicostituzione = value; } }

            [DataMember]
            public bool IsDomandaQuota100OrRicostituzione { get { return _IsDomandaQuota100OrRicostituzione; } set { _IsDomandaQuota100OrRicostituzione = value; } }

            [DataMember]
            public bool IsDomandaInabilitaAmiantoOrRicostituzione { get { return _IsDomandaInabilitaAmiantoOrRicostituzione; } set { _IsDomandaInabilitaAmiantoOrRicostituzione = value; } }

            [DataMember]
            public bool? IsDatiENPALSRecuperati { get { return _IsDatiENPALSRecuperati; } set { _IsDatiENPALSRecuperati = value; } }

            [DataMember]
            public bool? LavoratorePubblico { get; set; }

            [DataMember]
            public byte? NumeroFigli { get; set; }

            [DataMember]
            public byte? SceltaLavoratriciMadri { get; set; }

            [DataMember]
            public System.Nullable<System.DateTime> DataOpzione { get; set; }

            [DataMember]
            public System.Nullable<System.DateTime> DataRaggiungimentoOpzione { get; set; }

            [DataMember]
            public byte? TipoFelpe { get; set; }

            [DataMember]
            public string NaturaPensione { get; set; }

            [DataMember]
            public bool? IsPLUnicarpe { get { return _IsPLUnicarpe; } set { _IsPLUnicarpe = value; } }

            [DataMember]
            public bool? IsRichiestaBonus { get { return _IsRichiestaBonus; } set { _IsRichiestaBonus = value; } }

            [DataMember]
            public bool IsDomandaVecchiaiaOrAnzianitaMaggiorazioneAmianto { get { return _IsDomandaVecchiaiaOrAnzianitaMaggiorazioneAmianto; } set { _IsDomandaVecchiaiaOrAnzianitaMaggiorazioneAmianto = value; } }

            [DataMember]
            public bool IsSceltaLavoratriciMadriEmpty { get { return _IsSceltaLavoratriciMadriEmpty; } set { _IsSceltaLavoratriciMadriEmpty = value; } }

            [DataMember]
            public System.Nullable<bool> IsDatiAggiuntiviFromJSON { get { return _IsDatiAggiuntiviFromJSON; } set { _IsDatiAggiuntiviFromJSON = value; } }

            [DataMember]
            public bool IsDomandaQuota102OrRicostituzione { get { return _IsDomandaQuota102OrRicostituzione; } set { _IsDomandaQuota102OrRicostituzione = value; } }

            [DataMember]
            public System.Nullable<System.DateTime> DataCondizioniPerComputo { get { return _DataCondizioniPerComputo; } set { _DataCondizioniPerComputo = value; } }

            [DataMember]
            public System.Nullable<int> NCertificato { get; set; }

            [DataMember]
            public bool? IsPLInvalidita { get { return _IsPLInvalidita; } set { _IsPLInvalidita = value; } }

            [DataMember]
            public bool? IsRicRinnovata { get { return _IsRicRinnovata; } set { _IsRicRinnovata = value; } }

            [DataMember]
            public bool? IsRicExtracalcolo { get { return _IsRicExtracalcolo; } set { _IsRicExtracalcolo = value; } }

            [DataMember]
            public string CodeGestione { get { return _CodeGestione; } set { _CodeGestione = value; } }

            [DataMember]
            public string CodeFondo { get { return _CodeFondo; } set { _CodeFondo = value; } }

            [DataMember]
            public byte? TipoAutomazione { get; set; }

            [DataMember]
            public bool IsDomandaAnticipataFlessibileOrRicostituzione { get { return _IsDomandaAnticipataFlessibileOrRicostituzione; } set { _IsDomandaAnticipataFlessibileOrRicostituzione = value; } }

            [DataMember]
            public bool IsDomandaOpzioneDonna_Legge197_2022_Art1_Comma292_LetteraA { get { return _IsDomandaOpzioneDonna_Legge197_2022_Art1_Comma292_LetteraA; } set { _IsDomandaOpzioneDonna_Legge197_2022_Art1_Comma292_LetteraA = value; } }

            [DataMember]
            public bool IsDomandaOpzioneDonna_Legge197_2022_Art1_Comma292_LetteraB { get { return _IsDomandaOpzioneDonna_Legge197_2022_Art1_Comma292_LetteraB; } set { _IsDomandaOpzioneDonna_Legge197_2022_Art1_Comma292_LetteraB = value; } }

            [DataMember]
            public bool IsRicostituzioneOpzioneDonna_Legge197_2022_Art1_Comma292_LetteraA { get { return _IsRicostituzioneOpzioneDonna_Legge197_2022_Art1_Comma292_LetteraA; } set { _IsRicostituzioneOpzioneDonna_Legge197_2022_Art1_Comma292_LetteraA = value; } }

            [DataMember]
            public bool IsRicostituzioneOpzioneDonna_Legge197_2022_Art1_Comma292_LetteraB { get { return _IsRicostituzioneOpzioneDonna_Legge197_2022_Art1_Comma292_LetteraB; } set { _IsRicostituzioneOpzioneDonna_Legge197_2022_Art1_Comma292_LetteraB = value; } }

            [DataMember]
            public string CodiceSedeGP1ALZ6 { get { return _CodiceSedeGP1ALZ6; } set { _CodiceSedeGP1ALZ6 = value; } }

            [DataMember]
            public string CodiceSede { get { return _CodiceSede; } set { _CodiceSede = value; } }

            [DataMember]
            public string Caratterizzazione { get { return _Caratterizzazione; } set { _Caratterizzazione = value; } }

            [DataMember]
            public byte? IdTipoPLPerRIC { get { return _IdTipoPLPerRIC; } set { _IdTipoPLPerRIC = value; } }

            [DataMember]
            private bool IsDomandaAnticipataFlessibileOpzioneContributivoOrRicostituzione { get { return _IsDomandaAnticipataFlessibileOpzioneContributivoOrRicostituzione; } set { _IsDomandaAnticipataFlessibileOpzioneContributivoOrRicostituzione = value; } }

            //ENG - Memo 166/2023
            [DataMember]
            private bool IsDomandaVecchiaiaAOICalcoloContributivo { get { return _IsDomandaVecchiaiaAOICalcoloContributivo; } set { _IsDomandaVecchiaiaAOICalcoloContributivo = value; } }
            [DataMember]
            public bool IsDomandaTipoContributivo { get { return _IsDomandaTipoContributivo; } set { _IsDomandaTipoContributivo = value; } }

            //ENG - Memo 06_2024
            [DataMember]
            public int? CodProPe { get { return _CodProPe; } set { _CodProPe = value; } }

            [DataMember]
            public string DirittoAutonomo { get; set; }

            [DataMember]
            public string GP1AV91B { get { return _GP1AV91B; } set { _GP1AV91B = value; } }

            [DataMember]
            public bool IsDomandaAnticipataFlessibileLeggeBilancio2024OrRicostituzione { get { return _IsDomandaAnticipataFlessibileLeggeBilancio2024OrRicostituzione; } set { _IsDomandaAnticipataFlessibileLeggeBilancio2024OrRicostituzione = value; } }

            [DataMember]
            public bool IsDomandaAnticipataFlessibileLeggeBilancio2024OpzioneContributivoOrRicostituzione { get { return _IsDomandaAnticipataFlessibileLeggeBilancio2024OpzioneContributivoOrRicostituzione; } set { _IsDomandaAnticipataFlessibileLeggeBilancio2024OpzioneContributivoOrRicostituzione = value; } }

            [DataMember]
            public byte? CodiceSpecifico { get; set; }

            [DataMember]
            public string GP1AJ11 { get { return _GP1AJ11; } set { _GP1AJ11 = value; } }

            [DataMember]
            public bool IsDomandaVOAUTAnticipataFlessibileLeggeBilancio2024FiltroGSEOrRicostituzione { get { return _IsDomandaVOAUTAnticipataFlessibileLeggeBilancio2024FiltroGSEOrRicostituzione; } set { _IsDomandaVOAUTAnticipataFlessibileLeggeBilancio2024FiltroGSEOrRicostituzione = value; } }

            [DataMember]
            public bool IsDomandaVOAUTAnticipataTipoContributivoFiltroGSEOrRicostituzione { get { return _IsDomandaVOAUTAnticipataTipoContributivoFiltroGSEOrRicostituzione; } set { _IsDomandaVOAUTAnticipataTipoContributivoFiltroGSEOrRicostituzione = value; } }

            [DataMember]
            public bool IsDomandaVOAUTVecchiaiaTipoContributivoFiltroGSEOrRicostituzione { get { return _IsDomandaVOAUTVecchiaiaTipoContributivoFiltroGSEOrRicostituzione; } set { _IsDomandaVOAUTVecchiaiaTipoContributivoFiltroGSEOrRicostituzione = value; } }

            [DataMember]
            public char? GP1AJSP { get { return _GP1AJSP; } set { _GP1AJSP = value; } }

            //ENG - Implementazione Meta Processo
            [DataMember]
            public byte? CentroOperativoGP1ALZ6 { get { return _CentroOperativoGP1ALZ6; } set { _CentroOperativoGP1ALZ6 = value; } }

            [DataMember]
            public System.Nullable<System.DateTime> FineAssicurazione { get { return _FineAssicurazione; } set { _FineAssicurazione = value; } }

            #endregion public data member

            public enum TipoDomanda
            {
                Normale,
                Ricostituzione,
                Superstiti,
                Ripristino,
                RipristinoSuperstiti,
                Riliquidazione,
                RiliquidazioneSuperstiti
            };

            public enum TipoAppDomanda
            {
                AGO,
                FS,
                CI
            };
        }

        #endregion nested class
    }
}