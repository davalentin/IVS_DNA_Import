using INPS.DNA;
using INPS.Pensioni.LiquidazionePensione.Presenter;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.AltreFunzioni.GestioneAziendeESOAMB
{
    public partial class UCAziendeESOAMB : CustomBaseUserControl, IAziendeESOAMB
    {
        #region IViewUI

        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }

        #endregion IViewUI

        #region IAziendeESOAMB

        public AreaAziendeESOAMB AziendeESOAMB { get; set; }
        public string CommaSeparatedDescrizione { get; set; }

        #endregion IAziendeESOAMB

        /// <summary>
        /// page load, richiama il caricaDati e il ValorizzaGriglia
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
                AbilitaFiltro();
                ValorizzaGrigliaAziendeESOAMB(true);
                ValorizzaGrigliaGvAziendeGGmmAAAA();
                ValorizzaAutoComplete();
            }

            RaiseHideInfo(this, null);

        }

        #region metodi protected gridview aziende ESOAMB

        protected void gvAziendeESOAMB_onPageIndexChanging(Object sender, GridViewPageEventArgs e)
        {
            try
            {
                gvAziendeESOAMB.EditIndex = -1;
                gvAziendeESOAMB.PageIndex = e.NewPageIndex;
                gvAziendeESOAMB_Load();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCAziendeESOAMB, Errore nel metodo gvAziendeESOAMB_onPageIndexChanging" + ex);
            }
        }

        protected void gvAziendeESOAMB_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            try
            {
                //Reset the edit index.
                if (((List<AziendeESOAMB>)ViewState[EnumViewState.AziendeESOAMB.ToString()]).Count() < 2)
                    gvAziendeESOAMB.EditIndex = 0;
                else
                    gvAziendeESOAMB.EditIndex = -1;
                //Bind data to the GridView control.
                gvAziendeESOAMB_Load();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCAziendeESOAMB, Errore nel metodo gvAziendeESOAMB_RowCancelingEdit " + ex);
            }
        }

        protected void gvAziendeESOAMB_RowEditing(object sender, GridViewEditEventArgs e)
        {
            try
            {
                gvAziendeESOAMB.EditIndex = e.NewEditIndex;
                gvAziendeESOAMB_Load();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCAziendeESOAMB, Errore nel metodo gvAziendeESOAMB_RowEditing " + ex);
            }
        }

        protected void gvAziendeESOAMB_RowCommand(object sender, GridViewCommandEventArgs e)
        {

            if (e.CommandName == "Elimina")
            {
                #region delete

                Control c = (Control)e.CommandSource;
                GridViewRow r = (GridViewRow)c.NamingContainer;

                PresenterAziendeESOAMB presenterAziendeESOAMB = new PresenterAziendeESOAMB();
                ValorizzaAziendeESOAMBPerDelete(r.DataItemIndex);

                presenterAziendeESOAMB.EliminaAziendeESOAMB(this);

                if (this.HasError)
                    RaiseShowAvviso(this, null);
                else
                {
                    this.ErrorMessage = "AziendaESOAMB eliminata correttamente";
                    RaiseShowAvviso(this, null);
                }

                AbilitaFiltro();
                ValorizzaGrigliaAziendeESOAMB(false);
                ValorizzaGrigliaGvAziendeGGmmAAAA();
                ViewState[EnumViewState.Filtro.ToString()] = false;

                #endregion delete
            }

            else if (e.CommandName == "Edit")
            {

            }

            else if (e.CommandName == "Salva")
            {
                #region salva

                Control c = (Control)e.CommandSource;
                GridViewRow r = (GridViewRow)c.NamingContainer;

                PresenterAziendeESOAMB presenterAziendeESOAMB = new PresenterAziendeESOAMB();
                ValorizzaAziendeESOAMBPerSave(r);
                if (this.HasError)
                    return;

                presenterAziendeESOAMB.InserisciAziendeESOAMB(this);

                if (this.HasError)
                    RaiseShowAvviso(this, null);
                else
                {
                    this.ErrorMessage = "Azienda ESOAMB inserita correttamente";
                    RaiseShowAvviso(this, null);
                    gvAziendeESOAMB.EditIndex = -1;
                }

                AbilitaFiltro();
                ValorizzaGrigliaAziendeESOAMB(false);
                ValorizzaGrigliaGvAziendeGGmmAAAA();
                ViewState[EnumViewState.Filtro.ToString()] = false;

                #endregion salva
            }
            else if (e.CommandName == "Annulla")
            {
                gvAziendeESOAMB.EditIndex = -1;
                gvAziendeESOAMB_Load();
            }
        }

        protected void gvAziendeESOAMB_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    List<AziendeESOAMB> elencoAziendeESOAMB = (List<AziendeESOAMB>)ViewState[EnumViewState.AziendeESOAMB.ToString()];
                    if (e.Row.Cells[0].Controls.Count == 3)
                    {
                        CodeUtility.EnableEditableMode(e.Row.Cells[0], Keys.ValidationGroup_GrigliaAziendeESOAMB, Page.Theme);
                    }
                    else
                    {
                        if (e.Row.DataItemIndex == elencoAziendeESOAMB.Count - 1)
                        {
                            LinkButton add = ((LinkButton)(e.Row.Cells[0].Controls[0]));
                            add.Text = (string)"<img width=16 height=16 border=0 src=../App_themes/" + Page.Theme + "/Images/add24.png />";
                            add.ToolTip = "Aggiungi";
                            add.OnClientClick = "BlockUI();";
                        }
                        else
                        {
                            CodeUtility.EnableReadableMode(e.Row.Cells[0], e.Row.Cells[4], Page.Theme, Keys.btnDelete_GrigliaAziendeESOAMB);
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
                throw new INPS.DNA.DnaApplicationException("UCAziendeESOAMB, Errore nel metodo gvAziendeESOAMB_RowDataBound " + ex);
            }
        }

        #endregion metodi protected gridview aziende ESOAMB

        #region metodi protected filtro di ricerca

        protected void btnApplicaFiltro_Click(object sender, EventArgs e)
        {
            CaricaDati();
            gvAziendeESOAMB.EditIndex = -1;

            ViewState[EnumViewState.Filtro.ToString()] = true;
            FiltraGvAziendeESOAMB();

            gvAziendeESOAMB_Load();
            DisabilitaFiltro();
            RaiseHideInfo(this, null);
        }

        protected void btnAnnullaFiltro_Click(object sender, EventArgs e)
        {
            CaricaDati();
            gvAziendeESOAMB.EditIndex = -1;
            ViewState[EnumViewState.Filtro.ToString()] = false;
            ValorizzaGrigliaAziendeESOAMB(false);

            AbilitaFiltro();
            RaiseHideInfo(this, null);
        }

        #endregion metodi protected filtro di ricerca

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
                throw new INPS.DNA.DnaApplicationException("UCGestioneAziendeESOAMB, Errore nel metodo gvAziendeGGmmAAAA_onPageIndexChanging " + ex);
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
                throw new INPS.DNA.DnaApplicationException("UCGestioneAziendeESOAMB, Errore nel metodo gvAziendeGGmmAAAA_RowEditing " + ex);
            }
        }

        protected void gvAziendeGGmmAAAA_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Elimina")
            {
                #region Elimina
                Control c = (Control)e.CommandSource;
                GridViewRow r = (GridViewRow)c.NamingContainer;

                PresenterAziendeESOAMB presenterAziendeESOAMB = new PresenterAziendeESOAMB();

                ValorizzaAziendeGGmmAAAAPerDelete(r.DataItemIndex);
                presenterAziendeESOAMB.EliminaAziendeGGmmAAAA(this);

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
                ValorizzaGrigliaAziendeESOAMB(false);
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

                PresenterAziendeESOAMB presenterAziendeESOAMB = new PresenterAziendeESOAMB();
                ValorizzaAziendeGGmmAAAAPerSave(r);
                if (this.HasError)
                    return;

                presenterAziendeESOAMB.InserisciAziendeGGmmAAAA(this);

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
                ValorizzaGrigliaAziendeESOAMB(false);
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
                throw new INPS.DNA.DnaApplicationException("UCGestioneAziendeESOAMB, Errore nel metodo gvAziendeGGmmAAAA_RowDataBound " + ex);
            }
        }

        #endregion metodi protected GridView Aziende GGmmAAA

        #region metodi private gridview aziende ESOAMB

        private void gvAziendeESOAMB_Load()
        {
            try
            {
                gvAziendeESOAMB.DataSource = (List<AziendeESOAMB>)ViewState[EnumViewState.AziendeESOAMB.ToString()];
                gvAziendeESOAMB.DataBind();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCAziendeESOAMB, Errore nel metodo gvAziendeESOAMB_Load " + ex);
            }
        }

        private void ValorizzaGrigliaAziendeESOAMB(bool vaiAUltimaPagina)
        {
            FormattaElencoAziendeESOAMB();

            //// Va all'ultima pagina
            if (vaiAUltimaPagina)
            {
                if ((List<AziendeESOAMB>)ViewState[EnumViewState.AziendeESOAMB.ToString()] != null)
                    gvAziendeESOAMB.PageIndex = ((List<AziendeESOAMB>)ViewState[EnumViewState.AziendeESOAMB.ToString()]).Count / gvAziendeESOAMB.PageSize;
            }
            gvAziendeESOAMB_Load();
        }

        /// <summary>
        /// Errore
        /// </summary>
        private void FormattaElencoAziendeESOAMB()
        {
            List<AziendeESOAMB> elencoAziendeESOAMBWS = new List<AziendeESOAMB>();
            if (this.AziendeESOAMB.ElencoAziendeESOAMB == null || this.AziendeESOAMB.ElencoAziendeESOAMB.Count() == 0)
            {
                elencoAziendeESOAMBWS.Add(new AziendeESOAMB());
                gvAziendeESOAMB.EditIndex = 0;
            }
            else
            {
                elencoAziendeESOAMBWS = this.AziendeESOAMB.ElencoAziendeESOAMB.ToList();
                elencoAziendeESOAMBWS.Add(new AziendeESOAMB());
            }
            ViewState[EnumViewState.AziendeESOAMB.ToString()] = elencoAziendeESOAMBWS;
        }

        private void ValorizzaAziendeESOAMBPerSave(GridViewRow row)
        {
            List<AziendeESOAMB> elencoAziende = (List<AziendeESOAMB>)ViewState[EnumViewState.AziendeESOAMB.ToString()];

            elencoAziende[row.DataItemIndex].CodiceAziendaTraduzioneSuGP = (((TextBox)row.FindControl(Keys.txtCodiceAzienda)).Text);
            elencoAziende[row.DataItemIndex].Descrizione = (((TextBox)row.FindControl(Keys.txtDescrizione)).Text.ToUpperInvariant());
            elencoAziende[row.DataItemIndex].UltimaDecorrenzaAmmessa = Utility.GetDateFromString(((TextBox)row.FindControl(Keys.txtUltimaDecorrenza)).Text);

            ValorizzaAziendeESOAMB(elencoAziende, row.DataItemIndex);
        }

        private void ValorizzaAziendeESOAMB(List<AziendeESOAMB> elencoAziende, int index)
        {
            if (this.AziendeESOAMB == null)

                this.AziendeESOAMB = new AreaAziendeESOAMB();
            this.AziendeESOAMB.AziendaESOAMB = new AziendeESOAMB();

            AziendeESOAMB.AziendaESOAMB.CodiceAziendaTraduzioneSuGP = elencoAziende[index].CodiceAziendaTraduzioneSuGP;
            AziendeESOAMB.AziendaESOAMB.Descrizione = elencoAziende[index].Descrizione;
            AziendeESOAMB.AziendaESOAMB.UltimaDecorrenzaAmmessa = elencoAziende[index].UltimaDecorrenzaAmmessa;
        }

        private void ValorizzaAziendeESOAMBPerDelete(int index)
        {
            List<AziendeESOAMB> elencoAziendeESOAMB = (List<AziendeESOAMB>)ViewState[EnumViewState.AziendeESOAMB.ToString()];

            ValorizzaAziendeESOAMB(elencoAziendeESOAMB, index);
        }

        #endregion metodi private griglia aziende ESOAMB

        #region metodi private filtro di ricerca

        private void FiltraGvAziendeESOAMB()
        {
            int count = 0;
            List<AziendeESOAMB> elencoAziendeESOAMB = (List<AziendeESOAMB>)ViewState[EnumViewState.AziendeESOAMB.ToString()];

            if (!string.IsNullOrEmpty(txtFiltroCodiceAzienda.Text.Trim()))
            {
                count++;
                elencoAziendeESOAMB = elencoAziendeESOAMB.FindAll(x => (x.CodiceAziendaTraduzioneSuGP != null ? x.CodiceAziendaTraduzioneSuGP.Trim().ToUpperInvariant() : null) == txtFiltroCodiceAzienda.Text.Trim().ToUpperInvariant());
            }

            if (!string.IsNullOrEmpty(txtFiltroDescrizione.Text.Trim()))
            {
                count++;
                elencoAziendeESOAMB = elencoAziendeESOAMB.FindAll(x => (x.Descrizione != null ? x.Descrizione.Trim().ToUpperInvariant() : null) == txtFiltroDescrizione.Text.Trim().ToUpperInvariant());
            }

            if (!string.IsNullOrEmpty(txtUltimaDecorrenzaAmmessaDa.Text.Trim()))
            {
                count++;
                elencoAziendeESOAMB = elencoAziendeESOAMB.FindAll(x => Utility.DataSuccessivaA(x.UltimaDecorrenzaAmmessa.GetValueOrDefault(),
                    Utility.GetDateFromString(txtUltimaDecorrenzaAmmessaDa.Text).GetValueOrDefault()));
            }

            if (!string.IsNullOrEmpty(txtUltimaDecorrenzaAmmessaA.Text.Trim()))
            {
                count++;
                elencoAziendeESOAMB = elencoAziendeESOAMB.FindAll(x => Utility.DataSuccessivaA(Utility.GetDateFromString(txtUltimaDecorrenzaAmmessaA.Text).GetValueOrDefault(),
                    x.UltimaDecorrenzaAmmessa.GetValueOrDefault()));
            }

            if (count > 0)
            {
                ViewState[EnumViewState.AziendeESOAMB.ToString()] = elencoAziendeESOAMB;
                elencoAziendeESOAMB.Add(new AziendeESOAMB());

                DisabilitaFiltro();
            }
        }

        private void AbilitaFiltro()
        {
            PulisciFiltro();
            btnApplicaFiltro.Enabled = true;
            btnAnnullaFiltro.Enabled = false;
            txtFiltroCodiceAzienda.Enabled = true;
            txtFiltroDescrizione.Enabled = true;
            txtUltimaDecorrenzaAmmessaDa.Enabled = true;
            txtUltimaDecorrenzaAmmessaA.Enabled = true;
        }

        private void DisabilitaFiltro()
        {
            btnApplicaFiltro.Enabled = false;
            btnAnnullaFiltro.Enabled = true;
            txtFiltroCodiceAzienda.Enabled = false;
            txtFiltroDescrizione.Enabled = false;
            txtUltimaDecorrenzaAmmessaDa.Enabled = false;
            txtUltimaDecorrenzaAmmessaA.Enabled = false;
        }

        private void PulisciFiltro()
        {
            txtFiltroCodiceAzienda.Text = string.Empty;
            txtFiltroDescrizione.Text = string.Empty;
            txtUltimaDecorrenzaAmmessaDa.Text = string.Empty;
            txtUltimaDecorrenzaAmmessaA.Text = string.Empty;
        }

        private void ValorizzaAutoComplete()
        {
            GetCommaSeparatedDescrizione();
            HiddenFieldDescrizione.Value = CommaSeparatedDescrizione;
        }

        private void GetCommaSeparatedDescrizione()
        {
            StringBuilder catBuilder = new StringBuilder();

            List<AziendeESOAMB> elencoAziendeESOAMB = (List<AziendeESOAMB>)ViewState[EnumViewState.AziendeESOAMB.ToString()];
            if (elencoAziendeESOAMB != null)
            {
                foreach (AziendeESOAMB az in elencoAziendeESOAMB)
                {
                    if (az.Descrizione != null)
                    {
                        string desc = az.Descrizione.Trim();
                        catBuilder.Append(desc);
                        catBuilder.Append(";");
                    }
                }
                if (catBuilder.Length > 0)
                    catBuilder.Remove(catBuilder.Length - 1, 1);
                CommaSeparatedDescrizione = catBuilder.ToString();
            }
        }

        #endregion metodi private filtro di ricerca

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
                throw new INPS.DNA.DnaApplicationException("UCGestioneAziendeESOAMB, Errore nel metodo gvAziendeGGmmAAAA_Load " + ex);
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
            if (this.AziendeESOAMB.ElencoAziendeAssegnoGGmmAAAA == null || this.AziendeESOAMB.ElencoAziendeAssegnoGGmmAAAA.Count() == 0)
            {
                elencoAziendeGGmmAAAA.Add(new GestioneAziendeScadenzaAssegnoGGmmAAAADecAziendeScadenzaAssegnoGGmmAAAA());
                gvAziendeGGmmAAAA.EditIndex = 0;
            }
            else
            {
                elencoAziendeGGmmAAAA = this.AziendeESOAMB.ElencoAziendeAssegnoGGmmAAAA.ToList();
                elencoAziendeGGmmAAAA.Add(new GestioneAziendeScadenzaAssegnoGGmmAAAADecAziendeScadenzaAssegnoGGmmAAAA());
            }
            
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
            if (this.AziendeESOAMB == null)
                this.AziendeESOAMB = new AreaAziendeESOAMB();
            this.AziendeESOAMB.AziendaGGmmAAAA = new GestioneAziendeScadenzaAssegnoGGmmAAAADecAziendeScadenzaAssegnoGGmmAAAA();

            AziendeESOAMB.AziendaGGmmAAAA.Id = aziendeGGmmAAAA.Id;
            AziendeESOAMB.AziendaGGmmAAAA.TraduzioneSuGP = aziendeGGmmAAAA.TraduzioneSuGP;
            AziendeESOAMB.AziendaGGmmAAAA.ProgressivoRichiesto = aziendeGGmmAAAA.ProgressivoRichiesto;
        }

        private string PrelevaDescrizioneAziendeViewStateAziende(GestioneAziendeScadenzaAssegnoGGmmAAAADecAziendeScadenzaAssegnoGGmmAAAA azGGmmAAAA)
        {
            AziendeESOAMB azApp = new AziendeESOAMB();
            List<AziendeESOAMB> listaAziendeWS = new List<AziendeESOAMB>();

            if (ViewState[EnumViewState.AziendeESOAMB.ToString()] != null)
            {
                listaAziendeWS = (List<AziendeESOAMB>)ViewState[EnumViewState.AziendeESOAMB.ToString()];

                azApp = listaAziendeWS.Find(x => x.CodiceAziendaTraduzioneSuGP != null && x.CodiceAziendaTraduzioneSuGP.PadLeft(4, '0') == azGGmmAAAA.TraduzioneSuGP.PadLeft(4, '0'));
            }

            if (azApp != null)
                return azApp.Descrizione;
            else
                return string.Empty;
        }

        #endregion metodi privati gvAziendeGGmmAAAA

        /// <summary>
        /// CaricaDati, chiama il CaricaArea del presenter
        /// </summary>
        private void CaricaDati()
        {
            PresenterAziendeESOAMB presenterazESOAMB = new PresenterAziendeESOAMB();
            presenterazESOAMB.CaricaAreaAziendeESOAMB(this);

            if (this.HasError)
            {
                RaiseShowAvviso(this, null);
                return;
            }
        }

        #region Enum

        public enum EnumViewState
        {
            AziendeESOAMB,
            Filtro,
            AziendeGGmmAAAA
        }
        #endregion Enum

        #region keys

        private class Keys
        {
            public const string ValidationGroup_GrigliaAziendeESOAMB = "GrigliaAziendeESOAMB";
            public const string ValidationGroup_GrigliaAziendeGGmmAAAA = "GrigliaAziendeGGmmAAAA";
            public const string btnDelete_GrigliaAziendeESOAMB = "btnDelete";
            public const string txtCodiceAzienda = "txtCodiceAzienda";
            public const string txtDescrizione = "txtDescrizione";
            public const string txtUltimaDecorrenza = "txtDataUltimaDecorrenzaIVS";
            public const string txtCodiceAziendaGGmmAAAA = "txtCodiceAziendaGGmmAAAA";
            public const string txtCodiceProgressivoRichiesto = "txtCodiceProgressivoRichiesto";
            public const string BtnElimina_GrigliaAziendeGGmmAAAA = "btnElimina";
            public const string lblDescrizione = "lblDescrizione";
        }

        #endregion keys

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
    }
}