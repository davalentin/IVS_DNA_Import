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

namespace INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.AltreFunzioni.GestioneAziendeESOTRA
{
    public partial class UCAziendeESOTRA : CustomBaseUserControl, IAziendeESOTRA
    {
        #region IViewUI

        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }

        #endregion IViewUI

        #region IAziendeESOTRA

        public AreaAziendeESOTRA AziendeESOTRA { get; set; }
        public string CommaSeparatedDescrizione { get; set; }

        #endregion IAziendeESOTRA

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
                ValorizzaGrigliaAziendeESOTRA(true);

                ValorizzaAutoComplete();
            }

            RaiseHideInfo(this, null);

        }

        #region metodi protected gridview aziende ESOTRA

        protected void gvAziendeESOTRA_onPageIndexChanging(Object sender, GridViewPageEventArgs e)
        {
            try
            {
                gvAziendeESOTRA.EditIndex = -1;
                gvAziendeESOTRA.PageIndex = e.NewPageIndex;
                gvAziendeESOTRA_Load();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCAziendeESOTRA, Errore nel metodo gvAziendeESOTRA_onPageIndexChanging" + ex);
            }
        }

        protected void gvAziendeESOTRA_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            try
            {
                //Reset the edit index.
                if (((List<AziendeESOTRA>)ViewState[EnumViewState.AziendeESOTRA.ToString()]).Count() < 2)
                    gvAziendeESOTRA.EditIndex = 0;
                else
                    gvAziendeESOTRA.EditIndex = -1;
                //Bind data to the GridView control.
                gvAziendeESOTRA_Load();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCAziendeESOTRA, Errore nel metodo gvAziendeESOTRA_RowCancelingEdit " + ex);
            }
        }

        protected void gvAziendeESOTRA_RowEditing(object sender, GridViewEditEventArgs e)
        {
            try
            {
                gvAziendeESOTRA.EditIndex = e.NewEditIndex;
                gvAziendeESOTRA_Load();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCAziendeESOTRA, Errore nel metodo gvAziendeESOTRA_RowEditing " + ex);
            }
        }

        protected void gvAziendeESOTRA_RowCommand(object sender, GridViewCommandEventArgs e)
        {

            if (e.CommandName == "Elimina")
            {
                #region delete

                Control c = (Control)e.CommandSource;
                GridViewRow r = (GridViewRow)c.NamingContainer;

                PresenterAziendeESOTRA presenterAziendeESOTRA = new PresenterAziendeESOTRA();
                ValorizzaAziendeESOTRAPerDelete(r.DataItemIndex);

                presenterAziendeESOTRA.EliminaAziendeESOTRA(this);

                if (this.HasError)
                    RaiseShowAvviso(this, null);
                else
                {
                    this.ErrorMessage = "AziendaESOTRA eliminata correttamente";
                    RaiseShowAvviso(this, null);
                }

                AbilitaFiltro();
                ValorizzaGrigliaAziendeESOTRA(false);
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

                PresenterAziendeESOTRA presenterAziendeESOTRA = new PresenterAziendeESOTRA();
                ValorizzaAziendeESOTRAPerSave(r);
                if (this.HasError)
                    return;

                presenterAziendeESOTRA.InserisciAziendeESOTRA(this);

                if (this.HasError)
                    RaiseShowAvviso(this, null);
                else
                {
                    this.ErrorMessage = "Azienda ESOTRA inserita correttamente";
                    RaiseShowAvviso(this, null);
                    gvAziendeESOTRA.EditIndex = -1;
                }

                AbilitaFiltro();
                ValorizzaGrigliaAziendeESOTRA(false);
                ViewState[EnumViewState.Filtro.ToString()] = false;

                #endregion salva
            }
            else if (e.CommandName == "Annulla")
            {
                gvAziendeESOTRA.EditIndex = -1;
                gvAziendeESOTRA_Load();
            }
        }

        protected void gvAziendeESOTRA_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    List<AziendeESOTRA> elencoAziendeESOTRA = (List<AziendeESOTRA>)ViewState[EnumViewState.AziendeESOTRA.ToString()];
                    if (e.Row.Cells[0].Controls.Count == 3)
                    {
                        CodeUtility.EnableEditableMode(e.Row.Cells[0], Keys.ValidationGroup_GrigliaAziendeESOTRA, Page.Theme);
                    }
                    else
                    {
                        if (e.Row.DataItemIndex == elencoAziendeESOTRA.Count - 1)
                        {
                            LinkButton add = ((LinkButton)(e.Row.Cells[0].Controls[0]));
                            add.Text = (string)"<img width=16 height=16 border=0 src=../App_themes/" + Page.Theme + "/Images/add24.png />";
                            add.ToolTip = "Aggiungi";
                            add.OnClientClick = "BlockUI();";
                        }
                        else
                        {
                            CodeUtility.EnableReadableMode(e.Row.Cells[0], e.Row.Cells[4], Page.Theme, Keys.btnDelete_GrigliaAziendeESOTRA);
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
                throw new INPS.DNA.DnaApplicationException("UCAziendeESOTRA, Errore nel metodo gvAziendeESOTRA_RowDataBound " + ex);
            }
        }

        #endregion metodi protected gridview aziende ESOTRA

        #region metodi protected filtro di ricerca

        protected void btnApplicaFiltro_Click(object sender, EventArgs e)
        {
            CaricaDati();
            gvAziendeESOTRA.EditIndex = -1;

            ViewState[EnumViewState.Filtro.ToString()] = true;
            FiltraGvAziendeESOTRA();

            gvAziendeESOTRA_Load();
            DisabilitaFiltro();
            RaiseHideInfo(this, null);
        }

        protected void btnAnnullaFiltro_Click(object sender, EventArgs e)
        {
            CaricaDati();
            gvAziendeESOTRA.EditIndex = -1;
            ViewState[EnumViewState.Filtro.ToString()] = false;
            ValorizzaGrigliaAziendeESOTRA(false);

            AbilitaFiltro();
            RaiseHideInfo(this, null);
        }

        #endregion metodi protected filtro di ricerca

        #region metodi private gridview aziende ESOTRA

        private void gvAziendeESOTRA_Load()
        {
            try
            {
                gvAziendeESOTRA.DataSource = (List<AziendeESOTRA>)ViewState[EnumViewState.AziendeESOTRA.ToString()];
                gvAziendeESOTRA.DataBind();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCAziendeESOTRA, Errore nel metodo gvAziendeESOTRA_Load " + ex);
            }
        }

        private void ValorizzaGrigliaAziendeESOTRA(bool vaiAUltimaPagina)
        {
            FormattaElencoAziendeESOTRA();

            //// Va all'ultima pagina
            if (vaiAUltimaPagina)
            {
                if ((List<AziendeESOTRA>)ViewState[EnumViewState.AziendeESOTRA.ToString()] != null)
                    gvAziendeESOTRA.PageIndex = ((List<AziendeESOTRA>)ViewState[EnumViewState.AziendeESOTRA.ToString()]).Count / gvAziendeESOTRA.PageSize;
            }
            gvAziendeESOTRA_Load();
        }

        /// <summary>
        /// Errore
        /// </summary>
        private void FormattaElencoAziendeESOTRA()
        {
            List<AziendeESOTRA> elencoAziendeESOTRAWS = new List<AziendeESOTRA>();
            if (this.AziendeESOTRA.elencoAziendeESOTRA == null || this.AziendeESOTRA.elencoAziendeESOTRA.Count() == 0)
            {
                elencoAziendeESOTRAWS.Add(new AziendeESOTRA());
            }
            else
            {
                elencoAziendeESOTRAWS = this.AziendeESOTRA.elencoAziendeESOTRA.ToList();
                elencoAziendeESOTRAWS.Add(new AziendeESOTRA());
            }

            if (elencoAziendeESOTRAWS.Count() < 2)
                gvAziendeESOTRA.EditIndex = 0;

            ViewState[EnumViewState.AziendeESOTRA.ToString()] = elencoAziendeESOTRAWS;
        }

        private void ValorizzaAziendeESOTRAPerSave(GridViewRow row)
        {
            List<AziendeESOTRA> elencoAziende = (List<AziendeESOTRA>)ViewState[EnumViewState.AziendeESOTRA.ToString()];

            elencoAziende[row.DataItemIndex].CodiceAziendaTraduzioneSuGP = (((TextBox)row.FindControl(Keys.txtCodiceAzienda)).Text);
            elencoAziende[row.DataItemIndex].Descrizione = (((TextBox)row.FindControl(Keys.txtDescrizione)).Text.ToUpperInvariant());
            elencoAziende[row.DataItemIndex].UltimaDecorrenzaAmmessa = Utility.GetDateFromString(((TextBox)row.FindControl(Keys.txtUltimaDecorrenza)).Text);

            ValorizzaAziendeESOTRA(elencoAziende, row.DataItemIndex);
        }

        private void ValorizzaAziendeESOTRA(List<AziendeESOTRA> elencoAziende, int index)
        {
            if (this.AziendeESOTRA == null)

                this.AziendeESOTRA = new AreaAziendeESOTRA();
            this.AziendeESOTRA.AziendaESOTRA = new AziendeESOTRA();

            AziendeESOTRA.AziendaESOTRA.CodiceAziendaTraduzioneSuGP = elencoAziende[index].CodiceAziendaTraduzioneSuGP;
            AziendeESOTRA.AziendaESOTRA.Descrizione = elencoAziende[index].Descrizione;
            AziendeESOTRA.AziendaESOTRA.UltimaDecorrenzaAmmessa = elencoAziende[index].UltimaDecorrenzaAmmessa;
        }

        private void ValorizzaAziendeESOTRAPerDelete(int index)
        {
            List<AziendeESOTRA> elencoAziendeESOTRA = (List<AziendeESOTRA>)ViewState[EnumViewState.AziendeESOTRA.ToString()];

            ValorizzaAziendeESOTRA(elencoAziendeESOTRA, index);
        }

        #endregion metodi private griglia aziende ESOTRA

        #region metodi private filtro di ricerca

        private void FiltraGvAziendeESOTRA()
        {
            int count = 0;
            List<AziendeESOTRA> elencoAziendeESOTRA = (List<AziendeESOTRA>)ViewState[EnumViewState.AziendeESOTRA.ToString()];

            if (!string.IsNullOrEmpty(txtFiltroCodiceAzienda.Text.Trim()))
            {
                count++;
                elencoAziendeESOTRA = elencoAziendeESOTRA.FindAll(x => (x.CodiceAziendaTraduzioneSuGP != null ? x.CodiceAziendaTraduzioneSuGP.Trim().ToUpperInvariant() : null) == txtFiltroCodiceAzienda.Text.Trim().ToUpperInvariant());
            }

            if (!string.IsNullOrEmpty(txtFiltroDescrizione.Text.Trim()))
            {
                count++;
                elencoAziendeESOTRA = elencoAziendeESOTRA.FindAll(x => (x.Descrizione != null ? x.Descrizione.Trim().ToUpperInvariant() : null) == txtFiltroDescrizione.Text.Trim().ToUpperInvariant());
            }

            if (!string.IsNullOrEmpty(txtUltimaDecorrenzaAmmessaDa.Text.Trim()))
            {
                count++;
                elencoAziendeESOTRA = elencoAziendeESOTRA.FindAll(x => Utility.DataSuccessivaA(x.UltimaDecorrenzaAmmessa.GetValueOrDefault(),
                    Utility.GetDateFromString(txtUltimaDecorrenzaAmmessaDa.Text).GetValueOrDefault()));
            }

            if (!string.IsNullOrEmpty(txtUltimaDecorrenzaAmmessaA.Text.Trim()))
            {
                count++;
                elencoAziendeESOTRA = elencoAziendeESOTRA.FindAll(x => Utility.DataSuccessivaA(Utility.GetDateFromString(txtUltimaDecorrenzaAmmessaA.Text).GetValueOrDefault(),
                    x.UltimaDecorrenzaAmmessa.GetValueOrDefault()));
            }

            if (count > 0)
            {
                ViewState[EnumViewState.AziendeESOTRA.ToString()] = elencoAziendeESOTRA;
                elencoAziendeESOTRA.Add(new AziendeESOTRA());

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

            List<AziendeESOTRA> elencoAziendeESOTRA = (List<AziendeESOTRA>)ViewState[EnumViewState.AziendeESOTRA.ToString()];
            if (elencoAziendeESOTRA != null)
            {
                foreach (AziendeESOTRA az in elencoAziendeESOTRA)
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

        /// <summary>
        /// CaricaDati, chiama il CaricaArea del presenter
        /// </summary>
        private void CaricaDati()
        {
            PresenterAziendeESOTRA presenterazESOTRA = new PresenterAziendeESOTRA();
            presenterazESOTRA.CaricaAreaAziendeESOTRA(this);

            if (this.HasError)
            {
                RaiseShowAvviso(this, null);
                return;
            }
        }

        #region Enum

        public enum EnumViewState
        {
            AziendeESOTRA,
            Filtro
        }
        #endregion Enum

        #region keys

        private class Keys
        {
            public const string ValidationGroup_GrigliaAziendeESOTRA = "GrigliaAziendeESOTRA";
            public const string btnDelete_GrigliaAziendeESOTRA = "btnDelete";
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
    }
}