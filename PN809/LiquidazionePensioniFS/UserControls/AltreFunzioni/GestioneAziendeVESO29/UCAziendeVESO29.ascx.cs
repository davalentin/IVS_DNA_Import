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

namespace INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.AltreFunzioni.GestioneAziendeVESO29
{
    public partial class UCAziendeVESO29 : CustomBaseUserControl, IAziendeVESO29
    {
        #region IViewUI

        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }

        #endregion IViewUI

        #region IAziendeVESO29

        public AreaAziendeVESO29 AziendeVESO29 { get; set; }
        public string CommaSeparatedDescrizione { get; set; }

        #endregion IAziendeVESO29

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
                ValorizzaGrigliaAziendeVESO29(true);

                ValorizzaAutoComplete();
            }

            RaiseHideInfo(this, null);

        }

        #region metodi protected gridview aziende veso29

        protected void gvAziendeVESO29_onPageIndexChanging(Object sender, GridViewPageEventArgs e)
        {
            try
            {
                gvAziendeVESO29.EditIndex = -1;
                gvAziendeVESO29.PageIndex = e.NewPageIndex;
                gvAziendeVESO29_Load();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCAziendeVESO29, Errore nel metodo gvAziendeVESO29_onPageIndexChanging" + ex);
            }
        }

        protected void gvAziendeVESO29_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            try
            {
                //Reset the edit index.
                if (((List<AziendeVESO29>)ViewState[EnumViewState.AziendeVESO29.ToString()]).Count() < 2)
                    gvAziendeVESO29.EditIndex = 0;
                else
                    gvAziendeVESO29.EditIndex = -1;
                //Bind data to the GridView control.
                gvAziendeVESO29_Load();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCAziendeVESO29, Errore nel metodo gvAziendeVESO29_RowCancelingEdit " + ex);
            }
        }

        protected void gvAziendeVESO29_RowEditing(object sender, GridViewEditEventArgs e)
        {
            try
            {
                gvAziendeVESO29.EditIndex = e.NewEditIndex;
                gvAziendeVESO29_Load();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCAziendeVESO29, Errore nel metodo gvAziendeVESO29_RowEditing " + ex);
            }
        }

        protected void gvAziendeVESO29_RowCommand(object sender, GridViewCommandEventArgs e)
        {

            if (e.CommandName == "Elimina")
            {
                #region delete

                Control c = (Control)e.CommandSource;
                GridViewRow r = (GridViewRow)c.NamingContainer;

                PresenterAziendeVESO29 presenterAziendeVESO29 = new PresenterAziendeVESO29();
                ValorizzaAziendeVESO29PerDelete(r.DataItemIndex);

                presenterAziendeVESO29.EliminaAziendeVESO29(this);

                if (this.HasError)
                    RaiseShowAvviso(this, null);
                else
                {
                    this.ErrorMessage = "AziendaVESO29 eliminata correttamente";
                    RaiseShowAvviso(this, null);
                }

                AbilitaFiltro();
                ValorizzaGrigliaAziendeVESO29(false);
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

                PresenterAziendeVESO29 presenterAziendeVESO29 = new PresenterAziendeVESO29();
                ValorizzaAziendeVESO29PerSave(r);
                if (this.HasError)
                    return;

                presenterAziendeVESO29.InserisciAziendeVESO29(this);

                if (this.HasError)
                    RaiseShowAvviso(this, null);
                else
                {
                    this.ErrorMessage = "Azienda VESO29 inserita correttamente";
                    RaiseShowAvviso(this, null);
                    gvAziendeVESO29.EditIndex = -1;
                }

                AbilitaFiltro();
                ValorizzaGrigliaAziendeVESO29(false);
                ViewState[EnumViewState.Filtro.ToString()] = false;

                #endregion salva
            }
            else if (e.CommandName == "Annulla")
            {
                gvAziendeVESO29.EditIndex = -1;
                gvAziendeVESO29_Load();
            }
        }

        protected void gvAziendeVESO29_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    List<AziendeVESO29> elencoAziendeVESO29 = (List<AziendeVESO29>)ViewState[EnumViewState.AziendeVESO29.ToString()];
                    if (e.Row.Cells[0].Controls.Count == 3)
                    {
                        CodeUtility.EnableEditableMode(e.Row.Cells[0], Keys.ValidationGroup_GrigliaAziendeVESO29, Page.Theme);
                    }
                    else
                    {
                        if (e.Row.DataItemIndex == elencoAziendeVESO29.Count - 1)
                        {
                            LinkButton add = ((LinkButton)(e.Row.Cells[0].Controls[0]));
                            add.Text = (string)"<img width=16 height=16 border=0 src=../App_themes/" + Page.Theme + "/Images/add24.png />";
                            add.ToolTip = "Aggiungi";
                            add.OnClientClick = "BlockUI();";
                        }
                        else
                        {
                            CodeUtility.EnableReadableMode(e.Row.Cells[0], e.Row.Cells[4], Page.Theme, Keys.btnDelete_GrigliaAziendeVESO29);
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
                throw new INPS.DNA.DnaApplicationException("UCAziendeVESO29, Errore nel metodo gvAziendeVESO29_RowDataBound " + ex);
            }
        }

        #endregion metodi protected gridview aziende veso29

        #region metodi protected filtro di ricerca

        protected void btnApplicaFiltro_Click(object sender, EventArgs e)
        {
            CaricaDati();
            gvAziendeVESO29.EditIndex = -1;

            ViewState[EnumViewState.Filtro.ToString()] = true;
            FiltraGvAziendeVESO29();

            gvAziendeVESO29_Load();
            DisabilitaFiltro();
            RaiseHideInfo(this, null);
        }

        protected void btnAnnullaFiltro_Click(object sender, EventArgs e)
        {
            CaricaDati();
            gvAziendeVESO29.EditIndex = -1;
            ViewState[EnumViewState.Filtro.ToString()] = false;
            ValorizzaGrigliaAziendeVESO29(false);

            AbilitaFiltro();
            RaiseHideInfo(this, null);
        }

        #endregion metodi protected filtro di ricerca

        #region metodi private gridview aziende veso29

        private void gvAziendeVESO29_Load()
        {
            try
            {
                gvAziendeVESO29.DataSource = (List<AziendeVESO29>)ViewState[EnumViewState.AziendeVESO29.ToString()];
                gvAziendeVESO29.DataBind();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCAziendeVESO29, Errore nel metodo gvAziendeVESO29_Load " + ex);
            }
        }

        private void ValorizzaGrigliaAziendeVESO29(bool vaiAUltimaPagina)
        {
            FormattaElencoAziendeVESO29();

            //// Va all'ultima pagina
            if (vaiAUltimaPagina)
            {
                if ((List<AziendeVESO29>)ViewState[EnumViewState.AziendeVESO29.ToString()] != null)
                    gvAziendeVESO29.PageIndex = ((List<AziendeVESO29>)ViewState[EnumViewState.AziendeVESO29.ToString()]).Count / gvAziendeVESO29.PageSize;
            }
            gvAziendeVESO29_Load();
        }

        /// <summary>
        /// Errore
        /// </summary>
        private void FormattaElencoAziendeVESO29()
        {
            List<AziendeVESO29> elencoAziendeVESO29WS = new List<AziendeVESO29>();
            if (this.AziendeVESO29.elencoAziendeVESO29 == null || this.AziendeVESO29.elencoAziendeVESO29.Count() == 0)
            {
                elencoAziendeVESO29WS.Add(new AziendeVESO29());
            }
            else
            {
                elencoAziendeVESO29WS = this.AziendeVESO29.elencoAziendeVESO29.ToList();
                elencoAziendeVESO29WS.Add(new AziendeVESO29());
            }

            if (elencoAziendeVESO29WS.Count() < 2)
                gvAziendeVESO29.EditIndex = 0;

            ViewState[EnumViewState.AziendeVESO29.ToString()] = elencoAziendeVESO29WS;
        }

        private void ValorizzaAziendeVESO29PerSave(GridViewRow row)
        {
            List<AziendeVESO29> elencoAziende = (List<AziendeVESO29>)ViewState[EnumViewState.AziendeVESO29.ToString()];

            elencoAziende[row.DataItemIndex].CodiceAziendaTraduzioneSuGP = (((TextBox)row.FindControl(Keys.txtCodiceAzienda)).Text);
            elencoAziende[row.DataItemIndex].Descrizione = (((TextBox)row.FindControl(Keys.txtDescrizione)).Text.ToUpperInvariant());
            elencoAziende[row.DataItemIndex].UltimaDecorrenzaAmmessa = Utility.GetDateFromString(((TextBox)row.FindControl(Keys.txtUltimaDecorrenza)).Text);

            ValorizzaAziendeVESO29(elencoAziende, row.DataItemIndex);
        }

        private void ValorizzaAziendeVESO29(List<AziendeVESO29> elencoAziende, int index)
        {
            if (this.AziendeVESO29 == null)

                this.AziendeVESO29 = new AreaAziendeVESO29();
            this.AziendeVESO29.AziendaVESO29 = new AziendeVESO29();

            AziendeVESO29.AziendaVESO29.CodiceAziendaTraduzioneSuGP = elencoAziende[index].CodiceAziendaTraduzioneSuGP;
            AziendeVESO29.AziendaVESO29.Descrizione = elencoAziende[index].Descrizione;
            AziendeVESO29.AziendaVESO29.UltimaDecorrenzaAmmessa = elencoAziende[index].UltimaDecorrenzaAmmessa;
        }

        private void ValorizzaAziendeVESO29PerDelete(int index)
        {
            List<AziendeVESO29> elencoAziendeVESO29 = (List<AziendeVESO29>)ViewState[EnumViewState.AziendeVESO29.ToString()];

            ValorizzaAziendeVESO29(elencoAziendeVESO29, index);
        }

        #endregion metodi private griglia aziende veso29

        #region metodi private filtro di ricerca

        private void FiltraGvAziendeVESO29()
        {
            int count = 0;
            List<AziendeVESO29> elencoAziendeVESO29 = (List<AziendeVESO29>)ViewState[EnumViewState.AziendeVESO29.ToString()];

            if (!string.IsNullOrEmpty(txtFiltroCodiceAzienda.Text.Trim()))
            {
                count++;
                elencoAziendeVESO29 = elencoAziendeVESO29.FindAll(x => (x.CodiceAziendaTraduzioneSuGP != null ? x.CodiceAziendaTraduzioneSuGP.Trim().ToUpperInvariant() : null) == txtFiltroCodiceAzienda.Text.Trim().ToUpperInvariant());
            }

            if (!string.IsNullOrEmpty(txtFiltroDescrizione.Text.Trim()))
            {
                count++;
                elencoAziendeVESO29 = elencoAziendeVESO29.FindAll(x => (x.Descrizione != null ? x.Descrizione.Trim().ToUpperInvariant() : null) == txtFiltroDescrizione.Text.Trim().ToUpperInvariant());
            }

            if (!string.IsNullOrEmpty(txtUltimaDecorrenzaAmmessaDa.Text.Trim()))
            {
                count++;
                elencoAziendeVESO29 = elencoAziendeVESO29.FindAll(x => Utility.DataSuccessivaA(x.UltimaDecorrenzaAmmessa.GetValueOrDefault(),
                    Utility.GetDateFromString(txtUltimaDecorrenzaAmmessaDa.Text).GetValueOrDefault()));
            }

            if (!string.IsNullOrEmpty(txtUltimaDecorrenzaAmmessaA.Text.Trim()))
            {
                count++;
                elencoAziendeVESO29 = elencoAziendeVESO29.FindAll(x => Utility.DataSuccessivaA(Utility.GetDateFromString(txtUltimaDecorrenzaAmmessaA.Text).GetValueOrDefault(),
                    x.UltimaDecorrenzaAmmessa.GetValueOrDefault()));
            }

            if (count > 0)
            {
                ViewState[EnumViewState.AziendeVESO29.ToString()] = elencoAziendeVESO29;
                elencoAziendeVESO29.Add(new AziendeVESO29());

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

            List<AziendeVESO29> elencoAziendeVESO29 = (List<AziendeVESO29>)ViewState[EnumViewState.AziendeVESO29.ToString()];
            if (elencoAziendeVESO29 != null)
            {
                foreach (AziendeVESO29 az in elencoAziendeVESO29)
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
        /// CaricaDati, chiama il CaricaArea del presenter
        /// </summary>
        private void CaricaDati()
        {
            PresenterAziendeVESO29 presenterazVESO29 = new PresenterAziendeVESO29();
            presenterazVESO29.CaricaAreaAziendeVESO29(this);

            if (this.HasError)
            {
                RaiseShowAvviso(this, null);
                return;
            }
        }

        #region Enum

        public enum EnumViewState
        {
            AziendeVESO29,
            Filtro
        }
        #endregion Enum

        #region keys

        private class Keys
        {
            public const string ValidationGroup_GrigliaAziendeVESO29 = "GrigliaAziendeVESO29";
            public const string btnDelete_GrigliaAziendeVESO29 = "btnDelete";
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