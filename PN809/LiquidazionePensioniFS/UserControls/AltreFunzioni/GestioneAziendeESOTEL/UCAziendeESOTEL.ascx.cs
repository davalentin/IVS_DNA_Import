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

namespace INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.AltreFunzioni.GestioneAziendeESOTEL
{
    public partial class UCAziendeESOTEL : CustomBaseUserControl, IAziendeESOTEL
    {
        #region IViewUI

        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }

        #endregion IViewUI

        #region IAziendeESOTEL

        public AreaAziendeESOTEL AziendeESOTEL { get; set; }
        public string CommaSeparatedDescrizione { get; set; }

        #endregion IAziendeESOTEL

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
                ValorizzaGrigliaAziendeESOTEL(true);

                ValorizzaAutoComplete();
            }

            RaiseHideInfo(this, null);

        }

        #region metodi protected gridview aziende ESOTEL

        protected void gvAziendeESOTEL_onPageIndexChanging(Object sender, GridViewPageEventArgs e)
        {
            try
            {
                gvAziendeESOTEL.EditIndex = -1;
                gvAziendeESOTEL.PageIndex = e.NewPageIndex;
                gvAziendeESOTEL_Load();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCAziendeESOTEL, Errore nel metodo gvAziendeESOTEL_onPageIndexChanging" + ex);
            }
        }

        protected void gvAziendeESOTEL_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            try
            {
                //Reset the edit index.
                if (((List<AziendeESOTEL>)ViewState[EnumViewState.AziendeESOTEL.ToString()]).Count() < 2)
                    gvAziendeESOTEL.EditIndex = 0;
                else
                    gvAziendeESOTEL.EditIndex = -1;
                //Bind data to the GridView control.
                gvAziendeESOTEL_Load();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCAziendeESOTEL, Errore nel metodo gvAziendeESOTEL_RowCancelingEdit " + ex);
            }
        }

        protected void gvAziendeESOTEL_RowEditing(object sender, GridViewEditEventArgs e)
        {
            try
            {
                gvAziendeESOTEL.EditIndex = e.NewEditIndex;
                gvAziendeESOTEL_Load();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCAziendeESOTEL, Errore nel metodo gvAziendeESOTEL_RowEditing " + ex);
            }
        }

        protected void gvAziendeESOTEL_RowCommand(object sender, GridViewCommandEventArgs e)
        {

            if (e.CommandName == "Elimina")
            {
                #region delete

                Control c = (Control)e.CommandSource;
                GridViewRow r = (GridViewRow)c.NamingContainer;

                PresenterAziendeESOTEL presenterAziendeESOTEL = new PresenterAziendeESOTEL();
                ValorizzaAziendeESOTELPerDelete(r.DataItemIndex);

                presenterAziendeESOTEL.EliminaAziendeESOTEL(this);

                if (this.HasError)
                    RaiseShowAvviso(this, null);
                else
                {
                    this.ErrorMessage = "AziendaESOTEL eliminata correttamente";
                    RaiseShowAvviso(this, null);
                }

                AbilitaFiltro();
                ValorizzaGrigliaAziendeESOTEL(false);
                ViewState[EnumViewState.Filtro.ToString()] = false;

                #endregion delete
            }

            else if (e.CommandName == "Salva")
            {
                #region salva

                Control c = (Control)e.CommandSource;
                GridViewRow r = (GridViewRow)c.NamingContainer;

                PresenterAziendeESOTEL presenterAziendeESOTEL = new PresenterAziendeESOTEL();
                ValorizzaAziendeESOTELPerSave(r);
                if (this.HasError)
                    return;

                presenterAziendeESOTEL.InserisciAziendeESOTEL(this);

                if (this.HasError)
                    RaiseShowAvviso(this, null);
                else
                {
                    this.ErrorMessage = "Azienda ESOTEL inserita correttamente";
                    RaiseShowAvviso(this, null);
                    gvAziendeESOTEL.EditIndex = -1;
                }

                AbilitaFiltro();
                ValorizzaGrigliaAziendeESOTEL(false);
                ViewState[EnumViewState.Filtro.ToString()] = false;

                #endregion salva
            }
            else if (e.CommandName == "Annulla")
            {
                gvAziendeESOTEL.EditIndex = -1;
                gvAziendeESOTEL_Load();
            }
        }

        protected void gvAziendeESOTEL_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    List<AziendeESOTEL> elencoAziendeESOTEL = (List<AziendeESOTEL>)ViewState[EnumViewState.AziendeESOTEL.ToString()];
                    if (e.Row.Cells[0].Controls.Count == 3)
                    {
                        CodeUtility.EnableEditableMode(e.Row.Cells[0], Keys.ValidationGroup_GrigliaAziendeESOTEL, Page.Theme);
                    }
                    else
                    {
                        if (e.Row.DataItemIndex == elencoAziendeESOTEL.Count - 1)
                        {
                            LinkButton add = ((LinkButton)(e.Row.Cells[0].Controls[0]));
                            add.Text = (string)"<img width=16 height=16 border=0 src=../App_themes/" + Page.Theme + "/Images/add24.png />";
                            add.ToolTip = "Aggiungi";
                            add.OnClientClick = "BlockUI();";
                        }
                        else
                        {
                            CodeUtility.EnableReadableMode(e.Row.Cells[0], e.Row.Cells[4], Page.Theme, Keys.btnDelete_GrigliaAziendeESOTEL);
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
                throw new INPS.DNA.DnaApplicationException("UCAziendeESOTEL, Errore nel metodo gvAziendeESOTEL_RowDataBound " + ex);
            }
        }

        #endregion metodi protected gridview aziende ESOTEL

        #region metodi protected filtro di ricerca

        protected void btnApplicaFiltro_Click(object sender, EventArgs e)
        {
            CaricaDati();
            gvAziendeESOTEL.EditIndex = -1;

            ViewState[EnumViewState.Filtro.ToString()] = true;
            FiltraGvAziendeESOTEL();

            gvAziendeESOTEL_Load();
            DisabilitaFiltro();
            RaiseHideInfo(this, null);
        }

        protected void btnAnnullaFiltro_Click(object sender, EventArgs e)
        {
            CaricaDati();
            gvAziendeESOTEL.EditIndex = -1;
            ViewState[EnumViewState.Filtro.ToString()] = false;
            ValorizzaGrigliaAziendeESOTEL(false);

            AbilitaFiltro();
            RaiseHideInfo(this, null);
        }

        #endregion metodi protected filtro di ricerca

        #region metodi private gridview aziende ESOTEL

        private void gvAziendeESOTEL_Load()
        {
            try
            {
                gvAziendeESOTEL.DataSource = (List<AziendeESOTEL>)ViewState[EnumViewState.AziendeESOTEL.ToString()];
                gvAziendeESOTEL.DataBind();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCAziendeESOTEL, Errore nel metodo gvAziendeESOTEL_Load " + ex);
            }
        }

        private void ValorizzaGrigliaAziendeESOTEL(bool vaiAUltimaPagina)
        {
            FormattaElencoAziendeESOTEL();

            //// Va all'ultima pagina
            if (vaiAUltimaPagina)
            {
                if ((List<AziendeESOTEL>)ViewState[EnumViewState.AziendeESOTEL.ToString()] != null)
                    gvAziendeESOTEL.PageIndex = ((List<AziendeESOTEL>)ViewState[EnumViewState.AziendeESOTEL.ToString()]).Count / gvAziendeESOTEL.PageSize;
            }
            gvAziendeESOTEL_Load();
        }

        /// <summary>
        /// Errore
        /// </summary>
        private void FormattaElencoAziendeESOTEL()
        {
            List<AziendeESOTEL> elencoAziendeESOTELWS = new List<AziendeESOTEL>();
            if (this.AziendeESOTEL.elencoAziendeESOTEL == null || this.AziendeESOTEL.elencoAziendeESOTEL.Count() == 0)
            {
                elencoAziendeESOTELWS.Add(new AziendeESOTEL());
            }
            else
            {
                elencoAziendeESOTELWS = this.AziendeESOTEL.elencoAziendeESOTEL.ToList();
                elencoAziendeESOTELWS.Add(new AziendeESOTEL());
            }

            if (elencoAziendeESOTELWS.Count() < 2)
                gvAziendeESOTEL.EditIndex = 0;

            ViewState[EnumViewState.AziendeESOTEL.ToString()] = elencoAziendeESOTELWS;
        }

        private void ValorizzaAziendeESOTELPerSave(GridViewRow row)
        {
            List<AziendeESOTEL> elencoAziende = (List<AziendeESOTEL>)ViewState[EnumViewState.AziendeESOTEL.ToString()];

            elencoAziende[row.DataItemIndex].CodiceAziendaTraduzioneSuGP = (((TextBox)row.FindControl(Keys.txtCodiceAzienda)).Text);
            elencoAziende[row.DataItemIndex].Descrizione = (((TextBox)row.FindControl(Keys.txtDescrizione)).Text.ToUpperInvariant());
            elencoAziende[row.DataItemIndex].UltimaDecorrenzaAmmessa = Utility.GetDateFromString(((TextBox)row.FindControl(Keys.txtUltimaDecorrenza)).Text);

            ValorizzaAziendeESOTEL(elencoAziende, row.DataItemIndex);
        }

        private void ValorizzaAziendeESOTEL(List<AziendeESOTEL> elencoAziende, int index)
        {
            if (this.AziendeESOTEL == null)

                this.AziendeESOTEL = new AreaAziendeESOTEL();
            this.AziendeESOTEL.AziendaESOTEL = new AziendeESOTEL();

            AziendeESOTEL.AziendaESOTEL.CodiceAziendaTraduzioneSuGP = elencoAziende[index].CodiceAziendaTraduzioneSuGP;
            AziendeESOTEL.AziendaESOTEL.Descrizione = elencoAziende[index].Descrizione;
            AziendeESOTEL.AziendaESOTEL.UltimaDecorrenzaAmmessa = elencoAziende[index].UltimaDecorrenzaAmmessa;
        }

        private void ValorizzaAziendeESOTELPerDelete(int index)
        {
            List<AziendeESOTEL> elencoAziendeESOTEL = (List<AziendeESOTEL>)ViewState[EnumViewState.AziendeESOTEL.ToString()];

            ValorizzaAziendeESOTEL(elencoAziendeESOTEL, index);
        }

        #endregion metodi private griglia aziende ESOTEL

        #region metodi private filtro di ricerca

        private void FiltraGvAziendeESOTEL()
        {
            int count = 0;
            List<AziendeESOTEL> elencoAziendeESOTEL = (List<AziendeESOTEL>)ViewState[EnumViewState.AziendeESOTEL.ToString()];

            if (!string.IsNullOrEmpty(txtFiltroCodiceAzienda.Text.Trim()))
            {
                count++;
                elencoAziendeESOTEL = elencoAziendeESOTEL.FindAll(x => (x.CodiceAziendaTraduzioneSuGP != null ? x.CodiceAziendaTraduzioneSuGP.Trim().ToUpperInvariant() : null) == txtFiltroCodiceAzienda.Text.Trim().ToUpperInvariant());
            }

            if (!string.IsNullOrEmpty(txtFiltroDescrizione.Text.Trim()))
            {
                count++;
                elencoAziendeESOTEL = elencoAziendeESOTEL.FindAll(x => (x.Descrizione != null ? x.Descrizione.Trim().ToUpperInvariant() : null) == txtFiltroDescrizione.Text.Trim().ToUpperInvariant());
            }

            if (!string.IsNullOrEmpty(txtUltimaDecorrenzaAmmessaDa.Text.Trim()))
            {
                count++;
                elencoAziendeESOTEL = elencoAziendeESOTEL.FindAll(x => Utility.DataSuccessivaA(x.UltimaDecorrenzaAmmessa.GetValueOrDefault(),
                    Utility.GetDateFromString(txtUltimaDecorrenzaAmmessaDa.Text).GetValueOrDefault()));
            }

            if (!string.IsNullOrEmpty(txtUltimaDecorrenzaAmmessaA.Text.Trim()))
            {
                count++;
                elencoAziendeESOTEL = elencoAziendeESOTEL.FindAll(x => Utility.DataSuccessivaA(Utility.GetDateFromString(txtUltimaDecorrenzaAmmessaA.Text).GetValueOrDefault(),
                    x.UltimaDecorrenzaAmmessa.GetValueOrDefault()));
            }

            if (count > 0)
            {
                ViewState[EnumViewState.AziendeESOTEL.ToString()] = elencoAziendeESOTEL;
                elencoAziendeESOTEL.Add(new AziendeESOTEL());

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

            List<AziendeESOTEL> elencoAziendeESOTEL = (List<AziendeESOTEL>)ViewState[EnumViewState.AziendeESOTEL.ToString()];
            if (elencoAziendeESOTEL != null)
            {
                foreach (AziendeESOTEL az in elencoAziendeESOTEL)
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
            PresenterAziendeESOTEL presenterazESOTEL = new PresenterAziendeESOTEL();
            presenterazESOTEL.CaricaAreaAziendeESOTEL(this);

            if (this.HasError)
            {
                RaiseShowAvviso(this, null);
                return;
            }
        }

        #region Enum

        public enum EnumViewState
        {
            AziendeESOTEL,
            Filtro
        }
        #endregion Enum

        #region keys

        private class Keys
        {
            public const string ValidationGroup_GrigliaAziendeESOTEL = "GrigliaAziendeESOTEL";
            public const string btnDelete_GrigliaAziendeESOTEL = "btnDelete";
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