using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using INPS.DNA;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter;


namespace INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.AltreFunzioni.GestioneAziendeESPA
{
    public partial class UCGestioneAziendeESPA : CustomBaseUserControl, IBancheFideiussioneESPA
    {
        /// <summary>
        /// proprietà ereditate da IViewUI
        /// </summary>
        #region IViewUI

        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }

        #endregion IViewUI

        /// <summary>
        /// proprietà ereditate da IBancheFideiussioneESPA
        /// </summary>
        #region IBancheFideiussioneESPA

        public AreaBancaFideiussioneESPA AziendeESPA { get; set; }

        #endregion IBancheFideiussioneESPA

        #region protected methods

        /// <summary>
        /// page load, richiama metodi ValorizzaGriglia, Abilita filtro
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (ViewState[EnumViewState.Filtro.ToString()] == null)
                ViewState[EnumViewState.Filtro.ToString()] = false;

            if (!IsPostBack)
            {
                CaricaDati();
                ValorizzaGrigliaGvBancheFideiussione();
                ValorizzaGrigliaGvAziende();
                ValorizzaGrigliaGvAziendeGGmmAAAA();
                AbilitaFiltro();
            }

            RaiseHideInfo(this, null);
        }

        #region metodi protected filtro di ricerca

        protected void btnApplicaFiltro_Click(object sender, EventArgs e)
        {
            ViewState[EnumViewState.Filtro.ToString()] = true;
            FiltraGvBancheFideiussione();
            FiltraGvAziende();
            FiltraGvAziendeGGmmAAAA();
            gvBancheFideiussione_Load();
            gvAziende_Load();
            gvAziendeGGmmAAAA_Load();
            DisabilitaFiltro();
            RaiseHideInfo(this, null);
        }

        protected void btnAnnullaFiltro_Click(object sender, EventArgs e)
        {
            CaricaDati();
            gvBancheFideiussione.EditIndex = -1;
            gvAziende.EditIndex = -1;
            gvAziendeGGmmAAAA.EditIndex = -1;
            ViewState[EnumViewState.Filtro.ToString()] = false;
            ValorizzaGrigliaGvBancheFideiussione();
            ValorizzaGrigliaGvAziende();
            ValorizzaGrigliaGvAziendeGGmmAAAA();

            AbilitaFiltro();
            RaiseHideInfo(this, null);
        }

        #endregion metodi protected filtro di ricerca



        #region metodi protected GridView BancheFideiussione

        protected void gvBancheFideiussione_onPageIndexChanging(Object sender, GridViewPageEventArgs e)
        {
            try
            {
                gvBancheFideiussione.EditIndex = -1;
                gvBancheFideiussione.PageIndex = e.NewPageIndex;
                gvBancheFideiussione_Load();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCGestioneBancheFideiussioneESPA, Errore nel metodo gvBancheFideiussione_onPageIndexChanging" + ex);
            }
        }

        protected void gvBancheFideiussione_onRowDeleting(object sender, GridViewDeleteEventArgs e)
        {

        }

        protected void gvBancheFideiussione_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            try
            {
                //Reset the edit index.
                if (((List<GestioneBancheFideiussioneESPADecBancaFideiussione>)ViewState[EnumViewState.BancheFideiussione.ToString()]).Count() < 2)
                    gvBancheFideiussione.EditIndex = 0;
                else
                    gvBancheFideiussione.EditIndex = -1;
                //Bind data to the GridView control.
                gvBancheFideiussione_Load();

            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCGestioneAziendeESPA, Errore nel metodo gvBancheFideiussione_RowCancelingEdit " + ex);
            }

        }

        protected void gvBancheFideiussione_RowEditing(object sender, GridViewEditEventArgs e)
        {
            try
            {
                gvBancheFideiussione.EditIndex = e.NewEditIndex;
                gvBancheFideiussione_Load();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCGestioneAziendeESPA, Errore nel metodo gvBancheFideiussione_RowEditing " + ex);
            }
        }

        protected void gvBancheFideiussione_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Delete")
            {
                #region Delete
                Control c = (Control)e.CommandSource;
                GridViewRow r = (GridViewRow)c.NamingContainer;

                PresenterAziendeESPA presenterAziendaESPA = new PresenterAziendeESPA();
                ValorizzaBancheFideiussionePerDelete(r.DataItemIndex);
                presenterAziendaESPA.EliminaBancheFideiussione(this);

                if (this.HasError)
                {
                    RaiseShowAvviso(this, null);
                    return;
                }
                else
                {
                    this.ErrorMessage = "Banca Fideiussione ESPA eliminata correttamente";
                    RaiseShowAvviso(this, null);
                }

                AbilitaFiltro();
                ValorizzaGrigliaGvBancheFideiussione();
                ValorizzaGrigliaGvAziende();
                ViewState[EnumViewState.Filtro.ToString()] = false;
                #endregion Delete
            }
            else if (e.CommandName == "Edit")
            {

            }
            else if (e.CommandName == "Salva")
            {
                #region Salva
                Control c = (Control)e.CommandSource;
                GridViewRow r = (GridViewRow)c.NamingContainer;

                PresenterAziendeESPA presenterAziendeESPA = new PresenterAziendeESPA();
                ValorizzaBancheFideiussionePerSave(r);
                if (this.HasError)
                    return;

                presenterAziendeESPA.InserisciBancheFideiussione(this);

                if (this.HasError)
                {
                    RaiseShowAvviso(this, null);
                    return;
                }
                else
                {
                    this.ErrorMessage = "Banca Fideiussione ESPA inserita correttamente";
                    RaiseShowAvviso(this, null);
                    gvBancheFideiussione.EditIndex = -1;
                }

                AbilitaFiltro();
                ValorizzaGrigliaGvBancheFideiussione();
                ValorizzaGrigliaGvAziende();
                ViewState[EnumViewState.Filtro.ToString()] = false;
                #endregion Salva
            }
            else if (e.CommandName == "Annulla")
            {
                gvBancheFideiussione.EditIndex = -1;
                gvBancheFideiussione_Load();
            }
        }

        protected void gvBancheFideiussione_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    List<GestioneBancheFideiussioneESPADecBancaFideiussione> elencoBancheFideiussione = (List<GestioneBancheFideiussioneESPADecBancaFideiussione>)ViewState[EnumViewState.BancheFideiussione.ToString()];
                    if (e.Row.Cells[0].Controls.Count == 3)
                    {
                        CodeUtility.EnableEditableMode(e.Row.Cells[0], Keys.ValidationGroup_GrigliaBanche, Page.Theme);
                    }
                    else
                    {
                        if (e.Row.DataItemIndex == elencoBancheFideiussione.Count - 1)
                        {
                            LinkButton add = ((LinkButton)(e.Row.Cells[0].Controls[0]));
                            add.Text = (string)"<img width=16 height=16 border=0 src=../App_themes/" + Page.Theme + "/Images/add24.png />";
                            add.ToolTip = "Aggiungi";
                            add.OnClientClick = "BlockUI();";
                        }
                        else
                        {
                            CodeUtility.EnableReadableMode(e.Row.Cells[0], e.Row.Cells[4], Page.Theme, Keys.BtnDelete_GrigliaBanche);
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
                throw new INPS.DNA.DnaApplicationException("UCGestioneBancheFideiussione, Errore nel metodo gvBancheFideiussione_RowDataBound " + ex);
            }
        }

        #endregion metodi protected GridView BancheFideiussione



        #region metodi protected GridView Aziende

        protected void gvAziende_onPageIndexChanging(Object sender, GridViewPageEventArgs e)
        {
            try
            {
                gvAziende.EditIndex = -1;
                gvAziende.PageIndex = e.NewPageIndex;
                gvAziende_Load();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCGestioneBancheFideiussione, Errore nel metodo gvAziende_onPageIndexChanging" + ex);
            }
        }

        protected void gvAziende_RowEditing(object sender, GridViewEditEventArgs e)
        {
            try
            {
                gvAziende.EditIndex = e.NewEditIndex;
                gvAziende_Load();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCGestioneBancheFideiussione, Errore nel metodo gvAziende_RowEditing " + ex);
            }
        }

        protected void gvAziende_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Elimina" || e.CommandName == "Edit")
            {

            }
            else if (e.CommandName == "Salva")
            {
                Control c = (Control)e.CommandSource;
                GridViewRow r = (GridViewRow)c.NamingContainer;

                PresenterAziendeESPA presenterAziendeESPA = new PresenterAziendeESPA();
                ValorizzaAziendePerSave(r);
                if (this.HasError)
                    return;

                presenterAziendeESPA.InserisciAziende(this);

                if (this.HasError)
                {
                    RaiseShowAvviso(this, null);
                    return;
                }
                else
                {
                    this.ErrorMessage = "Azienda ESPA inserita correttamente";
                    RaiseShowAvviso(this, null);
                    gvAziende.EditIndex = -1;
                }

                AbilitaFiltro();
                ValorizzaGrigliaGvAziende();
                ValorizzaGrigliaGvBancheFideiussione();
                ViewState[EnumViewState.Filtro.ToString()] = false;
            }
            else if (e.CommandName == "Annulla")
            {
                gvAziende.EditIndex = -1;
                gvAziende_Load();
            }
        }

        protected void gvAziende_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    List<GestioneDecodificaAziendaDecAzienda> elencoAziende = (List<GestioneDecodificaAziendaDecAzienda>)ViewState[EnumViewState.Aziende.ToString()];

                    if (e.Row.Cells[0].Controls.Count == 3)
                    {
                        CodeUtility.EnableEditableMode(e.Row.Cells[0], Keys.ValidationGroup_GrigliaAziende, Page.Theme);
                    }

                    else
                    {
                        if (e.Row.DataItemIndex == elencoAziende.Count - 1)
                        {
                            LinkButton add = ((LinkButton)(e.Row.Cells[0].Controls[0]));
                            add.Text = (string)"<img width=16 height=16 border=0 src=../App_themes/" + Page.Theme + "/Images/add24.png />";
                            add.ToolTip = "Aggiungi";
                            add.OnClientClick = "BlockUI();";
                        }
                        else
                        {
                            LinkButton edit = ((LinkButton)(e.Row.Cells[0].Controls[0]));
                            edit.Text = string.Empty;
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
                throw new INPS.DNA.DnaApplicationException("UCGestioneBancheFideiussione, Errore nel metodo gvAziende_RowDataBound " + ex);
            }
        }

        #endregion metodi protected dellla GridView Aziende



        #region metodi protected GridView Aziende GGmmAAA

        protected void gvAziendeGGmmAAAA_onPageIndexChanging(Object sender, GridViewPageEventArgs e)
        {
            try
            {
                gvAziendeGGmmAAAA.EditIndex = -1;
                gvAziendeGGmmAAAA.PageIndex = e.NewPageIndex;
                gvAziendeGGmmAAAA_Load();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCGestioneBancheFideiussione, Errore nel metodo gvAziende_onPageIndexChanging" + ex);
            }
        }

        protected void gvAziendeGGmmAAAA_RowEditing(object sender, GridViewEditEventArgs e)
        {
            try
            {
                gvAziendeGGmmAAAA.EditIndex = e.NewEditIndex;
                gvAziendeGGmmAAAA_Load();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCGestioneBancheFideiussione, Errore nel metodo gvAziendeGGmmAAAA_RowEditing " + ex);
            }
        }

        protected void gvAziendeGGmmAAAA_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Elimina")
            {
                #region Elimina
                Control c = (Control)e.CommandSource;
                GridViewRow r = (GridViewRow)c.NamingContainer;

                PresenterAziendeESPA presenterAziendeESPA = new PresenterAziendeESPA();

                ValorizzaAziendeGGmmAAAAPerDelete(r.DataItemIndex);
                presenterAziendeESPA.EliminaAziendeGGmmAAAA(this);

                if (this.HasError)
                {
                    RaiseShowAvviso(this, null);
                    return;
                }
                else
                {
                    this.ErrorMessage = "AziendaGGmmAAAA eliminata correttamente";
                    RaiseShowAvviso(this, null);
                }

                AbilitaFiltro();
                ValorizzaGrigliaGvBancheFideiussione();
                ValorizzaGrigliaGvAziende();
                ValorizzaGrigliaGvAziendeGGmmAAAA();
                ViewState[EnumViewState.Filtro.ToString()] = false;
                #endregion Elimina
            }
            else if (e.CommandName == "Edit")
            {

            }
            else if (e.CommandName == "Salva")
            {
                #region salva
                Control c = (Control)e.CommandSource;
                GridViewRow r = (GridViewRow)c.NamingContainer;

                PresenterAziendeESPA presenterAziendeESPA = new PresenterAziendeESPA();
                ValorizzaAziendeGGmmAAAAPerSave(r);
                if (this.HasError)
                    return;

                presenterAziendeESPA.InserisciAziendeGGmmAAAA(this);

                if (this.HasError)
                {
                    RaiseShowAvviso(this, null);
                    return;
                }
                else
                {
                    this.ErrorMessage = "Azienda con assegno in formato GGmmAAAA inserita correttamente";
                    RaiseShowAvviso(this, null);
                    gvAziendeGGmmAAAA.EditIndex = -1;
                }

                AbilitaFiltro();
                ValorizzaGrigliaGvAziende();
                ValorizzaGrigliaGvBancheFideiussione();
                ValorizzaGrigliaGvAziendeGGmmAAAA();
                ViewState[EnumViewState.Filtro.ToString()] = false;
                #endregion salva
            }
            else if (e.CommandName == "Annulla")
            {
                gvAziendeGGmmAAAA.EditIndex = -1;
                gvAziendeGGmmAAAA_Load();
            }
        }

        protected void gvAziendeGGmmAAAA_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    List<GestioneAziendeScadenzaAssegnoGGmmAAAADecAziendeScadenzaAssegnoGGmmAAAA> elencoAziendeGGmmAAAA = (List<GestioneAziendeScadenzaAssegnoGGmmAAAADecAziendeScadenzaAssegnoGGmmAAAA>)ViewState[EnumViewState.AziendeGGmmAAAA.ToString()];

                    if (e.Row.Cells[0].Controls.Count == 3)
                    {
                        CodeUtility.EnableEditableMode(e.Row.Cells[0], Keys.ValidationGroup_GrigliaAziendeGGmmAAAA, Page.Theme);
                    }

                    else
                    {
                        if (e.Row.DataItemIndex == elencoAziendeGGmmAAAA.Count - 1)
                        {
                            LinkButton add = ((LinkButton)(e.Row.Cells[0].Controls[0]));
                            add.Text = (string)"<img width=16 height=16 border=0 src=../App_themes/" + Page.Theme + "/Images/add24.png />";
                            add.ToolTip = "Aggiungi";
                            add.OnClientClick = "BlockUI();";
                        }
                        else
                        {
                            CodeUtility.EnableReadableMode(e.Row.Cells[0], e.Row.Cells[3], Page.Theme, Keys.BtnElimina_GrigliaAziendeGGmmAAAA);

                            LinkButton edit = ((LinkButton)(e.Row.Cells[0].Controls[0]));
                            edit.Text = string.Empty;

                            Label lblDescrizione = (Label)e.Row.FindControl(Keys.lblDescrizione);

                            lblDescrizione.Text = PrelevaDescrizioneAziendeViewStateAziende(((GestioneAziendeScadenzaAssegnoGGmmAAAADecAziendeScadenzaAssegnoGGmmAAAA)e.Row.DataItem));
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
                throw new INPS.DNA.DnaApplicationException("UCGestioneBancheFideiussione, Errore nel metodo gvAziendeGGmmAAAA_RowDataBound " + ex);
            }
        }

        #endregion metodi protected GridView Aziende GGmmAAA

        #endregion protected methods

        #region private methods

        private void CaricaDati()
        {
            PresenterAziendeESPA presenterAziendaESPA = new PresenterAziendeESPA();
            presenterAziendaESPA.CaricaAreaBancheAziende(this);

            if (this.HasError)
            {
                RaiseShowAvviso(this, null);
                return;
            }
        }

        /// <summary>
        /// metodi privati del Filtro di ricerca
        /// </summary>
        #region metodi privati filtro di ricerca

        private void FiltraGvBancheFideiussione()
        {
            int count = 0;
            List<GestioneBancheFideiussioneESPADecBancaFideiussione> elencoBancheFideiussioneWS = (List<GestioneBancheFideiussioneESPADecBancaFideiussione>)ViewState[EnumViewState.BancheFideiussione.ToString()];

            if (!string.IsNullOrEmpty(txtFiltroCodiceAzienda.Text.Trim()))
            {
                count++;
                elencoBancheFideiussioneWS = elencoBancheFideiussioneWS.FindAll(x => x.CodiceAzienda == txtFiltroCodiceAzienda.Text.ToUpperInvariant());
            }

            if (!string.IsNullOrEmpty(txtFiltroMatricola.Text.Trim()))
            {
                count++;
                elencoBancheFideiussioneWS = elencoBancheFideiussioneWS.FindAll(x => x.Matricola == txtFiltroMatricola.Text.ToUpperInvariant());
            }

            if (!string.IsNullOrEmpty(txtFiltroProgressivo.Text.Trim()))
            {
                count++;
                elencoBancheFideiussioneWS = elencoBancheFideiussioneWS.FindAll(x => x.Progressivo.GetValueOrDefault().ToString() == txtFiltroProgressivo.Text);
            }

            if (!string.IsNullOrEmpty(txtFiltroAnno.Text))
            {
                count++;
                elencoBancheFideiussioneWS = elencoBancheFideiussioneWS.FindAll(x => x.Anno.GetValueOrDefault().ToString() == txtFiltroAnno.Text);
            }
            if (count > 0)
            {
                ViewState[EnumViewState.BancheFideiussione.ToString()] = elencoBancheFideiussioneWS;
                elencoBancheFideiussioneWS.Add(new GestioneBancheFideiussioneESPADecBancaFideiussione());
                DisabilitaFiltro();
            }
        }

        private void FiltraGvAziende()
        {
            int count = 0;
            List<GestioneDecodificaAziendaDecAzienda> elencoAziendeWS = (List<GestioneDecodificaAziendaDecAzienda>)ViewState[EnumViewState.Aziende.ToString()];


            if (!string.IsNullOrEmpty(txtFiltroCodiceAzienda.Text.Trim()))
            {
                count++;
                elencoAziendeWS = elencoAziendeWS.FindAll(x => x.TraduzioneSuGP == txtFiltroCodiceAzienda.Text.ToUpperInvariant());
            }
            if (count > 0)
            {
                ViewState[EnumViewState.Aziende.ToString()] = elencoAziendeWS;
                elencoAziendeWS.Add(new GestioneDecodificaAziendaDecAzienda());
                DisabilitaFiltro();
            }
        }

        private void FiltraGvAziendeGGmmAAAA()
        {
            int count = 0;
            List<GestioneAziendeScadenzaAssegnoGGmmAAAADecAziendeScadenzaAssegnoGGmmAAAA> elencoAziendeGGmmAAAAWS = (List<GestioneAziendeScadenzaAssegnoGGmmAAAADecAziendeScadenzaAssegnoGGmmAAAA>)ViewState[EnumViewState.AziendeGGmmAAAA.ToString()];
            if (!string.IsNullOrEmpty(txtFiltroCodiceAzienda.Text.Trim()))
            {
                count++;
                elencoAziendeGGmmAAAAWS = elencoAziendeGGmmAAAAWS.FindAll(x => x.TraduzioneSuGP == txtFiltroCodiceAzienda.Text.ToUpperInvariant());
            }
            if (count > 0)
            {
                ViewState[EnumViewState.AziendeGGmmAAAA.ToString()] = elencoAziendeGGmmAAAAWS;
                elencoAziendeGGmmAAAAWS.Add(new GestioneAziendeScadenzaAssegnoGGmmAAAADecAziendeScadenzaAssegnoGGmmAAAA());
                DisabilitaFiltro();
            }
        }

        private void AbilitaFiltro()
        {
            PulisciFiltro();
            btnApplicaFiltro.Enabled = true;
            btnAnnullaFiltro.Enabled = false;
            txtFiltroCodiceAzienda.Enabled = true;
            txtFiltroMatricola.Enabled = true;
            txtFiltroProgressivo.Enabled = true;
            txtFiltroAnno.Enabled = true;
            gvBancheFideiussione.EditIndex = -1;
            gvAziende.EditIndex = -1;
            //gvAziendeGGmmAAAA.EditIndex = -1;
        }

        private void DisabilitaFiltro()
        {
            btnApplicaFiltro.Enabled = false;
            btnAnnullaFiltro.Enabled = true;
            txtFiltroCodiceAzienda.Enabled = false;
            txtFiltroMatricola.Enabled = false;
            txtFiltroProgressivo.Enabled = false;
            txtFiltroAnno.Enabled = false;
        }

        private void PulisciFiltro()
        {
            txtFiltroCodiceAzienda.Text = string.Empty;
            txtFiltroMatricola.Text = string.Empty;
            txtFiltroProgressivo.Text = string.Empty;
            txtFiltroAnno.Text = string.Empty;
        }

        #endregion metodi privati del filtro di ricerca

        #region metodi privati GvBancheFideiussione

        /// <summary>
        /// metodo valorizza griglia 
        /// richiama metodo presenter carica banche fideiussione
        /// richiama metodo private formatta
        /// richiama metodo load
        /// </summary>
        private void ValorizzaGrigliaGvBancheFideiussione()
        {
            FormattaElencoBancheFideiussione();

            //// Va all'ultima pagina
            if ((List<GestioneBancheFideiussioneESPADecBancaFideiussione>)ViewState[EnumViewState.BancheFideiussione.ToString()] != null)
                gvBancheFideiussione.PageIndex = ((List<GestioneBancheFideiussioneESPADecBancaFideiussione>)ViewState[EnumViewState.BancheFideiussione.ToString()]).Count / gvBancheFideiussione.PageSize;

            gvBancheFideiussione_Load();
        }

        private void FormattaElencoBancheFideiussione()
        {
            List<GestioneBancheFideiussioneESPADecBancaFideiussione> elencoBancheFideiussioneWS = new List<GestioneBancheFideiussioneESPADecBancaFideiussione>();
            if (this.AziendeESPA.ElencoBancheFideiussione == null || this.AziendeESPA.ElencoBancheFideiussione.Count() == 0)
            {
                elencoBancheFideiussioneWS.Add(new GestioneBancheFideiussioneESPADecBancaFideiussione());
            }
            else
            {
                elencoBancheFideiussioneWS = this.AziendeESPA.ElencoBancheFideiussione.ToList();
                elencoBancheFideiussioneWS.Add(new GestioneBancheFideiussioneESPADecBancaFideiussione());
            }

            if (elencoBancheFideiussioneWS.Count() < 2)

                gvBancheFideiussione.EditIndex = 0;

            ViewState[EnumViewState.BancheFideiussione.ToString()] = elencoBancheFideiussioneWS;
        }

        private void gvBancheFideiussione_Load()
        {
            try
            {
                gvBancheFideiussione.DataSource = (List<GestioneBancheFideiussioneESPADecBancaFideiussione>)ViewState[EnumViewState.BancheFideiussione.ToString()];
                gvBancheFideiussione.DataBind();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCGestioneBancheFideiussione, Errore nel metodo gvBancheFideiussione_Load " + ex);
            }
        }

        private void ValorizzaBancheFideiussionePerSave(GridViewRow row)
        {
            List<GestioneBancheFideiussioneESPADecBancaFideiussione> elencoBancheFideiussione = (List<GestioneBancheFideiussioneESPADecBancaFideiussione>)ViewState[EnumViewState.BancheFideiussione.ToString()];

            GestioneBancheFideiussioneESPADecBancaFideiussione bncApp = new GestioneBancheFideiussioneESPADecBancaFideiussione();

            bncApp.Id = elencoBancheFideiussione[row.DataItemIndex].Id;

            bncApp.CodiceAzienda = (((TextBox)row.FindControl(Keys.txtCodiceAzienda)).Text);
            bncApp.Matricola = (((TextBox)row.FindControl(Keys.txtMatricola)).Text);
            bncApp.BancaFideiussione = (((TextBox)row.FindControl(Keys.txtBancaFideiussione)).Text.ToUpperInvariant());
            bncApp.Progressivo = CodeUtility.StringToNullableByte(((TextBox)row.FindControl(Keys.txtProgressivo)).Text);
            bncApp.Anno = CodeUtility.StringToNullableShort(((TextBox)row.FindControl(Keys.txtAnno)).Text);
            bncApp.InizioEsodo = Utility.GetDateFromString(((TextBox)row.FindControl(Keys.txtInizioEsodo)).Text);
            bncApp.FineEsodo = Utility.GetDateFromString(((TextBox)row.FindControl(Keys.txtFineEsodo)).Text);
            bncApp.ABI = CodeUtility.StringToNullableInt(((TextBox)row.FindControl(Keys.txtABI)).Text);
            bncApp.CAB = CodeUtility.StringToNullableInt(((TextBox)row.FindControl(Keys.txtCAB)).Text);

            ValorizzaBancheFideiussione(bncApp);
        }

        private void ValorizzaBancheFideiussionePerDelete(int index)
        {
            List<GestioneBancheFideiussioneESPADecBancaFideiussione> elencoBancheFideiussione = (List<GestioneBancheFideiussioneESPADecBancaFideiussione>)ViewState[EnumViewState.BancheFideiussione.ToString()];

            GestioneBancheFideiussioneESPADecBancaFideiussione bncApp = elencoBancheFideiussione[index];
            ValorizzaBancheFideiussione(bncApp);
        }

        private void ValorizzaBancheFideiussione(GestioneBancheFideiussioneESPADecBancaFideiussione bancheFideiussione)
        {
            if (this.AziendeESPA == null)

                this.AziendeESPA = new AreaBancaFideiussioneESPA();
            this.AziendeESPA.BancaFideiussione = new GestioneBancheFideiussioneESPADecBancaFideiussione();

            AziendeESPA.BancaFideiussione.Id = bancheFideiussione.Id;
            AziendeESPA.BancaFideiussione.CodiceAzienda = bancheFideiussione.CodiceAzienda;
            AziendeESPA.BancaFideiussione.Matricola = bancheFideiussione.Matricola;
            AziendeESPA.BancaFideiussione.BancaFideiussione = bancheFideiussione.BancaFideiussione;
            AziendeESPA.BancaFideiussione.Progressivo = bancheFideiussione.Progressivo;
            AziendeESPA.BancaFideiussione.Anno = bancheFideiussione.Anno;
            AziendeESPA.BancaFideiussione.InizioEsodo = bancheFideiussione.InizioEsodo;
            AziendeESPA.BancaFideiussione.FineEsodo = bancheFideiussione.FineEsodo;
            AziendeESPA.BancaFideiussione.ABI = bancheFideiussione.ABI;
            AziendeESPA.BancaFideiussione.CAB = bancheFideiussione.CAB;
        }

        #endregion metodi privati gvBancheFideiussione



        #region metodi privati gvAziende

        private void ValorizzaGrigliaGvAziende()
        {
            FormattaElencoAziende();

            //// Va all'ultima pagina
            if ((List<GestioneDecodificaAziendaDecAzienda>)ViewState[EnumViewState.Aziende.ToString()] != null)
                gvAziende.PageIndex = ((List<GestioneDecodificaAziendaDecAzienda>)ViewState[EnumViewState.Aziende.ToString()]).Count / gvAziende.PageSize;

            gvAziende_Load();
        }

        private void FormattaElencoAziende()
        {
            List<GestioneDecodificaAziendaDecAzienda> elencoAziende = new List<GestioneDecodificaAziendaDecAzienda>();
            if (this.AziendeESPA.ElencoAziende == null || this.AziendeESPA.ElencoAziende.Count() == 0)
            {
                elencoAziende.Add(new GestioneDecodificaAziendaDecAzienda());
            }
            else
            {
                elencoAziende = this.AziendeESPA.ElencoAziende.ToList();
                elencoAziende.Add(new GestioneDecodificaAziendaDecAzienda());
            }

            if (elencoAziende.Count() < 2)

                gvAziende.EditIndex = 0;

            ViewState[EnumViewState.Aziende.ToString()] = elencoAziende;
        }

        private void gvAziende_Load()
        {
            try
            {
                gvAziende.DataSource = (List<GestioneDecodificaAziendaDecAzienda>)ViewState[EnumViewState.Aziende.ToString()];
                gvAziende.DataBind();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCGestioneBancheFideiussione, Errore nel metodo gvAziende_Load " + ex);
            }
        }

        private void ValorizzaAziendePerSave(GridViewRow r)
        {
            List<GestioneDecodificaAziendaDecAzienda> elencoAziende = (List<GestioneDecodificaAziendaDecAzienda>)ViewState[EnumViewState.Aziende.ToString()];

            GestioneDecodificaAziendaDecAzienda azApp = new GestioneDecodificaAziendaDecAzienda();

            azApp.Id = elencoAziende[r.DataItemIndex].Id;

            azApp.TraduzioneSuGP = (((TextBox)r.FindControl(Keys.txtCodiceAzienda)).Text);
            azApp.Descrizione = (((TextBox)r.FindControl(Keys.txtDescrizione)).Text);

            ValorizzaAziende(azApp);
        }

        private void ValorizzaAziende(GestioneDecodificaAziendaDecAzienda aziende)
        {
            if (this.AziendeESPA == null)

                this.AziendeESPA = new AreaBancaFideiussioneESPA();
            this.AziendeESPA.Azienda = new GestioneDecodificaAziendaDecAzienda();

            AziendeESPA.Azienda.Id = aziende.Id;
            AziendeESPA.Azienda.TraduzioneSuGP = aziende.TraduzioneSuGP;
            AziendeESPA.Azienda.Descrizione = aziende.Descrizione;
            AziendeESPA.Azienda.SiglaCategoria = Keys.ESPA;
        }

        #endregion metodi privati gvAziende



        #region metodi privati gvAziendeGGmmAAAA

        private void gvAziendeGGmmAAAA_Load()
        {
            try
            {
                gvAziendeGGmmAAAA.DataSource = (List<GestioneAziendeScadenzaAssegnoGGmmAAAADecAziendeScadenzaAssegnoGGmmAAAA>)ViewState[EnumViewState.AziendeGGmmAAAA.ToString()];
                gvAziendeGGmmAAAA.DataBind();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCGestioneBancheFideiussione, Errore nel metodo gvAziendeGGmmAAAA_Load " + ex);
            }
        }

        private void ValorizzaGrigliaGvAziendeGGmmAAAA()
        {
            FormattaElencoAziendeGGmmAAAA();

            //// Va all'ultima pagina
            if ((List<GestioneAziendeScadenzaAssegnoGGmmAAAADecAziendeScadenzaAssegnoGGmmAAAA>)ViewState[EnumViewState.AziendeGGmmAAAA.ToString()] != null)
                gvAziendeGGmmAAAA.PageIndex = ((List<GestioneAziendeScadenzaAssegnoGGmmAAAADecAziendeScadenzaAssegnoGGmmAAAA>)ViewState[EnumViewState.AziendeGGmmAAAA.ToString()]).Count / gvAziendeGGmmAAAA.PageSize;

            gvAziendeGGmmAAAA_Load();
        }

        private void FormattaElencoAziendeGGmmAAAA()
        {
            List<GestioneAziendeScadenzaAssegnoGGmmAAAADecAziendeScadenzaAssegnoGGmmAAAA> elencoAziendeGGmmAAAA = new List<GestioneAziendeScadenzaAssegnoGGmmAAAADecAziendeScadenzaAssegnoGGmmAAAA>();
            if (this.AziendeESPA.ElencoAziendeAssegnoGGmmAAAA == null || this.AziendeESPA.ElencoAziendeAssegnoGGmmAAAA.Count() == 0)
            {
                elencoAziendeGGmmAAAA.Add(new GestioneAziendeScadenzaAssegnoGGmmAAAADecAziendeScadenzaAssegnoGGmmAAAA());
            }
            else
            {
                elencoAziendeGGmmAAAA = this.AziendeESPA.ElencoAziendeAssegnoGGmmAAAA.ToList();
                elencoAziendeGGmmAAAA.Add(new GestioneAziendeScadenzaAssegnoGGmmAAAADecAziendeScadenzaAssegnoGGmmAAAA());
            }

            if (elencoAziendeGGmmAAAA.Count() < 2)

                gvAziendeGGmmAAAA.EditIndex = 0;

            ViewState[EnumViewState.AziendeGGmmAAAA.ToString()] = elencoAziendeGGmmAAAA;
        }

        private void ValorizzaAziendeGGmmAAAAPerDelete(int index)
        {
            List<GestioneAziendeScadenzaAssegnoGGmmAAAADecAziendeScadenzaAssegnoGGmmAAAA> elencoAziendeGGmmAAAA = (List<GestioneAziendeScadenzaAssegnoGGmmAAAADecAziendeScadenzaAssegnoGGmmAAAA>)ViewState[EnumViewState.AziendeGGmmAAAA.ToString()];
            GestioneAziendeScadenzaAssegnoGGmmAAAADecAziendeScadenzaAssegnoGGmmAAAA azApp = elencoAziendeGGmmAAAA[index];
            ValorizzaAziendeGGmmAAAA(azApp);
        }

        private void ValorizzaAziendeGGmmAAAAPerSave(GridViewRow r)
        {
            List<GestioneAziendeScadenzaAssegnoGGmmAAAADecAziendeScadenzaAssegnoGGmmAAAA> elencoAziendeGGmmAAAA = (List<GestioneAziendeScadenzaAssegnoGGmmAAAADecAziendeScadenzaAssegnoGGmmAAAA>)ViewState[EnumViewState.AziendeGGmmAAAA.ToString()];

            GestioneAziendeScadenzaAssegnoGGmmAAAADecAziendeScadenzaAssegnoGGmmAAAA azApp = new GestioneAziendeScadenzaAssegnoGGmmAAAADecAziendeScadenzaAssegnoGGmmAAAA();
            azApp.Id = elencoAziendeGGmmAAAA[r.DataItemIndex].Id;

            azApp.TraduzioneSuGP = (((TextBox)r.FindControl(Keys.txtCodiceAziendaGGmmAAAA)).Text);
            azApp.ProgressivoRichiesto = CodeUtility.StringToNullableByte((((TextBox)r.FindControl(Keys.txtCodiceProgressivoRichiesto)).Text));

            ValorizzaAziendeGGmmAAAA(azApp);
        }

        private void ValorizzaAziendeGGmmAAAA(GestioneAziendeScadenzaAssegnoGGmmAAAADecAziendeScadenzaAssegnoGGmmAAAA aziendeGGmmAAAA)
        {
            if (this.AziendeESPA == null)
                this.AziendeESPA = new AreaBancaFideiussioneESPA();
            this.AziendeESPA.AziendaGGmmAAAA = new GestioneAziendeScadenzaAssegnoGGmmAAAADecAziendeScadenzaAssegnoGGmmAAAA();

            AziendeESPA.AziendaGGmmAAAA.Id = aziendeGGmmAAAA.Id;
            AziendeESPA.AziendaGGmmAAAA.TraduzioneSuGP = aziendeGGmmAAAA.TraduzioneSuGP;
            AziendeESPA.AziendaGGmmAAAA.ProgressivoRichiesto = aziendeGGmmAAAA.ProgressivoRichiesto;
        }

        private string PrelevaDescrizioneAziendeViewStateAziende(GestioneAziendeScadenzaAssegnoGGmmAAAADecAziendeScadenzaAssegnoGGmmAAAA azGGmmAAAA)
        {
            GestioneDecodificaAziendaDecAzienda azApp = new GestioneDecodificaAziendaDecAzienda();
            List<GestioneDecodificaAziendaDecAzienda> listaAziendeWS = new List<GestioneDecodificaAziendaDecAzienda>();

            if (ViewState[EnumViewState.Aziende.ToString()] != null)
            {
                listaAziendeWS = (List<GestioneDecodificaAziendaDecAzienda>)ViewState[EnumViewState.Aziende.ToString()];

                azApp = listaAziendeWS.Find(x => x.TraduzioneSuGP != null && x.TraduzioneSuGP.PadLeft(4, '0') == azGGmmAAAA.TraduzioneSuGP.PadLeft(4, '0'));
            }

            if (azApp != null)
                return azApp.Descrizione;
            else
                return string.Empty;
        }

        #endregion metodi privati gvAziendeGGmmAAAA

        #endregion private methods

        #region Events
        public event EventHandler ShowAvviso;
        public event EventHandler HideInfo;

        protected void RaiseShowAvviso(object sender, EventArgs e)
        {
            if (ShowAvviso != null)
                ShowAvviso(sender, e);
        }

        protected void RaiseHideInfo(object sender, EventArgs e)
        {
            if (HideInfo != null)
                HideInfo(sender, e);
        }
        #endregion Events

        #region Enum
        public enum EnumViewState
        {
            BancheFideiussione,
            Filtro,
            Aziende,
            AziendeGGmmAAAA
        }

        #endregion Enum

        #region private classes
        private class Keys
        {
            public const string ValidationGroup_GrigliaBanche = "GrigliaBanche";
            public const string ValidationGroup_GrigliaAziende = "GrigliaAziende";
            public const string ValidationGroup_GrigliaAziendeGGmmAAAA = "GrigliaAziendeGGmmAAAA";
            public const string BtnDelete_GrigliaBanche = "btnDelete";
            public const string BtnElimina_GrigliaAziendeGGmmAAAA = "btnElimina";
            public const string txtCodiceAziendaGGmmAAAA = "txtCodiceAziendaGGmmAAAA";
            public const string txtCodiceProgressivoRichiesto = "txtCodiceProgressivoRichiesto";
            public const string txtCodiceAzienda = "txtCodiceAzienda";
            public const string txtMatricola = "txtMatricola";
            public const string txtBancaFideiussione = "txtBancaFideiussione";
            public const string txtProgressivo = "txtProgressivo";
            public const string txtAnno = "txtAnno";
            public const string txtInizioEsodo = "txtInizioEsodo";
            public const string txtFineEsodo = "txtFineEsodo";
            public const string txtABI = "txtABI";
            public const string txtCAB = "txtCAB";
            public const string txtDescrizione = "txtDescrizione";
            public const string lblDescrizione = "lblDescrizione";
            public const string ESPA = "ESPA";
        }
        #endregion private classes
    }
}
