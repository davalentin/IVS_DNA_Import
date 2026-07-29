using System;
using System.Linq;
using System.Web.UI;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using INPS.Pensioni.LiquidazionePensione.Presenter;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneAgo;
using System.Collections.Generic;
using INPS.Pensioni.LiquidazionePensione.Presenter.Enum;

namespace INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.LiquidazionePensioneAgo
{
    public partial class UCDatiGenerici : CustomBaseUserControl, ITitolarePensione, ILiquidazionePensioneAgo, IDanteCausa
    {
        #region IViewUI
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        #endregion IViewUI

        #region ILiquidazionePensioneAgo
        public AreaLiquidazionePensione areaLiquidazionePensioneAgo { get; set; }
        public Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda { get; set; }
        #endregion ILiquidazionePensioneAgo

        #region ITitolare
        public AreaTitolare TitolarePensione { get; set; }
        #endregion ITitolare

        #region IDanteCausa
        public AreaDanteCausa areaDanteCausa { get; set; }
        public AreaRispostaRiepilogo.DatiRiepilogoDomanda domandaDante { get; set; }
        public long numDomanda { get; set; }
        #endregion IDanteCausa

        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            SetHiddenFieldIsRicostituzione();

            AreaTitolare.DatiPensione datiPensione = GetDatiPensione(this);

            HiddenFieldSiglaCategoria.Value = this.domanda.Categoria.Trim();
            HiddenFieldFiltro.Value = datiPensione.Filtro;

            if (!Page.IsPostBack)
            {
            }
            BindClick();
            AddInputClass();
        }

        protected void SalvaDatiGenerici_Click(Object sender, EventArgs e)
        {
            areaLiquidazionePensioneAgo = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneAgo.AreaLiquidazionePensione();
            areaLiquidazionePensioneAgo.DatiGenerici = GetDatiGenerici();

            Presenter.PresenterLiquidazionePensione presenterLiquidazione = new PresenterLiquidazionePensione();
            presenterLiquidazione.SalvaDatiGenericiAgo(this);


            if (HasError)
                ManageCodNatura();

            RaiseShowAvviso(this, null);
        }

        protected void EliminaDatiGenerici_Click(Object sender, EventArgs e)
        {
            Presenter.PresenterLiquidazionePensione presenterLiquidazione = new PresenterLiquidazionePensione();
            presenterLiquidazione.EliminaDatiGenericiAgo(this);

            if (!this.HasError)
            {
                ClearForm();
                ValorizzaEtichetteDatiGenerici(this);
            }

            RaiseShowAvvisoElimina(this, null);
        }

        internal void ValorizzaEtichetteDatiGenerici(ILiquidazionePensioneAgo liquidazioneAgo)
        {
            if (TitolarePensione == null)
                TitolarePensione = new AreaTitolare();
            TitolarePensione.Pensione = GetDatiPensione(this);

            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

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

            //ENG - Memo 28/2024
            string ctrlAbilitazioneMemo28 = string.Empty;
            if (ViewState["AbilitazioneMemo28_2024"] != null)
                ctrlAbilitazioneMemo28 = (string)ViewState["AbilitazioneMemo28_2024"];
            else
            {
                Presenter.PresenterControlliDinamici presenter = new PresenterControlliDinamici();
                Presenter.SvrLiquidazione.AreaEsito esito = presenter.GetControlloDinamicoByNomeControllo("AbilitazioneMemo28_2024", out ctrlAbilitazioneMemo28);
                if (esito != null && esito.RisultatoOperazione == Presenter.SvrLiquidazione.AreaEsito.TipoEsito.OK)
                    ViewState["AbilitazioneMemo28_2024"] = ctrlAbilitazioneMemo28;
            }

            //ENG - Memo 123/2024 
            string controlloDinamicoMemo123_2024 = string.Empty;
            if (ViewState["AbilitazioneRIC_TRFMemo123_2024"] != null)
                controlloDinamicoMemo123_2024 = (string)ViewState["AbilitazioneRIC_TRFMemo123_2024"];
            else
            {
                Presenter.PresenterControlliDinamici pres = new PresenterControlliDinamici();
                Presenter.SvrLiquidazione.AreaEsito esit = pres.GetControlloDinamicoByNomeControllo("AbilitazioneRIC_TRFMemo123_2024", out controlloDinamicoMemo123_2024);
                if (esit != null && esit.RisultatoOperazione == Presenter.SvrLiquidazione.AreaEsito.TipoEsito.OK)
                    ViewState["AbilitazioneRIC_TRFMemo123_2024"] = controlloDinamicoMemo123_2024;
            }

            //ENG - Memo 123/2024 
            string controlloDinamicoMemo123_2024OpzioneContrib = string.Empty;
            if (ViewState["AbilitazioneRIC_TRFMemo123_2024OpzioneContrib"] != null)
                controlloDinamicoMemo123_2024OpzioneContrib = (string)ViewState["AbilitazioneRIC_TRFMemo123_2024OpzioneContrib"];
            else
            {
                Presenter.PresenterControlliDinamici pres = new PresenterControlliDinamici();
                Presenter.SvrLiquidazione.AreaEsito esit = pres.GetControlloDinamicoByNomeControllo("AbilitazioneRIC_TRFMemo123_2024OpzioneContrib", out controlloDinamicoMemo123_2024OpzioneContrib);
                if (esit != null && esit.RisultatoOperazione == Presenter.SvrLiquidazione.AreaEsito.TipoEsito.OK)
                    ViewState["AbilitazioneRIC_TRFMemo123_2024OpzioneContrib"] = controlloDinamicoMemo123_2024OpzioneContrib;
            }

            //ENG - Spacchettate SOPGI
            if (this.areaDanteCausa == null)
            {
                PresenterDanteCausa presenterDanteCausa = new PresenterDanteCausa();
                presenterDanteCausa.GetDatiDanteCausa(this);
            }

            CodeUtility.TipologiaPensioneGruppo tipologiaGruppoPensione = CodeUtility.TipologiaPensioneGruppo.gr_NessunValore;
            CodeUtility.TipologiaPensioneProdotto tipologiaProdottoPensione = CodeUtility.TipologiaPensioneProdotto.pr_NessunValore;
            CodeUtility.TipologiaPensioneTipo tipologiaTipoPensione = CodeUtility.TipologiaPensioneTipo.tp_NessunValore;
            CodeUtility.GetTipologiaPensione(TitolarePensione.Pensione.CodeGruppo, TitolarePensione.Pensione.CodeProdotto, TitolarePensione.Pensione.CodeTipo, out tipologiaGruppoPensione, out tipologiaProdottoPensione, out tipologiaTipoPensione);

            LoadDdl(liquidazioneAgo, TitolarePensione.Pensione);

            RenderControls(TitolarePensione.Pensione, liquidazioneAgo);

            ManageTrasformazioneAOI(TitolarePensione.Pensione);
            ManageCheckBenefici(TitolarePensione.Pensione, liquidazioneAgo);
            ManageCodiceLiquidazione(TitolarePensione.Pensione);
            ManageConfermeInvalidita(TitolarePensione.Pensione, this.domanda);
            ManageModalitaLiquidazione(TitolarePensione.Pensione);
            ManageCodiceMobilita(TitolarePensione.Pensione, this.domanda.IsDomandaENPALS);
            ManageTipoCalcolo(this.domanda.Categoria, TitolarePensione.Pensione, liquidazioneAgo);

            CodeUtility.DisableEliminaForRicostituzioni(btnEliminaDatiGenerici);

            if (liquidazioneAgo != null && liquidazioneAgo.areaLiquidazionePensioneAgo != null)
                CodeUtility.ManagePanelEsenzioneFiscaleAGO_CI(ref pnlEsenzioneFiscale, liquidazioneAgo.areaLiquidazionePensioneAgo.IsEsenzioneFiscaleEstero.Value, TitolarePensione.Pensione.CodeGruppo, this.domanda.IsDomandaRiapertura);

            if (TitolarePensione.Pensione.DecorrenzaOriginaria.HasValue)
                lblDecorrenzaPensioneDatiGenerici.Text = TitolarePensione.Pensione.DecorrenzaOriginaria.ToString().Substring(3, 7);

            if (TitolarePensione.Pensione.DecorrenzaOriginaria != null)
            {
                string inputDecorrenza = TitolarePensione.Pensione.DecorrenzaOriginaria.ToString();
                if (Utility.IsDomandaIOCUM(this.domanda.Categoria))
                    lblDecorrenzaPensioneDatiGenerici.Text = inputDecorrenza.Substring(0, 10);
                else
                    lblDecorrenzaPensioneDatiGenerici.Text = inputDecorrenza.Substring(3, 7);
            }

            if (TitolarePensione.Pensione.DataPresentazioneDomanda != null)
                txtDataDomanda.Text = string.Format("{0:dd/MM/yyyy}", TitolarePensione.Pensione.DataPresentazioneDomanda.Value);

            if (liquidazioneAgo != null && liquidazioneAgo.areaLiquidazionePensioneAgo != null && liquidazioneAgo.areaLiquidazionePensioneAgo.DatiGenerici != null)
            {
                if (liquidazioneAgo.areaLiquidazionePensioneAgo.IsRichiestaBonusBookingAbilitata.GetValueOrDefault() || liquidazioneAgo.areaLiquidazionePensioneAgo.IsRichiestaBonus154Abilitata.GetValueOrDefault())
                {
                    ManageRichiestaBonus(TitolarePensione.Pensione, liquidazioneAgo.areaLiquidazionePensioneAgo.DatiGenerici);
                }
                ddlCodNatura1DG.ClearSelection();
                if (!string.IsNullOrEmpty(liquidazioneAgo.areaLiquidazionePensioneAgo.DatiGenerici.NaturaPensione))
                {
                    //codice natura 1
                    if (ddlCodNatura1DG.Items.FindByValue(liquidazioneAgo.areaLiquidazionePensioneAgo.DatiGenerici.NaturaPensione.Substring(0, 1)) != null)
                        ddlCodNatura1DG.SelectedValue = liquidazioneAgo.areaLiquidazionePensioneAgo.DatiGenerici.NaturaPensione.Substring(0, 1);
                    //codice natura 2
                    if (ddlCodNatura2DG.Items.FindByValue(liquidazioneAgo.areaLiquidazionePensioneAgo.DatiGenerici.NaturaPensione.Substring(1, 1)) != null)
                        ddlCodNatura2DG.SelectedValue = liquidazioneAgo.areaLiquidazionePensioneAgo.DatiGenerici.NaturaPensione.Substring(1, 1);
                    //codice natura 3
                    if (ddlCodNatura3DG.Items.FindByValue(liquidazioneAgo.areaLiquidazionePensioneAgo.DatiGenerici.NaturaPensione.Substring(2, 1)) != null)
                        ddlCodNatura3DG.SelectedValue = liquidazioneAgo.areaLiquidazionePensioneAgo.DatiGenerici.NaturaPensione.Substring(2, 1);
                }
                //Pensioni in regime di cumulo
                if (Utility.IsDomandaCumulo(this.domanda.Categoria))
                {
                    if (isVOCUM_C9A(TitolarePensione.Pensione, this.domanda))
                    {
                        ddlContributivoCum.SelectedValue = "8";
                        ddlContributivoCum.Enabled = false;
                    }
                    else
                    {
                        ddlContributivoCum.SelectedValue = liquidazioneAgo.areaLiquidazionePensioneAgo.DatiGenerici.Contributivo.ToString();
                    }
                    
                    //ENG - Domanda VOCUM codice natura 3zo byte V bloccato
                    if (!string.IsNullOrEmpty(liquidazioneAgo.areaLiquidazionePensioneAgo.DatiGenerici.NaturaPensione))
                    {
                        if (liquidazioneAgo.areaLiquidazionePensioneAgo.DatiGenerici.NaturaPensione.Substring(2, 1) == "V")
                            ddlCodNatura3DG.Enabled = false;
                    }
                    if (liquidazioneAgo.areaLiquidazionePensioneAgo.DatiGenerici.EnteCassa.HasValue)
                        ddlEnteCassa.SelectedValue = liquidazioneAgo.areaLiquidazionePensioneAgo.DatiGenerici.EnteCassa.ToString();

                    if (!(Utility.IsDomandaIOCUM(this.domanda.Categoria) && tipologiaProdottoPensione == CodeUtility.TipologiaPensioneProdotto.pr_InabilitaPensione &&
                        (tipologiaTipoPensione == CodeUtility.TipologiaPensioneTipo.tp_Inabilita_Ordinaria || tipologiaTipoPensione == CodeUtility.TipologiaPensioneTipo.tp_Inabilita_Art2_C12_Legge335)))
                    {
                        if (liquidazioneAgo.areaLiquidazionePensioneAgo.DatiGenerici.EnteIstruttoreExInpdap.HasValue)
                            ddlEnteIstruttoreFondoExInpdap.SelectedValue = liquidazioneAgo.areaLiquidazionePensioneAgo.DatiGenerici.EnteIstruttoreExInpdap.ToString().ToLowerInvariant();
                        else
                            ddlEnteIstruttoreFondoExInpdap.SelectedIndex = 2;


                        if (this.domanda.Categoria.Trim().ToUpperInvariant() == "IOCUM" && 
                            (TitolarePensione.Pensione.CodeGruppo == "0002" && TitolarePensione.Pensione.CodeProdotto == "0012" &&
                            TitolarePensione.Pensione.CodeTipo == "0047"))
                        {
                            ddlEnteIstruttoreFondoExInpdap.Enabled = false;
                            ddlEnteIstruttoreFondoExInpdap.SelectedIndex = 1;
                        }
                    }
                    else
                    {
                        ddlEnteIstruttoreFondoExInpdap.Enabled = false;
                        if (tipologiaTipoPensione == CodeUtility.TipologiaPensioneTipo.tp_Inabilita_Ordinaria)
                            ddlEnteIstruttoreFondoExInpdap.SelectedIndex = 2;
                        else if (tipologiaTipoPensione == CodeUtility.TipologiaPensioneTipo.tp_Inabilita_Art2_C12_Legge335)
                            ddlEnteIstruttoreFondoExInpdap.SelectedIndex = 1;
                    }

                    if (!((TitolarePensione.Pensione.IsDomandaQuota100OrRicostituzione || TitolarePensione.Pensione.IsDomandaQuota102OrRicostituzione || TitolarePensione.Pensione.IsDomandaAnticipataFlessibileOrRicostituzione || TitolarePensione.Pensione.IsDomandaAnticipataFlessibileOpzioneContributivoOrRicostituzione) && !TitolarePensione.Pensione.IsDomandaCumuloAutomatica))
                    {
                        if (liquidazioneAgo.areaLiquidazionePensioneAgo.DatiGenerici.TipoCumulo.HasValue)
                            ddlTipoCumulo.SelectedValue = liquidazioneAgo.areaLiquidazionePensioneAgo.DatiGenerici.TipoCumulo.Value.ToString();
                        if (liquidazioneAgo.areaLiquidazionePensioneAgo.DatiGenerici.CumuloEsterno.HasValue)
                            ddlCumuloEsterno.SelectedValue = liquidazioneAgo.areaLiquidazionePensioneAgo.DatiGenerici.CumuloEsterno.Value.ToString();
                    }
                    lblEnteIstruttore.InnerText = "Ente istruttore fondo ex INPDAP/IPOST/FFSS";

                    if (CodeUtility.IsRicostituzioneOrRiapertura(TitolarePensione.Pensione, this.domanda.IsDomandaRiapertura))
                    {
                        HiddenFieldIsRicTfrTotCum.Value = "SI";
                        ddlEnteCassa.Enabled = false;
                    }
                }
                else if (Utility.IsDomandaTotalizzazione(this.domanda.Categoria))
                {
                    ddlContributivoCum.SelectedValue = liquidazioneAgo.areaLiquidazionePensioneAgo.DatiGenerici.Contributivo.ToString();
                    if (liquidazioneAgo.areaLiquidazionePensioneAgo.DatiGenerici.EnteCassa.HasValue)
                    {
                        ddlEnteCassa.SelectedValue = liquidazioneAgo.areaLiquidazionePensioneAgo.DatiGenerici.EnteCassa.ToString();
                        if (Utility.IsDomandaVOTOT(this.domanda.Categoria) || TitolarePensione.Pensione.IsDomandaCumuloAutomatica)
                            ddlEnteCassa.Enabled = false;

                        if (CodeUtility.IsRicostituzioneOrRiapertura(TitolarePensione.Pensione, this.domanda.IsDomandaRiapertura))
                        {
                            HiddenFieldIsRicTfrTotCum.Value = "SI";
                            ddlEnteCassa.Enabled = false;
                            chkExCombattente.Enabled = false;
                            chkBenefici.Enabled = false;
                            HiddenFieldChkBeneficiDisabled.Value = "true";
                        }
                    }

                    ddlEnteIstruttoreFondoExInpdap.Visible = false;
                    lblEnteIstruttore.Visible = false;
                }
                else if (Utility.IsDomandaUnicarpe(TitolarePensione.Pensione, true) != Utility.TipoUnicarpe.Automatica && Utility.IsDomandaDAI(this.domanda.Categoria) && CodeUtility.IsRicostituzione(TitolarePensione.Pensione) && !liquidazioneAgo.areaLiquidazionePensioneAgo.IsDatiRetributiviPresenti.GetValueOrDefault() && !liquidazioneAgo.areaLiquidazionePensioneAgo.IsDatiContributiviPresenti.GetValueOrDefault())
                {
                    pnlTipoCalcolo.Visible = false;
                }
                //pensioni normali
                else
                {
                    if (liquidazioneAgo.areaLiquidazionePensioneAgo.DatiGenerici.TipoCalcolo.HasValue)
                        ddlTipoCalcolo.SelectedValue = liquidazioneAgo.areaLiquidazionePensioneAgo.DatiGenerici.TipoCalcolo.Value.ToString();
                    else
                        ddlTipoCalcolo.SelectedIndex = 0;
                    HiddenContributivoStorico.Value = liquidazioneAgo.areaLiquidazionePensioneAgo.DatiLiquidazionePensioneStorico != null ? liquidazioneAgo.areaLiquidazionePensioneAgo.DatiLiquidazionePensioneStorico.Contributivo != null ? liquidazioneAgo.areaLiquidazionePensioneAgo.DatiLiquidazionePensioneStorico.Contributivo.ToString() : "" : "";
                }

                #region ddlCodiciArretrati
                if (Utility.IsDomandaIndennitaUnaTantum_AGO(TitolarePensione.Pensione))
                {
                    if (liquidazioneAgo.areaLiquidazionePensioneAgo.DatiGenerici.CodiceArretrati.HasValue)
                        ddlCodiciArretrati.SelectedValue = liquidazioneAgo.areaLiquidazionePensioneAgo.DatiGenerici.CodiceArretrati.Value.ToString();
                    else ddlCodiciArretrati.SelectedIndex = 2;
                    ddlCodiciArretrati.Enabled = false;
                }
                else
                {
                    if ((Utility.IsDomandaCRED27(this.domanda.Categoria) || Utility.IsDomandaCOOP28(this.domanda.Categoria)) &&
                        !CodeUtility.IsRicostituzioneOrRiapertura(TitolarePensione.Pensione, this.domanda.IsDomandaRiapertura))
                        ddlCodiciArretrati.SelectedIndex = 1;
                    else if (liquidazioneAgo.areaLiquidazionePensioneAgo.DatiGenerici.CodiceArretrati.HasValue)
                        ddlCodiciArretrati.SelectedValue = liquidazioneAgo.areaLiquidazionePensioneAgo.DatiGenerici.CodiceArretrati.Value.ToString();
                    else if (Utility.IsDomandaVOESO(this.domanda.Categoria) || Utility.IsDomandaVESO33(this.domanda.Categoria) ||
                             Utility.IsDomandaVESO92(this.domanda.Categoria) || Utility.IsDomandaVOCOOP(this.domanda.Categoria) ||
                             Utility.IsDomandaVOCRED(this.domanda.Categoria) || Utility.IsDomandaINDCOM(this.domanda.Categoria) ||
                             Utility.IsDomandaESPA(this.domanda.Categoria) || Utility.IsDomandaESOPMI(this.domanda.Categoria))
                        ddlCodiciArretrati.SelectedIndex = 1;
                    else
                        ddlCodiciArretrati.SelectedIndex = 0;

                    if (CodeUtility.IsRicostituzioneOrRiapertura(TitolarePensione.Pensione, this.domanda.IsDomandaRiapertura) &&
                      (Utility.IsDomandaVOCOOP(this.domanda.Categoria) || Utility.IsDomandaVOCRED(this.domanda.Categoria) || Utility.IsDomandaVESO92(this.domanda.Categoria) ||
                      Utility.IsDomandaESPA(this.domanda.Categoria) || Utility.IsDomandaESOPMI(this.domanda.Categoria)))
                        ddlCodiciArretrati.SelectedIndex = 2;
                    else if (CodeUtility.IsRicostituzioneOrRiapertura(TitolarePensione.Pensione, this.domanda.IsDomandaRiapertura) &&
                      (Utility.IsDomandaVOESO(this.domanda.Categoria) || Utility.IsDomandaVESO29(this.domanda.Categoria) || Utility.IsDomandaVESO33(this.domanda.Categoria)))
                        ddlCodiciArretrati.SelectedIndex = 1;
                    else if (Utility.IsDomandaBancari(this.domanda.Categoria) && !CodeUtility.IsRicostituzioneOrRiapertura(TitolarePensione.Pensione, this.domanda.IsDomandaRiapertura) && !Utility.IsDomandaRipristino(TitolarePensione.Pensione))
                    {
                        ddlCodiciArretrati.SelectedIndex = 2;
                        ddlCodiciArretrati.Enabled = false;
                    }
                    else if (Utility.IsDomandaESOTEL(this.domanda.Categoria))
                    {
                        ddlCodiciArretrati.SelectedIndex = 1;
                        ddlCodiciArretrati.Enabled = false;
                    }
                }

                var anagraficaTit = (AreaRispostaRiepilogo.DatiRiepilogoAnagrafica)Session["Anagrafica"];
                if (TitolarePensione.Pensione.TipoAutomazione.HasValue && ddlCodiciArretrati.SelectedIndex == 0)
                {
                    if (TitolarePensione.Pensione.TipoAutomazione.Value == 2 && anagraficaTit != null && anagraficaTit.DataMorte.HasValue)
                        ddlCodiciArretrati.SelectedIndex = 2;
                    else
                        ddlCodiciArretrati.SelectedIndex = 1;
                }
                #endregion ddlCodiciArretrati
                if (Utility.IsDomandaAdeguamentoRinnoviContrattualiGDP(TitolarePensione.Pensione) && TitolarePensione != null && TitolarePensione.Pensione != null && TitolarePensione.Pensione.DecorrenzaOriginaria.HasValue)
                {
                    txtDecorrenzaArretrati.Text = String.Format("{0:MM/yyyy}", TitolarePensione.Pensione.DecorrenzaOriginaria.Value);
                    txtDecorrenzaArretrati.Enabled = false;
                }
                else if (liquidazioneAgo.areaLiquidazionePensioneAgo.DatiGenerici.DecorrenzaCalcoloArretrati.HasValue)
                    txtDecorrenzaArretrati.Text = String.Format("{0:MM/yyyy}", liquidazioneAgo.areaLiquidazionePensioneAgo.DatiGenerici.DecorrenzaCalcoloArretrati.Value);
                else if (TitolarePensione.Pensione.TipoAutomazione.HasValue)
                {
                    if (TitolarePensione.Pensione.TipoAutomazione.Value == 1 && TitolarePensione.Pensione.DataPresentazioneDomanda.HasValue)
                    {
                        txtDecorrenzaArretrati.Text = "01/" + TitolarePensione.Pensione.DataPresentazioneDomanda.Value.Year;
                    }
                    //else 
                    //{
                    //    string controlloDinamico = string.Empty;
                    //    Presenter.PresenterControlliDinamici presenter = new PresenterControlliDinamici();
                    //    Presenter.SvrLiquidazione.AreaEsito esito = presenter.GetAnnoCompetenza((UtilityTipoAppartenenza)Utility.GetTipoAppartenenzaRuolo((Ruoli)Session["Ruolo"]), out controlloDinamico);
                    //    if (esito != null && esito.RisultatoOperazione == Presenter.SvrLiquidazione.AreaEsito.TipoEsito.OK && !string.IsNullOrEmpty(controlloDinamico))
                    //        txtDecorrenzaArretrati.Text = "01/" + controlloDinamico;
                    //}                   
                }

                if (!this.domanda.Categoria.StartsWith("V") && !(Utility.IsDomandaVOESO(this.domanda.Categoria) || Utility.IsDomandaVESO33(this.domanda.Categoria) || Utility.IsDomandaVESO92(this.domanda.Categoria) ||
                    Utility.IsDomandaVOCRED_CRED27(this.domanda.Categoria) || Utility.IsDomandaVOCOOP_COOP28(this.domanda.Categoria) || Utility.IsDomandaAPESociale(this.domanda.Categoria) ||
                    Utility.IsDomandaESOAMB(this.domanda.Categoria) || Utility.IsDomandaESOTEL(this.domanda.Categoria) || Utility.IsDomandaIndennitaUnaTantum_AGO(TitolarePensione.Pensione) ||
                    Utility.IsDomandaESPA(this.domanda.Categoria) || Utility.IsDomandaESOPMI(this.domanda.Categoria)))
                {
                    pnlScadRevSanitaria.Visible = true;
                    if (liquidazioneAgo.areaLiquidazionePensioneAgo.DatiGenerici.ScadenzaRevisioneSanitaria.HasValue)
                        txtScadRevSanitaria.Text = String.Format("{0:MM/yyyy}", liquidazioneAgo.areaLiquidazionePensioneAgo.DatiGenerici.ScadenzaRevisioneSanitaria.Value);
                    else
                    {
                        if (Utility.IsDomandaINDCOM(TitolarePensione.Pensione) && !Utility.IsDomandaINDCOM124(TitolarePensione.Pensione, this.domanda.Categoria))
                        {
                            DateTime? dataLimiteSuperiore = null;
                            if (Utility.IsDomandaINDCOM156(TitolarePensione.Pensione, this.domanda.Categoria))
                                dataLimiteSuperiore = new DateTime(2028, 2, 1);

                            var anagrafica = (AreaRispostaRiepilogo.DatiRiepilogoAnagrafica)Session["Anagrafica"];
                            if (anagrafica != null)
                            {
                                var scadenza = Utility.GetDataLimiteScadenzaIndenizzoINDCOM(TitolarePensione.Pensione.CodeTipo, anagrafica.Sesso.GetValueOrDefault().ToString(), anagrafica.DataNascita,
                                    liquidazioneAgo.areaLiquidazionePensioneAgo.ListaCtrlScadenzaIndennizzoINDCOM, TitolarePensione.Pensione, this.domanda.Categoria);
                                if (scadenza.HasValue)
                                {
                                    if (dataLimiteSuperiore.HasValue && Utility.DataStrettamenteSuccessivaA(scadenza.Value, dataLimiteSuperiore.Value))
                                        scadenza = dataLimiteSuperiore;

                                    txtScadRevSanitaria.Text = String.Format("{0:MM/yyyy}", scadenza.Value);
                                }
                            }
                        }
                    }
                }

                //ENG - Memo 28_2024
                if (!String.IsNullOrEmpty(ctrlAbilitazioneMemo28) && ctrlAbilitazioneMemo28.Trim().ToUpperInvariant() == "SI")
                {
                    if (Utility.IsRicostituzione(TitolarePensione.Pensione) && TitolarePensione.Pensione.DecorrenzaOriginaria.HasValue
                        && Utility.DataStrettamenteSuccessivaA(TitolarePensione.Pensione.DecorrenzaOriginaria.Value, new DateTime(2024, 1, 1)))
                    {
                        //RIC DI PL 0001/0001/0017
                        if (TitolarePensione.Pensione.IdTipoPLPerRIC == 7)
                        {
                            if (!String.IsNullOrEmpty(liquidazioneAgo.areaLiquidazionePensioneAgo.DatiGenerici.NaturaPensione)
                                && (liquidazioneAgo.areaLiquidazionePensioneAgo.DatiGenerici.NaturaPensione.Substring(0, 1) == "1" || liquidazioneAgo.areaLiquidazionePensioneAgo.DatiGenerici.NaturaPensione.Substring(0, 1) == "2"))
                            {
                                if (liquidazioneAgo.areaLiquidazionePensioneAgo.DatiGenerici.ScadenzaRevisioneSanitaria.HasValue)
                                    txtScadRevSanitaria.Text = String.Format("{0:MM/yyyy}", liquidazioneAgo.areaLiquidazionePensioneAgo.DatiGenerici.ScadenzaRevisioneSanitaria.Value);
                            }
                        }

                        //RIC DI PL 0001/0001/0045 pav
                        if (TitolarePensione.Pensione.IdTipoPLPerRIC == 26)
                        {
                            if (liquidazioneAgo.areaLiquidazionePensioneAgo.DatiGenerici.ScadenzaRevisioneSanitaria.HasValue)
                                txtScadRevSanitaria.Text = String.Format("{0:MM/yyyy}", liquidazioneAgo.areaLiquidazionePensioneAgo.DatiGenerici.ScadenzaRevisioneSanitaria.Value);
                        }
                    }
                }

                if (liquidazioneAgo.areaLiquidazionePensioneAgo.DatiGenerici.DataCompletezza.HasValue)
                    txtDataCompletezza.Text = String.Format("{0:dd/MM/yyyy}", liquidazioneAgo.areaLiquidazionePensioneAgo.DatiGenerici.DataCompletezza.Value);
                else if (TitolarePensione.Pensione.TipoAutomazione.HasValue)
                    txtDataCompletezza.Text = String.Format("{0:dd/MM/yyyy}", TitolarePensione.Pensione.DataPresentazioneDomanda.Value);

                if (liquidazioneAgo.areaLiquidazionePensioneAgo.DatiGenerici.DataInteressiLegali.HasValue)
                    txtInteressiLegali.Text = String.Format("{0:dd/MM/yyyy}", liquidazioneAgo.areaLiquidazionePensioneAgo.DatiGenerici.DataInteressiLegali.Value);

                if (liquidazioneAgo.areaLiquidazionePensioneAgo.DatiGenerici.CausaCarico.HasValue)
                {
                    ddlCausaCarico.SelectedValue = liquidazioneAgo.areaLiquidazionePensioneAgo.DatiGenerici.CausaCarico.Value.ToString();
                    ddlCausaCarico.Enabled = false;
                }
                else
                {
                    ddlCausaCarico.SelectedIndex = 0;
                    ddlCausaCarico.Enabled = true;
                }

                if (liquidazioneAgo.areaLiquidazionePensioneAgo.DatiGenerici.DataInizioCalcolo.HasValue)
                    txtDataCalcolo.Text = String.Format("{0:MM/yyyy}", liquidazioneAgo.areaLiquidazionePensioneAgo.DatiGenerici.DataInizioCalcolo.Value);

                if (liquidazioneAgo.areaLiquidazionePensioneAgo.DatiGenerici.CodiceComunicazioneCampo4.HasValue)
                {
                    ddlEsenzioneFiscale.SelectedValue = liquidazioneAgo.areaLiquidazionePensioneAgo.DatiGenerici.CodiceComunicazioneCampo4.ToString();

                    if (CodeUtility.IsRicostituzioneOrRiapertura(TitolarePensione.Pensione, this.domanda.IsDomandaRiapertura) && liquidazioneAgo.areaLiquidazionePensioneAgo.IsEsenzioneFiscaleVittima)
                    {
                        if (ddlEsenzioneFiscale.SelectedValue == "1")
                        {
                            ddlEsenzioneFiscale.Enabled = false;
                        }
                    }
                }
                else
                {
                    if (CodeUtility.IsRicostituzioneOrRiapertura(TitolarePensione.Pensione, this.domanda.IsDomandaRiapertura))
                    {
                        //tutte le domande di trasformazione e ricostituzione
                        if (liquidazioneAgo.areaLiquidazionePensioneAgo.IsEsenzioneFiscaleEsteroFromDetrazioni.GetValueOrDefault())
                        {
                            if (ddlEsenzioneFiscale.Items.FindByValue("2") != null)
                                ddlEsenzioneFiscale.SelectedValue = ddlEsenzioneFiscale.Items.FindByValue("2").Value;
                        }

                        if (liquidazioneAgo.areaLiquidazionePensioneAgo.IsEsenzioneFiscaleVittima)
                        {
                            if (ddlEsenzioneFiscale.Items.FindByValue("1") != null)
                            {
                                ddlEsenzioneFiscale.SelectedValue = ddlEsenzioneFiscale.Items.FindByValue("1").Value;
                                ddlEsenzioneFiscale.Enabled = false;
                            }
                        }
                    }
                    else
                        ddlEsenzioneFiscale.SelectedIndex = 0;
                }

                if (!String.IsNullOrEmpty(liquidazioneAgo.areaLiquidazionePensioneAgo.DatiGenerici.ModalitaLiquidazione))
                    ddlModalitaLiquidazione.SelectedValue = liquidazioneAgo.areaLiquidazionePensioneAgo.DatiGenerici.ModalitaLiquidazione;
                else
                    ddlModalitaLiquidazione.SelectedIndex = 0;

                if (Utility.IsDomandaUnicarpe(TitolarePensione.Pensione, true) != Utility.TipoUnicarpe.Automatica &&
                   (liquidazioneAgo.areaLiquidazionePensioneAgo.IsSperimentaleDonna.GetValueOrDefault() || TitolarePensione.Pensione.IsDomandaQuota100OrRicostituzione || TitolarePensione.Pensione.IsDomandaQuota102OrRicostituzione))
                {
                    ddlCodMobilita.SelectedIndex = 0;
                    ddlCodMobilita.Enabled = false;
                }
                else
                {
                    if (liquidazioneAgo.areaLiquidazionePensioneAgo.DatiGenerici.CodiceMobilita.HasValue)
                        ddlCodMobilita.SelectedValue = liquidazioneAgo.areaLiquidazionePensioneAgo.DatiGenerici.CodiceMobilita.Value.ToString();
                    else
                        ddlCodMobilita.SelectedIndex = 0;
                }

                if (liquidazioneAgo.areaLiquidazionePensioneAgo.DatiGenerici.CodiceDomandaRicorso.HasValue)
                    ddlCodDomRicorso.SelectedValue = liquidazioneAgo.areaLiquidazionePensioneAgo.DatiGenerici.CodiceDomandaRicorso.Value.ToString();
                else
                    ddlCodDomRicorso.SelectedIndex = 0;

                if (!chkBenefici.Checked && !Utility.IsDomandaESOTEL(this.domanda.Categoria))
                {
                    chkBenefici.Checked = liquidazioneAgo.areaLiquidazionePensioneAgo.DatiGenerici.Benefici.HasValue && liquidazioneAgo.areaLiquidazionePensioneAgo.DatiGenerici.Benefici.Value ? true : false;
                    if (chkBenefici.Checked)
                        HiddenFieldChkBeneficiChecked.Value = "true";
                    else
                        HiddenFieldChkBeneficiChecked.Value = "false";
                }
                chkExCombattente.Checked = liquidazioneAgo.areaLiquidazionePensioneAgo.DatiGenerici.ExCombattente.HasValue && liquidazioneAgo.areaLiquidazionePensioneAgo.DatiGenerici.ExCombattente.Value ? true : false;
                if (!chkTrasfAOI.Checked)
                    chkTrasfAOI.Checked = liquidazioneAgo.areaLiquidazionePensioneAgo.DatiGenerici.TrasformazioneAOI.HasValue && liquidazioneAgo.areaLiquidazionePensioneAgo.DatiGenerici.TrasformazioneAOI.Value ? true : false;
                if (!chkRichiestaBonus.Checked)
                    chkRichiestaBonus.Checked = liquidazioneAgo.areaLiquidazionePensioneAgo.DatiGenerici.IsRichiestaBonus.HasValue && liquidazioneAgo.areaLiquidazionePensioneAgo.DatiGenerici.IsRichiestaBonus.Value ? true : false;

                if (liquidazioneAgo.areaLiquidazionePensioneAgo.DatiGenerici.TrattenutaInpdap.HasValue)
                {
                    if ((bool)liquidazioneAgo.areaLiquidazionePensioneAgo.DatiGenerici.TrattenutaInpdap.Value)
                        ddlTrattINPDAP.SelectedValue = "SI";
                    else if ((bool)liquidazioneAgo.areaLiquidazionePensioneAgo.DatiGenerici.TrattenutaInpdap.Value == false)
                        ddlTrattINPDAP.SelectedValue = "NO";
                }
                else
                    ddlTrattINPDAP.SelectedIndex = 0;

                if (liquidazioneAgo.areaLiquidazionePensioneAgo.DatiGenerici.DataRinunciaTrattenutaInpdap.HasValue)
                    txtDecTrattINPDAP.Text = String.Format("{0:MM/yyyy}", liquidazioneAgo.areaLiquidazionePensioneAgo.DatiGenerici.DataRinunciaTrattenutaInpdap.Value);

                if (CodeUtility.IsRicostituzione(TitolarePensione.Pensione) && !Utility.IsDomandaEccezioneMemo86(this.domanda.Categoria, TitolarePensione.Pensione.NaturaPensione, TitolarePensione.Pensione) && TitolarePensione.Pensione.DataPresentazioneDomanda != null &&
                    Utility.DataStrettamenteSuccessivaA(TitolarePensione.Pensione.DataPresentazioneDomanda.Value, new DateTime(2022, 02, 20)))
                {
                    HiddenFieldIsRICPost20022022.Value = "SI";

                    //ENG - Aggiornamento Memo86
                    string controlloDinamicoAggiornamentoMemo86 = string.Empty;
                    Presenter.PresenterControlliDinamici presenterAggiornamentoMemo86 = new PresenterControlliDinamici();
                    Presenter.SvrLiquidazione.AreaEsito esitoCaricamentoControlloDinamicoAggiornamentoMemo86 = presenterAggiornamentoMemo86.GetControlloDinamicoByNomeControllo("DataAttivazioneMemo86Del12_06_2023", out controlloDinamicoAggiornamentoMemo86);

                    if (esitoCaricamentoControlloDinamicoAggiornamentoMemo86 != null
                        && esitoCaricamentoControlloDinamicoAggiornamentoMemo86.RisultatoOperazione == Presenter.SvrLiquidazione.AreaEsito.TipoEsito.OK
                        && !String.IsNullOrEmpty(controlloDinamicoAggiornamentoMemo86) && !String.IsNullOrEmpty(controlloDinamicoAggiornamentoMemo86.Trim())
                        && liquidazioneAgo.areaLiquidazionePensioneAgo.DataPrelievoDomanda.HasValue
                        && Utility.DataSuccessivaA(liquidazioneAgo.areaLiquidazionePensioneAgo.DataPrelievoDomanda.Value, Utility.DataFromString(controlloDinamicoAggiornamentoMemo86.Trim(), Utility.FormatoData.AAAAmmGG).Value))
                    {
                        VerificaAdesioneFondoCreditoAggiornamentoMemo86(liquidazioneAgo.areaLiquidazionePensioneAgo.IsPresenteTrattenutaFondoCreditoDaPrelievo, liquidazioneAgo.areaLiquidazionePensioneAgo.IsDataRinunciaTrattenutaInpdapStorico);
                    }
                    else
                    {
                        if (liquidazioneAgo != null && liquidazioneAgo.areaLiquidazionePensioneAgo != null && liquidazioneAgo.areaLiquidazionePensioneAgo.IsDataRinunciaTrattenutaInpdapStorico.GetValueOrDefault())
                        {
                            ddlTrattINPDAP.Enabled = false;
                            txtDecTrattINPDAP.Enabled = false;
                        }
                        else
                            VerificaAdesioneFondoCredito();
                    }
                }

                //Per tutte le domande che abbiano gruppo = ‘0002’, prodotto = ‘0011’, 
                //e che non siano trasformazioni da provvisoria a definitiva, 
                //i campi Trattenuta INPDAP e Decorrenza Trattenuta INPDAP dovranno essere resi non editabili (e popolati a blank).
                if (TitolarePensione.Pensione.CodeGruppo.Trim() == "0002" && TitolarePensione.Pensione.CodeProdotto.Trim() == "0011" && !this.domanda.IsDomandaRiapertura)
                {
                    ddlTrattINPDAP.SelectedIndex = 0;
                    ddlTrattINPDAP.Enabled = false;
                    txtDecTrattINPDAP.Text = string.Empty;
                    txtDecTrattINPDAP.Enabled = false;
                }

                if (CodeUtility.IsRicostituzione(TitolarePensione.Pensione) && liquidazioneAgo.areaLiquidazionePensioneAgo.IsDataRinunciaTrattenutaInpdapStorico.GetValueOrDefault())
                {
                    ddlTrattINPDAP.Enabled = false;
                    txtDecTrattINPDAP.Enabled = false;
                }

                if (liquidazioneAgo.areaLiquidazionePensioneAgo.DatiGenerici.NRiconoscimentiInvalidita.HasValue)
                    ddlConfermeInvalidita.SelectedValue = liquidazioneAgo.areaLiquidazionePensioneAgo.DatiGenerici.NRiconoscimentiInvalidita.Value.ToString();
                else ddlConfermeInvalidita.SelectedIndex = 0;

                if (liquidazioneAgo.areaLiquidazionePensioneAgo.DatiGenerici.CodiceLiquidazione.HasValue)
                    txtCodiceLiquidazione.Text = liquidazioneAgo.areaLiquidazionePensioneAgo.DatiGenerici.CodiceLiquidazione.Value.ToString();
                else if (Utility.IsDomandaReversibilita(TitolarePensione.Pensione) && !this.domanda.IsDomandaENPALS && !Utility.IsDomandaSpacchettamentoSOPGIPost072022(this.domanda.Categoria, TitolarePensione.Pensione, this.areaDanteCausa))
                    txtCodiceLiquidazione.Text = "P";

                string CodFase = string.Empty;
                Presenter.SvrLiquidazione.ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
                Presenter.SvrLiquidazione.AreaEsito esito = objWS.GetCodFaseByNDomus(out CodFase, domanda.NumeroDomanda);
                string Gruppo = this.TitolarePensione.Pensione.CodeGruppo;
                string Prodotto = this.TitolarePensione.Pensione.CodeProdotto;
                string Caratterizzazione = this.TitolarePensione.Pensione.Caratterizzazione;
                string TipoLetturaUnicarpe = this.TitolarePensione.Pensione.TipoLetturaUnicarpe.ToString();
                if (Utility.checkMemo74_88(CodFase, Gruppo, Prodotto, Caratterizzazione, TipoLetturaUnicarpe) || liquidazioneAgo.areaLiquidazionePensioneAgo.IsFlagProvvisoriaCheckedAndEnabled.GetValueOrDefault())
                {
                    chkProvvisoria.Checked = true;
                    chkProvvisoria.Enabled = false;
                }
                else
                    chkProvvisoria.Checked = liquidazioneAgo.areaLiquidazionePensioneAgo.DatiGenerici.FlagProvvisoria.HasValue && liquidazioneAgo.areaLiquidazionePensioneAgo.DatiGenerici.FlagProvvisoria.Value ? true : false;

                if (liquidazioneAgo.areaLiquidazionePensioneAgo.DatiLiquidazionePensioneStorico != null && liquidazioneAgo.areaLiquidazionePensioneAgo.DatiLiquidazionePensioneStorico.DecorrenzaMaggiorazioneSociale != null &&
                    liquidazioneAgo.areaLiquidazionePensioneAgo.DatiLiquidazionePensioneStorico.DecorrenzaMaggiorazioneSociale != DateTime.MinValue)
                {

                    DateTime? dataSistema = null;
                    GetDataSistema(UtilityTipoAppartenenza.AGO, out dataSistema);
                    Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoAnagrafica anagraficaTitolare = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoAnagrafica)Session["Anagrafica"];
                    if (anagraficaTitolare != null && anagraficaTitolare.DataNascita.HasValue && dataSistema.HasValue &&
                        !Utility.DataStrettamenteSuccessivaA(anagraficaTitolare.DataNascita.Value.AddYears(60), dataSistema.Value))
                        chkMaggiorazioni.Enabled = false;

                    chkMaggiorazioni.Checked = true;
                }
                else if (liquidazioneAgo.areaLiquidazionePensioneAgo.DatiGenerici.Maggiorazioni.HasValue)
                {
                    chkMaggiorazioni.Checked = liquidazioneAgo.areaLiquidazionePensioneAgo.DatiGenerici.Maggiorazioni.Value;
                    if (liquidazioneAgo.areaLiquidazionePensioneAgo.DatiGenerici.TrattamentoDisagi.GetValueOrDefault())
                        chkMaggiorazioni.ToolTip = "È presente una richiesta di maggiorazione sociale in domanda";
                }
                else if (liquidazioneAgo.areaLiquidazionePensioneAgo.DatiGenerici.TrattamentoDisagi.GetValueOrDefault())
                {
                    chkMaggiorazioni.Checked = true;
                    chkMaggiorazioni.ToolTip = "È presente una richiesta di maggiorazione sociale in domanda";
                }
                bool? trattamentoDisagi = liquidazioneAgo.areaLiquidazionePensioneAgo.DatiGenerici.TrattamentoDisagi;
                HiddenTrattamentoDisagi.Value = !trattamentoDisagi.HasValue ? "" : (trattamentoDisagi.Value == true ? "true" : "false");

                if (Utility.IsRicostituzione_Reddituale(TitolarePensione.Pensione) && TitolarePensione.Pensione.CodeTipo == "0101")
                    chkMaggiorazioni.Checked = true;
            }

            if (TitolarePensione.Pensione.FlagUnicarpe.HasValue)
                GestioneEtichetteIsUnicarpe(TitolarePensione.Pensione);

            if (liquidazioneAgo != null && liquidazioneAgo.areaLiquidazionePensioneAgo.IsRipristino.GetValueOrDefault())
                ddlCodDomRicorso.SelectedValue = "9";

            GestioneEtichetteENPALS(liquidazioneAgo);

            if (HiddenFieldIsRicostituzione.Value == "SI")
                GestioneEtichetteRic(TitolarePensione.Pensione, liquidazioneAgo);

            ManageDomandeAUT(this.domanda, this.TitolarePensione.Pensione);
            ManageDomandeSPED(this.domanda, TitolarePensione.Pensione);
            ManageDomandeIndennitaUnaTantum(TitolarePensione.Pensione);
            ManageDomandeSupplementari(TitolarePensione.Pensione);
            ManageVESO33_VESO92(this.domanda, this.TitolarePensione.Pensione, liquidazioneAgo != null && liquidazioneAgo.areaLiquidazionePensioneAgo != null ? liquidazioneAgo.areaLiquidazionePensioneAgo.IsDomandaVESO92WithFiltroL92 : false);
            ManageForPensioniVecchiaiaCalcoloContrib(liquidazioneAgo, this.TitolarePensione.Pensione);
            if (liquidazioneAgo != null)
                ManageNaturaPerINPDAI(liquidazioneAgo.areaLiquidazionePensioneAgo);
            ManageVOCOOP_VOESO_VOCRED_CRED27_COOP28(this.domanda);
            ManageVESO29_ESOTEL_ESOAMB();
            ManageRipristini(liquidazioneAgo, this.domanda);
            ManageRiliquidazioni(TitolarePensione.Pensione, liquidazioneAgo, this.domanda);
            ManageDomandeINDCOM();
            ManageDomandeTotilizzazione(liquidazioneAgo, TitolarePensione.Pensione);
            ManageESPA(this.domanda, this.TitolarePensione.Pensione);
            ManageDomandeMIN(this.domanda, this.TitolarePensione.Pensione);
            ManageDomandePescatori(this.domanda);
            ManageDomandeBancari(this.domanda, this.TitolarePensione.Pensione);
            ManageTipoCalcoloTraRicEsodati(this.domanda, this.TitolarePensione.Pensione, liquidazioneAgo.areaLiquidazionePensioneAgo != null ? liquidazioneAgo.areaLiquidazionePensioneAgo.DatiLiquidazionePensioneStorico : null);
            ManageTipoCalcoloRicNonContributive(this.domanda, this.TitolarePensione.Pensione);
            ManageRenditaFacoltativa_Casalinghe(this.TitolarePensione.Pensione);
            ManageVOST();
            ManagePSO();
            ManagePMO(this.TitolarePensione.Pensione);
            //ManageAnte96 da eseguire sempre in ultimo poichè vince su tutto
            ManageAnte96(liquidazioneAgo.areaLiquidazionePensioneAgo);
            ManageESOPMI(this.domanda, this.TitolarePensione.Pensione);
            ManageINPGI(this.domanda, this.TitolarePensione.Pensione);
            ManageBancRicAnte1991(this.TitolarePensione.Pensione);

            if (ddlCodNatura1DG.SelectedValue == "5")
                ddlCodNatura1DG.Enabled = false;

            //ENG - sulle ricostituzioni della nuova opzione donna rendere non editabili tutti i campi del pannello liquidazione pensione – generici ad eccezione della “data completezza”, “decorrenza arretrati” e primo codice natura
            if (CodeUtility.IsRicostituzione(TitolarePensione.Pensione) && liquidazioneAgo != null && liquidazioneAgo.areaLiquidazionePensioneAgo != null &&
                liquidazioneAgo.areaLiquidazionePensioneAgo.IsOpzioneDonna_Legge197_2022_Art1_Comma292OrRicostituzione.GetValueOrDefault())
            {
                ddlCodNatura1DG.Enabled = true;
                ddlCodNatura3DG.Enabled = false;
                ddlEsenzioneFiscale.Enabled = false;
                chkExCombattente.Enabled = false;
                chkMaggiorazioni.Enabled = false;
                chkBenefici.Enabled = false;
                HiddenFieldChkBeneficiDisabled.Value = "true";
            }

            //ENG - TFR della nuova opzione donna rendere non editabile il terzo byte del codice natura
            if (this.domanda.IsDomandaRiapertura && liquidazioneAgo != null && liquidazioneAgo.areaLiquidazionePensioneAgo != null &&
                liquidazioneAgo.areaLiquidazionePensioneAgo.IsOpzioneDonna_Legge197_2022_Art1_Comma292OrRicostituzione.GetValueOrDefault())
            {
                ddlCodNatura3DG.Enabled = false;
            }

            if (liquidazioneAgo != null && liquidazioneAgo.areaLiquidazionePensioneAgo != null && liquidazioneAgo.areaLiquidazionePensioneAgo.IsPrepensionamentoEditoriaFiltroEBA.GetValueOrDefault())
            {
                ddlCodNatura3DG.SelectedValue = ddlCodNatura3DG.Items.FindByText("O").Value;
                ddlCodNatura3DG.Enabled = false;
            }

            //ENG - Per le Ric con terzo codice natura Z e attività economica = 67 e professione individuale = 011 se non sono presenti i dati maggiorazione e benefici dal prelievo bisogna deselezionare il check benefici e renderelo non editabile
            if (liquidazioneAgo != null && liquidazioneAgo.areaLiquidazionePensioneAgo != null && liquidazioneAgo.areaLiquidazionePensioneAgo.IsRicConTerzoCodNaturaZAttEconomica67ProfIndividuale11.GetValueOrDefault())
            {
                chkBenefici.Checked = false;
                HiddenFieldChkBeneficiChecked.Value = "false";
                chkBenefici.Enabled = false;
                HiddenFieldChkBeneficiDisabled.Value = "true";
                HiddenFieldIsRicConTerzoCodNaturaZAttEconomica67ProfIndividuale11.Value = "true";
            }

            if (Utility.IsRicostituzione_PerVariazioneDatiSupplemento(TitolarePensione.Pensione) || (ViewState["AbilitazioneMemo50_2023"] != null && (string)ViewState["AbilitazioneMemo50_2023"] == "SI" && CodeUtility.IsRicostituzioneSupplemento(TitolarePensione.Pensione)))
                ddlTipoCalcolo.Enabled = false;

            if (this.TitolarePensione.Pensione.IsDomandaAnticipataFlessibileOpzioneContributivoOrRicostituzione)
                ddlCodNatura2DG.Enabled = false;

            //ENG - RIC REVERSIBILITA
            if (Utility.IsRicostituzione(TitolarePensione.Pensione) && Utility.IsDomandaPensioneReversibilitaOrRicostituzione(this.domanda.Categoria, TitolarePensione.Pensione, areaDanteCausa) && !Utility.IsDomandaCumulo(this.domanda.Categoria))
            {
                if (liquidazioneAgo != null && liquidazioneAgo.areaLiquidazionePensioneAgo != null && !String.IsNullOrEmpty(liquidazioneAgo.areaLiquidazionePensioneAgo.TipoSettimaneBeneficio)
                    && (liquidazioneAgo.areaLiquidazionePensioneAgo.TipoSettimaneBeneficio == "14" || liquidazioneAgo.areaLiquidazionePensioneAgo.TipoSettimaneBeneficio == "18"
                    || liquidazioneAgo.areaLiquidazionePensioneAgo.TipoSettimaneBeneficio == "19" || liquidazioneAgo.areaLiquidazionePensioneAgo.TipoSettimaneBeneficio == "12" || liquidazioneAgo.areaLiquidazionePensioneAgo.TipoSettimaneBeneficio == "24"))
                {
                    chkBenefici.Checked = false;
                    HiddenFieldChkBeneficiChecked.Value = "false";
                    chkBenefici.Enabled = false;
                    HiddenFieldChkBeneficiDisabled.Value = "true";
                }
            }

            //ENG - Memo 108_2024
            if (Utility.IsDomandaVOCUM(this.domanda.Categoria))
            {
                string ctrlMemo108_2024 = string.Empty;
                Presenter.PresenterControlliDinamici presenterMemo108_2024 = new PresenterControlliDinamici();
                Presenter.SvrLiquidazione.AreaEsito esitoCaricamentoControlloDinamicoMemo108_2024 = presenterMemo108_2024.GetControlloDinamicoByNomeControllo("AbilitazioneMemo108_2024", out ctrlMemo108_2024);

                if (esitoCaricamentoControlloDinamicoMemo108_2024 != null
                    && esitoCaricamentoControlloDinamicoMemo108_2024.RisultatoOperazione == Presenter.SvrLiquidazione.AreaEsito.TipoEsito.OK && !String.IsNullOrEmpty(ctrlMemo108_2024)
                    && ctrlMemo108_2024.Trim().ToUpperInvariant() == "SI")
                {
                    if (liquidazioneAgo != null && liquidazioneAgo.areaLiquidazionePensioneAgo != null && liquidazioneAgo.areaLiquidazionePensioneAgo.IsFlagProvvisoriaFromCumulo.HasValue
                        && liquidazioneAgo.areaLiquidazionePensioneAgo.IsFlagProvvisoriaFromCumulo.Value)
                    {
                        chkProvvisoria.Checked = true;
                        chkProvvisoria.Enabled = false;
                    }
                }
            }

            //ENG - Per le Vocum Ape Precoci se viene inserito il bypass COMPARTO_SCUOLA per il controllo sul pannello titolare: “La decorrenza pensione deve essere di almeno 3 mesi successiva alla data di perfezionamento dei requisiti”,
            // bisogna valorizzare il terzo byte codice natura = “S” e renderlo non editabile
            if (liquidazioneAgo != null && liquidazioneAgo.areaLiquidazionePensioneAgo != null && liquidazioneAgo.areaLiquidazionePensioneAgo.IsBypassCompartoScuolaAttivo.GetValueOrDefault())
            {
                ddlCodNatura3DG.ClearSelection();
                if (ddlCodNatura3DG.Items.FindByValue("S") != null)
                    ddlCodNatura3DG.SelectedValue = "S";
                ddlCodNatura3DG.Enabled = false;
            }

            //ENG - INPGI migrate
            if (((Utility.IsRicostituzione(TitolarePensione.Pensione) && Utility.IsDomandaINPGI(this.domanda.Categoria)) ||
                (!this.domanda.IsDomandaRiapertura && this.domanda.Categoria.Trim().ToUpperInvariant() == "SOPGI" && Utility.IsDomandaReversibilita(TitolarePensione.Pensione)))
                && TitolarePensione.Pensione.GP1AV91B == "2" && ddlCodNatura1DG.SelectedValue == "5")
                chkMaggiorazioni.Enabled = false;


            //ENG - RIC CUMULO Motivi Contributivi: valorizzazione data interessi legali
            if ((Utility.IsDomandaVOCUM(this.domanda.Categoria) || Utility.IsDomandaIOCUM(this.domanda.Categoria) || Utility.IsDomandaSOCUM(this.domanda.Categoria))
                && Utility.IsRicostituzione_MotiviContributivi(TitolarePensione.Pensione) && TitolarePensione.Pensione.CodeTipo == "0198")
            {
                HdnSetDataInteressiLegaliRicCumulo.Value = "true";
            }
        }

        private void ManageINPGI(AreaRispostaRiepilogo.DatiRiepilogoDomanda datiRiepilogoDomanda, AreaTitolare.DatiPensione pensione)
        {
            if (Utility.IsRicostituzione(pensione) && Utility.IsDomandaINPGI(datiRiepilogoDomanda.Categoria))
                ddlTipoCalcolo.Enabled = false;

            //ENG - Spacchettamento SOPGI
            if (Utility.IsDomandaSpacchettamentoSOPGIPost072022(this.domanda.Categoria, pensione, this.areaDanteCausa))
            {
                if (Utility.IsDomandaReversibilita(pensione) || (Utility.IsDomandaIndiretta(pensione) && !this.areaDanteCausa.IsFascicoloGenerato.GetValueOrDefault()))
                {
                    ddlTipoCalcolo.Enabled = false;
                }

                if (Utility.IsDomandaReversibilita(pensione))
                {
                    chkProvvisoria.Checked = true;
                    chkProvvisoria.Enabled = false;
                }

            }
        }

        //Gestione visibilità per TRF/RIC Esodati
        private void ManageTipoCalcoloTraRicEsodati(AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda, AreaTitolare.DatiPensione pensione, DatiLiquidazionePensioneStorico datiStorico)
        {
            string categoria = domanda.Categoria;

            if (CodeUtility.IsRicostituzioneOrRiapertura(pensione, domanda.IsDomandaRiapertura))
            {
                if (Utility.IsDomandaVOCOOP(categoria) || Utility.IsDomandaVOESO(categoria) || Utility.IsDomandaVOCRED(categoria) ||
                    Utility.IsDomandaVESO33(categoria) || Utility.IsDomandaVESO92(categoria) || Utility.IsDomandaVESO29(categoria))
                {
                    if (datiStorico != null && datiStorico.Contributivo == '8')
                        ddlTipoCalcolo.Enabled = false;
                }
            }

        }

        //Gestione visibilità per VESO33 VESO92
        private void ManageVESO33_VESO92(AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda, AreaTitolare.DatiPensione pensione, bool? isDomandaVESO92WithFiltroL92)
        {
            string categoria = domanda.Categoria;
            string codeTipo = pensione.CodeTipo;

            if (Utility.IsDomandaVESO33(categoria))
            {
                //finalizzata a pensione anticipata quota 100
                if (codeTipo == "0054" && !CodeUtility.IsRicostituzioneOrRiapertura(TitolarePensione.Pensione, this.domanda.IsDomandaRiapertura))
                {
                    ddlCodNatura1DG.SelectedValue = ddlCodNatura1DG.Items.FindByText("1").Value;
                    ddlCodNatura1DG.Enabled = false;
                    ddlCodNatura3DG.SelectedValue = ddlCodNatura3DG.Items.FindByText("Q").Value;
                    ddlCodNatura3DG.Enabled = false;
                }
            }
            if (Utility.IsDomandaVESO33(categoria) || Utility.IsDomandaVESO92(categoria))
            {
                //per VESO92 e filtro L92 il calcolo deve essere bloccato a 'Contributivo'
                if ((Utility.IsDomandaVESO92(categoria) && isDomandaVESO92WithFiltroL92 == true)
                    || (CodeUtility.IsRicostituzione(pensione) && Utility.IsDomandaVESO92WithGP2BB05(this.domanda.Categoria, this.domanda.GP2BB05)))
                {
                    if (ddlTipoCalcolo.Items.FindByText("Contributivo") != null)
                        ddlTipoCalcolo.SelectedValue = ddlTipoCalcolo.Items.FindByText("Contributivo").Value;
                    ddlTipoCalcolo.Enabled = false;
                }

                //Cod Natura 1
                if (codeTipo == "0038")
                    //finalizzata a vecchiaia
                    ddlCodNatura1DG.SelectedValue = ddlCodNatura1DG.Items.FindByText(" ").Value;
                else if (codeTipo == "0039")
                    //finalizzata a pensione anticipata / anticipata quota 100
                    ddlCodNatura1DG.SelectedValue = ddlCodNatura1DG.Items.FindByText("1").Value;

                this.ddlCodNatura1DG.Enabled = false;

                if (CodeUtility.IsRicostituzioneOrRiapertura(TitolarePensione.Pensione, this.domanda.IsDomandaRiapertura))
                    ddlCodNatura3DG.Enabled = false;

                //Campi “Cieco / Ex Combattente”, “Maggiorazioni Sociali”, “Trattenuta INPDAP” e “Decorrenza Trattenuta INPDAP” resi non editabili.
                chkExCombattente.Enabled = false;
                chkMaggiorazioni.Enabled = false;
                ddlTrattINPDAP.ClearSelection();
                ddlTrattINPDAP.Enabled = false;
                txtDecTrattINPDAP.Text = string.Empty;
                txtDecTrattINPDAP.Enabled = false;
            }
        }

        //Gestione visibilità per ESPA
        private void ManageESPA(AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda, AreaTitolare.DatiPensione pensione)
        {
            string categoria = domanda.Categoria;
            string codeTipo = pensione.CodeTipo;
            if (Utility.IsDomandaESPA(categoria))
            {
                //Cod Natura 1
                if (codeTipo == "0038")
                    //finalizzata a vecchiaia
                    ddlCodNatura1DG.SelectedValue = ddlCodNatura1DG.Items.FindByText(" ").Value;
                else if (codeTipo == "0039")
                    //finalizzata a pensione anticipata
                    ddlCodNatura1DG.SelectedValue = ddlCodNatura1DG.Items.FindByText("1").Value;

                this.ddlCodNatura1DG.Enabled = false;

                if (CodeUtility.IsRicostituzioneOrRiapertura(TitolarePensione.Pensione, this.domanda.IsDomandaRiapertura))
                    ddlCodNatura3DG.Enabled = false;

                //Campi “Cieco / Ex Combattente”, “Maggiorazioni Sociali”, “Trattenuta INPDAP” e “Decorrenza Trattenuta INPDAP” resi non editabili.
                chkExCombattente.Enabled = false;
                chkMaggiorazioni.Enabled = false;
                ddlTrattINPDAP.ClearSelection();
                ddlTrattINPDAP.Enabled = false;
                txtDecTrattINPDAP.Text = string.Empty;
                txtDecTrattINPDAP.Enabled = false;
            }
        }

        //Gestione visibilità per VOCOOP VOESO e VOCRED
        private void ManageVOCOOP_VOESO_VOCRED_CRED27_COOP28(AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda)
        {
            string categoria = domanda.Categoria;
            if (Utility.IsDomandaVOCOOP_COOP28(categoria) || Utility.IsDomandaVOESO(categoria) || Utility.IsDomandaVOCRED_CRED27(categoria))
            {
                this.ddlCodNatura2DG.Enabled = false;

                //Campi “Cieco / Ex Combattente”, “Maggiorazioni Sociali” resi non editabili.
                chkExCombattente.Enabled = false;
                chkMaggiorazioni.Enabled = false;

                ddlTrattINPDAP.ClearSelection();
                ddlTrattINPDAP.Enabled = false;
                txtDecTrattINPDAP.Text = string.Empty;
                txtDecTrattINPDAP.Enabled = false;
            }

            bool isTrfRicVOCOOP_VOESO_VOCRED = CodeUtility.IsRicostituzioneOrRiapertura(TitolarePensione.Pensione, this.domanda.IsDomandaRiapertura) && (Utility.IsDomandaVOCOOP(categoria) || Utility.IsDomandaVOCRED(categoria) || Utility.IsDomandaVOESO(categoria));

            if (Utility.IsDomandaCRED27(this.domanda.Categoria) || Utility.IsDomandaCOOP28(this.domanda.Categoria))
            {
                if (this.domanda.CodTipo == "0038")
                    ddlCodNatura1DG.SelectedValue = ddlCodNatura1DG.Items.FindByText(" ").Value;
                else if (this.domanda.CodTipo == "0039")
                    ddlCodNatura1DG.SelectedValue = ddlCodNatura1DG.Items.FindByText("1").Value;
                else if (this.domanda.CodTipo == "0054" && !isTrfRicVOCOOP_VOESO_VOCRED)
                {
                    ddlCodNatura1DG.SelectedValue = ddlCodNatura1DG.Items.FindByText("1").Value;
                    ddlCodNatura3DG.SelectedValue = ddlCodNatura3DG.Items.FindByText("Q").Value;
                    ddlCodNatura3DG.Enabled = false;
                }
                ddlCodNatura1DG.Enabled = false;
            }

            if (isTrfRicVOCOOP_VOESO_VOCRED)
            {
                ddlCodNatura3DG.Enabled = false;
                ddlCodNatura1DG.Enabled = false;
            }
        }

        private void ManageVESO29_ESOTEL_ESOAMB()
        {
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];


            bool isDomandaVESO29 = Utility.IsDomandaVESO29(this.domanda.Categoria);
            bool isDomandaESOTEL = Utility.IsDomandaESOTEL(this.domanda.Categoria);
            bool isDomandaESOAMB = Utility.IsDomandaESOAMB(this.domanda.Categoria);

            if (isDomandaVESO29 || isDomandaESOTEL || isDomandaESOAMB)
            {
                // Campi non editabili
                chkExCombattente.Enabled = false;
                chkMaggiorazioni.Enabled = false;

                ddlTrattINPDAP.ClearSelection();
                ddlTrattINPDAP.Enabled = false;
                txtDecTrattINPDAP.Text = string.Empty;
                txtDecTrattINPDAP.Enabled = false;

                bool isTrfRicVESO29 = CodeUtility.IsRicostituzioneOrRiapertura(TitolarePensione.Pensione, this.domanda.IsDomandaRiapertura) && isDomandaVESO29;

                // Finalizzata a pensione anticipata quota 100
                if (this.domanda.CodTipo == "0054" && !isTrfRicVESO29)
                {
                    ddlCodNatura1DG.SelectedValue = ddlCodNatura1DG.Items.FindByText("1").Value;
                    ddlCodNatura1DG.Enabled = false;
                    ddlCodNatura3DG.SelectedValue = ddlCodNatura3DG.Items.FindByText("Q").Value;
                    ddlCodNatura3DG.Enabled = false;
                }

                if (isTrfRicVESO29)
                {
                    ddlCodNatura1DG.Enabled = false;
                    ddlCodNatura2DG.Enabled = false;
                    ddlCodNatura3DG.Enabled = false;
                }

                if (isDomandaESOTEL)
                {
                    ddlCodNatura1DG.Enabled = false;
                }
            }

            if (isDomandaESOTEL)
            {
                validateDataCompletezza.Visible = false;
                lblDataCompletezza.Visible = false;
                txtDataCompletezza.Visible = false;
                lblInteressiLegali.Visible = false;
                txtInteressiLegali.Visible = false;
            }
        }

        private void ManageRipristini(ILiquidazionePensioneAgo liquidazioneAgo, AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda)
        {
            if (Utility.IsDomandaRipristino(this.TitolarePensione.Pensione) && liquidazioneAgo != null)
            {

                if (liquidazioneAgo.areaLiquidazionePensioneAgo.DatiGenerici.TipoCalcolo.HasValue)
                    ddlTipoCalcolo.Enabled = false;

                ddlCodNatura1DG.Enabled = true;
                if (!string.IsNullOrEmpty(liquidazioneAgo.areaLiquidazionePensioneAgo.DatiGenerici.NaturaPensione))
                {
                    ddlCodNatura2DG.Enabled = false;
                    ddlCodNatura3DG.Enabled = false;
                    if (liquidazioneAgo.areaLiquidazionePensioneAgo.DatiGenerici.NaturaPensione.StartsWith("5"))
                        ddlCodNatura1DG.Enabled = false;
                }


                trCompletezzaAndInteressiLegali.Visible = false;
                lblInteressiLegali.Visible = false;
                txtInteressiLegali.Visible = false;
                txtInteressiLegali.Text = null;
                lblDataCompletezza.Visible = false;
                txtDataCompletezza.Visible = false;
                txtDataCompletezza.Text = null;
                chkBenefici.Enabled = false;
                HiddenFieldChkBeneficiDisabled.Value = "true";
                chkExCombattente.Enabled = true;

                if (!domanda.IsDomandaRiapertura && (this.domanda.Categoria.StartsWith("I") || this.domanda.Categoria.StartsWith("S") || (this.domanda.Categoria.StartsWith("V") && liquidazioneAgo.areaLiquidazionePensioneAgo.DatiGenerici.NaturaPensione.Substring(0, 1) == "5")))
                {
                    ddlTrattINPDAP.SelectedIndex = 0;
                    ddlTrattINPDAP.Enabled = false;
                    txtDecTrattINPDAP.Text = string.Empty;
                    txtDecTrattINPDAP.Enabled = false;
                }

                if (Utility.IsDomandaSOAUT(this.domanda.Categoria))
                {
                    ddlTrattINPDAP.SelectedIndex = 0;
                    ddlTrattINPDAP.Enabled = false;
                    txtDecTrattINPDAP.Text = string.Empty;
                    txtDecTrattINPDAP.Enabled = false;
                    if (liquidazioneAgo.areaLiquidazionePensioneAgo.DatiGenerici.NaturaPensione.Substring(0, 1) == "5")
                    {
                        chkExCombattente.Enabled = false;
                    }
                }
                if (Utility.IsDomandaBancari(this.domanda.Categoria))
                {
                    ddlCodiciArretrati.Enabled = false;
                }
            }
        }

        private void ManageRiliquidazioni(AreaTitolare.DatiPensione datiPensione, ILiquidazionePensioneAgo liquidazioneAgo, AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda)
        {
            if (Utility.IsDomandaRiliquidazione(this.TitolarePensione.Pensione) && liquidazioneAgo != null)
            {
                chkExCombattente.Enabled = true;
                ddlCodDomRicorso.SelectedValue = "8";
                if (Utility.IsDomandaSOAUT(this.domanda.Categoria) && !domanda.IsDomandaRiapertura)
                {
                    ddlTrattINPDAP.SelectedIndex = 0;
                    ddlTrattINPDAP.Enabled = false;
                    txtDecTrattINPDAP.Text = string.Empty;
                    txtDecTrattINPDAP.Enabled = false;
                    if (liquidazioneAgo.areaLiquidazionePensioneAgo.DatiGenerici != null && liquidazioneAgo.areaLiquidazionePensioneAgo.DatiGenerici.NaturaPensione != null
                        && liquidazioneAgo.areaLiquidazionePensioneAgo.DatiGenerici.NaturaPensione.Substring(0, 1) == "5")
                    {
                        chkExCombattente.Enabled = false;
                    }
                }
                chkMaggiorazioni.Enabled = true;
                chkBenefici.Enabled = true;


            }
        }

        private void ManageDomandeTotilizzazione(ILiquidazionePensioneAgo liquidazioneAgo, AreaTitolare.DatiPensione datiPensione)
        {
            if (Utility.IsDomandaTotalizzazione(this.domanda.Categoria))
            {
                CodeUtility.TipologiaPensioneGruppo tipologiaGruppoPensione = CodeUtility.TipologiaPensioneGruppo.gr_NessunValore;
                CodeUtility.TipologiaPensioneProdotto tipologiaProdottoPensione = CodeUtility.TipologiaPensioneProdotto.pr_NessunValore;
                CodeUtility.TipologiaPensioneTipo tipologiaTipoPensione = CodeUtility.TipologiaPensioneTipo.tp_NessunValore;
                CodeUtility.GetTipologiaPensione(datiPensione.CodeGruppo, datiPensione.CodeProdotto, datiPensione.CodeTipo, out tipologiaGruppoPensione, out tipologiaProdottoPensione, out tipologiaTipoPensione);

                if (Utility.IsDomandaVOTOT(this.domanda.Categoria))
                {
                    if (liquidazioneAgo != null && liquidazioneAgo.areaLiquidazionePensioneAgo.DatiGenerici.EnteCassa.HasValue
                        && liquidazioneAgo.areaLiquidazionePensioneAgo.DatiGenerici.EnteCassa != 1)
                    {
                        ddlTrattINPDAP.Enabled = false;
                        txtDecTrattINPDAP.Enabled = false;
                    }
                    ddlCodNatura2DG.Enabled = false;
                }
                else if (Utility.IsDomandaSOTOT(this.domanda.Categoria))
                {
                    //ENG - Integrazione Modifiche Accenture
                    if (Utility.IsRicostituzione(this.domanda.CodGruppo))
                    {
                        chkBenefici.Enabled = true;
                        HiddenFieldChkBeneficiDisabled.Value = "false";
                    }

                    ddlTrattINPDAP.ClearSelection();
                    ddlTrattINPDAP.Enabled = false;
                    txtDecTrattINPDAP.Text = string.Empty;
                    txtDecTrattINPDAP.Enabled = false;
                }
                else if (Utility.IsDomandaIOTOT(this.domanda.Categoria) && tipologiaProdottoPensione == CodeUtility.TipologiaPensioneProdotto.pr_InabilitaPensione)
                {
                    ddlTrattINPDAP.Enabled = true;
                    txtDecTrattINPDAP.Enabled = true;
                }
                else
                {
                    ddlTrattINPDAP.Enabled = false;
                    txtDecTrattINPDAP.Enabled = false;
                }

                ddlEnteIstruttoreFondoExInpdap.Visible = false;
                lblEnteIstruttore.Visible = false;
                ddlCodMobilita.ClearSelection();
                ddlCodMobilita.Enabled = false;

                if (datiPensione.IsDomandaTotAutomatica || CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura))
                    ddlContributivoCum.Enabled = false;

                if (isVOCUM_C9A(datiPensione, this.domanda))
                {
                    ddlContributivoCum.SelectedValue = "8";
                    ddlContributivoCum.Enabled = false;
                }
            }
        }

        private bool isVOCUM_C9A(AreaTitolare.DatiPensione datiPensione, AreaRispostaRiepilogo.DatiRiepilogoDomanda Domanda)
        {
            // Se è una Domanda Organizzazioni Internazionali(C9), VOCUM  e una delle 4 triplette restituisce true
            //0001-0001-0017 || 0001-0002-0017 || 0001-0001-0030 || 0001-0002-0030
            bool retVal = false;
            if (datiPensione.CodiceTipoRichiesta == "C9" && Utility.IsDomandaVOCUM(Domanda.Categoria) &&
                ((datiPensione.CodeGruppo == "0001" && datiPensione.CodeProdotto == "0001" && datiPensione.CodeTipo == "0017")
                || (datiPensione.CodeGruppo == "0001" && datiPensione.CodeProdotto == "0002" && datiPensione.CodeTipo == "0017")
                || (datiPensione.CodeGruppo == "0001" && datiPensione.CodeProdotto == "0001" && datiPensione.CodeTipo == "0030")
                || (datiPensione.CodeGruppo == "0001" && datiPensione.CodeProdotto == "0002" && datiPensione.CodeTipo == "0030")))
            {
                retVal = true;
            }

            return retVal;
        }

        private void ManageTipoCalcolo(string siglaCategoria, AreaTitolare.DatiPensione datiPensione, ILiquidazionePensioneAgo liquidazioneAgo)
        {
            if (Utility.IsDomandaCumulo(siglaCategoria) || Utility.IsDomandaTotalizzazione(this.domanda.Categoria))
            {
                pnlTipoCalcolo.Visible = false;
                pnlTipoCalcoloCum.Visible = true;
                pnlDatiGenericiCum.Visible = true;
            }
            else if (Utility.IsDomandaVESO92_L92(siglaCategoria, datiPensione.Filtro) || (Utility.IsDomandaVESO29(siglaCategoria) && !string.IsNullOrEmpty(datiPensione.Filtro) && datiPensione.Filtro.Trim() == "FS") ||
                     Utility.IsDomandaVOCRED_CRED27_DAP(this.domanda.Categoria, datiPensione.Filtro) || Utility.IsDomandaAPESociale(siglaCategoria) || Utility.IsDomandaSPED(siglaCategoria) ||
                     (Utility.IsDomandaVOESO(siglaCategoria) && !string.IsNullOrEmpty(datiPensione.Filtro) && datiPensione.Filtro.Trim() == "FS") ||
                     (Utility.IsDomandaESOTEL(siglaCategoria)) ||
                     (Utility.IsDomandaESOAMB(siglaCategoria) && !string.IsNullOrEmpty(datiPensione.Filtro) && datiPensione.Filtro.Trim() == "L26") ||
                     (CodeUtility.IsRicostituzione(datiPensione) && Utility.IsIsoPensioneWithGP2BB05(siglaCategoria, this.domanda.GP2BB05)) || Utility.IsDomandaINDCOM(this.domanda.Categoria)
                     || (Utility.IsDomandaVOESO(siglaCategoria) && !string.IsNullOrEmpty(datiPensione.Filtro) && datiPensione.Filtro == "ESA" && Utility.IsAssegnoStraordinarioRiscossioneTributiErariali(datiPensione)) ||
                     Utility.IsDomandaRenditaCasalinghe(this.domanda.Categoria) || Utility.IsDomandaRenditaFacoltativa(this.domanda.Categoria) ||
                     Utility.IsDomandaVOST(siglaCategoria) || Utility.IsDomandaPSO(siglaCategoria) || liquidazioneAgo.areaLiquidazionePensioneAgo.IsDomandaESPAFiltroL26.GetValueOrDefault() ||
                     liquidazioneAgo.areaLiquidazionePensioneAgo.IsDomandaVESO33FiltroDAP.GetValueOrDefault() || datiPensione.IsRicExtracalcolo.GetValueOrDefault() || liquidazioneAgo.areaLiquidazionePensioneAgo.IsDomandaCOOP28FiltroDAP.GetValueOrDefault())
            {
                pnlTipoCalcolo.Visible = false;
            }

            //ENG - Aggiornamento Memo 68/2022 IOPGI
            //ENG - Spacchettate SOPGI
            if ((Utility.IsDomandaVOPGI(siglaCategoria) || (Utility.IsDomandaIOPGI(siglaCategoria) && !Utility.IsDomandaIOPGI_AGI(siglaCategoria, datiPensione.Filtro)) || Utility.IsDomandaSpacchettamentoSOPGIPost072022(this.domanda.Categoria, datiPensione, this.areaDanteCausa)) && pnlTipoCalcolo.Visible == true)
            {
                lblTipoCalcolo.Text = "Tipo Calcolo AGO:";
            }
        }

        private void ManageForPensioniVecchiaiaCalcoloContrib(ILiquidazionePensioneAgo liquidazione, AreaTitolare.DatiPensione datiPensione)
        {
            if (datiPensione.CodeGruppo.Trim() == "0001" && datiPensione.CodeProdotto.Trim() == "0002" && datiPensione.CodeTipo.Trim() == "0017")
            {
                var itemTipoCalcolo = ddlTipoCalcolo.Items.FindByText("Contributivo");
                if (itemTipoCalcolo != null)
                {
                    ddlTipoCalcolo.SelectedValue = itemTipoCalcolo.Value;
                    ddlTipoCalcolo.Enabled = false;
                }
                if (!string.IsNullOrEmpty(datiPensione.CodiceTipoRichiesta) && datiPensione.CodiceTipoRichiesta == "92")
                {
                    var itemCodNat2 = ddlCodNatura2DG.Items.FindByText("I");
                    if (itemCodNat2 != null)
                    {
                        ddlCodNatura2DG.SelectedValue = itemCodNat2.Value;
                        ddlCodNatura2DG.Enabled = false;
                    }
                }
            }

            //FG - Controlli tipo contributivo
            if (liquidazione.areaLiquidazionePensioneAgo.IsPensioneTipoContributivo.GetValueOrDefault() ||
                datiPensione.IsDomandaAnticipataFlessibileOpzioneContributivoOrRicostituzione ||
                datiPensione.IsDomandaVecchiaiaAOICalcoloContributivo) //ENG - MEMO 166/2023
            {
                var itemTipoCalcolo = ddlTipoCalcolo.Items.FindByText("Contributivo");
                if (itemTipoCalcolo != null)
                {
                    ddlTipoCalcolo.SelectedValue = itemTipoCalcolo.Value;
                    ddlTipoCalcolo.Enabled = false;
                }

                var itemCodiceMobilita = ddlCodMobilita.Items.FindByText("");
                if (itemCodiceMobilita != null)
                {
                    ddlCodMobilita.SelectedValue = itemCodiceMobilita.Value;
                    ddlCodMobilita.Enabled = false;
                }
            }

            //ENG - Memo 123/2024
            if ((!CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura) && (datiPensione.IsDomandaAnticipataFlessibileLeggeBilancio2024OrRicostituzione || datiPensione.IsDomandaAnticipataFlessibileLeggeBilancio2024OpzioneContributivoOrRicostituzione)) ||
                (CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura) && ((ViewState["AbilitazioneRIC_TRFMemo123_2024"] != null && (string)ViewState["AbilitazioneRIC_TRFMemo123_2024"].ToString().Trim().ToUpperInvariant() == "SI" && datiPensione.IsDomandaAnticipataFlessibileLeggeBilancio2024OrRicostituzione)
                || (ViewState["AbilitazioneRIC_TRFMemo123_2024OpzioneContrib"] != null && (string)ViewState["AbilitazioneRIC_TRFMemo123_2024OpzioneContrib"].ToString().Trim().ToUpperInvariant() == "SI" && datiPensione.IsDomandaAnticipataFlessibileLeggeBilancio2024OpzioneContributivoOrRicostituzione))) ||
                datiPensione.IsDomandaVOAUTAnticipataFlessibileLeggeBilancio2024FiltroGSEOrRicostituzione)
            {
                var itemTipoCalcolo = ddlTipoCalcolo.Items.FindByText("Contributivo");
                if (itemTipoCalcolo != null)
                {
                    ddlTipoCalcolo.SelectedValue = itemTipoCalcolo.Value;
                    ddlTipoCalcolo.Enabled = false;
                }

                if (datiPensione.IsDomandaAnticipataFlessibileLeggeBilancio2024OpzioneContributivoOrRicostituzione)
                {
                    ddlCodNatura2DG.ClearSelection();
                    if (ddlCodNatura2DG.Items.FindByValue("J") != null)
                        ddlCodNatura2DG.SelectedValue = "J";
                    ddlCodNatura2DG.Enabled = false;
                }
            }
        }

        private void ManageNaturaPerINPDAI(AreaLiquidazionePensione areaLiquidazionePensione)
        {
            if (areaLiquidazionePensione != null && areaLiquidazionePensione.IsDatiCalcoloDAIAltraGestionePresent.HasValue)
            {
                if (areaLiquidazionePensione.IsDatiCalcoloDAIAltraGestionePresent.Value)
                {
                    if (ddlCodNatura2DG.Items.FindByText("E") != null)
                        ddlCodNatura2DG.SelectedValue = ddlCodNatura2DG.Items.FindByText("E").Value;
                    ddlCodNatura2DG.Enabled = false;
                }
                else
                {
                    if (ddlCodNatura2DG.Items.FindByText("B") != null)
                        ddlCodNatura2DG.SelectedValue = ddlCodNatura2DG.Items.FindByText("B").Value;
                    ddlCodNatura2DG.Enabled = false;
                }
            }
        }

        private void ManageRenditaFacoltativa_Casalinghe(AreaTitolare.DatiPensione datiPensione)
        {
            string categoria = this.domanda.Categoria;
            if (Utility.IsDomandaRenditaCasalinghe(categoria) || Utility.IsDomandaRenditaFacoltativa(categoria))
            {
                chkProvvisoria.Enabled = false;
                ddlTrattINPDAP.ClearSelection();
                ddlTrattINPDAP.Enabled = false;
                txtDecTrattINPDAP.Text = string.Empty;
                txtDecTrattINPDAP.Enabled = false;
                chkExCombattente.Enabled = false;
                ddlCodNatura2DG.Enabled = false;
                ddlCodNatura3DG.Enabled = false;

                if (CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura) && ddlCodNatura1DG.SelectedValue == "5")
                    ddlCodNatura1DG.Enabled = false;
            }
        }

        private void ManageVOST()
        {
            if (Utility.IsDomandaVOST(this.domanda.Categoria))
            {
                ddlCodNatura3DG.Enabled = false;
            }
        }

        private void ManagePSO()
        {
            if (Utility.IsDomandaPSO(this.domanda.Categoria))
            {
                ddlCodNatura2DG.ClearSelection();
                ddlCodNatura3DG.ClearSelection();
                ddlCodNatura2DG.Enabled = false;
                ddlCodNatura3DG.Enabled = false;
                chkExCombattente.Checked = false;
                chkExCombattente.Enabled = false;
                chkProvvisoria.Enabled = false;
                if (this.domanda.CodGruppo == "0005" && this.domanda.CodProdotto == "0043" && (this.domanda.CodTipo == "0014" || this.domanda.CodTipo == "0015"))
                {
                    chkBenefici.Enabled = false;
                    HiddenFieldChkBeneficiDisabled.Value = "true";
                    chkMaggiorazioni.Enabled = false;
                }
            }
        }

        private void ManagePMO(AreaTitolare.DatiPensione datiPensione)
        {
            if (Utility.IsDomandaPMO(this.domanda.Categoria))
            {
                chkProvvisoria.Enabled = false;
                ddlCodNatura3DG.ClearSelection();
                ddlCodNatura3DG.Enabled = false;
                if (!CodeUtility.IsRicostituzione(datiPensione) && !Utility.IsDomandaReversibilita(datiPensione))
                {
                    ddlTipoCalcolo.Enabled = false;
                    if (ddlTipoCalcolo.Items.FindByText("RETRIBUTIVO/RETRIBUTIVO EX COMMA 707") != null)
                        ddlTipoCalcolo.SelectedValue = ddlTipoCalcolo.Items.FindByText("RETRIBUTIVO/RETRIBUTIVO EX COMMA 707").Value;
                }
            }
        }

        private void ManageAnte96(AreaLiquidazionePensione areaLiquidazionePensioneAgo)
        {
            if (areaLiquidazionePensioneAgo != null && areaLiquidazionePensioneAgo.IsAnte96 != null)
            {
                switch (areaLiquidazionePensioneAgo.IsAnte96)
                {
                    case Presenter.SvrLiquidazioneAgo.UtilityTipoAnte96.Ante96Retributive:
                        if (ddlTipoCalcolo.Items.FindByText("RETRIBUTIVO/RETRIBUTIVO EX COMMA 707") != null)
                        {
                            ddlTipoCalcolo.SelectedValue = ddlTipoCalcolo.Items.FindByText("RETRIBUTIVO/RETRIBUTIVO EX COMMA 707").Value;
                            ddlTipoCalcolo.Enabled = false;
                        }
                        break;
                    case Presenter.SvrLiquidazioneAgo.UtilityTipoAnte96.Ante96Miste:
                        if (ddlTipoCalcolo.Items.FindByText("Misto") != null)
                        {
                            ddlTipoCalcolo.SelectedValue = ddlTipoCalcolo.Items.FindByText("Misto").Value;
                            ddlTipoCalcolo.Enabled = false;
                        }
                        break;
                    case Presenter.SvrLiquidazioneAgo.UtilityTipoAnte96.Ante96Contributive:
                        if (areaLiquidazionePensioneAgo.IsDatiRetributiviPresenti.GetValueOrDefault() && areaLiquidazionePensioneAgo.IsDatiContributiviPresenti.GetValueOrDefault())
                        {
                            if (ddlTipoCalcolo.Items.FindByText("Misto") != null)
                            {
                                ddlTipoCalcolo.SelectedValue = ddlTipoCalcolo.Items.FindByText("Misto").Value;
                                ddlTipoCalcolo.Enabled = false;
                            }
                        }
                        else
                        {
                            if (ddlTipoCalcolo.Items.FindByText("Contributivo") != null)
                            {
                                ddlTipoCalcolo.SelectedValue = ddlTipoCalcolo.Items.FindByText("Contributivo").Value;
                                ddlTipoCalcolo.Enabled = false;
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        private void ManageTipoCalcoloRicNonContributive(AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda, AreaTitolare.DatiPensione datiPensione)
        {
            string categoria = domanda.Categoria;

            if (CodeUtility.IsRicostituzione(datiPensione) && !CodeUtility.IsRicostituzioneContributiva(datiPensione) && !Utility.IsRicostituzione_AccreditoPeriodiMaternita(datiPensione))
            {
                if (Utility.IsDomandaFPLD(this.domanda.Categoria) || Utility.IsDomandaGestioneAutonomi(this.domanda.Categoria) || Utility.IsDomandaDAI(categoria) ||
                    Utility.IsDomandaAUT(categoria))
                {
                    ddlTipoCalcolo.Enabled = false;
                }
            }
        }

        private void ManageBancRicAnte1991(AreaTitolare.DatiPensione datiPensione)
        {
            this.domandaDante = this.domanda;
            this.numDomanda = long.Parse(this.domanda.NumeroDomanda);
            PresenterDanteCausa presenterDanteCausa = new PresenterDanteCausa();
            presenterDanteCausa.GetDatiDanteCausa(this);

            if (Utility.IsDomandaBancRicAnte1991(this.domanda.SiglaCategoriaPensione, datiPensione, this.areaDanteCausa))
            {
                pnlTipoCalcolo.Visible = false;
                pnlTrasformazioneAOI.Visible = false;
            }
        }

        //Gestione visibilità per ESOPMI
        private void ManageESOPMI(AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda, AreaTitolare.DatiPensione pensione)
        {
            string categoria = domanda.Categoria;
            string codeTipo = pensione.CodeTipo;
            if (Utility.IsDomandaESOPMI(categoria))
            {
                //Cod Natura 1
                if (codeTipo == "0038")
                    //finalizzata a vecchiaia
                    ddlCodNatura1DG.SelectedValue = ddlCodNatura1DG.Items.FindByText(" ").Value;
                else if (codeTipo == "0039")
                    //finalizzata a pensione anticipata
                    ddlCodNatura1DG.SelectedValue = ddlCodNatura1DG.Items.FindByText("1").Value;

                this.ddlCodNatura1DG.Enabled = false;

                if (CodeUtility.IsRicostituzioneOrRiapertura(TitolarePensione.Pensione, this.domanda.IsDomandaRiapertura))
                    ddlCodNatura3DG.Enabled = false;

                //Campi “Cieco / Ex Combattente”, “Maggiorazioni Sociali”, “Trattenuta INPDAP” e “Decorrenza Trattenuta INPDAP” resi non editabili.
                //chkExCombattente.Enabled = false;
                //chkMaggiorazioni.Enabled = false;
                //ddlTrattINPDAP.ClearSelection();
                //ddlTrattINPDAP.Enabled = false;
                //txtDecTrattINPDAP.Text = string.Empty;
                //txtDecTrattINPDAP.Enabled = false;
            }
        }

        internal Presenter.SvrLiquidazioneAgo.DatiGenerici GetDatiGenerici()
        {
            if (TitolarePensione == null)
                TitolarePensione = new AreaTitolare();
            TitolarePensione.Pensione = GetDatiPensione(this);

            INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneAgo.AreaLiquidazionePensione areaLiquidazionePensioneAgo = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneAgo.AreaLiquidazionePensione();
            areaLiquidazionePensioneAgo.DatiGenerici = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneAgo.DatiGenerici();

            string naturaPensione = "";
            naturaPensione = String.Concat(ddlCodNatura1DG.SelectedValue, ddlCodNatura2DG.SelectedValue, ddlCodNatura3DG.SelectedValue);
            areaLiquidazionePensioneAgo.DatiGenerici.NaturaPensione = naturaPensione;

            if (Utility.IsDomandaCumulo(this.domanda.Categoria))
            {
                //Pensioni in regime di cumulo
                if (!string.IsNullOrEmpty(ddlContributivoCum.SelectedValue))
                    areaLiquidazionePensioneAgo.DatiGenerici.Contributivo = char.Parse(ddlContributivoCum.SelectedValue);
                if (!string.IsNullOrEmpty(ddlEnteCassa.SelectedValue))
                    areaLiquidazionePensioneAgo.DatiGenerici.EnteCassa = long.Parse(ddlEnteCassa.SelectedValue);
                else if (!string.IsNullOrEmpty(hdnEnteCassa.Value))
                    areaLiquidazionePensioneAgo.DatiGenerici.EnteCassa = long.Parse(hdnEnteCassa.Value);
                if (!string.IsNullOrEmpty(ddlEnteIstruttoreFondoExInpdap.SelectedValue))
                    areaLiquidazionePensioneAgo.DatiGenerici.EnteIstruttoreExInpdap = bool.Parse(ddlEnteIstruttoreFondoExInpdap.SelectedValue);
                if (!string.IsNullOrEmpty(ddlTipoCumulo.SelectedValue))
                    areaLiquidazionePensioneAgo.DatiGenerici.TipoCumulo = CodeUtility.StringToNullableBool(ddlTipoCumulo.SelectedValue);
                if (!string.IsNullOrEmpty(ddlCumuloEsterno.SelectedValue))
                    areaLiquidazionePensioneAgo.DatiGenerici.CumuloEsterno = CodeUtility.StringToNullableChar(ddlCumuloEsterno.SelectedValue);

                //Se Tipo Cumulo è Interno (true), il campo CumuloEsterno non deve essere valorizzato
                if (areaLiquidazionePensioneAgo.DatiGenerici.TipoCumulo.GetValueOrDefault())
                    areaLiquidazionePensioneAgo.DatiGenerici.CumuloEsterno = null;
            }
            else if (Utility.IsDomandaTotalizzazione(this.domanda.Categoria))
            {
                //Pensioni TOT
                if (!string.IsNullOrEmpty(ddlContributivoCum.SelectedValue))
                    areaLiquidazionePensioneAgo.DatiGenerici.Contributivo = char.Parse(ddlContributivoCum.SelectedValue);
                if (!string.IsNullOrEmpty(ddlEnteCassa.SelectedValue))
                    areaLiquidazionePensioneAgo.DatiGenerici.EnteCassa = long.Parse(ddlEnteCassa.SelectedValue);
                else if (!string.IsNullOrEmpty(hdnEnteCassa.Value))
                    areaLiquidazionePensioneAgo.DatiGenerici.EnteCassa = long.Parse(hdnEnteCassa.Value);
            }
            else
            {
                if (!String.IsNullOrEmpty(ddlTipoCalcolo.SelectedValue))
                    areaLiquidazionePensioneAgo.DatiGenerici.TipoCalcolo = byte.Parse(ddlTipoCalcolo.SelectedValue);
                else
                    areaLiquidazionePensioneAgo.DatiGenerici.TipoCalcolo = null;
                if (!string.IsNullOrEmpty(HiddenContributivoStorico.Value))
                    areaLiquidazionePensioneAgo.DatiGenerici.Contributivo = char.Parse(HiddenContributivoStorico.Value);
            }
            if (!String.IsNullOrEmpty(ddlCodiciArretrati.SelectedValue))
                areaLiquidazionePensioneAgo.DatiGenerici.CodiceArretrati = byte.Parse(ddlCodiciArretrati.SelectedValue);
            else
                areaLiquidazionePensioneAgo.DatiGenerici.CodiceArretrati = null;

            if (!string.IsNullOrEmpty(txtDecorrenzaArretrati.Text) && !txtDecorrenzaArretrati.Text.ToUpperInvariant().Equals("MM/AAAA"))
                areaLiquidazionePensioneAgo.DatiGenerici.DecorrenzaCalcoloArretrati = Utility.GetDateFromString(txtDecorrenzaArretrati.Text);
            else
                areaLiquidazionePensioneAgo.DatiGenerici.DecorrenzaCalcoloArretrati = null;

            if (!string.IsNullOrEmpty(txtScadRevSanitaria.Text) && !txtScadRevSanitaria.Text.ToUpperInvariant().Equals("MM/AAAA"))
                areaLiquidazionePensioneAgo.DatiGenerici.ScadenzaRevisioneSanitaria = Utility.GetDateFromString(txtScadRevSanitaria.Text);
            else
                areaLiquidazionePensioneAgo.DatiGenerici.ScadenzaRevisioneSanitaria = null;

            if (!string.IsNullOrEmpty(txtDataCompletezza.Text) && !txtDataCompletezza.Text.ToUpperInvariant().Equals("GG/MM/AAAA"))
                areaLiquidazionePensioneAgo.DatiGenerici.DataCompletezza = Utility.GetDateFromString(txtDataCompletezza.Text);
            else
                areaLiquidazionePensioneAgo.DatiGenerici.DataCompletezza = null;

            if (!string.IsNullOrEmpty(HiddenIntLeg.Value) && !HiddenIntLeg.Value.ToUpperInvariant().Equals("GG/MM/AAAA"))
                areaLiquidazionePensioneAgo.DatiGenerici.DataInteressiLegali = Utility.GetDateFromString(HiddenIntLeg.Value);
            else
                areaLiquidazionePensioneAgo.DatiGenerici.DataInteressiLegali = null;

            if (!String.IsNullOrEmpty(ddlCausaCarico.SelectedValue))
                areaLiquidazionePensioneAgo.DatiGenerici.CausaCarico = byte.Parse(ddlCausaCarico.SelectedValue);
            else
                areaLiquidazionePensioneAgo.DatiGenerici.CausaCarico = null;

            if (ddlCausaCarico.SelectedValue != "9")
                areaLiquidazionePensioneAgo.DatiGenerici.DataInizioCalcolo = null;
            else
            {
                if (!string.IsNullOrEmpty(txtDataCalcolo.Text) && !txtDataCalcolo.Text.ToUpperInvariant().Equals("MM/AAAA"))
                    areaLiquidazionePensioneAgo.DatiGenerici.DataInizioCalcolo = Utility.GetDateFromString(txtDataCalcolo.Text);
                else
                    areaLiquidazionePensioneAgo.DatiGenerici.DataInizioCalcolo = null;
            }

            if (!String.IsNullOrEmpty(ddlEsenzioneFiscale.SelectedValue))
                areaLiquidazionePensioneAgo.DatiGenerici.CodiceComunicazioneCampo4 = byte.Parse(ddlEsenzioneFiscale.SelectedValue);
            else
                areaLiquidazionePensioneAgo.DatiGenerici.CodiceComunicazioneCampo4 = null;

            if (!String.IsNullOrEmpty(ddlModalitaLiquidazione.SelectedValue))
                areaLiquidazionePensioneAgo.DatiGenerici.ModalitaLiquidazione = ddlModalitaLiquidazione.SelectedValue;
            else
                areaLiquidazionePensioneAgo.DatiGenerici.ModalitaLiquidazione = null;

            if (!String.IsNullOrEmpty(ddlCodMobilita.SelectedValue))
                areaLiquidazionePensioneAgo.DatiGenerici.CodiceMobilita = byte.Parse(ddlCodMobilita.SelectedValue);
            else
                areaLiquidazionePensioneAgo.DatiGenerici.CodiceMobilita = null;

            if (!String.IsNullOrEmpty(ddlCodDomRicorso.SelectedValue))
                areaLiquidazionePensioneAgo.DatiGenerici.CodiceDomandaRicorso = byte.Parse(ddlCodDomRicorso.SelectedValue);
            else
                areaLiquidazionePensioneAgo.DatiGenerici.CodiceDomandaRicorso = null;

            //areaLiquidazionePensioneAgo.DatiGenerici.Benefici = chkBenefici.Checked == true ? chkBenefici.Checked : false;
            areaLiquidazionePensioneAgo.DatiGenerici.Benefici = HiddenFieldChkBenefici.Value == "true" ? true : false;
            areaLiquidazionePensioneAgo.DatiGenerici.ExCombattente = chkExCombattente.Checked == true ? chkExCombattente.Checked : false;
            areaLiquidazionePensioneAgo.DatiGenerici.TrasformazioneAOI = chkTrasfAOI.Checked == true ? chkTrasfAOI.Checked : false;
            if (pnlRichiestaBonus.Visible)
            {
                areaLiquidazionePensioneAgo.DatiGenerici.IsRichiestaBonus = chkRichiestaBonus.Checked == true ? chkRichiestaBonus.Checked : false;
                AreaTitolare.DatiPensione datiPensione = CodeUtility.GetDatiPensioneFromSession();
                datiPensione.IsRichiestaBonus = areaLiquidazionePensioneAgo.DatiGenerici.IsRichiestaBonus;
                if (datiPensione.CodeTipo != "0167" && chkRichiestaBonus.Checked == true)
                    areaLiquidazionePensioneAgo.DatiGenerici.AnnoDecorrenzaBonus = !String.IsNullOrEmpty(txtAnnoBonus.Text) && !txtAnnoBonus.Text.ToUpperInvariant().Equals("AAAA") ? txtAnnoBonus.Text : string.Empty;
                else
                    areaLiquidazionePensioneAgo.DatiGenerici.AnnoDecorrenzaBonus = !String.IsNullOrEmpty(hdnAnnoRichiestaBonus14.Value) ? hdnAnnoRichiestaBonus14.Value : string.Empty;
                Session["DatiPensione"] = datiPensione;
            }

            if (String.Equals(ddlTrattINPDAP.SelectedValue, "SI"))
                areaLiquidazionePensioneAgo.DatiGenerici.TrattenutaInpdap = true;
            else if (String.Equals(ddlTrattINPDAP.SelectedValue, "NO"))
                areaLiquidazionePensioneAgo.DatiGenerici.TrattenutaInpdap = false;
            else if (String.Equals(ddlTrattINPDAP.SelectedValue, ""))
                areaLiquidazionePensioneAgo.DatiGenerici.TrattenutaInpdap = null;

            if (!string.IsNullOrEmpty(txtDecTrattINPDAP.Text) && !txtDecTrattINPDAP.Text.ToUpperInvariant().Equals("MM/AAAA"))
                areaLiquidazionePensioneAgo.DatiGenerici.DataRinunciaTrattenutaInpdap = Utility.GetDateFromString(txtDecTrattINPDAP.Text);
            else
                areaLiquidazionePensioneAgo.DatiGenerici.DataRinunciaTrattenutaInpdap = null;

            areaLiquidazionePensioneAgo.DatiGenerici.NRiconoscimentiInvalidita = !String.IsNullOrEmpty(ddlConfermeInvalidita.SelectedValue) ? byte.Parse(ddlConfermeInvalidita.SelectedValue) : (byte?)null;


            areaLiquidazionePensioneAgo.DatiGenerici.CodiceLiquidazione = !String.IsNullOrEmpty(txtCodiceLiquidazione.Text) ? char.Parse(txtCodiceLiquidazione.Text) : (char?)null;
            areaLiquidazionePensioneAgo.DatiGenerici.FlagProvvisoria = chkProvvisoria.Checked == true ? chkProvvisoria.Checked : false;
            areaLiquidazionePensioneAgo.DatiGenerici.Maggiorazioni = chkMaggiorazioni.Checked == true ? chkMaggiorazioni.Checked : false;
            areaLiquidazionePensioneAgo.DatiGenerici.TrattamentoDisagi = HiddenTrattamentoDisagi.Value == "true" ? true : (HiddenTrattamentoDisagi.Value == "false" ? false : (bool?)null);

            return areaLiquidazionePensioneAgo.DatiGenerici;
        }

        internal void SetHiddenPrecedentePensioneValue(string value)
        {
            this.HiddenPrecedentePensione.Value = value;
        }

        internal void SetHiddenENPALS()
        {
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            HiddenENPALS.Value = (this.domanda.IsDomandaENPALS).ToString();
        }

        internal void ManageCodNatura()
        {
            Presenter.PresenterLiquidazionePensione presenterLiquidazione = new PresenterLiquidazionePensione();
            if (this.domanda == null)
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            presenterLiquidazione.GetLiquidazionePensioneAgo(this);

            Presenter.SvrLiquidazioneAgo.DatiIstruttoria datiIstruttoria = this.areaLiquidazionePensioneAgo.DatiIstruttoria;
            Presenter.SvrLiquidazioneAgo.DatiGenerici datiGenerici = this.areaLiquidazionePensioneAgo.DatiGenerici;

            if (datiIstruttoria != null && datiIstruttoria.CodiceBancaEsodati.HasValue && datiGenerici != null && !string.IsNullOrEmpty(datiGenerici.NaturaPensione))
                ddlCodNatura3DG.SelectedValue = datiGenerici.NaturaPensione.Substring(2, 1);

            if (datiIstruttoria != null && datiIstruttoria.Attivitausuranti.HasValue && datiGenerici != null && !string.IsNullOrEmpty(datiGenerici.NaturaPensione))
                ddlCodNatura1DG.SelectedValue = datiGenerici.NaturaPensione.Substring(0, 1);
        }

        #region Private Methods

        private void BindClick()
        {
            //chkBenefici.Attributes.Add("onclick", "javascript:SetCheckBox(this)");
            chkExCombattente.Attributes.Add("onclick", "javascript:SetCheckBox(this)");
            chkTrasfAOI.Attributes.Add("onclick", "javascript:SetCheckBox(this)");
            chkProvvisoria.Attributes.Add("onclick", "javascript:SetCheckBox(this)");
            chkMaggiorazioni.Attributes.Add("onclick", "javascript:SetCheckBox(this)");
            //sospeso in attesa di indicazioni circa i valori del secondo campo per valore 1° dropdownlist = 1: Sede
            //ddlCodComunicazioni1.Attributes.Add("onChange", "javascript:getDDLCodComunicazioni1Value()");
            ddlCodNatura2DG.Attributes.Add("onChange", "javascript:getDDLCodNatura2Value()");
            //txtInteressiLegali.Attributes.Add("onmouseout", "setDataInteressiLegali()");
            txtDataCompletezza.Attributes.Add("onblur", "setDataInteressiLegali()");
        }

        private void AddInputClass()
        {
            chkBenefici.InputAttributes.Add("EnableClass", "onClassBenefici");
            chkExCombattente.InputAttributes.Add("EnableClass", "onClassExCombattente");
            chkTrasfAOI.InputAttributes.Add("EnableClass", "onClassTrasfAOI");
            chkProvvisoria.InputAttributes.Add("EnableClass", "onClassProvvisoria");
            chkMaggiorazioni.InputAttributes.Add("EnableClass", "onClassMaggiorazioni");
        }

        private void LoadDdl(ILiquidazionePensioneAgo liquidazioneAgo, AreaTitolare.DatiPensione datiPensione)
        {
            if (this.domanda == null)
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            if (liquidazioneAgo != null && liquidazioneAgo.areaLiquidazionePensioneAgo != null)
            {
                if (liquidazioneAgo.areaLiquidazionePensioneAgo.listaDomandaRicorso != null && liquidazioneAgo.areaLiquidazionePensioneAgo.listaDomandaRicorso.Count() > 0)
                {
                    if (ddlCodDomRicorso.Items.Count == 0)
                    {
                        CodeUtility.SetValueDdl(ddlCodDomRicorso, string.Empty, string.Empty, string.Empty);
                        foreach (DomandaRicorso codeDomandaRicorso in liquidazioneAgo.areaLiquidazionePensioneAgo.listaDomandaRicorso)
                            CodeUtility.SetValueDdl(ddlCodDomRicorso, codeDomandaRicorso.Id.ToString() + " - " + codeDomandaRicorso.Descrizione, codeDomandaRicorso.Descrizione, codeDomandaRicorso.Id.ToString());
                    }
                }

                if (liquidazioneAgo.areaLiquidazionePensioneAgo.listaCodiciNatura != null && liquidazioneAgo.areaLiquidazionePensioneAgo.listaCodiciNatura.Count() > 0)
                {
                    CodeUtility.TipologiaPensioneGruppo tipologiaGruppoPensione = CodeUtility.TipologiaPensioneGruppo.gr_NessunValore;
                    CodeUtility.TipologiaPensioneProdotto tipologiaProdottoPensione = CodeUtility.TipologiaPensioneProdotto.pr_NessunValore;
                    CodeUtility.TipologiaPensioneTipo tipologiaTipoPensione = CodeUtility.TipologiaPensioneTipo.tp_NessunValore;
                    CodeUtility.GetTipologiaPensione(datiPensione.CodeGruppo, datiPensione.CodeProdotto, datiPensione.CodeTipo, out tipologiaGruppoPensione, out tipologiaProdottoPensione, out tipologiaTipoPensione);


                    if (ddlCodNatura1DG.Items.Count == 0 && ddlCodNatura2DG.Items.Count == 0 && ddlCodNatura3DG.Items.Count == 0)
                    {
                        CodeUtility.SetValueDdl(ddlCodNatura2DG, string.Empty, string.Empty, " ");
                        CodeUtility.SetValueDdl(ddlCodNatura3DG, string.Empty, string.Empty, " ");
                        foreach (Presenter.SvrLiquidazioneAgo.CodiciNatura codeNatura in liquidazioneAgo.areaLiquidazionePensioneAgo.listaCodiciNatura)
                        {
                            if (codeNatura.Posizione == 1)
                                CodeUtility.SetValueDdl(ddlCodNatura1DG, codeNatura.TraduzioneSuGP.ToString(), codeNatura.Descrizione, codeNatura.TraduzioneSuGP.ToString());
                            else if (codeNatura.Posizione == 2)
                                CodeUtility.SetValueDdl(ddlCodNatura2DG, codeNatura.TraduzioneSuGP.ToString(), codeNatura.Descrizione, codeNatura.TraduzioneSuGP.ToString());
                            else
                                CodeUtility.SetValueDdl(ddlCodNatura3DG, codeNatura.TraduzioneSuGP.ToString(), codeNatura.Descrizione, codeNatura.TraduzioneSuGP.ToString());
                        }
                    }
                }

                if (liquidazioneAgo.areaLiquidazionePensioneAgo.listaModalitaLiquidazione != null && liquidazioneAgo.areaLiquidazionePensioneAgo.listaModalitaLiquidazione.Count() > 0)
                {
                    if (ddlModalitaLiquidazione.Items.Count == 0)
                    {
                        CodeUtility.SetValueDdl(ddlModalitaLiquidazione, string.Empty, string.Empty, string.Empty);
                        foreach (DecModalitaLiquidazione codeModLiquidazione in liquidazioneAgo.areaLiquidazionePensioneAgo.listaModalitaLiquidazione)
                            CodeUtility.SetValueDdl(ddlModalitaLiquidazione, codeModLiquidazione.Descrizione, codeModLiquidazione.TraduzioneGp.ToString(), codeModLiquidazione.ValoreAggPeco);
                    }
                }

                if (liquidazioneAgo.areaLiquidazionePensioneAgo.listaRiconoscimentiInvalidita != null && liquidazioneAgo.areaLiquidazionePensioneAgo.listaRiconoscimentiInvalidita.Count() > 0)
                {
                    if (ddlConfermeInvalidita.Items.Count == 0)
                    {
                        CodeUtility.SetValueDdl(ddlConfermeInvalidita, string.Empty, string.Empty, string.Empty);
                        foreach (DecodificaRiconoscimentiInvalidita codeRiconoscimentiInvalidita in liquidazioneAgo.areaLiquidazionePensioneAgo.listaRiconoscimentiInvalidita)
                            if (codeRiconoscimentiInvalidita.Id != 0)
                                CodeUtility.SetValueDdl(ddlConfermeInvalidita, string.Concat(codeRiconoscimentiInvalidita.Id.ToString(), " - ", codeRiconoscimentiInvalidita.Descrizione), codeRiconoscimentiInvalidita.Descrizione, codeRiconoscimentiInvalidita.Id.ToString());
                        //CodeUtility.SetValueDdl(ddlConfermeInvalidita, codeRiconoscimentiInvalidita.Descrizione, codeRiconoscimentiInvalidita.Descrizione, codeRiconoscimentiInvalidita.Id.ToString());
                    }
                }


                if (Utility.IsDomandaCumulo(this.domanda.Categoria) || Utility.IsDomandaTotalizzazione(this.domanda.Categoria))
                {
                    //Pensioni in regime di cumulo
                    if (ddlEnteCassa.Items.Count == 0)
                    {
                        List<string> codiciDaEscludere = new List<string>() { "0803", "0804", "0812" };
                        CodeUtility.SetValueDdl(ddlEnteCassa, string.Empty, string.Empty, string.Empty);
                        foreach (DecodificaEnteCassaProfessionale item in liquidazioneAgo.areaLiquidazionePensioneAgo.listaDecodificaEnteCassaProfessionale)
                        {
                            if (!(Utility.IsDomandaTotalizzazione(this.domanda.Categoria) && codiciDaEscludere.Contains(item.TraduzioneSuGP)))
                                CodeUtility.SetValueDdl(ddlEnteCassa, string.Concat(item.TraduzioneSuGP, " - ", item.Descrizione), item.Descrizione, item.Id.ToString());
                        }
                    }
                }
            }

            CodeUtility areaDecodifica = new CodeUtility();
            Presenter.SvrLiquidazione.AreaDecodifica datiDecodifica = areaDecodifica.GetValuesDecodifica();

            Presenter.SvrLiquidazione.AreaDecodifica.DatiTipoCalcolo[] listaTipoCalcolo = datiDecodifica.ElencoTipoCalcolo;// areaDecodifica.GetValuesDecodifica().ElencoTipoCalcolo;

            if (ddlTipoCalcolo.Items.Count == 0)
            {
                var isMinContributiva = ((liquidazioneAgo.areaLiquidazionePensioneAgo.IsPensioneTipoContributivoConOpzione.GetValueOrDefault() || liquidazioneAgo.areaLiquidazionePensioneAgo.IsPensioneTipoContributivo.GetValueOrDefault() || datiPensione.IsDomandaAnticipataFlessibileOpzioneContributivoOrRicostituzione || datiPensione.IsDomandaVecchiaiaAOICalcoloContributivo ||
                                (!CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura) && (datiPensione.IsDomandaAnticipataFlessibileLeggeBilancio2024OrRicostituzione || datiPensione.IsDomandaAnticipataFlessibileLeggeBilancio2024OpzioneContributivoOrRicostituzione)) ||
                                (CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura) && ((ViewState["AbilitazioneRIC_TRFMemo123_2024"] != null && (string)ViewState["AbilitazioneRIC_TRFMemo123_2024"].ToString().Trim().ToUpperInvariant() == "SI" && datiPensione.IsDomandaAnticipataFlessibileLeggeBilancio2024OrRicostituzione)
                                || (ViewState["AbilitazioneRIC_TRFMemo123_2024OpzioneContrib"] != null && (string)ViewState["AbilitazioneRIC_TRFMemo123_2024OpzioneContrib"].ToString().Trim().ToUpperInvariant() == "SI" && datiPensione.IsDomandaAnticipataFlessibileLeggeBilancio2024OpzioneContributivoOrRicostituzione)))) && Utility.IsDomandaVOMIN(this.domanda.Categoria));

                CodeUtility.SetValueDdl(ddlTipoCalcolo, string.Empty, string.Empty, string.Empty);
                foreach (AreaDecodifica.DatiTipoCalcolo tipoCalcolo in listaTipoCalcolo)
                    if ((tipoCalcolo.Tipologia == "AGO" && tipoCalcolo.Tipo.Trim() == "Inps" && (tipoCalcolo.TraduzioneSuGP == 1 || tipoCalcolo.TraduzioneSuGP == 2 || tipoCalcolo.TraduzioneSuGP == 9)) ||
                        (this.domanda.IsDomandaENPALS && tipoCalcolo.Tipologia == "AGO" && tipoCalcolo.Tipo.Trim() == "Enpals" && (tipoCalcolo.TraduzioneSuGP == 9)) ||
                        (this.domanda.IsDomandaENPALS && tipoCalcolo.Tipologia == "AGO" && tipoCalcolo.Tipo.Trim() == "Enpals" && (tipoCalcolo.TraduzioneSuGP == 2)))
                    {
                        if (liquidazioneAgo != null && liquidazioneAgo.areaLiquidazionePensioneAgo != null &&
                            (liquidazioneAgo.areaLiquidazionePensioneAgo.IsPrepensionamentoEditoriaArt1c500L160_2019.GetValueOrDefault() || liquidazioneAgo.areaLiquidazionePensioneAgo.IsPrepensionamentoEditoriaFiltroEAA.GetValueOrDefault()
                            || (datiPensione.IsDomandaAnticipataFlessibileOrRicostituzione && !Utility.IsDomandaAUT(this.domanda.Categoria) && !CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura)))
                            && tipoCalcolo.TraduzioneSuGP == 1)
                            continue;

                        if ((tipoCalcolo.TraduzioneSuGP == 1 && liquidazioneAgo != null && liquidazioneAgo.areaLiquidazionePensioneAgo != null &&
                            liquidazioneAgo.areaLiquidazionePensioneAgo.DatiGenerici != null &&
                            liquidazioneAgo.areaLiquidazionePensioneAgo.DatiGenerici.TipoCalcolo.GetValueOrDefault() == 2 &&
                            Utility.IsDomandaEsodo(this.domanda.Categoria) && CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura)) ||
                            (this.domanda.Categoria.Trim() == "VO" && Utility.IsDomandaManualeInvaliditaOver80(datiPensione) && tipoCalcolo.TraduzioneSuGP.GetValueOrDefault() == 1))//Per VO invalidità >= 80% non prendo il tipo calcolo Contributivo
                            continue;
                        string descrizione = tipoCalcolo.Descrizione;
                        if (CodeUtility.IsRicostituzione(datiPensione) && Utility.IsDomandaINPGI(this.domanda.Categoria) && tipoCalcolo.TraduzioneSuGP.GetValueOrDefault() == 2)
                            descrizione = "RETRIBUTIVO/MISTO";
                        else if (!this.domanda.IsDomandaENPALS && tipoCalcolo.Tipo.Trim() == "Inps" && tipoCalcolo.TraduzioneSuGP.GetValueOrDefault() == 2)
                            descrizione = "RETRIBUTIVO/RETRIBUTIVO EX COMMA 707";
                        if (!((Utility.IsDomandaSPED(this.domanda.Categoria) || (Utility.IsDomandaVOMIN(this.domanda.Categoria) && !isMinContributiva) || Utility.IsDomandaVOP_PL_VecchiaiaAnzianita(this.domanda.Categoria, CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura), datiPensione) || Utility.IsDomandaVOBANC_PL_VecchiaiaAnzianita(this.domanda.Categoria, CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura), datiPensione)) && tipoCalcolo.Tipologia == "AGO" && tipoCalcolo.Tipo.Trim() == "Inps" && tipoCalcolo.TraduzioneSuGP == 1))
                            CodeUtility.SetValueDdl(ddlTipoCalcolo, descrizione, descrizione, tipoCalcolo.Id);
                    }
            }

            Presenter.SvrLiquidazione.AreaDecodifica.DatiCausaCarico[] listaCausaCarico = datiDecodifica.ElencoCausaCarico;// areaDecodifica.GetValuesDecodifica().ElencoCausaCarico;

            if (ddlCausaCarico.Items.Count == 0)
            {
                var isRicRendite = CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura) && (Utility.IsDomandaRenditaCasalinghe(this.domanda.Categoria) || Utility.IsDomandaRenditaFacoltativa(this.domanda.Categoria));
                CodeUtility.SetValueDdl(ddlCausaCarico, string.Empty, string.Empty, string.Empty);
                foreach (AreaDecodifica.DatiCausaCarico causaCarico in listaCausaCarico)
                    if (causaCarico.Id == "1" || causaCarico.Id == "9" || isRicRendite)
                        CodeUtility.SetValueDdl(ddlCausaCarico, causaCarico.Id + " - " + causaCarico.Descrizione, causaCarico.Descrizione, causaCarico.Id);
            }

            AreaDecodifica.DatiCodeMobilita[] listaCodeMobilita = datiDecodifica.ElencoCodeMobilita;

            if (ddlCodMobilita.Items.Count == 0)
            {
                List<string> codiciDaAmmettereBanc = new List<string>() { "1", "2", "3", "4", "8" };
                CodeUtility.SetValueDdl(ddlCodMobilita, string.Empty, string.Empty, string.Empty);
                foreach (AreaDecodifica.DatiCodeMobilita codeMobilita in listaCodeMobilita)
                {
                    if (Utility.IsDomandaBancari(this.domanda.Categoria) && !CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura))
                    {
                        if (codiciDaAmmettereBanc.Contains(codeMobilita.Id))
                            CodeUtility.SetValueDdl(ddlCodMobilita, codeMobilita.Descrizione, codeMobilita.Descrizione, codeMobilita.Id);
                    }
                    else
                        CodeUtility.SetValueDdl(ddlCodMobilita, codeMobilita.Descrizione, codeMobilita.Descrizione, codeMobilita.Id);
                }
            }

            AreaDecodifica.DatiComunicazioneCampo4[] listaComunicazioneC4 = datiDecodifica.ElencoComunicazioneCampo4;

            if (ddlEsenzioneFiscale.Items.Count == 0)
            {
                CodeUtility.SetValueDdl(ddlEsenzioneFiscale, "NESSUNA ESENZIONE", "NESSUNA ESENZIONE", string.Empty);

                foreach (AreaDecodifica.DatiComunicazioneCampo4 comunicazioneCampo4 in listaComunicazioneC4)
                {
                    if (CodeUtility.LoadRecordEsenzioneFiscaleAGO_CI(comunicazioneCampo4.Id, datiPensione.CodeGruppo, this.domanda.IsDomandaRiapertura, liquidazioneAgo.areaLiquidazionePensioneAgo.IsEsenzioneFiscaleEstero, liquidazioneAgo.areaLiquidazionePensioneAgo.IsEsenzioneFiscaleVittima))
                        if (!this.domanda.Categoria.StartsWith("S") && CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura)
                            && comunicazioneCampo4.Id == "1")
                            CodeUtility.SetValueDdl(ddlEsenzioneFiscale, "ESENZIONE FISCALE VITTIME TERRORISMO/DOVERE", "ESENZIONE FISCALE VITTIME TERRORISMO/DOVERE", comunicazioneCampo4.Id);
                        else
                            CodeUtility.SetValueDdl(ddlEsenzioneFiscale, comunicazioneCampo4.Descrizione, comunicazioneCampo4.Descrizione, comunicazioneCampo4.Id);
                }
            }
        }

        private void GestioneEtichetteIsUnicarpe(AreaTitolare.DatiPensione datiPensione)
        {
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            Utility.TipoUnicarpe tipoUnicarpe = Utility.IsDomandaUnicarpe(datiPensione, true);
            if (tipoUnicarpe == Utility.TipoUnicarpe.Automatica)
            {
                //pnlScadRevSanitaria.Enabled = false;
                ddlTipoCalcolo.Enabled = false;
                ddlCodMobilita.Enabled = false;
                ddlModalitaLiquidazione.Enabled = false;
                if (CodeUtility.IsRicostituzione(datiPensione) && !this.domanda.IsDomandaENPALS)
                {
                    chkBenefici.Enabled = false;
                    HiddenFieldChkBeneficiDisabled.Value = "true";
                }
            }
        }

        private void ClearForm()
        {
            CodeUtility.ClearForm(this, 0);
            SetDefaultValue();
        }

        private void SetDefaultValue()
        {
            txtScadRevSanitaria.Text = "MM/AAAA";
            txtDecorrenzaArretrati.Text = "MM/AAAA";
            txtDataCompletezza.Text = "GG/MM/AAAA";
            txtDataCalcolo.Text = "MM/AAAA";
            txtDecTrattINPDAP.Text = "MM/AAAA";
        }

        private void ManageTrasformazioneAOI(AreaTitolare.DatiPensione datiPensione)
        {

            if (this.domanda == null)
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            if (datiPensione == null)
                datiPensione = GetDatiPensione(this);

            CodeUtility.TipologiaPensioneGruppo tipologiaGruppoPensione = CodeUtility.TipologiaPensioneGruppo.gr_NessunValore;
            CodeUtility.TipologiaPensioneProdotto tipologiaProdottoPensione = CodeUtility.TipologiaPensioneProdotto.pr_NessunValore;
            CodeUtility.TipologiaPensioneTipo tipologiaTipoPensione = CodeUtility.TipologiaPensioneTipo.tp_NessunValore;
            CodeUtility.GetTipologiaPensione(datiPensione.CodeGruppo, datiPensione.CodeProdotto, datiPensione.CodeTipo, out tipologiaGruppoPensione, out tipologiaProdottoPensione, out tipologiaTipoPensione);

            if (tipologiaTipoPensione == CodeUtility.TipologiaPensioneTipo.tp_Vecchiaia_TrasfAOI)
            {
                pnlTrasformazioneAOI.Visible = true;
                chkTrasfAOI.Checked = true;
                chkTrasfAOI.Enabled = false;
            }

            if ((this.domanda.IsDomandaRiapertura && !Utility.IsDomandaRipristinoOrRiliquidazione(datiPensione)) || tipologiaGruppoPensione == CodeUtility.TipologiaPensioneGruppo.gr_Ricostituzione)
            {
                pnlTrasformazioneAOI.Visible = true;
                chkTrasfAOI.Enabled = false;
            }
        }

        private void ManageCheckBenefici(AreaTitolare.DatiPensione datiPensione, ILiquidazionePensioneAgo liquidazione)
        {
            if (datiPensione == null)
                datiPensione = GetDatiPensione(this);

            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            //ENG - memo 28/2024   
            string controlloDinamico28_2024 = string.Empty;
            if (ViewState["AbilitazioneMemo28_2024"] != null)
                controlloDinamico28_2024 = (string)ViewState["AbilitazioneMemo28_2024"];
            else
            {
                Presenter.PresenterControlliDinamici presenter = new PresenterControlliDinamici();
                Presenter.SvrLiquidazione.AreaEsito esito = presenter.GetControlloDinamicoByNomeControllo("AbilitazioneMemo28_2024", out controlloDinamico28_2024);
                if (esito != null && esito.RisultatoOperazione == Presenter.SvrLiquidazione.AreaEsito.TipoEsito.OK)
                    ViewState["AbilitazioneMemo28_2024"] = controlloDinamico28_2024;
            }


            CodeUtility.TipologiaPensioneGruppo tipologiaGruppoPensione = CodeUtility.TipologiaPensioneGruppo.gr_NessunValore;
            CodeUtility.TipologiaPensioneProdotto tipologiaProdottoPensione = CodeUtility.TipologiaPensioneProdotto.pr_NessunValore;
            CodeUtility.TipologiaPensioneTipo tipologiaTipoPensione = CodeUtility.TipologiaPensioneTipo.tp_NessunValore;
            CodeUtility.GetTipologiaPensione(datiPensione.CodeGruppo, datiPensione.CodeProdotto, datiPensione.CodeTipo, out tipologiaGruppoPensione, out tipologiaProdottoPensione, out tipologiaTipoPensione);
            Utility.TipoUnicarpe tipoUnicarpe = Utility.IsDomandaUnicarpe(datiPensione, true);

            if (!(liquidazione.areaLiquidazionePensioneAgo != null && liquidazione.areaLiquidazionePensioneAgo.DatiGenerici != null && liquidazione.areaLiquidazionePensioneAgo.DatiGenerici.Benefici.HasValue && !liquidazione.areaLiquidazionePensioneAgo.DatiGenerici.Benefici.Value) &&
                ((Utility.IsDomandaVOESO(this.domanda.Categoria) && !CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura)) || Utility.IsDomandaVESO33(this.domanda.Categoria) || Utility.IsDomandaVOCRED_CRED27(this.domanda.Categoria) ||
                 Utility.IsDomandaVOCOOP_COOP28(this.domanda.Categoria) || Utility.IsDomandaVESO29(this.domanda.Categoria) ||
                 Utility.IsDomandaESOAMB(this.domanda.Categoria) || Utility.IsDomandaESPA(this.domanda.Categoria) || Utility.IsDomandaVESO92(this.domanda.Categoria) ||
                 datiPensione.IsDomandaQuota100OrRicostituzione || datiPensione.IsDomandaQuota102OrRicostituzione || datiPensione.SceltaLavoratriciMadri.HasValue || datiPensione.IsDomandaAnticipataFlessibileOrRicostituzione || datiPensione.IsDomandaAnticipataFlessibileOpzioneContributivoOrRicostituzione ||
                 (liquidazione.areaLiquidazionePensioneAgo != null && (liquidazione.areaLiquidazionePensioneAgo.IsPrepensionamentoEditoriaArt1c154L205_2017.GetValueOrDefault() ||
                  liquidazione.areaLiquidazionePensioneAgo.IsPrepensionamentoEditoriaArt1c500L160_2019.GetValueOrDefault() ||
                  liquidazione.areaLiquidazionePensioneAgo.IsBeneficioInabilitaByPrimoCodiceNatura.GetValueOrDefault() ||
                  (tipoUnicarpe == Utility.TipoUnicarpe.Automatica && liquidazione.areaLiquidazionePensioneAgo.IsBeneficioNonVedente.GetValueOrDefault()) ||
                  liquidazione.areaLiquidazionePensioneAgo.IsBeneficioNonVedenteFromStorico.GetValueOrDefault() ||
                  liquidazione.areaLiquidazionePensioneAgo.IsPrepensionamentoEditoriaFiltroEAA.GetValueOrDefault())) || Utility.IsDomandaESOPMI(this.domanda.Categoria) || liquidazione.areaLiquidazionePensioneAgo.IsPrepensionamentoEditoriaFiltroEBA.GetValueOrDefault() ||
                  (!CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura) && (datiPensione.IsDomandaAnticipataFlessibileLeggeBilancio2024OrRicostituzione || datiPensione.IsDomandaAnticipataFlessibileLeggeBilancio2024OpzioneContributivoOrRicostituzione))
                  || (CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura) && ((ViewState["AbilitazioneRIC_TRFMemo123_2024"] != null && (string)ViewState["AbilitazioneRIC_TRFMemo123_2024"].ToString().Trim().ToUpperInvariant() == "SI" && datiPensione.IsDomandaAnticipataFlessibileLeggeBilancio2024OrRicostituzione)
                  || (ViewState["AbilitazioneRIC_TRFMemo123_2024OpzioneContrib"] != null && (string)ViewState["AbilitazioneRIC_TRFMemo123_2024OpzioneContrib"].ToString().Trim().ToUpperInvariant() == "SI" && datiPensione.IsDomandaAnticipataFlessibileLeggeBilancio2024OpzioneContributivoOrRicostituzione))) ||
                  datiPensione.IsDomandaVOAUTAnticipataFlessibileLeggeBilancio2024FiltroGSEOrRicostituzione))
            {
                chkBenefici.Checked = true;
                HiddenFieldChkBeneficiChecked.Value = "true";
            }

            if (tipologiaProdottoPensione == CodeUtility.TipologiaPensioneProdotto.pr_InabilitaPensione || datiPensione.IsDomandaQuota100OrRicostituzione || datiPensione.IsDomandaQuota102OrRicostituzione || datiPensione.IsDomandaAnticipataFlessibileOrRicostituzione ||
                datiPensione.IsDomandaAnticipataFlessibileOpzioneContributivoOrRicostituzione ||
                (liquidazione.areaLiquidazionePensioneAgo != null &&
                (liquidazione.areaLiquidazionePensioneAgo.IsBeneficioArt24Comma15BisFromFELPE.GetValueOrDefault() || liquidazione.areaLiquidazionePensioneAgo.IsBeneficioApePrecociFromFELPE.GetValueOrDefault() ||
                 liquidazione.areaLiquidazionePensioneAgo.IsPrepensionamentoEditoriaArt1c500L160_2019.GetValueOrDefault() ||
                (tipoUnicarpe == Utility.TipoUnicarpe.Automatica && liquidazione.areaLiquidazionePensioneAgo.IsBeneficioNonVedente.GetValueOrDefault()) ||
                liquidazione.areaLiquidazionePensioneAgo.IsBeneficioNonVedenteFromStorico.GetValueOrDefault()) ||
                liquidazione.areaLiquidazionePensioneAgo.IsPrepensionamentoEditoriaFiltroEAA.GetValueOrDefault()) ||
                 datiPensione.SceltaLavoratriciMadri.HasValue || ((Utility.IsDomandaVESO33(this.domanda.Categoria) || Utility.IsDomandaCRED27(this.domanda.Categoria) || Utility.IsDomandaCOOP28(this.domanda.Categoria) ||
                 Utility.IsDomandaVESO29(this.domanda.Categoria) || Utility.IsDomandaESOAMB(this.domanda.Categoria)) && datiPensione.CodeTipo == "0054") ||
                 Utility.IsDomandaVESO92(this.domanda.Categoria) || Utility.IsDomandaESPA(this.domanda.Categoria) || Utility.IsDomandaESOPMI(this.domanda.Categoria) || liquidazione.areaLiquidazionePensioneAgo.IsPrepensionamentoEditoriaFiltroEBA.GetValueOrDefault() ||
                (!CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura) && (datiPensione.IsDomandaAnticipataFlessibileLeggeBilancio2024OrRicostituzione || datiPensione.IsDomandaAnticipataFlessibileLeggeBilancio2024OpzioneContributivoOrRicostituzione))
                || (CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura) && ((ViewState["AbilitazioneRIC_TRFMemo123_2024"] != null && (string)ViewState["AbilitazioneRIC_TRFMemo123_2024"].ToString().Trim().ToUpperInvariant() == "SI" && datiPensione.IsDomandaAnticipataFlessibileLeggeBilancio2024OrRicostituzione)
                || (ViewState["AbilitazioneRIC_TRFMemo123_2024OpzioneContrib"] != null && (string)ViewState["AbilitazioneRIC_TRFMemo123_2024OpzioneContrib"].ToString().Trim().ToUpperInvariant() == "SI" && datiPensione.IsDomandaAnticipataFlessibileLeggeBilancio2024OpzioneContributivoOrRicostituzione))) ||
                datiPensione.IsDomandaVOAUTAnticipataFlessibileLeggeBilancio2024FiltroGSEOrRicostituzione)
            {
                chkBenefici.Enabled = false;
                HiddenFieldChkBeneficiDisabled.Value = "true";
            }

            if ((this.domanda.IsDomandaENPALS && (tipologiaTipoPensione == CodeUtility.TipologiaPensioneTipo.tp_Precoci ||
                (liquidazione != null && liquidazione.areaLiquidazionePensioneAgo != null &&
                liquidazione.areaLiquidazionePensioneAgo.IsPensioneInvaliditaInabilitaENPALSOrCasellario.HasValue && liquidazione.areaLiquidazionePensioneAgo.IsPensioneInvaliditaInabilitaENPALSOrCasellario.Value))) ||
                Utility.IsDomandaVecchiaiaENAV(datiPensione, domanda.Categoria))
            {
                chkBenefici.Checked = true;
                HiddenFieldChkBeneficiChecked.Value = "true";
                chkBenefici.Enabled = false;
                HiddenFieldChkBeneficiDisabled.Value = "true";
            }

            if (Utility.IsDomandaUsuranti(datiPensione))
            {
                chkBenefici.Checked = false;
                HiddenFieldChkBeneficiChecked.Value = "false";
                chkBenefici.Enabled = false;
                HiddenFieldChkBeneficiDisabled.Value = "true";
            }

            if (this.domanda.Categoria.Trim() == "VMP" || Utility.IsDomandaRenditaFacoltativa(this.domanda.Categoria))
            {
                chkBenefici.Checked = false;
                HiddenFieldChkBeneficiChecked.Value = "false";
                chkBenefici.Enabled = false;
                HiddenFieldChkBeneficiDisabled.Value = "true";
            }

            if (this.domanda.Categoria.Trim() == "VMP" || Utility.IsDomandaRenditaFacoltativa(this.domanda.Categoria) || Utility.IsDomandaPMO(this.domanda.Categoria))
            {
                chkBenefici.Checked = false;
                HiddenFieldChkBeneficiChecked.Value = "false";
                chkBenefici.Enabled = false;
                HiddenFieldChkBeneficiDisabled.Value = "true";
            }

            if (this.domanda.Categoria.Trim() == "IMP" && !CodeUtility.IsRicostituzione(datiPensione) && Utility.DataSuccessivaA(datiPensione.DecorrenzaOriginaria.GetValueOrDefault(), new DateTime(1984, 08, 01)))
            {
                chkBenefici.Checked = true;
                HiddenFieldChkBeneficiChecked.Value = "true";
                chkBenefici.Enabled = false;
                HiddenFieldChkBeneficiDisabled.Value = "true";
            }

            if (Utility.IsDomandaESOTEL(this.domanda.Categoria))
            {
                chkBenefici.Checked = false;
                HiddenFieldChkBeneficiChecked.Value = "false";
                chkBenefici.Enabled = false;
                HiddenFieldChkBeneficiDisabled.Value = "true";
            }

            //ENG - memo 28/2024
            if (!String.IsNullOrEmpty(controlloDinamico28_2024) && controlloDinamico28_2024.Trim().ToUpperInvariant() == "SI")
            {
                if ((datiPensione.CodeGruppo == "0001" && datiPensione.CodeProdotto == "0001" && datiPensione.CodeTipo == "0017") ||
                    (datiPensione.CodeGruppo == "0001" && datiPensione.CodeProdotto == "0001" && datiPensione.CodeTipo == "0045" &&
                    datiPensione.CodiceTipoRichiesta == "AV"))
                {
                    if (!datiPensione.SceltaLavoratriciMadri.HasValue)
                    {
                        HiddenFieldMemo28_2024.Value = "true";
                    }
                }
            }

            //ENG - Sistemazione problematica check benefici per GPT 0001/0002/0017
            if (datiPensione.CodeGruppo == "0001" && datiPensione.CodeProdotto == "0002" && datiPensione.CodeTipo == "0017")
            {
                if (!datiPensione.SceltaLavoratriciMadri.HasValue)
                {
                    HiddenFieldSaltaCheckBenefici0001_0002_0017.Value = "true";
                }
            }
        }

        private void ManageRichiestaBonus(AreaTitolare.DatiPensione datiPensione, DatiGenerici datiGenerici)
        {
            if (this.domanda == null)
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            if (datiPensione == null)
                datiPensione = GetDatiPensione(this);

            if (CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, false) && (datiPensione.CodeProdotto == "0101" || datiPensione.CodeProdotto == "0301" || datiPensione.CodeProdotto == "0401"))
            {
                pnlRichiestaBonus.Visible = true;
                if (datiPensione.CodeTipo == "0167")
                {
                    lblRichiestaBonus.Text = "Bonus 14°:";
                    chkRichiestaBonus.Checked = true;
                    chkRichiestaBonus.Enabled = false;
                    hdnAnnoRichiestaBonus14.Value = datiGenerici.AnnoDecorrenzaBonus;
                }
                else
                {
                    lblRichiestaBonus.Text = "Bonus 154:";
                }
                HiddenAnnoBonusBooking.Value = "SI";

                if (datiPensione.IsRichiestaBonus.HasValue && datiPensione.IsRichiestaBonus.Value)
                {
                    chkRichiestaBonus.Checked = true;
                    if (datiPensione.CodeTipo != "0167" && !string.IsNullOrEmpty(datiGenerici.AnnoDecorrenzaBonus))
                    {
                        txtAnnoBonus.Text = datiGenerici.AnnoDecorrenzaBonus;
                    }
                }
            }
        }

        private void ManageCodiceLiquidazione(AreaTitolare.DatiPensione datiPensione)
        {
            if (datiPensione == null)
                datiPensione = GetDatiPensione(this);

            if (Utility.IsDomandaReversibilita(datiPensione))
                pnlCodiceLiquidazione.Visible = true;
        }

        private void ManageConfermeInvalidita(AreaTitolare.DatiPensione datiPensione, AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda)
        {
            if (datiPensione == null)
                datiPensione = GetDatiPensione(this);

            CodeUtility.TipologiaPensioneGruppo tipologiaGruppoPensione = CodeUtility.TipologiaPensioneGruppo.gr_NessunValore;
            CodeUtility.TipologiaPensioneProdotto tipologiaProdottoPensione = CodeUtility.TipologiaPensioneProdotto.pr_NessunValore;
            CodeUtility.TipologiaPensioneTipo tipologiaTipoPensione = CodeUtility.TipologiaPensioneTipo.tp_NessunValore;
            CodeUtility.GetTipologiaPensione(datiPensione.CodeGruppo, datiPensione.CodeProdotto, datiPensione.CodeTipo, out tipologiaGruppoPensione, out tipologiaProdottoPensione, out tipologiaTipoPensione);

            if (tipologiaProdottoPensione == CodeUtility.TipologiaPensioneProdotto.pr_InvaliditaAssegno || Utility.IsDomandaRipristinoAssegnoInvalidita(datiPensione) || Utility.IsDomandaRliquidazioneAssegnoInvalidita(datiPensione))
                pnlConfermeInvalidita.Visible = true;
        }

        private void ManageModalitaLiquidazione(AreaTitolare.DatiPensione datiPensione)
        {
            if (datiPensione == null)
                datiPensione = GetDatiPensione(this);

            Utility.TipoUnicarpe tipoUnicarpe = Utility.IsDomandaUnicarpe(datiPensione, true);
            if (tipoUnicarpe == Utility.TipoUnicarpe.Automatica)
            {
                pnlModalitaLiquidazione.Visible = true;
                pnlProvvisoria.Visible = false;
            }
        }

        private void ManageCodiceMobilita(AreaTitolare.DatiPensione datiPensione, bool isDomandaENPALS)
        {
            CodeUtility.TipologiaPensioneGruppo tipologiaGruppoPensione = CodeUtility.TipologiaPensioneGruppo.gr_NessunValore;
            CodeUtility.TipologiaPensioneProdotto tipologiaProdottoPensione = CodeUtility.TipologiaPensioneProdotto.pr_NessunValore;
            CodeUtility.TipologiaPensioneTipo tipologiaTipoPensione = CodeUtility.TipologiaPensioneTipo.tp_NessunValore;
            CodeUtility.GetTipologiaPensione(datiPensione.CodeGruppo, datiPensione.CodeProdotto, datiPensione.CodeTipo, out tipologiaGruppoPensione, out tipologiaProdottoPensione, out tipologiaTipoPensione);

            if (tipologiaGruppoPensione == CodeUtility.TipologiaPensioneGruppo.gr_Anzianita_Vecchiaia && !isDomandaENPALS)
                pnlCodiceMobilita.Visible = true;
        }

        private void ManageDomandeAUT(AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda, AreaTitolare.DatiPensione pensione)
        {
            if (Utility.IsDomandaAUT(domanda.Categoria))
            {
                if (ddlTipoCalcolo.Items.FindByText("Contributivo") != null)
                    ddlTipoCalcolo.SelectedValue = ddlTipoCalcolo.Items.FindByText("Contributivo").Value;
                ddlTipoCalcolo.Enabled = false;
                ddlCodMobilita.ClearSelection();
                ddlCodMobilita.Enabled = false;
                txtCodiceLiquidazione.Text = string.Empty;
                txtCodiceLiquidazione.Enabled = false;
                chkMaggiorazioni.Enabled = false;

                if (Utility.IsDomandaAUTAnticipataInComputo(pensione, domanda.Categoria, true))
                {
                    if (ddlCodNatura2DG.Items.FindByText("S") != null)
                    {
                        ddlCodNatura2DG.SelectedValue = ddlCodNatura2DG.Items.FindByText("S").Value;
                        ddlCodNatura2DG.Enabled = false;
                    }
                }
                if (CodeUtility.IsRicostituzioneOrRiapertura(pensione, domanda.IsDomandaRiapertura))
                {
                    ddlCodNatura2DG.Enabled = false;
                    ddlCodNatura3DG.Enabled = false;
                }
                //ENG - Memo 116/2025
                if (pensione.IsDomandaVOAUTVecchiaiaTipoContributivoFiltroGSEOrRicostituzione || pensione.IsDomandaVOAUTAnticipataTipoContributivoFiltroGSEOrRicostituzione
                    || pensione.IsDomandaVOAUTAnticipataFlessibileLeggeBilancio2024FiltroGSEOrRicostituzione)
                {
                    ddlCodNatura2DG.ClearSelection();
                    if (ddlCodNatura2DG.Items.FindByValue("9") != null)
                        ddlCodNatura2DG.SelectedValue = "9";
                    ddlCodNatura2DG.Enabled = false;
                    ddlCodNatura3DG.ClearSelection();
                    if (ddlCodNatura3DG.Items.FindByValue("V") != null)
                        ddlCodNatura3DG.SelectedValue = "V";
                    ddlCodNatura3DG.Enabled = false;
                    pnlINPDAP.Visible = false;
                }
            }
        }

        private void ManageDomandeSPED(AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda, AreaTitolare.DatiPensione pensione)
        {
            if (Utility.IsDomandaSPED(domanda.Categoria))
            {
                ddlCodMobilita.ClearSelection();
                ddlCodMobilita.Enabled = false;
                txtCodiceLiquidazione.Text = string.Empty;
                txtCodiceLiquidazione.Enabled = false;
                chkExCombattente.Enabled = false;
                chkMaggiorazioni.Enabled = false;
                //confermato per tutte le SPED 
                ddlTrattINPDAP.ClearSelection();
                ddlTrattINPDAP.Enabled = false;
                txtDecTrattINPDAP.Text = string.Empty;
                txtDecTrattINPDAP.Enabled = false;

                ddlConfermeInvalidita.Enabled = false;

                if (CodeUtility.IsRicostituzioneOrRiapertura(pensione, domanda.IsDomandaRiapertura))
                {
                    chkExCombattente.Enabled = false;
                    chkMaggiorazioni.Enabled = false;
                    chkBenefici.Enabled = false;
                    HiddenFieldChkBeneficiDisabled.Value = "true";
                    chkTrasfAOI.Enabled = false;
                }
            }
        }

        private void ManageDomandeIndennitaUnaTantum(AreaTitolare.DatiPensione datiPensione)
        {
            if (Utility.IsDomandaIndennitaUnaTantum_AGO(datiPensione))
            {
                if (ddlTipoCalcolo.Items.FindByText("Contributivo") != null)
                    ddlTipoCalcolo.SelectedValue = ddlTipoCalcolo.Items.FindByText("Contributivo").Value;
                ddlTipoCalcolo.Enabled = false;
                ddlTrattINPDAP.ClearSelection();
                ddlTrattINPDAP.Enabled = false;
                txtDecTrattINPDAP.Text = string.Empty;
                txtDecTrattINPDAP.Enabled = false;
                chkExCombattente.Enabled = false;
                chkMaggiorazioni.Enabled = false;
                chkBenefici.Enabled = false;
                HiddenFieldChkBeneficiDisabled.Value = "true";

                chkExCombattente.Checked = false;
                chkMaggiorazioni.Checked = false;
                chkBenefici.Checked = false;
                HiddenFieldChkBeneficiChecked.Value = "false";

                if (ddlCodNatura2DG.Items.FindByText("U") != null)
                    ddlCodNatura2DG.SelectedValue = ddlCodNatura2DG.Items.FindByText("U").Value;
                ddlCodNatura2DG.Enabled = false;
                ddlCodNatura3DG.Enabled = false;

                lblInteressiLegali.Visible = false;
                txtInteressiLegali.Visible = false;
                txtInteressiLegali.Text = null;

                chkProvvisoria.Enabled = false;
            }
        }

        private void ManageDomandeSupplementari(AreaTitolare.DatiPensione datiPensione)
        {
            if (Utility.IsDomandaSupplementare(datiPensione))
            {
                chkBenefici.Enabled = true;
                HiddenFieldChkBeneficiDisabled.Value = "false";
                chkBenefici.Checked = false;
                HiddenFieldChkBeneficiChecked.Value = "false";
                ddlTrattINPDAP.ClearSelection();
                ddlTrattINPDAP.Enabled = false;
                txtDecTrattINPDAP.Text = string.Empty;
                txtDecTrattINPDAP.Enabled = false;
            }
        }

        private void ManageDomandeINDCOM()
        {
            if (Utility.IsDomandaINDCOM(this.domanda.Categoria))
            {
                chkExCombattente.Enabled = false;
                chkMaggiorazioni.Enabled = false;
                chkBenefici.Enabled = false;
                HiddenFieldChkBeneficiDisabled.Value = "true";
                ddlTrattINPDAP.ClearSelection();
                ddlTrattINPDAP.Enabled = false;
                txtDecTrattINPDAP.Text = string.Empty;
                txtDecTrattINPDAP.Enabled = false;
                chkProvvisoria.Enabled = false;
                chkProvvisoria.Checked = false;
                if (!Utility.IsRicostituzione(TitolarePensione.Pensione.CodeGruppo))
                {
                    ddlCodNatura1DG.Enabled = false;
                    ddlCodNatura1DG.ClearSelection();
                }
                ddlCodNatura2DG.Enabled = false;
                ddlCodNatura2DG.ClearSelection();
                ddlCodNatura3DG.Enabled = false;
                ddlCodNatura3DG.ClearSelection();
                lblInteressiLegali.Visible = false;
                txtInteressiLegali.Visible = false;
                txtInteressiLegali.Text = null;
                //qualora fossero presenti
                ddlConfermeInvalidita.Enabled = false;
                ddlConfermeInvalidita.ClearSelection();
                ddlModalitaLiquidazione.Enabled = false;
                ddlModalitaLiquidazione.ClearSelection();

                lblDataRevisioneSanitaria.InnerText = "Scadenza Indennizzo:";
            }
        }

        private void ManageDomandeMIN(AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda, AreaTitolare.DatiPensione pensione)
        {
            if (Utility.IsDomandaVOMIN(domanda.Categoria))
            {
                ddlCodMobilita.ClearSelection();
                ddlCodMobilita.Enabled = false;
            }

            if (Utility.IsDomandaSOMIN(domanda.Categoria))
            {
                ddlTrattINPDAP.ClearSelection();
                ddlTrattINPDAP.Enabled = false;
                txtDecTrattINPDAP.Text = string.Empty;
                txtDecTrattINPDAP.Enabled = false;
            }
        }

        private void ManageDomandePescatori(AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda)
        {
            if (Utility.IsDomandaPescatori(domanda.Categoria))
            {
                ddlCodMobilita.ClearSelection();
                ddlCodMobilita.Enabled = false;
            }
        }

        private void ManageDomandeBancari(AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda, AreaTitolare.DatiPensione pensione)
        {
            if (Utility.IsDomandaBancari(domanda.Categoria))
            {
                ddlTrattINPDAP.ClearSelection();
                ddlTrattINPDAP.Enabled = false;
                txtDecTrattINPDAP.Text = string.Empty;
                txtDecTrattINPDAP.Enabled = false;
                if (Utility.IsDomandaBancari(this.domanda.Categoria) && CodeUtility.IsRicostituzioneOrRiapertura(pensione, this.domanda.IsDomandaRiapertura))
                {
                    ddlCodMobilita.Enabled = false;
                }
            }
        }


        private void GestioneEtichetteRic(AreaTitolare.DatiPensione datiPensione, ILiquidazionePensioneAgo liquidazione)
        {
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            if (datiPensione == null)
                datiPensione = GetDatiPensione(this);

            Utility.TipoUnicarpe tipoUnicarpe = Utility.IsDomandaUnicarpe(datiPensione, true);

            if (this.domanda.IsDomandaRiapertura && tipoUnicarpe == Utility.TipoUnicarpe.Automatica && !this.domanda.Categoria.StartsWith("S"))
            {
                ddlEsenzioneFiscale.Enabled = false;
                chkExCombattente.Enabled = false;
                chkBenefici.Enabled = false;
                HiddenFieldChkBeneficiDisabled.Value = "true";
                chkMaggiorazioni.Enabled = false;
                ddlTrattINPDAP.Enabled = false;
                txtDecTrattINPDAP.Enabled = false;
            }

            ddlCodiciArretrati.Enabled = false;
            if (!Utility.IsDomandaINDCOM(this.domanda.Categoria))
                pnlScadRevSanitaria.Enabled = false;
            chkProvvisoria.Enabled = false;
            pnlCodDomRicorso.Visible = false;
            ddlEnteCassa.Enabled = false;
            txtDataCalcolo.Enabled = false;

            if (!this.domanda.Categoria.StartsWith("S"))
            {
                ddlCodNatura2DG.Enabled = false;
                if (liquidazione != null && liquidazione.areaLiquidazionePensioneAgo != null && liquidazione.areaLiquidazionePensioneAgo.DatiLiquidazionePensioneStorico != null &&
                    !string.IsNullOrEmpty(liquidazione.areaLiquidazionePensioneAgo.DatiLiquidazionePensioneStorico.NaturaPensione) && liquidazione.areaLiquidazionePensioneAgo.DatiLiquidazionePensioneStorico.NaturaPensione.Substring(2, 1) == " ")
                    ddlCodNatura3DG.Enabled = true;
                else
                    ddlCodNatura3DG.Enabled = false;
                ddlConfermeInvalidita.Enabled = false;
            }

            if (this.domanda.Categoria.Trim() == "VO" && liquidazione.areaLiquidazionePensioneAgo.IsRiaperturaPerCausaPersa.GetValueOrDefault())
                ddlCodNatura2DG.Enabled = true;
            //ENG - RIC dei poligrafici per la tripletta 0001-0001-0162 sbloccare il campo esenzione fiscale
            if (liquidazione != null && liquidazione.areaLiquidazionePensioneAgo != null && liquidazione.areaLiquidazionePensioneAgo.IsPrepensionamentoEditoriaArt1c500L160_2019.GetValueOrDefault())
            {
                ddlTipoCalcolo.Enabled = false;
                ddlCausaCarico.Enabled = false;

                if (datiPensione.IdTipoPLPerRIC.HasValue && (Utility.TipoPLPerRIC)TitolarePensione.Pensione.IdTipoPLPerRIC == Utility.TipoPLPerRIC.RicPrepensionamentoEditoriaArt1c500L160_2019)
                {
                    ddlEsenzioneFiscale.Enabled = true;
                }
                else
                {
                    ddlEsenzioneFiscale.Enabled = false;
                }
                chkExCombattente.Enabled = false;
                chkMaggiorazioni.Enabled = false;
                ddlTrattINPDAP.Enabled = false;
                txtDecTrattINPDAP.Enabled = false;
            }

            if (liquidazione != null && liquidazione.areaLiquidazionePensioneAgo != null && liquidazione.areaLiquidazionePensioneAgo.DatiGenerici != null &&
                !string.IsNullOrEmpty(liquidazione.areaLiquidazionePensioneAgo.DatiGenerici.NaturaPensione) &&
                liquidazione.areaLiquidazionePensioneAgo.DatiGenerici.NaturaPensione.Substring(2, 1) == "O")
                ddlCodNatura1DG.Enabled = true;
        }

        private void GestioneEtichetteENPALS(ILiquidazionePensioneAgo liquidazione)
        {
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            AreaTitolare.DatiPensione datiPensione = GetDatiPensione(this);

            if (this.domanda.IsDomandaENPALS)
            {
                bool isENPALSManuale = CodeUtility.IsEnpalsManualePL(this.domanda.IsDomandaENPALS, CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura), datiPensione.IsDatiENPALSRecuperati);
                pnlINPDAP.Visible = false;
                if (!isENPALSManuale)
                    chkProvvisoria.Enabled = false;

                if (liquidazione != null && liquidazione.areaLiquidazionePensioneAgo != null)
                {
                    if (liquidazione.areaLiquidazionePensioneAgo.IsDomandaCasellario.HasValue && liquidazione.areaLiquidazionePensioneAgo.IsDomandaCasellario.Value)
                        ddlTipoCalcolo.Enabled = true;
                    else if (!isENPALSManuale)
                        ddlTipoCalcolo.Enabled = false;

                    if (!isENPALSManuale &&
                        liquidazione.areaLiquidazionePensioneAgo.IsDatiExCombattenteENPALSPresenti.HasValue && liquidazione.areaLiquidazionePensioneAgo.IsDatiExCombattenteENPALSPresenti.Value)
                        chkExCombattente.Enabled = false;

                    if (!isENPALSManuale &&
                        liquidazione.areaLiquidazionePensioneAgo.IsDatiBeneficiENPALSPresenti.HasValue && liquidazione.areaLiquidazionePensioneAgo.IsDatiBeneficiENPALSPresenti.Value)
                    {
                        chkBenefici.Enabled = false;
                        HiddenFieldChkBeneficiDisabled.Value = "true";
                    }
                }
            }
        }

        internal void SetHiddenFieldIsRicostituzione()
        {
            if (this.domanda == null)
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            if (TitolarePensione == null)
                TitolarePensione = new AreaTitolare();
            TitolarePensione.Pensione = GetDatiPensione(this);

            CodeUtility.TipologiaPensioneGruppo tipologiaGruppoPensione = CodeUtility.TipologiaPensioneGruppo.gr_NessunValore;
            CodeUtility.TipologiaPensioneProdotto tipologiaProdottoPensione = CodeUtility.TipologiaPensioneProdotto.pr_NessunValore;
            CodeUtility.TipologiaPensioneTipo tipologiaTipoPensione = CodeUtility.TipologiaPensioneTipo.tp_NessunValore;
            CodeUtility.GetTipologiaPensione(TitolarePensione.Pensione.CodeGruppo, TitolarePensione.Pensione.CodeProdotto, TitolarePensione.Pensione.CodeTipo, out tipologiaGruppoPensione, out tipologiaProdottoPensione, out tipologiaTipoPensione);

            if (tipologiaGruppoPensione == CodeUtility.TipologiaPensioneGruppo.gr_Ricostituzione || this.domanda.IsDomandaRiapertura)
                HiddenFieldIsRicostituzione.Value = "SI";
            else
                HiddenFieldIsRicostituzione.Value = "NO";
        }

        private void RenderControls(AreaTitolare.DatiPensione datiPensione, ILiquidazionePensioneAgo liquidazioneAgo)
        {
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];


            CodeUtility.TipologiaPensioneGruppo tipologiaGruppoPensione = CodeUtility.TipologiaPensioneGruppo.gr_NessunValore;
            CodeUtility.TipologiaPensioneProdotto tipologiaProdottoPensione = CodeUtility.TipologiaPensioneProdotto.pr_NessunValore;
            CodeUtility.TipologiaPensioneTipo tipologiaTipoPensione = CodeUtility.TipologiaPensioneTipo.tp_NessunValore;
            CodeUtility.GetTipologiaPensione(datiPensione.CodeGruppo, datiPensione.CodeProdotto, datiPensione.CodeTipo, out tipologiaGruppoPensione, out tipologiaProdottoPensione, out tipologiaTipoPensione);

            if (liquidazioneAgo != null && liquidazioneAgo.areaLiquidazionePensioneAgo != null)
            {
                if (ddlCodNatura1DG.Enabled && liquidazioneAgo.areaLiquidazionePensioneAgo.IsVecchiaiaInvaliditaSupplementare.HasValue)
                {
                    if (!(Utility.IsDomandaAUT(this.domanda.Categoria) &&
                        (liquidazioneAgo.areaLiquidazionePensioneAgo.DatiGenerici.CodiceDomandaRicorso.GetValueOrDefault() == 8 || liquidazioneAgo.areaLiquidazionePensioneAgo.DatiGenerici.CodiceDomandaRicorso.GetValueOrDefault() == 9)) &&
                        !(CodeUtility.IsRicostituzione(datiPensione) && Utility.IsDomandaDAI(this.domanda.Categoria)))
                    {
                        ddlCodNatura1DG.Enabled = !liquidazioneAgo.areaLiquidazionePensioneAgo.IsVecchiaiaInvaliditaSupplementare.Value;
                    }
                }

                if (liquidazioneAgo.areaLiquidazionePensioneAgo.DatiGenerici != null)
                {
                    if (!string.IsNullOrEmpty(liquidazioneAgo.areaLiquidazionePensioneAgo.DatiGenerici.NaturaPensione))
                    {
                        //codice natura 2
                        if (liquidazioneAgo.areaLiquidazionePensioneAgo.IsCodiceNatura2Enabled.HasValue)
                            ddlCodNatura2DG.Enabled = liquidazioneAgo.areaLiquidazionePensioneAgo.IsCodiceNatura2Enabled.Value;
                        if (ddlCodNatura2DG.Enabled && liquidazioneAgo.areaLiquidazionePensioneAgo.IsSperimentaleDonna.HasValue)
                            ddlCodNatura2DG.Enabled = !liquidazioneAgo.areaLiquidazionePensioneAgo.IsSperimentaleDonna.Value;
                        if (ddlCodNatura2DG.Enabled && liquidazioneAgo.areaLiquidazionePensioneAgo.IsRimpatriatiAlbania.HasValue)
                            ddlCodNatura2DG.Enabled = !liquidazioneAgo.areaLiquidazionePensioneAgo.IsRimpatriatiAlbania.Value;
                        if (ddlCodNatura2DG.Enabled && liquidazioneAgo.areaLiquidazionePensioneAgo.IsDatiCalcoloDAIAltraGestionePresent.HasValue)
                            ddlCodNatura2DG.Enabled = false;
                        if (ddlCodNatura2DG.Enabled && (liquidazioneAgo.areaLiquidazionePensioneAgo.IsPrepensionamentoEditoriaFiltroEAA.GetValueOrDefault() ||
                            liquidazioneAgo.areaLiquidazionePensioneAgo.IsPrepensionamentoEditoriaArt1c154L205_2017.GetValueOrDefault()))
                            ddlCodNatura2DG.Enabled = false;
                        if (ddlCodNatura2DG.Enabled && liquidazioneAgo.areaLiquidazionePensioneAgo.IsDomandaInabilitaSpecificaENPALS.GetValueOrDefault())
                            ddlCodNatura2DG.Enabled = false;
                        if (ddlCodNatura2DG.Enabled && (CodeUtility.IsTipoContributivoConOpzione(datiPensione, liquidazioneAgo.areaLiquidazionePensioneAgo.IsPensioneTipoContributivoConOpzione) ||
                            datiPensione.IsDomandaAnticipataFlessibileOpzioneContributivoOrRicostituzione))
                            ddlCodNatura2DG.Enabled = false;
                        if (ddlCodNatura2DG.Enabled && CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura) && Utility.IsDomandaSOCUM(this.domanda.Categoria))
                            ddlCodNatura2DG.Enabled = false;
                        //ENG - per le pensioni della nuova opzione donna (tipo 0190) il secondo byte del codice natura "O" deve essere sempre selezionato e bloccato
                        if (liquidazioneAgo.areaLiquidazionePensioneAgo.IsOpzioneDonna_Legge197_2022_Art1_Comma292OrRicostituzione.HasValue)
                        {
                            CodeUtility.DisableCodNatura2PerOpzioneDonna_Legge197_2022_Art1_Comma292(ddlCodNatura2DG, liquidazioneAgo.areaLiquidazionePensioneAgo.IsOpzioneDonna_Legge197_2022_Art1_Comma292OrRicostituzione.Value);
                        }
                        //codice natura 3
                        if (liquidazioneAgo.areaLiquidazionePensioneAgo.IsUsuranti.HasValue)
                            ddlCodNatura3DG.Enabled = !liquidazioneAgo.areaLiquidazionePensioneAgo.IsUsuranti.Value;
                        if (ddlCodNatura3DG.Enabled && liquidazioneAgo.areaLiquidazionePensioneAgo.IsDomandaTrasformazioneInvalidita.HasValue && liquidazioneAgo.areaLiquidazionePensioneAgo.IsDomandaTrasformazioneInvalidita.Value)
                            ddlCodNatura3DG.Enabled = false;
                        if (ddlCodNatura3DG.Enabled && (liquidazioneAgo.areaLiquidazionePensioneAgo.IsPrepensionamentoEditoriaFiltroEAA.GetValueOrDefault() ||
                            liquidazioneAgo.areaLiquidazionePensioneAgo.IsPrepensionamentoEditoriaArt1c154L205_2017.GetValueOrDefault() ||
                            liquidazioneAgo.areaLiquidazionePensioneAgo.IsPrepensionamentoEditoriaArt1c500L160_2019.GetValueOrDefault()))
                            ddlCodNatura3DG.Enabled = false;
                        if (Utility.IsDomandaCumulo(this.domanda.Categoria) && datiPensione.IsDomandaCumuloAutomatica &&
                            liquidazioneAgo.areaLiquidazionePensioneAgo.DatiGenerici.NaturaPensione.Substring(2, 1) == "S")
                            ddlCodNatura3DG.Enabled = false;
                        if (ddlCodNatura3DG.Enabled && CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura) && Utility.IsDomandaSOCUM(this.domanda.Categoria))
                            ddlCodNatura3DG.Enabled = false;

                        if (CodeUtility.IsRicostituzione(datiPensione) && Utility.IsDomandaDAI(this.domanda.Categoria) && this.domanda.Categoria.StartsWith("S") &&
                            liquidazioneAgo.areaLiquidazionePensioneAgo.DatiGenerici.NaturaPensione.Substring(2, 1) == "T")
                        {
                            ddlCodNatura2DG.Enabled = false;
                            ddlCodNatura3DG.Enabled = false;
                        }

                        if (liquidazioneAgo.areaLiquidazionePensioneAgo.IsRicEnpalsMotiviContributivi.HasValue)
                            ddlCodNatura3DG.Enabled = !liquidazioneAgo.areaLiquidazionePensioneAgo.IsRicEnpalsMotiviContributivi.Value;
                    }
                }
                if (liquidazioneAgo.areaLiquidazionePensioneAgo.IsSperimentaleDonna.GetValueOrDefault() || liquidazioneAgo.areaLiquidazionePensioneAgo.IsOpzioneDonna_Legge197_2022_Art1_Comma292OrRicostituzione.GetValueOrDefault())
                    ddlTipoCalcolo.Enabled = false;

                if (Utility.IsDomandaAUT(this.domanda.Categoria) &&
                    !(liquidazioneAgo.areaLiquidazionePensioneAgo.DatiGenerici.CodiceDomandaRicorso.GetValueOrDefault() == 8 || liquidazioneAgo.areaLiquidazionePensioneAgo.DatiGenerici.CodiceDomandaRicorso.GetValueOrDefault() == 9) &&
                    (datiPensione.CodeTipo == "0009" || datiPensione.CodeTipo == "0192"))
                {
                    ddlCodNatura1DG.Enabled = false;
                }

                if (CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura) && Utility.IsDomandaSOAUT(this.domanda.Categoria)
                    && liquidazioneAgo.areaLiquidazionePensioneAgo.DatiGenerici != null && liquidazioneAgo.areaLiquidazionePensioneAgo.DatiGenerici.NaturaPensione != null
                    && liquidazioneAgo.areaLiquidazionePensioneAgo.DatiGenerici.NaturaPensione.Substring(0, 1) == "5")
                {
                    ddlCodNatura1DG.Enabled = false;
                }

                if (Utility.IsDomandaSOPED(this.domanda.Categoria)) //Da requisito, solo indirette, ma dovremmo gestire solo quelle. Da valutare per le RIC
                {
                    ddlCodNatura2DG.Enabled = false;
                }

                if (CodeUtility.IsRicostituzione(datiPensione) && liquidazioneAgo.areaLiquidazionePensioneAgo.IsPrepensionamentoEditoriaArt1c500L160_2019.GetValueOrDefault())
                    ddlCodNatura2DG.Enabled = false;
            }
            if (Utility.IsDomandaCumulo(this.domanda.Categoria))
            {
                ddlCodMobilita.Enabled = false;
                if (datiPensione.IsDomandaAPEPrecociOrRicostituzione)
                {
                    if (ddlEnteCassa.Items.FindByValue("1") != null)
                        ddlEnteCassa.SelectedValue = "1";
                    ddlEnteCassa.Enabled = false;
                }
                if (datiPensione.IsDomandaCumuloAutomatica)
                {
                    ddlContributivoCum.Enabled = false;
                    ddlEnteCassa.Enabled = false;
                    //chkProvvisoria.Enabled = false;
                    ddlTipoCumulo.Enabled = false;
                    ddlCumuloEsterno.Enabled = false;
                }
                if (CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura))
                {
                    ddlContributivoCum.Enabled = false;
                    ddlEnteIstruttoreFondoExInpdap.Enabled = false;
                    chkBenefici.Enabled = false;
                    HiddenFieldChkBeneficiDisabled.Value = "true";
                    chkExCombattente.Enabled = false;
                }

                if (Utility.IsDomandaVOCUM(this.domanda.Categoria))
                {
                    if (isVOCUM_C9A(datiPensione, this.domanda))
                        ddlContributivoCum.Enabled = false;

                    if (!this.domanda.IsDomandaRiapertura)
                    {
                        trTipologiaCumulo.Visible = true;
                        if (CodeUtility.IsRicostituzione(datiPensione) || TitolarePensione.Pensione.IsDomandaQuota100OrRicostituzione || TitolarePensione.Pensione.IsDomandaQuota102OrRicostituzione || TitolarePensione.Pensione.IsDomandaAnticipataFlessibileOrRicostituzione || TitolarePensione.Pensione.IsDomandaAnticipataFlessibileOpzioneContributivoOrRicostituzione)
                        {
                            ddlTipoCumulo.Enabled = false;
                            ddlCumuloEsterno.Enabled = false;
                        }
                        if ((TitolarePensione.Pensione.IsDomandaQuota100OrRicostituzione || TitolarePensione.Pensione.IsDomandaQuota102OrRicostituzione || TitolarePensione.Pensione.IsDomandaAnticipataFlessibileOrRicostituzione || TitolarePensione.Pensione.IsDomandaAnticipataFlessibileOpzioneContributivoOrRicostituzione) && !TitolarePensione.Pensione.IsDomandaCumuloAutomatica)
                        {
                            ddlTipoCumulo.SelectedValue = "True";
                            ddlCumuloEsterno.SelectedIndex = 0;
                        }
                    }
                }
                else
                {
                    trTipologiaCumulo.Visible = true;
                    //sempre editabile per tutti i tipi di PL di IOCUM e SOCUM
                    if (CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura))
                    {
                        ddlTipoCumulo.Enabled = false;
                        ddlCumuloEsterno.Enabled = false;
                    }
                    else
                        ddlTipoCumulo.Enabled = true;
                    //IOCUM & SOCUM Indirette
                    if (Utility.IsDomandaIOCUM(this.domanda.Categoria) || (Utility.IsDomandaSOCUM(this.domanda.Categoria) && Utility.IsDomandaIndiretta(datiPensione)))
                    {
                        HiddenFieldBlindCumuloEsterno.Value = "true";
                    }
                }

                if ((Utility.IsDomandaIOCUM(this.domanda.Categoria) && tipologiaProdottoPensione == CodeUtility.TipologiaPensioneProdotto.pr_InabilitaPensione))
                {
                    HiddenFieldBlindEnteCassa.Value = "true";
                    if (tipologiaTipoPensione != CodeUtility.TipologiaPensioneTipo.tp_Inabilita_Ordinaria && tipologiaTipoPensione != CodeUtility.TipologiaPensioneTipo.tp_Inabilita_Art2_C12_Legge335 && !Utility.IsPensioneInabilitaProficuoLavoroCumulo(this.domanda.Categoria, datiPensione))
                    {
                        trMessaggioInformativo.Visible = true;
                        lblMessaggioInformativo.Text = "Attenzione se Ente Istruttore è pari a SI, tornare sul quadro Titolare e salvare la decorrenza pensione nel formato GG/MM/AAAA";
                    }
                }

                if (Utility.IsDomandaSOCUM(this.domanda.Categoria))
                {
                    ddlTrattINPDAP.SelectedIndex = 0;
                    ddlTrattINPDAP.Enabled = false;
                    txtDecTrattINPDAP.Text = string.Empty;
                    txtDecTrattINPDAP.Enabled = false;
                    lblEnteIstruttore.Visible = false;
                    ddlEnteIstruttoreFondoExInpdap.Visible = false;
                    ddlEnteIstruttoreFondoExInpdap.ClearSelection();
                }
            }

            if (Utility.IsDomandaVOESO(this.domanda.Categoria) || Utility.IsDomandaVESO33(this.domanda.Categoria) || Utility.IsDomandaVESO92(this.domanda.Categoria) ||
                Utility.IsDomandaVOCRED_CRED27(this.domanda.Categoria) || Utility.IsDomandaVOCOOP_COOP28(this.domanda.Categoria) || Utility.IsDomandaSOCUM(this.domanda.Categoria) ||
                Utility.IsDomandaESPA(this.domanda.Categoria) || Utility.IsDomandaPSO(this.domanda.Categoria) || Utility.IsDomandaPMO(this.domanda.Categoria) || Utility.IsDomandaESOPMI(this.domanda.Categoria) || Utility.IsDomandaINDCOM124(TitolarePensione.Pensione, this.domanda.Categoria))
            {
                txtScadRevSanitaria.Enabled = false;
                HdnRemoveScadSanCalendar.Value = "true";
            }
            if (Utility.IsDomandaAPESociale(this.domanda.Categoria))
            {
                ddlCodNatura3DG.Enabled = false;
                ddlCodNatura3DG.SelectedIndex = 0;

                chkExCombattente.Enabled = false;
                chkBenefici.Enabled = false;
                HiddenFieldChkBeneficiDisabled.Value = "true";
                chkMaggiorazioni.Enabled = false;
                ddlTrattINPDAP.Enabled = false;
                txtDecTrattINPDAP.Enabled = false;
            }
            if (Utility.IsDomandaReversibilita(datiPensione))
                txtCodiceLiquidazione.Enabled = false;

            //ENG - PL VESO33 data completezza e data interessi legali non visibili
            if (Utility.IsDomandaVOESO(this.domanda.Categoria) || Utility.IsDomandaVESO92(this.domanda.Categoria)
                || Utility.IsDomandaVOCRED(this.domanda.Categoria) || Utility.IsDomandaVOCOOP(this.domanda.Categoria)
                || Utility.IsDomandaVESO33(this.domanda.Categoria)
                || Utility.IsDomandaESPA(this.domanda.Categoria))
                trCompletezzaAndInteressiLegali.Visible = false;
            if (Utility.IsDomandaVOCOOP_COOP28(this.domanda.Categoria) || Utility.IsDomandaVOCRED_CRED27(this.domanda.Categoria) ||
                Utility.IsDomandaVOESO(this.domanda.Categoria) || Utility.IsDomandaVESO92(this.domanda.Categoria) ||
                Utility.IsDomandaVESO29(this.domanda.Categoria) || Utility.IsDomandaESOTEL(this.domanda.Categoria) ||
                Utility.IsDomandaESOAMB(this.domanda.Categoria) || Utility.IsDomandaESPA(this.domanda.Categoria))
                lblEtichettaDecorrenzaPensioneDatiGenerici.Text = "Decorrenza Assegno:";

            if (Utility.IsDomandaVOST(this.domanda.Categoria))
            {
                ddlTrattINPDAP.ClearSelection();
                ddlTrattINPDAP.Enabled = false;
                txtDecTrattINPDAP.Text = string.Empty;
                txtDecTrattINPDAP.Enabled = false;
                chkExCombattente.Checked = false;
                chkExCombattente.Enabled = false;
                chkMaggiorazioni.Checked = false;
                chkMaggiorazioni.Enabled = false;
                chkBenefici.Checked = false;
                HiddenFieldChkBeneficiChecked.Value = "false";
                chkBenefici.Enabled = false;
                HiddenFieldChkBeneficiDisabled.Value = "true";
                chkTrasfAOI.Checked = false;
                chkTrasfAOI.Enabled = false;
                //lblInteressiLegali.Visible = false;
                //txtInteressiLegali.Visible = false;
                trCompletezzaAndInteressiLegali.Visible = false;
                txtInteressiLegali.Text = null;
                txtDataCompletezza.Text = null;
            }

            if (Utility.IsDomandaRenditaCasalinghe(this.domanda.Categoria) || Utility.IsDomandaRenditaFacoltativa(this.domanda.Categoria))
            {
                chkMaggiorazioni.Checked = false;
                chkMaggiorazioni.Enabled = false;
            }

            //ENG - Per le PL/TRF/RIC delle CRED27, COOP28, VESO29 non deve essere visibile la Data Interessi Legali
            if (Utility.IsDomandaCRED27(this.domanda.Categoria) || Utility.IsDomandaCOOP28(this.domanda.Categoria) || Utility.IsDomandaVESO29(this.domanda.Categoria))
            {
                lblInteressiLegali.Visible = false;
                txtInteressiLegali.Visible = false;
            }
        }

        protected void VerificaAdesioneFondoCredito()
        {
            AreaRispostaRiepilogo.DatiRiepilogoAnagrafica titolare = (AreaRispostaRiepilogo.DatiRiepilogoAnagrafica)Session["Anagrafica"];
            Presenter.PresenterLiquidazionePensione presenterLiquidazione = new PresenterLiquidazionePensione();
            presenterLiquidazione.VerificaAdesioneFondoCredito(titolare.CodiceFiscale, this);

            if (this.HasError)
            {
                ddlTrattINPDAP.SelectedValue = "NO";
                txtDecTrattINPDAP.Text = "MM/AAAA";
                txtDecTrattINPDAP.Enabled = false;
                ddlTrattINPDAP.Enabled = false;
            }
            else
            {
                ddlTrattINPDAP.SelectedValue = "SI";
                ddlTrattINPDAP.Enabled = false;
                txtDecTrattINPDAP.Enabled = true;
            }
        }

        //ENG - Aggiornamento Memo86
        protected void VerificaAdesioneFondoCreditoAggiornamentoMemo86(bool? isPresenteTrattenutaFondoCreditoDaPrelievo, bool? IsDataRinunciaTrattenutaInpdapStorico)
        {
            AreaRispostaRiepilogo.DatiRiepilogoAnagrafica titolare = (AreaRispostaRiepilogo.DatiRiepilogoAnagrafica)Session["Anagrafica"];
            Presenter.PresenterLiquidazionePensione presenterLiquidazione = new PresenterLiquidazionePensione();
            presenterLiquidazione.VerificaAdesioneFondoCredito(titolare.CodiceFiscale, this);
            bool isPresenteAdesioneFondoCredito = (this.HasError) ? false : true;

            //casistica blank, casistica discordante, casistica concordante NO
            if (!isPresenteTrattenutaFondoCreditoDaPrelievo.HasValue ||
                isPresenteTrattenutaFondoCreditoDaPrelievo.Value != isPresenteAdesioneFondoCredito ||
                (!isPresenteTrattenutaFondoCreditoDaPrelievo.Value && !isPresenteAdesioneFondoCredito))
            {
                if (!isPresenteAdesioneFondoCredito)
                {
                    ddlTrattINPDAP.SelectedValue = "NO";
                    txtDecTrattINPDAP.Text = "MM/AAAA";
                    txtDecTrattINPDAP.Enabled = false;
                    ddlTrattINPDAP.Enabled = false;
                }
                else
                {
                    ddlTrattINPDAP.SelectedValue = "SI";
                    ddlTrattINPDAP.Enabled = false;
                    txtDecTrattINPDAP.Enabled = true;
                }

                if (isPresenteTrattenutaFondoCreditoDaPrelievo.HasValue && isPresenteTrattenutaFondoCreditoDaPrelievo.Value != isPresenteAdesioneFondoCredito)
                    RaiseShowAvvisoTrattenutaFondoCredito(this, null);
            }
            else if (isPresenteTrattenutaFondoCreditoDaPrelievo.Value && isPresenteAdesioneFondoCredito) //casistica concordante SI
            {
                ddlTrattINPDAP.Enabled = false;
                txtDecTrattINPDAP.Enabled = false;
            }
        }

        #endregion Private Methods

        #region Events

        public event EventHandler ShowAvviso;
        public event EventHandler ShowAvvisoElimina;

        //ENG - Aggiornamento Memo86
        public event EventHandler ShowAvvisoTrattenutaFondoCredito;

        protected void RaiseShowAvviso(object sender, EventArgs e)
        {
            ShowAvviso(sender, e);
        }

        protected void RaiseShowAvvisoElimina(object sender, EventArgs e)
        {
            ShowAvvisoElimina(sender, e);
        }

        //ENG - Aggiornamento Memo86
        protected void RaiseShowAvvisoTrattenutaFondoCredito(object sender, EventArgs e)
        {
            ShowAvvisoTrattenutaFondoCredito(sender, e);
        }


        #endregion Events
    }
}

