using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using INPS.DNA;
using INPS.Pensioni.LiquidazionePensione.Presenter;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneAgo;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;

namespace INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DatiContributiviAgo
{
    public partial class UCDatiCalcoloAgo : CustomBaseUserControl, IDatiContributiviAgo, ITitolarePensione, IDanteCausa
    {
        #region IDatiContributiviAgo
        public Presenter.SvrLiquidazioneAgo.AreaDatiContributivi areaDatiContributiviAgo { get; set; }
        public Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda { get; set; }

        #endregion IDatiContributiviAgo

        #region IViewUI
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        #endregion IViewUI

        #region ITitolare
        public AreaTitolare TitolarePensione { get; set; }
        #endregion ITitolare

        #region IDanteCausa
        public AreaDanteCausa areaDanteCausa { get; set; }
        public AreaRispostaRiepilogo.DatiRiepilogoDomanda domandaDante { get; set; }
        public long numDomanda { get; set; }
        #endregion IDanteCausa

        #region Enum
        public enum ColonneGvDatiRetributivi { Sett707 = 5, QuoteRet = 6, QuoteRetr707 = 7 };
        public enum ColonneGvDatiContributivi { QuotaContr = 5 };
        #endregion Enum

        #region Public

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
            }
        }

        public void ValorizzaEtichetteDatiCalcoloAGO(IDatiContributiviAgo Dati)
        {
            if (this.domanda == null)
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            string valoreControllo = string.Empty;
            if (ViewState["AbilitazioneRilascioRIC_31082020"] != null)
                valoreControllo = (string)ViewState["AbilitazioneRilascioRIC_31082020"];
            else
            {
                Presenter.PresenterControlliDinamici presenter = new PresenterControlliDinamici();
                Presenter.SvrLiquidazione.AreaEsito esito = presenter.GetControlloDinamicoByNomeControllo("AbilitazioneRilascioRIC_31082020", out valoreControllo);
                if (esito != null && esito.RisultatoOperazione == Presenter.SvrLiquidazione.AreaEsito.TipoEsito.OK)
                    ViewState["AbilitazioneRilascioRIC_31082020"] = valoreControllo;
            }

            //ENG - Aggiornamento Memo 123_2021 
            string valoreControlloMemo123_2021 = string.Empty;
            if (ViewState["AbilitazioneMemo123_2021"] != null)
                valoreControlloMemo123_2021 = (string)ViewState["AbilitazioneMemo123_2021"];
            else
            {
                Presenter.PresenterControlliDinamici presenter = new PresenterControlliDinamici();
                Presenter.SvrLiquidazione.AreaEsito esito = presenter.GetControlloDinamicoByNomeControllo("AbilitazioneMemo123_2021", out valoreControlloMemo123_2021);
                if (esito != null && esito.RisultatoOperazione == Presenter.SvrLiquidazione.AreaEsito.TipoEsito.OK
                    && !String.IsNullOrEmpty(valoreControlloMemo123_2021) && !String.IsNullOrEmpty(valoreControlloMemo123_2021.Trim()))
                    ViewState["AbilitazioneMemo123_2021"] = valoreControlloMemo123_2021.Trim();
            }


            ViewState["areaDatiContributiviAgo"] = Dati.areaDatiContributiviAgo;
            if (Dati.areaDatiContributiviAgo != null)
                ViewState["IsBeneficioVittimeTerrorismo"] = Dati.areaDatiContributiviAgo.IsBeneficioVittimeTerrorismo;


            //ENG - MEMO 50/2023 
            string valoreControlloMemo50_2023 = string.Empty;
            if (ViewState["AbilitazioneMemo50_2023"] != null)
                valoreControlloMemo50_2023 = (string)ViewState["AbilitazioneMemo50_2023"];
            else
            {
                Presenter.PresenterControlliDinamici presenter = new PresenterControlliDinamici();
                Presenter.SvrLiquidazione.AreaEsito esito = presenter.GetControlloDinamicoByNomeControllo("AbilitazioneMemo50_2023", out valoreControlloMemo50_2023);
                if (esito != null && esito.RisultatoOperazione == Presenter.SvrLiquidazione.AreaEsito.TipoEsito.OK
                    && !String.IsNullOrEmpty(valoreControlloMemo50_2023) && !String.IsNullOrEmpty(valoreControlloMemo50_2023.Trim()))
                    ViewState["AbilitazioneMemo50_2023"] = valoreControlloMemo50_2023.Trim();
            }

            //ENG - ComputoNoEditabileMemo90_2016 
            string valoreControlloComputoNoEditabileMemo90_2016 = string.Empty;
            if (ViewState["ComputoNoEditabileMemo90_2016"] != null)
                valoreControlloComputoNoEditabileMemo90_2016 = (string)ViewState["ComputoNoEditabileMemo90_2016"];
            else
            {
                Presenter.PresenterControlliDinamici presenter = new PresenterControlliDinamici();
                Presenter.SvrLiquidazione.AreaEsito esito = presenter.GetControlloDinamicoByNomeControllo("ComputoNoEditabileMemo90_2016", out valoreControlloComputoNoEditabileMemo90_2016);
                if (esito != null && esito.RisultatoOperazione == Presenter.SvrLiquidazione.AreaEsito.TipoEsito.OK
                    && !String.IsNullOrEmpty(valoreControlloComputoNoEditabileMemo90_2016) && !String.IsNullOrEmpty(valoreControlloComputoNoEditabileMemo90_2016.Trim()))
                    ViewState["ComputoNoEditabileMemo90_2016"] = valoreControlloComputoNoEditabileMemo90_2016.Trim();
            }

            //ENG - Eliminazione Scarto Oneri
            string valoreControlloEliminazioneScartoOneri0031_0105_0112 = string.Empty;
            if (ViewState["EliminazioneScartoOneri0031_0105_0112"] != null)
                valoreControlloEliminazioneScartoOneri0031_0105_0112 = (string)ViewState["EliminazioneScartoOneri0031_0105_0112"];
            else
            {
                Presenter.PresenterControlliDinamici presenter = new PresenterControlliDinamici();
                Presenter.SvrLiquidazione.AreaEsito esito = presenter.GetControlloDinamicoByNomeControllo("EliminazioneScartoOneri0031_0105_0112", out valoreControlloEliminazioneScartoOneri0031_0105_0112);
                if (esito != null && esito.RisultatoOperazione == Presenter.SvrLiquidazione.AreaEsito.TipoEsito.OK
                    && !String.IsNullOrEmpty(valoreControlloEliminazioneScartoOneri0031_0105_0112) && !String.IsNullOrEmpty(valoreControlloEliminazioneScartoOneri0031_0105_0112.Trim()))
                    ViewState["EliminazioneScartoOneri0031_0105_0112"] = valoreControlloEliminazioneScartoOneri0031_0105_0112.Trim();
            }

            AreaTitolare.DatiPensione datiPensione = new AreaTitolare.DatiPensione();
            datiPensione = GetDatiPensione(this);

            //ENG - Spacchettate SOPGI
            if (this.areaDanteCausa == null)
            {
                PresenterDanteCausa presenterDanteCausa = new PresenterDanteCausa();
                presenterDanteCausa.GetDatiDanteCausa(this);
            }

            if (Dati.areaDatiContributiviAgo != null)
            {
                ViewState[EnumViewState.IsPrimoRecordRetrGestioneS.ToString()] = Dati.areaDatiContributiviAgo.DatiCalcolo.IsPrimoRecordRetrGestioneS;
                if (Utility.IsDomandaAUT(this.domanda.Categoria))
                {
                    if (Dati.areaDatiContributiviAgo.DatiCalcolo != null)
                        ddlFacoltaComputo.SelectedValue = Dati.areaDatiContributiviAgo.DatiCalcolo.FacoltaComputo == true ? "SI" : "NO";
                }
                if (Dati.areaDatiContributiviAgo.IsPnlImportoLordoAllaDecVisible)
                {
                    GestioneContribDatiCalcolo datiCalcolo = Dati.areaDatiContributiviAgo.DatiCalcolo;
                    if (datiCalcolo != null)
                    {
                        txtImportoLordoAllaDecorrenza.Text = datiCalcolo.ImportoLordoAllaDecorrenza.ToString();

                        Utility.TipoUnicarpe tipoUnicarpe = Utility.IsDomandaUnicarpe(datiPensione, true);
                        string categoria = this.domanda.Categoria;

                        bool isAutomatica = tipoUnicarpe == Utility.TipoUnicarpe.Automatica;
                        bool disableField = isAutomatica && (
                            Utility.IsDomandaVESO92(categoria) ||
                            Utility.IsDomandaESOTEL(categoria) ||
                            Utility.IsDomandaESPA(categoria)
                        );

                        txtImportoLordoAllaDecorrenza.Enabled = !disableField;
                    }

                    if (!string.IsNullOrEmpty(Dati.areaDatiContributiviAgo.GestioneImportoLordoAllaDec))
                        lblGestioneImportoLordoAllaDec.Text = Dati.areaDatiContributiviAgo.GestioneImportoLordoAllaDec;

                    if (Utility.IsDomandaVESO33_DAP(this.domanda.Categoria, datiPensione.Filtro) && CodeUtility.IsRicostituzione(datiPensione))
                        txtImportoLordoAllaDecorrenza.Enabled = true;
                    if (CodeUtility.IsRicostituzione(datiPensione) && Dati.areaDatiContributiviAgo.IsEliminataPerCauseVarie && Dati.areaDatiContributiviAgo.IsMemo102Abilitato.GetValueOrDefault() && (Utility.IsDomandaVOESO(this.domanda.Categoria) || Utility.IsDomandaVESO33(this.domanda.Categoria) || Utility.IsDomandaVESO92(this.domanda.Categoria) || Utility.IsDomandaVOCOOP_COOP28(this.domanda.Categoria) ||
                        Utility.IsDomandaVOCRED_CRED27(this.domanda.Categoria) || Utility.IsDomandaESPA(this.domanda.Categoria) || Utility.IsDomandaESOTEL(this.domanda.Categoria) || Utility.IsDomandaESOAMB(this.domanda.Categoria)))
                        txtImportoLordoAllaDecorrenza.Enabled = false;
                }
                if (Utility.IsDomandaAPESociale(this.domanda.Categoria))
                {
                    if (Dati.areaDatiContributiviAgo.DatiCalcolo != null)
                    {
                        txtImportoLordo.Text = Dati.areaDatiContributiviAgo.DatiCalcolo.ImportoLordo.ToString();
                    }
                }
                if (Utility.IsDomandaRenditaCasalinghe(this.domanda.Categoria) || Utility.IsDomandaRenditaFacoltativa(this.domanda.Categoria))
                {
                    if (Dati.areaDatiContributiviAgo.DatiCalcolo != null)
                    {
                        txtImportoMensileAllaDecorrenzaOriginaria.Text = Dati.areaDatiContributiviAgo.DatiCalcolo.ImportoMensileAllaDecorrenzaOriginaria.ToString();
                        txtImportoMensileAlGennaio2001.Text = Dati.areaDatiContributiviAgo.DatiCalcolo.ImportoMensileAlGennaio2001.ToString();
                    }
                }

                //ENG - Aggiornamento Memo 68/2022 IOPGI
                //ENG - Spacchettate SOPGI
                if (!string.IsNullOrEmpty(valoreControllo) && valoreControllo != "NO" && CodeUtility.IsRicostituzione(datiPensione) && Dati.areaDatiContributiviAgo.DatiCalcolo.TipoCalcolo == GestioneContribTipoCalcolo.Retributivo &&
               Dati.areaDatiContributiviAgo.IsFineAssicurazionePost2012 && !Utility.IsDomandaVOPGI(domanda.Categoria) && !(Utility.IsDomandaIOPGI(domanda.Categoria) && !Utility.IsDomandaIOPGI_AGI(domanda.Categoria, datiPensione.Filtro))
                    && !Utility.IsDomandaSpacchettamentoSOPGIPost072022(this.domanda.Categoria, datiPensione, this.areaDanteCausa))
                {
                    lblQuotaD.Visible = true;
                }
            }

            CodeUtility.TipologiaPensioneGruppo tipologiaGruppoPensione = CodeUtility.TipologiaPensioneGruppo.gr_NessunValore;
            CodeUtility.TipologiaPensioneProdotto tipologiaProdottoPensione = CodeUtility.TipologiaPensioneProdotto.pr_NessunValore;
            CodeUtility.TipologiaPensioneTipo tipologiaTipoPensione = CodeUtility.TipologiaPensioneTipo.tp_NessunValore;
            CodeUtility.GetTipologiaPensione(datiPensione.CodeGruppo, datiPensione.CodeProdotto, datiPensione.CodeTipo, out tipologiaGruppoPensione, out tipologiaProdottoPensione, out tipologiaTipoPensione);

            List<string> lstCodiciAmmessi;
            if (Utility.IsDomandaVOAUT(this.domanda.Categoria) && !CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura) && !Utility.IsDomandaRipristino(datiPensione)
                && !(Utility.IsDomandaAUT(this.domanda.Categoria) && !this.domanda.IsDomandaRiapertura && (Utility.IsDomandaRiliquidazioneVecchiaiaAnticipate(datiPensione) || Utility.IsDomandaRiliquidazioneIndiretta(datiPensione) || Utility.IsDomandaRliquidazioneAssegnoInvalidita(datiPensione))))
            {
                ddlFacoltaComputo.Enabled = false;

                if (datiPensione.IsDomandaQuota100OrRicostituzione ||
                    datiPensione.IsDomandaQuota102OrRicostituzione ||
                    datiPensione.IsDomandaAnticipataFlessibileOrRicostituzione ||
                    datiPensione.IsDomandaAnticipataFlessibileOpzioneContributivoOrRicostituzione ||
                    tipologiaTipoPensione == CodeUtility.TipologiaPensioneTipo.tp_Anzianita_InComputo ||
                    tipologiaTipoPensione == CodeUtility.TipologiaPensioneTipo.tp_Vecchiaia_InComputo ||
                    (datiPensione.IsDomandaAnticipataFlessibileLeggeBilancio2024OrRicostituzione && !datiPensione.IsDomandaVOAUTAnticipataFlessibileLeggeBilancio2024FiltroGSEOrRicostituzione) ||
                    datiPensione.IsDomandaAnticipataFlessibileLeggeBilancio2024OpzioneContributivoOrRicostituzione)
                {
                    ddlFacoltaComputo.SelectedValue = "SI";
                }
                else if (tipologiaTipoPensione == CodeUtility.TipologiaPensioneTipo.tp_Vecchiaia_TrasfAOI)
                {
                    if (Dati.areaDatiContributiviAgo.DatiCalcolo != null && Dati.areaDatiContributiviAgo.DatiCalcolo.CodiceP18PrecedentePensione == 74)
                        ddlFacoltaComputo.SelectedValue = Dati.areaDatiContributiviAgo != null && Dati.areaDatiContributiviAgo.DatiCalcolo != null
                        && Dati.areaDatiContributiviAgo.DatiCalcolo.FacoltaComputo.GetValueOrDefault() ? "SI" : "NO";
                    else
                    {
                        if (Dati.areaDatiContributiviAgo.DatiCalcolo != null && Dati.areaDatiContributiviAgo.DatiCalcolo.FacoltaComputo != null)
                            ddlFacoltaComputo.SelectedValue = Dati.areaDatiContributiviAgo.DatiCalcolo.FacoltaComputo == true ? "SI" : "NO";
                        else
                            ddlFacoltaComputo.SelectedValue = "NO";
                        ddlFacoltaComputo.Enabled = true;
                    }
                }
                else if (Utility.IsDomandaSupplementare(datiPensione))
                {
                    if (Dati.areaDatiContributiviAgo.DatiCalcolo != null && Dati.areaDatiContributiviAgo.DatiCalcolo.FacoltaComputo != null)
                        ddlFacoltaComputo.SelectedValue = Dati.areaDatiContributiviAgo.DatiCalcolo.FacoltaComputo == true ? "SI" : "NO";
                    else
                        ddlFacoltaComputo.SelectedValue = "NO";
                    ddlFacoltaComputo.Enabled = true;
                }
                else
                {
                    ddlFacoltaComputo.SelectedValue = "NO";
                }

                //ENG - Aggiornamento Memo 123_2021
                if (tipologiaTipoPensione != CodeUtility.TipologiaPensioneTipo.tp_Vecchiaia_TrasfAOI && !Utility.IsDomandaSupplementare(datiPensione))
                {
                    //ENG - Aggiornamento Memo 123_2021
                    if (ddlFacoltaComputo.SelectedValue == "SI")
                    {
                        lstCodiciAmmessi = new List<string> { "G", "C1", "C2", "C3", "C4", "C5", "D1", "E1", "E2", "A5", "A6", "A7", "A8", "A9", "B1", "B2", "B3", "B4", "1", "2", "3", "4" };
                        if (ViewState["AbilitazioneMemo123_2021"] != null && (string)ViewState["AbilitazioneMemo123_2021"] == "SI")
                            lstCodiciAmmessi.Add("F0");
                    }
                    else
                        lstCodiciAmmessi = new List<string> { "G" };
                    ViewState["lstCodiciAmmessi"] = lstCodiciAmmessi;
                }
            }
            else if (CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura) && (datiPensione.IsDomandaVOAUTAnticipataTipoContributivoFiltroGSEOrRicostituzione ||
                datiPensione.IsDomandaVOAUTVecchiaiaTipoContributivoFiltroGSEOrRicostituzione || datiPensione.IsDomandaVOAUTAnticipataFlessibileLeggeBilancio2024FiltroGSEOrRicostituzione))
            {
                lstCodiciAmmessi = new List<string> { "G" };
                ViewState["lstCodiciAmmessi"] = lstCodiciAmmessi;
            }

            //AUT TFR/RIC + Ripristini blindato con quanto ricevuto da prelievo (GetDatiTGP1ByChiavePensione_AOI_AUT servizio common)
            if (Utility.IsDomandaAUT(this.domanda.Categoria) && (CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, domanda.IsDomandaRiapertura) || Utility.IsDomandaRipristino(datiPensione)))
            {
                if (Dati.areaDatiContributiviAgo.DatiCalcolo != null && Dati.areaDatiContributiviAgo.DatiCalcolo.FacoltaComputo != null)
                {
                    ddlFacoltaComputo.Enabled = false;
                    ddlFacoltaComputo.SelectedValue = Dati.areaDatiContributiviAgo.DatiCalcolo.FacoltaComputo == true ? "SI" : "NO";
                }
            }

            LoadDecodificaData(Dati);
            BindDataForPanels(Dati.areaDatiContributiviAgo);

            if (Utility.IsDomandaAUT(this.domanda.Categoria) && !this.domanda.IsDomandaRiapertura && (Utility.IsDomandaRiliquidazioneVecchiaiaAnticipate(datiPensione) || Utility.IsDomandaRiliquidazioneIndiretta(datiPensione) || Utility.IsDomandaRliquidazioneAssegnoInvalidita(datiPensione)))
            {
                ddlFacoltaComputo.Enabled = true;
            }

            if (Dati.areaDatiContributiviAgo != null && ((CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura) &&
                Dati.areaDatiContributiviAgo.IsBeneficioVittimeTerrorismo.GetValueOrDefault() && !(ViewState["EliminazioneScartoOneri0031_0105_0112"] != null && (string)ViewState["EliminazioneScartoOneri0031_0105_0112"] == "SI" && Utility.IsDomandaBeneficioTerrorismoLegge206_2004(datiPensione))) || (Utility.IsDomandaRipristino(datiPensione) && !this.domanda.IsDomandaRiapertura)))
            {
                gvDatiRetributivi.Enabled = false;
                gvDatiContributivi.Enabled = false;
                pnlDomandeAUT.Enabled = false;
                pnlImportoLordoDecorrenza.Enabled = false;
                pnlDatiCalcoloAPESociale.Enabled = false;
                btnEliminaDatiCalcolo.Enabled = false;
            }

            if (CodeUtility.IsRicostituzione(datiPensione) && !CodeUtility.IsRicostituzioneContributiva(datiPensione) && !Utility.IsRicostituzione_AccreditoPeriodiMaternita(datiPensione)
                && !(ViewState["EliminazioneScartoOneri0031_0105_0112"] != null && (string)ViewState["EliminazioneScartoOneri0031_0105_0112"] == "SI" && Utility.IsDomandaBeneficioTerrorismoLegge206_2004(datiPensione)))
            {
                if (Utility.IsDomandaFPLD(this.domanda.Categoria) || Utility.IsDomandaGestioneAutonomi(this.domanda.Categoria) || Utility.IsDomandaDAI(this.domanda.Categoria) ||
                    Utility.IsDomandaAUT(this.domanda.Categoria))
                {
                    pnlDomandeAUT.Enabled = false;
                    pnlImportoLordoDecorrenza.Enabled = false;
                    pnlDatiCalcoloAPESociale.Enabled = false;
                    btnEliminaDatiCalcolo.Enabled = false;
                    lblRicNonContrib.Visible = true;
                }
            }
            //if (areaDatiContributiviAgo != null && areaDatiContributiviAgo.IsAnte96 != null && Utility.IsRicostituzione_MotiviContributivi(datiPensione) &&
            //   ((this.areaDanteCausa != null && Utility.IsDomandaPensioneReversibilitaOrRicostituzione(this.domanda.Categoria, datiPensione, this.areaDanteCausa) && this.areaDanteCausa.DatiPensioneDiretta != null && this.areaDanteCausa.DatiPensioneDiretta.DecorrenzaPensione.HasValue && !Utility.DataStrettamenteSuccessivaA(this.areaDanteCausa.DatiPensioneDiretta.DecorrenzaPensione.GetValueOrDefault(), new DateTime(01 / 01 / 1990)))
            //   || (!Utility.IsDomandaReversibilita(datiPensione) && !Utility.DataStrettamenteSuccessivaA(datiPensione.DecorrenzaOriginaria.GetValueOrDefault(), new DateTime(01 / 01 / 1990)))))
            if (areaDatiContributiviAgo != null && areaDatiContributiviAgo.IsAnte96 != null && Utility.IsRicostituzione_MotiviContributivi(datiPensione))
            {
                DateTime data = new DateTime();
                if (this.areaDanteCausa != null &&
                    Utility.IsDomandaPensioneReversibilitaOrRicostituzione(this.domanda.Categoria, datiPensione, this.areaDanteCausa) &&
                    this.areaDanteCausa.DatiPensioneDiretta != null &&
                    this.areaDanteCausa.DatiPensioneDiretta.DecorrenzaPensione.HasValue)

                    data = this.areaDanteCausa.DatiPensioneDiretta.DecorrenzaPensione.GetValueOrDefault();

                else data = datiPensione.DecorrenzaOriginaria.GetValueOrDefault();

                if (Utility.DataStrettamenteSuccessivaA(new DateTime(1990, 01, 01), data) && !Dati.areaDatiContributiviAgo.DatiCalcolo.SbloccaPannelliAnte96)
                {
                    gvDatiRetributivi.Enabled = false;
                    gvDatiContributivi.Enabled = false;
                }
            }


            //ENG - Aggiornamento Memo 68/2022 IOPGI
            //ENG - Spacchettate SOPGI
            if (Utility.IsDomandaVOPGI(this.domanda.Categoria) || (Utility.IsDomandaIOPGI(this.domanda.Categoria) && !Utility.IsDomandaIOPGI_AGI(this.domanda.Categoria, datiPensione.Filtro))
                || Utility.IsDomandaSpacchettamentoSOPGIPost072022(this.domanda.Categoria, datiPensione, this.areaDanteCausa))
            {
                pnlCoefficienteContributivo.Visible = true;
                txtCoefficienteContributivo.Text = Dati.areaDatiContributiviAgo.DatiCalcolo.PL_Coeftrasf != null ? Dati.areaDatiContributiviAgo.DatiCalcolo.PL_Coeftrasf.ToString() : string.Empty;
            }

            if (pnlImportoLordoDecorrenza.Visible && CodeUtility.IsRicostituzione(datiPensione) && Utility.IsDomandaESPA(this.domanda.Categoria) && !datiPensione.IsRicExtracalcolo.GetValueOrDefault())
            {
                pnlImportoLordoDecorrenza.Enabled = false;
            }

            //ENG - RIC VOPGI NON CONTRIBUTIVE
            if (Utility.IsRicostituzione(datiPensione) && !Utility.IsRicostituzione_MotiviContributivi(datiPensione) && Utility.IsDomandaVOPGI(this.domanda.Categoria))
            {
                txtCoefficienteContributivo.Enabled = false;
                lblRicNonContrib.Visible = true;
            }

            //ENG - Spacchettate SOPGI
            if (Utility.IsDomandaSpacchettamentoSOPGIPost072022(this.domanda.Categoria, datiPensione, this.areaDanteCausa))
            {
                if (Utility.IsDomandaReversibilita(datiPensione) || (Utility.IsDomandaIndiretta(datiPensione) && this.domanda.IsDomandaRiapertura && !this.areaDanteCausa.IsFascicoloGenerato.GetValueOrDefault()))
                {
                    txtCoefficienteContributivo.Enabled = false;
                }
            }

            if (Utility.IsDomandaINPGI(this.domanda.Categoria) && Utility.IsDomandaUnicarpe(datiPensione, true) == Utility.TipoUnicarpe.Automatica)
                txtCoefficienteContributivo.Enabled = false;

            //ENG - VOAUT 0001-0002-0192, IOAUT 0002-0011-0045 o 0002-0012-0045, SOAUT 0003-0022-0045 ddlFacoltaComputo disabilitato e con valore "SI"
            if (Utility.IsDomandaAUT(this.domanda.Categoria) && (
                (datiPensione.CodeGruppo == "0001" && datiPensione.CodeProdotto == "0002" && datiPensione.CodeTipo == "0192") ||
                (datiPensione.CodeGruppo == "0002" && datiPensione.CodeProdotto == "0011" && datiPensione.CodeTipo == "0045") ||
                (datiPensione.CodeGruppo == "0002" && datiPensione.CodeProdotto == "0012" && datiPensione.CodeTipo == "0045") ||
                (datiPensione.CodeGruppo == "0003" && datiPensione.CodeProdotto == "0022" && datiPensione.CodeTipo == "0045")))
            {
                ddlFacoltaComputo.SelectedValue = "SI";
                ddlFacoltaComputo.Enabled = false;
            }

            //ENG - Per le seguenti triplette la facoltà computo è non editabile e valorizzata con "NO"
            //ENG - Nuovo controllo dinamico "ComputoNoEditabileMemo90_2016"
            if (datiPensione.CodeGestione == "005" && datiPensione.CodeFondo == "001" &&
                ((datiPensione.CodeGruppo == "0001" && datiPensione.CodeProdotto == "0002" && datiPensione.CodeTipo == "0009") ||
                 (datiPensione.CodeGruppo == "0002" && datiPensione.CodeProdotto == "0011" && datiPensione.CodeTipo == "0001") ||
                 (datiPensione.CodeGruppo == "0002" && datiPensione.CodeProdotto == "0012" && datiPensione.CodeTipo == "0001") ||
                 (datiPensione.CodeGruppo == "0003" && datiPensione.CodeProdotto == "0022" && datiPensione.CodeTipo == "0001")))
            {
                if (ViewState["ComputoNoEditabileMemo90_2016"] != null && (string)ViewState["ComputoNoEditabileMemo90_2016"] == "SI")
                {
                    ddlFacoltaComputo.SelectedValue = "NO";
                    ddlFacoltaComputo.Enabled = false;
                }
            }

            //ENG - Memo 116/2025
            if (datiPensione.IsDomandaVOAUTAnticipataFlessibileLeggeBilancio2024FiltroGSEOrRicostituzione ||
                datiPensione.IsDomandaVOAUTAnticipataTipoContributivoFiltroGSEOrRicostituzione || datiPensione.IsDomandaVOAUTVecchiaiaTipoContributivoFiltroGSEOrRicostituzione)
            {
                pnlContributiItaEdEsteriAl1295.Visible = true;
                if (Dati != null && Dati.areaDatiContributiviAgo != null && Dati.areaDatiContributiviAgo.DatiCalcolo != null &&
                    Dati.areaDatiContributiviAgo.DatiCalcolo.ContributiItalianiEdEsteriAl1295 != null)
                    txtContributiItalianiEsteri.Text = Dati.areaDatiContributiviAgo.DatiCalcolo.ContributiItalianiEdEsteriAl1295.ToString();
                else
                    txtContributiItalianiEsteri.Text = string.Empty;
            }

            // Memo 79
            if (Utility.IsDomandaOrganizzazioniInternazionali(datiPensione))
            {
                modalitaEditContributivi.Value = "false";
            }
        }

        public void btnSalvaDatiCalcolo_Click(object sender, EventArgs e)
        {
            this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
            this.areaDatiContributiviAgo = new AreaDatiContributivi();

            //ENG - Aggiornamento Memo 68/2022 IOPGI
            if (this.TitolarePensione == null)
                this.TitolarePensione = new AreaTitolare();
            if (this.TitolarePensione.Pensione == null)
                this.TitolarePensione.Pensione = GetDatiPensione(this);



            RecuperaCampi(this.areaDatiContributiviAgo);



            //ENG - Aggiornamento Memo 68/2022 IOPGI
            //ENG - Spacchettate SOPGI
            if (this.areaDatiContributiviAgo.DatiCalcolo != null &&
                ((this.areaDatiContributiviAgo.DatiCalcolo.lDatiRetributivi != null && this.areaDatiContributiviAgo.DatiCalcolo.lDatiRetributivi.Count() > 0) ||
                (this.areaDatiContributiviAgo.DatiCalcolo.lDatiContributivi != null && this.areaDatiContributiviAgo.DatiCalcolo.lDatiContributivi.Count() > 0)) ||
                ((Utility.IsDomandaVOPGI(this.domanda.Categoria) || (Utility.IsDomandaIOPGI(this.domanda.Categoria) && !Utility.IsDomandaIOPGI_AGI(this.domanda.Categoria, this.TitolarePensione.Pensione.Filtro)) || Utility.IsDomandaSpacchettamentoSOPGIPost072022(this.domanda.Categoria, this.TitolarePensione.Pensione, this.areaDanteCausa)) && this.areaDatiContributiviAgo.DatiCalcolo.PL_Coeftrasf.HasValue) ||
                (this.areaDatiContributiviAgo.DatiCalcolo.IsUnicarpe && Utility.IsDomandaINPGI(this.domanda.Categoria) && this.TitolarePensione != null && this.TitolarePensione.Pensione != null && this.TitolarePensione.Pensione.FineAssicurazione.HasValue && TitolarePensione.Pensione.FineAssicurazione.Value <= new DateTime(2022, 06, 30)))
            {
                PresenterDatiContributiviAGO presenterDatiContributiviAgo = new PresenterDatiContributiviAGO();
                presenterDatiContributiviAgo.SalvaDatiCalcolo(this);



                // serve per il salvataggio completo (rivisitabile)
                try
                {
                    ((Web.DatiContributiviAgo)sender).HasError = this.HasError;
                    ((Web.DatiContributiviAgo)sender).ErrorMessage = this.ErrorMessage;
                }
                catch (Exception)
                {
                    // Eccezione ignorata
                }
                AreaTitolare.DatiPensione datiPensione = new AreaTitolare.DatiPensione();
                datiPensione = GetDatiPensione(this);



                if (CodeUtility.IsRicostituzione(datiPensione) && !CodeUtility.IsRicostituzioneContributiva(datiPensione) && !Utility.IsRicostituzione_AccreditoPeriodiMaternita(datiPensione) && !(ViewState["EliminazioneScartoOneri0031_0105_0112"] != null && (string)ViewState["EliminazioneScartoOneri0031_0105_0112"] == "SI" && Utility.IsDomandaBeneficioTerrorismoLegge206_2004(datiPensione)) && (Utility.IsDomandaFPLD(this.domanda.Categoria) || Utility.IsDomandaGestioneAutonomi(this.domanda.Categoria) ||
                    Utility.IsDomandaDAI(this.domanda.Categoria) || Utility.IsDomandaAUT(this.domanda.Categoria)))
                    btnEliminaDatiCalcolo.Enabled = false;
                else
                    btnEliminaDatiCalcolo.Enabled = true;

            }
            else if (!string.IsNullOrEmpty(txtImportoLordoAllaDecorrenza.Text) || !string.IsNullOrEmpty(txtImportoLordo.Text) || !string.IsNullOrEmpty(txtImportoMensileAllaDecorrenzaOriginaria.Text) ||
                !string.IsNullOrEmpty(txtImportoMensileAlGennaio2001.Text))
            {
                PresenterDatiContributiviAGO presenterDatiContributiviAgo = new PresenterDatiContributiviAGO();
                presenterDatiContributiviAgo.SalvaDatiCalcolo(this);
            }
            else
            {
                this.HasError = true;
                if (this.areaDatiContributiviAgo != null && this.areaDatiContributiviAgo.IsDomandaINPGIFineAssicurazionePost30062022.GetValueOrDefault())
                    this.ErrorMessage = "Per data fine assicurazione maggiore al 30/06/2022 è necessaria la presenza di almeno una quota D.";
                else
                    this.ErrorMessage = "Non ci sono Dati Calcolo da salvare";
            }



            if (this.HasError)
            {
                RaiseHideAvviso(this, null);
                RaiseShowAvviso(this, null);
                this.areaDatiContributiviAgo = (AreaDatiContributivi)ViewState["areaDatiContributiviAgo"];
            }
            else
            {
                this.ErrorMessage = "Dati Calcolo salvati correttamente.";
                RaiseHideAvviso(this, null);
                RaiseShowAvviso(this, null);



                ViewState["areaDatiContributiviAgo"] = this.areaDatiContributiviAgo;
            }

            bool isAnte96Misto = (areaDatiContributiviAgo.IsAnte96 == Presenter.SvrLiquidazioneAgo.UtilityTipoAnte96.Ante96Miste);
            ReLoadData(this.areaDatiContributiviAgo.DatiCalcolo.lDatiRetributivi != null ?
                    MapDatiRetributiviForView(this.areaDatiContributiviAgo) : null,
                    this.areaDatiContributiviAgo.DatiCalcolo.lDatiContributivi != null ?
                    MapDatiContributiviForView(this.areaDatiContributiviAgo) : null, isAnte96Misto);
        }



        internal void RecuperaCampi(AreaDatiContributivi areaDatiContributiviAgo)
        {
            this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            //ENG - Aggiornamento Memo 68/2022 IOPGI
            if (this.TitolarePensione == null)
                this.TitolarePensione = new AreaTitolare();
            if (this.TitolarePensione.Pensione == null)
                this.TitolarePensione.Pensione = GetDatiPensione(this);

            if (this.areaDanteCausa == null)
            {
                PresenterDanteCausa presenterDanteCausa = new PresenterDanteCausa();
                presenterDanteCausa.GetDatiDanteCausa(this);
                Session["AreaDanteCausa"] = this.areaDanteCausa;
            }

            List<DatiRetributiviLocal> listaDatiRetribApp = (List<DatiRetributiviLocal>)ViewState[EnumViewState.ElencoDatiRetributivi.ToString()];
            List<DatiContributiviLocal> listaDatiContribApp = (List<DatiContributiviLocal>)ViewState[EnumViewState.ElencoDatiContributivi.ToString()];

            areaDatiContributiviAgo.DatiCalcolo = new GestioneContribDatiCalcolo();
            areaDatiContributiviAgo.DatiCalcolo.TipoCalcolo = ((AreaDatiContributivi)ViewState["areaDatiContributiviAgo"]).DatiCalcolo.TipoCalcolo;
            areaDatiContributiviAgo.DatiCalcolo.IdPensione = ((AreaDatiContributivi)ViewState["areaDatiContributiviAgo"]).DatiCalcolo.IdPensione;
            areaDatiContributiviAgo.DatiCalcolo.IsUnicarpe = ((AreaDatiContributivi)ViewState["areaDatiContributiviAgo"]).DatiCalcolo.IsUnicarpe;
            areaDatiContributiviAgo.IsFineAssicurazionePost2012 = ((AreaDatiContributivi)ViewState["areaDatiContributiviAgo"]).IsFineAssicurazionePost2012;
            areaDatiContributiviAgo.IsPnlImportoLordoAllaDecVisible = ((AreaDatiContributivi)ViewState["areaDatiContributiviAgo"]).IsPnlImportoLordoAllaDecVisible;
            areaDatiContributiviAgo.IsSettimane707Visible = ((AreaDatiContributivi)ViewState["areaDatiContributiviAgo"]).IsSettimane707Visible;
            areaDatiContributiviAgo.IsDomandaINPGIFineAssicurazionePost30062022 = ((AreaDatiContributivi)ViewState["areaDatiContributiviAgo"]).IsDomandaINPGIFineAssicurazionePost30062022;

            //ENG - Aggiornamento Memo 68/2022 IOPGI
            //ENG - Spacchettate SOPGI
            if (Utility.IsDomandaVOPGI(this.domanda.Categoria) || (Utility.IsDomandaIOPGI(this.domanda.Categoria) && !Utility.IsDomandaIOPGI_AGI(this.domanda.Categoria, this.TitolarePensione.Pensione.Filtro))
                || Utility.IsDomandaSpacchettamentoSOPGIPost072022(this.domanda.Categoria, this.TitolarePensione.Pensione, this.areaDanteCausa))
                areaDatiContributiviAgo.DatiCalcolo.PL_Coeftrasf = !string.IsNullOrEmpty(txtCoefficienteContributivo.Text) ? decimal.Parse(txtCoefficienteContributivo.Text) : (decimal?)null;
            else
                areaDatiContributiviAgo.DatiCalcolo.PL_Coeftrasf = ((AreaDatiContributivi)ViewState["areaDatiContributiviAgo"]).DatiCalcolo.PL_Coeftrasf;
            areaDatiContributiviAgo.IsDatiContributiviVittimeVisible = ((AreaDatiContributivi)ViewState["areaDatiContributiviAgo"]).IsDatiContributiviVittimeVisible;
            areaDatiContributiviAgo.IsDatiRetributiviVittimeVisible = ((AreaDatiContributivi)ViewState["areaDatiContributiviAgo"]).IsDatiRetributiviVittimeVisible;
            areaDatiContributiviAgo.IsSettimane707INPGIVisible = ((AreaDatiContributivi)ViewState["areaDatiContributiviAgo"]).IsSettimane707INPGIVisible;

            if ((listaDatiRetribApp != null && listaDatiRetribApp.Count() > 0) || (listaDatiContribApp != null && listaDatiContribApp.Count() > 0))
            {
                if ((listaDatiContribApp != null && listaDatiContribApp.Count() > 0))
                {
                    List<GestioneAggiornamentoPECODatiContributivi> listContr = GetDataContributiviToSave(listaDatiContribApp);
                    int nDatiContributivi = listContr.Count();
                    areaDatiContributiviAgo.DatiCalcolo.lDatiContributivi = new GestioneAggiornamentoPECODatiContributivi[nDatiContributivi];
                    areaDatiContributiviAgo.DatiCalcolo.lDatiContributivi = listContr.ToArray();
                }

                if ((listaDatiRetribApp != null && listaDatiRetribApp.Count() > 0))
                {
                    List<GestioneAggiornamentoPECODatiRetributivi> listRetr = GetDataRetributiviToSave(listaDatiRetribApp);
                    int nDatiRetributivi = listRetr.Count();
                    areaDatiContributiviAgo.DatiCalcolo.lDatiRetributivi = new GestioneAggiornamentoPECODatiRetributivi[nDatiRetributivi];
                    areaDatiContributiviAgo.DatiCalcolo.lDatiRetributivi = listRetr.ToArray();
                }
                if (Utility.IsDomandaAUT(this.domanda.Categoria))
                {
                    areaDatiContributiviAgo.DatiCalcolo.FacoltaComputo = ddlFacoltaComputo.SelectedValue == "SI" ? true : false;
                }
            }
            else if (!string.IsNullOrEmpty(txtImportoLordoAllaDecorrenza.Text))
            {
                areaDatiContributiviAgo.DatiCalcolo.ImportoLordoAllaDecorrenza = !string.IsNullOrEmpty(txtImportoLordoAllaDecorrenza.Text) ? decimal.Parse(txtImportoLordoAllaDecorrenza.Text) : (decimal?)null;
            }
            else if (!string.IsNullOrEmpty(txtImportoLordo.Text))
            {
                areaDatiContributiviAgo.DatiCalcolo.ImportoLordo = !string.IsNullOrEmpty(txtImportoLordo.Text) ? decimal.Parse(txtImportoLordo.Text) : (decimal?)null;
            }
            else if (Utility.IsDomandaRenditaCasalinghe(this.domanda.Categoria) || Utility.IsDomandaRenditaFacoltativa(this.domanda.Categoria))
            {
                areaDatiContributiviAgo.DatiCalcolo.ImportoMensileAllaDecorrenzaOriginaria = !string.IsNullOrEmpty(txtImportoMensileAllaDecorrenzaOriginaria.Text) ? decimal.Parse(txtImportoMensileAllaDecorrenzaOriginaria.Text) : (decimal?)null;
                areaDatiContributiviAgo.DatiCalcolo.ImportoMensileAlGennaio2001 = !string.IsNullOrEmpty(txtImportoMensileAlGennaio2001.Text) ? decimal.Parse(txtImportoMensileAlGennaio2001.Text) : (decimal?)null;
            }

            //ENG - Memo 116/2025
            if (pnlContributiItaEdEsteriAl1295.Visible == true)
            {
                if (!string.IsNullOrEmpty(txtContributiItalianiEsteri.Text))
                    areaDatiContributiviAgo.DatiCalcolo.ContributiItalianiEdEsteriAl1295 = int.Parse(txtContributiItalianiEsteri.Text);
                else
                    areaDatiContributiviAgo.DatiCalcolo.ContributiItalianiEdEsteriAl1295 = null;
            }
        }

        public void btnEliminaDatiCalcolo_Click(object sender, EventArgs e)
        {
            this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            List<DatiRetributiviLocal> listaDatiRetribApp = (List<DatiRetributiviLocal>)ViewState[EnumViewState.ElencoDatiRetributivi.ToString()];
            List<DatiContributiviLocal> listaDatiContribApp = (List<DatiContributiviLocal>)ViewState[EnumViewState.ElencoDatiContributivi.ToString()];

            if ((listaDatiRetribApp != null && listaDatiRetribApp.Count() > 0) || (listaDatiContribApp != null && listaDatiContribApp.Count() > 0) ||
                !string.IsNullOrEmpty(txtImportoLordoAllaDecorrenza.Text) || !string.IsNullOrEmpty(txtImportoLordo.Text) || !string.IsNullOrEmpty(txtImportoMensileAllaDecorrenzaOriginaria.Text) ||
                !string.IsNullOrEmpty(txtImportoMensileAlGennaio2001.Text))
            {
                PresenterDatiContributiviAGO presenterDatiContributiviAgo = new PresenterDatiContributiviAGO();
                presenterDatiContributiviAgo.EliminaDatiCalcolo(this);

                if (!this.HasError)
                {
                    if (listaDatiRetribApp != null && listaDatiRetribApp.Count() > 0)
                        modalitaEditRetributivi.Value = "false";

                    if ((listaDatiContribApp != null && listaDatiContribApp.Count() > 0))
                        modalitaEditContributivi.Value = "false";

                    InitializeData(this, null);
                }
                else
                    this.ErrorMessage = "Non ci sono Dati Calcolo da eliminare";
            }
            else
            {
                this.HasError = true;
                this.ErrorMessage = "Non ci sono Dati Calcolo da eliminare";
            }

            if (this.HasError)
                RaiseShowAvviso(this, null);
            else
            {
                this.ErrorMessage = "Dati Calcolo eliminati correttamente.";
                RaiseShowAvviso(this, null);
            }
        }

        public void EnabledBtnEliminaDatiCalcolo(bool enabled)
        {
            btnEliminaDatiCalcolo.Enabled = enabled;
        }

        internal void DisabilitaPulsanti()
        {
            btnSalvaDatiCalcolo.Enabled = btnPopUp.Enabled = false;
            btnEliminaDatiCalcolo.Enabled = false;
        }

        #endregion Public

        #region Private

        private void BindDataForPanels(AreaDatiContributivi areaDatiContributivi)
        {
            AreaTitolare.DatiPensione datiPensione = null;
            datiPensione = GetDatiPensione(this);

            if (this.domanda == null)
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            CodeUtility.TipologiaPensioneGruppo tipologiaGruppoPensione = CodeUtility.TipologiaPensioneGruppo.gr_NessunValore;
            CodeUtility.TipologiaPensioneProdotto tipologiaProdottoPensione = CodeUtility.TipologiaPensioneProdotto.pr_NessunValore;
            CodeUtility.TipologiaPensioneTipo tipologiaTipoPensione = CodeUtility.TipologiaPensioneTipo.tp_NessunValore;
            CodeUtility.GetTipologiaPensione(datiPensione.CodeGruppo, datiPensione.CodeProdotto, datiPensione.CodeTipo, out tipologiaGruppoPensione, out tipologiaProdottoPensione, out tipologiaTipoPensione);

            if (areaDatiContributivi != null)
            {
                if (areaDatiContributivi.IsPnlImportoLordoAllaDecVisible)
                {
                    //VESO92 FILTRO L92
                    pnlImportoLordoDecorrenza.Visible = true;
                    pdivRetributivo.Visible = false;
                    pdivContributivo.Visible = false;
                    pnlDomandeAUT.Visible = false;
                }
                else if (Utility.IsDomandaAPESociale(this.domanda.Categoria))
                {
                    pnlDatiCalcoloAPESociale.Visible = true;
                    pnlImportoLordoDecorrenza.Visible = false;
                    pdivRetributivo.Visible = false;
                    pdivContributivo.Visible = false;
                    pnlDomandeAUT.Visible = false;

                    if (Utility.IsDomandaUnicarpe(datiPensione, true) == Utility.TipoUnicarpe.Automatica || CodeUtility.IsRicostituzione(datiPensione))
                        txtImportoLordo.Enabled = false;
                }
                else if (Utility.IsDomandaRenditaCasalinghe(this.domanda.Categoria) || Utility.IsDomandaRenditaFacoltativa(this.domanda.Categoria))
                {
                    pnlDatiCalcoloAPESociale.Visible = false;
                    pnlDatiCalcoloRendita.Visible = true;
                    pnlDomandeAUT.Visible = false;
                    pnlImportoLordoDecorrenza.Visible = false;
                    pdivContributivo.Visible = false;
                    pdivRetributivo.Visible = false;
                    if (Utility.DataSuccessivaA(datiPensione.DecorrenzaOriginaria.GetValueOrDefault(), new DateTime(2001, 01, 01)))
                    {
                        trLblImportoMensileAlGennaio2001.Visible = false;
                        trTxtImportoMensileAlGennaio2001.Visible = false;
                    }
                }
                else if (areaDatiContributivi.DatiCalcolo != null && areaDatiContributivi.DatiCalcolo.IsUnicarpe)
                {
                    if (areaDatiContributivi.DatiCalcolo.lDatiContributivi != null && areaDatiContributivi.DatiCalcolo.lDatiRetributivi != null) // misto
                    {
                        pdivRetributivo.Visible = true;
                        pdivContributivo.Visible = true;
                        InitBindDataContributivi();
                        InitBindDataRetributivi();
                        return;
                    }
                    if (areaDatiContributivi.DatiCalcolo.lDatiContributivi == null && areaDatiContributivi.DatiCalcolo.lDatiRetributivi != null) // retributivo
                    {
                        pdivRetributivo.Visible = true;
                        pdivContributivo.Visible = false;
                        InitBindDataRetributivi();
                        return;
                    }
                    if (areaDatiContributivi.DatiCalcolo.lDatiContributivi != null && areaDatiContributivi.DatiCalcolo.lDatiRetributivi == null) // contributivo         
                    {
                        pdivRetributivo.Visible = false;
                        pdivContributivo.Visible = true;
                        InitBindDataContributivi();
                        return;

                    }
                    if (areaDatiContributivi.DatiCalcolo.lDatiContributivi == null && areaDatiContributivi.DatiCalcolo.lDatiRetributivi == null) // non valido
                    {
                        pdivRetributivo.Visible = false;
                        pdivContributivo.Visible = false;
                        return;
                    }
                }
                //ENG - Aggiornamento Memo 68/2022 IOPGI
                //ENG - Spacchettate SOPGI
                else if (Utility.IsDomandaVOPGI(this.domanda.Categoria) || (Utility.IsDomandaIOPGI(this.domanda.Categoria) && !Utility.IsDomandaIOPGI_AGI(this.domanda.Categoria, datiPensione.Filtro))
                    || Utility.IsDomandaSpacchettamentoSOPGIPost072022(this.domanda.Categoria, datiPensione, this.areaDanteCausa))
                {
                    //VOPGI NO AGI - 0001/0001/0017 E 0001/0002/0017
                    if (Utility.IsDomandaVOPGI(this.domanda.Categoria) && !Utility.IsDomandaVOPGI_AGI(this.domanda.Categoria, datiPensione.Filtro, datiPensione.DirittoAutonomo, datiPensione.GP1AJ11) &&
                        ((datiPensione.CodeGruppo == "0001" && datiPensione.CodeProdotto == "0001" && datiPensione.CodeTipo == "0017") || (datiPensione.CodeGruppo == "0001" && datiPensione.CodeProdotto == "0002" && datiPensione.CodeTipo == "0017")) &&
                        areaDatiContributivi != null && areaDatiContributivi.InizioAssicurazione.HasValue && Utility.DataStrettamenteSuccessivaA(areaDatiContributivi.InizioAssicurazione.Value, new DateTime(1995, 12, 31)))
                    {
                        //Visibile solo griglia dati contributivi
                        pdivRetributivo.Visible = false;
                        pdivContributivo.Visible = true;
                        InitBindDataContributivi();
                    }
                    //VOPGI NO AGI - 0001/0001/0001 E 0001/0002/0001 e tipo calcolo contributivo
                    else if (Utility.IsDomandaVOPGI(this.domanda.Categoria) && !Utility.IsDomandaVOPGI_AGI(this.domanda.Categoria, datiPensione.Filtro, datiPensione.DirittoAutonomo, datiPensione.GP1AJ11) && areaDatiContributivi.DatiCalcolo.TipoCalcolo == GestioneContribTipoCalcolo.Contributivo &&
                            ((datiPensione.CodeGruppo == "0001" && datiPensione.CodeProdotto == "0001" && datiPensione.CodeTipo == "0001") || (datiPensione.CodeGruppo == "0001" && datiPensione.CodeProdotto == "0002" && datiPensione.CodeTipo == "0001")))
                    {
                        //Visibile solo griglia dati contributivi
                        pdivRetributivo.Visible = false;
                        pdivContributivo.Visible = true;
                        InitBindDataContributivi();

                    }
                    else
                    {
                        pdivRetributivo.Visible = true;
                        pdivContributivo.Visible = true;
                        InitBindDataRetributivi();
                        InitBindDataContributivi();
                    }
                }
                else
                {
                    switch (areaDatiContributivi.DatiCalcolo.TipoCalcolo)
                    {
                        case GestioneContribTipoCalcolo.Contributivo:
                            pdivRetributivo.Visible = false;
                            pdivContributivo.Visible = true;
                            InitBindDataContributivi();
                            if (Utility.IsDomandaAUT(this.domanda.Categoria))
                            {
                                if (!datiPensione.IsDomandaVOAUTAnticipataFlessibileLeggeBilancio2024FiltroGSEOrRicostituzione && !datiPensione.IsDomandaVOAUTAnticipataTipoContributivoFiltroGSEOrRicostituzione &&
                                    !datiPensione.IsDomandaVOAUTVecchiaiaTipoContributivoFiltroGSEOrRicostituzione)
                                    pnlDomandeAUT.Visible = true;
                                //if (!CodeUtility.IsRicostituzioneOrRiapertura(TitolarePensione.Pensione, this.domanda.IsDomandaRiapertura))
                                //    HdnCodGestioneRIC.Value = "F0;F1";
                                if (!Utility.IsDomandaVOAUT(this.domanda.Categoria) || tipologiaTipoPensione == CodeUtility.TipologiaPensioneTipo.tp_Vecchiaia_TrasfAOI || Utility.IsDomandaSupplementare(datiPensione)
                                    || (Utility.IsDomandaAUT(this.domanda.Categoria) && !this.domanda.IsDomandaRiapertura && (Utility.IsDomandaRiliquidazioneVecchiaiaAnticipate(datiPensione) || Utility.IsDomandaRiliquidazioneIndiretta(datiPensione) || Utility.IsDomandaRliquidazioneAssegnoInvalidita(datiPensione)))) //per le VOAUT è gestito diversamente
                                {
                                    HdnCodGestioneAUT.Value = "C1;C2;C3;C4;C5;D1;E1;E2;A5;A6;A7;A8;A9;B1;B2;B3;B4;1;2;3;4"; //codici che per le AUT devono apparire se 'FacoltaComputo'=SI
                                    //ENG - Aggiornamento Memo 123_2021 
                                    if (ViewState["AbilitazioneMemo123_2021"] != null && (string)ViewState["AbilitazioneMemo123_2021"] == "SI")
                                        HdnCodGestioneAUT.Value = HdnCodGestioneAUT.Value.Insert(0, "F0;");
                                }
                            }
                            break;
                        case GestioneContribTipoCalcolo.Retributivo:
                            pdivRetributivo.Visible = true;
                            InitBindDataRetributivi();

                            if (areaDatiContributivi.IsFineAssicurazionePost2012)
                            {
                                pdivContributivo.Visible = true;
                                InitBindDataContributivi();
                            }
                            else
                            {
                                //Rifeerimento mail: FW: Reeng Pensioni AGO - Modifiche applicative inabilità del 14/01/2014
                                if (tipologiaProdottoPensione == CodeUtility.TipologiaPensioneProdotto.pr_InabilitaPensione && datiPensione.DecorrenzaOriginaria.HasValue &&
                                    Utility.DataStrettamenteSuccessivaA(datiPensione.DecorrenzaOriginaria.Value, new DateTime(2011, 12, 31)))
                                {
                                    pdivContributivo.Visible = true;
                                    InitBindDataContributivi();
                                    hfInabilitaConDecorrenzaPost122011.Value = "true";
                                }
                                else
                                {
                                    pdivContributivo.Visible = false;
                                }
                            }
                            break;
                        case GestioneContribTipoCalcolo.Misto:
                            pdivRetributivo.Visible = true;
                            pdivContributivo.Visible = true;
                            InitBindDataRetributivi();
                            InitBindDataContributivi();
                            break;
                        case GestioneContribTipoCalcolo.NonValido:
                            pdivRetributivo.Visible = false;
                            pdivContributivo.Visible = false;
                            break;
                    }
                }
                if (Utility.IsDomandaVOPGI(this.domanda.Categoria))
                    HdnIsDomandaVOPGI.Value = "true";

                //ENG - Aggiornamento Memo 68/2022 IOPGI
                if (Utility.IsDomandaIOPGI(this.domanda.Categoria) && !Utility.IsDomandaIOPGI_AGI(this.domanda.Categoria, datiPensione.Filtro))
                    HdnIsDomandaIOPGI.Value = "true";

                //ENG  - Spacchettamento SOPGI
                if (Utility.IsDomandaSpacchettamentoSOPGIPost072022(this.domanda.Categoria, datiPensione, this.areaDanteCausa))
                    HdnIsDomandaSpacchettamentoSOPGI.Value = "true";
            }
        }

        private void InitBindDataRetributivi()
        {
            List<DatiRetributiviLocal> elencoDatiRetributivi = new List<DatiRetributiviLocal>();

            AreaTitolare.DatiPensione datiPensione = new AreaTitolare.DatiPensione();
            datiPensione = GetDatiPensione(this);
            if (this.domanda == null)
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            if (((AreaDatiContributivi)ViewState["areaDatiContributiviAgo"]).DatiCalcolo.lDatiRetributivi != null)
                elencoDatiRetributivi = MapDatiRetributiviForView((AreaDatiContributivi)ViewState["areaDatiContributiviAgo"]);

            AreaDatiContributivi areaDatiContributiviAgo = ViewState["areaDatiContributiviAgo"] as AreaDatiContributivi;

            bool isAnte96Misto = (areaDatiContributiviAgo.IsAnte96 == Presenter.SvrLiquidazioneAgo.UtilityTipoAnte96.Ante96Miste);
            PrevalorizzaRecordRetributivo(elencoDatiRetributivi, datiPensione, isAnte96Misto);

            //DatiRetributiviLocal Empty = elencoDatiRetributivi.Find(delegate(DatiRetributiviLocal code)
            //{
            //    return (code.Decorrenza == string.Empty && code.Gestione == string.Empty && code.RetribuzioneMedia == string.Empty &&
            //            code.Settimane == string.Empty && code.Quota == string.Empty);
            //});

            //if (Empty == null && !((AreaDatiContributivi)ViewState["areaDatiContributiviAgo"]).DatiCalcolo.IsUnicarpe && !(ViewState[EnumViewState.IsPrimoRecordRetrGestioneS.ToString()] != null && (bool)ViewState[EnumViewState.IsPrimoRecordRetrGestioneS.ToString()]) &&
            //    !(!CodeUtility.IsRicostituzioneContributiva(datiPensione) && !Utility.IsRicostituzione_AccreditoPeriodiMaternita(datiPensione) &&
            //    CodeUtility.IsRicostituzione(datiPensione) && (Utility.IsDomandaFPLD(this.domanda.Categoria) || Utility.IsDomandaGestioneAutonomi(this.domanda.Categoria) || Utility.IsDomandaDAI(this.domanda.Categoria) ||
            //    Utility.IsDomandaAUT(this.domanda.Categoria))))
            //    elencoDatiRetributivi.Add(new DatiRetributiviLocal(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty));            

            gvDatiRetributivi.DataSource = elencoDatiRetributivi;

            if (areaDatiContributiviAgo != null && areaDatiContributiviAgo.IsAnte96 != null)
            {
                gvDatiRetributivi.Columns[4].HeaderText = "Retribuzione media settimanale";
            }

            ViewState[EnumViewState.ElencoDatiRetributivi.ToString()] = elencoDatiRetributivi;
            gvDatiRetributivi.DataBind();
        }

        private void InitBindDataContributivi()
        {
            List<DatiContributiviLocal> elencoDatiContributivi = new List<DatiContributiviLocal>();

            AreaTitolare.DatiPensione datiPensione = new AreaTitolare.DatiPensione();
            datiPensione = GetDatiPensione(this);
            if (this.domanda == null)
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
            if (this.TitolarePensione == null)
                this.TitolarePensione = new AreaTitolare();
            if (this.TitolarePensione.Pensione == null)
            {
                this.TitolarePensione.Pensione = GetDatiPensione(this);
                Session["Pensione"] = this.TitolarePensione.Pensione;
            }

            this.domandaDante = this.domanda;
            this.numDomanda = long.Parse(this.domanda.NumeroDomanda);
            if (this.areaDanteCausa == null)
            {
                PresenterDanteCausa presenterDanteCausa = new PresenterDanteCausa();
                presenterDanteCausa.GetDatiDanteCausa(this);
                Session["AreaDanteCausa"] = this.areaDanteCausa;
            }

            if (((AreaDatiContributivi)ViewState["areaDatiContributiviAgo"]).DatiCalcolo.lDatiContributivi != null)
                elencoDatiContributivi = MapDatiContributiviForView((AreaDatiContributivi)ViewState["areaDatiContributiviAgo"]);

            DatiContributiviLocal Empty = elencoDatiContributivi.Find(delegate(DatiContributiviLocal code)
            {
                return (code.AmmontareContributivo == string.Empty && code.Gestione == string.Empty && code.Quota == string.Empty &&
                        code.MontanteContributivo == string.Empty && code.Settimane == string.Empty);
            });

            if (Empty == null && !(((AreaDatiContributivi)ViewState["areaDatiContributiviAgo"]).DatiCalcolo.IsUnicarpe) && !(!CodeUtility.IsRicostituzioneContributiva(datiPensione) && !Utility.IsRicostituzione_AccreditoPeriodiMaternita(datiPensione) &&
                !(ViewState["EliminazioneScartoOneri0031_0105_0112"] != null && (string)ViewState["EliminazioneScartoOneri0031_0105_0112"] == "SI" && Utility.IsDomandaBeneficioTerrorismoLegge206_2004(datiPensione)) &&
                CodeUtility.IsRicostituzione(datiPensione) && (Utility.IsDomandaFPLD(this.domanda.Categoria) || Utility.IsDomandaGestioneAutonomi(this.domanda.Categoria) || Utility.IsDomandaDAI(this.domanda.Categoria) ||
                Utility.IsDomandaAUT(this.domanda.Categoria))))
                elencoDatiContributivi.Add(new DatiContributiviLocal(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, null));

            gvDatiContributivi.DataSource = elencoDatiContributivi;
            ViewState[EnumViewState.ElencoDatiContributivi.ToString()] = elencoDatiContributivi;

            if (isDomandaCRED27_VOCRED_GestioneL())
                gvDatiContributivi.Columns[4].HeaderText = "Importo";

            if (Utility.IsDomandaBancRicAnte1991(this.domanda.SiglaCategoriaPensione, this.TitolarePensione.Pensione, this.areaDanteCausa))
            {
                gvDatiContributivi.Columns[3].Visible = false;

                gvDatiContributivi.Columns[6].Visible = false;
                gvDatiContributivi.Columns[4].HeaderText = "Base";

            }
            AreaDatiContributivi areaDatiContributiviAgo = ViewState["areaDatiContributiviAgo"] as AreaDatiContributivi;
            if (areaDatiContributiviAgo != null && (areaDatiContributiviAgo.IsAnte96 == Presenter.SvrLiquidazioneAgo.UtilityTipoAnte96.Ante96Contributive || areaDatiContributiviAgo.IsAnte96 == Presenter.SvrLiquidazioneAgo.UtilityTipoAnte96.Ante96Miste))
            {
                gvDatiContributivi.Columns[3].HeaderText = "IVS";
                gvDatiContributivi.Columns[4].HeaderText = "Base";
            }
            gvDatiContributivi.DataBind();

            SetPopUpContributivi(GetDatiPensione(this), elencoDatiContributivi);
        }

        private bool isDomandaCRED27_VOCRED_GestioneL()
        {
            if (this.domanda == null)
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
            AreaTitolare.DatiPensione datiPensione = null;
            datiPensione = GetDatiPensione(this);

            GestioneAggiornamentoPECODatiContributivi[] elencoDatiContributivi = (((AreaDatiContributivi)ViewState["areaDatiContributiviAgo"]).DatiCalcolo.lDatiContributivi != null) ? ((AreaDatiContributivi)ViewState["areaDatiContributiviAgo"]).DatiCalcolo.lDatiContributivi : null;
            DecodificaGestioneCalcoloContributivo[] listaDecodificaGestioneCalcoloContributivo = (((DecodificaGestioneCalcoloContributivo[])ViewState["listaCodeGestioneCalcoloContrib"]) != null) ? (DecodificaGestioneCalcoloContributivo[])ViewState["listaCodeGestioneCalcoloContrib"] : null;

            if (CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, domanda.IsDomandaRiapertura) && Utility.IsDomandaVOCRED_CRED27(this.domanda.Categoria) && elencoDatiContributivi != null && elencoDatiContributivi.Count() > 0 &&
                listaDecodificaGestioneCalcoloContributivo != null && listaDecodificaGestioneCalcoloContributivo.Count() > 0)
            {
                DecodificaGestioneCalcoloContributivo gestioneL = listaDecodificaGestioneCalcoloContributivo.ToList().Find(x => x.TraduzioneSuGP.Trim().ToUpperInvariant() == "L");
                if (gestioneL != null)
                {
                    if (elencoDatiContributivi.ToList().Exists(x => x.CodGestione == gestioneL.Id))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private List<DatiContributiviLocal> MapDatiContributiviForView(AreaDatiContributivi areaDatiContributivi)
        {
            List<DatiContributiviLocal> elencoDatiContributivi = new List<DatiContributiviLocal>();
            foreach (GestioneAggiornamentoPECODatiContributivi contr in areaDatiContributivi.DatiCalcolo.lDatiContributivi.ToList<GestioneAggiornamentoPECODatiContributivi>())
            {
                string settimana = string.Empty;
                string importo = string.Empty;
                string montante = string.Empty;
                string PL_Quotac = string.Empty;
                if (contr.Quota.HasValue)
                {
                    if (contr.Quota.HasValue && contr.Quota.Value.ToString().ToUpperInvariant() == "C")
                    {
                        settimana = contr.Settimane.HasValue ? contr.Settimane.Value.ToString() : string.Empty;
                        importo = contr.ImportoContributivo.HasValue ? contr.ImportoContributivo.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;
                        montante = contr.MontanteContributivo.HasValue ? contr.MontanteContributivo.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;
                    }
                    else if (contr.Quota.HasValue && contr.Quota.Value.ToString().ToUpperInvariant() == "D")
                    {
                        settimana = contr.SettimaneQuotaD.HasValue ? contr.SettimaneQuotaD.Value.ToString() : string.Empty;
                        importo = contr.ImportoContributivoQuotaD.HasValue ? contr.ImportoContributivoQuotaD.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;
                        montante = contr.MontanteContributivoQuotaD.HasValue ? contr.MontanteContributivoQuotaD.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;
                    }
                }
                else if (isDomandaCRED27_VOCRED_GestioneL())
                {
                    settimana = (contr.Settimane.HasValue && contr.Settimane.Value > 0) ? contr.Settimane.Value.ToString() : string.Empty;
                    importo = (contr.ImportoContributivo.HasValue && contr.ImportoContributivo.Value > 0) ? contr.ImportoContributivo.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;
                    montante = (contr.MontanteContributivo.HasValue && contr.MontanteContributivo.Value > 0) ? contr.MontanteContributivo.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;
                }


                PL_Quotac = contr.PL_Quotac.HasValue ? contr.PL_Quotac.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;
                elencoDatiContributivi.Add(new DatiContributiviLocal(contr.CodGestione.HasValue ? contr.CodGestione.Value.ToString() : string.Empty,
                    contr.Quota.HasValue ? contr.Quota.Value.ToString() : string.Empty,
                    settimana, importo, montante, PL_Quotac, contr.DecorrenzaCalcoloContibutivo));
            }
            return elencoDatiContributivi;
        }

        private static List<DatiRetributiviLocal> MapDatiRetributiviForView(AreaDatiContributivi areaDatiContributivi)
        {
            List<DatiRetributiviLocal> elencoDatiRetributivi = new List<DatiRetributiviLocal>();
            foreach (GestioneAggiornamentoPECODatiRetributivi retr in areaDatiContributivi.DatiCalcolo.lDatiRetributivi.ToList<GestioneAggiornamentoPECODatiRetributivi>())
            {
                string settimana = string.Empty;
                string rmsQuota = string.Empty;
                string PL_Quotar = string.Empty;
                string PL_Quotar707 = string.Empty;
                string rms = string.Empty;
                decimal? rMSExCombattente;
                int? nSettAnzianitaVV;
                int? nSettimaneExCombattente;

                if (retr.Quota.HasValue)
                {
                    if (retr.Quota.HasValue && retr.Quota.Value.ToString().ToUpperInvariant() == "A")
                    {
                        settimana = retr.SettimaneA.HasValue ? retr.SettimaneA.Value.ToString() : string.Empty;
                        rmsQuota = retr.RMSQuotaA.HasValue ? retr.RMSQuotaA.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;
                    }
                    else if (retr.Quota.HasValue && retr.Quota.Value.ToString().ToUpperInvariant() == "B")
                    {
                        settimana = retr.SettimaneB.HasValue ? retr.SettimaneB.Value.ToString() : string.Empty;
                        rmsQuota = retr.RMSQuotaB.HasValue ? retr.RMSQuotaB.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;
                    }
                }
                PL_Quotar = retr.PL_Quotar.HasValue ? retr.PL_Quotar.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;
                PL_Quotar707 = retr.PL_Quotar707.HasValue ? retr.PL_Quotar707.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;
                rms = retr.RMS.HasValue ? retr.RMS.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;
                rMSExCombattente = retr.RMSExCombattente;
                nSettAnzianitaVV = retr.NSettAnzianitaVV;
                nSettimaneExCombattente = retr.NSettimaneExCombattente;
                elencoDatiRetributivi.Add(new DatiRetributiviLocal(retr.CodGestione.HasValue ? retr.CodGestione.Value.ToString() : string.Empty,
                    retr.Quota.HasValue ? retr.Quota.Value.ToString() : string.Empty,
                    settimana, areaDatiContributivi.IsAnte96 != null ? string.Format("{0:dd/MM/yyyy HH:mm:ss}", retr.Decorrenza) : string.Format("{0:dd/MM/yyyy}", retr.Decorrenza), rmsQuota, retr.NSettimane707.HasValue ? retr.NSettimane707.Value.ToString() : string.Empty, PL_Quotar, PL_Quotar707, rms,
                    rMSExCombattente, nSettAnzianitaVV, nSettimaneExCombattente));
            }
            return elencoDatiRetributivi;
        }

        private void LoadDecodificaData(IDatiContributiviAgo areaDatiContributivi)
        {
            if (areaDatiContributivi.areaDatiContributiviAgo != null)
            {
                ViewState["listaCodeGestioneCalcoloRetrib"] = areaDatiContributivi.areaDatiContributiviAgo.listaDecodificaGestioneCalcoloRetributivo;
                ViewState["listaCodeGestioneCalcoloContrib"] = areaDatiContributivi.areaDatiContributiviAgo.listaDecodificaGestioneCalcoloContributivo;
            }
        }

        private string GetValueFromIdRetr(string id)
        {
            string ret = string.Empty;
            if (!String.IsNullOrEmpty(id))
            {
                long index = Convert.ToInt64(id);
                DecodificaGestioneCalcoloRetributivo[] listaCodeGestioneCalcoloRetrib = (DecodificaGestioneCalcoloRetributivo[])ViewState["listaCodeGestioneCalcoloRetrib"];
                DecodificaGestioneCalcoloRetributivo app = listaCodeGestioneCalcoloRetrib.ToList().Find(delegate(DecodificaGestioneCalcoloRetributivo code) { return (code.Id == index); });
                if (app != null)
                    ret = app.TraduzioneSuGP + " - " + app.Descrizione;
            }
            return ret;
        }

        private string GetValueFromIdContr(string id)
        {
            string ret = string.Empty;
            if (!String.IsNullOrEmpty(id))
            {
                long index = Convert.ToInt64(id);
                DecodificaGestioneCalcoloContributivo[] listaCodeGestioneCalcoloContrib = (DecodificaGestioneCalcoloContributivo[])ViewState["listaCodeGestioneCalcoloContrib"];
                DecodificaGestioneCalcoloContributivo app = listaCodeGestioneCalcoloContrib.ToList().Find(delegate(DecodificaGestioneCalcoloContributivo code) { return (code.Id == index); });
                if (app != null)
                {
                    if (app.TraduzioneSuGP == "FB")
                        ret = app.Descrizione;
                    else
                        ret = app.TraduzioneSuGP + " - " + app.Descrizione;
                }
            }
            return ret;
        }

        private void GestioneDdls(GridViewRow row, bool IsRetrib)
        {
            DropDownList ddlGestione = new DropDownList();
            ddlGestione = (DropDownList)row.FindControl("ddlCodiceGestione");
            ddlGestione.Items.Add(new ListItem(string.Empty, string.Empty));
            List<string> lstCodiciAmmessi = (List<string>)ViewState["lstCodiciAmmessi"];
            Label lblCodiceGestione_item = (Label)row.FindControl("lblCodiceGestione_item");
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            //ENG - Aggiornamento Memo 68/2022 IOPGI
            if (this.TitolarePensione == null)
                this.TitolarePensione = new AreaTitolare();
            if (this.TitolarePensione.Pensione == null)
                this.TitolarePensione.Pensione = GetDatiPensione(this);

            AreaDatiContributivi areaDatiContributiviAgo = ViewState["areaDatiContributiviAgo"] as AreaDatiContributivi;
            if (IsRetrib)
            {
                DecodificaGestioneCalcoloRetributivo[] listaCodeGestioneCalcoloRetrib = (DecodificaGestioneCalcoloRetributivo[])ViewState["listaCodeGestioneCalcoloRetrib"];
                IEnumerable<DecodificaGestioneCalcoloRetributivo> listaOrdinata = listaCodeGestioneCalcoloRetrib.OrderBy(x => x.TraduzioneSuGP);
                if (lstCodiciAmmessi != null && listaOrdinata != null)
                    listaOrdinata = listaOrdinata.Select(x => x).Where(x => lstCodiciAmmessi.Contains(x.TraduzioneSuGP.ToString().Trim())).ToList();
                foreach (DecodificaGestioneCalcoloRetributivo datiCodeGestioneCalcoloRetrib in listaOrdinata)
                {
                    ListItem li = new ListItem();
                    li.Attributes.Add("title", datiCodeGestioneCalcoloRetrib.Descrizione);
                    li.Text = datiCodeGestioneCalcoloRetrib.TraduzioneSuGP + " - " + datiCodeGestioneCalcoloRetrib.Descrizione;
                    li.Value = datiCodeGestioneCalcoloRetrib.Id.ToString();
                    //Eng - Ric Vr, VOART, VOCOM che dal prelievo arrivano con codice gestione I, M o N non devono essere modificabili
                    if (datiCodeGestioneCalcoloRetrib.TraduzioneSuGP != "I" && datiCodeGestioneCalcoloRetrib.TraduzioneSuGP != "M" && datiCodeGestioneCalcoloRetrib.TraduzioneSuGP != "N")
                        ddlGestione.Items.Add(li);
                }
                if (((DatiRetributiviLocal)(row.DataItem)).Gestione.Trim() == string.Empty)
                    ddlGestione.SelectedIndex = 0;
                else
                    if (ddlGestione.Items.FindByValue(((DatiRetributiviLocal)(row.DataItem)).Gestione.Trim()) != null)
                        ddlGestione.Items.FindByValue(((DatiRetributiviLocal)(row.DataItem)).Gestione.Trim()).Selected = true;
                    else
                        ddlGestione.SelectedIndex = 0;

                DropDownList ddlQuota = new DropDownList();
                ddlQuota = (DropDownList)row.FindControl("ddlQuota");
                ddlQuota.SelectedValue = ((DatiRetributiviLocal)(row.DataItem)).Quota;
            }
            else
            {
                DecodificaGestioneCalcoloContributivo[] listaCodeGestioneCalcoloContrib = (DecodificaGestioneCalcoloContributivo[])ViewState["listaCodeGestioneCalcoloContrib"];
                IEnumerable<DecodificaGestioneCalcoloContributivo> listaOrdinata = listaCodeGestioneCalcoloContrib.OrderBy(x => x.TraduzioneSuGP);
                if (lstCodiciAmmessi != null && listaOrdinata != null)
                    listaOrdinata = listaOrdinata.Select(x => x).Where(x => lstCodiciAmmessi.Contains(x.TraduzioneSuGP.ToString().Trim())).ToList();
                foreach (DecodificaGestioneCalcoloContributivo datiCodeGestioneCalcoloContrib in listaOrdinata)
                {
                    ListItem li = new ListItem();
                    li.Attributes.Add("title", datiCodeGestioneCalcoloContrib.Descrizione);
                    if (datiCodeGestioneCalcoloContrib.TraduzioneSuGP == "FB")
                        li.Text = datiCodeGestioneCalcoloContrib.Descrizione;
                    else
                        li.Text = datiCodeGestioneCalcoloContrib.TraduzioneSuGP + " - " + datiCodeGestioneCalcoloContrib.Descrizione;
                    li.Value = datiCodeGestioneCalcoloContrib.Id.ToString();

                    ddlGestione.Items.Add(li);
                }
                if (((DatiContributiviLocal)(row.DataItem)).Gestione.Trim() == string.Empty)
                    ddlGestione.SelectedIndex = 0;
                else
                    if (ddlGestione.Items.FindByValue(((DatiContributiviLocal)(row.DataItem)).Gestione.Trim()) != null)
                        ddlGestione.Items.FindByValue(((DatiContributiviLocal)(row.DataItem)).Gestione.Trim()).Selected = true;
                    else
                        ddlGestione.SelectedIndex = 0;

                DropDownList ddlQuota = new DropDownList();
                ddlQuota = (DropDownList)row.FindControl("ddlQuota");
                ddlQuota.SelectedValue = ((DatiContributiviLocal)(row.DataItem)).Quota;

                //Riferimento mail: LIQPENS - Segnalazioni AGO del 30/06/2014
                //Dati contributivi: se il sistema di calcolo è retributivo, il codice quota C  non deve essere presente nel menu di scelta
                //ENG - Aggiornamento Memo 68/2022 IOPGI
                //ENG - Spacchettate SOPGI
                if ((((AreaDatiContributivi)ViewState["areaDatiContributiviAgo"]).DatiCalcolo.TipoCalcolo) == GestioneContribTipoCalcolo.Retributivo &&
                    !Utility.IsDomandaVOPGI(domanda.Categoria) && !(Utility.IsDomandaIOPGI(domanda.Categoria) && !Utility.IsDomandaIOPGI_AGI(domanda.Categoria, this.TitolarePensione.Pensione.Filtro))
                    && !Utility.IsDomandaSpacchettamentoSOPGIPost072022(this.domanda.Categoria, this.TitolarePensione.Pensione, this.areaDanteCausa))
                    ddlQuota.Items.Remove(ddlQuota.Items.FindByValue("C"));
            }

            if (this.TitolarePensione == null)
                this.TitolarePensione = new AreaTitolare();
            if (this.TitolarePensione.Pensione == null)
                this.TitolarePensione.Pensione = GetDatiPensione(this);
            if (ddlGestione != null && ddlGestione.Items != null && ddlGestione.Items.Count > 0 &&
                (Utility.IsDomandaVESO29(this.domanda.Categoria) && !string.IsNullOrEmpty(this.TitolarePensione.Pensione.Filtro) && this.TitolarePensione.Pensione.Filtro.Trim() == "OBG"))
            {
                ddlGestione.Items.Cast<ListItem>().Where(x => x.Value != string.Empty && x.Value != "1").ToList().ForEach(x => ddlGestione.Items.Remove(x));
            }
        }

        private bool IsEmptyEditableRowRetrib(GridViewRow row)
        {
            if ((row.FindControl("txtRetribuzioneMedia") != null && ((TextBox)row.FindControl("txtRetribuzioneMedia")).Text != string.Empty) ||
                (row.FindControl("txtSettimaneRetributive") != null && ((TextBox)row.FindControl("txtSettimaneRetributive")).Text != string.Empty) ||
                /*row.FindControl("txtDecorrenza") != null && ((TextBox)row.FindControl("txtDecorrenza")).Text != string.Empty &&*/
                (row.FindControl("ddlCodiceGestione") != null && ((DropDownList)row.FindControl("ddlCodiceGestione")).SelectedIndex != 0) ||
                (row.FindControl("ddlQuota") != null && ((DropDownList)row.FindControl("ddlQuota")).SelectedIndex != 0) ||
                (row.FindControl("txtSettimaneRetributive707") != null && ((TextBox)row.FindControl("txtSettimaneRetributive707")).Text != string.Empty)
                )

                return false;
            else
                return true;
        }

        private bool IsEmptyReadableRowRetrib(GridViewRow row)
        {
            if ((row.FindControl("lblCodiceGestione_item") != null && ((Label)row.FindControl("lblCodiceGestione_item")).Text != string.Empty) ||
                (row.FindControl("lblQuota_item") != null && ((Label)row.FindControl("lblQuota_item")).Text != string.Empty) ||
                /*(row.FindControl("lblDecorrenza") != null && ((Label)row.FindControl("lblDecorrenza")).Text != string.Empty) ||*/
                (row.FindControl("lblSettimane") != null && ((Label)row.FindControl("lblSettimane")).Text != string.Empty) ||
                (row.FindControl("lblRetribuzioneMedia") != null && ((Label)row.FindControl("lblRetribuzioneMedia")).Text != string.Empty) ||
                (row.FindControl("lblSettimane707") != null && ((Label)row.FindControl("lblSettimane707")).Text != string.Empty))
                return false;
            else
                return true;
        }

        private bool IsEmptyEditableRowContr(GridViewRow row, bool isBancRicAnte91)
        {
            if (isBancRicAnte91)
            {
                if (row.FindControl("txtMontanteContributivo") != null && ((TextBox)row.FindControl("txtMontanteContributivo")).Text != string.Empty &&
                row.FindControl("ddlCodiceGestione") != null && ((DropDownList)row.FindControl("ddlCodiceGestione")).SelectedIndex != 0)
                    return false;

            }
            else if (row.FindControl("txtMontanteContributivo") != null && ((TextBox)row.FindControl("txtMontanteContributivo")).Text != string.Empty &&
               row.FindControl("ddlCodiceGestione") != null && ((DropDownList)row.FindControl("ddlCodiceGestione")).SelectedIndex != 0 &&
               row.FindControl("ddlQuota") != null && ((DropDownList)row.FindControl("ddlQuota")).SelectedIndex != 0 &&
               row.FindControl("txtAmmontareContributivo") != null && ((TextBox)row.FindControl("txtAmmontareContributivo")).Text != string.Empty &&
               row.FindControl("txtSettimaneContributive") != null && ((TextBox)row.FindControl("txtSettimaneContributive")).Text != string.Empty)
                return false;

            return true;
        }

        private bool IsEmptyReadableRowContr(GridViewRow row)
        {
            if ((row.FindControl("lblCodiceGestione_item") != null && ((Label)row.FindControl("lblCodiceGestione_item")).Text != string.Empty) ||
                (row.FindControl("lblSettimane") != null && ((Label)row.FindControl("lblSettimane")).Text != string.Empty) ||
                (row.FindControl("lblAmmontareContributivo") != null && ((Label)row.FindControl("lblAmmontareContributivo")).Text != string.Empty) ||
                (row.FindControl("lblMontanteContributivo") != null && ((Label)row.FindControl("lblMontanteContributivo")).Text != string.Empty))
                return false;
            else
                return true;
        }

        private bool IsListaEmpty(bool IsRetr)
        {
            if (IsRetr)
            {
                List<DatiRetributiviLocal> listaDatiRetrApp = (List<DatiRetributiviLocal>)ViewState[EnumViewState.ElencoDatiRetributivi.ToString()];
                if (listaDatiRetrApp == null || (listaDatiRetrApp.Count == 1 && listaDatiRetrApp[0].Decorrenza == string.Empty &&
                    listaDatiRetrApp[0].Gestione == string.Empty && listaDatiRetrApp[0].Quota == string.Empty &&
                    listaDatiRetrApp[0].RetribuzioneMedia == string.Empty && listaDatiRetrApp[0].Settimane == string.Empty))
                    return true;
                else
                    return false;
            }
            else
            {
                List<DatiContributiviLocal> listaDatiContrApp = (List<DatiContributiviLocal>)ViewState[EnumViewState.ElencoDatiContributivi.ToString()];
                if (listaDatiContrApp == null || (listaDatiContrApp.Count == 1 && listaDatiContrApp[0].AmmontareContributivo == string.Empty &&
                    listaDatiContrApp[0].Gestione == string.Empty && listaDatiContrApp[0].MontanteContributivo == string.Empty &&
                    listaDatiContrApp[0].Settimane == string.Empty && listaDatiContrApp[0].PL_Quotac == string.Empty))
                    return true;
                else
                    return false;
            }
        }

        private List<GestioneAggiornamentoPECODatiRetributivi> GetDataRetributiviToSave(List<DatiRetributiviLocal> lDatiRetributiviLocal)
        {
            List<GestioneAggiornamentoPECODatiRetributivi> lRetr = new List<GestioneAggiornamentoPECODatiRetributivi>();

            foreach (DatiRetributiviLocal datiRetributiviLocal in lDatiRetributiviLocal)
            {
                if (datiRetributiviLocal.Decorrenza == string.Empty && datiRetributiviLocal.Gestione == string.Empty &&
                    datiRetributiviLocal.Quota == string.Empty && datiRetributiviLocal.RetribuzioneMedia == string.Empty && datiRetributiviLocal.Settimane == string.Empty)
                    continue;

                GestioneAggiornamentoPECODatiRetributivi Retr = new GestioneAggiornamentoPECODatiRetributivi();

                // reperire l'id del campo ddl Cod Gestione selezionato
                if (datiRetributiviLocal.Gestione.Trim() != string.Empty)
                    Retr.CodGestione = Convert.ToInt64(datiRetributiviLocal.Gestione.Trim());
                else
                    Retr.CodGestione = null;

                Retr.Decorrenza = !string.IsNullOrEmpty(datiRetributiviLocal.Decorrenza) && !string.IsNullOrEmpty(datiRetributiviLocal.Decorrenza.Trim()) ?
                    Utility.GetDateFromString(datiRetributiviLocal.Decorrenza.Trim()) : (DateTime?)null;
                Retr.Quota = datiRetributiviLocal.Quota.Trim() != string.Empty ? Convert.ToChar(datiRetributiviLocal.Quota.Trim()) : (char?)null;
                Retr.NSettimane707 = !string.IsNullOrEmpty(datiRetributiviLocal.Settimane707) ? Convert.ToInt32(datiRetributiviLocal.Settimane707) : (int?)null;

                if (datiRetributiviLocal.Quota != string.Empty && datiRetributiviLocal.Quota.Trim().ToUpperInvariant() == "A")
                {
                    Retr.RMSQuotaA = datiRetributiviLocal.RetribuzioneMedia.Trim() != string.Empty ? Convert.ToDecimal(datiRetributiviLocal.RetribuzioneMedia.Trim()) : (decimal?)null;
                    Retr.SettimaneA = datiRetributiviLocal.Settimane.Trim() != string.Empty ? Convert.ToInt32(datiRetributiviLocal.Settimane.Trim()) : (int?)null;
                }
                else if (datiRetributiviLocal.Quota != string.Empty && datiRetributiviLocal.Quota.Trim().ToUpperInvariant() == "B")
                {
                    Retr.RMSQuotaB = datiRetributiviLocal.RetribuzioneMedia.Trim() != string.Empty ? Convert.ToDecimal(datiRetributiviLocal.RetribuzioneMedia.Trim()) : (decimal?)null;
                    Retr.SettimaneB = datiRetributiviLocal.Settimane.Trim() != string.Empty ? Convert.ToInt32(datiRetributiviLocal.Settimane.Trim()) : (int?)null;

                }
                Retr.PL_Quotar = datiRetributiviLocal.PL_Quotar != null && datiRetributiviLocal.PL_Quotar != string.Empty ? Convert.ToDecimal(datiRetributiviLocal.PL_Quotar.Trim()) : (decimal?)null;
                Retr.PL_Quotar707 = datiRetributiviLocal.PL_Quotar707 != null && datiRetributiviLocal.PL_Quotar707 != string.Empty ? Convert.ToDecimal(datiRetributiviLocal.PL_Quotar707.Trim()) : (decimal?)null;
                Retr.RMS = datiRetributiviLocal.RMS != null && datiRetributiviLocal.RMS != string.Empty ? Convert.ToDecimal(datiRetributiviLocal.RMS.Trim()) : (decimal?)null;
                Retr.RMSExCombattente = datiRetributiviLocal.RMSExCombattente;
                Retr.NSettAnzianitaVV = datiRetributiviLocal.NSettAnzianitaVV;
                Retr.NSettimaneExCombattente = datiRetributiviLocal.NSettimaneExCombattente;
                lRetr.Add(Retr);
            }
            return lRetr;
        }

        private List<GestioneAggiornamentoPECODatiContributivi> GetDataContributiviToSave(List<DatiContributiviLocal> lDatiContributiviLocal)
        {
            List<GestioneAggiornamentoPECODatiContributivi> lContr = new List<GestioneAggiornamentoPECODatiContributivi>();
            Presenter.SvrLiquidazioneAgo.UtilityTipoAnte96? IsAnte96 = null;
            AreaDatiContributivi areaDatiContributiviAgo = ViewState["areaDatiContributiviAgo"] as AreaDatiContributivi;
            if (areaDatiContributiviAgo != null) IsAnte96 = areaDatiContributiviAgo.IsAnte96;
            foreach (DatiContributiviLocal datiContributiviLocal in lDatiContributiviLocal)
            {
                if (datiContributiviLocal.AmmontareContributivo == string.Empty && datiContributiviLocal.Gestione == string.Empty && datiContributiviLocal.Quota == string.Empty &&
                    datiContributiviLocal.MontanteContributivo == string.Empty && datiContributiviLocal.Settimane == string.Empty && datiContributiviLocal.PL_Quotac == string.Empty)
                    continue;

                GestioneAggiornamentoPECODatiContributivi Contr = new GestioneAggiornamentoPECODatiContributivi();

                // reperire l'id del campo ddl Cod Gestione selezionato
                if (datiContributiviLocal.Gestione.Trim() != string.Empty)
                    Contr.CodGestione = Convert.ToInt64(datiContributiviLocal.Gestione.Trim());
                else
                    Contr.CodGestione = null;

                Contr.Quota = !String.IsNullOrEmpty(datiContributiviLocal.Quota) ? Convert.ToChar(datiContributiviLocal.Quota) : (char?)null;

                if (Contr.Quota.HasValue && Contr.Quota.Value.ToString().ToUpperInvariant() == "C")
                {
                    Contr.ImportoContributivo = datiContributiviLocal.AmmontareContributivo.Trim() != string.Empty ? Convert.ToDecimal(datiContributiviLocal.AmmontareContributivo.Trim()) : (decimal?)null;
                    Contr.MontanteContributivo = datiContributiviLocal.MontanteContributivo.Trim() != string.Empty ? Convert.ToDecimal(datiContributiviLocal.MontanteContributivo.Trim()) : (decimal?)null;
                    Contr.Settimane = datiContributiviLocal.Settimane.Trim() != string.Empty ? Convert.ToInt32(datiContributiviLocal.Settimane.Trim()) : (int?)null;
                }
                else if (Contr.Quota.HasValue && Contr.Quota.Value.ToString().ToUpperInvariant() == "D")
                {
                    Contr.ImportoContributivoQuotaD = datiContributiviLocal.AmmontareContributivo.Trim() != string.Empty ? Convert.ToDecimal(datiContributiviLocal.AmmontareContributivo.Trim()) : (decimal?)null;
                    Contr.MontanteContributivoQuotaD = datiContributiviLocal.MontanteContributivo.Trim() != string.Empty ? Convert.ToDecimal(datiContributiviLocal.MontanteContributivo.Trim()) : (decimal?)null;
                    Contr.SettimaneQuotaD = datiContributiviLocal.Settimane.Trim() != string.Empty ? Convert.ToInt32(datiContributiviLocal.Settimane.Trim()) : (int?)null;
                }
                else if (!Contr.Quota.HasValue && isDomandaCRED27_VOCRED_GestioneL())
                {
                    Contr.ImportoContributivo = datiContributiviLocal.AmmontareContributivo.Trim() != string.Empty ? Convert.ToDecimal(datiContributiviLocal.AmmontareContributivo.Trim()) : (decimal?)null;
                    Contr.MontanteContributivo = datiContributiviLocal.MontanteContributivo.Trim() != string.Empty ? Convert.ToDecimal(datiContributiviLocal.MontanteContributivo.Trim()) : (decimal?)null;
                    Contr.Settimane = datiContributiviLocal.Settimane.Trim() != string.Empty ? Convert.ToInt32(datiContributiviLocal.Settimane.Trim()) : (int?)null;

                }
                else if (!Contr.Quota.HasValue && (Utility.IsDomandaBancRicAnte1991(this.domanda.Categoria, this.TitolarePensione.Pensione, this.areaDanteCausa) || IsAnte96 != null))
                {
                    Contr.MontanteContributivo = datiContributiviLocal.MontanteContributivo.Trim() != string.Empty ? Convert.ToDecimal(datiContributiviLocal.MontanteContributivo.Trim()) : (decimal?)null;
                    if (IsAnte96 != null) Contr.ImportoContributivo = datiContributiviLocal.AmmontareContributivo.Trim() != string.Empty ? Convert.ToDecimal(datiContributiviLocal.AmmontareContributivo.Trim()) : (decimal?)null;
                }

                Contr.PL_Quotac = datiContributiviLocal.PL_Quotac != null && datiContributiviLocal.PL_Quotac != string.Empty ? Convert.ToDecimal(datiContributiviLocal.PL_Quotac.Trim()) : (decimal?)null;
                Contr.DecorrenzaCalcoloContibutivo = datiContributiviLocal.DecorrenzaCalcoloContibutivo;
                lContr.Add(Contr);
            }
            return lContr;
        }

        public List<GestioneAggiornamentoPECODatiRetributivi> GetDataRetributiviPage()
        {
            List<GestioneAggiornamentoPECODatiRetributivi> lstRet = null;
            List<DatiRetributiviLocal> listaDatiRetribApp = (List<DatiRetributiviLocal>)ViewState[EnumViewState.ElencoDatiRetributivi.ToString()];
            if (listaDatiRetribApp != null && listaDatiRetribApp.Count > 0)
                lstRet = GetDataRetributiviToSave(listaDatiRetribApp);
            return lstRet;
        }

        public List<GestioneAggiornamentoPECODatiContributivi> GetDataContributiviPage()
        {
            List<GestioneAggiornamentoPECODatiContributivi> lstRet = null;
            List<DatiContributiviLocal> listaDatiRetribApp = (List<DatiContributiviLocal>)ViewState[EnumViewState.ElencoDatiContributivi.ToString()];
            if (listaDatiRetribApp != null && listaDatiRetribApp.Count > 0)
                lstRet = GetDataContributiviToSave(listaDatiRetribApp);
            return lstRet;
        }

        private List<DatiRetributiviLocal> AddRecordRetributivi(List<DatiRetributiviLocal> listaRecord, String gestione, String quota, String decorrenza, String settimane, String retribuzioneMedia, string sett707, string quoteRetributivo, string quoteRetributivo707)
        {
            listaRecord.Add(new DatiRetributiviLocal(gestione, quota, settimane, decorrenza, retribuzioneMedia, sett707, quoteRetributivo, quoteRetributivo707));
            return listaRecord;
        }

        private List<DatiContributiviLocal> AddRecordContributivi(List<DatiContributiviLocal> listaRecord, String gestione, String quota, String settimane, String ammontareContributivo, String montanteContributivo, string quotaContributiva)
        {
            listaRecord.Add(new DatiContributiviLocal(gestione, quota, settimane, ammontareContributivo, montanteContributivo, quotaContributiva, null));
            return listaRecord;
        }

        private void ReLoadData(List<DatiRetributiviLocal> listaDatiRetribApp, List<DatiContributiviLocal> listaDatiContribApp, bool isAnte96Misto)
        {
            if (listaDatiRetribApp != null)
            {
                AreaTitolare.DatiPensione datiPensione = new AreaTitolare.DatiPensione();
                datiPensione = GetDatiPensione(this);
                if (this.domanda == null)
                    this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

                DatiRetributiviLocal EmptyRetr = listaDatiRetribApp.Find(delegate(DatiRetributiviLocal code)
                {
                    return (code.Decorrenza == string.Empty && code.Gestione == string.Empty && code.RetribuzioneMedia == string.Empty &&
                            code.Settimane == string.Empty && code.Quota == string.Empty);
                });

                if (EmptyRetr == null && (!((AreaDatiContributivi)ViewState["areaDatiContributiviAgo"]).DatiCalcolo.IsUnicarpe && !(!CodeUtility.IsRicostituzioneContributiva(datiPensione) && !Utility.IsRicostituzione_AccreditoPeriodiMaternita(datiPensione) &&
                    !(ViewState["EliminazioneScartoOneri0031_0105_0112"] != null && (string)ViewState["EliminazioneScartoOneri0031_0105_0112"] == "SI" && Utility.IsDomandaBeneficioTerrorismoLegge206_2004(datiPensione)) &&
                CodeUtility.IsRicostituzione(datiPensione) && (Utility.IsDomandaFPLD(this.domanda.Categoria) || Utility.IsDomandaGestioneAutonomi(this.domanda.Categoria) || Utility.IsDomandaDAI(this.domanda.Categoria) ||
                Utility.IsDomandaAUT(this.domanda.Categoria))) || isAnte96Misto))
                    listaDatiRetribApp.Add(new DatiRetributiviLocal(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty));

                gvDatiRetributivi.DataSource = listaDatiRetribApp;
                ViewState[EnumViewState.ElencoDatiRetributivi.ToString()] = listaDatiRetribApp;
                gvDatiRetributivi.DataBind();
            }

            if (listaDatiContribApp != null)
            {
                AreaTitolare.DatiPensione datiPensione = new AreaTitolare.DatiPensione();
                datiPensione = GetDatiPensione(this);
                if (this.domanda == null)
                    this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

                DatiContributiviLocal EmptyContr = listaDatiContribApp.Find(delegate(DatiContributiviLocal code)
                {
                    return (code.AmmontareContributivo == string.Empty && code.Gestione == string.Empty &&
                            code.MontanteContributivo == string.Empty && code.Settimane == string.Empty);
                });

                if (EmptyContr == null && !(((AreaDatiContributivi)ViewState["areaDatiContributiviAgo"]).DatiCalcolo.IsUnicarpe) && !(!CodeUtility.IsRicostituzioneContributiva(datiPensione) && !Utility.IsRicostituzione_AccreditoPeriodiMaternita(datiPensione) &&
                    !(ViewState["EliminazioneScartoOneri0031_0105_0112"] != null && (string)ViewState["EliminazioneScartoOneri0031_0105_0112"] == "SI" && Utility.IsDomandaBeneficioTerrorismoLegge206_2004(datiPensione)) &&
                CodeUtility.IsRicostituzione(datiPensione) && (Utility.IsDomandaFPLD(this.domanda.Categoria) || Utility.IsDomandaGestioneAutonomi(this.domanda.Categoria) || Utility.IsDomandaDAI(this.domanda.Categoria) ||
                Utility.IsDomandaAUT(this.domanda.Categoria))))
                    listaDatiContribApp.Add(new DatiContributiviLocal(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, null));

                gvDatiContributivi.DataSource = listaDatiContribApp;
                ViewState[EnumViewState.ElencoDatiContributivi.ToString()] = listaDatiContribApp;

                if (isDomandaCRED27_VOCRED_GestioneL())
                    gvDatiContributivi.Columns[4].HeaderText = "Importo";

                gvDatiContributivi.DataBind();



            }
        }

        private void TabDatiCalcoloRequiredValidators(bool abilitaRF)
        {
            foreach (GridViewRow row in gvDatiRetributivi.Rows)
            {
                foreach (Control ctrl in GetAllControls(row))
                {
                    if (ctrl is RequiredFieldValidator)
                    {
                        RequiredFieldValidator rfv = (RequiredFieldValidator)ctrl;
                        if (rfv.ValidationGroup == "UCTabDatiCalcoloAgoRetr")
                        {
                            rfv.Enabled = abilitaRF;
                        }
                    }
                }
            }
        }

        // Helper ricorsivo per trovare tutti i controlli annidati
        private List<Control> GetAllControls(Control parent)
        {
            List<Control> result = new List<Control>();
            foreach (Control ctrl in parent.Controls)
            {
                result.Add(ctrl);
                result.AddRange(GetAllControls(ctrl));
            }
            return result;
        }


        private void ManagePulsanti()
        {
            if (this.domanda == null)
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            if (this.TitolarePensione == null)
                this.TitolarePensione = new AreaTitolare();
            if (this.TitolarePensione.Pensione == null)
                this.TitolarePensione.Pensione = GetDatiPensione(this);

            CodeUtility.TipologiaPensioneGruppo tipologiaGruppoPensione = CodeUtility.TipologiaPensioneGruppo.gr_NessunValore;
            CodeUtility.TipologiaPensioneProdotto tipologiaProdottoPensione = CodeUtility.TipologiaPensioneProdotto.pr_NessunValore;
            CodeUtility.TipologiaPensioneTipo tipologiaTipoPensione = CodeUtility.TipologiaPensioneTipo.tp_NessunValore;
            CodeUtility.GetTipologiaPensione(TitolarePensione.Pensione.CodeGruppo, TitolarePensione.Pensione.CodeProdotto, TitolarePensione.Pensione.CodeTipo, out tipologiaGruppoPensione, out tipologiaProdottoPensione, out tipologiaTipoPensione);

            if (tipologiaGruppoPensione == CodeUtility.TipologiaPensioneGruppo.gr_Ricostituzione || this.domanda.IsDomandaRiapertura)

                if ((this.modalitaEditContributivi.Value == "true" && !IsListaEmpty(false)) || (this.modalitaEditRetributivi.Value == "true" && !IsListaEmpty(true)))
                {
                    RaiseGestisciTastoSalva(this, null);
                    btnSalvaDatiCalcolo.Enabled = btnPopUp.Enabled = false;
                    btnEliminaDatiCalcolo.Enabled = false;
                    return;
                }

            if (areaDatiContributiviAgo == null)
                areaDatiContributiviAgo = (AreaDatiContributivi)ViewState["areaDatiContributiviAgo"];

            if (areaDatiContributiviAgo != null && areaDatiContributiviAgo.DatiCalcolo != null)
            {
                AreaTitolare.DatiPensione datiPensione = GetDatiPensione(this);
                switch (areaDatiContributiviAgo.DatiCalcolo.TipoCalcolo)
                {
                    case GestioneContribTipoCalcolo.Contributivo:
                        if (!IsListaEmpty(false))
                        {
                            RaiseGestisciTastoSalva(this, null);
                            btnSalvaDatiCalcolo.Enabled = btnPopUp.Enabled = true;
                            btnEliminaDatiCalcolo.Enabled = true;
                        }
                        else
                        {
                            RaiseGestisciTastoSalva(this, null);
                            btnSalvaDatiCalcolo.Enabled = btnPopUp.Enabled = false;
                            btnEliminaDatiCalcolo.Enabled = false;
                        }
                        break;
                    case GestioneContribTipoCalcolo.Retributivo:
                        // Per le domande provenienti da Unicarpe potrebbe non arrivare la Quota Contributiva anche se la Fine Assicurazione è posteriore al 31/12/2011
                        if (Utility.IsDomandaOrganizzazioniInternazionali(datiPensione))
                        {
                            if (!IsListaEmpty(false) || !IsListaEmpty(true))
                            {
                                RaiseGestisciTastoSalva(this, null);
                                btnSalvaDatiCalcolo.Enabled = btnPopUp.Enabled = true;
                                btnEliminaDatiCalcolo.Enabled = true;
                                TabDatiCalcoloRequiredValidators(false);
                            }
                            else
                            {
                                RaiseGestisciTastoSalva(this, null);
                                btnSalvaDatiCalcolo.Enabled = btnPopUp.Enabled = false;
                                btnEliminaDatiCalcolo.Enabled = false;
                                TabDatiCalcoloRequiredValidators(true);
                            }
                        }
                        else if (areaDatiContributiviAgo.IsFineAssicurazionePost2012 && !areaDatiContributiviAgo.DatiCalcolo.IsUnicarpe)
                        {
                            if (!IsListaEmpty(false) && !IsListaEmpty(true))
                            {
                                RaiseGestisciTastoSalva(this, null);
                                btnSalvaDatiCalcolo.Enabled = btnPopUp.Enabled = true;
                                btnEliminaDatiCalcolo.Enabled = true;
                            }
                            else
                            {
                                RaiseGestisciTastoSalva(this, null);
                                btnSalvaDatiCalcolo.Enabled = btnPopUp.Enabled = false;
                                btnEliminaDatiCalcolo.Enabled = false;
                            }
                        }
                        else
                        {
                            if (!IsListaEmpty(true))
                            {
                                RaiseGestisciTastoSalva(this, null);
                                btnSalvaDatiCalcolo.Enabled = btnPopUp.Enabled = true;
                                btnEliminaDatiCalcolo.Enabled = true;
                            }
                            else
                            {
                                RaiseGestisciTastoSalva(this, null);
                                btnSalvaDatiCalcolo.Enabled = btnPopUp.Enabled = false;
                                btnEliminaDatiCalcolo.Enabled = false;
                            }
                        }
                        break;
                    case GestioneContribTipoCalcolo.Misto:
                        if (Utility.IsDomandaOrganizzazioniInternazionali(datiPensione))
                        {
                            if (!IsListaEmpty(false) || !IsListaEmpty(true))
                            {
                                RaiseGestisciTastoSalva(this, null);
                                btnSalvaDatiCalcolo.Enabled = btnPopUp.Enabled = true;
                                btnEliminaDatiCalcolo.Enabled = true;
                                TabDatiCalcoloRequiredValidators(false);
                            }
                            else
                            {
                                RaiseGestisciTastoSalva(this, null);
                                btnSalvaDatiCalcolo.Enabled = btnPopUp.Enabled = false;
                                btnEliminaDatiCalcolo.Enabled = false;
                                TabDatiCalcoloRequiredValidators(true);
                            }
                        }
                        else
                        {
                            if (!IsListaEmpty(false) && !IsListaEmpty(true))
                            {
                                RaiseGestisciTastoSalva(this, null);
                                btnSalvaDatiCalcolo.Enabled = btnPopUp.Enabled = true;
                                btnEliminaDatiCalcolo.Enabled = true;
                            }
                            else
                            {
                                RaiseGestisciTastoSalva(this, null);
                                btnSalvaDatiCalcolo.Enabled = btnPopUp.Enabled = false;
                                btnEliminaDatiCalcolo.Enabled = false;
                            }
                        }
                        break;
                    case GestioneContribTipoCalcolo.NonValido:
                        RaiseGestisciTastoSalva(this, null);
                        btnSalvaDatiCalcolo.Enabled = btnPopUp.Enabled = false;
                        btnEliminaDatiCalcolo.Enabled = false;
                        break;
                }

            }

            //ENG - Integrazione Modifiche Accenture
            if (Utility.IsDomandaRipristino(this.TitolarePensione.Pensione))
            {

                btnSalvaDatiCalcolo.Enabled = false;
                btnEliminaDatiCalcolo.Enabled = false;
            }

            if (tipologiaGruppoPensione == CodeUtility.TipologiaPensioneGruppo.gr_Ricostituzione && !CodeUtility.IsRicostituzioneContributiva(this.TitolarePensione.Pensione) && !Utility.IsRicostituzione_AccreditoPeriodiMaternita(this.TitolarePensione.Pensione) && !(ViewState["EliminazioneScartoOneri0031_0105_0112"] != null && (string)ViewState["EliminazioneScartoOneri0031_0105_0112"] == "SI" && Utility.IsDomandaBeneficioTerrorismoLegge206_2004(this.TitolarePensione.Pensione)) && (Utility.IsDomandaFPLD(this.domanda.Categoria) ||
                Utility.IsDomandaGestioneAutonomi(this.domanda.Categoria) || Utility.IsDomandaDAI(this.domanda.Categoria) || Utility.IsDomandaAUT(this.domanda.Categoria)))
            {
                btnEliminaDatiCalcolo.Enabled = false;
            }
            else if ((tipologiaGruppoPensione == CodeUtility.TipologiaPensioneGruppo.gr_Ricostituzione || this.domanda.IsDomandaRiapertura) && !this.domanda.Categoria.StartsWith("S"))
            {
                btnEliminaDatiCalcolo.Enabled = true;
            }

            if (CodeUtility.IsRicostituzioneOrRiapertura(TitolarePensione.Pensione, this.domanda.IsDomandaRiapertura) &&
                ViewState["IsBeneficioVittimeTerrorismo"] != null && (bool)ViewState["IsBeneficioVittimeTerrorismo"]
                && !(ViewState["EliminazioneScartoOneri0031_0105_0112"] != null && (string)ViewState["EliminazioneScartoOneri0031_0105_0112"] == "SI" && Utility.IsDomandaBeneficioTerrorismoLegge206_2004(this.TitolarePensione.Pensione)))
            {
                gvDatiRetributivi.Enabled = false;
                gvDatiContributivi.Enabled = false;
                pnlDomandeAUT.Enabled = false;
                pnlImportoLordoDecorrenza.Enabled = false;
                pnlDatiCalcoloAPESociale.Enabled = false;
                btnEliminaDatiCalcolo.Enabled = false;
            }

            if (Utility.IsDomandaUnicarpe(TitolarePensione.Pensione, true) == Utility.TipoUnicarpe.Automatica && CodeUtility.IsRicostituzione(TitolarePensione.Pensione))
            {
                btnEliminaDatiCalcolo.Enabled = false;
            }

            //ENG - Aggiornamento Memo 68/2022 IOPGI
            //ENG - Spacchettate SOPGI
            if (Utility.IsDomandaVOPGI(this.domanda.Categoria) || (Utility.IsDomandaIOPGI(this.domanda.Categoria) && !Utility.IsDomandaIOPGI_AGI(this.domanda.Categoria, TitolarePensione.Pensione.Filtro))
               || Utility.IsDomandaSpacchettamentoSOPGIPost072022(this.domanda.Categoria, TitolarePensione.Pensione, this.areaDanteCausa))
            {
                btnSalvaDatiCalcolo.Enabled = btnPopUp.Enabled = true;
                if (Utility.IsDomandaSpacchettamentoSOPGIPost072022(this.domanda.Categoria, TitolarePensione.Pensione, this.areaDanteCausa) &&
                    (Utility.IsDomandaReversibilita(TitolarePensione.Pensione) || CodeUtility.IsRicostituzioneOrRiapertura(TitolarePensione.Pensione, this.domanda.IsDomandaRiapertura)))
                    btnEliminaDatiCalcolo.Enabled = false;
                else
                    btnEliminaDatiCalcolo.Enabled = true;
            }

            if (Utility.IsDomandaESOPMI(this.domanda.Categoria))
            {
                pnlImportoLordoDecorrenza.Enabled = false;
            }

            //ENG - RIC VOPGI NON CONTRIBUTIVE
            if (Utility.IsRicostituzione(TitolarePensione.Pensione) && !Utility.IsRicostituzione_MotiviContributivi(TitolarePensione.Pensione) && Utility.IsDomandaVOPGI(this.domanda.Categoria))
                btnEliminaDatiCalcolo.Enabled = false;

            if (ViewState["AbilitazioneMemo123_2021"] != null && ViewState["AbilitazioneMemo123_2021"].ToString().Trim().ToUpperInvariant() == "SI" && TitolarePensione.Pensione.IdTipoPLPerRIC.HasValue && (TitolarePensione.Pensione.IdTipoPLPerRIC.Value == 21 || TitolarePensione.Pensione.IdTipoPLPerRIC.Value == 26))
                btnEliminaDatiCalcolo.Enabled = false;
        }

        #endregion Private

        #region gvDatiContributivi

        private void RimuoviDallaGriglia(ref List<DatiContributiviLocal> lista, int index)
        {
            if (lista != null && lista.Count > index)
            {
                lista.RemoveAt(index);
            }
        }

        protected void gvDatiContributivi_RowEditing(object sender, GridViewEditEventArgs e)
        {
            try
            {
                gvDatiContributivi.EditIndex = e.NewEditIndex;
                List<DatiContributiviLocal> listaDatiContrApp = (List<DatiContributiviLocal>)ViewState[EnumViewState.ElencoDatiContributivi.ToString()];
                gvDatiContributivi.DataSource = listaDatiContrApp;
                gvDatiContributivi.DataBind();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCDatiContributiviAgo, Errore nel metodo gvDatiContributivi_RowEditing " + ex);
            }
        }

        protected void gvDatiContributivi_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {

        }

        protected void gvDatiContributivi_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            try
            {
                gvDatiContributivi.EditIndex = -1;

                List<DatiContributiviLocal> listaDatiContrApp = (List<DatiContributiviLocal>)ViewState[EnumViewState.ElencoDatiContributivi.ToString()];
                gvDatiContributivi.DataSource = listaDatiContrApp;
                gvDatiContributivi.DataBind();

                ManagePulsanti();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new DnaApplicationException("UCDatiContributiviAgo, Errore nel metodo gvDatiContributivi_RowCancelingEdit " + ex);
            }
        }

        protected void gvDatiContributivi_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Elimina")
            {
                #region Elimina

                GridViewRow r = (GridViewRow)((WebControl)e.CommandSource).NamingContainer;
                List<DatiContributiviLocal> listaDatiContrApp = (List<DatiContributiviLocal>)ViewState[EnumViewState.ElencoDatiContributivi.ToString()];
                HiddenField hdnGUID = (HiddenField)r.FindControl(Keys.HdnGUID);
                int index = listaDatiContrApp.FindIndex(x => x.Id.ToString() == hdnGUID.Value);
                RimuoviDallaGriglia(ref listaDatiContrApp, index);
                this.modalitaEditContributivi.Value = "false";
                ViewState[EnumViewState.ElencoDatiContributivi.ToString()] = listaDatiContrApp;
                gvDatiContributivi.EditIndex = -1;
                gvDatiContributivi.DataSource = listaDatiContrApp;
                gvDatiContributivi.DataBind();

                SetPopUpContributivi(GetDatiPensione(this), listaDatiContrApp);
                ManageUpdateGridVittimeTerrorismoContributivo();
                #endregion Elimina
            }
            else if (e.CommandName == "Edit")
            {
                this.modalitaEditContributivi.Value = "true";
            }
            else if (e.CommandName == "Salva")
            {
                #region Salva

                if (this.TitolarePensione == null || this.TitolarePensione.Pensione == null)
                {
                    this.TitolarePensione = new AreaTitolare();
                    this.TitolarePensione.Pensione = GetDatiPensione(this);
                }

                if (this.areaDanteCausa == null)
                {
                    this.areaDanteCausa = (AreaDanteCausa)Session["AreaDanteCausa"];
                }

                Presenter.SvrLiquidazioneAgo.UtilityTipoAnte96? IsAnte96 = null;
                AreaDatiContributivi areaDatiContributiviAgo = ViewState["areaDatiContributiviAgo"] as AreaDatiContributivi;
                if (areaDatiContributiviAgo != null) IsAnte96 = areaDatiContributiviAgo.IsAnte96;
                bool isBancRicAnte91 = Utility.IsDomandaBancRicAnte1991(this.domanda.Categoria, this.TitolarePensione.Pensione, this.areaDanteCausa) || IsAnte96 != null;

                if (!IsEmptyEditableRowContr((GridViewRow)((Control)e.CommandSource).NamingContainer, isBancRicAnte91) || controlsDatiCRED27_VOCRED_GestioneL((GridViewRow)((Control)e.CommandSource).NamingContainer))
                {
                    GridViewRow r = (GridViewRow)((WebControl)e.CommandSource).NamingContainer;
                    List<DatiContributiviLocal> listaDatiContrApp = (List<DatiContributiviLocal>)ViewState[EnumViewState.ElencoDatiContributivi.ToString()];
                    HiddenField hdnGUID = (HiddenField)r.FindControl(Keys.HdnGUID);
                    int index = listaDatiContrApp.FindIndex(x => x.Id.ToString() == hdnGUID.Value);

                    listaDatiContrApp[index].Quota = ((DropDownList)r.FindControl(Keys.DatiContributivi_DdlQuota)).SelectedValue;
                    listaDatiContrApp[index].Settimane = ((TextBox)r.FindControl(Keys.DatiContribtutivi_TxtSettimaneContributive)).Text;
                    listaDatiContrApp[index].AmmontareContributivo = ((TextBox)r.FindControl(Keys.DatiContributivi_TxtAmmontareContributivo)).Text;
                    listaDatiContrApp[index].MontanteContributivo = ((TextBox)r.FindControl(Keys.DatiContributivi_TxtMontanteContributivo)).Text;
                    listaDatiContrApp[index].Gestione = ((DropDownList)r.FindControl(Keys.DatiContributivi_DdlCodiceGestione)).SelectedValue;
                    listaDatiContrApp[index].PL_Quotac = ((TextBox)r.FindControl(Keys.DatiContributivi_TxtQuotaContributiva)).Text;

                    // Sto inserendo un nuovo record
                    if (index == listaDatiContrApp.Count - 1)
                        listaDatiContrApp.Add(new DatiContributiviLocal(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, null));
                    gvDatiContributivi.EditIndex = -1;
                    ViewState[EnumViewState.ElencoDatiContributivi.ToString()] = listaDatiContrApp;
                    this.modalitaEditContributivi.Value = "false";
                    gvDatiContributivi.DataSource = listaDatiContrApp;
                    gvDatiContributivi.DataBind();

                    SetPopUpContributivi(GetDatiPensione(this), listaDatiContrApp);
                    ManageUpdateGridVittimeTerrorismoContributivo();
                }
                #endregion Salva
            }
            else if (e.CommandName == "Annulla")
            {
                List<DatiContributiviLocal> listaDatiContrApp = (List<DatiContributiviLocal>)ViewState[EnumViewState.ElencoDatiContributivi.ToString()];
                if (!IsListaEmpty(false))
                {
                    this.modalitaEditContributivi.Value = "false";
                    gvDatiContributivi.EditIndex = -1;
                    gvDatiContributivi.DataSource = listaDatiContrApp;
                    gvDatiContributivi.DataBind();
                }
            }
        }

        private void ManageUpdateGridVittimeTerrorismoContributivo()
        {
            Presenter.SvrLiquidazioneAgo.AreaDatiContributivi area = (Presenter.SvrLiquidazioneAgo.AreaDatiContributivi)ViewState["areaDatiContributiviAgo"];
            if (area != null && area.IsDatiContributiviVittimeVisible)
                RaiseUpdateDatiCalcoloTerrorismoContributivi(this, new EventArgs());
        }

        protected void gvDatiContributivi_DataBound(object sender, EventArgs e)
        {
            ManagePulsanti();
        }

        protected void gvDatiContributivi_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                AreaTitolare.DatiPensione datiPensione = null;
                datiPensione = GetDatiPensione(this);
                if (this.domanda == null)
                    this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

                AreaDatiContributivi areaDatiContributiviAgo = ViewState["areaDatiContributiviAgo"] as AreaDatiContributivi;

                if (this.TitolarePensione == null || this.TitolarePensione.Pensione == null)
                {
                    this.TitolarePensione = new AreaTitolare();
                    this.TitolarePensione.Pensione = GetDatiPensione(this);
                }

                if (this.areaDanteCausa == null)
                {
                    this.areaDanteCausa = (AreaDanteCausa)Session["AreaDanteCausa"];
                }
                bool isBancRicAnte91 = Utility.IsDomandaBancRicAnte1991(this.domanda.SiglaCategoriaPensione, this.TitolarePensione.Pensione, this.areaDanteCausa);


                //ENG - Spacchettamento SOPGI
                bool disabilitaDatiCalcoloSpacchettamentoSOPGI = false;
                if (Utility.IsDomandaSpacchettamentoSOPGIPost072022(this.domanda.Categoria, datiPensione, this.areaDanteCausa))
                {
                    if (Utility.IsDomandaReversibilita(datiPensione) || (Utility.IsDomandaIndiretta(datiPensione) && this.domanda.IsDomandaRiapertura && !this.areaDanteCausa.IsFascicoloGenerato.GetValueOrDefault()))
                    {
                        disabilitaDatiCalcoloSpacchettamentoSOPGI = true;
                    }
                }

                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    if ((((AreaDatiContributivi)ViewState["areaDatiContributiviAgo"]).DatiCalcolo.IsUnicarpe) || (CodeUtility.IsRicostituzione(datiPensione) && !CodeUtility.IsRicostituzioneContributiva(datiPensione) && !Utility.IsRicostituzione_AccreditoPeriodiMaternita(datiPensione) && !(ViewState["EliminazioneScartoOneri0031_0105_0112"] != null && (string)ViewState["EliminazioneScartoOneri0031_0105_0112"] == "SI" && Utility.IsDomandaBeneficioTerrorismoLegge206_2004(datiPensione)) &&
                        (Utility.IsDomandaFPLD(this.domanda.Categoria) || Utility.IsDomandaGestioneAutonomi(this.domanda.Categoria) || Utility.IsDomandaDAI(this.domanda.Categoria) || Utility.IsDomandaAUT(this.domanda.Categoria) || Utility.IsDomandaRipristino(datiPensione))) ||
                        (CodeUtility.IsRicostituzione(datiPensione) && areaDatiContributiviAgo.IsEliminataPerCauseVarie && areaDatiContributiviAgo.IsMemo102Abilitato.GetValueOrDefault() && (Utility.IsDomandaVOESO(this.domanda.Categoria) || Utility.IsDomandaVESO33(this.domanda.Categoria) || Utility.IsDomandaVESO92(this.domanda.Categoria) || Utility.IsDomandaVOCOOP_COOP28(this.domanda.Categoria) ||
                        Utility.IsDomandaVOCRED_CRED27(this.domanda.Categoria) || Utility.IsDomandaESPA(this.domanda.Categoria) || Utility.IsDomandaESOTEL(this.domanda.Categoria) || Utility.IsDomandaESOAMB(this.domanda.Categoria) || Utility.IsDomandaVESO29(this.domanda.Categoria)))
                        //ENG - RIC VOPGI NON CONTRIBUTIVE
                        || (Utility.IsRicostituzione(datiPensione) && !Utility.IsRicostituzione_MotiviContributivi(datiPensione) && Utility.IsDomandaVOPGI(this.domanda.Categoria))
                        || disabilitaDatiCalcoloSpacchettamentoSOPGI || Utility.IsRicostituzione_PerVariazioneDatiSupplemento(datiPensione) || (ViewState["AbilitazioneMemo50_2023"] != null && (string)ViewState["AbilitazioneMemo50_2023"] == "SI" && CodeUtility.IsRicostituzioneSupplemento(datiPensione)))// sola lettura
                    {
                        gvDatiContributivi.EditIndex = -1;

                        ((Label)e.Row.FindControl("lblQuota_item")).Text = ((DatiContributiviLocal)(e.Row.DataItem)).Quota;
                        ((Label)e.Row.FindControl("lblCodiceGestione_item")).Text = GetValueFromIdContr(((DatiContributiviLocal)(e.Row.DataItem)).Gestione);
                        ((Label)e.Row.FindControl("lblCodiceGestione_item")).Attributes.Add("title", ((Label)e.Row.FindControl("lblCodiceGestione_item")).Text);
                        ((Label)e.Row.FindControl("lblIdCodeGestione")).Text = ((DatiContributiviLocal)(e.Row.DataItem)).Gestione;
                        ((Label)e.Row.FindControl("lblSettimane")).Text = ((DatiContributiviLocal)(e.Row.DataItem)).Settimane;
                        ((Label)e.Row.FindControl("lblAmmontareContributivo")).Text = !string.IsNullOrEmpty(((DatiContributiviLocal)(e.Row.DataItem)).AmmontareContributivo) ? CodeUtility.ConvertDecimalFixedPoint(((DatiContributiviLocal)(e.Row.DataItem)).AmmontareContributivo, 4) : "";
                        ((Label)e.Row.FindControl("lblMontanteContributivo")).Text = !string.IsNullOrEmpty(((DatiContributiviLocal)(e.Row.DataItem)).MontanteContributivo) ? CodeUtility.ConvertDecimalFixedPoint(((DatiContributiviLocal)(e.Row.DataItem)).MontanteContributivo, 4) : "";
                        ((Label)e.Row.FindControl("lblQuotaContributiva")).Text = !string.IsNullOrEmpty(((DatiContributiviLocal)(e.Row.DataItem)).PL_Quotac) ? CodeUtility.ConvertDecimalFixedPoint(((DatiContributiviLocal)(e.Row.DataItem)).PL_Quotac, 4) : "";

                        LinkButton button = ((LinkButton)(e.Row.Cells[0].Controls[0]));
                        button.Enabled = false;
                        button.Text = "&nbsp;&nbsp;&nbsp;";
                    }
                    else
                    {
                        //prima riga
                        if (e.Row.DataItemIndex == 0)
                        {
                            //vuota
                            if (IsListaEmpty(false) && !Convert.ToBoolean(modalitaEditContributivi.Value))
                            {
                                ManagePulsanti();
                                gvDatiContributivi.EditIndex = 0;
                                modalitaEditContributivi.Value = "true";

                                gvDatiContributivi.DataSource = (List<DatiContributiviLocal>)ViewState[EnumViewState.ElencoDatiContributivi.ToString()];
                                gvDatiContributivi.DataBind();
                            }
                            else if (IsEmptyEditableRowContr(e.Row, isBancRicAnte91))
                            {
                                if (e.Row.Cells[0].Controls.Count == 3)  // edit mode
                                {
                                    GestioneDdls(e.Row, false);
                                    CodeUtility.EnableEditableMode(e.Row.Cells[0], "UCTabDatiCalcoloAgoContr", Page.Theme);
                                    LinkButton delete = ((LinkButton)(e.Row.Cells[5].FindControl("btnDeleteContributivi")));
                                    delete.Text = string.Empty;
                                    if (ViewState["AbilitazioneMemo123_2021"] != null && ViewState["AbilitazioneMemo123_2021"].ToString().Trim().ToUpperInvariant() == "SI" && datiPensione.IdTipoPLPerRIC.HasValue && (datiPensione.IdTipoPLPerRIC.Value == 21 || datiPensione.IdTipoPLPerRIC.Value == 26))
                                    {
                                        DropDownList quota = ((DropDownList)e.Row.FindControl("ddlQuota"));
                                        DropDownList codiceGestione = ((DropDownList)e.Row.FindControl("ddlCodiceGestione"));
                                        if (quota != null)
                                            quota.Enabled = false;
                                        if (codiceGestione != null)
                                            codiceGestione.Enabled = false;
                                    }
                                    disabilitaCampiContributiviCRED27_VOCRED_GestioneL(e.Row);

                                }
                                else
                                {
                                    ((Label)e.Row.FindControl("lblCodiceGestione_item")).Text = GetValueFromIdContr(((DatiContributiviLocal)(e.Row.DataItem)).Gestione);
                                    ((Label)e.Row.FindControl("lblCodiceGestione_item")).Attributes.Add("title", ((Label)e.Row.FindControl("lblCodiceGestione_item")).Text);
                                    ((Label)e.Row.FindControl("lblQuota_item")).Text = ((DatiContributiviLocal)(e.Row.DataItem)).Quota;

                                    ((Label)e.Row.FindControl("lblIdCodeGestione")).Text = ((DatiContributiviLocal)(e.Row.DataItem)).Gestione;
                                    ((Label)e.Row.FindControl("lblSettimane")).Text = ((DatiContributiviLocal)(e.Row.DataItem)).Settimane;
                                    ((Label)e.Row.FindControl("lblAmmontareContributivo")).Text = !string.IsNullOrEmpty(((DatiContributiviLocal)(e.Row.DataItem)).AmmontareContributivo) ? CodeUtility.ConvertDecimalFixedPoint(((DatiContributiviLocal)(e.Row.DataItem)).AmmontareContributivo, 4) : "";
                                    ((Label)e.Row.FindControl("lblMontanteContributivo")).Text = !string.IsNullOrEmpty(((DatiContributiviLocal)(e.Row.DataItem)).MontanteContributivo) ? CodeUtility.ConvertDecimalFixedPoint(((DatiContributiviLocal)(e.Row.DataItem)).MontanteContributivo, 4) : "";
                                    ((Label)e.Row.FindControl("lblQuotaContributiva")).Text = !string.IsNullOrEmpty(((DatiContributiviLocal)(e.Row.DataItem)).PL_Quotac) ? CodeUtility.ConvertDecimalFixedPoint(((DatiContributiviLocal)(e.Row.DataItem)).PL_Quotac, 4) : "";
                                    CodeUtility.EnableReadableMode(e.Row.Cells[0], e.Row.Cells[5], Page.Theme, "btnDeleteContributivi");

                                    if (isDomandaCRED27_VOCRED_GestioneL())
                                    {
                                        LinkButton deleteContributivi = ((LinkButton)e.Row.FindControl("btnDeleteContributivi"));
                                        if (deleteContributivi != null)
                                            deleteContributivi.Visible = false;
                                    }
                                    if (ViewState["AbilitazioneMemo123_2021"] != null && ViewState["AbilitazioneMemo123_2021"].ToString().Trim().ToUpperInvariant() == "SI" && datiPensione.IdTipoPLPerRIC.HasValue && (datiPensione.IdTipoPLPerRIC.Value == 21 || datiPensione.IdTipoPLPerRIC.Value == 26))
                                    {
                                        LinkButton deleteContributivi = ((LinkButton)e.Row.FindControl("btnDeleteContributivi"));
                                        if (deleteContributivi != null)
                                            deleteContributivi.Visible = false;
                                    }

                                }
                            }
                            else  //prima riga non vuota
                            {
                                if (e.Row.Cells[0].Controls.Count == 3)  // edit mode
                                {
                                    GestioneDdls(e.Row, false);
                                    //EnableEditableModeContr(e.Row.Cells[0]);
                                    CodeUtility.EnableEditableMode(e.Row.Cells[0], "UCTabDatiCalcoloAgoContr", Page.Theme);
                                    disabilitaCampiContributiviCRED27_VOCRED_GestioneL(e.Row);
                                    if (ViewState["AbilitazioneMemo123_2021"] != null && ViewState["AbilitazioneMemo123_2021"].ToString().Trim().ToUpperInvariant() == "SI" && datiPensione.IdTipoPLPerRIC.HasValue && (datiPensione.IdTipoPLPerRIC.Value == 21 || datiPensione.IdTipoPLPerRIC.Value == 26))
                                    {
                                        DropDownList quota = ((DropDownList)e.Row.FindControl("ddlQuota"));
                                        DropDownList codiceGestione = ((DropDownList)e.Row.FindControl("ddlCodiceGestione"));
                                        if (quota != null)
                                            quota.Enabled = false;
                                        if (codiceGestione != null)
                                            codiceGestione.Enabled = false;
                                    }
                                }
                                else
                                {
                                    ((Label)e.Row.FindControl("lblCodiceGestione_item")).Text = GetValueFromIdContr(((DatiContributiviLocal)(e.Row.DataItem)).Gestione);
                                    ((Label)e.Row.FindControl("lblCodiceGestione_item")).Attributes.Add("title", ((Label)e.Row.FindControl("lblCodiceGestione_item")).Text);
                                    ((Label)e.Row.FindControl("lblQuota_item")).Text = ((DatiContributiviLocal)(e.Row.DataItem)).Quota;

                                    ((Label)e.Row.FindControl("lblIdCodeGestione")).Text = ((DatiContributiviLocal)(e.Row.DataItem)).Gestione;
                                    ((Label)e.Row.FindControl("lblSettimane")).Text = ((DatiContributiviLocal)(e.Row.DataItem)).Settimane;
                                    ((Label)e.Row.FindControl("lblAmmontareContributivo")).Text = CodeUtility.ConvertDecimalFixedPoint(((DatiContributiviLocal)(e.Row.DataItem)).AmmontareContributivo, 4);
                                    ((Label)e.Row.FindControl("lblMontanteContributivo")).Text = CodeUtility.ConvertDecimalFixedPoint(((DatiContributiviLocal)(e.Row.DataItem)).MontanteContributivo, 4);
                                    ((Label)e.Row.FindControl("lblQuotaContributiva")).Text = !string.IsNullOrEmpty(((DatiContributiviLocal)(e.Row.DataItem)).PL_Quotac) ? CodeUtility.ConvertDecimalFixedPoint(((DatiContributiviLocal)(e.Row.DataItem)).PL_Quotac, 4) : "";

                                    CodeUtility.EnableReadableMode(e.Row.Cells[0], e.Row.Cells[5], Page.Theme, "btnDeleteContributivi");

                                    if (isDomandaCRED27_VOCRED_GestioneL())
                                    {
                                        LinkButton deleteContributivi = ((LinkButton)e.Row.FindControl("btnDeleteContributivi"));
                                        if (deleteContributivi != null)
                                            deleteContributivi.Visible = false;
                                    }
                                    if (ViewState["AbilitazioneMemo123_2021"] != null && ViewState["AbilitazioneMemo123_2021"].ToString().Trim().ToUpperInvariant() == "SI" && datiPensione.IdTipoPLPerRIC.HasValue && (datiPensione.IdTipoPLPerRIC.Value == 21 || datiPensione.IdTipoPLPerRIC.Value == 26))
                                    {
                                        LinkButton deleteContributivi = ((LinkButton)e.Row.FindControl("btnDeleteContributivi"));
                                        if (deleteContributivi != null)
                                            deleteContributivi.Visible = false;
                                    }
                                }
                            }
                        }
                        else  // righe successive
                        {

                            if (e.Row.Cells[0].Controls.Count == 3)  // edit mode
                            {
                                GestioneDdls(e.Row, false);
                                //EnableEditableModeContr(e.Row.Cells[0]);
                                CodeUtility.EnableEditableMode(e.Row.Cells[0], "UCTabDatiCalcoloAgoContr", Page.Theme);
                                if (ViewState["AbilitazioneMemo123_2021"] != null && ViewState["AbilitazioneMemo123_2021"].ToString().Trim().ToUpperInvariant() == "SI" && datiPensione.IdTipoPLPerRIC.HasValue && (datiPensione.IdTipoPLPerRIC.Value == 21 || datiPensione.IdTipoPLPerRIC.Value == 26))
                                {
                                    DropDownList quota = ((DropDownList)e.Row.FindControl("ddlQuota"));
                                    DropDownList codiceGestione = ((DropDownList)e.Row.FindControl("ddlCodiceGestione"));
                                    if (quota != null)
                                        quota.Enabled = false;
                                    if (codiceGestione != null)
                                        codiceGestione.Enabled = false;
                                }
                                disabilitaCampiContributiviCRED27_VOCRED_GestioneL(e.Row);
                            }

                            else if (e.Row.DataItemIndex == ((List<DatiContributiviLocal>)ViewState[EnumViewState.ElencoDatiContributivi.ToString()]).Count - 1)
                            {
                                if (!isDomandaCRED27_VOCRED_GestioneL() && !Utility.IsDomandaBancRicAnte1991(this.domanda.SiglaCategoriaPensione, this.TitolarePensione.Pensione, this.areaDanteCausa)
                                    && !(ViewState["AbilitazioneMemo123_2021"] != null && ViewState["AbilitazioneMemo123_2021"].ToString().Trim().ToUpperInvariant() == "SI" && datiPensione.IdTipoPLPerRIC.HasValue && (datiPensione.IdTipoPLPerRIC.Value == 21 || datiPensione.IdTipoPLPerRIC.Value == 26)))
                                {
                                    LinkButton add = ((LinkButton)(e.Row.Cells[0].Controls[0]));
                                    add.Text = (string)"<img width=16 height=16 border=0 src=../App_themes/" + Page.Theme + "/Images/add24.png />";
                                    add.ToolTip = "Aggiungi";
                                }
                                else
                                {
                                    LinkButton add = ((LinkButton)(e.Row.Cells[0].Controls[0]));
                                    add.Text = "&nbsp;&nbsp;&nbsp;";
                                    add.Enabled = false;
                                }
                            }
                            else
                            {
                                ((Label)e.Row.FindControl("lblCodiceGestione_item")).Text = GetValueFromIdContr(((DatiContributiviLocal)(e.Row.DataItem)).Gestione);
                                ((Label)e.Row.FindControl("lblCodiceGestione_item")).Attributes.Add("title", ((Label)e.Row.FindControl("lblCodiceGestione_item")).Text);
                                ((Label)e.Row.FindControl("lblQuota_item")).Text = ((DatiContributiviLocal)(e.Row.DataItem)).Quota;

                                ((Label)e.Row.FindControl("lblIdCodeGestione")).Text = ((DatiContributiviLocal)(e.Row.DataItem)).Gestione;
                                ((Label)e.Row.FindControl("lblSettimane")).Text = ((DatiContributiviLocal)(e.Row.DataItem)).Settimane;
                                ((Label)e.Row.FindControl("lblAmmontareContributivo")).Text = CodeUtility.ConvertDecimalFixedPoint(((DatiContributiviLocal)(e.Row.DataItem)).AmmontareContributivo, 4);
                                ((Label)e.Row.FindControl("lblMontanteContributivo")).Text = CodeUtility.ConvertDecimalFixedPoint(((DatiContributiviLocal)(e.Row.DataItem)).MontanteContributivo, 4);
                                ((Label)e.Row.FindControl("lblQuotaContributiva")).Text = !string.IsNullOrEmpty(((DatiContributiviLocal)(e.Row.DataItem)).PL_Quotac) ? CodeUtility.ConvertDecimalFixedPoint(((DatiContributiviLocal)(e.Row.DataItem)).PL_Quotac, 4) : "";
                                CodeUtility.EnableReadableMode(e.Row.Cells[0], e.Row.Cells[5], Page.Theme, "btnDeleteContributivi");

                                if (isDomandaCRED27_VOCRED_GestioneL())
                                {
                                    LinkButton deleteContributivi = ((LinkButton)e.Row.FindControl("btnDeleteContributivi"));
                                    if (deleteContributivi != null)
                                        deleteContributivi.Visible = false;
                                }
                                if (ViewState["AbilitazioneMemo123_2021"] != null && ViewState["AbilitazioneMemo123_2021"].ToString().Trim().ToUpperInvariant() == "SI" && datiPensione.IdTipoPLPerRIC.HasValue && (datiPensione.IdTipoPLPerRIC.Value == 21 || datiPensione.IdTipoPLPerRIC.Value == 26))
                                {
                                    LinkButton deleteContributivi = ((LinkButton)e.Row.FindControl("btnDeleteContributivi"));
                                    if (deleteContributivi != null)
                                        deleteContributivi.Visible = false;
                                }
                            }
                        }
                    }

                    if (isBancRicAnte91)
                    {
                        gvBancAnte91_RowDataBound(sender, e);
                    }

                    //ENG - Aggiornamento Memo 68/2022 IOPGI
                    //ENG - Spacchettate SOPGI
                    if (Utility.IsDomandaVOPGI(this.domanda.Categoria) || (Utility.IsDomandaIOPGI(this.domanda.Categoria) && !Utility.IsDomandaIOPGI_AGI(this.domanda.Categoria, datiPensione.Filtro))
                        || Utility.IsDomandaSpacchettamentoSOPGIPost072022(this.domanda.Categoria, datiPensione, this.areaDanteCausa))
                    {
                        RequiredFieldValidator validatorQuota = ((RequiredFieldValidator)e.Row.FindControl("RequiredFieldtxtQuotaContributiva"));
                        if (validatorQuota != null)
                            validatorQuota.Enabled = true;
                        RegularExpressionValidator validatorQuota2 = ((RegularExpressionValidator)e.Row.FindControl("regularTxtQuotaContributiva"));
                        if (validatorQuota2 != null)
                            validatorQuota2.Enabled = true;

                    }
                }
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCDatiContributiviAgo, Errore nel metodo gvDatiContributivi_RowDataBound " + ex);
            }
        }

        private void gvBancAnte91_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            RequiredFieldValidator validatorAmmontare = ((RequiredFieldValidator)e.Row.FindControl("RequiredFieldtxtAmmontareContributivo"));
            if (validatorAmmontare != null)
                validatorAmmontare.Enabled = false;
            RegularExpressionValidator validatorAmmontare2 = ((RegularExpressionValidator)e.Row.FindControl("regularTxtAmmontareContributivo"));
            if (validatorAmmontare2 != null)
                validatorAmmontare2.Enabled = false;

            RequiredFieldValidator validatorSettimane = ((RequiredFieldValidator)e.Row.FindControl("RequiredFieldtxtSettimaneContributive"));
            if (validatorSettimane != null)
                validatorSettimane.Enabled = false;

            RequiredFieldValidator validatorQuota = ((RequiredFieldValidator)e.Row.FindControl("RequiredFieldddlQuotaContrib"));
            if (validatorQuota != null)
                validatorQuota.Enabled = false;

            DropDownList ddlCodiceGestione = ((DropDownList)e.Row.FindControl("ddlCodiceGestione"));
            if (ddlCodiceGestione != null)
                ddlCodiceGestione.Enabled = false;

            Label labelQuota = ((Label)e.Row.FindControl("lblQuota_item"));
            if (labelQuota != null)
                labelQuota.Text = "";

            Label labelSettimane = ((Label)e.Row.FindControl("lblSettimane"));
            if (labelSettimane != null)
                labelSettimane.Text = "";

            DropDownList ddlQuota = ((DropDownList)e.Row.FindControl("ddlQuota"));
            if (ddlQuota != null)
            {
                ddlQuota.Enabled = false;
                ddlQuota.SelectedValue = null;
            }

            TextBox txtSettimaneContributive = ((TextBox)e.Row.FindControl("txtSettimaneContributive"));
            if (txtSettimaneContributive != null)
            {
                txtSettimaneContributive.Enabled = false;
                txtSettimaneContributive.Text = null;
            }
        }

        private void disabilitaCampiContributiviCRED27_VOCRED_GestioneL(GridViewRow rowContributiva)
        {
            AreaDatiContributivi areaDatiContributiviAgo = ViewState["areaDatiContributiviAgo"] as AreaDatiContributivi;
            Presenter.SvrLiquidazioneAgo.UtilityTipoAnte96? IsAnte96 = null;
            if (areaDatiContributiviAgo != null) IsAnte96 = areaDatiContributiviAgo.IsAnte96;
            if (IsAnte96 != null)
            {
                RegularExpressionValidator regularExpressionMontanteContributivo = ((RegularExpressionValidator)rowContributiva.FindControl("regularTxtMontanteContributivo"));
                if (regularExpressionMontanteContributivo != null)
                    regularExpressionMontanteContributivo.Enabled = false;

            }
            if (isDomandaCRED27_VOCRED_GestioneL())
            {
                DropDownList ddlCodiceGestione = ((DropDownList)(rowContributiva.FindControl("ddlCodiceGestione")));
                if (ddlCodiceGestione != null)
                    ddlCodiceGestione.Enabled = false;

                DropDownList ddlQuota = ((DropDownList)(rowContributiva.FindControl("ddlQuota")));
                if (ddlQuota != null)
                    ddlQuota.Enabled = false;

                TextBox txtSettimaneContributive = ((TextBox)rowContributiva.FindControl("txtSettimaneContributive"));
                if (txtSettimaneContributive != null)
                    txtSettimaneContributive.Enabled = false;

                TextBox txtAmmontareContributivo = ((TextBox)rowContributiva.FindControl("txtAmmontareContributivo"));
                if (txtAmmontareContributivo != null)
                    txtAmmontareContributivo.Enabled = false;

                RequiredFieldValidator validatorQuota = ((RequiredFieldValidator)rowContributiva.FindControl("RequiredFieldddlQuotaContrib"));
                if (validatorQuota != null)
                    validatorQuota.Enabled = false;

                RequiredFieldValidator validatorSettimaneContributive = ((RequiredFieldValidator)rowContributiva.FindControl("RequiredFieldtxtSettimaneContributive"));
                if (validatorSettimaneContributive != null)
                    validatorSettimaneContributive.Enabled = false;

                RequiredFieldValidator validatorAmmontareContributivo = ((RequiredFieldValidator)rowContributiva.FindControl("RequiredFieldtxtAmmontareContributivo"));
                if (validatorAmmontareContributivo != null)
                    validatorAmmontareContributivo.Enabled = false;

                RequiredFieldValidator validatorMontanteContributivo = ((RequiredFieldValidator)rowContributiva.FindControl("RequiredFieldtxtMontanteContributivo"));
                if (validatorMontanteContributivo != null)
                    validatorMontanteContributivo.ErrorMessage = "Importo contributivo: Campo obbligatorio";

            }
        }

        protected void gvDatiContributivi_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            try
            {
                gvDatiContributivi.EditIndex = -1;
                gvDatiContributivi.PageIndex = e.NewPageIndex;
                List<DatiContributiviLocal> elencoDatiContributivi = (List<DatiContributiviLocal>)ViewState[EnumViewState.ElencoDatiContributivi.ToString()];
                gvDatiContributivi.DataSource = elencoDatiContributivi;
                gvDatiContributivi.DataBind();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new DnaApplicationException("UCDatiContributiviAgo, Errore nel metodo gvDatiContributivi_PageIndexChanging" + ex);
            }
        }

        #endregion gvDatiContributivi

        #region gvDatiRetributivi

        private void RimuoviDallaGriglia(ref List<DatiRetributiviLocal> lista, int index)
        {
            if (lista != null && lista.Count > index)
            {
                lista.RemoveAt(index);
            }
        }

        protected void gvDatiRetributivi_RowEditing(object sender, GridViewEditEventArgs e)
        {
            try
            {
                gvDatiRetributivi.EditIndex = e.NewEditIndex;
                List<DatiRetributiviLocal> listaDatiRetrApp = (List<DatiRetributiviLocal>)ViewState[EnumViewState.ElencoDatiRetributivi.ToString()];
                gvDatiRetributivi.DataSource = listaDatiRetrApp;
                gvDatiRetributivi.DataBind();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCDatiRetributiviAgo, Errore nel metodo gvDatiRetributivi_RowEditing " + ex);
            }
        }

        protected void gvDatiRetributivi_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {

        }

        protected void gvDatiRetributivi_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            try
            {
                gvDatiRetributivi.EditIndex = -1;

                List<DatiRetributiviLocal> listaDatiRetrApp = (List<DatiRetributiviLocal>)ViewState[EnumViewState.ElencoDatiRetributivi.ToString()];
                gvDatiRetributivi.DataSource = listaDatiRetrApp;
                gvDatiRetributivi.DataBind();

                ManagePulsanti();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCDatiRetributiviAgo, Errore nel metodo gvDatiRetributivi_RowCancelingEdit " + ex);
            }
        }

        protected void gvDatiRetributivi_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Elimina")
            {
                #region Elimina

                GridViewRow r = (GridViewRow)((WebControl)e.CommandSource).NamingContainer;
                List<DatiRetributiviLocal> listaDatiRetribApp = (List<DatiRetributiviLocal>)ViewState[EnumViewState.ElencoDatiRetributivi.ToString()];
                HiddenField hdnGUID = (HiddenField)r.FindControl(Keys.HdnGUID);
                int index = listaDatiRetribApp.FindIndex(x => x.Id.ToString() == hdnGUID.Value);
                RimuoviDallaGriglia(ref listaDatiRetribApp, index);
                this.modalitaEditRetributivi.Value = "false";

                AreaTitolare.DatiPensione datiPensione = new AreaTitolare.DatiPensione();
                datiPensione = GetDatiPensione(this);
                if (areaDatiContributiviAgo == null)
                    areaDatiContributiviAgo = ViewState["areaDatiContributiviAgo"] as AreaDatiContributivi;
                bool isAnte96Misto = (areaDatiContributiviAgo.IsAnte96 == Presenter.SvrLiquidazioneAgo.UtilityTipoAnte96.Ante96Miste);
                PrevalorizzaRecordRetributivo(listaDatiRetribApp, datiPensione, isAnte96Misto);

                ViewState[EnumViewState.ElencoDatiRetributivi.ToString()] = listaDatiRetribApp;
                gvDatiRetributivi.EditIndex = -1;
                gvDatiRetributivi.DataSource = listaDatiRetribApp;
                gvDatiRetributivi.DataBind();

                ManageUpdateGridVittimeTerrorismoRetributivi();
                #endregion Elimina
            }
            else if (e.CommandName == "Edit")
            {
                modalitaEditRetributivi.Value = "true";
            }
            else if (e.CommandName == "Salva")
            {
                #region Salva

                if (!IsEmptyEditableRowRetrib((GridViewRow)((Control)e.CommandSource).NamingContainer))
                {
                    GridViewRow r = (GridViewRow)((WebControl)e.CommandSource).NamingContainer;
                    List<DatiRetributiviLocal> listaDatiRetrApp = (List<DatiRetributiviLocal>)ViewState[EnumViewState.ElencoDatiRetributivi.ToString()];
                    HiddenField hdnGUID = (HiddenField)r.FindControl(Keys.HdnGUID);
                    int index = listaDatiRetrApp.FindIndex(x => x.Id.ToString() == hdnGUID.Value);

                    Presenter.SvrLiquidazioneAgo.UtilityTipoAnte96? IsAnte96 = null;
                    AreaDatiContributivi areaDatiContributiviAgo = ViewState["areaDatiContributiviAgo"] as AreaDatiContributivi;
                    if (areaDatiContributiviAgo != null) IsAnte96 = areaDatiContributiviAgo.IsAnte96;

                    listaDatiRetrApp[index].Gestione = ((DropDownList)r.FindControl(Keys.DatiRetributivi_DdlCodiceGestione)).SelectedValue;
                    if ((IsAnte96 != null) && !areaDatiContributiviAgo.MostraQuotaAnte96.GetValueOrDefault())
                        listaDatiRetrApp[index].Quota = "A";
                    else
                        listaDatiRetrApp[index].Quota = ((DropDownList)r.FindControl(Keys.DatiRetributivi_DdlQuota)).SelectedValue;
                    listaDatiRetrApp[index].Decorrenza = ((Label)r.FindControl(Keys.DatiRetributivi_LblDecorrenza)).Text;
                    listaDatiRetrApp[index].Settimane = ((TextBox)r.FindControl(Keys.DatiRetributivi_TxtSettimaneRetributive)).Text;
                    listaDatiRetrApp[index].RetribuzioneMedia = ((TextBox)r.FindControl(Keys.DatiRetributivi_TxtRetribuzioneMedia)).Text;
                    listaDatiRetrApp[index].Settimane707 = ((TextBox)r.FindControl(Keys.DatiRetributivi_TxtSettimaneRetributive707)).Text;
                    listaDatiRetrApp[index].PL_Quotar = ((TextBox)r.FindControl(Keys.DatiRetributivi_TxtQuoteRetributivo)).Text;
                    listaDatiRetrApp[index].PL_Quotar707 = ((TextBox)r.FindControl(Keys.DatiRetributivi_TxtQuoteRetributivo707)).Text;

                    // Sto inserendo un nuovo record
                    if (index == listaDatiRetrApp.Count - 1)
                        listaDatiRetrApp.Add(new DatiRetributiviLocal(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty));
                    gvDatiRetributivi.EditIndex = -1;
                    ViewState[EnumViewState.ElencoDatiRetributivi.ToString()] = listaDatiRetrApp;
                    modalitaEditRetributivi.Value = "false";
                    gvDatiRetributivi.DataSource = listaDatiRetrApp;
                    gvDatiRetributivi.DataBind();
                }
                ManageUpdateGridVittimeTerrorismoRetributivi();
                #endregion Salva
            }
            else if (e.CommandName == "Annulla")
            {
                List<DatiRetributiviLocal> listaDatiRetrApp = (List<DatiRetributiviLocal>)ViewState[EnumViewState.ElencoDatiRetributivi.ToString()];
                if (!IsListaEmpty(true))
                {
                    modalitaEditRetributivi.Value = "false";
                    gvDatiRetributivi.EditIndex = -1;
                    gvDatiRetributivi.DataSource = listaDatiRetrApp;
                    gvDatiRetributivi.DataBind();
                }
            }


            ManagePulsanti();
        }

        protected void gvDatiRetributivi_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                AreaTitolare.DatiPensione datiPensione = null;
                datiPensione = GetDatiPensione(this);

                if (this.domanda == null)
                    this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

                AreaDatiContributivi areaDatiContributiviAgo = ViewState["areaDatiContributiviAgo"] as AreaDatiContributivi;

                //ENG - Spacchettamento SOPGI
                if (this.areaDanteCausa == null)
                {
                    this.areaDanteCausa = (AreaDanteCausa)Session["AreaDanteCausa"];
                }

                bool disabilitaDatiCalcoloSpacchettamentoSOPGI = false;
                if (Utility.IsDomandaSpacchettamentoSOPGIPost072022(this.domanda.Categoria, datiPensione, this.areaDanteCausa))
                {
                    if (Utility.IsDomandaReversibilita(datiPensione) || (Utility.IsDomandaIndiretta(datiPensione) && this.domanda.IsDomandaRiapertura && !this.areaDanteCausa.IsFascicoloGenerato.GetValueOrDefault()))
                    {
                        disabilitaDatiCalcoloSpacchettamentoSOPGI = true;
                    }
                }


                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    if (((((AreaDatiContributivi)ViewState["areaDatiContributiviAgo"]).DatiCalcolo.IsUnicarpe) || (CodeUtility.IsRicostituzione(datiPensione) && !CodeUtility.IsRicostituzioneContributiva(datiPensione) && !Utility.IsRicostituzione_AccreditoPeriodiMaternita(datiPensione) &&
                        !(ViewState["EliminazioneScartoOneri0031_0105_0112"] != null && (string)ViewState["EliminazioneScartoOneri0031_0105_0112"] == "SI" && Utility.IsDomandaBeneficioTerrorismoLegge206_2004(datiPensione)) &&
                        (Utility.IsDomandaFPLD(this.domanda.Categoria) || Utility.IsDomandaGestioneAutonomi(this.domanda.Categoria) || Utility.IsDomandaDAI(this.domanda.Categoria) || Utility.IsDomandaAUT(this.domanda.Categoria) || Utility.IsDomandaRipristino(datiPensione))) ||
                        ((CodeUtility.IsRicostituzione(datiPensione) && areaDatiContributiviAgo.IsEliminataPerCauseVarie && areaDatiContributiviAgo.IsMemo102Abilitato.GetValueOrDefault() && (Utility.IsDomandaVOESO(this.domanda.Categoria) || Utility.IsDomandaVESO33(this.domanda.Categoria) || Utility.IsDomandaVESO92(this.domanda.Categoria) || Utility.IsDomandaVOCOOP_COOP28(this.domanda.Categoria) ||
                        Utility.IsDomandaVOCRED_CRED27(this.domanda.Categoria) || Utility.IsDomandaESPA(this.domanda.Categoria) || Utility.IsDomandaESOTEL(this.domanda.Categoria) || Utility.IsDomandaESOAMB(this.domanda.Categoria) || Utility.IsDomandaVESO29(this.domanda.Categoria))))
                        //ENG - RIC VOPGI NON CONTRIBUTIVE
                        || (Utility.IsRicostituzione(datiPensione) && !Utility.IsRicostituzione_MotiviContributivi(datiPensione) && Utility.IsDomandaVOPGI(this.domanda.Categoria))
                        || disabilitaDatiCalcoloSpacchettamentoSOPGI || Utility.IsRicostituzione_PerVariazioneDatiSupplemento(datiPensione) || (ViewState["AbilitazioneMemo50_2023"] != null && (string)ViewState["AbilitazioneMemo50_2023"] == "SI" && CodeUtility.IsRicostituzioneSupplemento(datiPensione))
                         || (ViewState["AbilitazioneMemo123_2021"] != null && ViewState["AbilitazioneMemo123_2021"].ToString().Trim().ToUpperInvariant() == "SI" && datiPensione.IdTipoPLPerRIC.HasValue && (datiPensione.IdTipoPLPerRIC.Value == 21 || datiPensione.IdTipoPLPerRIC.Value == 26)))) // sola lettura
                    {
                        gvDatiRetributivi.EditIndex = -1;

                        ((Label)e.Row.FindControl("lblCodiceGestione_item")).Text = GetValueFromIdRetr(((DatiRetributiviLocal)(e.Row.DataItem)).Gestione);
                        ((Label)e.Row.FindControl("lblCodiceGestione_item")).Attributes.Add("title", ((Label)e.Row.FindControl("lblCodiceGestione_item")).Text);
                        ((Label)e.Row.FindControl("lblIdCodeGestione")).Text = ((DatiRetributiviLocal)(e.Row.DataItem)).Gestione;
                        ((Label)e.Row.FindControl("lblQuota_item")).Text = ((DatiRetributiviLocal)(e.Row.DataItem)).Quota;
                        ((Label)e.Row.FindControl("lblSettimane")).Text = ((DatiRetributiviLocal)(e.Row.DataItem)).Settimane;
                        ((Label)e.Row.FindControl("lblRetribuzioneMedia")).Text = !string.IsNullOrEmpty(((DatiRetributiviLocal)(e.Row.DataItem)).RetribuzioneMedia) ? CodeUtility.ConvertDecimalFixedPoint(((DatiRetributiviLocal)(e.Row.DataItem)).RetribuzioneMedia, 4) : "";
                        ((Label)e.Row.FindControl("lblSettimane707")).Text = ((DatiRetributiviLocal)(e.Row.DataItem)).Settimane707;
                        ((Label)e.Row.FindControl("lblQuoteRetributivo")).Text = !string.IsNullOrEmpty(((DatiRetributiviLocal)(e.Row.DataItem)).PL_Quotar) ? CodeUtility.ConvertDecimalFixedPoint(((DatiRetributiviLocal)(e.Row.DataItem)).PL_Quotar, 4) : "";
                        ((Label)e.Row.FindControl("lblQuoteRetributivo707")).Text = !string.IsNullOrEmpty(((DatiRetributiviLocal)(e.Row.DataItem)).PL_Quotar707) ? CodeUtility.ConvertDecimalFixedPoint(((DatiRetributiviLocal)(e.Row.DataItem)).PL_Quotar707, 4) : "";
                        LinkButton button = ((LinkButton)(e.Row.Cells[0].Controls[0]));
                        button.Enabled = false;
                        button.Text = "&nbsp;&nbsp;&nbsp;";


                    }
                    else
                    {
                        //prima riga
                        if (e.Row.DataItemIndex == 0)
                        {
                            //vuota
                            if ((IsListaPrecompilata() || IsListaEmpty(true)) && !Convert.ToBoolean(modalitaEditRetributivi.Value))
                            {
                                ManagePulsanti();
                                gvDatiRetributivi.EditIndex = 0;
                                modalitaEditRetributivi.Value = "true";

                                gvDatiRetributivi.DataSource = (List<DatiRetributiviLocal>)ViewState[EnumViewState.ElencoDatiRetributivi.ToString()];
                                gvDatiRetributivi.DataBind();
                            }
                            else if (IsEmptyEditableRowRetrib(e.Row))
                            {
                                if (e.Row.Cells[0].Controls.Count == 3)  // edit mode
                                {
                                    GestioneDdls(e.Row, true);
                                    CodeUtility.EnableEditableMode(e.Row.Cells[0], "UCTabDatiCalcoloAgoRetr", Page.Theme);
                                    LinkButton delete = ((LinkButton)(e.Row.Cells[5].FindControl("btnDeleteRetributivi")));
                                    //delete.Text = string.Empty;
                                    delete.Text = "-";
                                }
                                else
                                {
                                    ((Label)e.Row.FindControl("lblCodiceGestione_item")).Text = GetValueFromIdRetr(((DatiRetributiviLocal)(e.Row.DataItem)).Gestione);
                                    ((Label)e.Row.FindControl("lblCodiceGestione_item")).Attributes.Add("title", ((Label)e.Row.FindControl("lblCodiceGestione_item")).Text);
                                    ((Label)e.Row.FindControl("lblIdCodeGestione")).Text = ((DatiRetributiviLocal)(e.Row.DataItem)).Gestione;
                                    ((Label)e.Row.FindControl("lblQuota_item")).Text = ((DatiRetributiviLocal)(e.Row.DataItem)).Quota;
                                    ((Label)e.Row.FindControl("lblSettimane")).Text = ((DatiRetributiviLocal)(e.Row.DataItem)).Settimane;
                                    ((Label)e.Row.FindControl("lblRetribuzioneMedia")).Text = CodeUtility.ConvertDecimalFixedPoint(((DatiRetributiviLocal)(e.Row.DataItem)).RetribuzioneMedia, 4);
                                    ((Label)e.Row.FindControl("lblSettimane707")).Text = ((DatiRetributiviLocal)(e.Row.DataItem)).Settimane707;
                                    ((Label)e.Row.FindControl("lblQuoteRetributivo")).Text = !string.IsNullOrEmpty(((DatiRetributiviLocal)(e.Row.DataItem)).PL_Quotar) ? CodeUtility.ConvertDecimalFixedPoint(((DatiRetributiviLocal)(e.Row.DataItem)).PL_Quotar, 4) : "";
                                    ((Label)e.Row.FindControl("lblQuoteRetributivo707")).Text = !string.IsNullOrEmpty(((DatiRetributiviLocal)(e.Row.DataItem)).PL_Quotar707) ? CodeUtility.ConvertDecimalFixedPoint(((DatiRetributiviLocal)(e.Row.DataItem)).PL_Quotar707, 4) : "";
                                    if (ViewState[EnumViewState.IsPrimoRecordRetrGestioneS.ToString()] != null && (bool)ViewState[EnumViewState.IsPrimoRecordRetrGestioneS.ToString()])
                                    {
                                        //Eng - Ric Vr, VOART, VOCOM che dal prelievo arrivano con codice gestione I, M o N non devono essere modificabili
                                        if (!string.IsNullOrEmpty(GetValueFromIdRetr(((DatiRetributiviLocal)(e.Row.DataItem)).Gestione)))
                                        {
                                            switch (GetValueFromIdRetr(((DatiRetributiviLocal)(e.Row.DataItem)).Gestione).Substring(0, 1))
                                            {
                                                case "I":
                                                case "M":
                                                case "N":
                                                    LinkButton edit = ((LinkButton)(e.Row.Cells[0].Controls[0]));
                                                    edit.Text = "-";
                                                    edit.Enabled = false;
                                                    break;
                                                default:
                                                    CodeUtility.EnableReadableMode(e.Row.Cells[0], e.Row.Cells[7], Page.Theme, null);
                                                    break;
                                            }
                                        }
                                        else
                                            CodeUtility.EnableReadableMode(e.Row.Cells[0], e.Row.Cells[7], Page.Theme, null);
                                    }
                                    else
                                    {
                                        //Eng - Ric Vr, VOART, VOCOM che dal prelievo arrivano con codice gestione I, M o N non devono essere modificabili
                                        if (!string.IsNullOrEmpty(GetValueFromIdRetr(((DatiRetributiviLocal)(e.Row.DataItem)).Gestione)))
                                        {
                                            switch (GetValueFromIdRetr(((DatiRetributiviLocal)(e.Row.DataItem)).Gestione).Substring(0, 1))
                                            {
                                                case "I":
                                                case "M":
                                                case "N":
                                                    LinkButton edit = ((LinkButton)(e.Row.Cells[0].Controls[0]));
                                                    edit.Text = "-";
                                                    edit.Enabled = false;
                                                    break;
                                                default:
                                                    CodeUtility.EnableReadableMode(e.Row.Cells[0], e.Row.Cells[5], Page.Theme, "btnDeleteRetributivi");
                                                    break;
                                            }
                                        }
                                        else
                                            CodeUtility.EnableReadableMode(e.Row.Cells[0], e.Row.Cells[5], Page.Theme, "btnDeleteRetributivi");
                                    }
                                }

                            }
                            else  //prima riga non vuota
                            {
                                if (e.Row.Cells[0].Controls.Count == 3)  // edit mode
                                {
                                    GestioneDdls(e.Row, true);
                                    CodeUtility.EnableEditableMode(e.Row.Cells[0], "UCTabDatiCalcoloAgoRetr", Page.Theme);
                                    if (ViewState[EnumViewState.IsPrimoRecordRetrGestioneS.ToString()] != null && (bool)ViewState[EnumViewState.IsPrimoRecordRetrGestioneS.ToString()])
                                    {
                                        ((DropDownList)e.Row.FindControl("ddlCodiceGestione")).Enabled = false;
                                        ((DropDownList)e.Row.FindControl("ddlQuota")).Enabled = false;
                                    }
                                }
                                else
                                {
                                    ((Label)e.Row.FindControl("lblCodiceGestione_item")).Text = GetValueFromIdRetr(((DatiRetributiviLocal)(e.Row.DataItem)).Gestione);
                                    ((Label)e.Row.FindControl("lblCodiceGestione_item")).Attributes.Add("title", ((Label)e.Row.FindControl("lblCodiceGestione_item")).Text);
                                    ((Label)e.Row.FindControl("lblIdCodeGestione")).Text = ((DatiRetributiviLocal)(e.Row.DataItem)).Gestione;
                                    ((Label)e.Row.FindControl("lblQuota_item")).Text = ((DatiRetributiviLocal)(e.Row.DataItem)).Quota;
                                    ((Label)e.Row.FindControl("lblSettimane")).Text = ((DatiRetributiviLocal)(e.Row.DataItem)).Settimane;
                                    ((Label)e.Row.FindControl("lblRetribuzioneMedia")).Text = CodeUtility.ConvertDecimalFixedPoint(((DatiRetributiviLocal)(e.Row.DataItem)).RetribuzioneMedia, 4);
                                    ((Label)e.Row.FindControl("lblSettimane707")).Text = ((DatiRetributiviLocal)(e.Row.DataItem)).Settimane707;
                                    ((Label)e.Row.FindControl("lblQuoteRetributivo")).Text = !string.IsNullOrEmpty(((DatiRetributiviLocal)(e.Row.DataItem)).PL_Quotar) ? CodeUtility.ConvertDecimalFixedPoint(((DatiRetributiviLocal)(e.Row.DataItem)).PL_Quotar, 4) : "";
                                    ((Label)e.Row.FindControl("lblQuoteRetributivo707")).Text = !string.IsNullOrEmpty(((DatiRetributiviLocal)(e.Row.DataItem)).PL_Quotar707) ? CodeUtility.ConvertDecimalFixedPoint(((DatiRetributiviLocal)(e.Row.DataItem)).PL_Quotar707, 4) : "";

                                    //Eng - Ric Vr, VOART, VOCOM che dal prelievo arrivano con codice gestione I, M o N non devono essere modificabili
                                    if (!string.IsNullOrEmpty(GetValueFromIdRetr(((DatiRetributiviLocal)(e.Row.DataItem)).Gestione)))
                                    {
                                        switch (GetValueFromIdRetr(((DatiRetributiviLocal)(e.Row.DataItem)).Gestione).Substring(0, 1))
                                        {
                                            case "I":
                                            case "M":
                                            case "N":
                                                LinkButton edit = ((LinkButton)(e.Row.Cells[0].Controls[0]));
                                                edit.Text = "-";
                                                edit.Enabled = false;
                                                break;
                                            default:
                                                CodeUtility.EnableReadableMode(e.Row.Cells[0], e.Row.Cells[4], Page.Theme, "btnDeleteRetributivi");
                                                break;
                                        }
                                    }
                                    else
                                        CodeUtility.EnableReadableMode(e.Row.Cells[0], e.Row.Cells[4], Page.Theme, "btnDeleteRetributivi");
                                }
                            }
                        }
                        else  // righe successive
                        {
                            if (this.domanda == null)
                                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

                            if (this.TitolarePensione == null)
                                this.TitolarePensione = new AreaTitolare();
                            if (this.TitolarePensione.Pensione == null)
                                this.TitolarePensione.Pensione = GetDatiPensione(this);
                            this.domandaDante = this.domanda;
                            this.numDomanda = long.Parse(this.domanda.NumeroDomanda);

                            if (this.areaDanteCausa == null)
                            {
                                PresenterDanteCausa presenterDanteCausa = new PresenterDanteCausa();
                                presenterDanteCausa.GetDatiDanteCausa(this);
                            }

                            if (e.Row.Cells[0].Controls.Count == 3)  // edit mode
                            {
                                GestioneDdls(e.Row, true);
                                CodeUtility.EnableEditableMode(e.Row.Cells[0], "UCTabDatiCalcoloAgoRetr", Page.Theme);
                            }
                            else if (e.Row.DataItemIndex == ((List<DatiRetributiviLocal>)ViewState[EnumViewState.ElencoDatiRetributivi.ToString()]).Count - 1
                                && !Utility.IsDomandaBancRicAnte1991(this.domanda.SiglaCategoriaPensione, this.TitolarePensione.Pensione, this.areaDanteCausa))
                            {
                                LinkButton add = ((LinkButton)(e.Row.Cells[0].Controls[0]));
                                add.Text = (string)"<img width=16 height=16 border=0 src=../App_themes/" + Page.Theme + "/Images/add24.png />";
                                add.ToolTip = "Aggiungi";
                            }
                            else
                            {
                                ((Label)e.Row.FindControl("lblCodiceGestione_item")).Text = GetValueFromIdRetr(((DatiRetributiviLocal)(e.Row.DataItem)).Gestione);
                                ((Label)e.Row.FindControl("lblCodiceGestione_item")).Attributes.Add("title", ((Label)e.Row.FindControl("lblCodiceGestione_item")).Text);
                                ((Label)e.Row.FindControl("lblIdCodeGestione")).Text = ((DatiRetributiviLocal)(e.Row.DataItem)).Gestione;
                                ((Label)e.Row.FindControl("lblQuota_item")).Text = ((DatiRetributiviLocal)(e.Row.DataItem)).Quota;
                                ((Label)e.Row.FindControl("lblSettimane")).Text = ((DatiRetributiviLocal)(e.Row.DataItem)).Settimane;
                                ((Label)e.Row.FindControl("lblRetribuzioneMedia")).Text = CodeUtility.ConvertDecimalFixedPoint(((DatiRetributiviLocal)(e.Row.DataItem)).RetribuzioneMedia, 4);
                                ((Label)e.Row.FindControl("lblSettimane707")).Text = ((DatiRetributiviLocal)(e.Row.DataItem)).Settimane707;
                                ((Label)e.Row.FindControl("lblQuoteRetributivo")).Text = !string.IsNullOrEmpty(((DatiRetributiviLocal)(e.Row.DataItem)).PL_Quotar) ? CodeUtility.ConvertDecimalFixedPoint(((DatiRetributiviLocal)(e.Row.DataItem)).PL_Quotar, 4) : "";
                                ((Label)e.Row.FindControl("lblQuoteRetributivo707")).Text = !string.IsNullOrEmpty(((DatiRetributiviLocal)(e.Row.DataItem)).PL_Quotar707) ? CodeUtility.ConvertDecimalFixedPoint(((DatiRetributiviLocal)(e.Row.DataItem)).PL_Quotar707, 4) : "";

                                //Eng - Ric Vr, VOART, VOCOM che dal prelievo arrivano con codice gestione I, M o N non devono essere modificabili
                                if (!string.IsNullOrEmpty(GetValueFromIdRetr(((DatiRetributiviLocal)(e.Row.DataItem)).Gestione)))
                                {
                                    switch (GetValueFromIdRetr(((DatiRetributiviLocal)(e.Row.DataItem)).Gestione).Substring(0, 1))
                                    {
                                        case "I":
                                        case "M":
                                        case "N":
                                            LinkButton edit = ((LinkButton)(e.Row.Cells[0].Controls[0]));
                                            edit.Text = "-";
                                            edit.Enabled = false;
                                            break;
                                        default:
                                            CodeUtility.EnableReadableMode(e.Row.Cells[0], e.Row.Cells[4], Page.Theme, "btnDeleteRetributivi");
                                            break;
                                    }
                                }
                                else
                                    CodeUtility.EnableReadableMode(e.Row.Cells[0], e.Row.Cells[4], Page.Theme, "btnDeleteRetributivi");
                            }
                        }
                    }
                }
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCDatiRetributiviAgo, Errore nel metodo gvDatiRetributivi_RowDataBound " + ex);
            }
        }

        protected void gvDatiRetributivi_Load(object sender, EventArgs e)
        {
            AreaDatiContributivi areaDatiContributiviAgo = ViewState["areaDatiContributiviAgo"] as AreaDatiContributivi;
            if (this.domanda == null)
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            //ENG - Aggiornamento Memo 68/2022 IOPGI
            if (this.TitolarePensione == null)
                this.TitolarePensione = new AreaTitolare();
            if (this.TitolarePensione.Pensione == null)
                this.TitolarePensione.Pensione = GetDatiPensione(this);

            //ENG - Spacchettate SOPGI
            if (this.areaDanteCausa == null)
            {
                PresenterDanteCausa presenterDanteCausa = new PresenterDanteCausa();
                presenterDanteCausa.GetDatiDanteCausa(this);
            }

            if (areaDatiContributiviAgo != null)
            {
                gvDatiRetributivi.Columns[(int)ColonneGvDatiRetributivi.Sett707].Visible = areaDatiContributiviAgo.IsSettimane707Visible;
                GestisciAnte96(areaDatiContributiviAgo.IsAnte96, areaDatiContributiviAgo.MostraQuotaAnte96);

                //ENG - Aggiornamento Memo 68/2022 IOPGI
                //ENG - Spacchettate SOPGI
                if (Utility.IsDomandaVOPGI(this.domanda.Categoria) || (Utility.IsDomandaIOPGI(this.domanda.Categoria) && !Utility.IsDomandaIOPGI_AGI(this.domanda.Categoria, this.TitolarePensione.Pensione.Filtro))
                    || Utility.IsDomandaSpacchettamentoSOPGIPost072022(this.domanda.Categoria, this.TitolarePensione.Pensione, this.areaDanteCausa))
                {
                    gvDatiRetributivi.Columns[(int)ColonneGvDatiRetributivi.QuoteRet].Visible = true;
                    //if (!areaDatiContributiviAgo.IsDomandaVOPGIFiltroAGI.GetValueOrDefault())
                    //{
                    //    gvDatiRetributivi.Columns[(int)ColonneGvDatiRetributivi.Sett707].Visible = true;
                    //    gvDatiRetributivi.Columns[(int)ColonneGvDatiRetributivi.QuoteRetr707].Visible = true;
                    //}
                    //else
                    //{
                    //    gvDatiRetributivi.Columns[(int)ColonneGvDatiRetributivi.QuoteRetr707].Visible = areaDatiContributiviAgo.IsSettimane707INPGIVisible;
                    //}
                    gvDatiRetributivi.Columns[(int)ColonneGvDatiRetributivi.Sett707].Visible = false;
                    gvDatiRetributivi.Columns[(int)ColonneGvDatiRetributivi.QuoteRetr707].Visible = false;
                }
            }
        }

        private void GestisciAnte96(Presenter.SvrLiquidazioneAgo.UtilityTipoAnte96? IsAnte96, bool? mostraQuotaAnte96)
        {
            switch (IsAnte96)
            {
                case Presenter.SvrLiquidazioneAgo.UtilityTipoAnte96.Ante96Retributive:
                    if (!mostraQuotaAnte96.GetValueOrDefault())
                        gvDatiRetributivi.Columns[1].Visible = false;
                    break;
                case Presenter.SvrLiquidazioneAgo.UtilityTipoAnte96.Ante96Miste:
                    if (!mostraQuotaAnte96.GetValueOrDefault())
                        gvDatiRetributivi.Columns[1].Visible = false;
                    gvDatiContributivi.Columns[1].Visible = false;
                    gvDatiContributivi.Columns[2].Visible = false;
                    break;
                case Presenter.SvrLiquidazioneAgo.UtilityTipoAnte96.Ante96Contributive:
                    gvDatiContributivi.Columns[1].Visible = false;
                    gvDatiContributivi.Columns[2].Visible = false;
                    if (gvDatiRetributivi != null)
                        gvDatiRetributivi.Columns[1].Visible = false;
                    break;
                default:
                    break;
            }
        }

        protected void gvDatiContributivi_Load(object sender, EventArgs e)
        {
            AreaDatiContributivi areaDatiContributiviAgo = ViewState["areaDatiContributiviAgo"] as AreaDatiContributivi;
            if (this.domanda == null)
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            //ENG - Aggiornamento Memo 68/2022 IOPGI
            if (this.TitolarePensione == null)
                this.TitolarePensione = new AreaTitolare();
            if (this.TitolarePensione.Pensione == null)
                this.TitolarePensione.Pensione = GetDatiPensione(this);

            //ENG - Spacchettate SOPGI
            if (this.areaDanteCausa == null)
            {
                PresenterDanteCausa presenterDanteCausa = new PresenterDanteCausa();
                presenterDanteCausa.GetDatiDanteCausa(this);
            }

            if (areaDatiContributiviAgo != null)
            {
                GestisciAnte96(areaDatiContributiviAgo.IsAnte96, areaDatiContributiviAgo.MostraQuotaAnte96);
                //ENG - Aggiornamento Memo 68/2022 IOPGI
                //ENG - Spacchettate SOPGI
                if (Utility.IsDomandaVOPGI(this.domanda.Categoria) || (Utility.IsDomandaIOPGI(this.domanda.Categoria) && !Utility.IsDomandaIOPGI_AGI(this.domanda.Categoria, this.TitolarePensione.Pensione.Filtro))
                    || Utility.IsDomandaSpacchettamentoSOPGIPost072022(this.domanda.Categoria, this.TitolarePensione.Pensione, this.areaDanteCausa))
                {
                    gvDatiContributivi.Columns[(int)ColonneGvDatiContributivi.QuotaContr].Visible = true;
                }
            }
        }

        private void ManageUpdateGridVittimeTerrorismoRetributivi()
        {
            Presenter.SvrLiquidazioneAgo.AreaDatiContributivi area = (Presenter.SvrLiquidazioneAgo.AreaDatiContributivi)ViewState["areaDatiContributiviAgo"];
            if (area != null && area.IsDatiRetributiviVittimeVisible)
                RaiseUpdateDatiCalcoloTerrorismoRetributivi(this, new EventArgs());
        }

        protected void gvDatiRetributivi_DataBound(object sender, EventArgs e)
        {
            ManagePulsanti();
        }

        protected void gvDatiRetributivi_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            try
            {
                gvDatiRetributivi.EditIndex = -1;
                gvDatiRetributivi.PageIndex = e.NewPageIndex;
                List<DatiRetributiviLocal> elencoDatiRetributivi = (List<DatiRetributiviLocal>)ViewState[EnumViewState.ElencoDatiRetributivi.ToString()];
                gvDatiRetributivi.DataSource = elencoDatiRetributivi;
                gvDatiRetributivi.DataBind();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new DnaApplicationException("UCDatiContributiviAgo, Errore nel metodo gvDatiRetributivi_PageIndexChanging" + ex);
            }
        }

        #endregion gvDatiRetributivi

        #region EventHandler

        public event EventHandler ShowAvviso;
        public event EventHandler HideAvviso;
        public event EventHandler InitializeData;
        public event EventHandler AbilitaPopUpDatiContributivi;
        public event EventHandler DisabilitaPopUpDatiContributivi;
        public event EventHandler GestisciTastoSalva;
        public event EventHandler UpdateDatiCalcoloTerrorismoRetributivi;
        public event EventHandler UpdateDatiCalcoloTerrorismoContributivi;

        public void RaiseUpdateDatiCalcoloTerrorismoContributivi(object sender, EventArgs args)
        {
            if (UpdateDatiCalcoloTerrorismoContributivi != null)
                UpdateDatiCalcoloTerrorismoContributivi(sender, args);
        }

        public void RaiseUpdateDatiCalcoloTerrorismoRetributivi(object sender, EventArgs args)
        {
            if (UpdateDatiCalcoloTerrorismoRetributivi != null)
                UpdateDatiCalcoloTerrorismoRetributivi(sender, args);
        }

        protected void RaiseGestisciTastoSalva(object sender, EventArgs e)
        {
            if (GestisciTastoSalva != null)
                GestisciTastoSalva(sender, e);
        }

        protected void RaiseAbilitaPopUpDatiContributivi(object sender, EventArgs e)
        {
            AbilitaPopUpDatiContributivi(sender, e);
        }

        protected void RaiseDisabilitaPopUpDatiContributivi(object sender, EventArgs e)
        {
            DisabilitaPopUpDatiContributivi(sender, e);
        }

        protected void RaiseInitializeData(object sender, EventArgs e)
        {
            InitializeData(sender, e);
        }

        protected void RaiseShowAvviso(object sender, EventArgs e)
        {
            ShowAvviso(sender, e);
        }

        protected void RaiseHideAvviso(object sender, EventArgs e)
        {
            if (HideAvviso != null)
                HideAvviso(sender, e);
        }

        #endregion EventHandler

        public void SetPopUpContributivi(AreaTitolare.DatiPensione datiPensione, List<DatiContributiviLocal> lstDatiContributivi)
        {
            if (datiPensione.DecorrenzaOriginaria >= new DateTime(2015, 1, 1) && datiPensione.TipoLetturaUnicarpe != 'L')
            {
                DatiContributiviLocal recordContrib = lstDatiContributivi.Where(x => (x.Quota ?? "").Trim() == "C").FirstOrDefault();
                Presenter.SvrLiquidazioneAgo.UtilityTipoAnte96? IsAnte96 = null;

                if (areaDatiContributiviAgo != null) IsAnte96 = areaDatiContributiviAgo.IsAnte96;
                if (recordContrib != null && IsAnte96 == null)
                {
                    decimal montante;
                    decimal ammontare;
                    if (decimal.TryParse(recordContrib.MontanteContributivo, out montante) &&
                        decimal.TryParse(recordContrib.AmmontareContributivo, out ammontare) &&
                        montante < ammontare)
                    {
                        btnPopUp.Style.Add("display", "inline-block");
                        btnSalvaDatiCalcolo.Style.Add("display", "none");
                        RaiseAbilitaPopUpDatiContributivi(this, null);
                    }
                    else
                    {
                        btnPopUp.Style.Add("display", "none");
                        btnSalvaDatiCalcolo.Style.Add("display", "inline-block");
                        RaiseDisabilitaPopUpDatiContributivi(this, null);
                    }
                }
                else
                {
                    btnPopUp.Style.Add("display", "none");
                    btnSalvaDatiCalcolo.Style.Add("display", "inline-block");
                    RaiseDisabilitaPopUpDatiContributivi(this, null);
                }

            }

        }

        public bool controlsDatiCRED27_VOCRED_GestioneL(GridViewRow row)
        {
            if (isDomandaCRED27_VOCRED_GestioneL())
            {
                if (row.FindControl("ddlCodiceGestione") != null && ((DropDownList)row.FindControl("ddlCodiceGestione")).SelectedIndex != 0
                    && row.FindControl("txtMontanteContributivo") != null && ((TextBox)row.FindControl("txtMontanteContributivo")).Text != string.Empty)
                    return true;
                else
                    return false;
            }

            return false;
        }

        public bool controlsDatiVOPGI(GridViewRow row, string categoria)
        {
            if (Utility.IsDomandaVOPGI(categoria))
            {
                if (row.FindControl("txtQuotaContributiva") != null && ((TextBox)row.FindControl("txtQuotaContributiva")).Text != string.Empty)
                    return true;
                else
                    return false;
            }
            return true;
        }

        private void PrevalorizzaRecordRetributivo(List<DatiRetributiviLocal> elencoDatiRetributivi, AreaTitolare.DatiPensione datiPensione, bool isAnte96Misto)
        {
            if (elencoDatiRetributivi == null || elencoDatiRetributivi.Count == 0)
            {
                DatiRetributiviLocal dati = new DatiRetributiviLocal();
                string gestione = string.Empty;

                //Prevalorizza con S le domande da bonuscon la proprietà IsPrimoRecordRetrGestioneS impostata
                List<DecodificaGestioneCalcoloRetributivo> listaCodeGestioneCalcoloRetrib = ((DecodificaGestioneCalcoloRetributivo[])ViewState["listaCodeGestioneCalcoloRetrib"]).ToList();
                DecodificaGestioneCalcoloRetributivo app = null;
                if (ViewState[EnumViewState.IsPrimoRecordRetrGestioneS.ToString()] != null && (bool)ViewState[EnumViewState.IsPrimoRecordRetrGestioneS.ToString()])
                    app = listaCodeGestioneCalcoloRetrib.ToList().Find(delegate(DecodificaGestioneCalcoloRetributivo code) { return (code.TraduzioneSuGP.Trim() == "S"); });
                else
                {
                    DatiRetributiviLocal Empty = elencoDatiRetributivi.Find(delegate(DatiRetributiviLocal code)
                    {
                        return (code.Decorrenza == string.Empty && code.Gestione == string.Empty && code.RetribuzioneMedia == string.Empty &&
                                code.Settimane == string.Empty && code.Quota == string.Empty);
                    });

                    if (Empty == null && (!((AreaDatiContributivi)ViewState["areaDatiContributiviAgo"]).DatiCalcolo.IsUnicarpe && !(!CodeUtility.IsRicostituzioneContributiva(datiPensione) && !Utility.IsRicostituzione_AccreditoPeriodiMaternita(datiPensione) &&
                        !(ViewState["EliminazioneScartoOneri0031_0105_0112"] != null && (string)ViewState["EliminazioneScartoOneri0031_0105_0112"] == "SI" && Utility.IsDomandaBeneficioTerrorismoLegge206_2004(datiPensione)) &&
                        CodeUtility.IsRicostituzione(datiPensione) && (Utility.IsDomandaFPLD(this.domanda.Categoria) || Utility.IsDomandaGestioneAutonomi(this.domanda.Categoria) || Utility.IsDomandaDAI(this.domanda.Categoria) ||
                        Utility.IsDomandaAUT(this.domanda.Categoria))) || isAnte96Misto))
                        elencoDatiRetributivi.Add(new DatiRetributiviLocal(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty));
                    return;
                }

                if (app != null)
                    gestione = app.Id.ToString();

                dati.Gestione = gestione;
                dati.Quota = "A";

                if (elencoDatiRetributivi == null)
                    elencoDatiRetributivi = new List<DatiRetributiviLocal>();
                elencoDatiRetributivi.Add(dati);
            }
            else
            {
                DatiRetributiviLocal Empty = elencoDatiRetributivi.Find(delegate(DatiRetributiviLocal code)
                {
                    return (code.Decorrenza == string.Empty && code.Gestione == string.Empty && code.RetribuzioneMedia == string.Empty &&
                            code.Settimane == string.Empty && code.Quota == string.Empty);
                });

                if (Empty == null && (!((AreaDatiContributivi)ViewState["areaDatiContributiviAgo"]).DatiCalcolo.IsUnicarpe && !(!CodeUtility.IsRicostituzioneContributiva(datiPensione) && !Utility.IsRicostituzione_AccreditoPeriodiMaternita(datiPensione) &&
                    !(ViewState["EliminazioneScartoOneri0031_0105_0112"] != null && (string)ViewState["EliminazioneScartoOneri0031_0105_0112"] == "SI" && Utility.IsDomandaBeneficioTerrorismoLegge206_2004(datiPensione)) &&
                    CodeUtility.IsRicostituzione(datiPensione) && (Utility.IsDomandaFPLD(this.domanda.Categoria) || Utility.IsDomandaGestioneAutonomi(this.domanda.Categoria) || Utility.IsDomandaDAI(this.domanda.Categoria) ||
                    Utility.IsDomandaAUT(this.domanda.Categoria))) || isAnte96Misto))
                    elencoDatiRetributivi.Add(new DatiRetributiviLocal(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty));
            }

            ViewState[EnumViewState.ElencoDatiRetributivi.ToString()] = elencoDatiRetributivi;
        }

        private bool IsListaPrecompilata()
        {
            List<DatiRetributiviLocal> listaDatiRetrApp = (List<DatiRetributiviLocal>)ViewState[EnumViewState.ElencoDatiRetributivi.ToString()];
            if (listaDatiRetrApp != null && listaDatiRetrApp.Count == 1 && string.IsNullOrEmpty(listaDatiRetrApp[0].Decorrenza) &&
                !string.IsNullOrEmpty(listaDatiRetrApp[0].Gestione) && !string.IsNullOrEmpty(listaDatiRetrApp[0].Quota) &&
                string.IsNullOrEmpty(listaDatiRetrApp[0].RetribuzioneMedia) && string.IsNullOrEmpty(listaDatiRetrApp[0].Settimane))
                return true;
            else
                return false;
        }

        #region Enums
        public enum EnumViewState
        {
            ElencoDatiRetributivi,
            ElencoDatiContributivi,
            IsPrimoRecordRetrGestioneS
        }
        #endregion Enums

        #region Keys
        public class Keys
        {
            public const string HdnGUID = "hdnGUID";
            public const string DatiContributivi_DdlQuota = "ddlQuota";
            public const string DatiContribtutivi_TxtSettimaneContributive = "txtSettimaneContributive";
            public const string DatiContributivi_TxtAmmontareContributivo = "txtAmmontareContributivo";
            public const string DatiContributivi_TxtMontanteContributivo = "txtMontanteContributivo";
            public const string DatiContributivi_DdlCodiceGestione = "ddlCodiceGestione";
            public const string DatiRetributivi_DdlCodiceGestione = "ddlCodiceGestione";
            public const string DatiRetributivi_DdlQuota = "ddlQuota";
            public const string DatiRetributivi_LblDecorrenza = "lblDecorrenza";
            public const string DatiRetributivi_TxtSettimaneRetributive = "txtSettimaneRetributive";
            public const string DatiRetributivi_TxtRetribuzioneMedia = "txtRetribuzioneMedia";
            public const string DatiRetributivi_TxtSettimaneRetributive707 = "txtSettimaneRetributive707";
            public const string DatiRetributivi_TxtQuoteRetributivo = "txtQuoteRetributivo";
            public const string DatiRetributivi_TxtQuoteRetributivo707 = "txtQuoteRetributivo707";
            public const string DatiContributivi_TxtQuotaContributiva = "txtQuotaContributiva";
        }
        #endregion Keys
    }

    #region nested Class
    [Serializable]
    public class DatiContributiviLocal
    {
        public DatiContributiviLocal()
        {
            this.Id = Guid.NewGuid();
        }
        public DatiContributiviLocal(string strGestione, string strQuota, string strSettimane, string strAmmontareContributivo, string strMontanteContributivo, string strPL_Quotac, DateTime? DecorrenzaCalcoloContibutivo)
        {
            this.Id = Guid.NewGuid();
            this._strQuota = strQuota;
            this._strAmmontareContributivo = strAmmontareContributivo;
            this._strGestione = strGestione;
            this._strMontanteContributivo = strMontanteContributivo;
            this._strSettimane = strSettimane;
            this._strPL_Quotac = strPL_Quotac;
            this._DecorrenzaCalcoloContibutivo = DecorrenzaCalcoloContibutivo;
        }
        #region private properties
        private string _strQuota;
        private string _strGestione;
        private string _strSettimane;
        private string _strAmmontareContributivo;
        private string _strMontanteContributivo;
        private string _strPL_Quotac;
        private DateTime? _DecorrenzaCalcoloContibutivo;
        #endregion private properties

        #region public properties
        public Guid Id { get; set; }
        public string Quota { get { return _strQuota; } set { _strQuota = value; } }
        public string Gestione { get { return _strGestione; } set { _strGestione = value; } }
        public string Settimane { get { return _strSettimane; } set { _strSettimane = value; } }
        public string AmmontareContributivo { get { return _strAmmontareContributivo; } set { _strAmmontareContributivo = value; } }
        public string MontanteContributivo { get { return _strMontanteContributivo; } set { _strMontanteContributivo = value; } }
        public string PL_Quotac { get { return _strPL_Quotac; } set { _strPL_Quotac = value; } }

        public DateTime? DecorrenzaCalcoloContibutivo { get { return _DecorrenzaCalcoloContibutivo; } set { _DecorrenzaCalcoloContibutivo = value; } }
        #endregion public properties

    }

    [Serializable]
    public class DatiRetributiviLocal
    {
        public DatiRetributiviLocal()
        {
            this.Id = Guid.NewGuid();
        }
        public DatiRetributiviLocal(string strGestione, string strQuota, string strSettimane, string strDecorrenza, string strRetribuzioneMedia, string strSettimane707, string strPL_Quotar, string strPL_Quotar707, string strRMS = null,
            decimal? strRMSExCombattente = null, int? strNSettAnzianitaVV = null, int? strNSettimaneExCombattente = null)
        {
            this.Id = Guid.NewGuid();
            this._strQuota = strQuota;
            this._strGestione = strGestione;
            this._strDecorrenza = strDecorrenza;
            this._strSettimane = strSettimane;
            this._strRetribuzioneMedia = strRetribuzioneMedia;
            this._settimane707 = strSettimane707;
            this._strPL_Quotar = strPL_Quotar;
            this._strPL_Quotar707 = strPL_Quotar707;
            this._strRMS = strRMS;
            this._strRMSExCombattente = strRMSExCombattente;
            this._strNSettAnzianitaVV = strNSettAnzianitaVV;
            this._strNSettimaneExCombattente = strNSettimaneExCombattente;
        }

        #region private properties
        private string _strGestione;
        private string _strQuota;
        private string _strSettimane;
        private string _strDecorrenza;
        private string _strRetribuzioneMedia;
        private string _settimane707;
        private string _strPL_Quotar;
        private string _strPL_Quotar707;
        private string _strRMS;
        private decimal? _strRMSExCombattente;
        private int? _strNSettAnzianitaVV;
        private int? _strNSettimaneExCombattente;

        #endregion private properties

        #region public properties
        public Guid Id { get; set; }
        public string Gestione { get { return _strGestione; } set { _strGestione = value; } }
        public string Quota { get { return _strQuota; } set { _strQuota = value; } }
        public string Settimane { get { return _strSettimane; } set { _strSettimane = value; } }
        public string Decorrenza { get { return _strDecorrenza; } set { _strDecorrenza = value; } }
        public string RetribuzioneMedia { get { return _strRetribuzioneMedia; } set { _strRetribuzioneMedia = value; } }
        public string Settimane707 { get { return _settimane707; } set { _settimane707 = value; } }
        public string PL_Quotar { get { return _strPL_Quotar; } set { _strPL_Quotar = value; } }
        public string PL_Quotar707 { get { return _strPL_Quotar707; } set { _strPL_Quotar707 = value; } }
        public string RMS { get { return _strRMS; } set { _strRMS = value; } }
        public decimal? RMSExCombattente { get { return _strRMSExCombattente; } set { _strRMSExCombattente = value; } }
        public int? NSettAnzianitaVV { get { return _strNSettAnzianitaVV; } set { _strNSettAnzianitaVV = value; } }
        public int? NSettimaneExCombattente { get { return _strNSettimaneExCombattente; } set { _strNSettimaneExCombattente = value; } }
        #endregion public properties

    }

    #endregion nested Class

}
