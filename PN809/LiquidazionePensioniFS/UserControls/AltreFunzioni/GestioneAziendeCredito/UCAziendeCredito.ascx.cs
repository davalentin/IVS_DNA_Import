using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using INPS.DNA;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter;
using INPS.Pensioni.LiquidazionePensione.Presenter.Enum;

namespace INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.AltreFunzioni.GestioneAziendeCredito
{
    public partial class UCAziendeCredito : CustomBaseUserControl, IAziendeCredito
    {
        #region IViewUI

        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }

        #endregion IViewUI

        #region IAziendeCredito

        public AreaAziendeCredito AziendeCredito { get; set; }
        public string CommaSeparatedDescrizione { get; set; }

        #endregion IAziendeCredito

        /// <summary>
        /// page load, richiama il CaricaDdl che a sua volta chiama il caricaDati e il ValorizzaGriglia
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (ViewState[EnumViewState.Filtro.ToString()] == null)
                ViewState[EnumViewState.Filtro.ToString()] = false;

            if (!IsPostBack)
            {
                CaricaDdl();
            }

            RaiseHideInfo(this, null);

        }

        #region metodi protected gridview aziende Credito

        protected void gvAziendeCredito_onPageIndexChanging(Object sender, GridViewPageEventArgs e)
        {
            try
            {
                gvAziendeCredito.EditIndex = -1;
                gvAziendeCredito.PageIndex = e.NewPageIndex;
                gvAziendeCredito_Load();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCAziendeCredito, Errore nel metodo gvAziendeCredito_onPageIndexChanging" + ex);
            }
        }

        protected void gvAziendeCredito_onRowDeleting(object sender, GridViewDeleteEventArgs e)
        {

        }

        protected void gvAziendeCredito_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            try
            {
                //Reset the edit index.
                if (((List<AziendeCredito>)ViewState[EnumViewState.AziendeCredito.ToString()]).Count() < 2)
                    gvAziendeCredito.EditIndex = 0;
                else
                    gvAziendeCredito.EditIndex = -1;
                //Bind data to the GridView control.
                gvAziendeCredito_Load();

            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCAziendeCredito, Errore nel metodo gvAziendeCredito_RowCancelingEdit " + ex);
            }
        }

        protected void gvAziendeCredito_RowEditing(object sender, GridViewEditEventArgs e)
        {
            try
            {
                gvAziendeCredito.EditIndex = e.NewEditIndex;
                gvAziendeCredito_Load();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCAziendeCredito, Errore nel metodo gvAziendeCredito_RowEditing " + ex);
            }
        }

        protected void gvAziendeCredito_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            
            if (e.CommandName == "Elimina")
            {
                #region delete

                Control c = (Control)e.CommandSource;
                GridViewRow r = (GridViewRow)c.NamingContainer;

                PresenterAziendeCredito presenterAziendeCredito = new PresenterAziendeCredito();
                ValorizzaAziendeCreditoPerDelete(r.DataItemIndex);

                presenterAziendeCredito.EliminaAziendeCredito(ddlAziende.SelectedItem.Value, this);

                if (this.HasError)
                {
                    RaiseShowAvviso(this, null);
       
                }
                else
                {
                    this.ErrorMessage = "AziendaCredito eliminata correttamente";
                    RaiseShowAvviso(this, null);
                }

                AbilitaFiltro();
                ValorizzaGrigliaAziendeCredito(false);
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



                PresenterAziendeCredito presenterAziendeCredito = new PresenterAziendeCredito();
                ValorizzaAziendeCreditoPerSave(r);
                if (this.HasError)
                    return;

                presenterAziendeCredito.InserisciAziendeCredito(ddlAziende.SelectedItem.Value, this);

                if (this.HasError)
                {
                    RaiseShowAvviso(this, null);
                  
                }
                else
                {
                    this.ErrorMessage = "Azienda Credito inserita correttamente";
                    RaiseShowAvviso(this, null);
                    gvAziendeCredito.EditIndex = -1;
                }

                AbilitaFiltro();
                ValorizzaGrigliaAziendeCredito(false);
                ViewState[EnumViewState.Filtro.ToString()] = false;

                #endregion salva
            }

            else if (e.CommandName == "Annulla")
            {
                gvAziendeCredito.EditIndex = -1;
                gvAziendeCredito_Load();
            }
        }

        protected void gvAziendeCredito_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    List<AziendeCredito> elencoAziendeCredito = (List<AziendeCredito>)ViewState[EnumViewState.AziendeCredito.ToString()];
                    if (e.Row.Cells[0].Controls.Count == 3)
                    {
                        CodeUtility.EnableEditableMode(e.Row.Cells[0], Keys.ValidationGroup_GrigliaAziendeCredito, Page.Theme);
                    }
                    else
                    {
                        if (e.Row.DataItemIndex == elencoAziendeCredito.Count - 1)
                        {
                            LinkButton add = ((LinkButton)(e.Row.Cells[0].Controls[0]));
                            add.Text = (string)"<img width=16 height=16 border=0 src=../App_themes/" + Page.Theme + "/Images/add24.png />";
                            add.ToolTip = "Aggiungi";
                            add.OnClientClick = "BlockUI();";
                        }
                        else
                        {
                            CodeUtility.EnableReadableMode(e.Row.Cells[0], e.Row.Cells[4], Page.Theme, Keys.btnDelete_GrigliaAziendeCredito);
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
                throw new INPS.DNA.DnaApplicationException("UCAziendeCredito, Errore nel metodo gvAziendeCredito_RowDataBound " + ex);
            }
        }

        #endregion metodi protected gridview aziende Credito

        #region metodi protected filtro di ricerca

        protected void btnApplicaFiltro_Click(object sender, EventArgs e)
        {
            CaricaDati(ddlAziende.SelectedItem.Value);
            gvAziendeCredito.EditIndex = -1;

            ViewState[EnumViewState.Filtro.ToString()] = true;
            FiltraGvAziendeCredito();

            gvAziendeCredito_Load();
            DisabilitaFiltro();
            RaiseHideInfo(this, null);
        }

        protected void btnAnnullaFiltro_Click(object sender, EventArgs e)
        {
            CaricaDati(ddlAziende.SelectedItem.Value);
            gvAziendeCredito.EditIndex = -1;
            ViewState[EnumViewState.Filtro.ToString()] = false;
            ValorizzaGrigliaAziendeCredito(false);

            AbilitaFiltro();
            RaiseHideInfo(this, null);
        }

        #endregion metodi protected filtro di ricerca

        #region metodi private gridview aziende Credito

        private void gvAziendeCredito_Load()
        {
            try
            {
                gvAziendeCredito.DataSource = (List<AziendeCredito>)ViewState[EnumViewState.AziendeCredito.ToString()];
                gvAziendeCredito.DataBind();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCAziendeCredito, Errore nel metodo gvAziendeCredito_Load " + ex);
            }
        }

        private void ValorizzaGrigliaAziendeCredito(bool vaiAUltimaPagina)
        {
            FormattaElencoAziendeCredito();

            //// Va all'ultima pagina
            if (vaiAUltimaPagina)
            {

                if ((List<AziendeCredito>)ViewState[EnumViewState.AziendeCredito.ToString()] != null)
                    gvAziendeCredito.PageIndex = ((List<AziendeCredito>)ViewState[EnumViewState.AziendeCredito.ToString()]).Count / gvAziendeCredito.PageSize;
            }
                gvAziendeCredito_Load();
            
        }

        /// <summary>
        /// Errore
        /// </summary>
        private void FormattaElencoAziendeCredito()
        {
            List<AziendeCredito> elencoAziendeCreditoWS = new List<AziendeCredito>();
            if (this.AziendeCredito.elencoAziendeCredito == null || this.AziendeCredito.elencoAziendeCredito.Count() == 0)
            {
                elencoAziendeCreditoWS.Add(new AziendeCredito());
            }
            else
            {
                elencoAziendeCreditoWS = this.AziendeCredito.elencoAziendeCredito.ToList();
                elencoAziendeCreditoWS.Add(new AziendeCredito());
            }

            if (elencoAziendeCreditoWS.Count() < 2)

                gvAziendeCredito.EditIndex = 0;

            ViewState[EnumViewState.AziendeCredito.ToString()] = elencoAziendeCreditoWS;
        }

        private void ValorizzaAziendeCreditoPerSave(GridViewRow row)
        {
            List<AziendeCredito> elencoAziende = (List<AziendeCredito>)ViewState[EnumViewState.AziendeCredito.ToString()];

            elencoAziende[row.DataItemIndex].CodiceAziendaTraduzioneSuGP = (((TextBox)row.FindControl(Keys.txtCodiceAzienda)).Text);
            elencoAziende[row.DataItemIndex].Descrizione = (((TextBox)row.FindControl(Keys.txtDescrizione)).Text.ToUpperInvariant());
            elencoAziende[row.DataItemIndex].UltimaDecorrenzaAmmessa = Utility.GetDateFromString(((TextBox)row.FindControl(Keys.txtUltimaDecorrenza)).Text);
            elencoAziende[row.DataItemIndex].CodiceAziendaTraduzioneSuGP = (((TextBox)row.FindControl(Keys.txtCodiceAzienda)).Text);
            elencoAziende[row.DataItemIndex].SiglaCatPensione = ddlAziende.SelectedItem.Value;

            ValorizzaAziendeCredito(elencoAziende, row.DataItemIndex);
        }

        private void ValorizzaAziendeCredito(List<AziendeCredito> elencoAziende, int index)
        {
            if (this.AziendeCredito == null)

                this.AziendeCredito = new AreaAziendeCredito();
            this.AziendeCredito.AziendaCredito = new AziendeCredito();

            AziendeCredito.AziendaCredito.CodiceAziendaTraduzioneSuGP = elencoAziende[index].CodiceAziendaTraduzioneSuGP;
            AziendeCredito.AziendaCredito.Descrizione = elencoAziende[index].Descrizione;
            AziendeCredito.AziendaCredito.UltimaDecorrenzaAmmessa = elencoAziende[index].UltimaDecorrenzaAmmessa;
            AziendeCredito.AziendaCredito.SiglaCatPensione = elencoAziende[index].SiglaCatPensione;
        }

        private void ValorizzaAziendeCreditoPerDelete(int index)
        {
            List<AziendeCredito> elencoAziendeCredito = (List<AziendeCredito>)ViewState[EnumViewState.AziendeCredito.ToString()];

            ValorizzaAziendeCredito(elencoAziendeCredito, index);
        }

        #endregion metodi private griglia aziende Credito

        #region metodi private filtro di ricerca

        private void FiltraGvAziendeCredito()
        {
            int count = 0;
            List<AziendeCredito> elencoAziendeCredito = (List<AziendeCredito>)ViewState[EnumViewState.AziendeCredito.ToString()]; 

            if (!string.IsNullOrEmpty(txtFiltroCodiceAzienda.Text.Trim()))
            {
                count++;
                elencoAziendeCredito = elencoAziendeCredito.FindAll(x => (x.CodiceAziendaTraduzioneSuGP != null ? x.CodiceAziendaTraduzioneSuGP.Trim().ToUpperInvariant() : null) == txtFiltroCodiceAzienda.Text.Trim().ToUpperInvariant());
            }

            if (!string.IsNullOrEmpty(txtFiltroDescrizione.Text.Trim()))
            {
                count++;
                elencoAziendeCredito = elencoAziendeCredito.FindAll(x => (x.Descrizione != null ? x.Descrizione.Trim().ToUpperInvariant() : null) == txtFiltroDescrizione.Text.Trim().ToUpperInvariant());
            }

            if (!string.IsNullOrEmpty(txtUltimaDecorrenzaAmmessaDa.Text.Trim()))
            {
                count++;
                elencoAziendeCredito = elencoAziendeCredito.FindAll(x => Utility.DataSuccessivaA(x.UltimaDecorrenzaAmmessa.GetValueOrDefault(),
                    Utility.GetDateFromString(txtUltimaDecorrenzaAmmessaDa.Text).GetValueOrDefault()));
            }

            if (!string.IsNullOrEmpty(txtUltimaDecorrenzaAmmessaA.Text.Trim()))
            {
                count++;
                elencoAziendeCredito = elencoAziendeCredito.FindAll(x => Utility.DataSuccessivaA(Utility.GetDateFromString(txtUltimaDecorrenzaAmmessaA.Text).GetValueOrDefault(), 
                    x.UltimaDecorrenzaAmmessa.GetValueOrDefault()));
            }

            if (count > 0)
            {
                ViewState[EnumViewState.AziendeCredito.ToString()] = elencoAziendeCredito;
                elencoAziendeCredito.Add(new AziendeCredito());

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

            List<AziendeCredito> elencoAziendeCredito = (List<AziendeCredito>)ViewState[EnumViewState.AziendeCredito.ToString()];
            if (elencoAziendeCredito != null)
            {
                foreach (AziendeCredito az in elencoAziendeCredito)
                {
                    if (az.Descrizione != null)
                    {
                        string desc = az.Descrizione.Trim();
                        catBuilder.Append(desc);
                        catBuilder.Append(";");
                    }
                }
                catBuilder.Remove(catBuilder.Length - 1, 1);
                CommaSeparatedDescrizione = catBuilder.ToString();
            }
        }

        #endregion metodi private filtro di ricerca

        /// <summary>
        /// CaricaDdl, carico Aziende Credito in DropDownList
        /// </summary>
        private void CaricaDdl()
        {
            ddlAziende.Items.Clear();
            ddlAziende.Items.Add(new ListItem("VOCRED - CRED27", "VOCRED"));
            ddlAziende.Items.Add(new ListItem("VOCOOP - COOP28", "VOCOOP"));
        }

        /// <summary>
        /// CaricaDati, chiama il CaricaArea del presenter
        /// </summary>
        private void CaricaDati(string categoriaAzienda)
        {
            PresenterAziendeCredito presenterazCredito = new PresenterAziendeCredito();
            presenterazCredito.CaricaAreaAziendeCredito(categoriaAzienda, this);

            if (this.HasError)
            {
                RaiseShowAvviso(this, null);
                return;
            }
        }

        #region Enum

        public enum EnumViewState
        {
            AziendeCredito,
            Filtro
        }
        #endregion Enum

        #region keys

        private class Keys
        {
            public const string ValidationGroup_GrigliaAziendeCredito = "GrigliaAziendeCredito";
            public const string btnDelete_GrigliaAziendeCredito = "btnDelete";
            public const string txtCodiceAzienda = "txtCodiceAzienda";
            public const string txtDescrizione = "txtDescrizione";
            public const string txtUltimaDecorrenza = "txtDataUltimaDecorrenzaIVS";
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

        protected void btnSceltaAzienda_Click(object sender, EventArgs e)
        {
            pnlScelta.Visible = false;
            gvAziendeCredito.Visible = true;
            trFiltro.Visible = true;
            lblScelta.Text = "Aziende Credito: " + ddlAziende.SelectedItem.Text;

            //In base alla scelta chiamo il CaricaDati e il ValorizzaGrigliaAziendeCredito
            CaricaDati(ddlAziende.SelectedItem.Value);
            AbilitaFiltro();
            ValorizzaGrigliaAziendeCredito(true);

            ValorizzaAutoComplete();
        }
    }
}