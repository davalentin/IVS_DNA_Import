using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using INPS.Pensioni.LiquidazionePensione.Presenter;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using INPS.DNA;

namespace INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.Supplementi
{
    public partial class UCSupplementiAgoCI : CustomBaseUserControl, ISupplementi, ITitolarePensione
    {
        #region IViewUI
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        #endregion IViewUI

        #region ISup
        public long numDomanda { get; set; }
        public AreaSupplementi lstSupplementi { get; set; }
        public Presenter.SvrLiquidazione.AreaSupplementi risposta { get; set; }
        public AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda { get; set; }
        public DatiContribuzioneEnpals datiContribuzioneEnpals { get; set; }
        #endregion ISup

        #region ITitolarePensione
        public AreaTitolare TitolarePensione { get; set; }

        #endregion ITitolarePensione

        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.Visible)
            {
                if (this.domanda == null)
                    this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

                if (this.TitolarePensione == null)
                    this.TitolarePensione = new AreaTitolare();
                if (this.TitolarePensione.Pensione == null)
                    this.TitolarePensione.Pensione = GetDatiPensione(this);

                if (Page.IsPostBack)
                {
                    List<DatiSupplementi> elencoSupplementiContrib = new List<DatiSupplementi>();
                    List<DatiSupplementi> elencoSupplementiRetrib = new List<DatiSupplementi>();

                    elencoSupplementiRetrib = ViewState["elencoSupplementiRetrib"] as List<DatiSupplementi>;
                    if (elencoSupplementiRetrib == null)
                        elencoSupplementiRetrib = new List<DatiSupplementi>();
                    AddItemBlank(ref elencoSupplementiRetrib);

                    elencoSupplementiContrib = ViewState["elencoSupplementiContrib"] as List<DatiSupplementi>;
                    if (elencoSupplementiContrib == null)
                        elencoSupplementiContrib = new List<DatiSupplementi>();
                    AddItemBlank(ref elencoSupplementiContrib);
                }
            }
        }

        protected void btnSalvaTabSupplementi_Click(object sender, EventArgs e)
        {
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione.AreaEsito esito = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione.AreaEsito();
            PresenterSupplementi presenterSupplementi = new PresenterSupplementi();

            this.lstSupplementi = GetDatiUcSupplementi();
            presenterSupplementi.SalvaTabSupplementiByDomanda(this);

            if (!this.HasError)
            {
                List<DatiSupplementi> elencoSupplementiRetrib = ViewState["elencoSupplementiRetrib"] as List<DatiSupplementi>;
                List<DatiSupplementi> elencoSupplementiContrib = ViewState["elencoSupplementiContrib"] as List<DatiSupplementi>;
                List<DatiSupplementi> elencoSupplementiAnte96 = ViewState["elencoSupplementiAnte96"] as List<DatiSupplementi>;
                AddItemBlank(ref elencoSupplementiRetrib);
                AddItemBlank(ref elencoSupplementiContrib);
                AddItemBlank(ref elencoSupplementiAnte96);
                ViewState["elencoSupplementiRetrib"] = elencoSupplementiRetrib;
                ViewState["elencoSupplementiContrib"] = elencoSupplementiContrib;
                ViewState["elencoSupplementiAnte96"] = elencoSupplementiAnte96;

                GvSupplementi_Load();
                GvSupplementiContributivi_Load();
                GvSupplementiAnte96_Load();

                ManageButtons(elencoSupplementiContrib);
            }
            else
            {
                esito.Messaggio = this.ErrorMessage;
                esito.RisultatoOperazione = INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione.AreaEsito.TipoEsito.KO;
            }

            Utility.CustomEventArgs Cevent = new Utility.CustomEventArgs(this.domanda.TipoAppartenenza.Value, null);
            RaiseSalvaSupplementi(this, Cevent);
        }

        protected void btnEliminaTabSupplementi_Click(object sender, EventArgs e)
        {
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione.AreaEsito esito = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione.AreaEsito();

            PresenterSupplementi presenterSupplementi = new PresenterSupplementi();
            presenterSupplementi.EliminaTabSupplementiByDomanda(this);

            //ENG - MEMO 50/2023 
            string valoreControlloMemo50_2023 = string.Empty;
            if (ViewState["AbilitazioneMemo50_2023"] != null)
                valoreControlloMemo50_2023 = (string)ViewState["AbilitazioneMemo50_2023"];
            else
            {
                Presenter.PresenterControlliDinamici presenter = new PresenterControlliDinamici();
                Presenter.SvrLiquidazione.AreaEsito esitoControllo = presenter.GetControlloDinamicoByNomeControllo("AbilitazioneMemo50_2023", out valoreControlloMemo50_2023);
                if (esitoControllo != null && esitoControllo.RisultatoOperazione == Presenter.SvrLiquidazione.AreaEsito.TipoEsito.OK
                    && !String.IsNullOrEmpty(valoreControlloMemo50_2023) && !String.IsNullOrEmpty(valoreControlloMemo50_2023.Trim()))
                    ViewState["AbilitazioneMemo50_2023"] = valoreControlloMemo50_2023.Trim();
            }

            if (this.HasError == true)
            {
                if (ViewState["AbilitazioneMemo50_2023"] != null && (string)ViewState["AbilitazioneMemo50_2023"] == "SI" && this.domanda.TipoAppartenenza.GetValueOrDefault() == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.AGO &&
                    Utility.IsRicostituzione_MotiviContributivi(this.TitolarePensione.Pensione) && this.TitolarePensione.Pensione.CodeTipo == "0001" && !this.domanda.IsDomandaENPALS && !Utility.IsDomandaCumulo(this.domanda.Categoria))
                {
                    esito.Messaggio = this.ErrorMessage;
                    esito.RisultatoOperazione = INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione.AreaEsito.TipoEsito.KO;
                }
                else
                    this.ErrorMessage = "Errore durante l'eliminazione dei Supplementi";
            }
            else
            {
                ClearForm();
                ValorizzaEtichette(this);
            }

            Utility.CustomEventArgs Cevent = new Utility.CustomEventArgs(this.domanda.TipoAppartenenza.Value, null);
            RaiseEliminaSupplementi(this, Cevent);
        }

        internal AreaSupplementi GetDatiUcSupplementi()
        {
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            try
            {
                List<DatiSupplementi> elencoSupplementiRetrib = ViewState["elencoSupplementiRetrib"] as List<DatiSupplementi>;
                List<DatiSupplementi> elencoSupplementiContrib = ViewState["elencoSupplementiContrib"] as List<DatiSupplementi>;
                List<DatiSupplementi> elencoSupplementiAnte96 = ViewState["elencoSupplementiAnte96"] as List<DatiSupplementi>;
                removeItemBlank(ref elencoSupplementiRetrib);
                removeItemBlank(ref elencoSupplementiContrib);
                removeItemBlank(ref elencoSupplementiAnte96);

                List<DatiSupplementi> elencoSupplementiToSave = new List<DatiSupplementi>();
                if (elencoSupplementiRetrib != null && elencoSupplementiRetrib.Count > 0)
                    elencoSupplementiToSave.AddRange(elencoSupplementiRetrib);
                if (elencoSupplementiContrib != null && elencoSupplementiContrib.Count > 0)
                    elencoSupplementiToSave.AddRange(elencoSupplementiContrib);
                if (elencoSupplementiAnte96 != null && elencoSupplementiAnte96.Count > 0)
                    elencoSupplementiToSave.AddRange(elencoSupplementiAnte96);

                lstSupplementi = new AreaSupplementi();
                lstSupplementi.ListDatiSupplementi = elencoSupplementiToSave.ToArray();

                if (this.domanda.TipoAppartenenza == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.AGO)
                {
                    lstSupplementi.SupplementiBase = new SupplementiBase();
                    lstSupplementi.IntegrazioneArt11 = new IntegrazioneArt11();

                    if (string.IsNullOrEmpty(txtRenditaFacolOrdinaria.Text))
                        lstSupplementi.SupplementiBase.RenditaFacoltativaOrdinaria = null;
                    else
                        lstSupplementi.SupplementiBase.RenditaFacoltativaOrdinaria = decimal.Parse(txtRenditaFacolOrdinaria.Text);

                    lstSupplementi.SupplementiBase.RenditaFacoltativaConvenzionale = !string.IsNullOrEmpty(txtRenditafacolConv.Text) ? decimal.Parse(txtRenditafacolConv.Text) : (decimal?)null;

                    if (string.IsNullOrEmpty(txtDecorrenza.Text) || txtDecorrenza.Text.Equals("MM/AAAA"))
                        lstSupplementi.IntegrazioneArt11.Decorrenza = null;
                    else
                        lstSupplementi.IntegrazioneArt11.Decorrenza = Utility.GetDateFromString(txtDecorrenza.Text);

                    if (string.IsNullOrEmpty(txtImportoIVS.Text))
                        lstSupplementi.IntegrazioneArt11.ImportoIVS = null;
                    else
                        lstSupplementi.IntegrazioneArt11.ImportoIVS = decimal.Parse(txtImportoIVS.Text);
                }

                return lstSupplementi;
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCSupplementi, Errore nel metodo GetDatiUcSupplementi " + ex);
            }
        }

        protected object IsAGO()
        {
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            if (this.domanda.TipoAppartenenza == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.AGO)
                return true;

            return false;
        }

        protected object IsCI()
        {
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            if (this.domanda.TipoAppartenenza == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.CI)
                return true;

            return false;
        }

        #region Private Methods Common

        private void RenderControls(AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp tipoApp, AreaSupplementi areaSupplementi)
        {
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            if (this.TitolarePensione == null)
                this.TitolarePensione = new AreaTitolare();
            if (this.TitolarePensione.Pensione == null)
                this.TitolarePensione.Pensione = GetDatiPensione(this);

            if (tipoApp == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.AGO)
            {
                pnlIntegrazioneArt11.Visible = true;
                pnlRenditafacolConv.Visible = true;

                if (Utility.IsDomandaAUT(this.domanda.Categoria))
                {
                    txtRenditaFacolOrdinaria.Enabled = false;
                    txtRenditafacolConv.Enabled = false;
                    pnlIntegrazioneArt11.Visible = false;
                    pnlRenditafacolConv.Visible = false;
                }

                if (Utility.IsDomandaRipristino(this.TitolarePensione.Pensione) ||
                    (domanda.TipoAppartenenza.HasValue && domanda.TipoAppartenenza.Value == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.AGO && Utility.IsDomandaUnicarpe(this.TitolarePensione.Pensione, true) == Utility.TipoUnicarpe.Automatica && Utility.IsRicostituzione_MotiviContributivi(this.TitolarePensione.Pensione)))
                    pnlIntegrazioneArt11.Enabled = false;
            }

            if (((bool?)ViewState[EnumViewState.IsDomandaSperimentaleDonna.ToString()]).GetValueOrDefault() || Utility.IsDomandaAUT(this.domanda.Categoria) ||
                CodeUtility.IsContributivaPura(this.TitolarePensione.Pensione))
                pnlSupplementiAGOCIRetrib.Visible = false;

            if (Utility.IsDomandaRiliquidazioneIndiretta(this.TitolarePensione.Pensione) && (!string.IsNullOrEmpty(this.domanda.Categoria) && this.domanda.Categoria.Trim().ToUpperInvariant() != "SO"))
            {
                pnlSupplementi.Enabled = false;
            }
        }

        private void ValorizzaEtichetteAGO(AreaSupplementi risposta)
        {
            if (risposta != null)
            {
                if (this.domanda == null)
                    this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

                if (this.TitolarePensione == null)
                    this.TitolarePensione = new AreaTitolare();
                if (this.TitolarePensione.Pensione == null)
                    this.TitolarePensione.Pensione = GetDatiPensione(this);

                if (risposta.IntegrazioneArt11 != null)
                {
                    if (risposta.IntegrazioneArt11.Decorrenza.HasValue)
                        txtDecorrenza.Text = String.Format("{0:MM/yyyy}", risposta.IntegrazioneArt11.Decorrenza.Value);

                    if (risposta.IntegrazioneArt11.ImportoIVS.HasValue)
                        txtImportoIVS.Text = risposta.IntegrazioneArt11.ImportoIVS.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture);
                }
                else
                {
                    txtDecorrenza.Text = "MM/AAAA";
                    txtImportoIVS.Text = string.Empty;
                }

                if (risposta.SupplementiBase != null)
                {
                    if (risposta.SupplementiBase.RenditaFacoltativaOrdinaria.HasValue)
                        txtRenditaFacolOrdinaria.Text = Convert.ToString(risposta.SupplementiBase.RenditaFacoltativaOrdinaria.Value);

                    if (risposta.SupplementiBase.RenditaFacoltativaConvenzionale.HasValue)
                        txtRenditafacolConv.Text = Convert.ToString(risposta.SupplementiBase.RenditaFacoltativaConvenzionale.Value);
                }
                else
                {
                    txtRenditaFacolOrdinaria.Text = string.Empty;
                    txtRenditafacolConv.Text = string.Empty;
                }

                if (CodeUtility.IsRicostituzione(this.TitolarePensione.Pensione) && (Utility.IsDomandaFPLD(this.domanda.Categoria) || Utility.IsDomandaGestioneAutonomi(this.domanda.Categoria) ||
                    Utility.IsDomandaDAI(this.domanda.Categoria) || Utility.IsDomandaAUT(this.domanda.Categoria)))
                {
                    if ((CodeUtility.IsRicostituzioneSupplemento(this.TitolarePensione.Pensione) || CodeUtility.IsRicostituzioneContributiva(this.TitolarePensione.Pensione) || Utility.IsRicostituzione_MotiviDocumentali(this.TitolarePensione.Pensione)))
                    {
                        if (risposta.ListDatiSupplementi != null && risposta.ListDatiSupplementi.Count() > 0 && risposta.ListDatiSupplementi.Where(x => x.IsFromPrelievo == true).Any())
                        {
                            if (!Utility.IsRicostituzione_MotiviDocumentali(this.TitolarePensione.Pensione))
                                lblSuppFromPrelievo.Visible = true;
                        }
                    }
                    else
                    {
                        if (!Utility.IsRicostituzione_AccreditoPeriodiMaternita(this.TitolarePensione.Pensione))
                            lblRicNonContribNonSuppNonDoc.Visible = true;

                        btnEliminaTabSupplementi.Enabled = false;
                        pnlIntegrazioneArt11.Enabled = false;
                    }
                }

            }
            else
            {
                txtDecorrenza.Text = "MM/AAAA";
                txtImportoIVS.Text = string.Empty;
                txtRenditaFacolOrdinaria.Text = string.Empty;
                txtRenditafacolConv.Text = string.Empty;
            }

            if (this.TitolarePensione == null)
                this.TitolarePensione = new AreaTitolare();
            if (this.TitolarePensione.Pensione == null)
                this.TitolarePensione.Pensione = GetDatiPensione(this);

            if (Utility.IsDomandaUnicarpe(this.TitolarePensione.Pensione, true) == Utility.TipoUnicarpe.Automatica && CodeUtility.IsRicostituzione(this.TitolarePensione.Pensione) && ((domanda.TipoAppartenenza.HasValue && domanda.TipoAppartenenza.Value != AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.AGO) || !Utility.IsRicostituzione_MotiviContributivi(this.TitolarePensione.Pensione)))
            {
                pnlSupplementi.Enabled = false;
            }

            if (Utility.IsDomandaRipristino(this.TitolarePensione.Pensione))
            {
                btnEliminaTabSupplementi.Enabled = false;
            }
        }

        private void EnableReadableMode(TableCell cell_Edit, TableCell cell_Delete)
        {
            cell_Edit.Width = new Unit(40, UnitType.Pixel);
            LinkButton edit = ((LinkButton)(cell_Edit.Controls[0]));
            edit.Text = "<img width=20 height=20 border=0 src=../App_themes/" + Page.Theme + "/Images/pencil.png />";
            edit.ToolTip = "Modifica";

            LinkButton delete = ((LinkButton)(cell_Delete.FindControl("btnDelete")));
            delete.Text = "<img width=20 height=20 border=0 src=../App_themes/" + Page.Theme + "/Images/delete24.png />";
        }

        private void EnableReadableModeWithoutDelete(TableCell cell_Edit, TableCell cell_Delete)
        {
            cell_Edit.Width = new Unit(40, UnitType.Pixel);
            LinkButton edit = ((LinkButton)(cell_Edit.Controls[0]));
            edit.Text = "<img width=20 height=20 border=0 src=../App_themes/" + Page.Theme + "/Images/pencil.png />";
            edit.ToolTip = "Modifica";
        }

        private void GestioneTastoSalva()
        {
            List<DatiSupplementi> elencoSupplementiRetrib = ViewState["elencoSupplementiRetrib"] as List<DatiSupplementi>;
            List<DatiSupplementi> elencoSupplementiContrib = ViewState["elencoSupplementiContrib"] as List<DatiSupplementi>;

            if (modalitaEdit.Value == "true")
            {
                if (elencoSupplementiRetrib.Count > 1)
                {
                    btnSalvaTabSupplementi.Enabled = false;
                    RaiseDisabilitaTastoSalva(this, null);
                }
                else
                {
                    if (modalitaEditContrib.Value == "true")
                    {
                        btnSalvaTabSupplementi.Enabled = false;
                        RaiseDisabilitaTastoSalva(this, null);
                    }
                    else
                    {
                        btnSalvaTabSupplementi.Enabled = true;
                        RaiseAbilitaTastoSalva(this, null);
                    }
                }
            }
            else
            {
                if (modalitaEditContrib.Value == "true")
                {
                    if (elencoSupplementiContrib.Count > 1)
                    {
                        btnSalvaTabSupplementi.Enabled = false;
                        RaiseDisabilitaTastoSalva(this, null);
                    }
                    else
                    {
                        if (((bool?)ViewState[EnumViewState.IsDomandaSperimentaleDonna.ToString()]).GetValueOrDefault())
                        {
                            btnSalvaTabSupplementi.Enabled = false;
                            RaiseDisabilitaTastoSalva(this, null);
                        }
                        else
                        {
                            btnSalvaTabSupplementi.Enabled = true;
                            RaiseAbilitaTastoSalva(this, null);
                        }
                    }
                }
                else
                {
                    btnSalvaTabSupplementi.Enabled = true;
                    RaiseAbilitaTastoSalva(this, null);
                }
            }
        }

        internal void ValorizzaEtichette(ISupplementi supplementi)
        {
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            List<DatiSupplementi> elencoSupplementiContrib = new List<DatiSupplementi>();
            List<DatiSupplementi> elencoSupplementiRetrib = new List<DatiSupplementi>();
            List<DatiSupplementi> elencoSupplementiAnte96 = new List<DatiSupplementi>();

            bool isDomandaSperDonna = supplementi.risposta.IsDomandaSperimentaleDonna;
            ViewState[EnumViewState.IsDomandaSperimentaleDonna.ToString()] = isDomandaSperDonna;

            bool isReversibilitaOrRicostituzione = supplementi.risposta.IsReversibilitaOrRicostituzione;
            ViewState[EnumViewState.IsReversibilitaOrRicostituzione.ToString()] = isReversibilitaOrRicostituzione;

            bool IsPannelloSupplementiAnte96 = supplementi.risposta.IsPannelloSupplementiAnte96;
            ViewState[EnumViewState.IsPannelloSupplementiAnte96.ToString()] = IsPannelloSupplementiAnte96;

            bool IsTipoCalcoloModificato = supplementi.risposta.IsTipoCalcoloModificato.GetValueOrDefault();
            ViewState[EnumViewState.IsTipoCalcoloModificato.ToString()] = IsTipoCalcoloModificato;

            try
            {
                if (domanda.TipoAppartenenza.HasValue)
                {
                    RenderControls(domanda.TipoAppartenenza.Value, this.risposta);

                    if (domanda.TipoAppartenenza.Value == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.AGO)
                        ValorizzaEtichetteAGO(supplementi.risposta);
                }

                if (supplementi.risposta != null)
                {
                    ViewState["elencoTipoSupplementi"] = supplementi.risposta.ListTipoSupplementi.ToList();
                    ViewState[EnumViewState.DecodificaTipoQuota.ToString()] = supplementi.risposta.ListaDecodificaTipoQuota.ToList();

                    if (IsPannelloSupplementiAnte96)
                    {
                        foreach (DatiSupplementi datiSupp in supplementi.risposta.ListDatiSupplementi)
                        {
                            elencoSupplementiAnte96.Add(datiSupp);
                            datiSupp.NSettimaneSupplemento = datiSupp.NSettimaneSupplemento.HasValue ? datiSupp.NSettimaneSupplemento.Value : 0;
                        }
                    }
                    else
                    {
                        foreach (DatiSupplementi datiSupp in supplementi.risposta.ListDatiSupplementi)
                        {
                            switch (datiSupp.TipoSupplemento)
                            {
                                case 'C':
                                    elencoSupplementiContrib.Add(datiSupp);
                                    break;
                                case 'R':
                                    elencoSupplementiRetrib.Add(datiSupp);
                                    break;
                            }
                        }
                    }
                }

                if (IsPannelloSupplementiAnte96)
                {
                    AddItemBlank(ref elencoSupplementiAnte96);
                    ViewState["elencoSupplementiAnte96"] = elencoSupplementiAnte96;
                    GvSupplementiAnte96_Load();
                    ManageButtons(elencoSupplementiAnte96);
                    btnEliminaTabSupplementi.Enabled = false;
                    btnSalvaTabSupplementi.Enabled = false;
                    RaiseDisabilitaTastoSalva(this, null);
                    pnlIntegrazioneArt11.Enabled = false;
                    pnlSupplementiAGOCIContrib.Visible = false;
                    pnlSupplementiAGOCIRetrib.Visible = false;
                }
                else
                {
                    if (!isDomandaSperDonna && !Utility.IsDomandaAUT(this.domanda.Categoria))
                    {
                        AddItemBlank(ref elencoSupplementiRetrib);
                        ViewState["elencoSupplementiRetrib"] = elencoSupplementiRetrib;
                        GvSupplementi_Load();
                    }

                    AddItemBlank(ref elencoSupplementiContrib);
                    ViewState["elencoSupplementiContrib"] = elencoSupplementiContrib;
                    GvSupplementiContributivi_Load();
                }
            }
            catch (DNA.DnaApplicationException)
            {
                ManageButtons(elencoSupplementiContrib);
                throw;
            }

            ManageButtons(elencoSupplementiContrib);
        }

        private void LoadDecodificaData(ISupplementi areaDatiSupplementi)
        {
            //ViewState[EnumViewState.ListaCodeGestioneCalcoloRetrib.ToString()] = areaDatiSupplementi.areaDatiContributiviAgo.listaDecodificaGestioneCalcoloRetributivo.ToList();
            //ViewState[EnumViewState.ListaCodeGestioneCalcoloContrib.ToString()] = areaDatiSupplementi.areaDatiContributiviAgo.listaDecodificaGestioneCalcoloContributivo.ToList();
            //if (areaDatiSupplementi.areaDatiContributiviAgo.DatiExINPDAI != null)
            //{
            //    ViewState[EnumViewState.DecodificaTipoQuota.ToString()] = areaDatiSupplementi.areaDatiContributiviAgo.DatiExINPDAI.DecodificaTipoQuota.ToList();
            //    ViewState[EnumViewState.CtrlDecorrenzaRetrExINPDAI.ToString()] = areaDatiSupplementi.areaDatiContributiviAgo.DatiExINPDAI.CtrlDecorrenzaRetrExINPDAI.ToList();
            //}
        }

        private void ManageButtons(List<DatiSupplementi> listaDatiSupplementi)
        {
            if (this.TitolarePensione == null)
                this.TitolarePensione = new AreaTitolare();
            if (this.TitolarePensione.Pensione == null)
                this.TitolarePensione.Pensione = GetDatiPensione(this);

            if (this.TitolarePensione.Pensione.TipoLetturaUnicarpe != 'L')
            {
                if (listaDatiSupplementi != null && listaDatiSupplementi.Count > 0)
                {
                    if (listaDatiSupplementi.Exists(x => x.CodiceLiquidazione.GetValueOrDefault() == 3 && x.AmmontareContributivo.GetValueOrDefault() > x.MontanteSupplemento.GetValueOrDefault() &&
                        x.DecorrenzaSupplemento.HasValue && x.DecorrenzaSupplemento.Value.CompareTo(new DateTime(2015, 01, 01)) >= 0))
                    {
                        btnPopUp.Style.Remove("display");
                        btnSalvaTabSupplementi.Style.Remove("display");
                        btnSalvaTabSupplementi.Style.Add("display", "none");

                        RaiseShowPopUp(this, null);
                        return;
                    }
                }
            }

            btnPopUp.Style.Remove("display");
            btnSalvaTabSupplementi.Style.Remove("display");
            btnPopUp.Style.Add("display", "none");
            RaiseHidePopUp(this, null);
        }

        private void ClearForm()
        {
            CodeUtility.ClearForm(this, 0);
        }
        #endregion Private Methods Common

        #region Grid Retributivi

        protected void gvSupplementi_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (this.domanda == null)
                    this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

                if (this.TitolarePensione == null)
                    this.TitolarePensione = new AreaTitolare();
                if (this.TitolarePensione.Pensione == null)
                    this.TitolarePensione.Pensione = GetDatiPensione(this);
                List<DatiSupplementi> elencoSupplementiRetrib = new List<DatiSupplementi>();

                elencoSupplementiRetrib = ViewState["elencoSupplementiRetrib"] as List<DatiSupplementi>;

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
                bool isRicSupplementoTipoRetributivoMemo50 = false;

                if ((e.Row.RowType == DataControlRowType.DataRow || e.Row.RowType == DataControlRowType.Header) && (this.domanda == null || !Utility.IsDomandaDAI(this.domanda.Categoria)))
                {
                    e.Row.Cells[3].Visible = false;
                }
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    setDdl(e);
                    int num = ((List<DatiSupplementi>)ViewState["elencoSupplementiRetrib"]).Count;

                    if (ViewState["AbilitazioneMemo50_2023"] != null && (string)ViewState["AbilitazioneMemo50_2023"] == "SI" && this.domanda.TipoAppartenenza.GetValueOrDefault() == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.AGO &&
                        Utility.IsRicostituzione_Supplemento(this.TitolarePensione.Pensione) && !this.domanda.IsDomandaENPALS && !Utility.IsDomandaCumulo(this.domanda.Categoria) && elencoSupplementiRetrib != null && elencoSupplementiRetrib.Count() > 0)
                    {
                        DatiSupplementi supplementoRetributivo = elencoSupplementiRetrib.ElementAt(e.Row.DataItemIndex);
                        if (supplementoRetributivo.IsFromPrelievo == true && !elencoSupplementiRetrib.Exists(x => x.IsFromPrelievo == false && !string.IsNullOrEmpty(x.CodGestioneSupplemento)))
                            isRicSupplementoTipoRetributivoMemo50 = true;
                    }

                    //if (Utility.IsDomandaBancari(this.domanda.SiglaCategoriaPensione))
                    //{
                    //    DropDownList ddlquota = ((DropDownList)e.Row.FindControl("ddlQuotaSupplementi"));
                    //    if (ddlquota != null)
                    //        ddlquota.Enabled = false;
                    //}

                    if (ViewState["AbilitazioneMemo50_2023"] != null && (string)ViewState["AbilitazioneMemo50_2023"] == "SI" && this.domanda.TipoAppartenenza.GetValueOrDefault() == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.AGO &&
                        Utility.IsRicostituzione_MotiviDocumentali(this.TitolarePensione.Pensione) && !this.domanda.IsDomandaENPALS && !Utility.IsDomandaCumulo(this.domanda.Categoria) && elencoSupplementiRetrib != null && elencoSupplementiRetrib.Count() > 0)  // sola lettura
                    {
                        gvSupplementi.EditIndex = -1;

                        ((Label)e.Row.FindControl("lblTipoSupplementi")).Text = ((DatiSupplementi)(e.Row.DataItem)).CodGestioneSupplemento;
                        ((Label)e.Row.FindControl("lblQuotaSupplementi")).Text = ((DatiSupplementi)(e.Row.DataItem)).QuotaSupplemento.HasValue ? ((DatiSupplementi)(e.Row.DataItem)).QuotaSupplemento.ToString() : string.Empty;
                        ((Label)e.Row.FindControl("lblTipoQuotaSupplementi")).Text = ((DatiSupplementi)(e.Row.DataItem)).CodTipoQuota;
                        ((Label)e.Row.FindControl("lblDecorrenzaSupplementi")).Text = String.Format("{0:MM/yyyy}", ((DatiSupplementi)(e.Row.DataItem)).DecorrenzaSupplemento);
                        ((Label)e.Row.FindControl("lblSettimaneSupplementi")).Text = ((DatiSupplementi)(e.Row.DataItem)).NSettimaneSupplemento.HasValue ? ((DatiSupplementi)(e.Row.DataItem)).NSettimaneSupplemento.ToString() : string.Empty;
                        ((Label)e.Row.FindControl("lblRMSSupplementi")).Text = ((DatiSupplementi)(e.Row.DataItem)).RMSSupplemento.HasValue ? ((DatiSupplementi)(e.Row.DataItem)).RMSSupplemento.Value.ToString("0.0000") : string.Empty;
                        ((Label)e.Row.FindControl("lblCodLiqSupplementi")).Text = ((DatiSupplementi)(e.Row.DataItem)).CodiceLiquidazione.HasValue ? ((DatiSupplementi)(e.Row.DataItem)).CodiceLiquidazione.ToString() : string.Empty;
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
                            if (IsListaEmpty() && !Convert.ToBoolean(modalitaEdit.Value))
                            {
                                gvSupplementi.EditIndex = 0;
                                modalitaEdit.Value = "true";
                                GvSupplementi_Load();
                                GestioneTastoSalva();
                            }
                            else
                            {
                                if (e.Row.Cells[0].Controls.Count == 3)  // edit mode
                                {
                                    setCampiEdit(e);
                                    EnableEditableMode(e.Row.Cells[0]);
                                }
                                else
                                {
                                    ((Label)e.Row.FindControl("lblTipoSupplementi")).Text = ((DatiSupplementi)(e.Row.DataItem)).CodGestioneSupplemento;
                                    ((Label)e.Row.FindControl("lblQuotaSupplementi")).Text = ((DatiSupplementi)(e.Row.DataItem)).QuotaSupplemento.HasValue ? ((DatiSupplementi)(e.Row.DataItem)).QuotaSupplemento.ToString() : string.Empty;
                                    ((Label)e.Row.FindControl("lblTipoQuotaSupplementi")).Text = ((DatiSupplementi)(e.Row.DataItem)).CodTipoQuota;
                                    ((Label)e.Row.FindControl("lblDecorrenzaSupplementi")).Text = String.Format("{0:MM/yyyy}", ((DatiSupplementi)(e.Row.DataItem)).DecorrenzaSupplemento);
                                    ((Label)e.Row.FindControl("lblSettimaneSupplementi")).Text = ((DatiSupplementi)(e.Row.DataItem)).NSettimaneSupplemento.HasValue ? ((DatiSupplementi)(e.Row.DataItem)).NSettimaneSupplemento.ToString() : string.Empty;
                                    if (this.domanda.TipoAppartenenza == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.AGO)
                                        ((Label)e.Row.FindControl("lblRMSSupplementi")).Text = ((DatiSupplementi)(e.Row.DataItem)).RMSSupplemento.HasValue ? ((DatiSupplementi)(e.Row.DataItem)).RMSSupplemento.Value.ToString("0.0000") : string.Empty;
                                    else if (this.domanda.TipoAppartenenza == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.CI)
                                        ((Label)e.Row.FindControl("lblRMSSupplementi")).Text = ((DatiSupplementi)(e.Row.DataItem)).RMSSupplemento.HasValue ? ((DatiSupplementi)(e.Row.DataItem)).RMSSupplemento.Value.ToString("0.000000") : string.Empty;
                                    ((Label)e.Row.FindControl("lblCodLiqSupplementi")).Text = ((DatiSupplementi)(e.Row.DataItem)).CodiceLiquidazione.HasValue ? ((DatiSupplementi)(e.Row.DataItem)).CodiceLiquidazione.ToString() : string.Empty;

                                    if (CodeUtility.IsRicostituzione(this.TitolarePensione.Pensione) && (Utility.IsDomandaFPLD(this.domanda.Categoria) || Utility.IsDomandaGestioneAutonomi(this.domanda.Categoria) ||
                                        Utility.IsDomandaDAI(this.domanda.Categoria) || Utility.IsDomandaAUT(this.domanda.Categoria)))
                                    {
                                        if (((CodeUtility.IsRicostituzioneSupplemento(this.TitolarePensione.Pensione) && !isRicSupplementoTipoRetributivoMemo50) || CodeUtility.IsRicostituzioneContributiva(this.TitolarePensione.Pensione) || Utility.IsRicostituzione_MotiviDocumentali(this.TitolarePensione.Pensione) || Utility.IsRicostituzione_AccreditoPeriodiMaternita(this.TitolarePensione.Pensione)))
                                            EnableReadableMode(e.Row.Cells[0], e.Row.Cells[7]);
                                        else
                                        {
                                            LinkButton button = ((LinkButton)(e.Row.Cells[0].Controls[0]));
                                            button.Enabled = false;
                                            button.Text = "&nbsp;&nbsp;&nbsp;";
                                        }
                                    }
                                    else
                                        EnableReadableMode(e.Row.Cells[0], e.Row.Cells[7]);

                                    if (isRicSupplementoTipoRetributivoMemo50)
                                    {
                                        LinkButton button = ((LinkButton)(e.Row.Cells[0].Controls[0]));
                                        button.Enabled = false;
                                        button.Text = "&nbsp;&nbsp;&nbsp;";
                                    }
                                }
                            }
                        }
                        else // righe successive
                        {
                            if (e.Row.Cells[0].Controls.Count == 3)  // edit mode
                            {
                                setCampiEdit(e);
                                EnableEditableMode(e.Row.Cells[0]);

                                //ENG - MEMO 50/2023
                                if (ViewState["AbilitazioneMemo50_2023"] != null && (string)ViewState["AbilitazioneMemo50_2023"] == "SI" && this.domanda.TipoAppartenenza.GetValueOrDefault() == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.AGO &&
                                    Utility.IsRicostituzione_MotiviContributivi(TitolarePensione.Pensione) && TitolarePensione.Pensione.CodeTipo == "0001" && !this.domanda.IsDomandaENPALS && !Utility.IsDomandaCumulo(this.domanda.Categoria) && !(bool)ViewState[EnumViewState.IsTipoCalcoloModificato.ToString()])
                                {
                                    if (elencoSupplementiRetrib != null && elencoSupplementiRetrib.Count() > 0)
                                    {
                                        DatiSupplementi supplemento = elencoSupplementiRetrib.ElementAt(e.Row.DataItemIndex);
                                        if (!supplemento.IsFromPrelievo)
                                        {
                                            this.HasError = true;
                                            this.ErrorMessage = "L'inserimento di un nuovo supplemento è consentito solo per le domande di Ricostituzione per supplemento";
                                            RaiseShowAvviso(this, null);
                                            return;
                                        }
                                    }
                                }
                            }

                            else if (e.Row.DataItemIndex == num - 1 && IsItemBlankPresent(elencoSupplementiRetrib))
                            {
                                LinkButton add = ((LinkButton)(e.Row.Cells[0].Controls[0]));
                                add.Text = (string)"<img width=16 height=16 border=0 src=../App_themes/" + Page.Theme + "/Images/add24.png />";
                                add.ToolTip = "Aggiungi";
                            }
                            else
                            {
                                ((Label)e.Row.FindControl("lblTipoSupplementi")).Text = ((DatiSupplementi)(e.Row.DataItem)).CodGestioneSupplemento;
                                ((Label)e.Row.FindControl("lblQuotaSupplementi")).Text = ((DatiSupplementi)(e.Row.DataItem)).QuotaSupplemento.HasValue ? ((DatiSupplementi)(e.Row.DataItem)).QuotaSupplemento.ToString() : string.Empty;
                                ((Label)e.Row.FindControl("lblTipoQuotaSupplementi")).Text = ((DatiSupplementi)(e.Row.DataItem)).CodTipoQuota;
                                ((Label)e.Row.FindControl("lblDecorrenzaSupplementi")).Text = String.Format("{0:MM/yyyy}", ((DatiSupplementi)(e.Row.DataItem)).DecorrenzaSupplemento);
                                ((Label)e.Row.FindControl("lblSettimaneSupplementi")).Text = ((DatiSupplementi)(e.Row.DataItem)).NSettimaneSupplemento.HasValue ? ((DatiSupplementi)(e.Row.DataItem)).NSettimaneSupplemento.ToString() : string.Empty;
                                if (this.domanda.TipoAppartenenza == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.AGO)
                                    ((Label)e.Row.FindControl("lblRMSSupplementi")).Text = ((DatiSupplementi)(e.Row.DataItem)).RMSSupplemento.HasValue ? ((DatiSupplementi)(e.Row.DataItem)).RMSSupplemento.Value.ToString("0.0000") : string.Empty;
                                else if (this.domanda.TipoAppartenenza == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.CI)
                                    ((Label)e.Row.FindControl("lblRMSSupplementi")).Text = ((DatiSupplementi)(e.Row.DataItem)).RMSSupplemento.HasValue ? ((DatiSupplementi)(e.Row.DataItem)).RMSSupplemento.Value.ToString("0.000000") : string.Empty;
                                ((Label)e.Row.FindControl("lblCodLiqSupplementi")).Text = ((DatiSupplementi)(e.Row.DataItem)).CodiceLiquidazione.HasValue ? ((DatiSupplementi)(e.Row.DataItem)).CodiceLiquidazione.ToString() : string.Empty;

                                if (CodeUtility.IsRicostituzione(this.TitolarePensione.Pensione) && (Utility.IsDomandaFPLD(this.domanda.Categoria) || Utility.IsDomandaGestioneAutonomi(this.domanda.Categoria) ||
                                        Utility.IsDomandaDAI(this.domanda.Categoria) || Utility.IsDomandaAUT(this.domanda.Categoria)))
                                {
                                    if (((CodeUtility.IsRicostituzioneSupplemento(this.TitolarePensione.Pensione) && !isRicSupplementoTipoRetributivoMemo50) || CodeUtility.IsRicostituzioneContributiva(this.TitolarePensione.Pensione) || Utility.IsRicostituzione_MotiviDocumentali(this.TitolarePensione.Pensione) || Utility.IsRicostituzione_AccreditoPeriodiMaternita(this.TitolarePensione.Pensione)))
                                        EnableReadableMode(e.Row.Cells[0], e.Row.Cells[7]);
                                    else
                                    {
                                        LinkButton button = ((LinkButton)(e.Row.Cells[0].Controls[0]));
                                        button.Enabled = false;
                                        button.Text = "&nbsp;&nbsp;&nbsp;";
                                    }
                                }
                                else
                                    EnableReadableMode(e.Row.Cells[0], e.Row.Cells[7]);

                                if (isRicSupplementoTipoRetributivoMemo50)
                                {
                                    LinkButton button = ((LinkButton)(e.Row.Cells[0].Controls[0]));
                                    button.Enabled = false;
                                    button.Text = "&nbsp;&nbsp;&nbsp;";
                                }
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
                throw new INPS.DNA.DnaApplicationException("UCSupplementiAgoCI, Errore nel metodo gvSupplementi_RowDataBound " + ex);
            }
        }

        protected void gvSupplementi_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            List<DatiSupplementi> elencoSupplementi = ViewState["elencoSupplementiRetrib"] as List<DatiSupplementi>;
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

            int num = elencoSupplementi.Count;

            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            if (e.CommandName == "Delete")
            {
                this.modalitaEdit.Value = "false";

                Control c = (Control)e.CommandSource;
                GridViewRow r = (GridViewRow)c.NamingContainer;

                //ENG - MEMO 50/2023
                if (ViewState["AbilitazioneMemo50_2023"] != null && (string)ViewState["AbilitazioneMemo50_2023"] == "SI" && this.domanda.TipoAppartenenza.GetValueOrDefault() == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.AGO &&
                    Utility.IsRicostituzione_MotiviContributivi(TitolarePensione.Pensione) && TitolarePensione.Pensione.CodeTipo == "0001" && !this.domanda.IsDomandaENPALS && !Utility.IsDomandaCumulo(this.domanda.Categoria) && !(bool)ViewState[EnumViewState.IsTipoCalcoloModificato.ToString()])
                {
                    if (elencoSupplementi != null && elencoSupplementi.Count() > 0)
                    {
                        DatiSupplementi supplemento = elencoSupplementi.ElementAt(r.DataItemIndex);
                        if (supplemento.IsFromPrelievo)
                        {
                            this.HasError = true;
                            this.ErrorMessage = "Attenzione, non è possibile variare i dati del supplemento con il prodotto Webdom di ricostituzione per motivi contributivi di tipo ordinario. Utilizzare il prodotto Webdom di ricostituzione per variazione dati supplemento";
                            RaiseShowAvviso(this, null);
                            return;
                        }
                    }
                }

                elencoSupplementi.RemoveAt(r.DataItemIndex);

                if (elencoSupplementi.Count > 1)
                    gvSupplementi.EditIndex = -1;

                ViewState["elencoSupplementiRetrib"] = elencoSupplementi;
                GvSupplementi_Load();
            }
            else if (e.CommandName == "Edit")
            {
                modalitaEdit.Value = "true";
            }
            else if (e.CommandName == "Salva")
            {
                Control c = (Control)e.CommandSource;
                GridViewRow r = (GridViewRow)c.NamingContainer;
                string strGestione = ((DropDownList)(r.Cells[1].Controls[1])).SelectedValue;
                string strQuota = ((DropDownList)(r.Cells[2].Controls[1])).SelectedValue;
                string strTipoQuota = ((DropDownList)(r.Cells[3].Controls[1])).SelectedValue;
                string strCodLiquidazione = ((DropDownList)(r.Cells[7].Controls[1])).SelectedValue;
                DateTime decorrenzaQuota = Convert.ToDateTime(((TextBox)(r.Cells[4].Controls[1])).Text);
                if (Utility.IsDomandaDAI(this.domanda.Categoria) && !IsTipoQuotaValido(strGestione, strQuota, strTipoQuota))
                {
                    this.HasError = true;
                    this.ErrorMessage = "La terna Gestione '" + strGestione + "', Quota '" + strQuota + "' e Tipo Quota '" + strTipoQuota + "' non è valida.";
                    RaiseShowAvviso(this, null);
                    return;
                }
                if (!string.IsNullOrEmpty(valoreControllo) && valoreControllo != "NO" && (bool)ViewState[EnumViewState.IsReversibilitaOrRicostituzione.ToString()])
                {
                    if (decorrenzaQuota == TitolarePensione.Pensione.DecorrenzaOriginaria)
                    {
                        if (strQuota != "B" && !String.IsNullOrEmpty(strCodLiquidazione))
                        {
                            this.HasError = true;
                            this.ErrorMessage = "Il supplemento con decorrenza pari alla decorrenza della pensione deve essere di quota B";
                            RaiseShowAvviso(this, null);
                            return;
                        }
                        if (elencoSupplementi.Exists(x => x.DecorrenzaSupplemento < TitolarePensione.Pensione.DecorrenzaOriginaria &&
                            x.DecorrenzaSupplemento.Value.Year == TitolarePensione.Pensione.DecorrenzaOriginaria.Value.Year &&
                            !String.IsNullOrEmpty(strQuota) && x.QuotaSupplemento == Convert.ToChar(strQuota) && x.CodGestioneSupplemento == strGestione))
                        {
                            if (String.IsNullOrEmpty(strCodLiquidazione))
                            {
                                this.HasError = true;
                                this.ErrorMessage = "Il supplemento con decorrenza pari alla decorrenza della pensione deve avere codice liquidazione 6";
                                RaiseShowAvviso(this, null);
                                return;
                            }
                        }
                        else if (!String.IsNullOrEmpty(strCodLiquidazione))
                        {
                            this.HasError = true;
                            this.ErrorMessage = string.Format("Non è consentito inserire il codice liquidazione per il supplemento con decorrenza {0}", decorrenzaQuota.ToString("MM/yyyy"));
                            RaiseShowAvviso(this, null);
                            return;
                        }
                    }
                    else
                    {
                        if (!String.IsNullOrEmpty(strCodLiquidazione))
                        {
                            this.HasError = true;
                            this.ErrorMessage = "Il codice liquidazione 6 è ammesso solo per la quota B con decorrenza pari alla decorrenza della pensione";
                            RaiseShowAvviso(this, null);
                            return;
                        }
                    }
                }

                //ENG - MEMO 50/2023
                if (ViewState["AbilitazioneMemo50_2023"] != null && (string)ViewState["AbilitazioneMemo50_2023"] == "SI" && this.domanda.TipoAppartenenza.GetValueOrDefault() == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.AGO &&
                    Utility.IsRicostituzione_MotiviContributivi(TitolarePensione.Pensione) && TitolarePensione.Pensione.CodeTipo == "0001" && !this.domanda.IsDomandaENPALS && !Utility.IsDomandaCumulo(this.domanda.Categoria) && !(bool)ViewState[EnumViewState.IsTipoCalcoloModificato.ToString()])
                {
                    if (elencoSupplementi != null && elencoSupplementi.Count() > 0)
                    {
                        DatiSupplementi supplemento = elencoSupplementi.ElementAt(r.DataItemIndex);
                        if (supplemento.IsFromPrelievo)
                        {
                            this.HasError = true;
                            this.ErrorMessage = "Attenzione, non è possibile variare i dati del supplemento con il prodotto Webdom di ricostituzione per motivi contributivi di tipo ordinario. Utilizzare il prodotto Webdom di ricostituzione per variazione dati supplemento";
                            RaiseShowAvviso(this, null);
                            return;
                        }
                        else
                        {
                            this.HasError = true;
                            this.ErrorMessage = "L'inserimento di un nuovo supplemento è consentito solo per le domande di Ricostituzione per supplemento";
                            RaiseShowAvviso(this, null);
                            return;
                        }
                    }
                }

                if ((r.DataItemIndex - 1) == (num - 2))    //aggiunta riga (non si tratta di una modifica)
                {
                    DatiSupplementi supp = new DatiSupplementi();
                    supp.CodGestioneSupplemento = ((DropDownList)(r.Cells[1].Controls[1])).SelectedValue;
                    if (!String.IsNullOrEmpty((((DropDownList)(r.Cells[2].Controls[1])).SelectedValue)))
                        supp.QuotaSupplemento = (((DropDownList)(r.Cells[2].Controls[1])).SelectedValue)[0];
                    if (!String.IsNullOrEmpty((((DropDownList)(r.Cells[3].Controls[1])).SelectedValue)))
                        supp.CodTipoQuota = ((DropDownList)(r.Cells[3].Controls[1])).SelectedValue;
                    if (!String.IsNullOrEmpty(((TextBox)(r.Cells[4].Controls[1])).Text))
                        supp.DecorrenzaSupplemento = Convert.ToDateTime(((TextBox)(r.Cells[4].Controls[1])).Text);
                    if (!String.IsNullOrEmpty(((TextBox)(r.Cells[5].Controls[1])).Text))
                        supp.NSettimaneSupplemento = Convert.ToInt32(((TextBox)(r.Cells[5].Controls[1])).Text);
                    if (!String.IsNullOrEmpty(((TextBox)(r.Cells[6].Controls[1])).Text))
                        supp.RMSSupplemento = Decimal.Parse(((TextBox)(r.Cells[6].Controls[1])).Text);
                    if (!String.IsNullOrEmpty(((DropDownList)(r.Cells[7].Controls[1])).SelectedValue))
                        supp.CodiceLiquidazione = byte.Parse(((DropDownList)(r.Cells[7].Controls[1])).SelectedValue);
                    supp.TipoSupplemento = 'R';

                    elencoSupplementi.RemoveAt(num - 1);
                    elencoSupplementi.Add(supp);
                    AddItemBlank(ref elencoSupplementi);
                }
                else
                    saveValueRow(elencoSupplementi, e, r);

                modalitaEdit.Value = "false";
                gvSupplementi.EditIndex = -1;
                ViewState["elencoSupplementiRetrib"] = elencoSupplementi;
                GvSupplementi_Load();
            }
            else if (e.CommandName == "Annulla")
            {
                if (!IsListaEmpty())
                {
                    modalitaEdit.Value = "false";
                    gvSupplementi.EditIndex = -1;
                    GvSupplementi_Load();
                }
            }
            GestioneTastoSalva();
        }

        protected void gvSupplementi_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            try
            {
                //Reset the edit index.
                gvSupplementi.EditIndex = -1;

                //Bind data to the GridView control.
                GvSupplementi_Load();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCSupplementiAgoCI, Errore nel metodo gvSupplementi_RowCancelingEdit " + ex);
            }
        }

        protected void gvSupplementi_RowEditing(object sender, GridViewEditEventArgs e)
        {
            try
            {
                gvSupplementi.EditIndex = e.NewEditIndex;
                GvSupplementi_Load();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCSupplementiAgoCI, Errore nel metodo gvSupplementi_RowEditing " + ex);
            }
        }

        protected void gvSupplementi_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            try
            {
                List<DatiSupplementi> elencoSupplementi = ViewState["elencoSupplementiRetrib"] as List<DatiSupplementi>;

                if (elencoSupplementi.Count < 1)
                    inserisciSupplementi();
                GvSupplementi_Load();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCSupplementiAgoCI, Errore nel metodo gvSupplementi_RowDeleting " + ex);
            }
        }

        protected void gvSupplementi_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            try
            {
                List<DatiSupplementi> elencoSupplementi = ViewState["elencoSupplementiRetrib"] as List<DatiSupplementi>;
                GridViewRow row = gvSupplementi.Rows[e.RowIndex];
                if (((DropDownList)(row.Cells[2].Controls[1])).SelectedIndex != 0)
                {
                    int i = ((gvSupplementi.PageIndex * 10) + e.RowIndex);

                    if (elencoSupplementi.Count != i + 1)
                        elencoSupplementi.RemoveAt(elencoSupplementi.Count - 1);
                    gvSupplementi.EditIndex = -1;
                    ViewState["elencoSupplementiRetrib"] = elencoSupplementi;
                    GvSupplementi_Load();
                }
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCStatoCivile, Errore nel metodo gvStatoCivile_RowUpdating " + ex);
            }
        }

        protected void gvSupplementi_onPageIndexChanging(Object sender, GridViewPageEventArgs e)
        {
            try
            {
                gvSupplementi.PageIndex = e.NewPageIndex;
                GvSupplementi_Load();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCSupplementiAgoCI, Errore nel metodo gvSupplementi_onPageIndexChanging" + ex);
            }
        }

        protected void RaiseGetDecorrenzaPensione(object sender, EventArgs e)
        {
            if (GetDecorrenzaPensione != null)
                GetDecorrenzaPensione(sender, e);
        }

        protected void ddlCodGestioneSupplementi_SelectedIndexChanged(object sender, EventArgs e)
        {
            DropDownList ddlCodGestioneSupplementi = (DropDownList)sender;
            if (ddlCodGestioneSupplementi != null)
            {
                GridViewRow row = (GridViewRow)ddlCodGestioneSupplementi.NamingContainer;
                DropDownList ddlQuotaSupplementi = (DropDownList)(row.FindControl("ddlQuotaSupplementi"));
                DropDownList ddlTipoQuotaSupplementi = (DropDownList)(row.FindControl("ddlTipoQuotaSupplementi"));
                SetDdlTipoQuotaSupplementi(ddlCodGestioneSupplementi, ddlQuotaSupplementi, ddlTipoQuotaSupplementi);
            }
        }

        protected void ddlQuotaSupplementi_SelectedIndexChanged(object sender, EventArgs e)
        {
            DropDownList ddlQuotaSupplementi = (DropDownList)sender;
            if (ddlQuotaSupplementi != null)
            {
                GridViewRow row = (GridViewRow)ddlQuotaSupplementi.NamingContainer;
                DropDownList ddlCodGestioneSupplementi = (DropDownList)(row.FindControl("ddlCodGestioneSupplementi"));
                DropDownList ddlTipoQuotaSupplementi = (DropDownList)(row.FindControl("ddlTipoQuotaSupplementi"));
                SetDdlTipoQuotaSupplementi(ddlCodGestioneSupplementi, ddlQuotaSupplementi, ddlTipoQuotaSupplementi);
            }
        }
        #endregion Grid Retributivi

        #region Private Methods Grid Retributivi

        private void setDdl(GridViewRowEventArgs e)
        {
            DropDownList ddlCodGestioneSupplementi = (DropDownList)e.Row.FindControl("ddlCodGestioneSupplementi");
            if (ddlCodGestioneSupplementi != null)
            {
                List<TipoSupplementi> lTipoSupp = (List<TipoSupplementi>)ViewState["elencoTipoSupplementi"];
                foreach (TipoSupplementi tipo in lTipoSupp)
                {
                    ListItem item = new ListItem();
                    item.Text = item.Value = tipo.Id;
                    item.Attributes.Add("title", tipo.Descrizione);
                    ddlCodGestioneSupplementi.Items.Add(item);
                }
            }

            DropDownList ddlQuotaSupplementi = (DropDownList)e.Row.FindControl("ddlQuotaSupplementi");
            if (ddlQuotaSupplementi != null)
            {
                ddlQuotaSupplementi.Items[1].Attributes.Add("title", "A");
                ddlQuotaSupplementi.Items[2].Attributes.Add("title", "B");
            }
        }

        private void SetDdlTipoQuotaSupplementi(DropDownList ddlCodGestioneSupplementi, DropDownList ddlQuotaSupplementi, DropDownList ddlTipoQuotaSupplementi)
        {
            if (ddlTipoQuotaSupplementi != null)
            {
                ddlTipoQuotaSupplementi.Items.Clear();
                if (ddlCodGestioneSupplementi != null && ddlCodGestioneSupplementi.SelectedIndex > 0 && ddlQuotaSupplementi != null && ddlQuotaSupplementi.SelectedIndex > 0)
                {
                    List<TipoQuota> lTipoQuota = (List<TipoQuota>)ViewState[EnumViewState.DecodificaTipoQuota.ToString()];
                    if (lTipoQuota != null)
                    {
                        ddlTipoQuotaSupplementi.Items.Add(new ListItem(string.Empty, string.Empty));
                        foreach (TipoQuota tipoQuota in lTipoQuota)
                        {
                            if (!IsTipoQuotaValido(ddlCodGestioneSupplementi.SelectedValue, ddlQuotaSupplementi.SelectedValue, tipoQuota.Codice))
                                continue;
                            ListItem item = new ListItem();
                            item.Attributes.Add("title", tipoQuota.Decodifica);
                            item.Text = tipoQuota.Codice;
                            item.Value = tipoQuota.Codice;
                            ddlTipoQuotaSupplementi.Items.Add(item);
                        }
                    }
                }
            }
        }

        private static bool IsTipoQuotaValido(String codGestione, String quota, string codTipoQuota)
        {
            bool isValido = false;
            switch (codGestione)
            {
                case "A":
                    switch (quota)
                    {
                        case "A":
                            isValido = String.IsNullOrEmpty(codTipoQuota) || codTipoQuota == "A1";
                            break;
                        case "B":
                            isValido = codTipoQuota == "B1" || codTipoQuota == "B2" || codTipoQuota == "B3" || codTipoQuota == "B4" || codTipoQuota == "B6";
                            break;
                    }
                    break;
                case "1":
                case "2":
                case "3":
                case "4":
                    switch (quota)
                    {
                        case "A":
                            isValido = String.IsNullOrEmpty(codTipoQuota);
                            break;
                        case "B":
                            isValido = codTipoQuota == "B" || codTipoQuota == "B9";
                            break;
                    }
                    break;
                case "I":
                case "M":
                case "N":
                    switch (quota)
                    {
                        case "A":
                        case "B":
                            isValido = String.IsNullOrEmpty(codTipoQuota);
                            break;
                    }
                    break;
            }
            return isValido;
        }

        private void setCampiEdit(GridViewRowEventArgs e)
        {
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            DropDownList ddlQuotaSupplementi = new DropDownList();
            ddlQuotaSupplementi = (DropDownList)e.Row.FindControl("ddlQuotaSupplementi");
            DropDownList ddlCodGestioneSupplementi = new DropDownList();
            ddlCodGestioneSupplementi = (DropDownList)e.Row.FindControl("ddlCodGestioneSupplementi");
            DropDownList ddlCodLiqSupplementi = new DropDownList();
            ddlCodLiqSupplementi = (DropDownList)e.Row.FindControl("ddlCodLiqSupplementi");
            TextBox txtDecorrenzaSupplementi = (TextBox)e.Row.FindControl("txtDecorrenzaSupplementi");
            TextBox txtSettimaneSupplementi = (TextBox)e.Row.FindControl("txtSettimaneSupplementi");
            TextBox txtRMSSupplementi = (TextBox)e.Row.FindControl("txtRMSSupplementi");
            if (((DatiSupplementi)e.Row.DataItem).CodGestioneSupplemento != null)
                ddlCodGestioneSupplementi.SelectedValue = ((DatiSupplementi)e.Row.DataItem).CodGestioneSupplemento;
            ddlQuotaSupplementi.SelectedValue = ((DatiSupplementi)e.Row.DataItem).QuotaSupplemento.HasValue ? ((DatiSupplementi)e.Row.DataItem).QuotaSupplemento.ToString() : string.Empty;
            DropDownList ddlTipoQuotaSupplementi = (DropDownList)e.Row.FindControl("ddlTipoQuotaSupplementi");
            SetDdlTipoQuotaSupplementi(ddlCodGestioneSupplementi, ddlQuotaSupplementi, ddlTipoQuotaSupplementi);
            if (((DatiSupplementi)e.Row.DataItem).CodTipoQuota != null && ddlTipoQuotaSupplementi.Items.Count > 0)
                ddlTipoQuotaSupplementi.SelectedValue = ((DatiSupplementi)e.Row.DataItem).CodTipoQuota;
            txtDecorrenzaSupplementi.Text = String.Format("{0:MM/yyyy}", ((DatiSupplementi)e.Row.DataItem).DecorrenzaSupplemento);
            txtSettimaneSupplementi.Text = ((DatiSupplementi)e.Row.DataItem).NSettimaneSupplemento.HasValue ? ((DatiSupplementi)e.Row.DataItem).NSettimaneSupplemento.ToString() : string.Empty;
            if (this.domanda.TipoAppartenenza == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.AGO)
                txtRMSSupplementi.Text = ((DatiSupplementi)e.Row.DataItem).RMSSupplemento.HasValue ? ((DatiSupplementi)e.Row.DataItem).RMSSupplemento.Value.ToString("0.0000") : string.Empty;
            else if (this.domanda.TipoAppartenenza == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.CI)
                txtRMSSupplementi.Text = ((DatiSupplementi)e.Row.DataItem).RMSSupplemento.HasValue ? ((DatiSupplementi)e.Row.DataItem).RMSSupplemento.Value.ToString("0.000000") : string.Empty;
            if (((DatiSupplementi)e.Row.DataItem).CodiceLiquidazione != null && ddlCodLiqSupplementi.Items.Count > 0)
                ddlCodLiqSupplementi.SelectedValue = ((DatiSupplementi)e.Row.DataItem).CodiceLiquidazione.HasValue ? ((DatiSupplementi)e.Row.DataItem).CodiceLiquidazione.ToString() : string.Empty;
        }

        private void saveValueRow(List<DatiSupplementi> elencoSupplementi, GridViewCommandEventArgs e, GridViewRow r)
        {
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
            if (!String.IsNullOrEmpty((((DropDownList)(r.Cells[1].Controls[1])).SelectedValue)))
                elencoSupplementi[r.DataItemIndex].CodGestioneSupplemento = (((DropDownList)(r.Cells[1].Controls[1])).SelectedValue);
            if (!String.IsNullOrEmpty((((DropDownList)(r.Cells[2].Controls[1])).SelectedValue)))
                elencoSupplementi[r.DataItemIndex].QuotaSupplemento = (((DropDownList)(r.Cells[2].Controls[1])).SelectedValue)[0];
            else
                elencoSupplementi[r.DataItemIndex].QuotaSupplemento = null; // Campo non obbligatorio
            if (this.domanda != null && Utility.IsDomandaDAI(this.domanda.Categoria))
                elencoSupplementi[r.DataItemIndex].CodTipoQuota = ((DropDownList)(r.Cells[3].Controls[1])).SelectedValue;
            else
                elencoSupplementi[r.DataItemIndex].CodTipoQuota = null;
            if (!String.IsNullOrEmpty((((TextBox)(r.Cells[4].Controls[1])).Text)))
                elencoSupplementi[r.DataItemIndex].DecorrenzaSupplemento = Convert.ToDateTime(((TextBox)(r.Cells[4].Controls[1])).Text);
            if (!String.IsNullOrEmpty((((TextBox)(r.Cells[5].Controls[1])).Text)))
                elencoSupplementi[r.DataItemIndex].NSettimaneSupplemento = Convert.ToInt32(((TextBox)(r.Cells[5].Controls[1])).Text);
            if (!String.IsNullOrEmpty((((TextBox)(r.Cells[6].Controls[1])).Text)))
                elencoSupplementi[r.DataItemIndex].RMSSupplemento = Decimal.Parse(((TextBox)(r.Cells[6].Controls[1])).Text);
            else
                elencoSupplementi[r.DataItemIndex].RMSSupplemento = null; // Campo non obbligatorio

            if (!String.IsNullOrEmpty((((DropDownList)(r.Cells[7].Controls[1])).SelectedValue)))
                elencoSupplementi[r.DataItemIndex].CodiceLiquidazione = byte.Parse(((DropDownList)(r.Cells[7].Controls[1])).SelectedValue);
            else
                elencoSupplementi[r.DataItemIndex].CodiceLiquidazione = null;
        }

        private void GvSupplementi_Load()
        {
            try
            {
                if (this.domanda == null)
                    this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
                if (this.TitolarePensione == null)
                    this.TitolarePensione = new AreaTitolare();
                if (this.TitolarePensione.Pensione == null)
                    this.TitolarePensione.Pensione = GetDatiPensione(this);
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
                List<DatiSupplementi> elencoSupplementi = ViewState["elencoSupplementiRetrib"] as List<DatiSupplementi>;
                AddItemBlank(ref elencoSupplementi);
                if (!string.IsNullOrEmpty(valoreControllo) && valoreControllo != "NO" && (bool)ViewState[EnumViewState.IsReversibilitaOrRicostituzione.ToString()])
                {
                    gvSupplementi.Columns[6].Visible = true;
                }
                gvSupplementi.DataSource = elencoSupplementi;
                gvSupplementi.DataBind();

                if (Utility.IsDomandaRipristino(this.TitolarePensione.Pensione))
                    gvSupplementi.Enabled = false;
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCSupplementiAgoCI, Errore nel metodo GvSupplementi_Load " + ex);
            }
        }

        private void GvSupplementiAnte96_Load()
        {
            try
            {
                if (this.domanda == null)
                    this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
                if (this.TitolarePensione == null)
                    this.TitolarePensione = new AreaTitolare();
                if (this.TitolarePensione.Pensione == null)
                    this.TitolarePensione.Pensione = GetDatiPensione(this);

                List<DatiSupplementi> elencoSupplementi = ViewState["elencoSupplementiAnte96"] as List<DatiSupplementi>;
                AddItemBlank(ref elencoSupplementi);

                gvSupplementiAnte96.DataSource = elencoSupplementi;
                gvSupplementiAnte96.DataBind();

                gvSupplementiAnte96.Enabled = Utility.Sblocco_supplementi_ante96();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCSupplementiAgoCI, Errore nel metodo GvSupplementi_Load " + ex);
            }
        }
        private void inserisciSupplementi()
        {
            try
            {
                List<DatiSupplementi> elencoSupplementi = ViewState["elencoSupplementiRetrib"] as List<DatiSupplementi>;
                DatiSupplementi supp = new DatiSupplementi();
                elencoSupplementi.Add(supp);
                ViewState["elencoSupplementiRetrib"] = elencoSupplementi;
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCSupplementiAgoCI, Errore nel metodo InserisciSupplementi " + ex);
            }
        }

        private bool IsListaEmpty()
        {
            List<DatiSupplementi> listaDatiSuppl = ViewState["elencoSupplementiRetrib"] as List<DatiSupplementi>;

            if (listaDatiSuppl.Count == 1 && string.IsNullOrEmpty(listaDatiSuppl[0].CodGestioneSupplemento) &&
                string.IsNullOrEmpty(listaDatiSuppl[0].DecorrenzaSupplemento.ToString()) && string.IsNullOrEmpty(listaDatiSuppl[0].MontanteSupplemento.ToString()) &&
                string.IsNullOrEmpty(listaDatiSuppl[0].QuotaSupplemento.ToString()) && string.IsNullOrEmpty(listaDatiSuppl[0].RMSSupplemento.ToString()) &&
                string.IsNullOrEmpty(listaDatiSuppl[0].TipoSupplemento.ToString()) && string.IsNullOrEmpty(listaDatiSuppl[0].NSettimaneSupplemento.ToString()))
                return true;
            else
                return false;
        }

        private bool IsEmptyEditableRow(GridViewRow row)
        {
            if (row.FindControl("txtDecorrenzaSupplementi") != null && ((TextBox)row.FindControl("txtDecorrenzaSupplementi")).Text != string.Empty &&
                row.FindControl("txtSettimaneSupplementi") != null && ((TextBox)row.FindControl("txtSettimaneSupplementi")).Text != string.Empty &&
                row.FindControl("txtRMSSupplementi") != null && ((TextBox)row.FindControl("txtRMSSupplementi")).Text != string.Empty &&
                row.FindControl("ddlCodGestioneSupplementi") != null && ((DropDownList)row.FindControl("ddlCodGestioneSupplementi")).SelectedIndex != 0 &&
                row.FindControl("ddlQuotaSupplementi") != null && ((DropDownList)row.FindControl("ddlQuotaSupplementi")).SelectedIndex != 0)
                return false;
            else
                return true;

        }

        private bool IsEmptyReadableRow(GridViewRow row)
        {
            if (row.FindControl("lblTipoSupplementi") != null && ((TextBox)row.FindControl("lblTipoSupplementi")).Text != string.Empty &&
                row.FindControl("lblDecorrenzaSupplementi") != null && ((TextBox)row.FindControl("lblDecorrenzaSupplementi")).Text != string.Empty &&
                row.FindControl("lblSettimaneSupplementi") != null && ((TextBox)row.FindControl("lblSettimaneSupplementi")).Text != string.Empty &&
                row.FindControl("lblRMSSupplementi") != null && ((DropDownList)row.FindControl("lblRMSSupplementi")).SelectedIndex != 0 &&
                row.FindControl("lblQuotaSupplementi") != null && ((DropDownList)row.FindControl("lblQuotaSupplementi")).SelectedIndex != 0)
                return false;
            else
                return true;
        }

        private void EnableEditableMode(TableCell cell_CancelSave)
        {
            cell_CancelSave.Width = new Unit(40, UnitType.Pixel);

            LinkButton cancel = ((LinkButton)(cell_CancelSave.Controls[2]));
            cancel.Text = (string)"<img width=16 height=16 border=0 src=../App_themes/" + Page.Theme + "/Images/cancel24.png />";
            cancel.ToolTip = "Annulla";
            cancel.CommandName = "Annulla";

            LinkButton save = ((LinkButton)(cell_CancelSave.Controls[0]));
            save.Text = (string)"<img width=16 height=16 border=0 src=../App_themes/" + Page.Theme + "/Images/save24.png />";
            save.ToolTip = "Salva";
            save.CommandName = "Salva";
            save.CausesValidation = true;
            save.ValidationGroup = "UCTabSupplementiRetrib";

        }

        private void removeItemBlank(ref List<DatiSupplementi> lista)
        {
            if (lista != null && lista.Count() > 0)
            {
                int index = lista.FindIndex(delegate (DatiSupplementi code)
                {
                    return (string.IsNullOrEmpty(code.CodGestioneSupplemento) && string.IsNullOrEmpty(code.QuotaSupplemento.ToString()) &&
                        string.IsNullOrEmpty(code.DecorrenzaSupplemento.ToString()) && string.IsNullOrEmpty(code.NSettimaneSupplemento.ToString()) &&
                        string.IsNullOrEmpty(code.RMSSupplemento.ToString()));
                }
                    );

                if (index >= 0)
                {
                    lista.RemoveAt(index);
                }
            }
        }

        private void AddItemBlank(ref List<DatiSupplementi> lista)
        {
            //per FPLD, Gestione Autonomi, DAI e AUT si può inserire un supplemento solo in caso di Ricostituzione per Supplemento [Appunto n.36/2022 (JIRA L2_IST_LIQ-1857)]
            //12/05/2022 per FPLD, Gestione Autonomi, DAI e AUT si può inserire un supplemento anche in caso di Ricostituzione Contributiva e Documentale se presente un supplemento dal prelievo
            if (lista != null && !(!CodeUtility.IsRicostituzioneSupplemento(this.TitolarePensione.Pensione) && !CodeUtility.IsRicostituzioneContributiva(this.TitolarePensione.Pensione) && !Utility.IsRicostituzione_MotiviDocumentali(this.TitolarePensione.Pensione) && CodeUtility.IsRicostituzione(this.TitolarePensione.Pensione) && (Utility.IsDomandaFPLD(this.domanda.Categoria) ||
                Utility.IsDomandaGestioneAutonomi(this.domanda.Categoria) || Utility.IsDomandaDAI(this.domanda.Categoria) || Utility.IsDomandaAUT(this.domanda.Categoria))))
            {
                if (((Utility.IsRicostituzione_MotiviContributivi(this.TitolarePensione.Pensione) && !Utility.IsRicostituzione_PerVariazioneDatiSupplemento(this.TitolarePensione.Pensione)) || Utility.IsRicostituzione_MotiviDocumentali(this.TitolarePensione.Pensione)) &&
                   (Utility.IsDomandaFPLD(this.domanda.Categoria) || Utility.IsDomandaGestioneAutonomi(this.domanda.Categoria) || Utility.IsDomandaDAI(this.domanda.Categoria) || Utility.IsDomandaAUT(this.domanda.Categoria)))
                {
                    bool esisteSupplementoDaPrelievo = lista.Exists(x => x.IsFromPrelievo);
                    if (esisteSupplementoDaPrelievo)
                        inserisciRigaPerInserimentoSupplemento(ref lista);
                }
                else
                {
                    if ((bool)ViewState[EnumViewState.IsPannelloSupplementiAnte96.ToString()])
                    {
                        if (CodeUtility.IsRicostituzioneSupplemento(this.TitolarePensione.Pensione) || Utility.IsRicostituzione_PerVariazioneDatiSupplemento(this.TitolarePensione.Pensione))
                        {
                            inserisciRigaPerInserimentoSupplemento(ref lista);
                        }
                    }
                    else
                    {
                        inserisciRigaPerInserimentoSupplemento(ref lista);
                    }
                }

            }
            //ENG - MEMO 50/2023
            if ((bool)ViewState[EnumViewState.IsTipoCalcoloModificato.ToString()])
                inserisciRigaPerInserimentoSupplemento(ref lista);
        }

        private void inserisciRigaPerInserimentoSupplemento(ref List<DatiSupplementi> lista)
        {
            int index = lista.FindIndex(delegate (DatiSupplementi code)
            {
                return (string.IsNullOrEmpty(code.CodGestioneSupplemento) && string.IsNullOrEmpty(code.QuotaSupplemento.ToString()) &&
                    string.IsNullOrEmpty(code.DecorrenzaSupplemento.ToString()) && string.IsNullOrEmpty(code.NSettimaneSupplemento.ToString()) &&
                    string.IsNullOrEmpty(code.RMSSupplemento.ToString()));
            }
                );

            if (index < 0)
            {
                lista.Add(new DatiSupplementi());
            }
        }

        private bool IsItemBlankPresent(List<DatiSupplementi> lista)
        {
            if (lista != null)
            {
                int index = lista.FindIndex(delegate (DatiSupplementi code)
                {
                    return (string.IsNullOrEmpty(code.CodGestioneSupplemento) && string.IsNullOrEmpty(code.QuotaSupplemento.ToString()) &&
                        string.IsNullOrEmpty(code.DecorrenzaSupplemento.ToString()) && string.IsNullOrEmpty(code.NSettimaneSupplemento.ToString()) &&
                        string.IsNullOrEmpty(code.RMSSupplemento.ToString()));
                }
                    );

                if (index < 0)
                    return false;
                else
                    return true;
            }
            return false;
        }

        #endregion Private Methods Grid Retributivi

        #region Grid Contributivi

        protected void gvSupplementiContributivi_DataBinding(object sender, EventArgs e)
        {
            if (this.domanda.TipoAppartenenza.HasValue && this.domanda.TipoAppartenenza.Value == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.CI)
            {
                ((GridView)sender).Columns[1].Visible = true; //ENG - Modifica Supplementi CI Memo 177/2012
                ((GridView)sender).Columns[6].Visible = false;
            }
            else
                ((GridView)sender).Columns[2].HeaderStyle.Width = new Unit(120, UnitType.Pixel);
        }

        protected void gvSupplementiContributivi_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (this.domanda == null)
                    this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

                if (this.TitolarePensione == null)
                    this.TitolarePensione = new AreaTitolare();
                if (this.TitolarePensione.Pensione == null)
                    this.TitolarePensione.Pensione = GetDatiPensione(this);

                List<DatiSupplementi> elencoSupplementiContrib = ViewState["elencoSupplementiContrib"] as List<DatiSupplementi>;

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
                bool isRicSupplementoTipoContributivoMemo50 = false;

                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    setDdlContrib(e);
                    int num = ((List<DatiSupplementi>)ViewState["elencoSupplementiContrib"]).Count;

                    if (ViewState["AbilitazioneMemo50_2023"] != null && (string)ViewState["AbilitazioneMemo50_2023"] == "SI" && this.domanda.TipoAppartenenza.GetValueOrDefault() == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.AGO &&
                       Utility.IsRicostituzione_Supplemento(this.TitolarePensione.Pensione) && !this.domanda.IsDomandaENPALS && !Utility.IsDomandaCumulo(this.domanda.Categoria) && elencoSupplementiContrib != null && elencoSupplementiContrib.Count() > 0)
                    {
                        DatiSupplementi supplementoContributivo = elencoSupplementiContrib.ElementAt(e.Row.DataItemIndex);
                        if (supplementoContributivo.IsFromPrelievo == true && !elencoSupplementiContrib.Exists(x => x.IsFromPrelievo == false && !string.IsNullOrEmpty(x.CodGestioneSupplemento)))
                            isRicSupplementoTipoContributivoMemo50 = true;
                    }

                    if (ViewState["AbilitazioneMemo50_2023"] != null && (string)ViewState["AbilitazioneMemo50_2023"] == "SI" && this.domanda.TipoAppartenenza.GetValueOrDefault() == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.AGO &&
                       Utility.IsRicostituzione_MotiviDocumentali(this.TitolarePensione.Pensione) && !this.domanda.IsDomandaENPALS && !Utility.IsDomandaCumulo(this.domanda.Categoria) && elencoSupplementiContrib != null && elencoSupplementiContrib.Count() > 0)  // sola lettura
                    {
                        gvSupplementi.EditIndex = -1;

                        ((Label)e.Row.FindControl("lblTipoSupplementiContrib")).Text = ((DatiSupplementi)(e.Row.DataItem)).CodGestioneSupplemento;
                        ((Label)e.Row.FindControl("lblQuotaSupplementi")).Text = ((DatiSupplementi)(e.Row.DataItem)).QuotaSupplemento.HasValue ? ((DatiSupplementi)(e.Row.DataItem)).QuotaSupplemento.ToString() : string.Empty; //ENG - Modifica Supplementi CI Memo 177/2012
                        ((Label)e.Row.FindControl("lblDecorrenzaSupplementiContrib")).Text = String.Format("{0:MM/yyyy}", ((DatiSupplementi)(e.Row.DataItem)).DecorrenzaSupplemento);
                        ((Label)e.Row.FindControl("lblSettimaneSupplementiContrib")).Text = ((DatiSupplementi)(e.Row.DataItem)).NSettimaneSupplemento.HasValue ? ((DatiSupplementi)(e.Row.DataItem)).NSettimaneSupplemento.ToString() : string.Empty;
                        ((Label)e.Row.FindControl("lblMontanteIVS")).Text = ((DatiSupplementi)(e.Row.DataItem)).MontanteSupplemento.HasValue ? ((DatiSupplementi)(e.Row.DataItem)).MontanteSupplemento.ToString() : string.Empty;
                        ((Label)e.Row.FindControl("lblAmmontareContributivo")).Text = ((DatiSupplementi)(e.Row.DataItem)).AmmontareContributivo.HasValue ? ((DatiSupplementi)(e.Row.DataItem)).AmmontareContributivo.ToString() : string.Empty;
                        if (((DatiSupplementi)(e.Row.DataItem)).CodiceLiquidazione.HasValue && ((DatiSupplementi)(e.Row.DataItem)).CodiceLiquidazione.Value == 3)
                            ((Label)e.Row.FindControl("lblCodiceLiquidazione")).Text = "C";
                        else if (((DatiSupplementi)(e.Row.DataItem)).CodiceLiquidazione.HasValue && ((DatiSupplementi)(e.Row.DataItem)).CodiceLiquidazione.Value == 4)
                            ((Label)e.Row.FindControl("lblCodiceLiquidazione")).Text = "D";
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
                            if (IsListaEmptyContrib() && !Convert.ToBoolean(modalitaEditContrib.Value))
                            {
                                gvSupplementiContributivi.EditIndex = 0;
                                modalitaEditContrib.Value = "true";
                                GvSupplementiContributivi_Load();
                                GestioneTastoSalva();
                            }
                            else
                            {
                                if (e.Row.Cells[0].Controls.Count == 3)  // edit mode
                                {
                                    setCampiEditContrib(e);
                                    EnableEditableModeContrib(e.Row.Cells[0]);
                                }
                                else
                                {
                                    ((Label)e.Row.FindControl("lblTipoSupplementiContrib")).Text = ((DatiSupplementi)(e.Row.DataItem)).CodGestioneSupplemento;
                                    ((Label)e.Row.FindControl("lblQuotaSupplementi")).Text = ((DatiSupplementi)(e.Row.DataItem)).QuotaSupplemento.HasValue ? ((DatiSupplementi)(e.Row.DataItem)).QuotaSupplemento.ToString() : string.Empty; //ENG - Modifica Supplementi CI Memo 177/2012
                                    ((Label)e.Row.FindControl("lblDecorrenzaSupplementiContrib")).Text = String.Format("{0:MM/yyyy}", ((DatiSupplementi)(e.Row.DataItem)).DecorrenzaSupplemento);
                                    ((Label)e.Row.FindControl("lblSettimaneSupplementiContrib")).Text = ((DatiSupplementi)(e.Row.DataItem)).NSettimaneSupplemento.HasValue ? ((DatiSupplementi)(e.Row.DataItem)).NSettimaneSupplemento.ToString() : string.Empty;
                                    ((Label)e.Row.FindControl("lblMontanteIVS")).Text = ((DatiSupplementi)(e.Row.DataItem)).MontanteSupplemento.HasValue ? ((DatiSupplementi)(e.Row.DataItem)).MontanteSupplemento.ToString() : string.Empty;
                                    ((Label)e.Row.FindControl("lblAmmontareContributivo")).Text = ((DatiSupplementi)(e.Row.DataItem)).AmmontareContributivo.HasValue ? ((DatiSupplementi)(e.Row.DataItem)).AmmontareContributivo.ToString() : string.Empty;
                                    if (((DatiSupplementi)(e.Row.DataItem)).CodiceLiquidazione.HasValue && ((DatiSupplementi)(e.Row.DataItem)).CodiceLiquidazione.Value == 3)
                                        ((Label)e.Row.FindControl("lblCodiceLiquidazione")).Text = "C";
                                    else if (((DatiSupplementi)(e.Row.DataItem)).CodiceLiquidazione.HasValue && ((DatiSupplementi)(e.Row.DataItem)).CodiceLiquidazione.Value == 4)
                                        ((Label)e.Row.FindControl("lblCodiceLiquidazione")).Text = "D";

                                    if (CodeUtility.IsRicostituzione(this.TitolarePensione.Pensione) && (Utility.IsDomandaFPLD(this.domanda.Categoria) || Utility.IsDomandaGestioneAutonomi(this.domanda.Categoria) ||
                                        Utility.IsDomandaDAI(this.domanda.Categoria) || Utility.IsDomandaAUT(this.domanda.Categoria)))
                                    {
                                        if (((CodeUtility.IsRicostituzioneSupplemento(this.TitolarePensione.Pensione) && !isRicSupplementoTipoContributivoMemo50) || CodeUtility.IsRicostituzioneContributiva(this.TitolarePensione.Pensione) || Utility.IsRicostituzione_MotiviDocumentali(this.TitolarePensione.Pensione) || Utility.IsRicostituzione_AccreditoPeriodiMaternita(this.TitolarePensione.Pensione)))
                                            EnableReadableMode(e.Row.Cells[0], e.Row.Cells[7]);
                                        else
                                        {
                                            LinkButton button = ((LinkButton)(e.Row.Cells[0].Controls[0]));
                                            button.Enabled = false;
                                            button.Text = "&nbsp;&nbsp;&nbsp;";
                                        }
                                    }
                                    else
                                        EnableReadableMode(e.Row.Cells[0], e.Row.Cells[7]);

                                    if (isRicSupplementoTipoContributivoMemo50)
                                    {
                                        LinkButton button = ((LinkButton)(e.Row.Cells[0].Controls[0]));
                                        button.Enabled = false;
                                        button.Text = "&nbsp;&nbsp;&nbsp;";
                                    }
                                }
                            }
                        }
                        else // righe successive
                        {
                            if (e.Row.Cells[0].Controls.Count == 3)  // edit mode
                            {
                                setCampiEditContrib(e);
                                EnableEditableModeContrib(e.Row.Cells[0]);

                                //ENG - MEMO 50/2023
                                if (ViewState["AbilitazioneMemo50_2023"] != null && (string)ViewState["AbilitazioneMemo50_2023"] == "SI" && this.domanda.TipoAppartenenza.GetValueOrDefault() == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.AGO &&
                                    Utility.IsRicostituzione_MotiviContributivi(TitolarePensione.Pensione) && TitolarePensione.Pensione.CodeTipo == "0001" && !this.domanda.IsDomandaENPALS && !Utility.IsDomandaCumulo(this.domanda.Categoria) && !(bool)ViewState[EnumViewState.IsTipoCalcoloModificato.ToString()])
                                {
                                    if (elencoSupplementiContrib != null && elencoSupplementiContrib.Count() > 0)
                                    {
                                        DatiSupplementi supplemento = elencoSupplementiContrib.ElementAt(e.Row.DataItemIndex);
                                        if (!supplemento.IsFromPrelievo)
                                        {
                                            this.HasError = true;
                                            this.ErrorMessage = "L'inserimento di un nuovo supplemento è consentito solo per le domande di Ricostituzione per supplemento";
                                            RaiseShowAvviso(this, null);
                                            return;
                                        }
                                    }
                                }
                            }
                            else if (e.Row.DataItemIndex == num - 1 && IsItemBlankPresent(elencoSupplementiContrib))
                            {
                                LinkButton add = ((LinkButton)(e.Row.Cells[0].Controls[0]));
                                add.Text = (string)"<img width=16 height=16 border=0 src=../App_themes/" + Page.Theme + "/Images/add24.png />";
                                add.ToolTip = "Aggiungi";
                            }
                            else
                            {
                                ((Label)e.Row.FindControl("lblTipoSupplementiContrib")).Text = ((DatiSupplementi)(e.Row.DataItem)).CodGestioneSupplemento;
                                ((Label)e.Row.FindControl("lblQuotaSupplementi")).Text = ((DatiSupplementi)(e.Row.DataItem)).QuotaSupplemento.HasValue ? ((DatiSupplementi)(e.Row.DataItem)).QuotaSupplemento.ToString() : string.Empty; //ENG - Modifica Supplementi CI Memo 177/2012
                                ((Label)e.Row.FindControl("lblDecorrenzaSupplementiContrib")).Text = String.Format("{0:MM/yyyy}", ((DatiSupplementi)(e.Row.DataItem)).DecorrenzaSupplemento);
                                ((Label)e.Row.FindControl("lblSettimaneSupplementiContrib")).Text = ((DatiSupplementi)(e.Row.DataItem)).NSettimaneSupplemento.HasValue ? ((DatiSupplementi)(e.Row.DataItem)).NSettimaneSupplemento.ToString() : string.Empty;
                                ((Label)e.Row.FindControl("lblMontanteIVS")).Text = ((DatiSupplementi)(e.Row.DataItem)).MontanteSupplemento.HasValue ? ((DatiSupplementi)(e.Row.DataItem)).MontanteSupplemento.ToString() : string.Empty;
                                ((Label)e.Row.FindControl("lblAmmontareContributivo")).Text = ((DatiSupplementi)(e.Row.DataItem)).AmmontareContributivo.HasValue ? ((DatiSupplementi)(e.Row.DataItem)).AmmontareContributivo.ToString() : string.Empty;
                                if (((DatiSupplementi)(e.Row.DataItem)).CodiceLiquidazione.HasValue && ((DatiSupplementi)(e.Row.DataItem)).CodiceLiquidazione.Value == 3)
                                    ((Label)e.Row.FindControl("lblCodiceLiquidazione")).Text = "C";
                                else if (((DatiSupplementi)(e.Row.DataItem)).CodiceLiquidazione.HasValue && ((DatiSupplementi)(e.Row.DataItem)).CodiceLiquidazione.Value == 4)
                                    ((Label)e.Row.FindControl("lblCodiceLiquidazione")).Text = "D";

                                if (CodeUtility.IsRicostituzione(this.TitolarePensione.Pensione) && (Utility.IsDomandaFPLD(this.domanda.Categoria) || Utility.IsDomandaGestioneAutonomi(this.domanda.Categoria) ||
                                        Utility.IsDomandaDAI(this.domanda.Categoria) || Utility.IsDomandaAUT(this.domanda.Categoria)))
                                {
                                    if (((CodeUtility.IsRicostituzioneSupplemento(this.TitolarePensione.Pensione) && !isRicSupplementoTipoContributivoMemo50) || CodeUtility.IsRicostituzioneContributiva(this.TitolarePensione.Pensione) || Utility.IsRicostituzione_MotiviDocumentali(this.TitolarePensione.Pensione) || Utility.IsRicostituzione_AccreditoPeriodiMaternita(this.TitolarePensione.Pensione)))
                                        EnableReadableMode(e.Row.Cells[0], e.Row.Cells[7]);
                                    else
                                    {
                                        LinkButton button = ((LinkButton)(e.Row.Cells[0].Controls[0]));
                                        button.Enabled = false;
                                        button.Text = "&nbsp;&nbsp;&nbsp;";
                                    }
                                }
                                else
                                    EnableReadableMode(e.Row.Cells[0], e.Row.Cells[7]);

                                if (isRicSupplementoTipoContributivoMemo50)
                                {
                                    LinkButton button = ((LinkButton)(e.Row.Cells[0].Controls[0]));
                                    button.Enabled = false;
                                    button.Text = "&nbsp;&nbsp;&nbsp;";
                                }
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
                throw new INPS.DNA.DnaApplicationException("UCSupplementiAgoCI, Errore nel metodo gvSupplementi_RowDataBound " + ex);
            }
        }

        protected void gvSupplementiContributivi_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            List<DatiSupplementi> elencoSupplementi = ViewState["elencoSupplementiContrib"] as List<DatiSupplementi>;
            removeItemBlank(ref elencoSupplementi);
            AddItemBlank(ref elencoSupplementi);
            int num = elencoSupplementi.Count;

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

            if (e.CommandName == "Delete")
            {
                this.modalitaEditContrib.Value = "false";

                Control c = (Control)e.CommandSource;
                GridViewRow r = (GridViewRow)c.NamingContainer;

                //ENG - MEMO 50/2023
                if (ViewState["AbilitazioneMemo50_2023"] != null && (string)ViewState["AbilitazioneMemo50_2023"] == "SI" && this.domanda.TipoAppartenenza.GetValueOrDefault() == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.AGO &&
                    Utility.IsRicostituzione_MotiviContributivi(TitolarePensione.Pensione) && TitolarePensione.Pensione.CodeTipo == "0001" && !this.domanda.IsDomandaENPALS && !Utility.IsDomandaCumulo(this.domanda.Categoria) && !(bool)ViewState[EnumViewState.IsTipoCalcoloModificato.ToString()])
                {
                    if (elencoSupplementi != null && elencoSupplementi.Count() > 0)
                    {
                        DatiSupplementi supplemento = elencoSupplementi.ElementAt(r.DataItemIndex);
                        if (supplemento.IsFromPrelievo)
                        {
                            this.HasError = true;
                            this.ErrorMessage = "Attenzione, non è possibile variare i dati del supplemento con il prodotto Webdom di ricostituzione per motivi contributivi di tipo ordinario. Utilizzare il prodotto Webdom di ricostituzione per variazione dati supplemento";
                            RaiseShowAvviso(this, null);
                            return;
                        }
                    }
                }
                elencoSupplementi.RemoveAt(r.DataItemIndex);

                if (elencoSupplementi.Count > 1)
                    gvSupplementiContributivi.EditIndex = -1;

                ViewState["elencoSupplementiContrib"] = elencoSupplementi;
                GvSupplementiContributivi_Load();

                ManageButtons(elencoSupplementi);
            }
            else if (e.CommandName == "Edit")
            {
                modalitaEditContrib.Value = "true";
            }
            else if (e.CommandName == "Salva")
            {
                this.modalitaEditContrib.Value = "false";

                Control c = (Control)e.CommandSource;
                GridViewRow r = (GridViewRow)c.NamingContainer;

                //ENG - MEMO 50/2023
                if (ViewState["AbilitazioneMemo50_2023"] != null && (string)ViewState["AbilitazioneMemo50_2023"] == "SI" && this.domanda.TipoAppartenenza.GetValueOrDefault() == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.AGO &&
                    Utility.IsRicostituzione_MotiviContributivi(TitolarePensione.Pensione) && TitolarePensione.Pensione.CodeTipo == "0001" && !this.domanda.IsDomandaENPALS && !Utility.IsDomandaCumulo(this.domanda.Categoria) && !(bool)ViewState[EnumViewState.IsTipoCalcoloModificato.ToString()])
                {
                    if (elencoSupplementi != null && elencoSupplementi.Count() > 0)
                    {
                        DatiSupplementi supplemento = elencoSupplementi.ElementAt(r.DataItemIndex);
                        if (supplemento.IsFromPrelievo)
                        {
                            this.HasError = true;
                            this.ErrorMessage = "Attenzione, non è possibile variare i dati del supplemento con il prodotto Webdom di ricostituzione per motivi contributivi di tipo ordinario. Utilizzare il prodotto Webdom di ricostituzione per variazione dati supplemento";
                            RaiseShowAvviso(this, null);
                            return;
                        }
                        else
                        {
                            this.HasError = true;
                            this.ErrorMessage = "L'inserimento di un nuovo supplemento è consentito solo per le domande di Ricostituzione per supplemento";
                            RaiseShowAvviso(this, null);
                            return;
                        }
                    }
                }

                if ((r.DataItemIndex - 1) == (num - 2))    //aggiunta riga (non si tratta di una modifica)
                {
                    DatiSupplementi supp = new DatiSupplementi();
                    supp.CodGestioneSupplemento = ((DropDownList)(r.Cells[1].Controls[1])).SelectedValue;
                    char? bQuotaSupplemento = null; //ENG - Modifica Supplementi CI Memo 177/2012
                    supp.QuotaSupplemento = !string.IsNullOrEmpty(((DropDownList)(r.Cells[2].Controls[1])).SelectedValue) ? Convert.ToChar(((DropDownList)(r.Cells[2].Controls[1])).SelectedValue) : bQuotaSupplemento;
                    if (!string.IsNullOrEmpty((((TextBox)(r.Cells[3].Controls[1])).Text)))
                        supp.DecorrenzaSupplemento = Convert.ToDateTime(((TextBox)(r.Cells[3].Controls[1])).Text);
                    if (!string.IsNullOrEmpty((((TextBox)(r.Cells[4].Controls[1])).Text)))
                        supp.NSettimaneSupplemento = Convert.ToInt32(((TextBox)(r.Cells[4].Controls[1])).Text);
                    if (!string.IsNullOrEmpty((((TextBox)(r.Cells[5].Controls[1])).Text)))
                        supp.MontanteSupplemento = Decimal.Parse(((TextBox)(r.Cells[5].Controls[1])).Text);
                    if (!string.IsNullOrEmpty((((TextBox)(r.Cells[6].Controls[1])).Text)))
                        supp.AmmontareContributivo = Decimal.Parse(((TextBox)(r.Cells[6].Controls[1])).Text);
                    byte? bCodiceLiquidazione = null;
                    supp.CodiceLiquidazione = !string.IsNullOrEmpty(((DropDownList)(r.Cells[7].Controls[1])).SelectedValue) ? Convert.ToByte(((DropDownList)(r.Cells[7].Controls[1])).SelectedValue) : bCodiceLiquidazione;

                    supp.TipoSupplemento = 'C';

                    elencoSupplementi.RemoveAt(num - 1);
                    elencoSupplementi.Add(supp);
                    AddItemBlank(ref elencoSupplementi);
                    //DatiSupplementi newsup = new DatiSupplementi();
                    //elencoSupplementi.Add(newsup);
                }
                else
                    saveValueRowContrib(elencoSupplementi, e, r);

                gvSupplementiContributivi.EditIndex = -1;
                ViewState["elencoSupplementiContrib"] = elencoSupplementi;
                GvSupplementiContributivi_Load();

                ManageButtons(elencoSupplementi);
            }
            else if (e.CommandName == "Annulla")
            {
                if (!IsListaEmptyContrib())
                {
                    modalitaEditContrib.Value = "false";
                    gvSupplementiContributivi.EditIndex = -1;
                    GvSupplementiContributivi_Load();
                }
            }
            GestioneTastoSalva();
        }

        protected void gvSupplementiContributivi_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            try
            {
                //Reset the edit index.
                gvSupplementiContributivi.EditIndex = -1;
                //Bind data to the GridView control.
                GvSupplementiContributivi_Load();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCSupplementiAgoCI, Errore nel metodo gvSupplementi_RowCancelingEdit " + ex);
            }
        }

        protected void gvSupplementiContributivi_RowEditing(object sender, GridViewEditEventArgs e)
        {
            try
            {
                gvSupplementiContributivi.EditIndex = e.NewEditIndex;
                GvSupplementiContributivi_Load();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCSupplementiAgoCI, Errore nel metodo gvSupplementi_RowEditing " + ex);
            }
        }

        protected void gvSupplementiContributivi_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            try
            {
                List<DatiSupplementi> elencoSupplementi = ViewState["elencoSupplementiContrib"] as List<DatiSupplementi>;

                if (elencoSupplementi.Count < 1)
                    inserisciSupplementiContrib();
                GvSupplementiContributivi_Load();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCSupplementiAgoCI, Errore nel metodo gvSupplementi_RowDeleting " + ex);
            }
        }

        protected void gvSupplementiContributivi_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            try
            {
                List<DatiSupplementi> elencoSupplementi = ViewState["elencoSupplementiContrib"] as List<DatiSupplementi>;
                GridViewRow row = gvSupplementiContributivi.Rows[e.RowIndex];
                if (((DropDownList)(row.Cells[2].Controls[1])).SelectedIndex != 0)
                {
                    int i = ((gvSupplementiContributivi.PageIndex * 10) + e.RowIndex);

                    if (elencoSupplementi.Count != i + 1)
                        elencoSupplementi.RemoveAt(elencoSupplementi.Count - 1);
                    gvSupplementiContributivi.EditIndex = -1;
                    ViewState["elencoSupplementiContrib"] = elencoSupplementi;
                    GvSupplementiContributivi_Load();
                }
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCSupplementiAgoCI, Errore nel metodo gvSupplementi_RowUpdating " + ex);
            }
        }

        protected void gvSupplementiContributivi_onPageIndexChanging(Object sender, GridViewPageEventArgs e)
        {
            try
            {
                gvSupplementiContributivi.PageIndex = e.NewPageIndex;
                GvSupplementiContributivi_Load();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCSupplementiAgoCI, Errore nel metodo gvSupplementi_onPageIndexChanging" + ex);
            }
        }

        #endregion Grid Contributivi

        #region Private Methods Grid Contributivi

        private void setDdlContrib(GridViewRowEventArgs e)
        {
            DropDownList ddlCodGestioneSupplementi = (DropDownList)e.Row.FindControl("ddlCodGestioneSupplementiContrib");
            if (ddlCodGestioneSupplementi != null)
            {
                List<TipoSupplementi> lTipoSupp = (List<TipoSupplementi>)ViewState["elencoTipoSupplementi"];
                foreach (TipoSupplementi tipo in lTipoSupp)
                {
                    ListItem item = new ListItem();
                    item.Text = item.Value = tipo.Id;
                    item.Attributes.Add("title", tipo.Descrizione);
                    ddlCodGestioneSupplementi.Items.Add(item);
                }
            }
        }

        private void setCampiEditContrib(GridViewRowEventArgs e)
        {
            DropDownList ddlQuotaSupplementi = new DropDownList(); //ENG - Modifica Supplementi CI Memo 177/2012
            ddlQuotaSupplementi = (DropDownList)e.Row.FindControl("ddlQuotaSupplementi");
            DropDownList ddlCodGestioneSupplementi = new DropDownList();
            ddlCodGestioneSupplementi = (DropDownList)e.Row.FindControl("ddlCodGestioneSupplementiContrib");
            TextBox txtDecorrenzaSupplementi = (TextBox)e.Row.FindControl("txtDecorrenzaSupplementiContrib");
            TextBox txtSettimaneSupplementi = (TextBox)e.Row.FindControl("txtSettimaneSupplementiContrib");
            TextBox txtMontanteIVS = (TextBox)e.Row.FindControl("txtMontanteIVS");
            TextBox txtAmmontareContributivo = (TextBox)e.Row.FindControl("txtAmmontareContributivo");
            DropDownList ddlCodiceLiquidazione = new DropDownList();
            ddlCodiceLiquidazione = (DropDownList)e.Row.FindControl("ddlCodiceLiquidazione");

            if (((DatiSupplementi)e.Row.DataItem).CodGestioneSupplemento != null)
                ddlCodGestioneSupplementi.SelectedValue = ((DatiSupplementi)e.Row.DataItem).CodGestioneSupplemento;
            ddlQuotaSupplementi.SelectedValue = ((DatiSupplementi)e.Row.DataItem).QuotaSupplemento.HasValue ? ((DatiSupplementi)e.Row.DataItem).QuotaSupplemento.ToString() : string.Empty; //ENG - Modifica Supplementi CI Memo 177/2012
            txtDecorrenzaSupplementi.Text = String.Format("{0:MM/yyyy}", ((DatiSupplementi)e.Row.DataItem).DecorrenzaSupplemento);
            txtSettimaneSupplementi.Text = ((DatiSupplementi)e.Row.DataItem).NSettimaneSupplemento.HasValue ? ((DatiSupplementi)e.Row.DataItem).NSettimaneSupplemento.ToString() : string.Empty;
            txtMontanteIVS.Text = ((DatiSupplementi)e.Row.DataItem).MontanteSupplemento.HasValue ? ((DatiSupplementi)e.Row.DataItem).MontanteSupplemento.ToString() : string.Empty;
            txtAmmontareContributivo.Text = ((DatiSupplementi)e.Row.DataItem).AmmontareContributivo.HasValue ? ((DatiSupplementi)e.Row.DataItem).AmmontareContributivo.ToString() : string.Empty;
            ddlCodiceLiquidazione.SelectedValue = ((DatiSupplementi)e.Row.DataItem).CodiceLiquidazione.HasValue ? ((DatiSupplementi)e.Row.DataItem).CodiceLiquidazione.ToString() : string.Empty;
        }

        private void saveValueRowContrib(List<DatiSupplementi> elencoSupplementi, GridViewCommandEventArgs e, GridViewRow r)
        {
            if (!String.IsNullOrEmpty((((DropDownList)(r.Cells[1].Controls[1])).SelectedValue)))
                elencoSupplementi[r.DataItemIndex].CodGestioneSupplemento = ((DropDownList)(r.Cells[1].Controls[1])).SelectedValue;
            if (!String.IsNullOrEmpty((((DropDownList)(r.Cells[2].Controls[1])).SelectedValue))) //ENG - Modifica Supplementi CI Memo 177/2012
                elencoSupplementi[r.DataItemIndex].QuotaSupplemento = (((DropDownList)(r.Cells[2].Controls[1])).SelectedValue)[0];
            else
                elencoSupplementi[r.DataItemIndex].QuotaSupplemento = null; // Campo non obbligatorio
            if (!String.IsNullOrEmpty((((TextBox)(r.Cells[3].Controls[1])).Text)))
                elencoSupplementi[r.DataItemIndex].DecorrenzaSupplemento = Convert.ToDateTime(((TextBox)(r.Cells[3].Controls[1])).Text);
            if (!String.IsNullOrEmpty((((TextBox)(r.Cells[4].Controls[1])).Text)))
                elencoSupplementi[r.DataItemIndex].NSettimaneSupplemento = Convert.ToInt32(((TextBox)(r.Cells[4].Controls[1])).Text);
            if (!String.IsNullOrEmpty((((TextBox)(r.Cells[5].Controls[1])).Text)))
                elencoSupplementi[r.DataItemIndex].MontanteSupplemento = Decimal.Parse(((TextBox)(r.Cells[5].Controls[1])).Text);
            else
                elencoSupplementi[r.DataItemIndex].MontanteSupplemento = null; // Campo non obbligatorio
            if (!String.IsNullOrEmpty((((TextBox)(r.Cells[6].Controls[1])).Text)))
                elencoSupplementi[r.DataItemIndex].AmmontareContributivo = Decimal.Parse(((TextBox)(r.Cells[6].Controls[1])).Text);
            else
                elencoSupplementi[r.DataItemIndex].AmmontareContributivo = null; // Campo non obbligatorio
            if (!String.IsNullOrEmpty((((DropDownList)(r.Cells[7].Controls[1])).SelectedValue)))
                elencoSupplementi[r.DataItemIndex].CodiceLiquidazione = byte.Parse(((DropDownList)(r.Cells[7].Controls[1])).SelectedValue);
            else
                elencoSupplementi[r.DataItemIndex].CodiceLiquidazione = null; // Campo non obbligatorio
        }

        private void GvSupplementiContributivi_Load()
        {
            try
            {
                if (this.domanda == null)
                    this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
                List<DatiSupplementi> elencoSupplementi = ViewState["elencoSupplementiContrib"] as List<DatiSupplementi>;
                AddItemBlank(ref elencoSupplementi);
                gvSupplementiContributivi.DataSource = elencoSupplementi;
                gvSupplementiContributivi.DataBind();
                if (Utility.IsDomandaRipristino(this.TitolarePensione.Pensione))
                    gvSupplementiContributivi.Enabled = false;
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCSupplementiAgoCI, Errore nel metodo GvSupplementi_Load " + ex);
            }
        }

        private void inserisciSupplementiContrib()
        {
            try
            {
                List<DatiSupplementi> elencoSupplementi = ViewState["elencoSupplementiContrib"] as List<DatiSupplementi>;
                DatiSupplementi supp = new DatiSupplementi();
                elencoSupplementi.Add(supp);
                ViewState["elencoSupplementiContrib"] = elencoSupplementi;
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCSupplementiAgoCI, Errore nel metodo InserisciSupplementi " + ex);
            }
        }

        private bool IsListaEmptyContrib()
        {
            List<DatiSupplementi> listaDatiSuppl = ViewState["elencoSupplementiContrib"] as List<DatiSupplementi>;

            if (listaDatiSuppl.Count == 1 && string.IsNullOrEmpty(listaDatiSuppl[0].CodGestioneSupplemento) &&
                string.IsNullOrEmpty(listaDatiSuppl[0].DecorrenzaSupplemento.ToString()) && string.IsNullOrEmpty(listaDatiSuppl[0].MontanteSupplemento.ToString()) &&
                string.IsNullOrEmpty(listaDatiSuppl[0].QuotaSupplemento.ToString()) && string.IsNullOrEmpty(listaDatiSuppl[0].RMSSupplemento.ToString()) &&
                string.IsNullOrEmpty(listaDatiSuppl[0].TipoSupplemento.ToString()) && string.IsNullOrEmpty(listaDatiSuppl[0].NSettimaneSupplemento.ToString()))
                return true;
            else
                return false;
        }

        private bool IsEmptyEditableRowContrib(GridViewRow row)
        {
            if (row.FindControl("ddlCodGestioneSupplementiContrib") != null && ((DropDownList)row.FindControl("ddlCodGestioneSupplementiContrib")).SelectedIndex != 0 &&
                row.FindControl("txtDecorrenzaSupplementiContrib") != null && ((TextBox)row.FindControl("txtDecorrenzaSupplementiContrib")).Text != string.Empty &&
                row.FindControl("txtSettimaneSupplementiContrib") != null && ((TextBox)row.FindControl("txtSettimaneSupplementiContrib")).Text != string.Empty &&
                row.FindControl("txtMontanteIVS") != null && ((TextBox)row.FindControl("txtMontanteIVS")).Text != string.Empty &&
                row.FindControl("txtAmmontareContributivo") != null && ((TextBox)row.FindControl("txtAmmontareContributivo")).Text != string.Empty &&
                row.FindControl("ddlCodiceLiquidazione") != null && ((DropDownList)row.FindControl("ddlCodiceLiquidazione")).SelectedIndex != 0)
                return false;
            else
                return true;

        }

        private bool IsEmptyReadableRowContrib(GridViewRow row)
        {
            if (row.FindControl("lblCodGestioneSupplementiContrib") != null && ((DropDownList)row.FindControl("lblCodGestioneSupplementiContrib")).SelectedIndex != 0 &&
                row.FindControl("lblDecorrenzaSupplementiContrib") != null && ((TextBox)row.FindControl("lblDecorrenzaSupplementiContrib")).Text != string.Empty &&
                row.FindControl("lblSettimaneSupplementiContrib") != null && ((TextBox)row.FindControl("lblSettimaneSupplementiContrib")).Text != string.Empty &&
                row.FindControl("lblMontanteIVS") != null && ((TextBox)row.FindControl("lblMontanteIVS")).Text != string.Empty &&
                row.FindControl("lblAmmontareContributivo") != null && ((TextBox)row.FindControl("lblAmmontareContributivo")).Text != string.Empty &&
                row.FindControl("lblCodiceLiquidazione") != null && ((DropDownList)row.FindControl("lblCodiceLiquidazione")).SelectedIndex != 0)
                return false;
            else
                return true;
        }

        private void EnableEditableModeContrib(TableCell cell_CancelSave)
        {
            cell_CancelSave.Width = new Unit(40, UnitType.Pixel);




            LinkButton cancel = ((LinkButton)(cell_CancelSave.Controls[2]));
            cancel.Text = (string)"<img width=16 height=16 border=0 src=../App_themes/" + Page.Theme + "/Images/cancel24.png />";
            cancel.ToolTip = "Annulla";
            cancel.CommandName = "Annulla";

            LinkButton save = ((LinkButton)(cell_CancelSave.Controls[0]));
            save.Text = (string)"<img width=16 height=16 border=0 src=../App_themes/" + Page.Theme + "/Images/save24.png />";
            save.ToolTip = "Salva";
            save.CommandName = "Salva";
            save.CausesValidation = true;
            save.ValidationGroup = "UCTabSupplementiContrib";

        }

        #endregion Private Methods Grid Contributivi

        #region Events
        public event Utility.CustomEventHandler SalvaSupplementi;
        public event Utility.CustomEventHandler EliminaSupplementi;
        public event EventHandler ErrorSalvaSupplementi;
        public event EventHandler AbilitaTastoSalva;
        public event EventHandler DisabilitaTastoSalva;
        public event EventHandler GetDecorrenzaPensione;
        public event EventHandler ShowPopUp;
        public event EventHandler HidePopUp;
        public event EventHandler ShowAvviso;

        protected void RaiseAbilitaTastoSalva(object sender, EventArgs e)
        {
            if (AbilitaTastoSalva != null)
                AbilitaTastoSalva(sender, e);
        }

        protected void RaiseDisabilitaTastoSalva(object sender, EventArgs e)
        {
            if (DisabilitaTastoSalva != null)
                DisabilitaTastoSalva(sender, e);
        }

        protected void RaiseSalvaSupplementi(object sender, Utility.CustomEventArgs e)
        {
            if (SalvaSupplementi != null)
                SalvaSupplementi(sender, e);
        }

        protected void RaiseEliminaSupplementi(object sender, Utility.CustomEventArgs e)
        {
            if (EliminaSupplementi != null)
                EliminaSupplementi(sender, e);
        }

        protected void RaiseErrorSalvaSupplementi(object sender, EventArgs e)
        {
            if (ErrorSalvaSupplementi != null)
                ErrorSalvaSupplementi(sender, e);
        }

        protected void RaiseShowPopUp(object sender, EventArgs e)
        {
            if (ShowPopUp != null)
                ShowPopUp(sender, e);
        }

        protected void RaiseHidePopUp(object sender, EventArgs e)
        {
            if (HidePopUp != null)
                HidePopUp(sender, e);
        }

        protected void RaiseShowAvviso(object sender, EventArgs e)
        {
            ShowAvviso(sender, e);
        }
        #endregion Events

        #region enum
        enum EnumViewState
        {
            IsDomandaSperimentaleDonna,
            DecodificaTipoQuota,
            IsReversibilitaOrRicostituzione,
            IsPannelloSupplementiAnte96,
            IsTipoCalcoloModificato
        }
        #endregion enum

        #region Grid ante96
        protected void gvSupplementiAnte96_RowEditing(object sender, GridViewEditEventArgs e)
        {
            try
            {
                gvSupplementiAnte96.EditIndex = e.NewEditIndex;
                GvSupplementiAnte96_Load();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCSupplementiAgoCI, Errore nel metodo gvSupplementi_RowEditing " + ex);
            }
        }

        protected void gvSupplementiAnte96_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            try
            {
                gvSupplementiAnte96.PageIndex = e.NewPageIndex;
                GvSupplementiAnte96_Load();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCSupplementiAgoCI, Errore nel metodo gvSupplementiAnte96_onPageIndexChanging" + ex);
            }
        }

        protected void gvSupplementiAnte96_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            List<DatiSupplementi> elencoSupplementi = ViewState["elencoSupplementiAnte96"] as List<DatiSupplementi>;
            removeItemBlank(ref elencoSupplementi);
            AddItemBlank(ref elencoSupplementi);
            int num = elencoSupplementi.Count;

            if (e.CommandName == "Delete")
            {
                this.modalitaEditAnte96.Value = "false";

                Control c = (Control)e.CommandSource;
                GridViewRow r = (GridViewRow)c.NamingContainer;


                //ENG - MEMO 50/2023
                if (this.domanda.TipoAppartenenza.GetValueOrDefault() == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.AGO &&
                    Utility.IsRicostituzione_MotiviContributivi(TitolarePensione.Pensione) && TitolarePensione.Pensione.CodeTipo == "0001" && !this.domanda.IsDomandaENPALS && !Utility.IsDomandaCumulo(this.domanda.Categoria))
                {
                    if (elencoSupplementi != null && elencoSupplementi.Count() > 0)
                    {
                        DatiSupplementi supplemento = elencoSupplementi.ElementAt(r.DataItemIndex);
                        if (supplemento.IsFromPrelievo)
                        {
                            this.HasError = true;
                            this.ErrorMessage = "Attenzione, non è possibile variare i dati del supplemento con il prodotto Webdom di ricostituzione per motivi contributivi di tipo ordinario. Utilizzare il prodotto Webdom di ricostituzione per variazione dati supplemento";
                            RaiseShowAvviso(this, null);
                            return;
                        }
                    }
                }

                elencoSupplementi.RemoveAt(r.DataItemIndex);

                if (elencoSupplementi.Count > 1)
                    gvSupplementiAnte96.EditIndex = -1;

                ViewState["elencoSupplementiAnte96"] = elencoSupplementi;
                GvSupplementiAnte96_Load();

                ManageButtons(elencoSupplementi);
            }
            else if (e.CommandName == "Edit")
            {
                modalitaEditAnte96.Value = "true";
            }
            else if (e.CommandName == "Salva")
            {
                this.modalitaEditAnte96.Value = "false";

                Control c = (Control)e.CommandSource;
                GridViewRow r = (GridViewRow)c.NamingContainer;

                string errMessage = string.Empty;


                if (this.domanda.TipoAppartenenza.GetValueOrDefault() == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.AGO &&
                    !this.domanda.IsDomandaENPALS && !Utility.IsDomandaCumulo(this.domanda.Categoria) && CodeUtility.IsRicostituzione(this.TitolarePensione.Pensione) && !(CodeUtility.IsRicostituzioneSupplemento(this.TitolarePensione.Pensione) || Utility.IsRicostituzione_PerVariazioneDatiSupplemento(this.TitolarePensione.Pensione)))
                {
                    if (elencoSupplementi != null && elencoSupplementi.Count() > 0)
                    {
                        DatiSupplementi supplemento = elencoSupplementi.ElementAt(r.DataItemIndex);
                        if (supplemento.IsFromPrelievo)
                        {
                            this.HasError = true;
                            this.ErrorMessage = "Attenzione, non è possibile variare i dati del supplemento con il prodotto Webdom utilizzato. Utilizzare il prodotto Webdom di ricostituzione per variazione dati supplemento";
                            RaiseShowAvviso(this, null);
                            return;
                        }
                        else
                        {
                            this.HasError = true;
                            this.ErrorMessage = "L'inserimento di un nuovo supplemento è consentito solo per le domande di Ricostituzione per supplemento";
                            RaiseShowAvviso(this, null);
                            return;
                        }
                    }
                }

                if ((r.DataItemIndex - 1) == (num - 2))    //aggiunta riga (non si tratta di una modifica)
                {
                    DatiSupplementi supp = new DatiSupplementi();
                    supp.CodGestioneSupplemento = ((DropDownList)(r.Cells[2].Controls[1])).SelectedValue;
                    char? bQuotaSupplemento = null;
                    supp.QuotaSupplemento = !string.IsNullOrEmpty(((DropDownList)(r.Cells[3].Controls[1])).SelectedValue) ? Convert.ToChar(((DropDownList)(r.Cells[3].Controls[1])).SelectedValue) : bQuotaSupplemento;
                    if (!String.IsNullOrEmpty(((TextBox)(r.Cells[6].Controls[1])).Text))
                        supp.RMSSupplemento = Decimal.Parse(((TextBox)(r.Cells[6].Controls[1])).Text);
                    if (!string.IsNullOrEmpty((((TextBox)(r.Cells[1].Controls[1])).Text)))
                        supp.DecorrenzaSupplemento = Convert.ToDateTime(((TextBox)(r.Cells[1].Controls[1])).Text);
                    if (!string.IsNullOrEmpty((((TextBox)(r.Cells[5].Controls[1])).Text)))
                        supp.NSettimaneSupplemento = Convert.ToInt32(((TextBox)(r.Cells[5].Controls[1])).Text);
                    if (!string.IsNullOrEmpty((((TextBox)(r.Cells[4].Controls[1])).Text)))
                        supp.MontanteSupplemento = Decimal.Parse(((TextBox)(r.Cells[4].Controls[1])).Text);
                    if (!string.IsNullOrEmpty((((TextBox)(r.Cells[7].Controls[1])).Text)))
                        supp.AmmontareContributivo = Decimal.Parse(((TextBox)(r.Cells[7].Controls[1])).Text);
                    byte? bCodiceLiquidazione = null;
                    supp.CodiceLiquidazione = !string.IsNullOrEmpty(((DropDownList)(r.Cells[8].Controls[1])).SelectedValue) ? Convert.ToByte(((DropDownList)(r.Cells[8].Controls[1])).SelectedValue) : bCodiceLiquidazione;

                    //VALUATARE TIPO
                    if (supp.RMSSupplemento.HasValue)
                        supp.TipoSupplemento = 'R';
                    else
                        supp.TipoSupplemento = 'C';


                    //DateTime? decorrenza = !string.IsNullOrEmpty(((TextBox)(r.Cells[1].Controls[1])).Text) ? Convert.ToDateTime(((TextBox)(r.Cells[1].Controls[1])).Text) : (DateTime?)null;
                    //if (decorrenza.HasValue && Utility.DataStrettamenteSuccessivaA(new DateTime(1995, 12, 31), decorrenza.Value))
                    //{
                    //    this.HasError = true;
                    //    this.ErrorMessage = "Attenzione, non è possibile inserire supplementi precedenti al 1996";
                    //    RaiseShowAvviso(this, null);
                    //    return;
                    //}

                    //Controllo combinazioni valide di dati
                    errMessage = CheckCombinazioniAnte96(r);
                    if (!string.IsNullOrEmpty(errMessage))
                    {
                        this.HasError = true;
                        this.ErrorMessage = errMessage;
                        RaiseShowAvviso(this, null);
                        return;
                    }


                    elencoSupplementi.RemoveAt(num - 1);
                    elencoSupplementi.Add(supp);
                    AddItemBlank(ref elencoSupplementi);
                }
                else
                {
                    //if (elencoSupplementi != null && elencoSupplementi.Count() > 0)
                    //{
                    //    DatiSupplementi supplemento = elencoSupplementi.ElementAt(r.DataItemIndex);
                    //    DateTime? decorrenza = !string.IsNullOrEmpty(((TextBox)(r.Cells[1].Controls[1])).Text) ? Convert.ToDateTime(((TextBox)(r.Cells[1].Controls[1])).Text) : (DateTime?)null;
                    //    if (decorrenza.HasValue && Utility.DataStrettamenteSuccessivaA(new DateTime(1995, 12, 31), decorrenza.Value))
                    //    {
                    //        this.HasError = true;
                    //        this.ErrorMessage = "Attenzione, non è possibile inserire supplementi precedenti al 1996";
                    //        RaiseShowAvviso(this, null);
                    //        return;
                    //    }
                    //}
                    //Controllo combinazioni valide di dati
                    errMessage = CheckCombinazioniAnte96(r);
                    if (!string.IsNullOrEmpty(errMessage))
                    {
                        this.HasError = true;
                        this.ErrorMessage = errMessage;
                        RaiseShowAvviso(this, null);
                        return;
                    }
                    saveValueRowAnte96(elencoSupplementi, e, r);
                }

                gvSupplementiAnte96.EditIndex = -1;
                ViewState["elencoSupplementiAnte96"] = elencoSupplementi;
                GvSupplementiAnte96_Load();

                ManageButtons(elencoSupplementi);
            }
            else if (e.CommandName == "Annulla")
            {
                if (!IsListaEmptyAnte96())
                {
                    modalitaEditAnte96.Value = "false";
                    gvSupplementiAnte96.EditIndex = -1;
                    GvSupplementiAnte96_Load();
                }
            }
            GestioneTastoSalva();

        }

        private string CheckCombinazioniAnte96(GridViewRow r)
        {
            string message = string.Empty;
            if (String.IsNullOrEmpty(((TextBox)(r.Cells[6].Controls[1])).Text) && string.IsNullOrEmpty(((DropDownList)(r.Cells[3].Controls[1])).SelectedValue) && string.IsNullOrEmpty((((TextBox)(r.Cells[4].Controls[1])).Text)) && string.IsNullOrEmpty((((TextBox)(r.Cells[7].Controls[1])).Text)) && string.IsNullOrEmpty(((DropDownList)(r.Cells[8].Controls[1])).SelectedValue))
                message = "Inserire Decorrenza, Codice gestione, Settimane e almeno uno tra le seguenti accoppiate: “Retribuzione / Reddito medio” e Quota, oppure “RMS Sentenza 72 / IVS / Montante” e “RMS articolo 2/ Importo Contributivo” e Codice liquidazione";

            if (!String.IsNullOrEmpty(((TextBox)(r.Cells[6].Controls[1])).Text) && (!string.IsNullOrEmpty((((TextBox)(r.Cells[4].Controls[1])).Text)) || !string.IsNullOrEmpty((((TextBox)(r.Cells[7].Controls[1])).Text)) || !string.IsNullOrEmpty(((DropDownList)(r.Cells[8].Controls[1])).SelectedValue)))
                message = "Se inserito “Retribuzione / Reddito medio”, non deve essere inserito “RMS Sentenza 72 / IVS / Montante” o “RMS articolo 2/ Importo Contributivo” o Codice liquidazione e viceversa";

            if (!string.IsNullOrEmpty((((TextBox)(r.Cells[4].Controls[1])).Text)) || !string.IsNullOrEmpty((((TextBox)(r.Cells[7].Controls[1])).Text)) || !string.IsNullOrEmpty(((DropDownList)(r.Cells[8].Controls[1])).SelectedValue))
                if (string.IsNullOrEmpty((((TextBox)(r.Cells[4].Controls[1])).Text)) || string.IsNullOrEmpty((((TextBox)(r.Cells[7].Controls[1])).Text)) || string.IsNullOrEmpty(((DropDownList)(r.Cells[8].Controls[1])).SelectedValue))
                    message = "In presenza di un montante o ammontare di contributo è necessario compilare anche il codice liquidazione. Si veda il messaggio 13989/2007.";

            if (!string.IsNullOrEmpty(((DropDownList)(r.Cells[3].Controls[1])).SelectedValue) || !String.IsNullOrEmpty(((TextBox)(r.Cells[6].Controls[1])).Text))
                if (string.IsNullOrEmpty(((DropDownList)(r.Cells[3].Controls[1])).SelectedValue) || String.IsNullOrEmpty(((TextBox)(r.Cells[6].Controls[1])).Text))
                    message = "Se inserito il campo “Retribuzione / Reddito medio” deve essere inserito il campo “Quota” e viceversa";

            return message;
        }

        protected void gvSupplementiAnte96_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            try
            {
                //Reset the edit index.
                gvSupplementiAnte96.EditIndex = -1;

                //Bind data to the GridView control.
                GvSupplementiAnte96_Load();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCSupplementiAgoCI, Errore nel metodo gvSupplementiAnte96_RowCancelingEdit " + ex);
            }
        }

        protected void gvSupplementiAnte96_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            try
            {
                List<DatiSupplementi> elencoSupplementi = ViewState["elencoSupplementiAnte96"] as List<DatiSupplementi>;
                GridViewRow row = gvSupplementiAnte96.Rows[e.RowIndex];
                if (((DropDownList)(row.Cells[2].Controls[1])).SelectedIndex != 0)
                {
                    int i = ((gvSupplementiAnte96.PageIndex * 10) + e.RowIndex);

                    if (elencoSupplementi.Count != i + 1)
                        elencoSupplementi.RemoveAt(elencoSupplementi.Count - 1);
                    gvSupplementiContributivi.EditIndex = -1;
                    ViewState["elencoSupplementiAnte96"] = elencoSupplementi;
                    GvSupplementiAnte96_Load();
                }
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCSupplementiAgoCI, Errore nel metodo gvSupplementiAnte96_RowUpdating " + ex);
            }
        }

        protected void gvSupplementiAnte96_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (this.domanda == null)
                    this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

                if (this.TitolarePensione == null)
                    this.TitolarePensione = new AreaTitolare();
                if (this.TitolarePensione.Pensione == null)
                    this.TitolarePensione.Pensione = GetDatiPensione(this);

                List<DatiSupplementi> elencoSupplementiContrib = ViewState["elencoSupplementiAnte96"] as List<DatiSupplementi>;

                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    setDdlAnte96(e);
                    int num = ((List<DatiSupplementi>)ViewState["elencoSupplementiAnte96"]).Count;

                    //prima riga
                    if (e.Row.DataItemIndex == 0)
                    {
                        //vuota
                        if (IsListaEmptyAnte96() && !Convert.ToBoolean(modalitaEditAnte96.Value))
                        {
                            gvSupplementiAnte96.EditIndex = 0;
                            modalitaEditAnte96.Value = "true";
                            GvSupplementiAnte96_Load();
                            GestioneTastoSalva();
                        }
                        else
                        {
                            if (e.Row.Cells[0].Controls.Count == 3)  // edit mode
                            {
                                setCampiEditAnte96(e);
                                EnableEditableModeContrib(e.Row.Cells[0]);
                            }
                            else
                            {
                                ((Label)e.Row.FindControl("lblTipoSupplementi")).Text = ((DatiSupplementi)(e.Row.DataItem)).CodGestioneSupplemento;
                                ((Label)e.Row.FindControl("lblQuotaSupplementi")).Text = ((DatiSupplementi)(e.Row.DataItem)).QuotaSupplemento.HasValue ? ((DatiSupplementi)(e.Row.DataItem)).QuotaSupplemento.ToString() : string.Empty; //ENG - Modifica Supplementi CI Memo 177/2012
                                ((Label)e.Row.FindControl("lblDecorrenzaSupplementi")).Text = String.Format("{0:MM/yyyy}", ((DatiSupplementi)(e.Row.DataItem)).DecorrenzaSupplemento);
                                ((Label)e.Row.FindControl("lblSettimaneSupplementi")).Text = ((DatiSupplementi)(e.Row.DataItem)).NSettimaneSupplemento.HasValue ? ((DatiSupplementi)(e.Row.DataItem)).NSettimaneSupplemento.ToString() : string.Empty;
                                ((Label)e.Row.FindControl("lblMontanteIVS")).Text = ((DatiSupplementi)(e.Row.DataItem)).MontanteSupplemento.HasValue ? ((DatiSupplementi)(e.Row.DataItem)).MontanteSupplemento.ToString() : string.Empty;
                                ((Label)e.Row.FindControl("lblAmmontareContributivo")).Text = ((DatiSupplementi)(e.Row.DataItem)).AmmontareContributivo.HasValue ? ((DatiSupplementi)(e.Row.DataItem)).AmmontareContributivo.ToString() : string.Empty;
                                if (((DatiSupplementi)(e.Row.DataItem)).CodiceLiquidazione.HasValue && ((DatiSupplementi)(e.Row.DataItem)).CodiceLiquidazione.Value == 3)
                                    ((Label)e.Row.FindControl("lblCodiceLiquidazione")).Text = "C";
                                else if (((DatiSupplementi)(e.Row.DataItem)).CodiceLiquidazione.HasValue && ((DatiSupplementi)(e.Row.DataItem)).CodiceLiquidazione.Value == 4)
                                    ((Label)e.Row.FindControl("lblCodiceLiquidazione")).Text = "D";

                                EnableReadableMode(e.Row.Cells[0], e.Row.Cells[8]);


                                //if (((DatiSupplementi)(e.Row.DataItem)).DecorrenzaSupplemento.HasValue && Utility.DataStrettamenteSuccessivaA(new DateTime(1995, 12, 31), ((DatiSupplementi)(e.Row.DataItem)).DecorrenzaSupplemento.Value))
                                //{
                                //    LinkButton button = ((LinkButton)(e.Row.Cells[0].Controls[0]));
                                //    button.Enabled = false;
                                //    button.Text = "&nbsp;&nbsp;&nbsp;";
                                //    LinkButton delete = ((LinkButton)(e.Row.Cells[8].FindControl("btnDelete")));
                                //    delete.Enabled = false;
                                //    delete.Text = "&nbsp;&nbsp;&nbsp;";
                                //}
                                if (!Utility.IsRicostituzione_Supplemento(this.TitolarePensione.Pensione) && !Utility.IsRicostituzione_PerVariazioneDatiSupplemento(this.TitolarePensione.Pensione))
                                {
                                    LinkButton delete = ((LinkButton)(e.Row.Cells[8].FindControl("btnDelete")));
                                    delete.Enabled = false;
                                    delete.Text = "&nbsp;&nbsp;&nbsp;";
                                }
                            }
                        }
                    }
                    else // righe successive
                    {
                        if (e.Row.Cells[0].Controls.Count == 3)  // edit mode
                        {
                            setCampiEditAnte96(e);
                            EnableEditableModeContrib(e.Row.Cells[0]);
                        }
                        else if (e.Row.DataItemIndex == num - 1 && IsItemBlankPresent(elencoSupplementiContrib))
                        {
                            LinkButton add = ((LinkButton)(e.Row.Cells[0].Controls[0]));
                            add.Text = (string)"<img width=16 height=16 border=0 src=../App_themes/" + Page.Theme + "/Images/add24.png />";
                            add.ToolTip = "Aggiungi";
                        }
                        else
                        {
                            ((Label)e.Row.FindControl("lblTipoSupplementi")).Text = ((DatiSupplementi)(e.Row.DataItem)).CodGestioneSupplemento;
                            ((Label)e.Row.FindControl("lblQuotaSupplementi")).Text = ((DatiSupplementi)(e.Row.DataItem)).QuotaSupplemento.HasValue ? ((DatiSupplementi)(e.Row.DataItem)).QuotaSupplemento.ToString() : string.Empty; //ENG - Modifica Supplementi CI Memo 177/2012
                            ((Label)e.Row.FindControl("lblDecorrenzaSupplementi")).Text = String.Format("{0:MM/yyyy}", ((DatiSupplementi)(e.Row.DataItem)).DecorrenzaSupplemento);
                            ((Label)e.Row.FindControl("lblSettimaneSupplementi")).Text = ((DatiSupplementi)(e.Row.DataItem)).NSettimaneSupplemento.HasValue ? ((DatiSupplementi)(e.Row.DataItem)).NSettimaneSupplemento.ToString() : string.Empty;
                            ((Label)e.Row.FindControl("lblMontanteIVS")).Text = ((DatiSupplementi)(e.Row.DataItem)).MontanteSupplemento.HasValue ? ((DatiSupplementi)(e.Row.DataItem)).MontanteSupplemento.ToString() : string.Empty;
                            ((Label)e.Row.FindControl("lblAmmontareContributivo")).Text = ((DatiSupplementi)(e.Row.DataItem)).AmmontareContributivo.HasValue ? ((DatiSupplementi)(e.Row.DataItem)).AmmontareContributivo.ToString() : string.Empty;
                            if (((DatiSupplementi)(e.Row.DataItem)).CodiceLiquidazione.HasValue && ((DatiSupplementi)(e.Row.DataItem)).CodiceLiquidazione.Value == 3)
                                ((Label)e.Row.FindControl("lblCodiceLiquidazione")).Text = "C";
                            else if (((DatiSupplementi)(e.Row.DataItem)).CodiceLiquidazione.HasValue && ((DatiSupplementi)(e.Row.DataItem)).CodiceLiquidazione.Value == 4)
                                ((Label)e.Row.FindControl("lblCodiceLiquidazione")).Text = "D";

                            EnableReadableMode(e.Row.Cells[0], e.Row.Cells[7]);


                            //if (((DatiSupplementi)(e.Row.DataItem)).DecorrenzaSupplemento.HasValue && Utility.DataStrettamenteSuccessivaA(new DateTime(1995, 12, 31), ((DatiSupplementi)(e.Row.DataItem)).DecorrenzaSupplemento.Value))
                            //{
                            //    LinkButton button = ((LinkButton)(e.Row.Cells[0].Controls[0]));
                            //    button.Enabled = false;
                            //    button.Text = "&nbsp;&nbsp;&nbsp;";
                            //    LinkButton delete = ((LinkButton)(e.Row.Cells[8].FindControl("btnDelete")));
                            //    delete.Enabled = false;
                            //    delete.Text = "&nbsp;&nbsp;&nbsp;";
                            //}
                            if (!Utility.IsRicostituzione_Supplemento(this.TitolarePensione.Pensione) && !Utility.IsRicostituzione_PerVariazioneDatiSupplemento(this.TitolarePensione.Pensione))
                            {
                                LinkButton delete = ((LinkButton)(e.Row.Cells[8].FindControl("btnDelete")));
                                delete.Enabled = false;
                                delete.Text = "&nbsp;&nbsp;&nbsp;";
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
                throw new INPS.DNA.DnaApplicationException("UCSupplementiAgoCI, Errore nel metodo gvSupplementiAnte96_RowDataBound " + ex);
            }
        }

        protected void gvSupplementiAnte96_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            try
            {
                List<DatiSupplementi> elencoSupplementi = ViewState["elencoSupplementiAnte96"] as List<DatiSupplementi>;

                if (elencoSupplementi.Count < 1)
                    inserisciSupplementiAnte96();
                GvSupplementiAnte96_Load();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCSupplementiAgoCI, Errore nel metodo gvSupplementiAnte96_RowDeleting " + ex);
            }
        }
        #endregion

        #region Private Methods Grid ante96

        private void setDdlAnte96(GridViewRowEventArgs e)
        {
            DropDownList ddlCodGestioneSupplementi = (DropDownList)e.Row.FindControl("ddlCodGestioneSupplementi");
            if (ddlCodGestioneSupplementi != null)
            {
                List<TipoSupplementi> lTipoSupp = (List<TipoSupplementi>)ViewState["elencoTipoSupplementi"];
                foreach (TipoSupplementi tipo in lTipoSupp)
                {
                    ListItem item = new ListItem();
                    item.Text = item.Value = tipo.Id;
                    item.Attributes.Add("title", tipo.Descrizione);
                    ddlCodGestioneSupplementi.Items.Add(item);
                }
            }
        }

        private void setCampiEditAnte96(GridViewRowEventArgs e)
        {
            DropDownList ddlQuotaSupplementi = new DropDownList();
            ddlQuotaSupplementi = (DropDownList)e.Row.FindControl("ddlQuotaSupplementi");
            DropDownList ddlCodGestioneSupplementi = new DropDownList();
            ddlCodGestioneSupplementi = (DropDownList)e.Row.FindControl("ddlCodGestioneSupplementi");
            TextBox txtDecorrenzaSupplementi = (TextBox)e.Row.FindControl("txtDecorrenzaSupplementi");
            TextBox txtSettimaneSupplementi = (TextBox)e.Row.FindControl("txtSettimaneSupplementi");
            TextBox txtMontanteIVS = (TextBox)e.Row.FindControl("txtMontanteIVS");
            TextBox txtAmmontareContributivo = (TextBox)e.Row.FindControl("txtAmmontareContributivo");
            DropDownList ddlCodiceLiquidazione = new DropDownList();
            ddlCodiceLiquidazione = (DropDownList)e.Row.FindControl("ddlCodiceLiquidazione");
            TextBox txtRMSSupplementi = (TextBox)e.Row.FindControl("txtRMSSupplementi");

            if (((DatiSupplementi)e.Row.DataItem).CodGestioneSupplemento != null)
                ddlCodGestioneSupplementi.SelectedValue = ((DatiSupplementi)e.Row.DataItem).CodGestioneSupplemento;
            ddlQuotaSupplementi.SelectedValue = ((DatiSupplementi)e.Row.DataItem).QuotaSupplemento.HasValue ? ((DatiSupplementi)e.Row.DataItem).QuotaSupplemento.ToString() : string.Empty; //ENG - Modifica Supplementi CI Memo 177/2012
            txtDecorrenzaSupplementi.Text = String.Format("{0:MM/yyyy}", ((DatiSupplementi)e.Row.DataItem).DecorrenzaSupplemento);
            txtSettimaneSupplementi.Text = ((DatiSupplementi)e.Row.DataItem).NSettimaneSupplemento.HasValue ? ((DatiSupplementi)e.Row.DataItem).NSettimaneSupplemento.ToString() : string.Empty;
            txtMontanteIVS.Text = ((DatiSupplementi)e.Row.DataItem).MontanteSupplemento.HasValue ? ((DatiSupplementi)e.Row.DataItem).MontanteSupplemento.ToString() : string.Empty;
            txtAmmontareContributivo.Text = ((DatiSupplementi)e.Row.DataItem).AmmontareContributivo.HasValue ? ((DatiSupplementi)e.Row.DataItem).AmmontareContributivo.ToString() : string.Empty;
            ddlCodiceLiquidazione.SelectedValue = ((DatiSupplementi)e.Row.DataItem).CodiceLiquidazione.HasValue ? ((DatiSupplementi)e.Row.DataItem).CodiceLiquidazione.ToString() : string.Empty;
            txtRMSSupplementi.Text = ((DatiSupplementi)e.Row.DataItem).RMSSupplemento.HasValue ? ((DatiSupplementi)e.Row.DataItem).RMSSupplemento.Value.ToString("0.0000") : string.Empty;
        }

        private void saveValueRowAnte96(List<DatiSupplementi> elencoSupplementi, GridViewCommandEventArgs e, GridViewRow r)
        {
            if (!String.IsNullOrEmpty((((DropDownList)(r.Cells[2].Controls[1])).SelectedValue)))
                elencoSupplementi[r.DataItemIndex].CodGestioneSupplemento = ((DropDownList)(r.Cells[2].Controls[1])).SelectedValue;
            if (!String.IsNullOrEmpty((((DropDownList)(r.Cells[3].Controls[1])).SelectedValue))) //ENG - Modifica Supplementi CI Memo 177/2012
                elencoSupplementi[r.DataItemIndex].QuotaSupplemento = (((DropDownList)(r.Cells[3].Controls[1])).SelectedValue)[0];
            else
                elencoSupplementi[r.DataItemIndex].QuotaSupplemento = null; // Campo non obbligatorio
            if (!String.IsNullOrEmpty((((TextBox)(r.Cells[6].Controls[1])).Text)))
            {
                elencoSupplementi[r.DataItemIndex].RMSSupplemento = Decimal.Parse(((TextBox)(r.Cells[6].Controls[1])).Text);
                elencoSupplementi[r.DataItemIndex].TipoSupplemento = 'R';
            }
            else
            {
                elencoSupplementi[r.DataItemIndex].RMSSupplemento = null; // Campo non obbligatorio
                elencoSupplementi[r.DataItemIndex].TipoSupplemento = 'C';
            }

            if (!String.IsNullOrEmpty((((TextBox)(r.Cells[1].Controls[1])).Text)))
                elencoSupplementi[r.DataItemIndex].DecorrenzaSupplemento = Convert.ToDateTime(((TextBox)(r.Cells[1].Controls[1])).Text);
            if (!String.IsNullOrEmpty((((TextBox)(r.Cells[5].Controls[1])).Text)))
                elencoSupplementi[r.DataItemIndex].NSettimaneSupplemento = Convert.ToInt32(((TextBox)(r.Cells[5].Controls[1])).Text);
            if (!String.IsNullOrEmpty((((TextBox)(r.Cells[4].Controls[1])).Text)))
                elencoSupplementi[r.DataItemIndex].MontanteSupplemento = Decimal.Parse(((TextBox)(r.Cells[4].Controls[1])).Text);
            else
                elencoSupplementi[r.DataItemIndex].MontanteSupplemento = null; // Campo non obbligatorio
            if (!String.IsNullOrEmpty((((TextBox)(r.Cells[7].Controls[1])).Text)))
                elencoSupplementi[r.DataItemIndex].AmmontareContributivo = Decimal.Parse(((TextBox)(r.Cells[7].Controls[1])).Text);
            else
                elencoSupplementi[r.DataItemIndex].AmmontareContributivo = null; // Campo non obbligatorio
            if (!String.IsNullOrEmpty((((DropDownList)(r.Cells[8].Controls[1])).SelectedValue)))
                elencoSupplementi[r.DataItemIndex].CodiceLiquidazione = byte.Parse(((DropDownList)(r.Cells[8].Controls[1])).SelectedValue);
            else
                elencoSupplementi[r.DataItemIndex].CodiceLiquidazione = null; // Campo non obbligatorio
        }
        private bool IsListaEmptyAnte96()
        {
            List<DatiSupplementi> listaDatiSuppl = ViewState["elencoSupplementiAnte96"] as List<DatiSupplementi>;
            //TODO combinazioni contr/retr
            if (listaDatiSuppl.Count == 1 && string.IsNullOrEmpty(listaDatiSuppl[0].CodGestioneSupplemento) &&
                string.IsNullOrEmpty(listaDatiSuppl[0].DecorrenzaSupplemento.ToString()) && string.IsNullOrEmpty(listaDatiSuppl[0].MontanteSupplemento.ToString())
                && string.IsNullOrEmpty(listaDatiSuppl[0].RMSSupplemento.ToString()) &&
                string.IsNullOrEmpty(listaDatiSuppl[0].TipoSupplemento.ToString()) && string.IsNullOrEmpty(listaDatiSuppl[0].NSettimaneSupplemento.ToString()))
                return true;
            else
                return false;
        }

        private void inserisciSupplementiAnte96()
        {
            try
            {
                List<DatiSupplementi> elencoSupplementi = ViewState["elencoSupplementiAnte96"] as List<DatiSupplementi>;
                DatiSupplementi supp = new DatiSupplementi();
                elencoSupplementi.Add(supp);
                ViewState["elencoSupplementiAnte96"] = elencoSupplementi;
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCSupplementiAgoCI, Errore nel metodo InserisciSupplementiAnte96 " + ex);
            }
        }

        #endregion
    }
}

