using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneAgo;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using INPS.Pensioni.LiquidazionePensione.Presenter;
using INPS.DNA;
using System.ComponentModel;

namespace INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DatiContributiviAgo
{
    public partial class UCDatiCalcoloINPDAI : CustomBaseUserControl, IDatiContributiviAgo, ITitolarePensione
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

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        #region public methods
        public void ValorizzaEtichette(IDatiContributiviAgo dati)
        {
            if (this.domanda == null)
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
            ViewState[EnumViewState.AreaDatiContributiviAgo.ToString()] = dati.areaDatiContributiviAgo;
            ViewState[EnumViewState.IsPrimoRecordRetrGestioneS.ToString()] = dati.areaDatiContributiviAgo.DatiExINPDAI.IsPrimoRecordRetrGestioneS;
            ViewState[EnumViewState.DecorrenzaCalcoloRetr.ToString()] = dati.areaDatiContributiviAgo.DatiExINPDAI.DecorrenzaCalcoloRetr;

            AreaTitolare.DatiPensione datiPensione = new AreaTitolare.DatiPensione();
            datiPensione = GetDatiPensione(this);

            LoadDecodificaData(dati);
            BindDataForPanels(dati.areaDatiContributiviAgo);

            RenderControls(dati.areaDatiContributiviAgo);

            if (dati.areaDatiContributiviAgo != null && dati.areaDatiContributiviAgo.DatiExINPDAI != null)
            {
                if (dati.areaDatiContributiviAgo.DatiExINPDAI.AnzAl95.HasValue)
                    txtAnzAl95.Text = dati.areaDatiContributiviAgo.DatiExINPDAI.AnzAl95.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture);
                else if (areaDatiContributiviAgo.DatiCalcolo.IsUnicarpe)
                    txtAnzAl95.Text = (0.9999M).ToString(System.Globalization.CultureInfo.CurrentUICulture);

                if (dati.areaDatiContributiviAgo.DatiExINPDAI.QuotaAl95.HasValue)
                    txtQuotaAl95.Text = dati.areaDatiContributiviAgo.DatiExINPDAI.QuotaAl95.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture);
                else if (areaDatiContributiviAgo.DatiCalcolo.IsUnicarpe)
                    txtQuotaAl95.Text = (0.9999M).ToString(System.Globalization.CultureInfo.CurrentUICulture);

                if (!(Utility.IsDomandaDAI(this.domanda.Categoria) && Utility.IsDomandaRipristino(datiPensione) && Utility.DataStrettamenteSuccessivaA(new DateTime(1997, 01, 01), datiPensione.DecorrenzaOriginaria.GetValueOrDefault())))
                {
                    ImportoAl200312.Visible = false;
                }
                else
                {
                    ImportoAl200312.Visible = true;
                    txtImportoAl200312.Enabled = false;
                    if (dati.areaDatiContributiviAgo.DatiExINPDAI.ImportoAl200312.HasValue)
                        txtImportoAl200312.Text = dati.areaDatiContributiviAgo.DatiExINPDAI.ImportoAl200312.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture);
                    else
                        txtImportoAl200312.Text = "0";

                }
                /// tipo calcolo vincente INPDAI
                if (dati.areaDatiContributiviAgo.TipoCalcoloVincenteUnicarpe.HasValue)
                {
                    if (dati.areaDatiContributiviAgo.ListaTipoCalcoloVincenteDAI != null && dati.areaDatiContributiviAgo.ListaTipoCalcoloVincenteDAI.Count() > 0)
                    {
                        TipoCalcoloVincenteDAI tipoCalcoloVincente = dati.areaDatiContributiviAgo.ListaTipoCalcoloVincenteDAI.FirstOrDefault(x => x.Id == dati.areaDatiContributiviAgo.TipoCalcoloVincenteUnicarpe.Value);
                        if (tipoCalcoloVincente != null)
                            labelTipoCalcoloVincente.Text = tipoCalcoloVincente.Id + " - " + tipoCalcoloVincente.Descrizione;
                        else
                            labelTipoCalcoloVincente.Text = "Dato non disponibile";
                    }
                }
                else
                    labelTipoCalcoloVincente.Text = "Dato non disponibile";

                if (disabilitaPannelloDatiCalcoloRicSdai())
                {
                    txtAnzAl95.Enabled = false;
                    txtQuotaAl95.Enabled = false;
                }
                else
                {
                    txtAnzAl95.Enabled = !dati.areaDatiContributiviAgo.DatiExINPDAI.IsDataAnzianitaAl95Bloccato;
                    txtQuotaAl95.Enabled = !dati.areaDatiContributiviAgo.DatiExINPDAI.IsDataAnzianitaAl95Bloccato;
                }

                if (Utility.IsDomandaRipristino(datiPensione))
                {
                    gvDatiRetributivi.Enabled = false;
                    gvDatiContributivi.Enabled = false;
                    btnEliminaDatiCalcolo.Enabled = false;
                }
 
            }

            if (CodeUtility.IsRicostituzione(datiPensione) && !CodeUtility.IsRicostituzioneContributiva(datiPensione))
            {
                pnlContributoSolidarieta.Enabled = false;
                panelTipoCalcoloVincente.Enabled = false;
                lblRicNonContrib.Visible = true;
                btnEliminaDatiCalcolo.Enabled = false;
            }
        }

        private bool disabilitaPannelloDatiCalcoloRicSdai()
        {
            AreaTitolare.DatiPensione datiPensione = null;
            datiPensione = GetDatiPensione(this);

            if (this.domanda == null)
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            AreaDatiContributivi areaContributiviAgo = (AreaDatiContributivi)ViewState[EnumViewState.AreaDatiContributiviAgo.ToString()];

            if (datiPensione != null && domanda != null && !String.IsNullOrEmpty(domanda.Categoria) && domanda.Categoria.Trim().ToUpperInvariant() == "SDAI" &&
                   CodeUtility.IsRicostituzione(datiPensione))
            {
                GestioneAggiornamentoPECODatiContributivi[] listaDatiContributivi = areaContributiviAgo.DatiCalcolo.lDatiContributivi;
                DecodificaGestioneCalcoloContributivo[] listaDecodificaCalcoloContributivo = areaContributiviAgo.listaDecodificaGestioneCalcoloContributivo;

                if (listaDatiContributivi != null && listaDatiContributivi.Count() > 0 && listaDecodificaCalcoloContributivo != null && listaDecodificaCalcoloContributivo.Count() > 0)
                {
                    DecodificaGestioneCalcoloContributivo gestioneK = listaDecodificaCalcoloContributivo.ToList().Find(x => x.TraduzioneSuGP.Trim().ToUpperInvariant() == "K");
                    if (gestioneK != null)
                    {

                        if (listaDatiContributivi.ToList().Exists(x => x.CodGestione == gestioneK.Id))
                        {
                            return true;
                        }
                    }
                }
            }
            if (Utility.IsDomandaRipristino(datiPensione))
            {
                return true;
            }

            return false;
        }

        public void btnSalvaDatiCalcolo_Click(object sender, EventArgs e)
        {
            this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
            this.areaDatiContributiviAgo = new AreaDatiContributivi();

            AreaTitolare.DatiPensione datiPensione = new AreaTitolare.DatiPensione();
            datiPensione = GetDatiPensione(this);

            RecuperaCampi(this.areaDatiContributiviAgo);

            if ((this.areaDatiContributiviAgo.DatiCalcolo.lDatiRetributivi != null && this.areaDatiContributiviAgo.DatiCalcolo.lDatiRetributivi.Count() > 0) ||
                (this.areaDatiContributiviAgo.DatiCalcolo.lDatiContributivi != null && this.areaDatiContributiviAgo.DatiCalcolo.lDatiContributivi.Count() > 0))
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


                if (CodeUtility.IsRicostituzione(datiPensione) && !CodeUtility.IsRicostituzioneContributiva(datiPensione))
                    btnEliminaDatiCalcolo.Enabled = false;
                else
                    btnEliminaDatiCalcolo.Enabled = true;

                ViewState[EnumViewState.AreaDatiContributiviAgo.ToString()] = this.areaDatiContributiviAgo;
                ReLoadData(this.areaDatiContributiviAgo.DatiCalcolo.lDatiRetributivi != null ?
                    MapDatiRetributiviForView(this.areaDatiContributiviAgo) : null,
                    this.areaDatiContributiviAgo.DatiCalcolo.lDatiContributivi != null ?
                    MapDatiContributiviForView(this.areaDatiContributiviAgo) : null);

            }
            else
            {
                this.HasError = true;
                this.ErrorMessage = "Non ci sono Dati Calcolo da salvare";
            }


            if (this.HasError)
                RaiseShowAvviso(this, null);
            else
            {

                this.ErrorMessage = "Dati Calcolo salvati correttamente.";
                RaiseShowAvviso(this, null);
            }
        }

        internal void RecuperaCampi(AreaDatiContributivi areaDatiContributiviAgo)
        {
            List<DatiRetributiviLocal> listaDatiRetribApp = (List<DatiRetributiviLocal>)ViewState[EnumViewState.ElencoDatiRetributivi.ToString()];
            List<DatiContributiviLocal> listaDatiContribApp = (List<DatiContributiviLocal>)ViewState[EnumViewState.ElencoDatiContributivi.ToString()];

            if ((listaDatiRetribApp != null && listaDatiRetribApp.Count() > 0) || (listaDatiContribApp != null && listaDatiContribApp.Count() > 0))
            {
                areaDatiContributiviAgo.DatiCalcolo = new GestioneContribDatiCalcolo();

                areaDatiContributiviAgo.DatiCalcolo.TipoCalcolo = ((AreaDatiContributivi)ViewState[EnumViewState.AreaDatiContributiviAgo.ToString()]).DatiCalcolo.TipoCalcolo;
                areaDatiContributiviAgo.DatiCalcolo.IdPensione = ((AreaDatiContributivi)ViewState[EnumViewState.AreaDatiContributiviAgo.ToString()]).DatiCalcolo.IdPensione;
                areaDatiContributiviAgo.DatiCalcolo.IsUnicarpe = ((AreaDatiContributivi)ViewState[EnumViewState.AreaDatiContributiviAgo.ToString()]).DatiCalcolo.IsUnicarpe;
                areaDatiContributiviAgo.IsFineAssicurazionePost2012 = ((AreaDatiContributivi)ViewState[EnumViewState.AreaDatiContributiviAgo.ToString()]).IsFineAssicurazionePost2012;
                areaDatiContributiviAgo.IsSettimane707Visible = ((AreaDatiContributivi)ViewState[EnumViewState.AreaDatiContributiviAgo.ToString()]).IsSettimane707Visible;
                areaDatiContributiviAgo.DatiCalcolo.PL_Coeftrasf = ((AreaDatiContributivi)ViewState[EnumViewState.AreaDatiContributiviAgo.ToString()]).DatiCalcolo.PL_Coeftrasf;
                areaDatiContributiviAgo.IsDatiContributiviVittimeVisible = ((AreaDatiContributivi)ViewState[EnumViewState.AreaDatiContributiviAgo.ToString()]).IsDatiContributiviVittimeVisible;
                areaDatiContributiviAgo.IsDatiRetributiviVittimeVisible = ((AreaDatiContributivi)ViewState[EnumViewState.AreaDatiContributiviAgo.ToString()]).IsDatiRetributiviVittimeVisible;

                GetDatiINPDAI(areaDatiContributiviAgo);

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
            }
        }

        public void btnEliminaDatiCalcolo_Click(object sender, EventArgs e)
        {
            this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            List<DatiRetributiviLocal> listaDatiRetribApp = (List<DatiRetributiviLocal>)ViewState[EnumViewState.ElencoDatiRetributivi.ToString()];
            List<DatiContributiviLocal> listaDatiContribApp = (List<DatiContributiviLocal>)ViewState[EnumViewState.ElencoDatiContributivi.ToString()];

            if ((listaDatiRetribApp != null && listaDatiRetribApp.Count() > 0) || (listaDatiContribApp != null && listaDatiContribApp.Count() > 0))
            {
                PresenterDatiContributiviAGO presenterDatiContributiviAgo = new PresenterDatiContributiviAGO();
                presenterDatiContributiviAgo.EliminaDatiCalcolo(this);

                if (!this.HasError)
                {
                    if (listaDatiRetribApp != null && listaDatiRetribApp.Count() > 0)
                        modalitaEditRetributivi.Value = "false";

                    if ((listaDatiContribApp != null && listaDatiContribApp.Count() > 0))
                        modalitaEditContributivi.Value = "false";

                    txtAnzAl95.Text = null;
                    txtQuotaAl95.Text = null;

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

        internal void DisabilitaPulsanti()
        {
            btnSalvaDatiCalcolo.Enabled = false;
            btnEliminaDatiCalcolo.Enabled = false;
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
            List<GestioneAggiornamentoPECODatiContributivi> lstCont = null;
            List<DatiContributiviLocal> listaDatiContribApp = (List<DatiContributiviLocal>)ViewState[EnumViewState.ElencoDatiContributivi.ToString()];
            if (listaDatiContribApp != null && listaDatiContribApp.Count > 0)
                lstCont = GetDataContributiviToSave(listaDatiContribApp);
            return lstCont;
        }
        #endregion public methods

        #region private methods
        private void LoadDecodificaData(IDatiContributiviAgo areaDatiContributivi)
        {
            ViewState[EnumViewState.ListaCodeGestioneCalcoloRetrib.ToString()] = areaDatiContributivi.areaDatiContributiviAgo.listaDecodificaGestioneCalcoloRetributivo.ToList();
            ViewState[EnumViewState.ListaCodeGestioneCalcoloContrib.ToString()] = areaDatiContributivi.areaDatiContributiviAgo.listaDecodificaGestioneCalcoloContributivo.ToList();
            if (areaDatiContributivi.areaDatiContributiviAgo.DatiExINPDAI != null)
            {
                ViewState[EnumViewState.DecodificaTipoQuota.ToString()] = areaDatiContributivi.areaDatiContributiviAgo.DatiExINPDAI.DecodificaTipoQuota.ToList();
                ViewState[EnumViewState.CtrlDecorrenzaRetrExINPDAI.ToString()] = areaDatiContributivi.areaDatiContributiviAgo.DatiExINPDAI.CtrlDecorrenzaRetrExINPDAI.ToList();
            }
        }

        private void BindDataForPanels(AreaDatiContributivi areaDatiContributivi)
        {
            AreaTitolare.DatiPensione datiPensione = null;
            datiPensione = GetDatiPensione(this);

            CodeUtility.TipologiaPensioneGruppo tipologiaGruppoPensione = CodeUtility.TipologiaPensioneGruppo.gr_NessunValore;
            CodeUtility.TipologiaPensioneProdotto tipologiaProdottoPensione = CodeUtility.TipologiaPensioneProdotto.pr_NessunValore;
            CodeUtility.TipologiaPensioneTipo tipologiaTipoPensione = CodeUtility.TipologiaPensioneTipo.tp_NessunValore;
            CodeUtility.GetTipologiaPensione(datiPensione.CodeGruppo, datiPensione.CodeProdotto, datiPensione.CodeTipo, out tipologiaGruppoPensione, out tipologiaProdottoPensione, out tipologiaTipoPensione);

            if (areaDatiContributivi.DatiCalcolo.IsUnicarpe)
            {
                ///pannello tipo calcolo vincente visibile se domanda unicarpe
                panelTipoCalcoloVincente.Visible = true;

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
            else
            {
                switch (areaDatiContributivi.DatiCalcolo.TipoCalcolo)
                {
                    case GestioneContribTipoCalcolo.Contributivo:
                        pdivRetributivo.Visible = false;
                        pdivContributivo.Visible = true;
                        pnlContributoSolidarieta.Visible = false;
                        InitBindDataContributivi();
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
        }

        private void InitBindDataContributivi()
        {
            List<DatiContributiviLocal> elencoDatiContributivi = new List<DatiContributiviLocal>();
            AreaTitolare.DatiPensione datiPensione = null;
            datiPensione = GetDatiPensione(this);

            if (((AreaDatiContributivi)ViewState[EnumViewState.AreaDatiContributiviAgo.ToString()]).DatiCalcolo.lDatiContributivi != null)
                elencoDatiContributivi = MapDatiContributiviForView((AreaDatiContributivi)ViewState[EnumViewState.AreaDatiContributiviAgo.ToString()]);

            DatiContributiviLocal Empty = elencoDatiContributivi.Find(delegate (DatiContributiviLocal code)
            {
                return (code.AmmontareContributivo == string.Empty && code.Gestione == string.Empty && code.Quota == string.Empty &&
                        code.MontanteContributivo == string.Empty && code.Settimane == string.Empty);
            });

            if (Empty == null && !(((AreaDatiContributivi)ViewState[EnumViewState.AreaDatiContributiviAgo.ToString()]).DatiCalcolo.IsUnicarpe)
                && !(CodeUtility.IsRicostituzione(datiPensione) && !CodeUtility.IsRicostituzioneContributiva(datiPensione)))
                elencoDatiContributivi.Add(new DatiContributiviLocal(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty));

            gvDatiContributivi.DataSource = elencoDatiContributivi;
            ViewState[EnumViewState.ElencoDatiContributivi.ToString()] = elencoDatiContributivi;

            if (this.domanda == null)
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            if (CodeUtility.IsRicostituzione(datiPensione) && Utility.IsDomandaDAI(domanda.Categoria) &&
                        domanda.Categoria.StartsWith("S") && datiPensione.NaturaPensione.Substring(2, 1) == "T")
                gvDatiContributivi.Columns[4].HeaderText = "Importo";
            gvDatiContributivi.DataBind();
        }

        private void InitBindDataRetributivi()
        {
            List<DatiRetributiviLocal> elencoDatiRetributivi = new List<DatiRetributiviLocal>();

            if (((AreaDatiContributivi)ViewState[EnumViewState.AreaDatiContributiviAgo.ToString()]).DatiCalcolo.lDatiRetributivi != null)
                elencoDatiRetributivi = MapDatiRetributiviForView((AreaDatiContributivi)ViewState[EnumViewState.AreaDatiContributiviAgo.ToString()]);

            PrevalorizzaRecordRetributivo(elencoDatiRetributivi);

            gvDatiRetributivi.DataSource = elencoDatiRetributivi;
            ViewState[EnumViewState.ElencoDatiRetributivi.ToString()] = elencoDatiRetributivi;
            gvDatiRetributivi.DataBind();
        }

        private List<DatiContributiviLocal> MapDatiContributiviForView(AreaDatiContributivi areaDatiContributivi)
        {
            List<DatiContributiviLocal> elencoDatiContributivi = new List<DatiContributiviLocal>();
            DecodificaGestioneCalcoloContributivo decGestioneK = null;
            if (ViewState[EnumViewState.ListaCodeGestioneCalcoloContrib.ToString()] != null && ((List<DecodificaGestioneCalcoloContributivo>)ViewState[EnumViewState.ListaCodeGestioneCalcoloContrib.ToString()]).Exists(x => x.TraduzioneSuGP.Trim() == "K"))
                decGestioneK = ((List<DecodificaGestioneCalcoloContributivo>)ViewState[EnumViewState.ListaCodeGestioneCalcoloContrib.ToString()]).FirstOrDefault(x => x.TraduzioneSuGP.Trim() == "K");
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
                else if (decGestioneK != null && contr.CodGestione == decGestioneK.Id)
                {
                    settimana = contr.Settimane.HasValue ? contr.Settimane.Value.ToString() : string.Empty;
                    importo = contr.ImportoContributivo.HasValue ? contr.ImportoContributivo.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;
                    montante = contr.MontanteContributivo.HasValue ? contr.MontanteContributivo.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;
                }
                PL_Quotac = contr.PL_Quotac.HasValue ? contr.PL_Quotac.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;
                elencoDatiContributivi.Add(new DatiContributiviLocal(contr.CodGestione.HasValue ? contr.CodGestione.Value.ToString() : string.Empty,
                    contr.Quota.HasValue ? contr.Quota.Value.ToString() : string.Empty,
                    settimana, importo, montante, PL_Quotac));
            }
            return elencoDatiContributivi;
        }

        private List<DatiRetributiviLocal> MapDatiRetributiviForView(AreaDatiContributivi areaDatiContributivi)
        {
            List<DatiRetributiviLocal> elencoDatiRetributivi = new List<DatiRetributiviLocal>();
            foreach (GestioneAggiornamentoPECODatiRetributivi retr in areaDatiContributivi.DatiCalcolo.lDatiRetributivi.ToList<GestioneAggiornamentoPECODatiRetributivi>())
            {
                string settimana = string.Empty;
                string rmsQuota = string.Empty;
                string codiceGestione = retr.CodGestione.HasValue ? retr.CodGestione.Value.ToString() : string.Empty;
                string quota = retr.Quota.HasValue ? retr.Quota.Value.ToString() : string.Empty;
                string tipoQuota = !string.IsNullOrEmpty(retr.CodiceTipoQuota) ? retr.CodiceTipoQuota : null;
                string decorrenza = GetDecorrenzaRetributiva(codiceGestione, quota, tipoQuota);
                string PL_Quotar = string.Empty;
                string PL_Quotar707 = string.Empty;
                if (!string.IsNullOrEmpty(quota))
                {
                    if (quota.ToUpperInvariant() == "A")
                    {
                        settimana = retr.SettimaneA.HasValue ? retr.SettimaneA.Value.ToString() : string.Empty;
                        rmsQuota = retr.RMSQuotaA.HasValue ? retr.RMSQuotaA.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;
                    }
                    else if (quota.ToUpperInvariant() == "B")
                    {
                        settimana = retr.SettimaneB.HasValue ? retr.SettimaneB.Value.ToString() : string.Empty;
                        rmsQuota = retr.RMSQuotaB.HasValue ? retr.RMSQuotaB.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;
                    }
                }
                string strSett707 = retr.NSettimane707.HasValue ? retr.NSettimane707.ToString() : string.Empty;
                PL_Quotar = retr.PL_Quotar.HasValue ? retr.PL_Quotar.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;
                PL_Quotar707 = retr.PL_Quotar707.HasValue ? retr.PL_Quotar707.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;
                elencoDatiRetributivi.Add(new DatiRetributiviLocal(codiceGestione, quota, tipoQuota, settimana, decorrenza, rmsQuota, strSett707, PL_Quotar, PL_Quotar707));
            }
            return elencoDatiRetributivi;
        }

        private string GetDecorrenzaRetributiva(string codiceGestione, string quota, string tipoQuota)
        {
            string decorrenza = string.Empty;
            string codiceGestioneTraduzioneSuGP = string.Empty;

            if (string.IsNullOrEmpty(tipoQuota))
                tipoQuota = null;

            List<DecodificaGestioneCalcoloRetributivo> listaCodeGestioneCalcoloRetrib = (List<DecodificaGestioneCalcoloRetributivo>)ViewState[EnumViewState.ListaCodeGestioneCalcoloRetrib.ToString()];
            DecodificaGestioneCalcoloRetributivo app = listaCodeGestioneCalcoloRetrib.Find(delegate (DecodificaGestioneCalcoloRetributivo code) { return (code.Id.ToString() == codiceGestione); });
            if (app != null)
                codiceGestioneTraduzioneSuGP = app.TraduzioneSuGP.Trim();

            List<CtrlDecorrenzaRetrExINPDAI> listaCtrlDecorrenzaRetrExINPDAI = (List<CtrlDecorrenzaRetrExINPDAI>)ViewState[EnumViewState.CtrlDecorrenzaRetrExINPDAI.ToString()];
            if (listaCtrlDecorrenzaRetrExINPDAI != null && listaCtrlDecorrenzaRetrExINPDAI.Count > 0)
            {
                CtrlDecorrenzaRetrExINPDAI obj = listaCtrlDecorrenzaRetrExINPDAI.Find(x => x.Gestione.Trim() == codiceGestioneTraduzioneSuGP && x.Quota.ToString() == quota && x.TipoQuota == tipoQuota);
                if (obj != null)
                {
                    UtilityDifferenzaDateTime data = (UtilityDifferenzaDateTime)ViewState[EnumViewState.DecorrenzaCalcoloRetr.ToString()];

                    if (data != null)
                    {
                        if (obj.CodiceDecorrenza != 76)
                            decorrenza = obj.CodiceDecorrenza.ToString() + "/" + data.Year;
                        else
                            decorrenza = String.Format("{0:00}/{1:0000}", data.Month, data.Year);
                    }
                }
            }

            return decorrenza;
        }

        private void ManagePulsanti()
        {
            if (this.TitolarePensione == null)
                this.TitolarePensione = new AreaTitolare();
            if (this.TitolarePensione.Pensione == null)
                this.TitolarePensione.Pensione = GetDatiPensione(this);

            if ((this.modalitaEditContributivi.Value == "true" && !IsListaEmpty(false)) || (this.modalitaEditRetributivi.Value == "true" && !IsListaEmpty(true)))
            {
                RaiseGestisciTastoSalva(this, null);
                btnSalvaDatiCalcolo.Enabled = false;
                btnEliminaDatiCalcolo.Enabled = false;
                return;
            }

            if (areaDatiContributiviAgo == null)
                areaDatiContributiviAgo = (AreaDatiContributivi)ViewState[EnumViewState.AreaDatiContributiviAgo.ToString()];

            if (areaDatiContributiviAgo != null && areaDatiContributiviAgo.DatiCalcolo != null)
            {
                switch (areaDatiContributiviAgo.DatiCalcolo.TipoCalcolo)
                {
                    case GestioneContribTipoCalcolo.Contributivo:
                        if (!IsListaEmpty(false))
                        {
                            RaiseGestisciTastoSalva(this, null);
                            btnSalvaDatiCalcolo.Enabled = true;
                            btnEliminaDatiCalcolo.Enabled = true;
                        }
                        else
                        {
                            RaiseGestisciTastoSalva(this, null);
                            btnSalvaDatiCalcolo.Enabled = false;
                            btnEliminaDatiCalcolo.Enabled = false;
                        }
                        break;
                    case GestioneContribTipoCalcolo.Retributivo:
                        if (areaDatiContributiviAgo.IsFineAssicurazionePost2012)
                        {
                            if (!IsListaEmpty(false) && !IsListaEmpty(true))
                            {
                                RaiseGestisciTastoSalva(this, null);
                                btnSalvaDatiCalcolo.Enabled = true;
                                btnEliminaDatiCalcolo.Enabled = true;
                            }
                            else
                            {
                                RaiseGestisciTastoSalva(this, null);
                                btnSalvaDatiCalcolo.Enabled = false;
                                btnEliminaDatiCalcolo.Enabled = false;
                            }
                        }
                        else
                        {
                            if (!IsListaEmpty(true))
                            {
                                RaiseGestisciTastoSalva(this, null);
                                btnSalvaDatiCalcolo.Enabled = true;
                                btnEliminaDatiCalcolo.Enabled = true;
                            }
                            else
                            {
                                RaiseGestisciTastoSalva(this, null);
                                btnSalvaDatiCalcolo.Enabled = false;
                                btnEliminaDatiCalcolo.Enabled = false;
                            }
                        }
                        break;
                    case GestioneContribTipoCalcolo.Misto:
                        if (!IsListaEmpty(false) && !IsListaEmpty(true))
                        {
                            RaiseGestisciTastoSalva(this, null);
                            btnSalvaDatiCalcolo.Enabled = true;
                            btnEliminaDatiCalcolo.Enabled = true;
                        }
                        else
                        {
                            RaiseGestisciTastoSalva(this, null);
                            btnSalvaDatiCalcolo.Enabled = false;
                            btnEliminaDatiCalcolo.Enabled = false;
                        }
                        break;
                    case GestioneContribTipoCalcolo.NonValido:
                        RaiseGestisciTastoSalva(this, null);
                        btnSalvaDatiCalcolo.Enabled = false;
                        btnEliminaDatiCalcolo.Enabled = false;
                        break;
                }
            }

            if (CodeUtility.IsRicostituzione(this.TitolarePensione.Pensione) && !CodeUtility.IsRicostituzioneContributiva(this.TitolarePensione.Pensione))
                btnEliminaDatiCalcolo.Enabled = false;

        }

        private bool IsListaEmpty(bool IsRetr)
        {
            if (IsRetr)
            {
                List<DatiRetributiviLocal> listaDatiRetrApp = (List<DatiRetributiviLocal>)ViewState[EnumViewState.ElencoDatiRetributivi.ToString()];
                if (listaDatiRetrApp == null || (listaDatiRetrApp.Count == 1 && listaDatiRetrApp[0].Decorrenza == string.Empty &&
                    listaDatiRetrApp[0].Gestione == string.Empty && listaDatiRetrApp[0].Quota == string.Empty &&
                    listaDatiRetrApp[0].RetribuzioneMedia == string.Empty && listaDatiRetrApp[0].Settimane == string.Empty &&
                    listaDatiRetrApp[0].TipoQuota == string.Empty))
                    return true;
                else
                    return false;
            }
            else
            {
                List<DatiContributiviLocal> listaDatiContrApp = (List<DatiContributiviLocal>)ViewState[EnumViewState.ElencoDatiContributivi.ToString()];
                if (listaDatiContrApp == null || (listaDatiContrApp.Count == 1 && listaDatiContrApp[0].AmmontareContributivo == string.Empty &&
                    listaDatiContrApp[0].Gestione == string.Empty && listaDatiContrApp[0].MontanteContributivo == string.Empty &&
                    listaDatiContrApp[0].Settimane == string.Empty))
                    return true;
                else
                    return false;
            }
        }

        private bool IsEmptyReadableRowRetrib(GridViewRow row)
        {
            if ((row.FindControl(Utility.GetDescription(EnumControlli.LblCodiceGestioneRetr)) != null && ((Label)row.FindControl(Utility.GetDescription(EnumControlli.LblCodiceGestioneRetr))).Text != string.Empty) ||
                (row.FindControl(Utility.GetDescription(EnumControlli.LblQuotaRetr)) != null && ((Label)row.FindControl(Utility.GetDescription(EnumControlli.LblQuotaRetr))).Text != string.Empty) ||
                (row.FindControl(Utility.GetDescription(EnumControlli.LblTipoQuotaRetr)) != null && ((Label)row.FindControl(Utility.GetDescription(EnumControlli.LblTipoQuotaRetr))).Text != string.Empty) ||
                (row.FindControl(Utility.GetDescription(EnumControlli.LblDecorrenzaRetr)) != null && ((Label)row.FindControl(Utility.GetDescription(EnumControlli.LblDecorrenzaRetr))).Text != string.Empty) ||
                (row.FindControl(Utility.GetDescription(EnumControlli.LblSettimaneRetr)) != null && ((Label)row.FindControl(Utility.GetDescription(EnumControlli.LblSettimaneRetr))).Text != string.Empty) ||
                (row.FindControl(Utility.GetDescription(EnumControlli.LblRetribuzioneMediaRetr)) != null && ((Label)row.FindControl(Utility.GetDescription(EnumControlli.LblRetribuzioneMediaRetr))).Text != string.Empty))
                return false;
            else
                return true;
        }

        private bool IsEmptyEditableRowRetrib(GridViewRow row)
        {
            if (row.FindControl(Utility.GetDescription(EnumControlli.DdlCodiceGestioneRetr)) != null && ((DropDownList)row.FindControl(Utility.GetDescription(EnumControlli.DdlCodiceGestioneRetr))).SelectedIndex != 0 &&
                row.FindControl(Utility.GetDescription(EnumControlli.DdlQuotaRetr)) != null && ((DropDownList)row.FindControl(Utility.GetDescription(EnumControlli.DdlQuotaRetr))).SelectedIndex != 0 &&
                row.FindControl(Utility.GetDescription(EnumControlli.TxtSettimaneRetr)) != null && ((TextBox)row.FindControl(Utility.GetDescription(EnumControlli.TxtSettimaneRetr))).Text != string.Empty &&
                row.FindControl(Utility.GetDescription(EnumControlli.TxtRetribuzioneMediaRetr)) != null && ((TextBox)row.FindControl(Utility.GetDescription(EnumControlli.TxtRetribuzioneMediaRetr))).Text != string.Empty)
                return false;
            else
                return true;
        }

        private bool IsEmptyReadableRowContr(GridViewRow row)
        {
            if ((row.FindControl(Utility.GetDescription(EnumControlli.LblCodiceGestioneContr)) != null && ((Label)row.FindControl(Utility.GetDescription(EnumControlli.LblCodiceGestioneContr))).Text != string.Empty) ||
                (row.FindControl(Utility.GetDescription(EnumControlli.LblQuotaContr)) != null && ((Label)row.FindControl(Utility.GetDescription(EnumControlli.LblQuotaContr))).Text != string.Empty) ||
                (row.FindControl(Utility.GetDescription(EnumControlli.LblSettimaneContr)) != null && ((Label)row.FindControl(Utility.GetDescription(EnumControlli.LblSettimaneContr))).Text != string.Empty) ||
                (row.FindControl(Utility.GetDescription(EnumControlli.LblAmmontareContr)) != null && ((Label)row.FindControl(Utility.GetDescription(EnumControlli.LblAmmontareContr))).Text != string.Empty) ||
                (row.FindControl(Utility.GetDescription(EnumControlli.LblMontanteContr)) != null && ((Label)row.FindControl(Utility.GetDescription(EnumControlli.LblMontanteContr))).Text != string.Empty))
                return false;
            else
                return true;
        }

        private bool IsEmptyEditableRowContr(GridViewRow row)
        {
            if (row.FindControl(Utility.GetDescription(EnumControlli.TxtAmmontareContr)) != null && ((TextBox)row.FindControl(Utility.GetDescription(EnumControlli.TxtAmmontareContr))).Text != string.Empty &&
                row.FindControl(Utility.GetDescription(EnumControlli.TxtMontanteContr)) != null && ((TextBox)row.FindControl(Utility.GetDescription(EnumControlli.TxtMontanteContr))).Text != string.Empty &&
                row.FindControl(Utility.GetDescription(EnumControlli.TxtSettimaneContr)) != null && ((TextBox)row.FindControl(Utility.GetDescription(EnumControlli.TxtSettimaneContr))).Text != string.Empty &&
                row.FindControl(Utility.GetDescription(EnumControlli.DdlCodiceGestioneContr)) != null && ((DropDownList)row.FindControl(Utility.GetDescription(EnumControlli.DdlCodiceGestioneContr))).SelectedIndex != 0 &&
                row.FindControl(Utility.GetDescription(EnumControlli.DdlQuotaContr)) != null && ((DropDownList)row.FindControl(Utility.GetDescription(EnumControlli.DdlQuotaContr))).SelectedIndex != 0)
                return false;
            else
                return true;
        }

        private List<DatiRetributiviLocal> AddRecordRetributivi(List<DatiRetributiviLocal> listaRecord, String gestione, String quota, String tipoQuota, String decorrenza, String settimane,
            String retribuzioneMedia, string strSett707, string strPL_Quotar = null, string strPL_Quotar707 = null)
        {
            listaRecord.Add(new DatiRetributiviLocal(gestione, quota, tipoQuota, settimane, decorrenza, retribuzioneMedia, strSett707, strPL_Quotar, strPL_Quotar707));
            return listaRecord;
        }

        private List<DatiContributiviLocal> AddRecordContributivi(List<DatiContributiviLocal> listaRecord, String gestione, String quota, String settimane, String ammontareContributivo,
            String montanteContributivo, string strPL_Quotac = null)
        {
            listaRecord.Add(new DatiContributiviLocal(gestione, quota, settimane, ammontareContributivo, montanteContributivo, strPL_Quotac));
            return listaRecord;
        }

        private string GetValueFromIdRetr(string id)
        {
            string ret = string.Empty;
            if (!String.IsNullOrEmpty(id))
            {
                long index = Convert.ToInt64(id);
                List<DecodificaGestioneCalcoloRetributivo> listaCodeGestioneCalcoloRetrib = (List<DecodificaGestioneCalcoloRetributivo>)ViewState[EnumViewState.ListaCodeGestioneCalcoloRetrib.ToString()];
                DecodificaGestioneCalcoloRetributivo app = listaCodeGestioneCalcoloRetrib.Find(delegate (DecodificaGestioneCalcoloRetributivo code) { return (code.Id == index); });
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
                List<DecodificaGestioneCalcoloContributivo> listaCodeGestioneCalcoloContrib = (List<DecodificaGestioneCalcoloContributivo>)ViewState[EnumViewState.ListaCodeGestioneCalcoloContrib.ToString()];
                DecodificaGestioneCalcoloContributivo app = listaCodeGestioneCalcoloContrib.ToList().Find(delegate (DecodificaGestioneCalcoloContributivo code) { return (code.Id == index); });
                if (app != null)
                    ret = app.TraduzioneSuGP + " - " + app.Descrizione;
            }
            return ret;
        }

        private void GestioneDdls(GridViewRow row, bool IsRetrib)
        {
            DropDownList ddlGestione = new DropDownList();
            if (IsRetrib)
            {
                ddlGestione = (DropDownList)row.FindControl(Utility.GetDescription(EnumControlli.DdlCodiceGestioneRetr));
                ddlGestione.Items.Add(new ListItem(string.Empty, string.Empty));
                List<DecodificaGestioneCalcoloRetributivo> listaCodeGestioneCalcoloRetrib = (List<DecodificaGestioneCalcoloRetributivo>)ViewState[EnumViewState.ListaCodeGestioneCalcoloRetrib.ToString()];
                IEnumerable<DecodificaGestioneCalcoloRetributivo> listaOrdinata = listaCodeGestioneCalcoloRetrib.OrderBy(x => x.TraduzioneSuGP);
                foreach (DecodificaGestioneCalcoloRetributivo datiCodeGestioneCalcoloRetrib in listaOrdinata)
                {
                    ListItem li = new ListItem();
                    li.Attributes.Add("title", datiCodeGestioneCalcoloRetrib.Descrizione);
                    li.Text = datiCodeGestioneCalcoloRetrib.TraduzioneSuGP + " - " + datiCodeGestioneCalcoloRetrib.Descrizione;
                    li.Value = datiCodeGestioneCalcoloRetrib.Id.ToString();
                    ddlGestione.Items.Add(li);
                }
                if (((DatiRetributiviLocal)(row.DataItem)).Gestione.Trim() == string.Empty)
                    ddlGestione.SelectedIndex = 0;
                else
                    ddlGestione.Items.FindByValue(((DatiRetributiviLocal)(row.DataItem)).Gestione.Trim()).Selected = true;

                DropDownList ddlTipoQuotaGestioneAltre = new DropDownList();
                DropDownList ddlTipoQuotaGestioneA = new DropDownList();

                ddlTipoQuotaGestioneAltre = (DropDownList)row.FindControl(Utility.GetDescription(EnumControlli.DdlTipoQuotaRetrGestioneAltre));
                ddlTipoQuotaGestioneA = (DropDownList)row.FindControl(Utility.GetDescription(EnumControlli.DdlTipoQuotaRetrGestioneA));

                ddlTipoQuotaGestioneAltre.Items.Add(new ListItem(string.Empty, string.Empty));
                ddlTipoQuotaGestioneA.Items.Add(new ListItem("   - quota 17", string.Empty));

                List<DecodificaTipoQuota> listaDecodificaTipoQuota = (List<DecodificaTipoQuota>)ViewState[EnumViewState.DecodificaTipoQuota.ToString()];
                List<CtrlDecorrenzaRetrExINPDAI> listaCtrlDecorrenzaRetrExINPDAI = (List<CtrlDecorrenzaRetrExINPDAI>)ViewState[EnumViewState.CtrlDecorrenzaRetrExINPDAI.ToString()];

                if (listaDecodificaTipoQuota != null && listaDecodificaTipoQuota.Count() > 0)
                {
                    foreach (DecodificaTipoQuota decTipoQuota in listaDecodificaTipoQuota)
                    {
                        ListItem li = new ListItem();
                        li.Attributes.Add("title", decTipoQuota.Decodifica);
                        li.Text = decTipoQuota.Codice;
                        li.Value = decTipoQuota.Codice;
                        ddlTipoQuotaGestioneAltre.Items.Add(li);

                        if (listaCtrlDecorrenzaRetrExINPDAI != null && listaCtrlDecorrenzaRetrExINPDAI.Count() > 0)
                        {
                            CtrlDecorrenzaRetrExINPDAI ctrl = listaCtrlDecorrenzaRetrExINPDAI.Find(x => x.TipoQuota == decTipoQuota.Codice);
                            ListItem liGestioneA = new ListItem();
                            liGestioneA.Attributes.Add("title", decTipoQuota.Decodifica);
                            liGestioneA.Text = decTipoQuota.Codice + (ctrl != null ? " - quota " + ctrl.CodiceDecorrenza : string.Empty);
                            liGestioneA.Value = decTipoQuota.Codice;
                            ddlTipoQuotaGestioneA.Items.Add(liGestioneA);
                        }
                    }
                }
                ddlTipoQuotaGestioneAltre.SelectedValue = ((DatiRetributiviLocal)(row.DataItem)).TipoQuota;
                ddlTipoQuotaGestioneA.SelectedValue = ((DatiRetributiviLocal)(row.DataItem)).TipoQuota;

                DropDownList ddlQuota = new DropDownList();
                ddlQuota = (DropDownList)row.FindControl(Utility.GetDescription(EnumControlli.DdlQuotaRetr));
                ddlQuota.SelectedValue = ((DatiRetributiviLocal)(row.DataItem)).Quota;
            }
            else
            {
                ddlGestione = (DropDownList)row.FindControl(Utility.GetDescription(EnumControlli.DdlCodiceGestioneContr));
                ddlGestione.Items.Add(new ListItem(string.Empty, string.Empty));
                List<DecodificaGestioneCalcoloContributivo> listaCodeGestioneCalcoloContrib = (List<DecodificaGestioneCalcoloContributivo>)ViewState[EnumViewState.ListaCodeGestioneCalcoloContrib.ToString()];
                IEnumerable<DecodificaGestioneCalcoloContributivo> listaOrdinata = listaCodeGestioneCalcoloContrib.OrderBy(x => x.TraduzioneSuGP);
                foreach (DecodificaGestioneCalcoloContributivo datiCodeGestioneCalcoloContrib in listaOrdinata)
                {
                    ListItem li = new ListItem();
                    li.Attributes.Add("title", datiCodeGestioneCalcoloContrib.Descrizione);
                    li.Text = datiCodeGestioneCalcoloContrib.TraduzioneSuGP + " - " + datiCodeGestioneCalcoloContrib.Descrizione;
                    li.Value = datiCodeGestioneCalcoloContrib.Id.ToString();
                    ddlGestione.Items.Add(li);
                }
                if (((DatiContributiviLocal)(row.DataItem)).Gestione.Trim() == string.Empty)
                    ddlGestione.SelectedIndex = 0;
                else
                    ddlGestione.Items.FindByValue(((DatiContributiviLocal)(row.DataItem)).Gestione.Trim()).Selected = true;

                DropDownList ddlQuota = new DropDownList();
                ddlQuota = (DropDownList)row.FindControl(Utility.GetDescription(EnumControlli.DdlQuotaRetr));
                ddlQuota.SelectedValue = ((DatiContributiviLocal)(row.DataItem)).Quota;

                //Riferimento mail: LIQPENS - Segnalazioni AGO del 30/06/2014
                //Dati contributivi: se il sistema di calcolo è retributivo, il codice quota C  non deve essere presente nel menu di scelta
                if ((((AreaDatiContributivi)ViewState[EnumViewState.AreaDatiContributiviAgo.ToString()]).DatiCalcolo.TipoCalcolo) == GestioneContribTipoCalcolo.Retributivo)
                    ddlQuota.Items.Remove(ddlQuota.Items.FindByValue("C"));
            }
        }

        private List<GestioneAggiornamentoPECODatiContributivi> GetDataContributiviToSave(List<DatiContributiviLocal> lDatiContributiviLocal)
        {
            List<GestioneAggiornamentoPECODatiContributivi> lContr = new List<GestioneAggiornamentoPECODatiContributivi>();
            DecodificaGestioneCalcoloContributivo decGestioneK = null;
            if (ViewState[EnumViewState.ListaCodeGestioneCalcoloContrib.ToString()] != null && ((List<DecodificaGestioneCalcoloContributivo>)ViewState[EnumViewState.ListaCodeGestioneCalcoloContrib.ToString()]).Exists(x => x.TraduzioneSuGP.Trim() == "K"))
                decGestioneK = ((List<DecodificaGestioneCalcoloContributivo>)ViewState[EnumViewState.ListaCodeGestioneCalcoloContrib.ToString()]).FirstOrDefault(x => x.TraduzioneSuGP.Trim() == "K");
            foreach (DatiContributiviLocal datiContributiviLocal in lDatiContributiviLocal)
            {
                if (datiContributiviLocal.AmmontareContributivo == string.Empty && datiContributiviLocal.Gestione == string.Empty && datiContributiviLocal.Quota == string.Empty &&
                    datiContributiviLocal.MontanteContributivo == string.Empty && datiContributiviLocal.Settimane == string.Empty)
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
                else if (!Contr.Quota.HasValue && decGestioneK != null && Contr.CodGestione == decGestioneK.Id)
                {
                    Contr.ImportoContributivo = datiContributiviLocal.AmmontareContributivo.Trim() != string.Empty ? Convert.ToDecimal(datiContributiviLocal.AmmontareContributivo.Trim()) : (decimal?)null;
                    Contr.MontanteContributivo = datiContributiviLocal.MontanteContributivo.Trim() != string.Empty ? Convert.ToDecimal(datiContributiviLocal.MontanteContributivo.Trim()) : (decimal?)null;
                    Contr.Settimane = datiContributiviLocal.Settimane.Trim() != string.Empty ? Convert.ToInt32(datiContributiviLocal.Settimane.Trim()) : (int?)null;
                }
                Contr.PL_Quotac = datiContributiviLocal.PL_Quotac != null && datiContributiviLocal.PL_Quotac != string.Empty ? Convert.ToDecimal(datiContributiviLocal.PL_Quotac.Trim()) : (decimal?)null;

                lContr.Add(Contr);
            }
            return lContr;
        }

        private List<GestioneAggiornamentoPECODatiRetributivi> GetDataRetributiviToSave(List<DatiRetributiviLocal> lDatiRetributiviLocal)
        {
            List<GestioneAggiornamentoPECODatiRetributivi> lRetr = new List<GestioneAggiornamentoPECODatiRetributivi>();

            foreach (DatiRetributiviLocal datiRetributiviLocal in lDatiRetributiviLocal)
            {
                if (datiRetributiviLocal.Decorrenza == string.Empty && datiRetributiviLocal.Gestione == string.Empty && datiRetributiviLocal.Quota == string.Empty &&
                    datiRetributiviLocal.TipoQuota == string.Empty && datiRetributiviLocal.RetribuzioneMedia == string.Empty && datiRetributiviLocal.Settimane == string.Empty)
                    continue;

                GestioneAggiornamentoPECODatiRetributivi Retr = new GestioneAggiornamentoPECODatiRetributivi();

                // reperire l'id del campo ddl Cod Gestione selezionato
                if (datiRetributiviLocal.Gestione.Trim() != string.Empty)
                    Retr.CodGestione = Convert.ToInt64(datiRetributiviLocal.Gestione.Trim());
                else
                    Retr.CodGestione = null;

                Retr.Quota = datiRetributiviLocal.Quota.Trim() != string.Empty ? Convert.ToChar(datiRetributiviLocal.Quota.Trim()) : (char?)null;
                Retr.CodiceTipoQuota = !string.IsNullOrEmpty(datiRetributiviLocal.TipoQuota) ? datiRetributiviLocal.TipoQuota : null;

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
                Retr.NSettimane707 = !string.IsNullOrEmpty(datiRetributiviLocal.Settimane707) ? Int32.Parse(datiRetributiviLocal.Settimane707) : (int?)null;

                UtilityDifferenzaDateTime app = (UtilityDifferenzaDateTime)ViewState[EnumViewState.DecorrenzaCalcoloRetr.ToString()];
                Retr.Decorrenza = new DateTime(app.Year, app.Month, app.Day);
                Retr.PL_Quotar = datiRetributiviLocal.PL_Quotar != null && datiRetributiviLocal.PL_Quotar != string.Empty ? Convert.ToDecimal(datiRetributiviLocal.PL_Quotar.Trim()) : (decimal?)null;
                Retr.PL_Quotar707 = datiRetributiviLocal.PL_Quotar707 != null && datiRetributiviLocal.PL_Quotar707 != string.Empty ? Convert.ToDecimal(datiRetributiviLocal.PL_Quotar707.Trim()) : (decimal?)null;
                lRetr.Add(Retr);
            }
            return lRetr;
        }

        private void ReLoadData(List<DatiRetributiviLocal> listaDatiRetribApp, List<DatiContributiviLocal> listaDatiContribApp)
        {
            if (listaDatiRetribApp != null)
            {
                AreaTitolare.DatiPensione datiPensione = new AreaTitolare.DatiPensione();
                datiPensione = GetDatiPensione(this);

                DatiRetributiviLocal EmptyRetr = listaDatiRetribApp.Find(delegate (DatiRetributiviLocal code)
                {
                    return (code.Decorrenza == string.Empty && code.Gestione == string.Empty && code.RetribuzioneMedia == string.Empty &&
                            code.Settimane == string.Empty && code.Quota == string.Empty && code.TipoQuota == string.Empty);
                });

                if (EmptyRetr == null && !(((AreaDatiContributivi)ViewState[EnumViewState.AreaDatiContributiviAgo.ToString()]).DatiCalcolo.IsUnicarpe)
                    && !(CodeUtility.IsRicostituzione(datiPensione) && !CodeUtility.IsRicostituzioneContributiva(datiPensione)))
                    listaDatiRetribApp.Add(new DatiRetributiviLocal(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty));

                gvDatiRetributivi.DataSource = listaDatiRetribApp;
                ViewState[EnumViewState.ElencoDatiRetributivi.ToString()] = listaDatiRetribApp;
                gvDatiRetributivi.DataBind();
            }

            if (listaDatiContribApp != null)
            {
                DatiContributiviLocal EmptyContr = listaDatiContribApp.Find(delegate (DatiContributiviLocal code)
                {
                    return (code.AmmontareContributivo == string.Empty && code.Gestione == string.Empty &&
                            code.MontanteContributivo == string.Empty && code.Settimane == string.Empty);
                });

                if (EmptyContr == null && !(((AreaDatiContributivi)ViewState[EnumViewState.AreaDatiContributiviAgo.ToString()]).DatiCalcolo.IsUnicarpe))
                    listaDatiContribApp.Add(new DatiContributiviLocal(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty));

                gvDatiContributivi.DataSource = listaDatiContribApp;
                ViewState[EnumViewState.ElencoDatiContributivi.ToString()] = listaDatiContribApp;
                gvDatiContributivi.DataBind();
            }
        }

        private void GetDatiINPDAI(AreaDatiContributivi areaDatiContributiviAgo)
        {
            if (areaDatiContributiviAgo.DatiExINPDAI == null)
                areaDatiContributiviAgo.DatiExINPDAI = new GestioneContribDatiExINPDAI();

            if (!string.IsNullOrEmpty(txtAnzAl95.Text))
                areaDatiContributiviAgo.DatiExINPDAI.AnzAl95 = decimal.Parse(txtAnzAl95.Text);

            if (!string.IsNullOrEmpty(txtQuotaAl95.Text))
                areaDatiContributiviAgo.DatiExINPDAI.QuotaAl95 = decimal.Parse(txtQuotaAl95.Text);
        }

        private void PrevalorizzaRecordRetributivo(List<DatiRetributiviLocal> elencoDatiRetributivi)
        {
            AreaTitolare.DatiPensione datiPensione = new AreaTitolare.DatiPensione();
            datiPensione = GetDatiPensione(this);

            if (elencoDatiRetributivi == null || elencoDatiRetributivi.Count == 0)
            {
                DatiRetributiviLocal dati = new DatiRetributiviLocal();
                string gestione = string.Empty;

                List<DecodificaGestioneCalcoloRetributivo> listaCodeGestioneCalcoloRetrib = (List<DecodificaGestioneCalcoloRetributivo>)ViewState[EnumViewState.ListaCodeGestioneCalcoloRetrib.ToString()];
                DecodificaGestioneCalcoloRetributivo app = null;
                if (ViewState[EnumViewState.IsPrimoRecordRetrGestioneS.ToString()] != null && (bool)ViewState[EnumViewState.IsPrimoRecordRetrGestioneS.ToString()])
                    app = listaCodeGestioneCalcoloRetrib.ToList().Find(delegate (DecodificaGestioneCalcoloRetributivo code) { return (code.TraduzioneSuGP.Trim() == "S"); });
                else
                    app = listaCodeGestioneCalcoloRetrib.ToList().Find(delegate (DecodificaGestioneCalcoloRetributivo code) { return (code.TraduzioneSuGP.Trim() == "A"); });
                if (app != null)
                    gestione = app.Id.ToString();

                dati.Gestione = gestione;
                dati.Quota = "A";
                dati.TipoQuota = "A1";

                if (elencoDatiRetributivi == null)
                    elencoDatiRetributivi = new List<DatiRetributiviLocal>();
                elencoDatiRetributivi.Add(dati);
            }
            else
            {
                DatiRetributiviLocal Empty = elencoDatiRetributivi.Find(delegate (DatiRetributiviLocal code)
                {
                    return (code.Decorrenza == string.Empty && code.Gestione == string.Empty && code.RetribuzioneMedia == string.Empty &&
                            code.Settimane == string.Empty && code.Quota == string.Empty && code.TipoQuota == string.Empty);
                });

                if (Empty == null && !(((AreaDatiContributivi)ViewState[EnumViewState.AreaDatiContributiviAgo.ToString()]).DatiCalcolo.IsUnicarpe)
                    && !(CodeUtility.IsRicostituzione(datiPensione) && !CodeUtility.IsRicostituzioneContributiva(datiPensione)))
                    elencoDatiRetributivi.Add(new DatiRetributiviLocal(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty));
            }

            ViewState[EnumViewState.ElencoDatiRetributivi.ToString()] = elencoDatiRetributivi;
        }

        private bool IsListaPrecompilata()
        {
            List<DatiRetributiviLocal> listaDatiRetrApp = (List<DatiRetributiviLocal>)ViewState[EnumViewState.ElencoDatiRetributivi.ToString()];
            if (listaDatiRetrApp != null && listaDatiRetrApp.Count == 1 && string.IsNullOrEmpty(listaDatiRetrApp[0].Decorrenza) &&
                !string.IsNullOrEmpty(listaDatiRetrApp[0].Gestione) && !string.IsNullOrEmpty(listaDatiRetrApp[0].Quota) &&
                string.IsNullOrEmpty(listaDatiRetrApp[0].RetribuzioneMedia) && string.IsNullOrEmpty(listaDatiRetrApp[0].Settimane) &&
                !string.IsNullOrEmpty(listaDatiRetrApp[0].TipoQuota))
                return true;
            else
                return false;
        }

        private void RenderControls(AreaDatiContributivi areaDatiContributivi)
        {
            if (this.domanda == null)
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            if (this.TitolarePensione == null)
                this.TitolarePensione = new AreaTitolare();
            if (this.TitolarePensione.Pensione == null)
                this.TitolarePensione.Pensione = GetDatiPensione(this);

            if (areaDatiContributivi != null && areaDatiContributivi.DatiExINPDAI != null && areaDatiContributivi.DatiExINPDAI.IsDataAnzianitaAl95Bloccato)
            {
                txtAnzAl95.Enabled = false;
                txtQuotaAl95.Enabled = false;
            }
            if (areaDatiContributivi != null && areaDatiContributivi.DatiExINPDAI != null && !areaDatiContributivi.DatiExINPDAI.IsContribSolidarietaVisible)
            {
                pnlContributoSolidarieta.Visible = false;
            }

            if (this.domanda.IsDomandaRiapertura && Utility.IsDomandaUnicarpe(this.TitolarePensione.Pensione, true) == Utility.TipoUnicarpe.Automatica && !this.domanda.Categoria.StartsWith("S"))
                btnEliminaDatiCalcolo.Enabled = false;
        }

        private void ManageUpdateGridVittimeTerrorismoContributivo()
        {
            AreaDatiContributivi areaDatiContributiviAgo = ViewState[Utility.GetDescription(EnumViewState.AreaDatiContributiviAgo)] as AreaDatiContributivi;
            if (areaDatiContributiviAgo != null && areaDatiContributiviAgo.IsDatiContributiviVittimeVisible)
                RaiseUpdateDatiCalcoloTerrorismoContributivi(this, new EventArgs());
        }

        private void ManageUpdateGridVittimeTerrorismoRetributivi()
        {
            AreaDatiContributivi areaDatiContributiviAgo = ViewState[Utility.GetDescription(EnumViewState.AreaDatiContributiviAgo)] as AreaDatiContributivi;
            if (areaDatiContributiviAgo != null && areaDatiContributiviAgo.IsDatiRetributiviVittimeVisible)
                RaiseUpdateDatiCalcoloTerrorismoRetributivi(this, new EventArgs());
        }
        #endregion private methods

        #region gvDatiRetributivi
        protected void gvDatiRetributivi_Load(object sender, EventArgs e)
        {
            AreaDatiContributivi areaDatiContributiviAgo = ViewState[Utility.GetDescription(EnumViewState.AreaDatiContributiviAgo)] as AreaDatiContributivi;
            if (areaDatiContributiviAgo != null)
                gvDatiRetributivi.Columns[(int)EnumColonneGv.GvRetrSett707].Visible = areaDatiContributiviAgo.IsSettimane707Visible;

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
                List<DatiRetributiviLocal> listaDatiRetribApp = new List<DatiRetributiviLocal>();
                foreach (GridViewRow rApp in ((GridView)sender).Rows)
                {
                    string GestioneApp = string.Empty;
                    string QuotaApp = string.Empty;
                    string TipoQuotaApp = string.Empty;
                    string SettimaneApp = string.Empty;
                    string DecorrenzaApp = string.Empty;
                    string RetribuzioneMediaApp = string.Empty;
                    string Settimane707 = string.Empty;
                    string PL_Quotar = string.Empty;
                    string PL_Quotar707 = string.Empty;
                    if (!IsEmptyReadableRowRetrib(rApp))
                    {
                        if (rApp.DataItemIndex != r.DataItemIndex)
                        {
                            GestioneApp = ((Label)rApp.FindControl(Utility.GetDescription(EnumControlli.LblIdCodiceGestioneRetr))).Text;
                            QuotaApp = ((Label)rApp.FindControl(Utility.GetDescription(EnumControlli.LblQuotaRetr))).Text;
                            TipoQuotaApp = ((Label)rApp.FindControl(Utility.GetDescription(EnumControlli.LblTipoQuotaRetr))).Text;
                            DecorrenzaApp = ((Label)rApp.FindControl(Utility.GetDescription(EnumControlli.LblDecorrenzaRetr))).Text;
                            SettimaneApp = ((Label)rApp.FindControl(Utility.GetDescription(EnumControlli.LblSettimaneRetr))).Text;
                            RetribuzioneMediaApp = ((Label)rApp.FindControl(Utility.GetDescription(EnumControlli.LblRetribuzioneMediaRetr))).Text;
                            Settimane707 = ((Label)rApp.FindControl(Utility.GetDescription(EnumControlli.LblSettimane707))).Text;
                            PL_Quotar = ((HiddenField)rApp.FindControl(Utility.GetDescription(EnumControlli.LblPL_Quotar))).Value;
                            PL_Quotar707 = ((HiddenField)rApp.FindControl(Utility.GetDescription(EnumControlli.LblPL_Quotar707))).Value;

                            listaDatiRetribApp = AddRecordRetributivi(listaDatiRetribApp, GestioneApp, QuotaApp, TipoQuotaApp, DecorrenzaApp, SettimaneApp, RetribuzioneMediaApp, Settimane707, PL_Quotar, PL_Quotar707);
                        }
                    }
                }
                if (listaDatiRetribApp.Count == 0)
                    this.modalitaEditRetributivi.Value = "false";

                //listaDatiRetribApp.Add(new DatiRetributiviLocal(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty));
                PrevalorizzaRecordRetributivo(listaDatiRetribApp);

                ViewState[EnumViewState.ElencoDatiRetributivi.ToString()] = listaDatiRetribApp;

                gvDatiRetributivi.DataSource = listaDatiRetribApp;
                gvDatiRetributivi.DataBind();

                ManageUpdateGridVittimeTerrorismoRetributivi();
                RaiseHideAvviso(this, null);

                #endregion Elimina
            }
            else if (e.CommandName == "Edit")
            {
                modalitaEditRetributivi.Value = "true";
                RaiseHideAvviso(this, null);
            }
            else if (e.CommandName == "Salva")
            {
                #region Salva

                if (!IsEmptyEditableRowRetrib((GridViewRow)((Control)e.CommandSource).NamingContainer))
                {
                    List<DatiRetributiviLocal> listaDatiRetrApp = new List<DatiRetributiviLocal>();
                    foreach (GridViewRow rApp in ((GridView)sender).Rows)
                    {
                        string GestioneApp = string.Empty;
                        string QuotaApp = string.Empty;
                        string TipoQuotaApp = string.Empty;
                        string SettimaneApp = string.Empty;
                        string DecorrenzaApp = string.Empty;
                        string RetribuzioneMediaApp = string.Empty;
                        string Settimane707 = string.Empty;
                        string PL_Quotar = string.Empty;
                        string PL_Quotar707 = string.Empty;

                        if (!IsEmptyEditableRowRetrib(rApp))
                        {
                            GestioneApp = ((DropDownList)rApp.FindControl(Utility.GetDescription(EnumControlli.DdlCodiceGestioneRetr))).SelectedValue;
                            QuotaApp = ((DropDownList)rApp.FindControl(Utility.GetDescription(EnumControlli.DdlQuotaRetr))).SelectedValue;

                            List<DecodificaGestioneCalcoloRetributivo> listaCodeGestioneCalcoloRetrib = (List<DecodificaGestioneCalcoloRetributivo>)ViewState[EnumViewState.ListaCodeGestioneCalcoloRetrib.ToString()];
                            DecodificaGestioneCalcoloRetributivo gestioneA = listaCodeGestioneCalcoloRetrib.Find(x => x.TraduzioneSuGP.Trim() == "A");


                            //logica selezione gestione per quote di tipo A e per quote di altri tipi
                            if (gestioneA != null && GestioneApp == gestioneA.Id.ToString())
                            {
                                TipoQuotaApp = ((DropDownList)rApp.FindControl(Utility.GetDescription(EnumControlli.DdlTipoQuotaRetrGestioneA))).SelectedValue;
                            }
                            else
                            {
                                TipoQuotaApp = ((DropDownList)rApp.FindControl(Utility.GetDescription(EnumControlli.DdlTipoQuotaRetrGestioneAltre))).SelectedValue;
                            }

                            DecorrenzaApp = GetDecorrenzaRetributiva(GestioneApp, QuotaApp, TipoQuotaApp);
                            SettimaneApp = ((TextBox)rApp.FindControl(Utility.GetDescription(EnumControlli.TxtSettimaneRetr))).Text;
                            RetribuzioneMediaApp = ((TextBox)rApp.FindControl(Utility.GetDescription(EnumControlli.TxtRetribuzioneMediaRetr))).Text;
                            Settimane707 = ((TextBox)rApp.FindControl(Utility.GetDescription(EnumControlli.TxtSettimaneRetributive707))).Text;
                            PL_Quotar = ((HiddenField)rApp.FindControl(Utility.GetDescription(EnumControlli.LblPL_Quotar))).Value;
                            PL_Quotar707 = ((HiddenField)rApp.FindControl(Utility.GetDescription(EnumControlli.LblPL_Quotar707))).Value;
                            if (string.IsNullOrEmpty(DecorrenzaApp))
                            {
                                string strGestione = ((DropDownList)rApp.FindControl(Utility.GetDescription(EnumControlli.DdlCodiceGestioneRetr))).SelectedItem.Text.Split('-')[0].Trim();
                                this.HasError = true;
                                this.ErrorMessage = "La terna Gestione '" + strGestione + "', Quota '" + QuotaApp + "' e Tipo Quota '" + TipoQuotaApp + "' non è valida.";
                                RaiseShowAvviso(this, null);
                                return;
                            }
                            listaDatiRetrApp = AddRecordRetributivi(listaDatiRetrApp, GestioneApp, QuotaApp, TipoQuotaApp, DecorrenzaApp, SettimaneApp, RetribuzioneMediaApp, Settimane707, PL_Quotar, PL_Quotar707);
                        }
                        else if (!IsEmptyReadableRowRetrib(rApp))
                        {
                            GestioneApp = ((Label)rApp.FindControl(Utility.GetDescription(EnumControlli.LblIdCodiceGestioneRetr))).Text;
                            QuotaApp = ((Label)rApp.FindControl(Utility.GetDescription(EnumControlli.LblQuotaRetr))).Text;
                            TipoQuotaApp = ((Label)rApp.FindControl(Utility.GetDescription(EnumControlli.LblTipoQuotaRetr))).Text;
                            SettimaneApp = ((Label)rApp.FindControl(Utility.GetDescription(EnumControlli.LblSettimaneRetr))).Text;
                            DecorrenzaApp = ((Label)rApp.FindControl(Utility.GetDescription(EnumControlli.LblDecorrenzaRetr))).Text;
                            RetribuzioneMediaApp = ((Label)rApp.FindControl(Utility.GetDescription(EnumControlli.LblRetribuzioneMediaRetr))).Text;
                            Settimane707 = ((Label)rApp.FindControl(Utility.GetDescription(EnumControlli.LblSettimane707))).Text;
                            PL_Quotar = ((HiddenField)rApp.FindControl(Utility.GetDescription(EnumControlli.LblPL_Quotar))).Value;
                            PL_Quotar707 = ((HiddenField)rApp.FindControl(Utility.GetDescription(EnumControlli.LblPL_Quotar707))).Value;
                            listaDatiRetrApp = AddRecordRetributivi(listaDatiRetrApp, GestioneApp, QuotaApp, TipoQuotaApp, DecorrenzaApp, SettimaneApp, RetribuzioneMediaApp, Settimane707, PL_Quotar, PL_Quotar707);
                        }
                    }
                    listaDatiRetrApp.Add(new DatiRetributiviLocal(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty));
                    gvDatiRetributivi.EditIndex = -1;
                    ViewState[EnumViewState.ElencoDatiRetributivi.ToString()] = listaDatiRetrApp;
                    modalitaEditRetributivi.Value = "false";
                    gvDatiRetributivi.DataSource = listaDatiRetrApp;
                    gvDatiRetributivi.DataBind();

                    ManageUpdateGridVittimeTerrorismoRetributivi();
                    RaiseHideAvviso(this, null);
                }
                #endregion Salva
            }
            else if (e.CommandName == "Annulla")
            {
                #region Annulla
                List<DatiRetributiviLocal> listaDatiRetrApp = (List<DatiRetributiviLocal>)ViewState[EnumViewState.ElencoDatiRetributivi.ToString()];
                if (!IsListaEmpty(true))
                {
                    modalitaEditRetributivi.Value = "false";
                    gvDatiRetributivi.EditIndex = -1;
                    gvDatiRetributivi.DataSource = listaDatiRetrApp;
                    gvDatiRetributivi.DataBind();
                }

                RaiseHideAvviso(this, null);
                #endregion Annulla
            }
        }

        protected void gvDatiRetributivi_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (this.domanda == null)
                    this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

                AreaTitolare.DatiPensione datiPensione = null;
                datiPensione = GetDatiPensione(this);

                //Render columns for UNICARPE
                if (e.Row.RowType == DataControlRowType.DataRow || e.Row.RowType == DataControlRowType.Header)
                {
                    if (((AreaDatiContributivi)ViewState[EnumViewState.AreaDatiContributiviAgo.ToString()]).DatiCalcolo.IsUnicarpe ||
                        (CodeUtility.IsRicostituzione(datiPensione) && Utility.IsDomandaDAI(domanda.Categoria) &&
                        domanda.Categoria.StartsWith("S") && datiPensione.NaturaPensione.Substring(2, 1) == "T") ||
                        disabilitaPannelloDatiCalcoloRicSdai() || (CodeUtility.IsRicostituzione(datiPensione) && !CodeUtility.IsRicostituzioneContributiva(datiPensione)))
                    {
                        e.Row.Cells[0].Visible = false;
                        e.Row.Cells[e.Row.Cells.Count - 2].Visible = false;
                    }
                }

                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    if ((((AreaDatiContributivi)ViewState[EnumViewState.AreaDatiContributiviAgo.ToString()]).DatiCalcolo.IsUnicarpe) || (CodeUtility.IsRicostituzione(datiPensione) && !CodeUtility.IsRicostituzioneContributiva(datiPensione)))  // sola lettura
                    {
                        gvDatiRetributivi.EditIndex = -1;

                        ((Label)e.Row.FindControl(Utility.GetDescription(EnumControlli.LblCodiceGestioneRetr))).Text = GetValueFromIdRetr(((DatiRetributiviLocal)(e.Row.DataItem)).Gestione);
                        ((Label)e.Row.FindControl(Utility.GetDescription(EnumControlli.LblCodiceGestioneRetr))).Attributes.Add("title", ((Label)e.Row.FindControl(Utility.GetDescription(EnumControlli.LblCodiceGestioneRetr))).Text);
                        ((Label)e.Row.FindControl(Utility.GetDescription(EnumControlli.LblIdCodiceGestioneRetr))).Text = ((DatiRetributiviLocal)(e.Row.DataItem)).Gestione;
                        ((Label)e.Row.FindControl(Utility.GetDescription(EnumControlli.LblQuotaRetr))).Text = ((DatiRetributiviLocal)(e.Row.DataItem)).Quota;
                        ((Label)e.Row.FindControl(Utility.GetDescription(EnumControlli.LblTipoQuotaRetr))).Text = ((DatiRetributiviLocal)(e.Row.DataItem)).TipoQuota;
                        ((Label)e.Row.FindControl(Utility.GetDescription(EnumControlli.LblDecorrenzaRetr))).Text = ((DatiRetributiviLocal)(e.Row.DataItem)).Decorrenza;
                        ((Label)e.Row.FindControl(Utility.GetDescription(EnumControlli.LblSettimaneRetr))).Text = ((DatiRetributiviLocal)(e.Row.DataItem)).Settimane;
                        ((Label)e.Row.FindControl(Utility.GetDescription(EnumControlli.LblRetribuzioneMediaRetr))).Text = CodeUtility.ConvertDecimalFixedPoint(((DatiRetributiviLocal)(e.Row.DataItem)).RetribuzioneMedia, 6);
                        ((Label)e.Row.FindControl(Utility.GetDescription(EnumControlli.LblSettimane707))).Text = ((DatiRetributiviLocal)(e.Row.DataItem)).Settimane707;
                        ((HiddenField)e.Row.FindControl(Utility.GetDescription(EnumControlli.LblPL_Quotar))).Value = ((DatiRetributiviLocal)(e.Row.DataItem)).PL_Quotar;
                        ((HiddenField)e.Row.FindControl(Utility.GetDescription(EnumControlli.LblPL_Quotar707))).Value = ((DatiRetributiviLocal)(e.Row.DataItem)).PL_Quotar707;

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

                                    ((DropDownList)e.Row.FindControl(Utility.GetDescription(EnumControlli.DdlCodiceGestioneRetr))).Enabled = false;
                                    ((DropDownList)e.Row.FindControl(Utility.GetDescription(EnumControlli.DdlQuotaRetr))).Enabled = false;
                                    ((DropDownList)e.Row.FindControl(Utility.GetDescription(EnumControlli.DdlTipoQuotaRetrGestioneA))).Enabled = false;
                                    ((DropDownList)e.Row.FindControl(Utility.GetDescription(EnumControlli.DdlTipoQuotaRetrGestioneAltre))).Enabled = false;

                                    CodeUtility.EnableEditableMode(e.Row.Cells[0], "UCTabDatiCalcoloAgoRetr", Page.Theme);
                                    LinkButton delete = (LinkButton)(e.Row.FindControl(Utility.GetDescription(EnumControlli.BtnDeleteRetributivi)));
                                    delete.Text = string.Empty;
                                    if (!((AreaDatiContributivi)ViewState[EnumViewState.AreaDatiContributiviAgo.ToString()]).IsSettimane707Visible)
                                    {
                                        ((RequiredFieldValidator)e.Row.FindControl(Utility.GetDescription(EnumControlli.RfvtxtSettimaneRetributive707))).Enabled = false;
                                    }
                                    ((HiddenField)e.Row.FindControl(Utility.GetDescription(EnumControlli.LblPL_Quotar))).Value = ((DatiRetributiviLocal)(e.Row.DataItem)).PL_Quotar;
                                    ((HiddenField)e.Row.FindControl(Utility.GetDescription(EnumControlli.LblPL_Quotar707))).Value = ((DatiRetributiviLocal)(e.Row.DataItem)).PL_Quotar707;
                                }
                                else
                                {
                                    ((Label)e.Row.FindControl(Utility.GetDescription(EnumControlli.LblCodiceGestioneRetr))).Text = GetValueFromIdRetr(((DatiRetributiviLocal)(e.Row.DataItem)).Gestione);
                                    ((Label)e.Row.FindControl(Utility.GetDescription(EnumControlli.LblCodiceGestioneRetr))).Attributes.Add("title", ((Label)e.Row.FindControl(Utility.GetDescription(EnumControlli.LblCodiceGestioneRetr))).Text);
                                    ((Label)e.Row.FindControl(Utility.GetDescription(EnumControlli.LblIdCodiceGestioneRetr))).Text = ((DatiRetributiviLocal)(e.Row.DataItem)).Gestione;
                                    ((Label)e.Row.FindControl(Utility.GetDescription(EnumControlli.LblQuotaRetr))).Text = ((DatiRetributiviLocal)(e.Row.DataItem)).Quota;
                                    ((Label)e.Row.FindControl(Utility.GetDescription(EnumControlli.LblTipoQuotaRetr))).Text = ((DatiRetributiviLocal)(e.Row.DataItem)).TipoQuota;
                                    ((Label)e.Row.FindControl(Utility.GetDescription(EnumControlli.LblDecorrenzaRetr))).Text = ((DatiRetributiviLocal)(e.Row.DataItem)).Decorrenza;
                                    ((Label)e.Row.FindControl(Utility.GetDescription(EnumControlli.LblSettimaneRetr))).Text = ((DatiRetributiviLocal)(e.Row.DataItem)).Settimane;
                                    ((Label)e.Row.FindControl(Utility.GetDescription(EnumControlli.LblRetribuzioneMediaRetr))).Text = CodeUtility.ConvertDecimalFixedPoint(((DatiRetributiviLocal)(e.Row.DataItem)).RetribuzioneMedia, 6);
                                    ((Label)e.Row.FindControl(Utility.GetDescription(EnumControlli.LblSettimane707))).Text = ((DatiRetributiviLocal)(e.Row.DataItem)).Settimane707;
                                    ((HiddenField)e.Row.FindControl(Utility.GetDescription(EnumControlli.LblPL_Quotar))).Value = ((DatiRetributiviLocal)(e.Row.DataItem)).PL_Quotar;
                                    ((HiddenField)e.Row.FindControl(Utility.GetDescription(EnumControlli.LblPL_Quotar707))).Value = ((DatiRetributiviLocal)(e.Row.DataItem)).PL_Quotar707;

                                    CodeUtility.EnableReadableMode(e.Row.Cells[0], e.Row.Cells[7], Page.Theme, null);
                                }

                            }
                            else  //prima riga non vuota
                            {
                                if (e.Row.Cells[0].Controls.Count == 3)  // edit mode
                                {
                                    GestioneDdls(e.Row, true);
                                    CodeUtility.EnableEditableMode(e.Row.Cells[0], "UCTabDatiCalcoloAgoRetr", Page.Theme);

                                    if (!((AreaDatiContributivi)ViewState[EnumViewState.AreaDatiContributiviAgo.ToString()]).IsSettimane707Visible)
                                    {
                                        ((RequiredFieldValidator)e.Row.FindControl(Utility.GetDescription(EnumControlli.RfvtxtSettimaneRetributive707))).Enabled = false;
                                    }
                                    ((HiddenField)e.Row.FindControl(Utility.GetDescription(EnumControlli.LblPL_Quotar))).Value = ((DatiRetributiviLocal)(e.Row.DataItem)).PL_Quotar;
                                    ((HiddenField)e.Row.FindControl(Utility.GetDescription(EnumControlli.LblPL_Quotar707))).Value = ((DatiRetributiviLocal)(e.Row.DataItem)).PL_Quotar707;
                                }
                                else
                                {
                                    ((Label)e.Row.FindControl(Utility.GetDescription(EnumControlli.LblCodiceGestioneRetr))).Text = GetValueFromIdRetr(((DatiRetributiviLocal)(e.Row.DataItem)).Gestione);
                                    ((Label)e.Row.FindControl(Utility.GetDescription(EnumControlli.LblCodiceGestioneRetr))).Attributes.Add("title", ((Label)e.Row.FindControl(Utility.GetDescription(EnumControlli.LblCodiceGestioneRetr))).Text);
                                    ((Label)e.Row.FindControl(Utility.GetDescription(EnumControlli.LblIdCodiceGestioneRetr))).Text = ((DatiRetributiviLocal)(e.Row.DataItem)).Gestione;
                                    ((Label)e.Row.FindControl(Utility.GetDescription(EnumControlli.LblQuotaRetr))).Text = ((DatiRetributiviLocal)(e.Row.DataItem)).Quota;
                                    ((Label)e.Row.FindControl(Utility.GetDescription(EnumControlli.LblTipoQuotaRetr))).Text = ((DatiRetributiviLocal)(e.Row.DataItem)).TipoQuota;
                                    ((Label)e.Row.FindControl(Utility.GetDescription(EnumControlli.LblDecorrenzaRetr))).Text = ((DatiRetributiviLocal)(e.Row.DataItem)).Decorrenza;
                                    ((Label)e.Row.FindControl(Utility.GetDescription(EnumControlli.LblSettimaneRetr))).Text = ((DatiRetributiviLocal)(e.Row.DataItem)).Settimane;
                                    ((Label)e.Row.FindControl(Utility.GetDescription(EnumControlli.LblRetribuzioneMediaRetr))).Text = CodeUtility.ConvertDecimalFixedPoint(((DatiRetributiviLocal)(e.Row.DataItem)).RetribuzioneMedia, 6);
                                    ((Label)e.Row.FindControl(Utility.GetDescription(EnumControlli.LblSettimane707))).Text = ((DatiRetributiviLocal)(e.Row.DataItem)).Settimane707;
                                    ((HiddenField)e.Row.FindControl(Utility.GetDescription(EnumControlli.LblPL_Quotar))).Value = ((DatiRetributiviLocal)(e.Row.DataItem)).PL_Quotar;
                                    ((HiddenField)e.Row.FindControl(Utility.GetDescription(EnumControlli.LblPL_Quotar707))).Value = ((DatiRetributiviLocal)(e.Row.DataItem)).PL_Quotar707;

                                    CodeUtility.EnableReadableMode(e.Row.Cells[0], e.Row.Cells[7], Page.Theme, null);
                                }
                            }
                        }
                        else  // righe successive
                        {
                            if (e.Row.Cells[0].Controls.Count == 3)  // edit mode
                            {
                                GestioneDdls(e.Row, true);
                                CodeUtility.EnableEditableMode(e.Row.Cells[0], "UCTabDatiCalcoloAgoRetr", Page.Theme);
                                if (!((AreaDatiContributivi)ViewState[EnumViewState.AreaDatiContributiviAgo.ToString()]).IsSettimane707Visible)
                                {
                                    ((RequiredFieldValidator)e.Row.FindControl(Utility.GetDescription(EnumControlli.RfvtxtSettimaneRetributive707))).Enabled = false;
                                }
                                ((HiddenField)e.Row.FindControl(Utility.GetDescription(EnumControlli.LblPL_Quotar))).Value = ((DatiRetributiviLocal)(e.Row.DataItem)).PL_Quotar;
                                ((HiddenField)e.Row.FindControl(Utility.GetDescription(EnumControlli.LblPL_Quotar707))).Value = ((DatiRetributiviLocal)(e.Row.DataItem)).PL_Quotar707;
                            }

                            else if (e.Row.DataItemIndex == ((List<DatiRetributiviLocal>)ViewState[EnumViewState.ElencoDatiRetributivi.ToString()]).Count - 1)
                            {
                                LinkButton add = ((LinkButton)(e.Row.Cells[0].Controls[0]));
                                add.Text = (string)"<img width=16 height=16 border=0 src=../App_themes/" + Page.Theme + "/Images/add24.png />";
                                add.ToolTip = "Aggiungi";
                            }
                            else
                            {
                                ((Label)e.Row.FindControl(Utility.GetDescription(EnumControlli.LblCodiceGestioneRetr))).Text = GetValueFromIdRetr(((DatiRetributiviLocal)(e.Row.DataItem)).Gestione);
                                ((Label)e.Row.FindControl(Utility.GetDescription(EnumControlli.LblCodiceGestioneRetr))).Attributes.Add("title", ((Label)e.Row.FindControl(Utility.GetDescription(EnumControlli.LblCodiceGestioneRetr))).Text);
                                ((Label)e.Row.FindControl(Utility.GetDescription(EnumControlli.LblIdCodiceGestioneRetr))).Text = ((DatiRetributiviLocal)(e.Row.DataItem)).Gestione;
                                ((Label)e.Row.FindControl(Utility.GetDescription(EnumControlli.LblQuotaRetr))).Text = ((DatiRetributiviLocal)(e.Row.DataItem)).Quota;
                                ((Label)e.Row.FindControl(Utility.GetDescription(EnumControlli.LblTipoQuotaRetr))).Text = ((DatiRetributiviLocal)(e.Row.DataItem)).TipoQuota;
                                ((Label)e.Row.FindControl(Utility.GetDescription(EnumControlli.LblDecorrenzaRetr))).Text = ((DatiRetributiviLocal)(e.Row.DataItem)).Decorrenza;
                                ((Label)e.Row.FindControl(Utility.GetDescription(EnumControlli.LblSettimaneRetr))).Text = ((DatiRetributiviLocal)(e.Row.DataItem)).Settimane;
                                ((Label)e.Row.FindControl(Utility.GetDescription(EnumControlli.LblRetribuzioneMediaRetr))).Text = CodeUtility.ConvertDecimalFixedPoint(((DatiRetributiviLocal)(e.Row.DataItem)).RetribuzioneMedia, 6);
                                ((Label)e.Row.FindControl(Utility.GetDescription(EnumControlli.LblSettimane707))).Text = ((DatiRetributiviLocal)(e.Row.DataItem)).Settimane707;
                                ((HiddenField)e.Row.FindControl(Utility.GetDescription(EnumControlli.LblPL_Quotar))).Value = ((DatiRetributiviLocal)(e.Row.DataItem)).PL_Quotar;
                                ((HiddenField)e.Row.FindControl(Utility.GetDescription(EnumControlli.LblPL_Quotar707))).Value = ((DatiRetributiviLocal)(e.Row.DataItem)).PL_Quotar707;

                                CodeUtility.EnableReadableMode(e.Row.Cells[0], e.Row.Cells[7], Page.Theme, Utility.GetDescription(EnumControlli.BtnDeleteRetributivi));
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

        protected void gvDatiRetributivi_DataBound(object sender, EventArgs e)
        {
            ManagePulsanti();
        }
        #endregion gvDatiRetributivi

        #region gvDatiContributivi
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
                throw new INPS.DNA.DnaApplicationException("UCDatiContributiviAgo, Errore nel metodo gvDatiContributivi_RowCancelingEdit " + ex);
            }
        }

        protected void gvDatiContributivi_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Elimina")
            {
                #region Elimina

                GridViewRow r = (GridViewRow)((WebControl)e.CommandSource).NamingContainer;
                List<DatiContributiviLocal> listaDatiContrApp = new List<DatiContributiviLocal>();
                foreach (GridViewRow rApp in ((GridView)sender).Rows)
                {
                    string QuotaApp = string.Empty;
                    string CodGestioneApp = string.Empty;
                    string SettimaneApp = string.Empty;
                    string AmmontareContrApp = string.Empty;
                    string MontanteContrApp = string.Empty;
                    string PL_Quotac = string.Empty;
                    if (!IsEmptyReadableRowContr(rApp))
                    {
                        if (rApp.DataItemIndex != r.DataItemIndex)
                        {
                            QuotaApp = ((Label)rApp.FindControl(Utility.GetDescription(EnumControlli.LblQuotaContr))).Text;
                            SettimaneApp = ((Label)rApp.FindControl(Utility.GetDescription(EnumControlli.LblSettimaneContr))).Text;
                            AmmontareContrApp = ((Label)rApp.FindControl(Utility.GetDescription(EnumControlli.LblAmmontareContr))).Text;
                            MontanteContrApp = ((Label)rApp.FindControl(Utility.GetDescription(EnumControlli.LblMontanteContr))).Text;
                            CodGestioneApp = ((Label)rApp.FindControl(Utility.GetDescription(EnumControlli.LblIdCodiceGestioneContr))).Text;
                            PL_Quotac = ((HiddenField)rApp.FindControl(Utility.GetDescription(EnumControlli.LblPL_Quotac))).Value;
                            listaDatiContrApp = AddRecordContributivi(listaDatiContrApp, CodGestioneApp, QuotaApp, SettimaneApp, AmmontareContrApp, MontanteContrApp, PL_Quotac);
                        }
                    }
                }
                if (listaDatiContrApp.Count == 0)
                    this.modalitaEditContributivi.Value = "false";

                listaDatiContrApp.Add(new DatiContributiviLocal(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty));

                ViewState[EnumViewState.ElencoDatiContributivi.ToString()] = listaDatiContrApp;
                gvDatiContributivi.DataSource = listaDatiContrApp;
                gvDatiContributivi.DataBind();

                ManageUpdateGridVittimeTerrorismoContributivo();

                RaiseHideAvviso(this, null);

                #endregion Elimina
            }
            else if (e.CommandName == "Edit")
            {
                this.modalitaEditContributivi.Value = "true";
                RaiseHideAvviso(this, null);
            }
            else if (e.CommandName == "Salva")
            {
                #region Salva

                if (!IsEmptyEditableRowContr((GridViewRow)((Control)e.CommandSource).NamingContainer))
                {
                    List<DatiContributiviLocal> listaDatiContrApp = new List<DatiContributiviLocal>();
                    foreach (GridViewRow rApp in ((GridView)sender).Rows)
                    {
                        string QuotaApp = string.Empty;
                        string CodGestioneApp = string.Empty;
                        string SettimaneApp = string.Empty;
                        string AmmontareContrApp = string.Empty;
                        string MontanteContrApp = string.Empty;
                        string PL_Quotac = string.Empty;
                        if (!IsEmptyEditableRowContr(rApp))
                        {
                            QuotaApp = ((DropDownList)rApp.FindControl(Utility.GetDescription(EnumControlli.DdlQuotaContr))).SelectedValue;
                            SettimaneApp = ((TextBox)rApp.FindControl(Utility.GetDescription(EnumControlli.TxtSettimaneContr))).Text;
                            AmmontareContrApp = ((TextBox)rApp.FindControl(Utility.GetDescription(EnumControlli.TxtAmmontareContr))).Text;
                            MontanteContrApp = ((TextBox)rApp.FindControl(Utility.GetDescription(EnumControlli.TxtMontanteContr))).Text;
                            CodGestioneApp = ((DropDownList)rApp.FindControl(Utility.GetDescription(EnumControlli.DdlCodiceGestioneContr))).SelectedValue;
                            PL_Quotac = ((HiddenField)rApp.FindControl(Utility.GetDescription(EnumControlli.LblPL_Quotac))).Value;
                            listaDatiContrApp = AddRecordContributivi(listaDatiContrApp, CodGestioneApp, QuotaApp, SettimaneApp, AmmontareContrApp, MontanteContrApp, PL_Quotac);
                        }
                        else if (!IsEmptyReadableRowContr(rApp))
                        {
                            QuotaApp = ((Label)rApp.FindControl(Utility.GetDescription(EnumControlli.LblQuotaContr))).Text;
                            SettimaneApp = ((Label)rApp.FindControl(Utility.GetDescription(EnumControlli.LblSettimaneContr))).Text;
                            AmmontareContrApp = ((Label)rApp.FindControl(Utility.GetDescription(EnumControlli.LblAmmontareContr))).Text;
                            MontanteContrApp = ((Label)rApp.FindControl(Utility.GetDescription(EnumControlli.LblMontanteContr))).Text;
                            CodGestioneApp = ((Label)rApp.FindControl(Utility.GetDescription(EnumControlli.LblIdCodiceGestioneContr))).Text;
                            PL_Quotac = ((HiddenField)rApp.FindControl(Utility.GetDescription(EnumControlli.LblPL_Quotac))).Value;
                            listaDatiContrApp = AddRecordContributivi(listaDatiContrApp, CodGestioneApp, QuotaApp, SettimaneApp, AmmontareContrApp, MontanteContrApp, PL_Quotac);
                        }
                    }
                    listaDatiContrApp.Add(new DatiContributiviLocal(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty));
                    gvDatiContributivi.EditIndex = -1;
                    ViewState[EnumViewState.ElencoDatiContributivi.ToString()] = listaDatiContrApp;
                    this.modalitaEditContributivi.Value = "false";
                    gvDatiContributivi.DataSource = listaDatiContrApp;
                    gvDatiContributivi.DataBind();
                    ManageUpdateGridVittimeTerrorismoContributivo();
                }

                RaiseHideAvviso(this, null);
                #endregion Salva
            }
            else if (e.CommandName == "Annulla")
            {
                #region Annulla
                List<DatiContributiviLocal> listaDatiContrApp = (List<DatiContributiviLocal>)ViewState[EnumViewState.ElencoDatiContributivi.ToString()];
                if (!IsListaEmpty(false))
                {
                    this.modalitaEditContributivi.Value = "false";
                    gvDatiContributivi.EditIndex = -1;
                    gvDatiContributivi.DataSource = listaDatiContrApp;
                    gvDatiContributivi.DataBind();
                }
                RaiseHideAvviso(this, null);
                #endregion Annulla
            }

            ManagePulsanti();
        }

        protected void gvDatiContributivi_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (this.domanda == null)
                    this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

                AreaTitolare.DatiPensione datiPensione = null;
                datiPensione = GetDatiPensione(this);

                //Render columns for UNICARPE
                if (e.Row.RowType == DataControlRowType.DataRow || e.Row.RowType == DataControlRowType.Header)
                {
                    if (((AreaDatiContributivi)ViewState[EnumViewState.AreaDatiContributiviAgo.ToString()]).DatiCalcolo.IsUnicarpe ||
                        (CodeUtility.IsRicostituzione(datiPensione) && Utility.IsDomandaDAI(domanda.Categoria) &&
                        domanda.Categoria.StartsWith("S") && datiPensione.NaturaPensione.Substring(2, 1) == "T") ||
                        disabilitaPannelloDatiCalcoloRicSdai() || (CodeUtility.IsRicostituzione(datiPensione) && !CodeUtility.IsRicostituzioneContributiva(datiPensione)))
                    {
                        e.Row.Cells[0].Visible = false;
                        e.Row.Cells[e.Row.Cells.Count - 2].Visible = false;
                    }
                }
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    var Bypass_LIMITE7_INTERI_MONT_AMM = ((AreaDatiContributivi)ViewState[EnumViewState.AreaDatiContributiviAgo.ToString()]).Bypass_LIMITE7_INTERI_MONT_AMM;
                    RegularExpressionValidator regularTxtAmmontareContributivo = (RegularExpressionValidator)e.Row.FindControl("regularTxtAmmontareContributivo");
                    RegularExpressionValidator regularTxtMontanteContributivo = (RegularExpressionValidator)e.Row.FindControl("regularTxtMontanteContributivo"); 
                    if (Bypass_LIMITE7_INTERI_MONT_AMM.GetValueOrDefault() && regularTxtAmmontareContributivo != null)
                    {
                        regularTxtAmmontareContributivo.Enabled = false;
                    }
                    if (Bypass_LIMITE7_INTERI_MONT_AMM.GetValueOrDefault() && regularTxtMontanteContributivo != null)
                    {
                        regularTxtMontanteContributivo.Enabled = false;
                    }

                    if ((((AreaDatiContributivi)ViewState[EnumViewState.AreaDatiContributiviAgo.ToString()]).DatiCalcolo.IsUnicarpe) || (CodeUtility.IsRicostituzione(datiPensione) && !CodeUtility.IsRicostituzioneContributiva(datiPensione)))  // sola lettura
                    {
                        gvDatiContributivi.EditIndex = -1;

                        ((Label)e.Row.FindControl(Utility.GetDescription(EnumControlli.LblQuotaContr))).Text = ((DatiContributiviLocal)(e.Row.DataItem)).Quota;
                        ((Label)e.Row.FindControl(Utility.GetDescription(EnumControlli.LblCodiceGestioneContr))).Text = GetValueFromIdContr(((DatiContributiviLocal)(e.Row.DataItem)).Gestione);
                        ((Label)e.Row.FindControl(Utility.GetDescription(EnumControlli.LblCodiceGestioneContr))).Attributes.Add("title", ((Label)e.Row.FindControl(Utility.GetDescription(EnumControlli.LblCodiceGestioneContr))).Text);
                        ((Label)e.Row.FindControl(Utility.GetDescription(EnumControlli.LblIdCodiceGestioneContr))).Text = ((DatiContributiviLocal)(e.Row.DataItem)).Gestione;
                        ((Label)e.Row.FindControl(Utility.GetDescription(EnumControlli.LblSettimaneContr))).Text = ((DatiContributiviLocal)(e.Row.DataItem)).Settimane;
                        ((Label)e.Row.FindControl(Utility.GetDescription(EnumControlli.LblAmmontareContr))).Text = !string.IsNullOrEmpty(((DatiContributiviLocal)(e.Row.DataItem)).AmmontareContributivo) ? CodeUtility.ConvertDecimalFixedPoint(((DatiContributiviLocal)(e.Row.DataItem)).AmmontareContributivo, 4) : "";
                        ((Label)e.Row.FindControl(Utility.GetDescription(EnumControlli.LblMontanteContr))).Text = !string.IsNullOrEmpty(((DatiContributiviLocal)(e.Row.DataItem)).MontanteContributivo) ? CodeUtility.ConvertDecimalFixedPoint(((DatiContributiviLocal)(e.Row.DataItem)).MontanteContributivo, 4) : "";
                        ((HiddenField)e.Row.FindControl(Utility.GetDescription(EnumControlli.LblPL_Quotac))).Value = ((DatiContributiviLocal)(e.Row.DataItem)).PL_Quotac;
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
                            else if (IsEmptyEditableRowContr(e.Row))
                            {
                                if (e.Row.Cells[0].Controls.Count == 3)  // edit mode
                                {
                                    GestioneDdls(e.Row, false);
                                    CodeUtility.EnableEditableMode(e.Row.Cells[0], "UCTabDatiCalcoloAgoContr", Page.Theme);
                                    LinkButton delete = ((LinkButton)(e.Row.FindControl(Utility.GetDescription(EnumControlli.BtnDeleteContributivi))));
                                    delete.Text = string.Empty;
                                    ((HiddenField)e.Row.FindControl(Utility.GetDescription(EnumControlli.LblPL_Quotac))).Value = ((DatiContributiviLocal)(e.Row.DataItem)).PL_Quotac;
                                }
                                else
                                {
                                    ((Label)e.Row.FindControl(Utility.GetDescription(EnumControlli.LblCodiceGestioneContr))).Text = GetValueFromIdContr(((DatiContributiviLocal)(e.Row.DataItem)).Gestione);
                                    ((Label)e.Row.FindControl(Utility.GetDescription(EnumControlli.LblCodiceGestioneContr))).Attributes.Add("title", ((Label)e.Row.FindControl(Utility.GetDescription(EnumControlli.LblCodiceGestioneContr))).Text);
                                    ((Label)e.Row.FindControl(Utility.GetDescription(EnumControlli.LblQuotaContr))).Text = ((DatiContributiviLocal)(e.Row.DataItem)).Quota;

                                    ((Label)e.Row.FindControl(Utility.GetDescription(EnumControlli.LblIdCodiceGestioneContr))).Text = ((DatiContributiviLocal)(e.Row.DataItem)).Gestione;
                                    ((Label)e.Row.FindControl(Utility.GetDescription(EnumControlli.LblSettimaneContr))).Text = ((DatiContributiviLocal)(e.Row.DataItem)).Settimane;
                                    ((Label)e.Row.FindControl(Utility.GetDescription(EnumControlli.LblAmmontareContr))).Text = !string.IsNullOrEmpty(((DatiContributiviLocal)(e.Row.DataItem)).AmmontareContributivo) ? CodeUtility.ConvertDecimalFixedPoint(((DatiContributiviLocal)(e.Row.DataItem)).AmmontareContributivo, 4) : "";
                                    ((Label)e.Row.FindControl(Utility.GetDescription(EnumControlli.LblMontanteContr))).Text = !string.IsNullOrEmpty(((DatiContributiviLocal)(e.Row.DataItem)).MontanteContributivo) ? CodeUtility.ConvertDecimalFixedPoint(((DatiContributiviLocal)(e.Row.DataItem)).MontanteContributivo, 4) : "";
                                    ((HiddenField)e.Row.FindControl(Utility.GetDescription(EnumControlli.LblPL_Quotac))).Value = ((DatiContributiviLocal)(e.Row.DataItem)).PL_Quotac;
                                    CodeUtility.EnableReadableMode(e.Row.Cells[0], e.Row.Cells[6], Page.Theme, Utility.GetDescription(EnumControlli.BtnDeleteContributivi));
                                }
                            }
                            else  //prima riga non vuota
                            {
                                if (e.Row.Cells[0].Controls.Count == 3)  // edit mode
                                {
                                    GestioneDdls(e.Row, false);
                                    CodeUtility.EnableEditableMode(e.Row.Cells[0], "UCTabDatiCalcoloAgoContr", Page.Theme);
                                    ((HiddenField)e.Row.FindControl(Utility.GetDescription(EnumControlli.LblPL_Quotac))).Value = ((DatiContributiviLocal)(e.Row.DataItem)).PL_Quotac;
                                }
                                else
                                {
                                    ((Label)e.Row.FindControl(Utility.GetDescription(EnumControlli.LblCodiceGestioneContr))).Text = GetValueFromIdContr(((DatiContributiviLocal)(e.Row.DataItem)).Gestione);
                                    ((Label)e.Row.FindControl(Utility.GetDescription(EnumControlli.LblCodiceGestioneContr))).Attributes.Add("title", ((Label)e.Row.FindControl(Utility.GetDescription(EnumControlli.LblCodiceGestioneContr))).Text);
                                    ((Label)e.Row.FindControl(Utility.GetDescription(EnumControlli.LblQuotaContr))).Text = ((DatiContributiviLocal)(e.Row.DataItem)).Quota;

                                    ((Label)e.Row.FindControl(Utility.GetDescription(EnumControlli.LblIdCodiceGestioneContr))).Text = ((DatiContributiviLocal)(e.Row.DataItem)).Gestione;
                                    ((Label)e.Row.FindControl(Utility.GetDescription(EnumControlli.LblSettimaneContr))).Text = ((DatiContributiviLocal)(e.Row.DataItem)).Settimane;
                                    ((Label)e.Row.FindControl(Utility.GetDescription(EnumControlli.LblAmmontareContr))).Text = CodeUtility.ConvertDecimalFixedPoint(((DatiContributiviLocal)(e.Row.DataItem)).AmmontareContributivo, 4);
                                    ((Label)e.Row.FindControl(Utility.GetDescription(EnumControlli.LblMontanteContr))).Text = CodeUtility.ConvertDecimalFixedPoint(((DatiContributiviLocal)(e.Row.DataItem)).MontanteContributivo, 4);
                                    ((HiddenField)e.Row.FindControl(Utility.GetDescription(EnumControlli.LblPL_Quotac))).Value = ((DatiContributiviLocal)(e.Row.DataItem)).PL_Quotac;
                                    CodeUtility.EnableReadableMode(e.Row.Cells[0], e.Row.Cells[6], Page.Theme, Utility.GetDescription(EnumControlli.BtnDeleteContributivi));
                                }
                            }
                        }
                        else  // righe successive
                        {
                            if (e.Row.Cells[0].Controls.Count == 3)  // edit mode
                            {
                                GestioneDdls(e.Row, false);
                                CodeUtility.EnableEditableMode(e.Row.Cells[0], "UCTabDatiCalcoloAgoContr", Page.Theme);
                                ((HiddenField)e.Row.FindControl(Utility.GetDescription(EnumControlli.LblPL_Quotac))).Value = ((DatiContributiviLocal)(e.Row.DataItem)).PL_Quotac;
                            }

                            else if (e.Row.DataItemIndex == ((List<DatiContributiviLocal>)ViewState[EnumViewState.ElencoDatiContributivi.ToString()]).Count - 1)
                            {
                                LinkButton add = ((LinkButton)(e.Row.Cells[0].Controls[0]));
                                add.Text = (string)"<img width=16 height=16 border=0 src=../App_themes/" + Page.Theme + "/Images/add24.png />";
                                add.ToolTip = "Aggiungi";
                            }
                            else
                            {
                                ((Label)e.Row.FindControl(Utility.GetDescription(EnumControlli.LblCodiceGestioneContr))).Text = GetValueFromIdContr(((DatiContributiviLocal)(e.Row.DataItem)).Gestione);
                                ((Label)e.Row.FindControl(Utility.GetDescription(EnumControlli.LblCodiceGestioneContr))).Attributes.Add("title", ((Label)e.Row.FindControl(Utility.GetDescription(EnumControlli.LblCodiceGestioneContr))).Text);
                                ((Label)e.Row.FindControl(Utility.GetDescription(EnumControlli.LblQuotaContr))).Text = ((DatiContributiviLocal)(e.Row.DataItem)).Quota;

                                ((Label)e.Row.FindControl(Utility.GetDescription(EnumControlli.LblIdCodiceGestioneContr))).Text = ((DatiContributiviLocal)(e.Row.DataItem)).Gestione;
                                ((Label)e.Row.FindControl(Utility.GetDescription(EnumControlli.LblSettimaneContr))).Text = ((DatiContributiviLocal)(e.Row.DataItem)).Settimane;
                                ((Label)e.Row.FindControl(Utility.GetDescription(EnumControlli.LblAmmontareContr))).Text = CodeUtility.ConvertDecimalFixedPoint(((DatiContributiviLocal)(e.Row.DataItem)).AmmontareContributivo, 4);
                                ((Label)e.Row.FindControl(Utility.GetDescription(EnumControlli.LblMontanteContr))).Text = CodeUtility.ConvertDecimalFixedPoint(((DatiContributiviLocal)(e.Row.DataItem)).MontanteContributivo, 4);
                                ((HiddenField)e.Row.FindControl(Utility.GetDescription(EnumControlli.LblPL_Quotac))).Value = ((DatiContributiviLocal)(e.Row.DataItem)).PL_Quotac;
                                CodeUtility.EnableReadableMode(e.Row.Cells[0], e.Row.Cells[6], Page.Theme, Utility.GetDescription(EnumControlli.BtnDeleteContributivi));
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
                throw new INPS.DNA.DnaApplicationException("UCDatiContributiviAgo, Errore nel metodo gvDatiContributivi_RowDataBound " + ex);
            }
        }

        protected void gvDatiContributivi_DataBound(object sender, EventArgs e)
        {
            ManagePulsanti();
        }
        #endregion gvDatiContributivi

        #region Events

        public event EventHandler ShowAvviso;
        public event EventHandler InitializeData;
        public event EventHandler HideAvviso;
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

        protected void RaiseShowAvviso(object sender, EventArgs e)
        {
            ShowAvviso(sender, e);
        }

        protected void RaiseInitializeData(object sender, EventArgs e)
        {
            InitializeData(sender, e);
        }

        protected void RaiseHideAvviso(object sender, EventArgs e)
        {
            if (HideAvviso != null)
                HideAvviso(sender, e);
        }
        #endregion Events

        #region Enum
        public enum EnumViewState
        {
            AreaDatiContributiviAgo,
            ListaCodeGestioneCalcoloRetrib,
            ListaCodeGestioneCalcoloContrib,
            ElencoDatiContributivi,
            ElencoDatiRetributivi,
            DecodificaTipoQuota,
            CtrlDecorrenzaRetrExINPDAI,
            IsPrimoRecordRetrGestioneS,
            DecorrenzaCalcoloRetr
        }

        public enum EnumControlli
        {
            [Description("lblCodiceGestione_item")]
            LblCodiceGestioneRetr,
            [Description("ddlCodiceGestione")]
            DdlCodiceGestioneRetr,
            [Description("lblQuota_item")]
            LblQuotaRetr,
            [Description("ddlQuota")]
            DdlQuotaRetr,
            [Description("lblTipoQuota_item")]
            LblTipoQuotaRetr,
            [Description("ddlTipoQuotaGestioneA")]
            DdlTipoQuotaRetrGestioneA,
            [Description("ddlTipoQuotaGestioneAltre")]
            DdlTipoQuotaRetrGestioneAltre,
            [Description("lblDecorrenza")]
            LblDecorrenzaRetr,
            [Description("lblSettimane")]
            LblSettimaneRetr,
            [Description("txtSettimaneRetributive")]
            TxtSettimaneRetr,
            [Description("lblRetribuzioneMedia")]
            LblRetribuzioneMediaRetr,
            [Description("txtRetribuzioneMedia")]
            TxtRetribuzioneMediaRetr,
            [Description("lblCodiceGestione_item")]
            LblCodiceGestioneContr,
            [Description("ddlCodiceGestione")]
            DdlCodiceGestioneContr,
            [Description("lblQuota_item")]
            LblQuotaContr,
            [Description("ddlQuota")]
            DdlQuotaContr,
            [Description("lblSettimane")]
            LblSettimaneContr,
            [Description("txtSettimaneContributive")]
            TxtSettimaneContr,
            [Description("lblAmmontareContributivo")]
            LblAmmontareContr,
            [Description("txtAmmontareContributivo")]
            TxtAmmontareContr,
            [Description("lblMontanteContributivo")]
            LblMontanteContr,
            [Description("txtMontanteContributivo")]
            TxtMontanteContr,
            [Description("lblIdCodeGestione")]
            LblIdCodiceGestioneRetr,
            [Description("lblIdCodeGestione")]
            LblIdCodiceGestioneContr,
            [Description("btnDeleteRetributivi")]
            BtnDeleteRetributivi,
            [Description("btnDeleteContributivi")]
            BtnDeleteContributivi,
            [Description("lblSettimane707")]
            LblSettimane707,
            [Description("txtSettimaneRetributive707")]
            TxtSettimaneRetributive707,
            [Description("RFVtxtSettimaneRetributive707")]
            RfvtxtSettimaneRetributive707,
            [Description("lblPL_Quotar")]
            LblPL_Quotar,
            [Description("lblPL_Quotar707")]
            LblPL_Quotar707,
            [Description("lblPL_Quotac")]
            LblPL_Quotac
        }

        public enum EnumColonneGv
        {
            GvRetrSett707 = 6
        }

        #endregion Enum

        #region nested Class
        [Serializable]
        public class DatiContributiviLocal
        {
            public DatiContributiviLocal()
            { }
            public DatiContributiviLocal(string strGestione, string strQuota, string strSettimane, string strAmmontareContributivo, string strMontanteContributivo, string strPL_Quotac = null)
            {
                this._strQuota = strQuota;
                this._strAmmontareContributivo = strAmmontareContributivo;
                this._strGestione = strGestione;
                this._strMontanteContributivo = strMontanteContributivo;
                this._strSettimane = strSettimane;
                this._strPL_Quotac = strPL_Quotac;
            }
            #region private properties
            private string _strQuota;
            private string _strGestione;
            private string _strSettimane;
            private string _strAmmontareContributivo;
            private string _strMontanteContributivo;
            private string _strPL_Quotac;
            #endregion private properties

            #region public properties
            public string Quota { get { return _strQuota; } set { _strQuota = value; } }
            public string Gestione { get { return _strGestione; } set { _strGestione = value; } }
            public string Settimane { get { return _strSettimane; } set { _strSettimane = value; } }
            public string AmmontareContributivo { get { return _strAmmontareContributivo; } set { _strAmmontareContributivo = value; } }
            public string MontanteContributivo { get { return _strMontanteContributivo; } set { _strMontanteContributivo = value; } }
            public string PL_Quotac { get { return _strPL_Quotac; } set { _strPL_Quotac = value; } }
            #endregion public properties

        }

        [Serializable]
        public class DatiRetributiviLocal
        {
            public DatiRetributiviLocal()
            { }
            public DatiRetributiviLocal(string strGestione, string strQuota, string strTipoQuota, string strSettimane, string strDecorrenza, string strRetribuzioneMedia, string strSettimane707, string strPL_Quotar = null, string strPL_Quotar707 = null)
            {
                this._strQuota = strQuota;
                this._strGestione = strGestione;
                this._strTipoQuota = strTipoQuota;
                this._strDecorrenza = strDecorrenza;
                this._strSettimane = strSettimane;
                this._strRetribuzioneMedia = strRetribuzioneMedia;
                this._settimane707 = strSettimane707;
                this._strPL_Quotar = strPL_Quotar;
                this._strPL_Quotar707 = strPL_Quotar707;
            }

            #region private properties
            private string _strGestione;
            private string _strQuota;
            private string _strTipoQuota;
            private string _strSettimane;
            private string _strDecorrenza;
            private string _strRetribuzioneMedia;
            private string _settimane707;
            private string _strPL_Quotar;
            private string _strPL_Quotar707;
            #endregion private properties

            #region public properties
            public string Gestione { get { return _strGestione; } set { _strGestione = value; } }
            public string Quota { get { return _strQuota; } set { _strQuota = value; } }
            public string TipoQuota { get { return _strTipoQuota; } set { _strTipoQuota = value; } }
            public string Settimane { get { return _strSettimane; } set { _strSettimane = value; } }
            public string Decorrenza { get { return _strDecorrenza; } set { _strDecorrenza = value; } }
            public string RetribuzioneMedia { get { return _strRetribuzioneMedia; } set { _strRetribuzioneMedia = value; } }
            public string Settimane707 { get { return _settimane707; } set { _settimane707 = value; } }
            public string PL_Quotar { get { return _strPL_Quotar; } set { _strPL_Quotar = value; } }
            public string PL_Quotar707 { get { return _strPL_Quotar707; } set { _strPL_Quotar707 = value; } }

            #endregion public properties

        }

        #endregion nested Class




    }
}
