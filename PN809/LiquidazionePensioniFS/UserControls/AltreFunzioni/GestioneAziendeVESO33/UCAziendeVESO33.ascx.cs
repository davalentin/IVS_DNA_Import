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

namespace INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.AltreFunzioni.GestioneAziendeVESO33
{
    public partial class UCAziendeVESO33 : CustomBaseUserControl, IAziendeVESO33
    {
        #region IViewUI

        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }

        #endregion IViewUI

        #region IAziendeVESO33

        public AreaAziendeVESO33 AziendeVESO33 { get; set; }
        public string CommaSeparatedDescrizione { get; set; }

        #endregion IAziendeVESO33

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
                ValorizzaGrigliaAziendeVESO33(true);

                ValorizzaAutoComplete();
            }

            RaiseHideInfo(this, null);

        }

        #region metodi protected gridview aziende veso33

        protected void gvAziendeVESO33_onPageIndexChanging(Object sender, GridViewPageEventArgs e)
        {
            try
            {
                gvAziendeVESO33.EditIndex = -1;
                gvAziendeVESO33.PageIndex = e.NewPageIndex;
                gvAziendeVESO33_Load();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCAziendeVESO33, Errore nel metodo gvAziendeVESO33_onPageIndexChanging" + ex);
            }
        }

        protected void gvAziendeVESO33_onRowDeleting(object sender, GridViewDeleteEventArgs e)
        {

        }

        protected void gvAziendeVESO33_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            try
            {
                //Reset the edit index.
                if (((List<AziendeVESO33>)ViewState[EnumViewState.AziendeVESO33.ToString()]).Count() < 2)
                    gvAziendeVESO33.EditIndex = 0;
                else
                    gvAziendeVESO33.EditIndex = -1;
                //Bind data to the GridView control.
                gvAziendeVESO33_Load();

            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCAziendeVESO33, Errore nel metodo gvAziendeVESO33_RowCancelingEdit " + ex);
            }
        }

        protected void gvAziendeVESO33_RowEditing(object sender, GridViewEditEventArgs e)
        {
            try
            {
                gvAziendeVESO33.EditIndex = e.NewEditIndex;
                gvAziendeVESO33_Load();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCAziendeVESO33, Errore nel metodo gvAziendeVESO33_RowEditing " + ex);
            }
        }

        protected void gvAziendeVESO33_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            
            if (e.CommandName == "Elimina")
            {
                #region delete

                Control c = (Control)e.CommandSource;
                GridViewRow r = (GridViewRow)c.NamingContainer;

                PresenterAziendeVESO33 presenterAziendeVESO33 = new PresenterAziendeVESO33();
                ValorizzaAziendeVESO33PerDelete(r.DataItemIndex);
                
                presenterAziendeVESO33.EliminaAziendeVESO33(this);

                if (this.HasError)
                {
                    RaiseShowAvviso(this, null);
       
                }
                else
                {
                    this.ErrorMessage = "AziendaVESO33 eliminata correttamente";
                    RaiseShowAvviso(this, null);
                }

                AbilitaFiltro();
                ValorizzaGrigliaAziendeVESO33(false);
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



                PresenterAziendeVESO33 presenterAziendeVESO33 = new PresenterAziendeVESO33();
                ValorizzaAziendeVESO33PerSave(r);
                if (this.HasError)
                    return;

                presenterAziendeVESO33.InserisciAziendeVESO33(this);

                if (this.HasError)
                {
                    RaiseShowAvviso(this, null);
                  
                }
                else
                {
                    this.ErrorMessage = "Azienda VESO33 inserita correttamente";
                    RaiseShowAvviso(this, null);
                    gvAziendeVESO33.EditIndex = -1;
                }

                AbilitaFiltro();
                ValorizzaGrigliaAziendeVESO33(false);
                ViewState[EnumViewState.Filtro.ToString()] = false;

                #endregion salva
            }

            else if (e.CommandName == "Annulla")
            {
                gvAziendeVESO33.EditIndex = -1;
                gvAziendeVESO33_Load();
            }
        }

        protected void gvAziendeVESO33_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    List<AziendeVESO33> elencoAziendeVESO33 = (List<AziendeVESO33>)ViewState[EnumViewState.AziendeVESO33.ToString()];
                    if (e.Row.Cells[0].Controls.Count == 3)
                    {
                        CodeUtility.EnableEditableMode(e.Row.Cells[0], Keys.ValidationGroup_GrigliaAziendeVESO33, Page.Theme);
                    }
                    else
                    {
                        if (e.Row.DataItemIndex == elencoAziendeVESO33.Count - 1)
                        {
                            LinkButton add = ((LinkButton)(e.Row.Cells[0].Controls[0]));
                            add.Text = (string)"<img width=16 height=16 border=0 src=../App_themes/" + Page.Theme + "/Images/add24.png />";
                            add.ToolTip = "Aggiungi";
                            add.OnClientClick = "BlockUI();";
                        }
                        else
                        {
                            CodeUtility.EnableReadableMode(e.Row.Cells[0], e.Row.Cells[4], Page.Theme, Keys.btnDelete_GrigliaAziendeVESO33);
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
                throw new INPS.DNA.DnaApplicationException("UCAziendeVESO33, Errore nel metodo gvAziendeVESO33_RowDataBound " + ex);
            }
        }

        #endregion metodi protected gridview aziende veso33

        #region metodi protected filtro di ricerca

        protected void btnApplicaFiltro_Click(object sender, EventArgs e)
        {
            CaricaDati();
            gvAziendeVESO33.EditIndex = -1;

            ViewState[EnumViewState.Filtro.ToString()] = true;
            FiltraGvAziendeVESO33();
            
            gvAziendeVESO33_Load();
            DisabilitaFiltro();
            RaiseHideInfo(this, null);
        }

        protected void btnAnnullaFiltro_Click(object sender, EventArgs e)
        {
            CaricaDati();
            gvAziendeVESO33.EditIndex = -1;
            ViewState[EnumViewState.Filtro.ToString()] = false;
            ValorizzaGrigliaAziendeVESO33(false);

            AbilitaFiltro();
            RaiseHideInfo(this, null);
        }

        #endregion metodi protected filtro di ricerca

        #region metodi private gridview aziende veso33

        private void gvAziendeVESO33_Load()
        {
            try
            {
                gvAziendeVESO33.DataSource = (List<AziendeVESO33>)ViewState[EnumViewState.AziendeVESO33.ToString()];
                gvAziendeVESO33.DataBind();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCAziendeVESO33, Errore nel metodo gvAziendeVESO33_Load " + ex);
            }
        }

        private void ValorizzaGrigliaAziendeVESO33(bool vaiAUltimaPagina)
        {
            FormattaElencoAziendeVESO33();

            //// Va all'ultima pagina
            if (vaiAUltimaPagina)
            {

                if ((List<AziendeVESO33>)ViewState[EnumViewState.AziendeVESO33.ToString()] != null)
                    gvAziendeVESO33.PageIndex = ((List<AziendeVESO33>)ViewState[EnumViewState.AziendeVESO33.ToString()]).Count / gvAziendeVESO33.PageSize;
            }
                gvAziendeVESO33_Load();
            
        }

        /// <summary>
        /// Errore
        /// </summary>
        private void FormattaElencoAziendeVESO33()
        {
            List<AziendeVESO33> elencoAziendeVESO33WS = new List<AziendeVESO33>();
            if (this.AziendeVESO33.elencoAziendeVESO33 == null || this.AziendeVESO33.elencoAziendeVESO33.Count() == 0)
            {
                elencoAziendeVESO33WS.Add(new AziendeVESO33());
            }
            else
            {
                elencoAziendeVESO33WS = this.AziendeVESO33.elencoAziendeVESO33.ToList();
                elencoAziendeVESO33WS.Add(new AziendeVESO33());
            }

            if (elencoAziendeVESO33WS.Count() < 2)

                gvAziendeVESO33.EditIndex = 0;

            ViewState[EnumViewState.AziendeVESO33.ToString()] = elencoAziendeVESO33WS;
        }

        private void ValorizzaAziendeVESO33PerSave(GridViewRow row)
        {
            List<AziendeVESO33> elencoAziende = (List<AziendeVESO33>)ViewState[EnumViewState.AziendeVESO33.ToString()];

            elencoAziende[row.DataItemIndex].CodiceAziendaTraduzioneSuGP = (((TextBox)row.FindControl(Keys.txtCodiceAzienda)).Text);
            elencoAziende[row.DataItemIndex].Descrizione = (((TextBox)row.FindControl(Keys.txtDescrizione)).Text.ToUpperInvariant());
            elencoAziende[row.DataItemIndex].UltimaDecorrenzaAmmessa = Utility.GetDateFromString(((TextBox)row.FindControl(Keys.txtUltimaDecorrenza)).Text);

            ValorizzaAziendeVESO33(elencoAziende, row.DataItemIndex);
        }

        private void ValorizzaAziendeVESO33(List<AziendeVESO33> elencoAziende, int index)
        {
            if (this.AziendeVESO33 == null)

                this.AziendeVESO33 = new AreaAziendeVESO33();
            this.AziendeVESO33.AziendaVESO33 = new AziendeVESO33();

            AziendeVESO33.AziendaVESO33.CodiceAziendaTraduzioneSuGP = elencoAziende[index].CodiceAziendaTraduzioneSuGP;
            AziendeVESO33.AziendaVESO33.Descrizione = elencoAziende[index].Descrizione;
            AziendeVESO33.AziendaVESO33.UltimaDecorrenzaAmmessa = elencoAziende[index].UltimaDecorrenzaAmmessa;
        }

        private void ValorizzaAziendeVESO33PerDelete(int index)
        {
            List<AziendeVESO33> elencoAziendeVESO33 = (List<AziendeVESO33>)ViewState[EnumViewState.AziendeVESO33.ToString()];

            ValorizzaAziendeVESO33(elencoAziendeVESO33, index);
        }

        #endregion metodi private griglia aziende veso33

        #region metodi private filtro di ricerca

        private void FiltraGvAziendeVESO33()
        {
            int count = 0;
            List<AziendeVESO33> elencoAziendeVESO33 = (List<AziendeVESO33>)ViewState[EnumViewState.AziendeVESO33.ToString()];

            if (!string.IsNullOrEmpty(txtFiltroCodiceAzienda.Text.Trim()))
            {
                count++;
                elencoAziendeVESO33 = elencoAziendeVESO33.FindAll(x => (x.CodiceAziendaTraduzioneSuGP != null ? x.CodiceAziendaTraduzioneSuGP.Trim().ToUpperInvariant() : null) == txtFiltroCodiceAzienda.Text.Trim().ToUpperInvariant());
            }

            if (!string.IsNullOrEmpty(txtFiltroDescrizione.Text.Trim()))
            {
                count++;
                elencoAziendeVESO33 = elencoAziendeVESO33.FindAll(x => (x.Descrizione != null ? x.Descrizione.Trim().ToUpperInvariant() : null) == txtFiltroDescrizione.Text.Trim().ToUpperInvariant());
            }

            if (!string.IsNullOrEmpty(txtUltimaDecorrenzaAmmessaDa.Text.Trim()))
            {
                count++;
                elencoAziendeVESO33 = elencoAziendeVESO33.FindAll(x => Utility.DataSuccessivaA(x.UltimaDecorrenzaAmmessa.GetValueOrDefault(), 
                    Utility.GetDateFromString(txtUltimaDecorrenzaAmmessaDa.Text).GetValueOrDefault()));
            }

            if (!string.IsNullOrEmpty(txtUltimaDecorrenzaAmmessaA.Text.Trim()))
            {
                count++;
                elencoAziendeVESO33 = elencoAziendeVESO33.FindAll(x => Utility.DataSuccessivaA(Utility.GetDateFromString(txtUltimaDecorrenzaAmmessaA.Text).GetValueOrDefault(), 
                    x.UltimaDecorrenzaAmmessa.GetValueOrDefault()));
            }

            if (count > 0)
            {
                ViewState[EnumViewState.AziendeVESO33.ToString()] = elencoAziendeVESO33;
                elencoAziendeVESO33.Add(new AziendeVESO33());

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

            List<AziendeVESO33> elencoAziendeVESO33 = (List<AziendeVESO33>)ViewState[EnumViewState.AziendeVESO33.ToString()];
            if (elencoAziendeVESO33 != null)
            {
                foreach (AziendeVESO33 az in elencoAziendeVESO33)
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
            PresenterAziendeVESO33 presenterazVESO33 = new PresenterAziendeVESO33();
            presenterazVESO33.CaricaAreaAziendeVESO33(this);

            if (this.HasError)
            {
                RaiseShowAvviso(this, null);
                return;
            }
        }

        #region Enum

        public enum EnumViewState
        {
            AziendeVESO33,
            Filtro
        }
        #endregion Enum

        #region keys

        private class Keys
        {
            public const string ValidationGroup_GrigliaAziendeVESO33 = "GrigliaAziendeVESO33";
            public const string btnDelete_GrigliaAziendeVESO33 = "btnDelete";
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