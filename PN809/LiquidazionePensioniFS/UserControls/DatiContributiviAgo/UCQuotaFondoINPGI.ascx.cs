using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using INPS.DNA;
using INPS.Pensioni.LiquidazionePensione.Presenter;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneAgo;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;

namespace INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DatiContributiviAgo
{
    public partial class UCQuotaFondoINPGI : CustomBaseUserControl, IDatiContributiviAgo, ITitolarePensione, IDanteCausa
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

        #region Enum
        public enum ColonneGvDatiRetributiviINPGI { Sett707 = 5, Importo707 = 6 };
        #endregion Enum

        #region IDanteCausa
        public AreaDanteCausa areaDanteCausa { get; set; }
        public AreaRispostaRiepilogo.DatiRiepilogoDomanda domandaDante { get; set; }
        public long numDomanda { get; set; }
        #endregion IDanteCausa

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
            }
        }

        public void ValorizzaEtichetteQuotaFondoINPGI(IDatiContributiviAgo Dati)
        {
            if (this.domanda == null)
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            AreaTitolare.DatiPensione datiPensione = null;
            datiPensione = GetDatiPensione(this);

            ViewState["areaDatiContributiviAgo"] = Dati.areaDatiContributiviAgo;

            //ENG - Aggiornamento Memo INPGI            
            string ctrlAbilitazioneMemoINPGI_20240307 = string.Empty;
            if (ViewState["AbilitazioneModificheMemoINPGI_20240307"] != null)
                ctrlAbilitazioneMemoINPGI_20240307 = (string)ViewState["AbilitazioneModificheMemoINPGI_20240307"];
            else
            {
                Presenter.PresenterControlliDinamici presenter = new PresenterControlliDinamici();
                Presenter.SvrLiquidazione.AreaEsito esito = presenter.GetControlloDinamicoByNomeControllo("AbilitazioneModificheMemoINPGI_20240307", out ctrlAbilitazioneMemoINPGI_20240307);
                if (esito != null && esito.RisultatoOperazione == Presenter.SvrLiquidazione.AreaEsito.TipoEsito.OK)
                    ViewState["AbilitazioneModificheMemoINPGI_20240307"] = ctrlAbilitazioneMemoINPGI_20240307;
            }

            LoadDecodificaData(Dati);
            BindDataForPanels(Dati.areaDatiContributiviAgo);

            //ENG - RIC VOPGI NON CONTRIBUTIVE
            if ((Utility.IsRicostituzione(datiPensione) && !Utility.IsRicostituzione_MotiviContributivi(datiPensione) && Utility.IsDomandaVOPGI(this.domanda.Categoria)) || Utility.IsDomandaUnicarpe(datiPensione, true) == Utility.TipoUnicarpe.Automatica ||
                (((Utility.IsRicostituzione(datiPensione) && Utility.IsDomandaINPGI(this.domanda.Categoria)) ||
                (!this.domanda.IsDomandaRiapertura && this.domanda.Categoria.Trim().ToUpperInvariant() == "SOPGI" && Utility.IsDomandaReversibilita(datiPensione))) && datiPensione.GP1AV91B == "2"))
            {
                btnEliminaQuotaFondoINPGI.Enabled = false;
                lblRicNonContrib.Visible = true;
            }

            //ENG - Aggiornamento Memo INPGI
            if (!String.IsNullOrEmpty(ctrlAbilitazioneMemoINPGI_20240307) && ctrlAbilitazioneMemoINPGI_20240307.Trim().ToUpperInvariant() == "SI")
            {
                if (Utility.IsDomandaVOPGI_AGI(this.domanda.Categoria, datiPensione.Filtro, datiPensione.DirittoAutonomo, datiPensione.GP1AJ11) &&
                    datiPensione.DecorrenzaOriginaria.HasValue && !Utility.DataSuccessivaA(datiPensione.DecorrenzaOriginaria.Value, new DateTime(2017, 01, 01)))
                {
                    divContributiviINPGI.Visible = false;
                }
            }

            //ENG - INPGI migrate
            if ((((Utility.IsRicostituzione(datiPensione) || Utility.IsDomandaRipristino(datiPensione)) && Utility.IsDomandaINPGI(this.domanda.Categoria)) ||
                (!this.domanda.IsDomandaRiapertura && this.domanda.Categoria.Trim().ToUpperInvariant() == "SOPGI" && Utility.IsDomandaReversibilita(datiPensione))) && datiPensione.GP1AV91B == "2")
            {
                divContributiviINPGI.Visible = true;
                divRetributiviINPGI.Visible = false;
                gvContributiviINPGI.Columns[1].Visible = false;
                lblDatiContributivi.Text = "Pensione Mensile alla decorrenza:";
                lblRicNonContrib.Visible = false;
                gvContributiviINPGI.Columns[3].HeaderText = "Pensione mensile";
                gvContributiviINPGI.DataBind();
            }
        }

        private void BindDataForPanels(AreaDatiContributivi areaDatiContributivi)
        {
            if (areaDatiContributivi != null)
            {
                divRetributiviINPGI.Visible = true;
                divContributiviINPGI.Visible = true;
                InitBindDataRetributivi();
                InitBindDataContributivi();
                ValorizzaPeriodiPerDecodificaGestioneRetrib();
                ValorizzaPeriodiPerDecodificaGestioneContrib();
            }
        }

        private void InitBindDataContributivi()
        {
            List<DatiContributiviQuotaFondoINPGILocal> elencoDatiContributivi = new List<DatiContributiviQuotaFondoINPGILocal>();

            AreaTitolare.DatiPensione datiPensione = new AreaTitolare.DatiPensione();
            datiPensione = GetDatiPensione(this);
            if (this.domanda == null)
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            if (((AreaDatiContributivi)ViewState["areaDatiContributiviAgo"]).DatiQuotaFondoINPGI.lDatiContributiviQuotaFondoINPGI != null)
                elencoDatiContributivi = MapDatiContributiviForView((AreaDatiContributivi)ViewState["areaDatiContributiviAgo"]);

            DatiContributiviQuotaFondoINPGILocal Empty = elencoDatiContributivi.Find(delegate(DatiContributiviQuotaFondoINPGILocal code)
            {
                return (code.Gestione == string.Empty && code.Montante == string.Empty && code.Quota == string.Empty);
            });

            if (Empty == null)
                elencoDatiContributivi.Add(new DatiContributiviQuotaFondoINPGILocal(string.Empty, string.Empty, string.Empty, string.Empty));

            gvContributiviINPGI.DataSource = elencoDatiContributivi;
            ViewState[EnumViewState.ElencoDatiContributiviQuotaFondoINPGI.ToString()] = elencoDatiContributivi;


            gvContributiviINPGI.DataBind();

        }

        private static List<DatiContributiviQuotaFondoINPGILocal> MapDatiContributiviForView(AreaDatiContributivi areaDatiContributivi)
        {
            List<DatiContributiviQuotaFondoINPGILocal> elencoContributiviQuotaFondoINPGI = new List<DatiContributiviQuotaFondoINPGILocal>();
            foreach (GestioneQuotaFondoINPGIDatiCalcoloContributivoINPGI contr in areaDatiContributivi.DatiQuotaFondoINPGI.lDatiContributiviQuotaFondoINPGI.ToList<GestioneQuotaFondoINPGIDatiCalcoloContributivoINPGI>())
            {
                string quota = string.Empty;
                string montante = string.Empty;

                quota = contr.Quota.HasValue ? contr.Quota.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;
                montante = contr.Montante.HasValue ? contr.Montante.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;

                elencoContributiviQuotaFondoINPGI.Add(new DatiContributiviQuotaFondoINPGILocal(contr.CodiceGestione.HasValue ? contr.CodiceGestione.Value.ToString() : string.Empty, montante,
                    quota, contr.Settimane.HasValue ? contr.Settimane.Value.ToString() : string.Empty));
            }
            return elencoContributiviQuotaFondoINPGI;
        }

        private void InitBindDataRetributivi()
        {
            List<DatiRetributiviQuotaFondoINPGILocal> elencoDatiRetributivi = new List<DatiRetributiviQuotaFondoINPGILocal>();

            AreaTitolare.DatiPensione datiPensione = new AreaTitolare.DatiPensione();
            datiPensione = GetDatiPensione(this);
            if (this.domanda == null)
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            if (((AreaDatiContributivi)ViewState["areaDatiContributiviAgo"]).DatiQuotaFondoINPGI.lDatiRetributiviQuotaFondoINPGI != null)
                elencoDatiRetributivi = MapDatiRetributiviForView((AreaDatiContributivi)ViewState["areaDatiContributiviAgo"]);

            DatiRetributiviQuotaFondoINPGILocal Empty = elencoDatiRetributivi.Find(delegate(DatiRetributiviQuotaFondoINPGILocal code)
            {
                return (code.Gestione == string.Empty && code.Settimane == string.Empty && code.Quota == string.Empty);
            });

            if (Empty == null)
                elencoDatiRetributivi.Add(new DatiRetributiviQuotaFondoINPGILocal(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty));

            gvRetributiviINPGI.DataSource = elencoDatiRetributivi;
            ViewState[EnumViewState.ElencoDatiRetributiviQuotaFondoINPGI.ToString()] = elencoDatiRetributivi;


            gvRetributiviINPGI.DataBind();

            //SetPopUpContributivi(GetDatiPensione(this), elencoDatiContributivi);
        }

        private static List<DatiRetributiviQuotaFondoINPGILocal> MapDatiRetributiviForView(AreaDatiContributivi areaDatiContributivi)
        {
            List<DatiRetributiviQuotaFondoINPGILocal> elencoRetributiviQuotaFondoINPGI = new List<DatiRetributiviQuotaFondoINPGILocal>();
            foreach (GestioneQuotaFondoINPGIDatiCalcoloRetributivoINPGI contr in areaDatiContributivi.DatiQuotaFondoINPGI.lDatiRetributiviQuotaFondoINPGI.ToList<GestioneQuotaFondoINPGIDatiCalcoloRetributivoINPGI>())
            {
                string quota = string.Empty;
                string importoCalcolato = string.Empty;
                string importoComma707 = string.Empty;
                string retribuzioneMediaSettimanale = string.Empty;

                importoCalcolato = contr.ImportoCalcolato.HasValue ? contr.ImportoCalcolato.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;
                importoComma707 = contr.ImportoComma707.HasValue ? contr.ImportoComma707.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;
                retribuzioneMediaSettimanale = contr.RetribuzioneMediaSettimanale.HasValue ? contr.RetribuzioneMediaSettimanale.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;

                elencoRetributiviQuotaFondoINPGI.Add(new DatiRetributiviQuotaFondoINPGILocal(contr.CodiceGestione.HasValue ? contr.CodiceGestione.Value.ToString() : string.Empty, contr.Settimane.HasValue ? contr.Settimane.Value.ToString() : string.Empty,
                    quota, importoCalcolato, importoComma707, contr.SettimaneComma707.HasValue ? contr.SettimaneComma707.Value.ToString() : string.Empty, retribuzioneMediaSettimanale));
            }
            return elencoRetributiviQuotaFondoINPGI;
        }

        public void btnSalvaQuotaFondoINPGI_Click(object sender, EventArgs e)
        {
            this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
            this.areaDatiContributiviAgo = new AreaDatiContributivi();

            //ENG - RIC VOPGI NON CONTRIBUTIVE
            AreaTitolare.DatiPensione datiPensione = null;
            datiPensione = GetDatiPensione(this);

            RecuperaCampi(this.areaDatiContributiviAgo);

            if (this.areaDatiContributiviAgo.DatiQuotaFondoINPGI != null &&
                ((this.areaDatiContributiviAgo.DatiQuotaFondoINPGI.lDatiContributiviQuotaFondoINPGI != null && this.areaDatiContributiviAgo.DatiQuotaFondoINPGI.lDatiContributiviQuotaFondoINPGI.Count() > 0) ||
                (this.areaDatiContributiviAgo.DatiQuotaFondoINPGI.lDatiRetributiviQuotaFondoINPGI != null && this.areaDatiContributiviAgo.DatiQuotaFondoINPGI.lDatiRetributiviQuotaFondoINPGI.Count() > 0)))
            {
                PresenterDatiContributiviAGO presenterDatiContributiviAgo = new PresenterDatiContributiviAGO();
                presenterDatiContributiviAgo.SalvaDatiCalcoloQuotaFondoINPGI(this);

                //ENG - RIC VOPGI NON CONTRIBUTIVE
                if ((Utility.IsRicostituzione(datiPensione) && !Utility.IsRicostituzione_MotiviContributivi(datiPensione) && Utility.IsDomandaVOPGI(this.domanda.Categoria)) || Utility.IsDomandaUnicarpe(datiPensione, true) == Utility.TipoUnicarpe.Automatica ||
                    (((Utility.IsRicostituzione(datiPensione) && Utility.IsDomandaINPGI(this.domanda.Categoria)) ||
                    (!this.domanda.IsDomandaRiapertura && this.domanda.Categoria.Trim().ToUpperInvariant() == "SOPGI" && Utility.IsDomandaReversibilita(datiPensione))) && datiPensione.GP1AV91B == "2"))
                    btnEliminaQuotaFondoINPGI.Enabled = false;
                else
                    btnEliminaQuotaFondoINPGI.Enabled = true;

                ViewState["areaDatiContributiviAgo"] = this.areaDatiContributiviAgo;
                ReLoadData(this.areaDatiContributiviAgo.DatiQuotaFondoINPGI.lDatiRetributiviQuotaFondoINPGI != null ?
                    MapDatiRetributiviForView(this.areaDatiContributiviAgo) : null,
                    this.areaDatiContributiviAgo.DatiQuotaFondoINPGI.lDatiContributiviQuotaFondoINPGI != null ?
                    MapDatiContributiviForView(this.areaDatiContributiviAgo) : null);

            }
            else
            {
                this.HasError = true;
                this.ErrorMessage = "Non ci sono Dati Quota Fondo INPGI da salvare";
            }

            if (this.HasError)
            {
                RaiseHideAvviso(this, null);
                RaiseShowAvviso(this, null);
            }
            else
            {
                this.ErrorMessage = "Dati Quota Fondo INPGI salvati correttamente.";
                RaiseHideAvviso(this, null);
                RaiseShowAvviso(this, null);
            }
        }

        private void ReLoadData(List<DatiRetributiviQuotaFondoINPGILocal> listaDatiRetribApp, List<DatiContributiviQuotaFondoINPGILocal> listaDatiContribApp)
        {
            if (listaDatiRetribApp != null)
            {
                AreaTitolare.DatiPensione datiPensione = new AreaTitolare.DatiPensione();
                datiPensione = GetDatiPensione(this);
                if (this.domanda == null)
                    this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

                DatiRetributiviQuotaFondoINPGILocal EmptyRetr = listaDatiRetribApp.Find(delegate(DatiRetributiviQuotaFondoINPGILocal code)
                {
                    return (code.Gestione == string.Empty && code.Settimane == string.Empty && code.Quota == string.Empty);
                });

                if (EmptyRetr == null)
                    listaDatiRetribApp.Add(new DatiRetributiviQuotaFondoINPGILocal(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty));

                gvRetributiviINPGI.DataSource = listaDatiRetribApp;
                ViewState[EnumViewState.ElencoDatiRetributiviQuotaFondoINPGI.ToString()] = listaDatiRetribApp;
                gvRetributiviINPGI.DataBind();
            }

            if (listaDatiContribApp != null)
            {
                AreaTitolare.DatiPensione datiPensione = new AreaTitolare.DatiPensione();
                datiPensione = GetDatiPensione(this);
                if (this.domanda == null)
                    this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

                DatiContributiviQuotaFondoINPGILocal EmptyContr = listaDatiContribApp.Find(delegate(DatiContributiviQuotaFondoINPGILocal code)
                {
                    return (code.Gestione == string.Empty && code.Montante == string.Empty && code.Quota == string.Empty);
                });

                if (EmptyContr == null)
                    listaDatiContribApp.Add(new DatiContributiviQuotaFondoINPGILocal(string.Empty, string.Empty, string.Empty, string.Empty));

                gvContributiviINPGI.DataSource = listaDatiContribApp;
                ViewState[EnumViewState.ElencoDatiContributiviQuotaFondoINPGI.ToString()] = listaDatiContribApp;

                gvContributiviINPGI.DataBind();
            }
        }

        public void btnEliminaQuotaFondoINPGI_Click(object sender, EventArgs e)
        {
            this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            List<DatiContributiviQuotaFondoINPGILocal> listaDatiContrQuotaFondoIntApp = (List<DatiContributiviQuotaFondoINPGILocal>)ViewState[EnumViewState.ElencoDatiContributiviQuotaFondoINPGI.ToString()];
            List<DatiRetributiviQuotaFondoINPGILocal> listaDatiRetrQuotaFondoIntApp = (List<DatiRetributiviQuotaFondoINPGILocal>)ViewState[EnumViewState.ElencoDatiRetributiviQuotaFondoINPGI.ToString()];

            if ((listaDatiContrQuotaFondoIntApp != null && listaDatiContrQuotaFondoIntApp.Count() > 0) || (listaDatiRetrQuotaFondoIntApp != null && listaDatiRetrQuotaFondoIntApp.Count() > 0))
            {
                PresenterDatiContributiviAGO presenterDatiContributiviAgo = new PresenterDatiContributiviAGO();
                presenterDatiContributiviAgo.EliminaDatiCalcoloQuotaFondoINPGI(this);

                if (!this.HasError)
                {
                    if ((listaDatiContrQuotaFondoIntApp != null && listaDatiContrQuotaFondoIntApp.Count() > 0))
                        modalitaEditContributivi.Value = "false";
                    if ((listaDatiRetrQuotaFondoIntApp != null && listaDatiRetrQuotaFondoIntApp.Count() > 0))
                        modalitaEditRetributivi.Value = "false";
                    InitializeData(this, null);
                }
            }
            else
            {
                this.HasError = true;
                this.ErrorMessage = "Non ci sono Dati Quota Fondo INPGI da eliminare";
            }

            if (this.HasError)
                RaiseShowAvviso(this, null);
            else
            {
                this.ErrorMessage = "Dati Quota Fondo INPGI eliminati correttamente.";
                RaiseShowAvviso(this, null);
            }
        }

        internal void RecuperaCampi(AreaDatiContributivi areaDatiContributiviAgo)
        {
            this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            List<DatiContributiviQuotaFondoINPGILocal> listaDatiContrQuotaApp = (List<DatiContributiviQuotaFondoINPGILocal>)ViewState[EnumViewState.ElencoDatiContributiviQuotaFondoINPGI.ToString()];
            List<DatiRetributiviQuotaFondoINPGILocal> listaDatiRetrQuotaApp = (List<DatiRetributiviQuotaFondoINPGILocal>)ViewState[EnumViewState.ElencoDatiRetributiviQuotaFondoINPGI.ToString()];
            areaDatiContributiviAgo.DatiQuotaFondoINPGI = new GestioneContribDatiQuotaFondoINPGI();
            areaDatiContributiviAgo.DatiQuotaFondoINPGI.IdPensione = ((AreaDatiContributivi)ViewState["areaDatiContributiviAgo"]).DatiQuotaFondoINPGI.IdPensione;
            areaDatiContributiviAgo.IsSettimane707INPGIVisible = ((AreaDatiContributivi)ViewState["areaDatiContributiviAgo"]).IsSettimane707INPGIVisible;
            if (listaDatiContrQuotaApp != null && listaDatiContrQuotaApp.Count() > 0)
            {
                List<GestioneQuotaFondoINPGIDatiCalcoloContributivoINPGI> listQuotaFondo = GetDataContribQuotaFondoINPGIToSave(listaDatiContrQuotaApp);
                int nDatiQuotaFondo = listQuotaFondo.Count();
                areaDatiContributiviAgo.DatiQuotaFondoINPGI.lDatiContributiviQuotaFondoINPGI = new GestioneQuotaFondoINPGIDatiCalcoloContributivoINPGI[nDatiQuotaFondo];
                areaDatiContributiviAgo.DatiQuotaFondoINPGI.lDatiContributiviQuotaFondoINPGI = listQuotaFondo.ToArray();
            }
            if (listaDatiRetrQuotaApp != null && listaDatiRetrQuotaApp.Count() > 0)
            {
                List<GestioneQuotaFondoINPGIDatiCalcoloRetributivoINPGI> listQuotaFondo = GetDataRetribQuotaFondoINPGIToSave(listaDatiRetrQuotaApp, areaDatiContributiviAgo.IsSettimane707INPGIVisible);
                int nDatiQuotaFondo = listQuotaFondo.Count();
                areaDatiContributiviAgo.DatiQuotaFondoINPGI.lDatiRetributiviQuotaFondoINPGI = new GestioneQuotaFondoINPGIDatiCalcoloRetributivoINPGI[nDatiQuotaFondo];
                areaDatiContributiviAgo.DatiQuotaFondoINPGI.lDatiRetributiviQuotaFondoINPGI = listQuotaFondo.ToArray();
            }
        }

        private List<GestioneQuotaFondoINPGIDatiCalcoloContributivoINPGI> GetDataContribQuotaFondoINPGIToSave(List<DatiContributiviQuotaFondoINPGILocal> lDatiQuotaFondoINPGILocal)
        {
            List<GestioneQuotaFondoINPGIDatiCalcoloContributivoINPGI> lContr = new List<GestioneQuotaFondoINPGIDatiCalcoloContributivoINPGI>();

            foreach (DatiContributiviQuotaFondoINPGILocal datiQuotaFondoINPGILocal in lDatiQuotaFondoINPGILocal)
            {
                if (datiQuotaFondoINPGILocal.Montante == string.Empty && datiQuotaFondoINPGILocal.Gestione == string.Empty && datiQuotaFondoINPGILocal.Quota == string.Empty &&
                    datiQuotaFondoINPGILocal.Settimane == string.Empty)
                    continue;

                GestioneQuotaFondoINPGIDatiCalcoloContributivoINPGI Contr = new GestioneQuotaFondoINPGIDatiCalcoloContributivoINPGI();

                // reperire l'id del campo ddl Cod Gestione selezionato
                if (datiQuotaFondoINPGILocal.Gestione.Trim() != string.Empty)
                    Contr.CodiceGestione = Convert.ToInt64(datiQuotaFondoINPGILocal.Gestione.Trim());
                else
                    Contr.CodiceGestione = null;

                Contr.Montante = datiQuotaFondoINPGILocal.Montante.Trim() != string.Empty ? Convert.ToDecimal(datiQuotaFondoINPGILocal.Montante.Trim()) : (decimal?)null;
                Contr.Quota = datiQuotaFondoINPGILocal.Quota.Trim() != string.Empty ? Convert.ToDecimal(datiQuotaFondoINPGILocal.Quota.Trim()) : (decimal?)null;
                Contr.Settimane = datiQuotaFondoINPGILocal.Settimane.Trim() != string.Empty ? Convert.ToInt32(datiQuotaFondoINPGILocal.Settimane.Trim()) : (int?)null;

                lContr.Add(Contr);
            }
            return lContr;
        }

        private List<GestioneQuotaFondoINPGIDatiCalcoloRetributivoINPGI> GetDataRetribQuotaFondoINPGIToSave(List<DatiRetributiviQuotaFondoINPGILocal> lDatiQuotaFondoINPGILocal, bool IsSettimane707INPGIVisible)
        {
            List<GestioneQuotaFondoINPGIDatiCalcoloRetributivoINPGI> lContr = new List<GestioneQuotaFondoINPGIDatiCalcoloRetributivoINPGI>();

            foreach (DatiRetributiviQuotaFondoINPGILocal datiQuotaFondoINPGILocal in lDatiQuotaFondoINPGILocal)
            {
                if (datiQuotaFondoINPGILocal.Settimane == string.Empty && datiQuotaFondoINPGILocal.Gestione == string.Empty && datiQuotaFondoINPGILocal.Quota == string.Empty &&
                    datiQuotaFondoINPGILocal.ImportoCalcolato == string.Empty && datiQuotaFondoINPGILocal.ImportoComma707 == string.Empty && datiQuotaFondoINPGILocal.SettimaneComma707 == string.Empty)
                    continue;

                GestioneQuotaFondoINPGIDatiCalcoloRetributivoINPGI Contr = new GestioneQuotaFondoINPGIDatiCalcoloRetributivoINPGI();

                // reperire l'id del campo ddl Cod Gestione selezionato
                if (datiQuotaFondoINPGILocal.Gestione.Trim() != string.Empty)
                    Contr.CodiceGestione = Convert.ToInt64(datiQuotaFondoINPGILocal.Gestione.Trim());
                else
                    Contr.CodiceGestione = null;

                Contr.Settimane = datiQuotaFondoINPGILocal.Settimane.Trim() != string.Empty ? Convert.ToInt32(datiQuotaFondoINPGILocal.Settimane.Trim()) : (int?)null;
                Contr.ImportoCalcolato = datiQuotaFondoINPGILocal.ImportoCalcolato.Trim() != string.Empty ? Convert.ToDecimal(datiQuotaFondoINPGILocal.ImportoCalcolato.Trim()) : (decimal?)null;
                Contr.ImportoComma707 = (IsSettimane707INPGIVisible && datiQuotaFondoINPGILocal.ImportoComma707.Trim() != string.Empty) ? Convert.ToDecimal(datiQuotaFondoINPGILocal.ImportoComma707.Trim()) : (decimal?)null;
                Contr.SettimaneComma707 = (IsSettimane707INPGIVisible && datiQuotaFondoINPGILocal.SettimaneComma707.Trim() != string.Empty) ? Convert.ToInt32(datiQuotaFondoINPGILocal.SettimaneComma707.Trim()) : (int?)null;
                Contr.RetribuzioneMediaSettimanale = datiQuotaFondoINPGILocal.RetribuzioneMediaSettimanale.Trim() != string.Empty ? Convert.ToDecimal(datiQuotaFondoINPGILocal.RetribuzioneMediaSettimanale.Trim()) : (decimal?)null;
                lContr.Add(Contr);
            }
            return lContr;
        }

        protected void gvContributiviINPGI_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            AreaTitolare.DatiPensione datiPensione = GetDatiPensione(this);
            List<DatiContributiviQuotaFondoINPGILocal> listaDatiQuotaFondoIntApp = (List<DatiContributiviQuotaFondoINPGILocal>)ViewState[EnumViewState.ElencoDatiContributiviQuotaFondoINPGI.ToString()];
            this.areaDatiContributiviAgo = (AreaDatiContributivi)ViewState["areaDatiContributiviAgo"];

            if (e.CommandName == "Elimina")
            {
                Control c = (Control)e.CommandSource;
                GridViewRow r = (GridViewRow)c.NamingContainer;
                listaDatiQuotaFondoIntApp.RemoveAt(r.DataItemIndex);
                ViewState[EnumViewState.ElencoDatiContributiviQuotaFondoINPGI.ToString()] = listaDatiQuotaFondoIntApp;
                gvContributiviINPGI.EditIndex = -1;
                gvContributiviINPGI.DataSource = listaDatiQuotaFondoIntApp;
                gvContributiviINPGI.DataBind();

            }
            else if (e.CommandName == "Edit")
            {
                this.modalitaEditContributivi.Value = "true";
            }
            else if (e.CommandName == "Salva")
            {
                if (!IsEmptyEditableRowContr((GridViewRow)((Control)e.CommandSource).NamingContainer))
                {
                    GridViewRow r = (GridViewRow)((WebControl)e.CommandSource).NamingContainer;
                    int index = r.DataItemIndex;

                    listaDatiQuotaFondoIntApp[index].Quota = ((TextBox)r.FindControl("txtQuota")).Text;
                    listaDatiQuotaFondoIntApp[index].Montante = ((TextBox)r.FindControl("txtMontante")).Text;
                    listaDatiQuotaFondoIntApp[index].Gestione = ((DropDownList)r.FindControl("ddlCodiceGestioneQuotaFondo")).SelectedValue;
                    listaDatiQuotaFondoIntApp[index].Settimane = ((TextBox)r.FindControl("txtSettimaneContr")).Text;

                    // Sto inserendo un nuovo record
                    if (index == listaDatiQuotaFondoIntApp.Count - 1)
                        listaDatiQuotaFondoIntApp.Add(new DatiContributiviQuotaFondoINPGILocal(string.Empty, string.Empty, string.Empty, string.Empty));
                    gvContributiviINPGI.EditIndex = -1;
                    ViewState[EnumViewState.ElencoDatiContributiviQuotaFondoINPGI.ToString()] = listaDatiQuotaFondoIntApp;
                    this.modalitaEditContributivi.Value = "false";
                    btnSalvaQuotaFondoINPGI.Enabled = true;
                    btnEliminaQuotaFondoINPGI.Enabled = true;
                    RaiseGestisciTastoSalva(this, null);
                    gvContributiviINPGI.DataSource = listaDatiQuotaFondoIntApp;
                    gvContributiviINPGI.DataBind();
                }
            }
            else if (e.CommandName == "Annulla")
            {
                if (!IsListaEmpty(false))
                {
                    this.modalitaEditContributivi.Value = "false";
                    gvContributiviINPGI.EditIndex = -1;
                    gvContributiviINPGI.DataSource = listaDatiQuotaFondoIntApp;
                    gvContributiviINPGI.DataBind();
                    RaiseGestisciTastoSalva(this, null);
                    btnSalvaQuotaFondoINPGI.Enabled = true;
                    btnEliminaQuotaFondoINPGI.Enabled = true;
                }
            }
        }

        protected void gvRetributiviINPGI_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            AreaTitolare.DatiPensione datiPensione = GetDatiPensione(this);
            List<DatiRetributiviQuotaFondoINPGILocal> listaDatiQuotaFondoIntApp = (List<DatiRetributiviQuotaFondoINPGILocal>)ViewState[EnumViewState.ElencoDatiRetributiviQuotaFondoINPGI.ToString()];
            this.areaDatiContributiviAgo = (AreaDatiContributivi)ViewState["areaDatiContributiviAgo"];

            if (e.CommandName == "Elimina")
            {
                Control c = (Control)e.CommandSource;
                GridViewRow r = (GridViewRow)c.NamingContainer;
                listaDatiQuotaFondoIntApp.RemoveAt(r.DataItemIndex);
                ViewState[EnumViewState.ElencoDatiRetributiviQuotaFondoINPGI.ToString()] = listaDatiQuotaFondoIntApp;
                gvRetributiviINPGI.EditIndex = -1;
                gvRetributiviINPGI.DataSource = listaDatiQuotaFondoIntApp;
                gvRetributiviINPGI.DataBind();

            }
            else if (e.CommandName == "Edit")
            {
                this.modalitaEditRetributivi.Value = "true";
            }
            else if (e.CommandName == "Salva")
            {
                if (!IsEmptyEditableRowRetrib((GridViewRow)((Control)e.CommandSource).NamingContainer))
                {
                    GridViewRow r = (GridViewRow)((WebControl)e.CommandSource).NamingContainer;
                    int index = r.DataItemIndex;

                    listaDatiQuotaFondoIntApp[index].Settimane = ((TextBox)r.FindControl("txtSettimane")).Text;
                    listaDatiQuotaFondoIntApp[index].Gestione = ((DropDownList)r.FindControl("ddlCodiceGestioneRetribQuotaFondo")).SelectedValue;
                    listaDatiQuotaFondoIntApp[index].ImportoCalcolato = ((TextBox)r.FindControl("txtImportoCalcolato")).Text;
                    listaDatiQuotaFondoIntApp[index].ImportoComma707 = ((TextBox)r.FindControl("txtImportoComma707")).Text;
                    listaDatiQuotaFondoIntApp[index].SettimaneComma707 = ((TextBox)r.FindControl("txtSettimaneComma707")).Text;
                    listaDatiQuotaFondoIntApp[index].RetribuzioneMediaSettimanale = ((TextBox)r.FindControl("txtRetribuzioneMediaSettimanale")).Text;

                    // Sto inserendo un nuovo record
                    if (index == listaDatiQuotaFondoIntApp.Count - 1)
                        listaDatiQuotaFondoIntApp.Add(new DatiRetributiviQuotaFondoINPGILocal(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty));
                    gvRetributiviINPGI.EditIndex = -1;
                    ViewState[EnumViewState.ElencoDatiRetributiviQuotaFondoINPGI.ToString()] = listaDatiQuotaFondoIntApp;
                    this.modalitaEditRetributivi.Value = "false";
                    btnSalvaQuotaFondoINPGI.Enabled = true;
                    btnEliminaQuotaFondoINPGI.Enabled = true;
                    RaiseGestisciTastoSalva(this, null);
                    gvRetributiviINPGI.DataSource = listaDatiQuotaFondoIntApp;
                    gvRetributiviINPGI.DataBind();
                }
            }
            else if (e.CommandName == "Annulla")
            {
                if (!IsListaEmpty(false))
                {
                    this.modalitaEditRetributivi.Value = "false";
                    gvRetributiviINPGI.EditIndex = -1;
                    gvRetributiviINPGI.DataSource = listaDatiQuotaFondoIntApp;
                    gvRetributiviINPGI.DataBind();
                    RaiseGestisciTastoSalva(this, null);
                    btnSalvaQuotaFondoINPGI.Enabled = true;
                    btnEliminaQuotaFondoINPGI.Enabled = true;
                }
            }
        }

        protected void gvContributiviINPGI_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            try
            {
                gvContributiviINPGI.EditIndex = -1;
                List<DatiContributiviQuotaFondoINPGILocal> listaDatiContrApp = (List<DatiContributiviQuotaFondoINPGILocal>)ViewState[EnumViewState.ElencoDatiContributiviQuotaFondoINPGI.ToString()];
                gvContributiviINPGI.DataSource = listaDatiContrApp;
                gvContributiviINPGI.DataBind();
                btnSalvaQuotaFondoINPGI.Enabled = true;
                btnEliminaQuotaFondoINPGI.Enabled = true;
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new DnaApplicationException("UCQuotaFondoINPGI, Errore nel metodo gvContributiviINPGI_RowCancelingEdit " + ex);
            }
        }

        protected void gvRetributiviINPGI_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            try
            {
                gvRetributiviINPGI.EditIndex = -1;
                List<DatiRetributiviQuotaFondoINPGILocal> listaDatiRetrApp = (List<DatiRetributiviQuotaFondoINPGILocal>)ViewState[EnumViewState.ElencoDatiRetributiviQuotaFondoINPGI.ToString()];
                gvRetributiviINPGI.DataSource = listaDatiRetrApp;
                gvRetributiviINPGI.DataBind();
                btnSalvaQuotaFondoINPGI.Enabled = true;
                btnEliminaQuotaFondoINPGI.Enabled = true;
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new DnaApplicationException("UCQuotaFondoINPGI, Errore nel metodo gvRetributiviINPGI_RowCancelingEdit " + ex);
            }
        }

        protected void gvContributiviINPGI_RowEditing(object sender, GridViewEditEventArgs e)
        {
            try
            {
                gvContributiviINPGI.EditIndex = e.NewEditIndex;
                List<DatiContributiviQuotaFondoINPGILocal> listaDatiContrApp = (List<DatiContributiviQuotaFondoINPGILocal>)ViewState[EnumViewState.ElencoDatiContributiviQuotaFondoINPGI.ToString()];
                gvContributiviINPGI.DataSource = listaDatiContrApp;
                gvContributiviINPGI.DataBind();

            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCQuotaFondoINPGI, Errore nel metodo gvContributiviINPGI_RowEditing " + ex);
            }
        }
        protected void gvRetributiviINPGI_RowEditing(object sender, GridViewEditEventArgs e)
        {
            try
            {
                gvRetributiviINPGI.EditIndex = e.NewEditIndex;
                List<DatiRetributiviQuotaFondoINPGILocal> listaDatiRetrApp = (List<DatiRetributiviQuotaFondoINPGILocal>)ViewState[EnumViewState.ElencoDatiRetributiviQuotaFondoINPGI.ToString()];
                gvRetributiviINPGI.DataSource = listaDatiRetrApp;
                gvRetributiviINPGI.DataBind();

            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCQuotaFondoINPGI, Errore nel metodo gvRetributiviINPGI_RowEditing " + ex);
            }
        }

        protected void gvContributiviINPGI_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {

        }

        protected void gvRetributiviINPGI_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {

        }

        protected void gvContributiviINPGI_DataBound(object sender, EventArgs e)
        {

        }

        protected void gvRetributiviINPGI_DataBound(object sender, EventArgs e)
        {

        }

        private bool IsEmptyEditableRowContr(GridViewRow row)
        {
            if (row.FindControl("txtMontante") != null && ((TextBox)row.FindControl("txtMontante")).Text != string.Empty &&
                row.FindControl("ddlCodiceGestioneQuotaFondo") != null && ((DropDownList)row.FindControl("ddlCodiceGestioneQuotaFondo")).SelectedIndex != 0 &&
                row.FindControl("txtQuota") != null && ((TextBox)row.FindControl("txtQuota")).Text != string.Empty &&
                row.FindControl("txtSettimaneContr") != null && ((TextBox)row.FindControl("txtSettimaneContr")).Text != string.Empty)
                return false;
            else
                return true;
        }

        private bool IsEmptyEditableRowRetrib(GridViewRow row)
        {
            if ((row.FindControl("txtSettimane") != null && ((TextBox)row.FindControl("txtSettimane")).Text != string.Empty) ||
                (row.FindControl("ddlCodiceGestioneRetribQuotaFondo") != null && ((DropDownList)row.FindControl("ddlCodiceGestioneRetribQuotaFondo")).SelectedIndex != 0) ||
                (row.FindControl("txtImportoCalcolato") != null && ((TextBox)row.FindControl("txtImportoCalcolato")).Text != string.Empty) ||
                (row.FindControl("txtImportoComma707") != null && ((TextBox)row.FindControl("txtImportoComma707")).Text != string.Empty) ||
                (row.FindControl("txtSettimaneComma707") != null && ((TextBox)row.FindControl("txtSettimaneComma707")).Text != string.Empty) ||
                (row.FindControl("txtRetribuzioneMediaSettimanale") != null && ((TextBox)row.FindControl("txtRetribuzioneMediaSettimanale")).Text != string.Empty))
                return false;
            else
                return true;
        }

        private void GestioneDdls(GridViewRow row, bool IsRetrib)
        {
            if (IsRetrib)
            {
                DropDownList ddlGestione = new DropDownList();
                ddlGestione = (DropDownList)row.FindControl("ddlCodiceGestioneRetribQuotaFondo");
                ddlGestione.Items.Add(new ListItem(string.Empty, string.Empty));
                Label lblCodiceGestione_item = (Label)row.FindControl("lblCodiceGestioneRetribQuotaFondo_item");

                DecodificaGestioneQuotaFondoINPGI[] listaCodeGestioneCalcoloRetrib = (DecodificaGestioneQuotaFondoINPGI[])ViewState["listaCodeGestioneQuotaFondoINPGI"];
                IEnumerable<DecodificaGestioneQuotaFondoINPGI> listaOrdinata = listaCodeGestioneCalcoloRetrib.OrderBy(x => x.Descrizione);
                listaOrdinata = listaOrdinata.Select(x => x).Where(x => x.TipoQuota == "R").ToList();
                foreach (DecodificaGestioneQuotaFondoINPGI datiCodeGestioneCalcoloRetrib in listaOrdinata)
                {
                    ListItem li = new ListItem();
                    li.Attributes.Add("title", datiCodeGestioneCalcoloRetrib.Descrizione);
                    li.Text = datiCodeGestioneCalcoloRetrib.Descrizione;
                    li.Value = datiCodeGestioneCalcoloRetrib.Id.ToString();
                    ddlGestione.Items.Add(li);
                }
                if (((DatiRetributiviQuotaFondoINPGILocal)(row.DataItem)).Gestione.Trim() == string.Empty)
                    ddlGestione.SelectedIndex = 0;
                else
                    if (ddlGestione.Items.FindByValue(((DatiRetributiviQuotaFondoINPGILocal)(row.DataItem)).Gestione.Trim()) != null)
                        ddlGestione.Items.FindByValue(((DatiRetributiviQuotaFondoINPGILocal)(row.DataItem)).Gestione.Trim()).Selected = true;
                    else
                        ddlGestione.SelectedIndex = 0;
            }
            else
            {
                DropDownList ddlGestione = new DropDownList();
                ddlGestione = (DropDownList)row.FindControl("ddlCodiceGestioneQuotaFondo");
                ddlGestione.Items.Add(new ListItem(string.Empty, string.Empty));
                Label lblCodiceGestione_item = (Label)row.FindControl("lblCodiceGestioneQuotaFondo_item");

                DecodificaGestioneQuotaFondoINPGI[] listaCodeGestioneCalcoloContrib = (DecodificaGestioneQuotaFondoINPGI[])ViewState["listaCodeGestioneQuotaFondoINPGI"];
                IEnumerable<DecodificaGestioneQuotaFondoINPGI> listaOrdinata = listaCodeGestioneCalcoloContrib.OrderBy(x => x.Descrizione);
                listaOrdinata = listaOrdinata.Select(x => x).Where(x => x.TipoQuota == "C").ToList();
                foreach (DecodificaGestioneQuotaFondoINPGI datiCodeGestioneCalcoloContrib in listaOrdinata)
                {
                    ListItem li = new ListItem();
                    li.Attributes.Add("title", datiCodeGestioneCalcoloContrib.Descrizione);
                    li.Text = datiCodeGestioneCalcoloContrib.Descrizione;
                    li.Value = datiCodeGestioneCalcoloContrib.Id.ToString();

                    ddlGestione.Items.Add(li);
                }
                if (((DatiContributiviQuotaFondoINPGILocal)(row.DataItem)).Gestione.Trim() == string.Empty)
                    ddlGestione.SelectedIndex = 0;
                else
                    if (ddlGestione.Items.FindByValue(((DatiContributiviQuotaFondoINPGILocal)(row.DataItem)).Gestione.Trim()) != null)
                        ddlGestione.Items.FindByValue(((DatiContributiviQuotaFondoINPGILocal)(row.DataItem)).Gestione.Trim()).Selected = true;
                    else
                        ddlGestione.SelectedIndex = 0;
            }
        }

        protected void gvContributiviINPGI_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                AreaTitolare.DatiPensione datiPensione = null;
                datiPensione = GetDatiPensione(this);

                //ENG - Spacchettamento SOPGI
                if (this.areaDanteCausa == null)
                {
                    PresenterDanteCausa presenterDanteCausa = new PresenterDanteCausa();
                    presenterDanteCausa.GetDatiDanteCausa(this);
                }

                //ENG - Spacchettamento SOPGI
                bool disabilitaDatiCalcoloSpacchettamentoSOPGI = false;
                if (Utility.IsDomandaSpacchettamentoSOPGIPost072022(this.domanda.Categoria, datiPensione, this.areaDanteCausa))
                {
                    if (Utility.IsDomandaReversibilita(datiPensione) || (Utility.IsDomandaIndiretta(datiPensione) && !this.areaDanteCausa.IsFascicoloGenerato.GetValueOrDefault()))
                    {
                        disabilitaDatiCalcoloSpacchettamentoSOPGI = true;
                    }
                }

                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    //ENG - RIC VOPGI NON CONTRIBUTIVE
                    //sola lettura
                    if (((Utility.IsRicostituzione(datiPensione) || Utility.IsDomandaRipristino(datiPensione)) && !Utility.IsRicostituzione_MotiviContributivi(datiPensione) && Utility.IsDomandaVOPGI(this.domanda.Categoria))
                        || disabilitaDatiCalcoloSpacchettamentoSOPGI || Utility.IsDomandaUnicarpe(datiPensione, true) == Utility.TipoUnicarpe.Automatica
                        || (((Utility.IsRicostituzione(datiPensione) && Utility.IsDomandaINPGI(this.domanda.Categoria)) || 
                        (!this.domanda.IsDomandaRiapertura && this.domanda.Categoria.Trim().ToUpperInvariant() == "SOPGI" && Utility.IsDomandaReversibilita(datiPensione))) && datiPensione.GP1AV91B == "2"))
                    {
                        gvContributiviINPGI.EditIndex = -1;
                        ((Label)e.Row.FindControl("lblCodiceGestioneQuotaFondo_item")).Text = GetValueFromId(((DatiContributiviQuotaFondoINPGILocal)(e.Row.DataItem)).Gestione);
                        ((Label)e.Row.FindControl("lblCodiceGestioneQuotaFondo_item")).Attributes.Add("title", ((Label)e.Row.FindControl("lblCodiceGestioneQuotaFondo_item")).Text);
                        ((Label)e.Row.FindControl("lblIdCodeGestione")).Text = ((DatiContributiviQuotaFondoINPGILocal)(e.Row.DataItem)).Gestione;
                        ((Label)e.Row.FindControl("lblQuota")).Text = !string.IsNullOrEmpty(((DatiContributiviQuotaFondoINPGILocal)(e.Row.DataItem)).Quota) ? CodeUtility.ConvertDecimalFixedPoint(((DatiContributiviQuotaFondoINPGILocal)(e.Row.DataItem)).Quota, 4) : "";
                        ((Label)e.Row.FindControl("lblMontante")).Text = !string.IsNullOrEmpty(((DatiContributiviQuotaFondoINPGILocal)(e.Row.DataItem)).Montante) ? CodeUtility.ConvertDecimalFixedPoint(((DatiContributiviQuotaFondoINPGILocal)(e.Row.DataItem)).Montante, 4) : "";
                        ((Label)e.Row.FindControl("lblPeriodoContr")).Text = GetValueFromIdGestione(((DatiContributiviQuotaFondoINPGILocal)(e.Row.DataItem)).Gestione);
                        ((Label)e.Row.FindControl("lblSettimaneContr")).Text = ((DatiContributiviQuotaFondoINPGILocal)(e.Row.DataItem)).Settimane;

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
                                //ManagePulsanti(); ----> ???
                                gvContributiviINPGI.EditIndex = 0;
                                modalitaEditContributivi.Value = "true";

                                gvContributiviINPGI.DataSource = (List<DatiContributiviQuotaFondoINPGILocal>)ViewState[EnumViewState.ElencoDatiContributiviQuotaFondoINPGI.ToString()];
                                gvContributiviINPGI.DataBind();
                            }
                            else if (IsEmptyEditableRowContr(e.Row))
                            {
                                if (e.Row.Cells[0].Controls.Count == 3)  // edit mode
                                {
                                    GestioneDdls(e.Row, false);
                                    CodeUtility.EnableEditableMode(e.Row.Cells[0], "UCTabQuotaContrFondoINPGIAgo", Page.Theme);
                                    LinkButton delete = ((LinkButton)(e.Row.Cells[5].FindControl("btnDeleteQuotaContribFondoINPGI")));
                                    delete.Text = string.Empty;

                                }
                                else
                                {
                                    ((Label)e.Row.FindControl("lblCodiceGestioneQuotaFondo_item")).Text = GetValueFromId(((DatiContributiviQuotaFondoINPGILocal)(e.Row.DataItem)).Gestione);
                                    ((Label)e.Row.FindControl("lblCodiceGestioneQuotaFondo_item")).Attributes.Add("title", ((Label)e.Row.FindControl("lblCodiceGestioneQuotaFondo_item")).Text);
                                    ((Label)e.Row.FindControl("lblIdCodeGestione")).Text = ((DatiContributiviQuotaFondoINPGILocal)(e.Row.DataItem)).Gestione;
                                    ((Label)e.Row.FindControl("lblQuota")).Text = !string.IsNullOrEmpty(((DatiContributiviQuotaFondoINPGILocal)(e.Row.DataItem)).Quota) ? CodeUtility.ConvertDecimalFixedPoint(((DatiContributiviQuotaFondoINPGILocal)(e.Row.DataItem)).Quota, 4) : "";
                                    ((Label)e.Row.FindControl("lblMontante")).Text = !string.IsNullOrEmpty(((DatiContributiviQuotaFondoINPGILocal)(e.Row.DataItem)).Montante) ? CodeUtility.ConvertDecimalFixedPoint(((DatiContributiviQuotaFondoINPGILocal)(e.Row.DataItem)).Montante, 4) : "";
                                    ((Label)e.Row.FindControl("lblPeriodoContr")).Text = GetValueFromIdGestione(((DatiContributiviQuotaFondoINPGILocal)(e.Row.DataItem)).Gestione);
                                    ((Label)e.Row.FindControl("lblSettimaneContr")).Text = ((DatiContributiviQuotaFondoINPGILocal)(e.Row.DataItem)).Settimane;
                                    CodeUtility.EnableReadableMode(e.Row.Cells[0], e.Row.Cells[5], Page.Theme, "btnDeleteQuotaContribFondoINPGI");
                                }
                            }
                            else  //prima riga non vuota
                            {
                                if (e.Row.Cells[0].Controls.Count == 3)  // edit mode
                                {
                                    GestioneDdls(e.Row, false);
                                    CodeUtility.EnableEditableMode(e.Row.Cells[0], "UCTabQuotaContrFondoINPGIAgo", Page.Theme);

                                }
                                else
                                {
                                    ((Label)e.Row.FindControl("lblCodiceGestioneQuotaFondo_item")).Text = GetValueFromId(((DatiContributiviQuotaFondoINPGILocal)(e.Row.DataItem)).Gestione);
                                    ((Label)e.Row.FindControl("lblCodiceGestioneQuotaFondo_item")).Attributes.Add("title", ((Label)e.Row.FindControl("lblCodiceGestioneQuotaFondo_item")).Text);
                                    ((Label)e.Row.FindControl("lblIdCodeGestione")).Text = ((DatiContributiviQuotaFondoINPGILocal)(e.Row.DataItem)).Gestione;
                                    ((Label)e.Row.FindControl("lblQuota")).Text = !string.IsNullOrEmpty(((DatiContributiviQuotaFondoINPGILocal)(e.Row.DataItem)).Quota) ? CodeUtility.ConvertDecimalFixedPoint(((DatiContributiviQuotaFondoINPGILocal)(e.Row.DataItem)).Quota, 4) : "";
                                    ((Label)e.Row.FindControl("lblMontante")).Text = !string.IsNullOrEmpty(((DatiContributiviQuotaFondoINPGILocal)(e.Row.DataItem)).Montante) ? CodeUtility.ConvertDecimalFixedPoint(((DatiContributiviQuotaFondoINPGILocal)(e.Row.DataItem)).Montante, 4) : "";
                                    ((Label)e.Row.FindControl("lblPeriodoContr")).Text = GetValueFromIdGestione(((DatiContributiviQuotaFondoINPGILocal)(e.Row.DataItem)).Gestione);
                                    ((Label)e.Row.FindControl("lblSettimaneContr")).Text = ((DatiContributiviQuotaFondoINPGILocal)(e.Row.DataItem)).Settimane;
                                    CodeUtility.EnableReadableMode(e.Row.Cells[0], e.Row.Cells[5], Page.Theme, "btnDeleteQuotaContribFondoINPGI");
                                }
                            }
                        }
                        else  // righe successive
                        {
                            if (e.Row.Cells[0].Controls.Count == 3)  // edit mode
                            {
                                GestioneDdls(e.Row, false);
                                CodeUtility.EnableEditableMode(e.Row.Cells[0], "UCTabQuotaContrFondoINPGIAgo", Page.Theme);
                            }

                            else if (e.Row.DataItemIndex == ((List<DatiContributiviQuotaFondoINPGILocal>)ViewState[EnumViewState.ElencoDatiContributiviQuotaFondoINPGI.ToString()]).Count - 1)
                            {
                                LinkButton add = ((LinkButton)(e.Row.Cells[0].Controls[0]));
                                add.Text = (string)"<img width=16 height=16 border=0 src=../App_themes/" + Page.Theme + "/Images/add24.png />";
                                add.ToolTip = "Aggiungi";
                            }
                            else
                            {
                                ((Label)e.Row.FindControl("lblCodiceGestioneQuotaFondo_item")).Text = GetValueFromId(((DatiContributiviQuotaFondoINPGILocal)(e.Row.DataItem)).Gestione);
                                ((Label)e.Row.FindControl("lblCodiceGestioneQuotaFondo_item")).Attributes.Add("title", ((Label)e.Row.FindControl("lblCodiceGestioneQuotaFondo_item")).Text);
                                ((Label)e.Row.FindControl("lblIdCodeGestione")).Text = ((DatiContributiviQuotaFondoINPGILocal)(e.Row.DataItem)).Gestione;
                                ((Label)e.Row.FindControl("lblQuota")).Text = !string.IsNullOrEmpty(((DatiContributiviQuotaFondoINPGILocal)(e.Row.DataItem)).Quota) ? CodeUtility.ConvertDecimalFixedPoint(((DatiContributiviQuotaFondoINPGILocal)(e.Row.DataItem)).Quota, 4) : "";
                                ((Label)e.Row.FindControl("lblMontante")).Text = !string.IsNullOrEmpty(((DatiContributiviQuotaFondoINPGILocal)(e.Row.DataItem)).Montante) ? CodeUtility.ConvertDecimalFixedPoint(((DatiContributiviQuotaFondoINPGILocal)(e.Row.DataItem)).Montante, 4) : "";
                                ((Label)e.Row.FindControl("lblPeriodoContr")).Text = GetValueFromIdGestione(((DatiContributiviQuotaFondoINPGILocal)(e.Row.DataItem)).Gestione);
                                ((Label)e.Row.FindControl("lblSettimaneContr")).Text = ((DatiContributiviQuotaFondoINPGILocal)(e.Row.DataItem)).Settimane;
                                CodeUtility.EnableReadableMode(e.Row.Cells[0], e.Row.Cells[5], Page.Theme, "btnDeleteQuotaContribFondoINPGI");

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
                throw new INPS.DNA.DnaApplicationException("UCDatiContributiviAgo, Errore nel metodo gvContributiviINPGI_RowDataBound " + ex);
            }
        }

        protected void gvRetributiviINPGI_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                AreaTitolare.DatiPensione datiPensione = null;
                datiPensione = GetDatiPensione(this);

                //ENG - Spacchettamento SOPGI
                if (this.areaDanteCausa == null)
                {
                    PresenterDanteCausa presenterDanteCausa = new PresenterDanteCausa();
                    presenterDanteCausa.GetDatiDanteCausa(this);
                }

                //ENG - Spacchettamento SOPGI
                bool disabilitaDatiCalcoloSpacchettamentoSOPGI = false;
                if (Utility.IsDomandaSpacchettamentoSOPGIPost072022(this.domanda.Categoria, datiPensione, this.areaDanteCausa))
                {
                    if (Utility.IsDomandaReversibilita(datiPensione) || (Utility.IsDomandaIndiretta(datiPensione) && !this.areaDanteCausa.IsFascicoloGenerato.GetValueOrDefault()))
                    {
                        disabilitaDatiCalcoloSpacchettamentoSOPGI = true;
                    }
                }

                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    //ENG - RIC VOPGI NON CONTRIBUTIVE
                    //sola lettura
                    if ((Utility.IsRicostituzione(datiPensione) && !Utility.IsRicostituzione_MotiviContributivi(datiPensione) && Utility.IsDomandaVOPGI(this.domanda.Categoria) || Utility.IsDomandaUnicarpe(datiPensione, true) == Utility.TipoUnicarpe.Automatica)
                        || disabilitaDatiCalcoloSpacchettamentoSOPGI)
                    {
                        gvRetributiviINPGI.EditIndex = -1;
                        ((Label)e.Row.FindControl("lblCodiceGestioneRetribQuotaFondo_item")).Text = GetValueFromId(((DatiRetributiviQuotaFondoINPGILocal)(e.Row.DataItem)).Gestione);
                        ((Label)e.Row.FindControl("lblCodiceGestioneRetribQuotaFondo_item")).Attributes.Add("title", ((Label)e.Row.FindControl("lblCodiceGestioneRetribQuotaFondo_item")).Text);
                        ((Label)e.Row.FindControl("lblIdCodeGestione")).Text = ((DatiRetributiviQuotaFondoINPGILocal)(e.Row.DataItem)).Gestione;
                        ((Label)e.Row.FindControl("lblSettimane")).Text = ((DatiRetributiviQuotaFondoINPGILocal)(e.Row.DataItem)).Settimane;
                        ((Label)e.Row.FindControl("lblPeriodoRetr")).Text = GetValueFromIdGestione(((DatiRetributiviQuotaFondoINPGILocal)(e.Row.DataItem)).Gestione);
                        ((Label)e.Row.FindControl("lblImportoCalcolato")).Text = !string.IsNullOrEmpty(((DatiRetributiviQuotaFondoINPGILocal)(e.Row.DataItem)).ImportoCalcolato) ? CodeUtility.ConvertDecimalFixedPoint(((DatiRetributiviQuotaFondoINPGILocal)(e.Row.DataItem)).ImportoCalcolato, 4) : "";
                        ((Label)e.Row.FindControl("lblImportoComma707")).Text = !string.IsNullOrEmpty(((DatiRetributiviQuotaFondoINPGILocal)(e.Row.DataItem)).ImportoComma707) ? CodeUtility.ConvertDecimalFixedPoint(((DatiRetributiviQuotaFondoINPGILocal)(e.Row.DataItem)).ImportoComma707, 4) : "";
                        ((Label)e.Row.FindControl("lblSettimaneComma707")).Text = ((DatiRetributiviQuotaFondoINPGILocal)(e.Row.DataItem)).SettimaneComma707;
                        ((Label)e.Row.FindControl("lblRetribuzioneMediaSettimanale")).Text = !string.IsNullOrEmpty(((DatiRetributiviQuotaFondoINPGILocal)(e.Row.DataItem)).RetribuzioneMediaSettimanale) ? CodeUtility.ConvertDecimalFixedPoint(((DatiRetributiviQuotaFondoINPGILocal)(e.Row.DataItem)).RetribuzioneMediaSettimanale, 4) : "";

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
                            if (IsListaEmpty(true) && !Convert.ToBoolean(modalitaEditRetributivi.Value))
                            {
                                //ManagePulsanti(); ----> ???
                                gvRetributiviINPGI.EditIndex = 0;
                                modalitaEditRetributivi.Value = "true";

                                gvRetributiviINPGI.DataSource = (List<DatiRetributiviQuotaFondoINPGILocal>)ViewState[EnumViewState.ElencoDatiRetributiviQuotaFondoINPGI.ToString()];
                                gvRetributiviINPGI.DataBind();
                            }
                            else if (IsEmptyEditableRowRetrib(e.Row))
                            {
                                if (e.Row.Cells[0].Controls.Count == 3)  // edit mode
                                {
                                    GestioneDdls(e.Row, true);
                                    CodeUtility.EnableEditableMode(e.Row.Cells[0], "UCTabQuotaRetrFondoINPGIAgo", Page.Theme);
                                    LinkButton delete = ((LinkButton)(e.Row.Cells[5].FindControl("btnDeleteQuotaRetribFondoINPGI")));
                                    delete.Text = string.Empty;

                                }
                                else
                                {
                                    ((Label)e.Row.FindControl("lblCodiceGestioneRetribQuotaFondo_item")).Text = GetValueFromId(((DatiRetributiviQuotaFondoINPGILocal)(e.Row.DataItem)).Gestione);
                                    ((Label)e.Row.FindControl("lblCodiceGestioneRetribQuotaFondo_item")).Attributes.Add("title", ((Label)e.Row.FindControl("lblCodiceGestioneRetribQuotaFondo_item")).Text);
                                    ((Label)e.Row.FindControl("lblIdCodeGestione")).Text = ((DatiRetributiviQuotaFondoINPGILocal)(e.Row.DataItem)).Gestione;
                                    ((Label)e.Row.FindControl("lblSettimane")).Text = ((DatiRetributiviQuotaFondoINPGILocal)(e.Row.DataItem)).Settimane;
                                    ((Label)e.Row.FindControl("lblPeriodoRetr")).Text = GetValueFromIdGestione(((DatiRetributiviQuotaFondoINPGILocal)(e.Row.DataItem)).Gestione);
                                    ((Label)e.Row.FindControl("lblImportoCalcolato")).Text = !string.IsNullOrEmpty(((DatiRetributiviQuotaFondoINPGILocal)(e.Row.DataItem)).ImportoCalcolato) ? CodeUtility.ConvertDecimalFixedPoint(((DatiRetributiviQuotaFondoINPGILocal)(e.Row.DataItem)).ImportoCalcolato, 4) : "";
                                    ((Label)e.Row.FindControl("lblImportoComma707")).Text = !string.IsNullOrEmpty(((DatiRetributiviQuotaFondoINPGILocal)(e.Row.DataItem)).ImportoComma707) ? CodeUtility.ConvertDecimalFixedPoint(((DatiRetributiviQuotaFondoINPGILocal)(e.Row.DataItem)).ImportoComma707, 4) : "";
                                    ((Label)e.Row.FindControl("lblSettimaneComma707")).Text = ((DatiRetributiviQuotaFondoINPGILocal)(e.Row.DataItem)).SettimaneComma707;
                                    ((Label)e.Row.FindControl("lblRetribuzioneMediaSettimanale")).Text = !string.IsNullOrEmpty(((DatiRetributiviQuotaFondoINPGILocal)(e.Row.DataItem)).RetribuzioneMediaSettimanale) ? CodeUtility.ConvertDecimalFixedPoint(((DatiRetributiviQuotaFondoINPGILocal)(e.Row.DataItem)).RetribuzioneMediaSettimanale, 4) : "";

                                    CodeUtility.EnableReadableMode(e.Row.Cells[0], e.Row.Cells[5], Page.Theme, "btnDeleteQuotaRetribFondoINPGI");
                                }
                            }
                            else  //prima riga non vuota
                            {
                                if (e.Row.Cells[0].Controls.Count == 3)  // edit mode
                                {
                                    GestioneDdls(e.Row, true);
                                    CodeUtility.EnableEditableMode(e.Row.Cells[0], "UCTabQuotaRetrFondoINPGIAgo", Page.Theme);

                                }
                                else
                                {
                                    ((Label)e.Row.FindControl("lblCodiceGestioneRetribQuotaFondo_item")).Text = GetValueFromId(((DatiRetributiviQuotaFondoINPGILocal)(e.Row.DataItem)).Gestione);
                                    ((Label)e.Row.FindControl("lblCodiceGestioneRetribQuotaFondo_item")).Attributes.Add("title", ((Label)e.Row.FindControl("lblCodiceGestioneRetribQuotaFondo_item")).Text);
                                    ((Label)e.Row.FindControl("lblIdCodeGestione")).Text = ((DatiRetributiviQuotaFondoINPGILocal)(e.Row.DataItem)).Gestione;
                                    ((Label)e.Row.FindControl("lblSettimane")).Text = ((DatiRetributiviQuotaFondoINPGILocal)(e.Row.DataItem)).Settimane;
                                    ((Label)e.Row.FindControl("lblPeriodoRetr")).Text = GetValueFromIdGestione(((DatiRetributiviQuotaFondoINPGILocal)(e.Row.DataItem)).Gestione);
                                    ((Label)e.Row.FindControl("lblImportoCalcolato")).Text = !string.IsNullOrEmpty(((DatiRetributiviQuotaFondoINPGILocal)(e.Row.DataItem)).ImportoCalcolato) ? CodeUtility.ConvertDecimalFixedPoint(((DatiRetributiviQuotaFondoINPGILocal)(e.Row.DataItem)).ImportoCalcolato, 4) : "";
                                    ((Label)e.Row.FindControl("lblImportoComma707")).Text = !string.IsNullOrEmpty(((DatiRetributiviQuotaFondoINPGILocal)(e.Row.DataItem)).ImportoComma707) ? CodeUtility.ConvertDecimalFixedPoint(((DatiRetributiviQuotaFondoINPGILocal)(e.Row.DataItem)).ImportoComma707, 4) : "";
                                    ((Label)e.Row.FindControl("lblSettimaneComma707")).Text = ((DatiRetributiviQuotaFondoINPGILocal)(e.Row.DataItem)).SettimaneComma707;
                                    ((Label)e.Row.FindControl("lblRetribuzioneMediaSettimanale")).Text = !string.IsNullOrEmpty(((DatiRetributiviQuotaFondoINPGILocal)(e.Row.DataItem)).RetribuzioneMediaSettimanale) ? CodeUtility.ConvertDecimalFixedPoint(((DatiRetributiviQuotaFondoINPGILocal)(e.Row.DataItem)).RetribuzioneMediaSettimanale, 4) : "";
                                    CodeUtility.EnableReadableMode(e.Row.Cells[0], e.Row.Cells[5], Page.Theme, "btnDeleteQuotaRetribFondoINPGI");
                                }
                            }
                        }
                        else  // righe successive
                        {
                            if (e.Row.Cells[0].Controls.Count == 3)  // edit mode
                            {
                                GestioneDdls(e.Row, true);
                                CodeUtility.EnableEditableMode(e.Row.Cells[0], "UCTabQuotaRetrFondoINPGIAgo", Page.Theme);
                            }

                            else if (e.Row.DataItemIndex == ((List<DatiRetributiviQuotaFondoINPGILocal>)ViewState[EnumViewState.ElencoDatiRetributiviQuotaFondoINPGI.ToString()]).Count - 1)
                            {
                                LinkButton add = ((LinkButton)(e.Row.Cells[0].Controls[0]));
                                add.Text = (string)"<img width=16 height=16 border=0 src=../App_themes/" + Page.Theme + "/Images/add24.png />";
                                add.ToolTip = "Aggiungi";
                            }
                            else
                            {
                                ((Label)e.Row.FindControl("lblCodiceGestioneRetribQuotaFondo_item")).Text = GetValueFromId(((DatiRetributiviQuotaFondoINPGILocal)(e.Row.DataItem)).Gestione);
                                ((Label)e.Row.FindControl("lblCodiceGestioneRetribQuotaFondo_item")).Attributes.Add("title", ((Label)e.Row.FindControl("lblCodiceGestioneRetribQuotaFondo_item")).Text);
                                ((Label)e.Row.FindControl("lblIdCodeGestione")).Text = ((DatiRetributiviQuotaFondoINPGILocal)(e.Row.DataItem)).Gestione;
                                ((Label)e.Row.FindControl("lblSettimane")).Text = ((DatiRetributiviQuotaFondoINPGILocal)(e.Row.DataItem)).Settimane;
                                ((Label)e.Row.FindControl("lblPeriodoRetr")).Text = GetValueFromIdGestione(((DatiRetributiviQuotaFondoINPGILocal)(e.Row.DataItem)).Gestione);
                                ((Label)e.Row.FindControl("lblImportoCalcolato")).Text = !string.IsNullOrEmpty(((DatiRetributiviQuotaFondoINPGILocal)(e.Row.DataItem)).ImportoCalcolato) ? CodeUtility.ConvertDecimalFixedPoint(((DatiRetributiviQuotaFondoINPGILocal)(e.Row.DataItem)).ImportoCalcolato, 4) : "";
                                ((Label)e.Row.FindControl("lblImportoComma707")).Text = !string.IsNullOrEmpty(((DatiRetributiviQuotaFondoINPGILocal)(e.Row.DataItem)).ImportoComma707) ? CodeUtility.ConvertDecimalFixedPoint(((DatiRetributiviQuotaFondoINPGILocal)(e.Row.DataItem)).ImportoComma707, 4) : "";
                                ((Label)e.Row.FindControl("lblSettimaneComma707")).Text = ((DatiRetributiviQuotaFondoINPGILocal)(e.Row.DataItem)).SettimaneComma707;
                                ((Label)e.Row.FindControl("lblRetribuzioneMediaSettimanale")).Text = !string.IsNullOrEmpty(((DatiRetributiviQuotaFondoINPGILocal)(e.Row.DataItem)).RetribuzioneMediaSettimanale) ? CodeUtility.ConvertDecimalFixedPoint(((DatiRetributiviQuotaFondoINPGILocal)(e.Row.DataItem)).RetribuzioneMediaSettimanale, 4) : "";
                                CodeUtility.EnableReadableMode(e.Row.Cells[0], e.Row.Cells[5], Page.Theme, "btnDeleteQuotaRetribFondoINPGI");

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
                throw new INPS.DNA.DnaApplicationException("UCDatiContributiviAgo, Errore nel metodo gvRetributiviINPGI_RowDataBound " + ex);
            }
        }

        private void LoadDecodificaData(IDatiContributiviAgo areaDatiContributivi)
        {
            if (areaDatiContributivi.areaDatiContributiviAgo != null)
            {
                ViewState["listaCodeGestioneQuotaFondoINPGI"] = areaDatiContributivi.areaDatiContributiviAgo.listaDecodificaGestioneQuotaFondoINPGI;
            }
        }

        private bool IsListaEmpty(bool IsRetr)
        {
            if (IsRetr)
            {
                List<DatiRetributiviQuotaFondoINPGILocal> listaDatiRetrApp = (List<DatiRetributiviQuotaFondoINPGILocal>)ViewState[EnumViewState.ElencoDatiRetributiviQuotaFondoINPGI.ToString()];
                if (listaDatiRetrApp == null || (listaDatiRetrApp.Count == 1 &&
                    listaDatiRetrApp[0].Gestione == string.Empty && listaDatiRetrApp[0].Quota == string.Empty && listaDatiRetrApp[0].Settimane == string.Empty &&
                    listaDatiRetrApp[0].ImportoCalcolato == string.Empty && listaDatiRetrApp[0].ImportoComma707 == string.Empty && listaDatiRetrApp[0].SettimaneComma707 == string.Empty))
                    return true;
                else
                    return false;
            }
            else
            {
                List<DatiContributiviQuotaFondoINPGILocal> listaDatiContrApp = (List<DatiContributiviQuotaFondoINPGILocal>)ViewState[EnumViewState.ElencoDatiContributiviQuotaFondoINPGI.ToString()];
                if (listaDatiContrApp == null || (listaDatiContrApp.Count == 1 && listaDatiContrApp[0].Gestione == string.Empty && listaDatiContrApp[0].Montante == string.Empty &&
                    listaDatiContrApp[0].Quota == string.Empty && listaDatiContrApp[0].Settimane == string.Empty))
                    return true;
                else
                    return false;
            }
        }

        private string GetValueFromId(string id)
        {
            string ret = string.Empty;
            if (!String.IsNullOrEmpty(id))
            {
                long index = Convert.ToInt64(id);
                DecodificaGestioneQuotaFondoINPGI[] listaCodeGestioneQuotaFondoINPGI = (DecodificaGestioneQuotaFondoINPGI[])ViewState["listaCodeGestioneQuotaFondoINPGI"];
                DecodificaGestioneQuotaFondoINPGI app = listaCodeGestioneQuotaFondoINPGI.ToList().Find(delegate(DecodificaGestioneQuotaFondoINPGI code) { return (code.Id == index); });
                if (app != null)
                    ret = app.Descrizione;
            }
            return ret;
        }

        private string GetValueFromIdGestione(string idGestione)
        {
            string periodo = string.Empty;
            DecodificaGestioneQuotaFondoINPGI[] listaCodeGestioneCalcolo = (DecodificaGestioneQuotaFondoINPGI[])ViewState["listaCodeGestioneQuotaFondoINPGI"];
            if (!string.IsNullOrEmpty(idGestione))
            {
                DecodificaGestioneQuotaFondoINPGI gestione = listaCodeGestioneCalcolo.Where(x => x.Id == long.Parse(idGestione)).Select(x => x).FirstOrDefault();
                if (gestione != null)
                {
                    if (!gestione.PeriodoDal.HasValue && gestione.PeriodoAl.HasValue)
                        periodo = "Fino a " + gestione.PeriodoAl.Value.ToString("MM/yyyy");
                    else if (gestione.PeriodoDal.HasValue && !gestione.PeriodoAl.HasValue)
                        periodo = "Da " + gestione.PeriodoDal.Value.ToString("MM/yyyy") + " in poi";
                    else if (gestione.PeriodoDal.HasValue && gestione.PeriodoAl.HasValue)
                        periodo = "Da " + gestione.PeriodoDal.Value.ToString("MM/yyyy") + " a " + gestione.PeriodoAl.Value.ToString("MM/yyyy");
                }
            }
            return periodo;
        }

        private void ValorizzaPeriodiPerDecodificaGestioneRetrib()
        {
            DecodificaGestioneQuotaFondoINPGI[] listaCodeGestioneCalcolo = (DecodificaGestioneQuotaFondoINPGI[])ViewState["listaCodeGestioneQuotaFondoINPGI"];
            IEnumerable<DecodificaGestioneQuotaFondoINPGI> listaGestioneRetrib = listaCodeGestioneCalcolo.Select(x => x).Where(x => x.TipoQuota == "R").ToList();

            //ENG - Aggiornamento Memo INPGI - Necessario per risolvere la problematica relativa all'inserimento di una nuova quota sulla tabella di decodifica. I periodi devono seguire lo stesso
            //ordinamento delle quote
            if (ViewState["AbilitazioneModificheMemoINPGI_20240307"] != null && ViewState["AbilitazioneModificheMemoINPGI_20240307"].ToString().Trim().ToUpperInvariant() == "SI")
            {
                listaGestioneRetrib = listaGestioneRetrib.OrderBy(x => x.Descrizione);
            }

            string periodo = string.Empty;
            foreach (DecodificaGestioneQuotaFondoINPGI gestione in listaGestioneRetrib)
            {
                if (!gestione.PeriodoDal.HasValue && gestione.PeriodoAl.HasValue)
                    periodo = periodo + "Fino a " + gestione.PeriodoAl.Value.ToString("MM/yyyy");
                else if (gestione.PeriodoDal.HasValue && !gestione.PeriodoAl.HasValue)
                    periodo = periodo + "Da " + gestione.PeriodoDal.Value.ToString("MM/yyyy") + " in poi";
                else
                    periodo = periodo + "Da " + gestione.PeriodoDal.Value.ToString("MM/yyyy") + " a " + gestione.PeriodoAl.Value.ToString("MM/yyyy");
                periodo = periodo + ";";
            }
            hdnPeriodiRetrib.Value = periodo;
        }

        private void ValorizzaPeriodiPerDecodificaGestioneContrib()
        {
            DecodificaGestioneQuotaFondoINPGI[] listaCodeGestioneCalcolo = (DecodificaGestioneQuotaFondoINPGI[])ViewState["listaCodeGestioneQuotaFondoINPGI"];
            IEnumerable<DecodificaGestioneQuotaFondoINPGI> listaGestioneContrib = listaCodeGestioneCalcolo.Select(x => x).Where(x => x.TipoQuota == "C").ToList();
            string periodo = string.Empty;
            foreach (DecodificaGestioneQuotaFondoINPGI gestione in listaGestioneContrib)
            {
                if (!gestione.PeriodoDal.HasValue && gestione.PeriodoAl.HasValue)
                    periodo = periodo + "Fino a " + gestione.PeriodoAl.Value.ToString("MM/yyyy");
                else if (gestione.PeriodoDal.HasValue && !gestione.PeriodoAl.HasValue)
                    periodo = periodo + "Da " + gestione.PeriodoDal.Value.ToString("MM/yyyy") + " in poi";
                else if (gestione.PeriodoDal.HasValue && gestione.PeriodoAl.HasValue)
                    periodo = periodo + "Da " + gestione.PeriodoDal.Value.ToString("MM/yyyy") + " a " + gestione.PeriodoAl.Value.ToString("MM/yyyy");
                periodo = periodo + ";";
            }
            hdnPeriodiContrib.Value = periodo;
        }

        protected void gvRetributiviINPGI_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {

        }

        protected void gvContributiviINPGI_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {

        }

        protected void gvRetributiviINPGI_Load(object sender, EventArgs e)
        {
            AreaDatiContributivi areaDatiContributiviAgo = ViewState["areaDatiContributiviAgo"] as AreaDatiContributivi;
            if (areaDatiContributiviAgo != null)
            {
                gvRetributiviINPGI.Columns[(int)ColonneGvDatiRetributiviINPGI.Sett707].Visible = areaDatiContributiviAgo.IsSettimane707INPGIVisible;
                gvRetributiviINPGI.Columns[(int)ColonneGvDatiRetributiviINPGI.Importo707].Visible = areaDatiContributiviAgo.IsSettimane707INPGIVisible;
            }
        }


        #region EventHandler

        public event EventHandler ShowAvviso;
        public event EventHandler HideAvviso;
        public event EventHandler GestisciTastoSalva;
        public event EventHandler InitializeData;

        protected void RaiseShowAvviso(object sender, EventArgs e)
        {
            ShowAvviso(sender, e);
        }

        protected void RaiseHideAvviso(object sender, EventArgs e)
        {
            if (HideAvviso != null)
                HideAvviso(sender, e);
        }

        protected void RaiseGestisciTastoSalva(object sender, EventArgs e)
        {
            if (GestisciTastoSalva != null)
                GestisciTastoSalva(sender, e);
        }

        protected void RaiseInitializeData(object sender, EventArgs e)
        {
            InitializeData(sender, e);
        }

        #endregion EventHandler

        #region Enums
        public enum EnumViewState
        {
            ElencoDatiContributiviQuotaFondoINPGI,
            ElencoDatiRetributiviQuotaFondoINPGI
        }
        #endregion Enums
    }

    #region nested Class
    [Serializable]
    public class DatiContributiviQuotaFondoINPGILocal
    {
        public DatiContributiviQuotaFondoINPGILocal()
        {
            this.Id = Guid.NewGuid();
        }
        public DatiContributiviQuotaFondoINPGILocal(string strGestione, string strMontante, string strQuota, string strSettimane)
        {
            this.Id = Guid.NewGuid();
            this._strMontante = strMontante;
            this._strGestione = strGestione;
            this._strQuota = strQuota;
            this._strSettimane = strSettimane;
        }
        #region private properties
        private string _strMontante;
        private string _strGestione;
        private string _strQuota;
        private string _strSettimane;
        #endregion private properties

        #region public properties
        public Guid Id { get; set; }
        public string Montante { get { return _strMontante; } set { _strMontante = value; } }
        public string Gestione { get { return _strGestione; } set { _strGestione = value; } }
        public string Quota { get { return _strQuota; } set { _strQuota = value; } }
        public string Settimane { get { return _strSettimane; } set { _strSettimane = value; } }
        #endregion public properties

    }

    [Serializable]
    public class DatiRetributiviQuotaFondoINPGILocal
    {
        public DatiRetributiviQuotaFondoINPGILocal()
        {
            this.Id = Guid.NewGuid();
        }
        public DatiRetributiviQuotaFondoINPGILocal(string strGestione, string strSettimane, string strQuota, string strImportoCalcolato, string strImportoComma707, string strSettimaneComma707, string strRetribuzioneMediaSettimanale)
        {
            this.Id = Guid.NewGuid();
            this._strSettimane = strSettimane;
            this._strGestione = strGestione;
            this._strQuota = strQuota;
            this._strImportoCalcolato = strImportoCalcolato;
            this._strImportoComma707 = strImportoComma707;
            this._strSettimaneComma707 = strSettimaneComma707;
            this._strRetribuzioneMediaSettimanale = strRetribuzioneMediaSettimanale;
        }
        #region private properties
        private string _strSettimane;
        private string _strGestione;
        private string _strQuota;
        private string _strImportoCalcolato;
        private string _strImportoComma707;
        private string _strSettimaneComma707;
        private string _strRetribuzioneMediaSettimanale;
        #endregion private properties

        #region public properties
        public Guid Id { get; set; }
        public string Settimane { get { return _strSettimane; } set { _strSettimane = value; } }
        public string Gestione { get { return _strGestione; } set { _strGestione = value; } }
        public string Quota { get { return _strQuota; } set { _strQuota = value; } }
        public string ImportoCalcolato { get { return _strImportoCalcolato; } set { _strImportoCalcolato = value; } }
        public string ImportoComma707 { get { return _strImportoComma707; } set { _strImportoComma707 = value; } }
        public string SettimaneComma707 { get { return _strSettimaneComma707; } set { _strSettimaneComma707 = value; } }
        public string RetribuzioneMediaSettimanale { get { return _strRetribuzioneMediaSettimanale; } set { _strRetribuzioneMediaSettimanale = value; } }
        #endregion public properties
    }
    #endregion nested Class
}