using INPS.DNA;
using INPS.Pensioni.LiquidazionePensione.Presenter;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.AltreFunzioni.GestioneAziendeVOESO
{
    public partial class UCAziendeVOESO : CustomBaseUserControl, IAziendeVOESO
    {
        #region IViewUI

        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }

        #endregion IViewUI

        #region IAziendeVOESO
        public AreaAziendeVOESO AziendeVOESO { get; set; }
        #endregion IAziendeVOESO

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

        protected void btnSceltaAzienda_Click(object sender, EventArgs e)
        {
            pnlScelta.Visible = false;
            gvAziendeVOESO.Visible = true;
            trFiltro.Visible = true;
            switch(ddlAziende.SelectedValue)
            {
                case "0033":
                    lblScelta.Text = "Aziende VOESO - Dipendenti Ex Monopoli";
                    break;
                case "0034":
                    lblScelta.Text = "Aziende VOESO - Riscossione Tributi Erariali";
                    break;
                case "0036":
                    lblScelta.Text = "Aziende VOESO - Ferrovie dello Stato";
                    break;
                case "0053":
                    lblScelta.Text = "Aziende VOESO - Ferrovie dello Stato (solidaristico)";
                    break;
            }

            //In base alla scelta chiamo il CaricaDati e il ValorizzaGrigliaAziendeCredito
            CaricaDati(ddlAziende.SelectedItem.Value);
            AbilitaFiltro();
            ValorizzaGrigliaAziendeVOESO(true);
            RaiseChangeTipo(this, null);
        }

        public string GetTipoVOESO()
        {
            return ddlAziende.SelectedValue;
        }

        #region private methods

        /// <summary>
        /// CaricaDdl, carico Aziende VOESO in DropDownList
        /// </summary>
        private void CaricaDdl()
        {
            ddlAziende.Items.Clear();
            ddlAziende.Items.Add(new ListItem("VOESO - Dipendenti Ex Monopoli", "0033"));
            ddlAziende.Items.Add(new ListItem("VOESO - Riscossione Tributi Erariali", "0034"));
            ddlAziende.Items.Add(new ListItem("VOESO - Ferrovie dello Stato", "0036"));
            ddlAziende.Items.Add(new ListItem("VOESO - Ferrovie dello Stato (solidaristico)", "0053"));
        }

        /// <summary>
        /// CaricaDati, chiama il CaricaArea del presenter
        /// </summary>
        private void CaricaDati(string categoriaAzienda)
        {
            PresenterAziendeVOESO presenterazVOESO = new PresenterAziendeVOESO();
            presenterazVOESO.CaricaAreaAziendeVOESO(categoriaAzienda, this);

            if (this.HasError)
            {
                RaiseShowAvviso(this, null);
                return;
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

        private void ValorizzaGrigliaAziendeVOESO(bool vaiAUltimaPagina)
        {
            FormattaElencoAziendeVOESO();

            //// Va all'ultima pagina
            if (vaiAUltimaPagina)
            {

                if ((List<AziendeVOESO>)ViewState[EnumViewState.AziendeVOESO.ToString()] != null)
                    gvAziendeVOESO.PageIndex = ((List<AziendeVOESO>)ViewState[EnumViewState.AziendeVOESO.ToString()]).Count / gvAziendeVOESO.PageSize;
            }
            gvAziendeVOESO_Load();
        }

        private void FormattaElencoAziendeVOESO()
        {
            List<AziendeVOESO> elencoAziendeVOESOWS = new List<AziendeVOESO>();
            if (this.AziendeVOESO.ElencoAziendeVOESO == null || this.AziendeVOESO.ElencoAziendeVOESO.Count() == 0)
                elencoAziendeVOESOWS.Add(new AziendeVOESO());
            else
            {
                elencoAziendeVOESOWS = this.AziendeVOESO.ElencoAziendeVOESO.ToList();
                elencoAziendeVOESOWS.Add(new AziendeVOESO());
            }

            if (elencoAziendeVOESOWS.Count() < 2)
                gvAziendeVOESO.EditIndex = 0;

            ViewState[EnumViewState.AziendeVOESO.ToString()] = elencoAziendeVOESOWS;
        }

        private void ValorizzaAziendeVOESOPerDelete(int index)
        {
            List<AziendeVOESO> elencoAziendeCredito = (List<AziendeVOESO>)ViewState[EnumViewState.AziendeVOESO.ToString()];

            ValorizzaAziendeVOESO(elencoAziendeCredito, index);
        }

        private void ValorizzaAziendeVOESOPerSave(GridViewRow row)
        {
            List<AziendeVOESO> elencoAziende = (List<AziendeVOESO>)ViewState[EnumViewState.AziendeVOESO.ToString()];

            elencoAziende[row.DataItemIndex].Descrizione = (((TextBox)row.FindControl(Keys.txtDescrizione)).Text.ToUpperInvariant());
            elencoAziende[row.DataItemIndex].UltimaDecorrenzaAmmessa = Utility.GetDateFromString(((TextBox)row.FindControl(Keys.txtUltimaDecorrenza)).Text);
            elencoAziende[row.DataItemIndex].CodiceAziendaTraduzioneSuGP = (((TextBox)row.FindControl(Keys.txtCodiceAzienda)).Text);
            elencoAziende[row.DataItemIndex].Tipo = ddlAziende.SelectedItem.Value;

            ValorizzaAziendeVOESO(elencoAziende, row.DataItemIndex);
        }

        private void ValorizzaAziendeVOESO(List<AziendeVOESO> elencoAziende, int index)
        {
            if (this.AziendeVOESO == null)

                this.AziendeVOESO = new AreaAziendeVOESO();
            this.AziendeVOESO.AziendaVOESO = new AziendeVOESO();

            AziendeVOESO.AziendaVOESO.CodiceAziendaTraduzioneSuGP = elencoAziende[index].CodiceAziendaTraduzioneSuGP;
            AziendeVOESO.AziendaVOESO.Descrizione = elencoAziende[index].Descrizione;
            AziendeVOESO.AziendaVOESO.UltimaDecorrenzaAmmessa = elencoAziende[index].UltimaDecorrenzaAmmessa;
            AziendeVOESO.AziendaVOESO.Tipo = elencoAziende[index].Tipo;
        }

        private void FiltraGvAziendeVOESO()
        {
            int count = 0;
            List<AziendeVOESO> elencoAziendeVOESO = (List<AziendeVOESO>)ViewState[EnumViewState.AziendeVOESO.ToString()];

            if (!string.IsNullOrEmpty(txtFiltroCodiceAzienda.Text.Trim()))
            {
                count++;
                elencoAziendeVOESO = elencoAziendeVOESO.FindAll(x => (x.CodiceAziendaTraduzioneSuGP != null ? x.CodiceAziendaTraduzioneSuGP.Trim().ToUpperInvariant() : null) == txtFiltroCodiceAzienda.Text.Trim().ToUpperInvariant());
            }

            if (!string.IsNullOrEmpty(txtFiltroDescrizione.Text.Trim()))
            {
                count++;
                elencoAziendeVOESO = elencoAziendeVOESO.FindAll(x => (x.Descrizione != null ? x.Descrizione.Trim().ToUpperInvariant() : null) == txtFiltroDescrizione.Text.Trim().ToUpperInvariant());
            }

            if (!string.IsNullOrEmpty(txtUltimaDecorrenzaAmmessaDa.Text.Trim()))
            {
                count++;
                elencoAziendeVOESO = elencoAziendeVOESO.FindAll(x => x.UltimaDecorrenzaAmmessa.HasValue && 
                    Utility.DataSuccessivaA(x.UltimaDecorrenzaAmmessa.Value, Utility.GetDateFromString(txtUltimaDecorrenzaAmmessaDa.Text).Value));
            }

            if (!string.IsNullOrEmpty(txtUltimaDecorrenzaAmmessaA.Text.Trim()))
            {
                count++;
                elencoAziendeVOESO = elencoAziendeVOESO.FindAll(x => x.UltimaDecorrenzaAmmessa.HasValue && 
                    Utility.DataSuccessivaA(Utility.GetDateFromString(txtUltimaDecorrenzaAmmessaA.Text).Value, x.UltimaDecorrenzaAmmessa.Value));
            }

            if (count > 0)
            {
                ViewState[EnumViewState.AziendeVOESO.ToString()] = elencoAziendeVOESO;
                elencoAziendeVOESO.Add(new AziendeVOESO());

                DisabilitaFiltro();
            }
        }

        #endregion private methods

        #region metodi protected filtro di ricerca

        protected void btnApplicaFiltro_Click(object sender, EventArgs e)
        {
            CaricaDati(ddlAziende.SelectedItem.Value);
            gvAziendeVOESO.EditIndex = -1;

            ViewState[EnumViewState.Filtro.ToString()] = true;
            FiltraGvAziendeVOESO();

            gvAziendeVOESO_Load();
            DisabilitaFiltro();
            RaiseHideInfo(this, null);
        }

        protected void btnAnnullaFiltro_Click(object sender, EventArgs e)
        {
            CaricaDati(ddlAziende.SelectedItem.Value);
            gvAziendeVOESO.EditIndex = -1;
            ViewState[EnumViewState.Filtro.ToString()] = false;
            ValorizzaGrigliaAziendeVOESO(false);

            AbilitaFiltro();
            RaiseHideInfo(this, null);
        }

        #endregion metodi protected filtro di ricerca

        #region gvAziendeVOESO
        private void gvAziendeVOESO_Load()
        {
            try
            {
                gvAziendeVOESO.DataSource = (List<AziendeVOESO>)ViewState[EnumViewState.AziendeVOESO.ToString()];
                gvAziendeVOESO.DataBind();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCAziendeVOESO, Errore nel metodo gvAziendeVOESO_Load " + ex);
            }
        }

        protected void gvAziendeVOESO_onPageIndexChanging(Object sender, GridViewPageEventArgs e)
        {
            try
            {
                gvAziendeVOESO.EditIndex = -1;
                gvAziendeVOESO.PageIndex = e.NewPageIndex;
                gvAziendeVOESO_Load();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCAziendeVOESO, Errore nel metodo gvAziendeVOESO_onPageIndexChanging" + ex);
            }
        }

        protected void gvAziendeVOESO_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            try
            {
                //Reset the edit index.
                if (((List<AziendeVOESO>)ViewState[EnumViewState.AziendeVOESO.ToString()]).Count() < 2)
                    gvAziendeVOESO.EditIndex = 0;
                else
                    gvAziendeVOESO.EditIndex = -1;
                //Bind data to the GridView control.
                gvAziendeVOESO_Load();

            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCAziendeVOESO, Errore nel metodo gvAziendeVOESO_RowCancelingEdit " + ex);
            }
        }

        protected void gvAziendeVOESO_RowEditing(object sender, GridViewEditEventArgs e)
        {
            try
            {
                gvAziendeVOESO.EditIndex = e.NewEditIndex;
                gvAziendeVOESO_Load();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCAziendeVOESO, Errore nel metodo gvAziendeVOESO_RowEditing " + ex);
            }
        }

        protected void gvAziendeVOESO_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Elimina")
            {
                #region delete

                Control c = (Control)e.CommandSource;
                GridViewRow r = (GridViewRow)c.NamingContainer;

                PresenterAziendeVOESO presenterAziendeVOESO = new PresenterAziendeVOESO();
                ValorizzaAziendeVOESOPerDelete(r.DataItemIndex);

                presenterAziendeVOESO.EliminaAziendeVOESO(ddlAziende.SelectedItem.Value, this);

                if (this.HasError)
                    RaiseShowAvviso(this, null);
                else
                {
                    this.ErrorMessage = "Azienda eliminata correttamente";
                    RaiseShowAvviso(this, null);
                }

                AbilitaFiltro();
                ValorizzaGrigliaAziendeVOESO(false);
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
                
                PresenterAziendeVOESO presenterAziendeVOESO = new PresenterAziendeVOESO();
                ValorizzaAziendeVOESOPerSave(r);
                if (this.HasError)
                    return;

                presenterAziendeVOESO.InserisciAziendeVOESO(ddlAziende.SelectedItem.Value, this);

                if (this.HasError)
                    RaiseShowAvviso(this, null);
                else
                {
                    this.ErrorMessage = "Azienda inserita correttamente";
                    RaiseShowAvviso(this, null);
                    gvAziendeVOESO.EditIndex = -1;
                }

                AbilitaFiltro();
                ValorizzaGrigliaAziendeVOESO(false);
                ViewState[EnumViewState.Filtro.ToString()] = false;

                #endregion salva
            }

            else if (e.CommandName == "Annulla")
            {
                gvAziendeVOESO.EditIndex = -1;
                gvAziendeVOESO_Load();
            }
        }

        protected void gvAziendeVOESO_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    List<AziendeVOESO> elencoAziendeCredito = (List<AziendeVOESO>)ViewState[EnumViewState.AziendeVOESO.ToString()];
                    if (e.Row.Cells[0].Controls.Count == 3)
                    {
                        CodeUtility.EnableEditableMode(e.Row.Cells[0], Keys.ValidationGroup_GrigliaAziendeVOESO, Page.Theme);
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
                            CodeUtility.EnableReadableMode(e.Row.Cells[0], e.Row.Cells[4], Page.Theme, Keys.btnDelete_GrigliaAziendeVOESO);
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
                throw new INPS.DNA.DnaApplicationException("UCAziendeVOESO, Errore nel metodo gvAziendeVOESO_RowDataBound " + ex);
            }
        }
        #endregion gvAziedeVOESO

        #region Events
        public event EventHandler ShowAvviso;
        public event EventHandler HideInfo;
        public event EventHandler ChangeTipo;

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

        protected void RaiseChangeTipo(object sender, EventArgs e)
        {
            if (ChangeTipo != null)
                ChangeTipo(sender, e);
        }
        #endregion Events

        #region Enum

        public enum EnumViewState
        {
            AziendeVOESO,
            Filtro
        }
        #endregion Enum

        #region Keys
        public class Keys
        {
            public const string VOESO = "VOESO";
            public const string ValidationGroup_GrigliaAziendeVOESO = "GrigliaAziendeVOESO";
            public const string btnDelete_GrigliaAziendeVOESO = "btnDelete";
            public const string txtDescrizione = "txtDescrizione";
            public const string txtUltimaDecorrenza = "txtDataUltimaDecorrenzaIVS";
            public const string txtCodiceAzienda = "txtCodiceAzienda";
        }
        #endregion Keys
    }
}