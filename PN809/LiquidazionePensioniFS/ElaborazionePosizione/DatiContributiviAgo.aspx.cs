using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using INPS.DNA.Security;
using INPS.DNA.Security.Idm;
using INPS.DNA.UI.Web;
using INPS.DNA.UI.Web.Intranet;
using INPS.Pensioni.LiquidazionePensione.Presenter;
using INPS.Pensioni.LiquidazionePensione.Presenter.Enum;
using INPS.Pensioni.LiquidazionePensione.Presenter.Contract;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.View.Web;
using INPS.Pensioni.LiquidazionePensione.View.Web.UserControls;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneAgo;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using INPS.DNA;
using INPS.DNA.Presenter.Interface;

namespace INPS.Pensioni.LiquidazionePensione.View.Web
{
    [NavigationRules(AllowedReferrer = AllowedReferrer.Application, CheckSequenceOnPostBack = false)]

    public partial class DatiContributiviAgo : CustomBasePage, IInfoLiquidazione, IDatiContributiviAgo, IQuadriSemafori
    {

        #region IInfoLiquidazione
        public InfoLiquidazione InfoLiquidazione { get; set; }
        #endregion IInfoLiquidazione

        #region IViewUI
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        #endregion IViewUI

        #region IDatiContributiviAgo
        public Presenter.SvrLiquidazioneAgo.AreaDatiContributivi areaDatiContributiviAgo { get; set; }
        public AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda { get; set; }

        #endregion IDatiContributiviAgo

        #region IQuadriSemafori
        public AreaQuadri areaQuadri { get; set; }
        public AreaInfoPratica areaInfoPratica { get; set; }
        #endregion IQuadriSemafori

        protected void Page_Load(object sender, EventArgs e)
        {
            imgDatiCalcolo.ImageUrl = "~/App_Themes/" + Page.Theme + "/Images/rosso_tab.png";

            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            if (!IsPostBack)
            {
                this.InfoLiquidazione = ValorizzaInfoLiquidazione(ucInfoLiquidazione);
                InitData();

                RenderSemafori();
                if (Session["Pratiche"] != null)
                {
                    btnTornaPosizioni.Visible = true;
                    btnTornaARicerca.Visible = false;
                }
            }
            if (VS_ElencoValidationGroupname != null && VS_ElencoValidationGroupname.Count > 0)
            {
                foreach (string validationGroupname in VS_ElencoValidationGroupname)
                    AddValidationSummary(validationGroupname);
            }
        }


        protected void event_ucInitializeData(object sender, EventArgs e)
        {
            InitData();
        }

        public void InitData()
        {
            CaricaDatiCalcolo();
            AreaTitolare.DatiPensione datiPensione = CodeUtility.GetDatiPensioneFromSession();
            if (this.domanda.Tipo == "RIC" || ControllaTipoCalcolo())
            {
                if (this.domanda.IsDomandaENPALS)
                {
                    bool abilitaPulsanteElimina = false;
                    if ((AreaQuadri)Session["Semaforo"] != null)
                    {
                        this.areaQuadri = (AreaQuadri)Session["Semaforo"];
                        abilitaPulsanteElimina = this.areaQuadri.QuadroDatiContributivi.TabDatiCalcoloENPALS != AreaQuadri.Semaforo.Rosso_Abilitato;
                    }

                    ucDatiCalcoloENPALS.areaDatiContributiviAgo = this.areaDatiContributiviAgo;
                    ucDatiCalcoloENPALS.ValorizzaEtichetteDatiCalcoloENPALS(this, abilitaPulsanteElimina);
                }
                else if (Utility.IsDomandaDAI(this.domanda.Categoria))
                {
                    ucDatiCalcoloINPDAI.areaDatiContributiviAgo = this.areaDatiContributiviAgo;
                    ucDatiCalcoloINPDAI.ValorizzaEtichette(this);
                }
                else if (Utility.IsDomandaCumulo(this.domanda.Categoria) || Utility.IsDomandaTotalizzazione(this.domanda.Categoria))
                {
                    ucQuotePensione.areaDatiContributiviAgo = this.areaDatiContributiviAgo;
                    ucQuotePensione.ValorizzaEtichette(this);
                    if (Utility.IsDomandaTotalizzazione(this.domanda.Categoria))
                        href_quotePensione.InnerText = "Quote Totalizzazione";

                    ucMiglioramentiContrattuali.areaDatiContributiviAgo = this.areaDatiContributiviAgo;
                    ucMiglioramentiContrattuali.ValorizzaEtichette(this);
                }
                else
                {
                    ucDatiCalcoloAgo.areaDatiContributiviAgo = this.areaDatiContributiviAgo;
                    ucDatiCalcoloAgo.ValorizzaEtichetteDatiCalcoloAGO(this);
                }

                if (this.areaDatiContributiviAgo != null &&
                    (this.areaDatiContributiviAgo.DatiCalcoloStorico != null || this.areaDatiContributiviAgo.DatiCalcoloQuotePensioneStorico != null || this.areaDatiContributiviAgo.DatiExINPDAIStorico != null))
                {
                    ucStorico.areaDatiContributiviAgo = this.areaDatiContributiviAgo;
                    ucStorico.ValorizzaEtichette();
                }
            }

            if (this.areaDatiContributiviAgo != null && this.areaDatiContributiviAgo.DatiCalcoloVittimeTerrorismo != null)
            {
                ucDatiCalcoloVittimeTerrorismo.areaDatiContributiviAgo = this.areaDatiContributiviAgo;
                ucDatiCalcoloVittimeTerrorismo.ValorizzaEtichetteDatiCalcoloVittimeTerrorismo(this);
            }

            if (this.areaDatiContributiviAgo != null && this.areaDatiContributiviAgo.DatiQuotaFondoIntegrativo != null)
            {
                ucQuotaFondoIntegrativo.areaDatiContributiviAgo = this.areaDatiContributiviAgo;
                ucQuotaFondoIntegrativo.ValorizzaEtichetteQuotaFondoIntegrativo(this);
                //ENG - RIC Esattoriali: gestiti i flussi per il recupero dei dati dal prelievo
                if (this.areaDatiContributiviAgo.IsRicOTrfEsattoriali.GetValueOrDefault())
                    ViewState[EnumViewState.IsRicostituzioneORiaperturaEsattoriali.ToString()] = true;
            }

            if (Utility.IsDomandaINPGI(this.domanda.Categoria) && this.areaDatiContributiviAgo != null && this.areaDatiContributiviAgo.DatiQuotaFondoINPGI != null)
            {
                ucQuotaFondoINPGI.areaDatiContributiviAgo = this.areaDatiContributiviAgo;
                ucQuotaFondoINPGI.ValorizzaEtichetteQuotaFondoINPGI(this);
            }
            //ENG - MEMO 74_2023
            //ENG - Memo 116/2025
            if (this.areaDatiContributiviAgo.IsMemo74_2023Abilitato.GetValueOrDefault() || datiPensione.IsDomandaVOAUTAnticipataFlessibileLeggeBilancio2024FiltroGSEOrRicostituzione ||
                datiPensione.IsDomandaVOAUTAnticipataTipoContributivoFiltroGSEOrRicostituzione || datiPensione.IsDomandaVOAUTVecchiaiaTipoContributivoFiltroGSEOrRicostituzione)
            {
                ucDatiEsteri.areaDatiContributiviAgo = this.areaDatiContributiviAgo;
                if (!this.areaDatiContributiviAgo.IsDatiEsteriFromServices.GetValueOrDefault())
                    btnSalva.Enabled = false;
            }

            if (this.areaDatiContributiviAgo != null && this.areaDatiContributiviAgo.IsRicOTrfAutmaticaINPGI.GetValueOrDefault())
            {
                UCQuotaFondoINPGIStoricoGP.areaDatiContributiviAgo = this.areaDatiContributiviAgo;
                UCQuotaFondoINPGIStoricoGP.ValorizzaEtichetteQuotaFondoINPGI(this);
            }
        }



        protected void Page_PreRender(object sender, EventArgs e)
        {
            try
            {
                if ((AreaQuadri)Session["Semaforo"] != null)
                {
                    domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
                    this.areaQuadri = (AreaQuadri)Session["Semaforo"];
                    ValorizzaSemaforiTab(imgDatiCalcolo, this.areaQuadri.QuadroDatiContributivi.TabDatiCalcolo, pnlTabDatiCalcolo);
                    ValorizzaSemaforiTab(imgDatiCalcoloENPALS, this.areaQuadri.QuadroDatiContributivi.TabDatiCalcoloENPALS, pnlTabDatiCalcoloENPALS);
                    ValorizzaSemaforiTab(imgDatiCalcoloINPDAI, this.areaQuadri.QuadroDatiContributivi.TabDatiCalcoloINPDAI, pnlTabDatiCalcoloINPDAI);
                    ValorizzaSemaforiTab(imgQuotePensione, this.areaQuadri.QuadroDatiContributivi.TabQuotePensione, pnlTabQuotePensione);
                    ValorizzaSemaforiTab(imgDatiCalcoloTerrorismo, this.areaQuadri.QuadroDatiContributivi.TabVittime, pnlTabDatiCalcoloTerrorismo);
                    ValorizzaSemaforiTab(imgStorico, this.areaQuadri.QuadroDatiContributivi.TabStorico, pnlTabStorico);
                    ValorizzaSemaforiTab(imgQuotaFondoIntegrativo, this.areaQuadri.QuadroDatiContributivi.TabQuotaFondoIntegrativo, pnlTabQuotaFondoIntegrativo);
                    ValorizzaSemaforiTab(imgQuotaFondoINPGI, this.areaQuadri.QuadroDatiContributivi.TabQuotaFondoINPGI, pnlTabQuotaFondoINPGI);
                    //ENG - MEMO 74_2023 Memo 116/2025
                    ValorizzaSemaforiTab(imgDatiEsteri, this.areaQuadri.QuadroDatiContributivi.TabDatiEsteri, pnlTabDatiEsteri);
                    ValorizzaSemaforiTab(imgMiglioramentiContrattuali, this.areaQuadri.QuadroDatiContributivi.TabMiglioramentiContrattuali, pnlTabMiglioramentiContrattuali);
                    ValorizzaSemaforiTab(imgQuotaFondoINPGIStorico, this.areaQuadri.QuadroDatiContributivi.TabQuotaFondoINPGIStorico, pnlQuotaFondoInpgiStorico);
                }
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("DatiContributivi, Errore nel metodo Page_PreRender " + ex);
            }

        }

        private void CaricaDatiCalcolo()
        {
            PresenterDatiContributiviAGO presenterDatiContributiviAgo = new PresenterDatiContributiviAGO();
            this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            // isDataFromDb = true se i dati calcolo sono presenti a db
            bool isDataFromDb = presenterDatiContributiviAgo.GetDatiContributivi(this);
            if (HasError)
            {
                ucAvviso.Visible = true;
                ucAvviso.Tipo = TipoAvviso.Warning;
                ucAvviso.Messaggio = ErrorMessage;

                AreaTitolare.DatiPensione datiPensione = CodeUtility.GetDatiPensioneFromSession();

                if ((Utility.IsDomandaUnicarpe(datiPensione, true) == Utility.TipoUnicarpe.Automatica || CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura)) &&
                    (this.areaDatiContributiviAgo == null || this.areaDatiContributiviAgo.DatiCalcolo == null ||
                    (this.areaDatiContributiviAgo.DatiCalcolo.lDatiRetributivi == null && this.areaDatiContributiviAgo.DatiCalcolo.lDatiContributivi == null)))
                {
                    btnSalva.Enabled = false;
                    btnPopUpPage.Enabled = false;
                    ucDatiCalcoloAgo.DisabilitaPulsanti();
                    ucDatiCalcoloINPDAI.DisabilitaPulsanti();
                }

                return;
            }
            else
            {
                ucAvviso.Visible = false;
                ucAvviso.Messaggio = "";
                ucDatiCalcoloAgo.EnabledBtnEliminaDatiCalcolo(isDataFromDb);
            }
        }

        private bool ControllaTipoCalcolo()
        {
            this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
            AreaTitolare.DatiPensione datiPensione = CodeUtility.GetDatiPensioneFromSession();

            bool bReturn = false;
            if (HasError)
            {
                ucAvviso.Visible = true;
                ucAvviso.Tipo = TipoAvviso.Warning;
                ucAvviso.Messaggio = ErrorMessage;
                return false;
            }
            else
            {
                ucAvviso.Visible = false;
                ucAvviso.Messaggio = "";
                if (this.areaDatiContributiviAgo != null &&
                    (Utility.IsDomandaCumulo(this.domanda.Categoria) ||
                        this.areaDatiContributiviAgo.IsPnlImportoLordoAllaDecVisible ||
                        Utility.IsDomandaAPESociale(this.domanda.Categoria) ||
                        Utility.IsDomandaTotalizzazione(this.domanda.Categoria) ||
                        Utility.IsDomandaRenditaCasalinghe(this.domanda.Categoria) ||
                        Utility.IsDomandaRenditaFacoltativa(this.domanda.Categoria))
                    )
                {
                    //per le domande di cumulo non è inseribile in tipo calcolo
                    bReturn = true;
                }
                else if (this.areaDatiContributiviAgo != null && this.areaDatiContributiviAgo.DatiCalcolo != null)
                {
                    if (this.areaDatiContributiviAgo.DatiCalcolo.IsUnicarpe)
                    {
                        if (!(Utility.IsDomandaINPGI(this.domanda.Categoria) && datiPensione != null && datiPensione.FineAssicurazione.HasValue && datiPensione.FineAssicurazione.Value <= new DateTime(2022, 06, 30)))
                        {
                            switch (this.areaDatiContributiviAgo.DatiCalcolo.TipoCalcolo)
                            {
                                case GestioneContribTipoCalcolo.Contributivo:

                                    if (this.areaDatiContributiviAgo.DatiCalcolo.lDatiRetributivi != null)
                                    {
                                        ucDatiCalcoloAgo.btnSalvaDatiCalcolo.Enabled = false;
                                        ucDatiCalcoloINPDAI.btnSalvaDatiCalcolo.Enabled = false;
                                        ucAvviso.Visible = true;
                                        ucAvviso.Tipo = TipoAvviso.Warning;
                                        ucAvviso.Messaggio = "'Tipo Calcolo' incongruente con i dati calcolo; verificare le informazioni provenienti da FELPE.";
                                        btnSalva.Enabled = false;
                                    }
                                    bReturn = true;
                                    break;

                                case GestioneContribTipoCalcolo.Retributivo:
                                    if (this.areaDatiContributiviAgo.DatiCalcolo.lDatiContributivi != null && !this.areaDatiContributiviAgo.IsFineAssicurazionePost2012 && !this.areaDatiContributiviAgo.IsPensioneInabilitaPost2012)
                                    {
                                        ucDatiCalcoloAgo.btnSalvaDatiCalcolo.Enabled = false;
                                        ucDatiCalcoloINPDAI.btnSalvaDatiCalcolo.Enabled = false;
                                        ucAvviso.Visible = true;
                                        ucAvviso.Tipo = TipoAvviso.Warning;
                                        ucAvviso.Messaggio = "'Tipo Calcolo' incongruente con i dati calcolo; verificare le informazioni provenienti da FELPE.";
                                        btnSalva.Enabled = false;
                                    }
                                    bReturn = true;
                                    break;

                                case GestioneContribTipoCalcolo.Misto:

                                    if (this.areaDatiContributiviAgo.DatiCalcolo.lDatiContributivi == null || this.areaDatiContributiviAgo.DatiCalcolo.lDatiRetributivi == null)
                                    {
                                        ucDatiCalcoloAgo.btnSalvaDatiCalcolo.Enabled = false;
                                        ucDatiCalcoloINPDAI.btnSalvaDatiCalcolo.Enabled = false;
                                        ucAvviso.Visible = true;
                                        ucAvviso.Tipo = TipoAvviso.Warning;
                                        ucAvviso.Messaggio = "'Tipo Calcolo' incongruente con i dati calcolo; verificare le informazioni provenienti da FELPE.";
                                        btnSalva.Enabled = false;
                                    }
                                    bReturn = true;
                                    break;

                                case GestioneContribTipoCalcolo.NonValido:

                                    ucDatiCalcoloAgo.Visible = false;
                                    ucDatiCalcoloINPDAI.Visible = false;
                                    ucAvviso.Visible = true;
                                    ucAvviso.Tipo = TipoAvviso.Warning;
                                    ucAvviso.Messaggio = "E' necessario salvare il 'Tipo Calcolo' dal menu 'Liquidazione Pensione' prima di poter inserire i dati calcolo";
                                    btnSalva.Enabled = false;
                                    bReturn = false;
                                    break;
                            }
                        }
                        else
                        {
                            bReturn = true;
                        }
                    }
                    else
                    {
                        if (!Utility.IsDomandaESOTEL(this.domanda.Categoria) && this.areaDatiContributiviAgo.DatiCalcolo.TipoCalcolo == GestioneContribTipoCalcolo.NonValido)
                        {
                            ucDatiCalcoloAgo.Visible = false;
                            ucDatiCalcoloINPDAI.Visible = false;
                            ucAvviso.Visible = true;
                            ucAvviso.Tipo = TipoAvviso.Warning;
                            ucAvviso.Messaggio = "E' necessario salvare il 'Tipo Calcolo' dal menu 'Liquidazione Pensione' prima di poter inserire i dati calcolo";
                            btnSalva.Enabled = false;
                        }
                        bReturn = true;
                    }
                }
                else
                {
                    ucDatiCalcoloAgo.Visible = false;
                    ucDatiCalcoloENPALS.Visible = false;
                    ucDatiCalcoloINPDAI.Visible = false;
                    ucAvviso.Visible = true;
                    ucAvviso.Tipo = TipoAvviso.Warning;
                    ucAvviso.Messaggio = "E' necessario salvare il 'Tipo Calcolo' dal menu 'Liquidazione Pensione' prima di poter inserire i dati calcolo";
                    btnSalva.Enabled = false;
                    bReturn = false;

                    // Il seguente blocco è stato commentato perchè areaDatiContributiviAgo è null e quindi il sistema va in crash sul controllo
                    //if (this.areaDatiContributiviAgo.DatiCalcolo.TipoCalcolo == GestioneContribTipoCalcolo.NonValido)
                    //{
                    //    ucDatiCalcoloAgo.Visible = false;
                    //    ucAvviso.Visible = true;
                    //    ucAvviso.Tipo = TipoAvviso.Warning;
                    //    ucAvviso.Messaggio = "E' necessario salvare il 'Tipo Calcolo' dal menu 'Liquidazione Pensione' prima di poter inserire i dati calcolo";
                    //    btnSalva.Enabled = false;
                    //    bReturn = false;
                    //}
                    //else
                    //{
                    //    ucDatiCalcoloAgo.Visible = true;
                    //    ucAvviso.Visible = false;
                    //    ucAvviso.Messaggio = "";
                    //    btnSalva.Enabled = true;
                    //    bReturn = true;
                    //}
                }
            }
            return bReturn;
        }
        //event_ucGestisciTastoSalvaVittimeTerrorismo
        protected void event_ucGestisciTastoSalvaVittimeTerrorismo(object sender, Utility.EventMessageArgs e)
        {
            if (e.Message == "SI" && !CodeUtility.IsGridViewInEditPresent(this))
            {
                if (btnSalva.Enabled == false)
                    btnSalva.Enabled = true;
                if (btnPopUpPage.Enabled == false)
                    btnPopUpPage.Enabled = true;
            }
            else
            {
                if (btnSalva.Enabled == true)
                    btnSalva.Enabled = false;
                if (btnPopUpPage.Enabled == true)
                    btnPopUpPage.Enabled = false;
            }
        }


        protected void event_ucGestisciTastoSalva(object sender, EventArgs e)
        {
            //ENG - Memo 116/2025
            AreaTitolare.DatiPensione datiPensione = CodeUtility.GetDatiPensioneFromSession();
            if (!(datiPensione != null && (datiPensione.IsDomandaVOAUTAnticipataFlessibileLeggeBilancio2024FiltroGSEOrRicostituzione ||
                datiPensione.IsDomandaVOAUTAnticipataTipoContributivoFiltroGSEOrRicostituzione || datiPensione.IsDomandaVOAUTVecchiaiaTipoContributivoFiltroGSEOrRicostituzione)))
            {
                if (!CodeUtility.IsGridViewInEditPresent(this))
                {
                    if (btnSalva.Enabled == false)
                        btnSalva.Enabled = true;
                    if (btnPopUpPage.Enabled == false)
                        btnPopUpPage.Enabled = true;
                }
                else
                {
                    if (btnSalva.Enabled == true)
                        btnSalva.Enabled = false;
                    if (btnPopUpPage.Enabled == true)
                        btnPopUpPage.Enabled = false;
                }
            }
        }

        protected void event_ucAbilitaPopUpDatiContributivi(object sender, EventArgs args)
        {
            btnPopUpPage.Style.Add("display", "inline-block");
            btnSalva.Style.Add("display", "none");
        }

        protected void event_ucDisabilitaPopUpDatiContributivi(object sender, EventArgs args)
        {
            btnSalva.Style.Add("display", "inline-block");
            btnPopUpPage.Style.Add("display", "none");
        }

        protected void event_ucHideAvviso(object sender, EventArgs e)
        {
            ucAvviso.Visible = false;
            ucAvviso.Messaggio = string.Empty;
        }

        protected void event_ucShowAvviso(object sender, EventArgs e)
        {
            if (((IViewUI)sender).HasError)
            {
                ucAvviso.Tipo = TipoAvviso.Warning;
                ucAvviso.Visible = true;
                //if (!string.IsNullOrEmpty(ucAvviso.Messaggio))
                //    ucAvviso.Messaggio = string.Concat(ucAvviso.Messaggio + "<br />", ((IViewUI)sender).ErrorMessage);
                //else
                ucAvviso.Messaggio = ((IViewUI)sender).ErrorMessage;
            }
            else
            {
                //if (!string.IsNullOrEmpty(ucAvviso.Messaggio))
                //{
                //    ucAvviso.Tipo = TipoAvviso.Warning;
                //    ucAvviso.Messaggio = string.Concat(ucAvviso.Messaggio + "<br />", ((IViewUI)sender).ErrorMessage);// "Dati Calcolo salvati correttamente";
                //}
                //else
                //{
                ucAvviso.Tipo = TipoAvviso.Ok;
                ucAvviso.Messaggio = ((IViewUI)sender).ErrorMessage;// "Dati Calcolo salvati correttamente";
                //}
                ucAvviso.Visible = true;

                this.areaInfoPratica = new AreaInfoPratica();
                List<AreaQuadri.Tab> elencoTab = new List<AreaQuadri.Tab>();
                elencoTab.Add(AreaQuadri.Tab.DatiCalcolo);
                elencoTab.Add(AreaQuadri.Tab.Redditi);
                this.areaInfoPratica.ElencoTab = elencoTab.ToArray();

                CodeUtility.AggiornaSemafori(this, this, ucInfoLiquidazione);
            }
        }

        protected void event_ucUpdateDatiCalcoloTerrorismoRetributivi(object sender, EventArgs e)
        {
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
            List<GestioneAggiornamentoPECODatiRetributivi> lstDatiRetributiviPage = null;
            if (Utility.IsDomandaDAI(this.domanda.Categoria))
                lstDatiRetributiviPage = ucDatiCalcoloINPDAI.GetDataRetributiviPage();
            else
                lstDatiRetributiviPage = ucDatiCalcoloAgo.GetDataRetributiviPage();
            ucDatiCalcoloVittimeTerrorismo.UpdateGridRetributivi(lstDatiRetributiviPage);

        }
        protected void event_ucUpdateDatiCalcoloTerrorismoContributivi(object sender, EventArgs e)
        {
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
            List<GestioneAggiornamentoPECODatiContributivi> lstDatiContributiviPage = null;
            if (Utility.IsDomandaDAI(this.domanda.Categoria))
                lstDatiContributiviPage = ucDatiCalcoloINPDAI.GetDataContributiviPage();
            else
                lstDatiContributiviPage = ucDatiCalcoloAgo.GetDataContributiviPage();
            ucDatiCalcoloVittimeTerrorismo.UpdateGridContributivi(lstDatiContributiviPage);
        }

        protected void event_ucShowAvvisoDatiProRata(object sender, EventArgs e)
        {
            INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DatiContributiviAgo.UCDatiEsteri tabDatiEsteri = (INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DatiContributiviAgo.UCDatiEsteri)sender;
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            this.areaInfoPratica = new AreaInfoPratica();
            List<AreaQuadri.Tab> elencoTab = new List<AreaQuadri.Tab>();
            elencoTab.Add(AreaQuadri.Tab.DatiCalcolo);
            this.areaInfoPratica.ElencoTab = elencoTab.ToArray();

            CodeUtility.AggiornaSemafori(this, this, ucInfoLiquidazione);

            if (tabDatiEsteri.HasError)
            {
                ucAvviso.Tipo = TipoAvviso.Warning;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = tabDatiEsteri.ErrorMessage;
            }
            else
            {
                ucAvviso.Tipo = TipoAvviso.Ok;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = "Dati Esteri salvati correttamente";
            }
        }

        protected void event_ucShowAvvisoEliminaDatiProRata(object sender, EventArgs e)
        {
            INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DatiContributiviAgo.UCDatiEsteri tabDatiEsteri = (INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DatiContributiviAgo.UCDatiEsteri)sender;
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            this.areaInfoPratica = new AreaInfoPratica();
            List<AreaQuadri.Tab> elencoTab = new List<AreaQuadri.Tab>();
            elencoTab.Add(AreaQuadri.Tab.DatiCalcolo);
            this.areaInfoPratica.ElencoTab = elencoTab.ToArray();

            CodeUtility.AggiornaSemafori(this, this, ucInfoLiquidazione);

            if (tabDatiEsteri.HasError)
            {
                ucAvviso.Tipo = TipoAvviso.Warning;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = tabDatiEsteri.ErrorMessage;
            }
            else
            {
                ucAvviso.Tipo = TipoAvviso.Ok;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = "Dati Esteri eliminati correttamente";
            }
        }

        protected void event_ucNascondiAvviso(object sender, EventArgs e)
        {
            ucAvviso.Messaggio = "";
            ucAvviso.Visible = false;
        }

        protected void event_AddValidationGroupname(object sender, EventArgs e)
        {
            if (VS_ElencoValidationGroupname == null)
                VS_ElencoValidationGroupname = new List<string>();
            VS_ElencoValidationGroupname.Add(ucQuotePensione.ValidationGroupname);
            AddValidationSummary(ucQuotePensione.ValidationGroupname);
        }

        private void AddValidationSummary(string validationGroupname)
        {
            if (pnlValidationSummary.FindControl(validationGroupname) == null)
            {
                ValidationSummary validationsummary = new ValidationSummary();
                validationsummary.ID = validationGroupname;
                validationsummary.ValidationGroup = validationGroupname;
                validationsummary.Font.Size = FontUnit.Small;
                validationsummary.CssClass = "errorBox";
                pnlValidationSummary.Controls.Add(validationsummary);
            }
        }

        protected void btnSalva_Click(object sender, EventArgs e)
        {
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
            AreaTitolare.DatiPensione datiPensione = CodeUtility.GetDatiPensioneFromSession();
            areaDatiContributiviAgo = new Presenter.SvrLiquidazioneAgo.AreaDatiContributivi();
            PresenterDatiContributiviAGO presenterDatiContributiviAgo = new PresenterDatiContributiviAGO();

            if (this.domanda.IsDomandaENPALS)
            {
                this.areaDatiContributiviAgo.DatiCalcoloENPALS = ucDatiCalcoloENPALS.GetDatiCalcoloENPALS();
            }
            else if (Utility.IsDomandaDAI(this.domanda.Categoria))
            {
                ucDatiCalcoloINPDAI.RecuperaCampi(this.areaDatiContributiviAgo);
            }
            else if (Utility.IsDomandaCumulo(this.domanda.Categoria) || Utility.IsDomandaTotalizzazione(this.domanda.Categoria))
            {
                ucQuotePensione.RecuperaCampi(this.areaDatiContributiviAgo);
                ucMiglioramentiContrattuali.RecuperaCampi(this.areaDatiContributiviAgo);
            }
            else
            {
                ucDatiCalcoloAgo.RecuperaCampi(this.areaDatiContributiviAgo);
            }

            ucDatiCalcoloVittimeTerrorismo.RecuperaCampi(this.areaDatiContributiviAgo);

            if (Utility.IsDomandaAnticipataEsattoriali(datiPensione, this.domanda.Categoria) || (ViewState[EnumViewState.IsRicostituzioneORiaperturaEsattoriali.ToString()] != null && (bool)ViewState[EnumViewState.IsRicostituzioneORiaperturaEsattoriali.ToString()]))
            {
                ucQuotaFondoIntegrativo.RecuperaCampi(this.areaDatiContributiviAgo);
            }

            if (Utility.IsDomandaINPGI(this.domanda.Categoria))
            {
                ucQuotaFondoINPGI.RecuperaCampi(this.areaDatiContributiviAgo);
            }
            //ENG - MEMO 74_2023
            //ENG - Memo 116/2025
            if (this.areaDatiContributiviAgo.IsMemo74_2023Abilitato.GetValueOrDefault() || datiPensione.IsDomandaVOAUTAnticipataFlessibileLeggeBilancio2024FiltroGSEOrRicostituzione ||
                datiPensione.IsDomandaVOAUTAnticipataTipoContributivoFiltroGSEOrRicostituzione || datiPensione.IsDomandaVOAUTVecchiaiaTipoContributivoFiltroGSEOrRicostituzione)
            {
                this.areaDatiContributiviAgo.ProRata = ucDatiEsteri.GetDatiProRata();
            }

            presenterDatiContributiviAgo.SalvaDatiContributivi(this);

            if (this.HasError)
            {
                ucAvviso.Tipo = TipoAvviso.Warning;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = this.ErrorMessage;
            }
            else
            {
                ucAvviso.Tipo = TipoAvviso.Ok;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = "Dati Calcolo salvati correttamente";
            }

            this.areaInfoPratica = new AreaInfoPratica();
            List<AreaQuadri.Tab> elencoTab = new List<AreaQuadri.Tab>();
            elencoTab.Add(AreaQuadri.Tab.DatiCalcolo);
            elencoTab.Add(AreaQuadri.Tab.Redditi);
            this.areaInfoPratica.ElencoTab = elencoTab.ToArray();

            CodeUtility.AggiornaSemafori(this, this, ucInfoLiquidazione);

        }

        private void RenderSemafori()
        {
            if ((AreaQuadri)Session["Semaforo"] != null)
            {
                domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
                this.areaQuadri = (AreaQuadri)Session["Semaforo"];

                if (this.areaQuadri.QuadroDatiContributivi.TabDatiCalcolo != AreaQuadri.Semaforo.Rosso_NonAbilitato)
                    hdnSelected.Value = "#dati_contributivi_ago";
                else if (this.areaQuadri.QuadroDatiContributivi.TabDatiCalcoloENPALS != AreaQuadri.Semaforo.Rosso_NonAbilitato)
                    hdnSelected.Value = "#dati_contributivi_ENPALS";
                else if (this.areaQuadri.QuadroDatiContributivi.TabDatiCalcoloINPDAI != AreaQuadri.Semaforo.Rosso_NonAbilitato)
                    hdnSelected.Value = "#dati_contributivi_INPDAI";
                else if (this.areaQuadri.QuadroDatiContributivi.TabQuotePensione != AreaQuadri.Semaforo.Rosso_NonAbilitato)
                    hdnSelected.Value = "#dati_quotePensione";
                else if (this.areaQuadri.QuadroDatiContributivi.TabVittime != AreaQuadri.Semaforo.Rosso_NonAbilitato)
                    hdnSelected.Value = "#dati_contributivi_Terrorismo";
                else if (this.areaQuadri.QuadroDatiContributivi.TabQuotaFondoIntegrativo != AreaQuadri.Semaforo.Rosso_NonAbilitato)
                    hdnSelected.Value = "#quota_fondo_integrativo";
                else if (this.areaQuadri.QuadroDatiContributivi.TabQuotaFondoINPGI != AreaQuadri.Semaforo.Rosso_NonAbilitato)
                    hdnSelected.Value = "#quota_fondo_inpgi";
                else if (this.areaQuadri.QuadroDatiContributivi.TabDatiEsteri != AreaQuadri.Semaforo.Rosso_NonAbilitato)
                    hdnSelected.Value = "#dati_esteri";
                else if (this.areaQuadri.QuadroDatiContributivi.TabMiglioramentiContrattuali != AreaQuadri.Semaforo.Rosso_NonAbilitato)
                    hdnSelected.Value = "#dati_miglioramentiContrattuali";
            }
        }

        private List<string> VS_ElencoValidationGroupname
        {
            get { return (List<string>)ViewState["elencoValidationGroupname"]; }
            set { ViewState["elencoValidationGroupname"] = (List<string>)value; }
        }

        #region Enums
        public enum EnumViewState
        {
            IsRicostituzioneORiaperturaEsattoriali
        }
        #endregion Enums
    }
}
