using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using INPS.Pensioni.LiquidazionePensione.Presenter;
using INPS.Pensioni.LiquidazionePensione.Presenter.Enum;
using INPS.DNA;

namespace INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.AltreFunzioni.GestioneAbilitazioneTrasformazioni
{
    public partial class UCGestioneAbilTrasf : CustomBaseUserControl, ITrasformazioniAbilitate
    {
        #region IViewUI
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        #endregion IViewUI

        #region ITrasformazioniAbilitate
        public AreaTrasformazioniAbilitate TrasformazioniAbilitate { get; set; }
        public AreaTrasformazioniAbilitate.DatiTrasformazioneAbilitata datiTrasformazioneAbilitata { get; set; }
        public UtilityTipoAppartenenza tipoAppRuolo { get; set; }
        #endregion ITrasformazioniAbilitate

        protected void Page_Load(object sender, EventArgs e)
        {
            this.tipoAppRuolo = (UtilityTipoAppartenenza)Utility.GetTipoAppartenenzaRuolo((Ruoli)Session["Ruolo"]);
            if (!IsPostBack)
            {
                ValorizzaGriglia();
                AbilitaFiltro();
                txtFiltroTipologia.Text = tipoAppRuolo.ToString();
                txtOpTipologia.Text = tipoAppRuolo.ToString();
            }
        }

        protected void gvTrasformazioni_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Delete")
            {
                Control c = (Control)e.CommandSource;
                GridViewRow r = (GridViewRow)c.NamingContainer;

                PresenterTrasformazioniAbilitate presenterTrasformazioniAbilitate = new PresenterTrasformazioniAbilitate();
                ValorizzaTrasformazioneAbilitataPerDelete(r.DataItemIndex);
                presenterTrasformazioniAbilitate.EliminaTrasformazioneAbilitata(this);

                if (this.HasError)
                {
                    RaiseShowAvviso(this, null);
                    return;
                }

                ValorizzaGriglia();
                AbilitaFiltro();
                RaiseShowInfo(this, null);
            }
            else if (e.CommandName == "Edit")
            {
                RaiseHideInfo(this, null);
            }
            else if (e.CommandName == "Salva")
            {
                Control c = (Control)e.CommandSource;
                GridViewRow r = (GridViewRow)c.NamingContainer;

                PresenterTrasformazioniAbilitate presenterTrasformazioniAbilitate = new PresenterTrasformazioniAbilitate();
                ValorizzaTrasformazioneAbilitataPerSave(r);
                if (this.HasError)
                    return;

                presenterTrasformazioniAbilitate.SalvaTrasformazioneAbilitata(this);

                if (this.HasError)
                {
                    RaiseShowAvviso(this, null);
                    return;
                }

                gvTrasformazioni.EditIndex = -1;

                ValorizzaGriglia();
                AbilitaFiltro();
                RaiseShowInfo(this, null);
            }
            else if (e.CommandName == "Cancel")
            {
                GridView r = (GridView)e.CommandSource;
                int index = r.EditIndex + (r.PageIndex * r.PageSize);
                if (index == ((List<TrasfAbilitate>)ViewState[EnumViewState.TrasformazioniAbilitate.ToString()]).Count - 1)
                    ((List<TrasfAbilitate>)ViewState[EnumViewState.TrasformazioniAbilitate.ToString()])[index] = new TrasfAbilitate();

                RaiseHideInfo(this, null);
            }

        }

        protected void gvTrasformazioni_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            try
            {
                //Reset the edit index.
                if (((List<TrasfAbilitate>)ViewState[EnumViewState.TrasformazioniAbilitate.ToString()]).Count() < 2)
                    gvTrasformazioni.EditIndex = 0;
                else
                    gvTrasformazioni.EditIndex = -1;
                //Bind data to the GridView control.
                gvTrasformazioni_Load();

            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCGestioneAbilTrasf, Errore nel metodo gvTrasformazioni_RowCancelingEdit " + ex);
            }

        }

        protected void gvTrasformazioni_RowEditing(object sender, GridViewEditEventArgs e)
        {
            try
            {
                gvTrasformazioni.EditIndex = e.NewEditIndex;
                gvTrasformazioni_Load();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCGestioneAbilTrasf, Errore nel metodo gvTrasformazioni_RowEditing " + ex);
            }
        }

        protected void gvTrasformazioni_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    List<TrasfAbilitate> elencoTrasformazioniAbilitate = (List<TrasfAbilitate>)ViewState[EnumViewState.TrasformazioniAbilitate.ToString()];
                    if (e.Row.Cells[0].Controls.Count == 3)
                    {
                        LinkButton cancel = ((LinkButton)(e.Row.Cells[0].Controls[2]));
                        cancel.Text = (string)"<img width=16 height=16 border=0 src=../App_themes/" + Page.Theme + "/Images/cancel24.png />";
                        cancel.ToolTip = "Annulla";
                        cancel.OnClientClick = "BlockUI();";

                        LinkButton save = ((LinkButton)(e.Row.Cells[0].Controls[0]));
                        save.Text = (string)"<img width=16 height=16 border=0 src=../App_themes/" + Page.Theme + "/Images/save24.png />";
                        save.ToolTip = "Salva";
                        save.CausesValidation = true;
                        save.ValidationGroup = "UCTrasformazioni";
                        save.CommandName = "Salva";
                        save.OnClientClick = "BlockUI();";

                        TextBox txtSiglaCategoria = new TextBox();
                        txtSiglaCategoria = (TextBox)e.Row.FindControl("txtSiglaCategoria");
                        txtSiglaCategoria.Text = ((TrasfAbilitate)e.Row.DataItem).SiglaCategoria;
                        if (!string.IsNullOrEmpty(txtSiglaCategoria.Text))
                            txtSiglaCategoria.Enabled = false;

                        TextBox txtSede = new TextBox();
                        txtSede = (TextBox)e.Row.FindControl("txtSede");
                        txtSede.Text = ((TrasfAbilitate)e.Row.DataItem).Sede;
                        if (!string.IsNullOrEmpty(txtSede.Text))
                            txtSede.Enabled = false;

                        DropDownList ddlTipologia = new DropDownList();
                        ddlTipologia = (DropDownList)e.Row.FindControl("ddlTipologia");
                        LoadDdlTipologia(ddlTipologia);
                        ddlTipologia.SelectedValue = tipoAppRuolo.ToString();
                    }
                    else
                    {
                        if (e.Row.DataItemIndex == elencoTrasformazioniAbilitate.Count - 1)
                        {
                            LinkButton add = ((LinkButton)(e.Row.Cells[0].Controls[0]));
                            add.Text = (string)"<img width=16 height=16 border=0 src=../App_themes/" + Page.Theme + "/Images/add24.png />";
                            add.ToolTip = "Aggiungi";
                            add.OnClientClick = "BlockUI();";
                        }
                        else
                        {
                            LinkButton edit = ((LinkButton)(e.Row.Cells[0].Controls[0]));
                            LinkButton delete = (LinkButton)e.Row.FindControl("btnDelete");
                            int index = e.Row.DataItemIndex;
                            if (index >= 0 && index <= elencoTrasformazioniAbilitate.Count - 2)
                            {
                                edit.Text = (string)"<img width=24 height=24 border=0 src=../App_themes/" + Page.Theme + "/Images/pencil.png />";
                                edit.ToolTip = "Modifica";
                                edit.OnClientClick = "BlockUI();";

                                delete.Text = (string)"<img width=24 height=24 border=0 src=../App_themes/" + Page.Theme + "/Images/delete24.png />";
                                delete.ToolTip = "Elimina";
                                delete.OnClientClick = "BlockUI();";
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
                throw new INPS.DNA.DnaApplicationException("UCGestioneAbilTrasf, Errore nel metodo gvTrasformazioni_RowDataBound " + ex);
            }
        }

        protected void gvTrasformazioni_onPageIndexChanging(Object sender, GridViewPageEventArgs e)
        {
            try
            {
                gvTrasformazioni.EditIndex = -1;
                gvTrasformazioni.PageIndex = e.NewPageIndex;
                gvTrasformazioni_Load();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCGestioneAbilTrasf, Errore nel metodo gvTrasformazioni_onPageIndexChanging" + ex);
            }
        }

        protected void gvTrasformazioni_onRowDeleting(object sender, GridViewDeleteEventArgs e)
        {

        }

        protected void btnApplicaFiltro_Click(object sender, EventArgs e)
        {
            Filtra();
            gvTrasformazioni_Load();
            DisabilitaFiltro();
            RaiseHideInfo(this, null);
        }

        protected void btnAnnullaFiltro_Click(object sender, EventArgs e)
        {
            ValorizzaGriglia();
            PulisciFiltro();
            AbilitaFiltro();
            RaiseHideInfo(this, null);
        }

        protected void btnEseguiOp_Click(object sender, EventArgs e)
        {
            if (ddlOpOperazione.SelectedIndex == 0 ||
                string.IsNullOrEmpty(txtOpSiglaCategoria.Text.Trim()))
            {
                this.HasError = true;
                this.ErrorMessage = "Errore nell'esecuzione dell'operazione su tutte le sedi: parametri mancanti";
                RaiseShowAvviso(this, null);
                return;
            }

            this.datiTrasformazioneAbilitata = new AreaTrasformazioniAbilitate.DatiTrasformazioneAbilitata();
            this.datiTrasformazioneAbilitata.SiglaCategoria = txtOpSiglaCategoria.Text.Trim();
            switch (txtOpTipologia.Text)
            {
                case "FS":
                    this.datiTrasformazioneAbilitata.Tipologia = AreaTrasformazioniAbilitate.DatiTrasformazioneAbilitata.Tipo.FS;
                    break;
                case "CI":
                    this.datiTrasformazioneAbilitata.Tipologia = AreaTrasformazioniAbilitate.DatiTrasformazioneAbilitata.Tipo.CI;
                    break;
                case "AGO":
                    this.datiTrasformazioneAbilitata.Tipologia = AreaTrasformazioniAbilitate.DatiTrasformazioneAbilitata.Tipo.AGO;
                    break;
            }

            PresenterTrasformazioniAbilitate presenterTrasformazioniAbilitate = new PresenterTrasformazioniAbilitate();
            switch (ddlOpOperazione.SelectedValue)
            {
                case "SAVE":
                    presenterTrasformazioniAbilitate.SalvaTrasformazioniAbilitateSuTutteLeSedi(this);
                    break;
                case "DELETE":
                    presenterTrasformazioniAbilitate.EliminaTrasformazioniAbilitateSuTutteLeSedi(this);
                    break;
            }

            if (this.HasError)
            {
                RaiseShowAvviso(this, null);
                return;
            }

            gvTrasformazioni.EditIndex = -1;

            ValorizzaGriglia();
            AbilitaFiltro();
            RaiseShowInfo(this, null);
        }

        #region private methods
        private void ValorizzaGriglia()
        {
            PresenterTrasformazioniAbilitate presenterTrasformazioniAbilitate = new PresenterTrasformazioniAbilitate();
            presenterTrasformazioniAbilitate.CaricaTrasformazioniAbilitate(this);

            if (this.HasError)
            {
                RaiseShowAvviso(this, null);
                return;
            }

            FormattaElencoTrasformazioniAbilitate();
            ViewState[EnumViewState.Tipologie.ToString()] = this.TrasformazioniAbilitate.ElencoTipologie;
            // Va all'ultima pagina
            if ((List<TrasfAbilitate>)ViewState[EnumViewState.TrasformazioniAbilitate.ToString()] != null)
                gvTrasformazioni.PageIndex = ((List<TrasfAbilitate>)ViewState[EnumViewState.TrasformazioniAbilitate.ToString()]).Count / gvTrasformazioni.PageSize;

            gvTrasformazioni_Load();
        }

        private void FormattaElencoTrasformazioniAbilitate()
        {
            List<TrasfAbilitate> elencoTrasfAbilitate = new List<TrasfAbilitate>();
            if (this.TrasformazioniAbilitate.ElencoTrasformazioniAbilitate == null)
            {
                elencoTrasfAbilitate.Add(new TrasfAbilitate());
            }
            else
            {
                foreach (AreaTrasformazioniAbilitate.DatiTrasformazioneAbilitata tra in this.TrasformazioniAbilitate.ElencoTrasformazioniAbilitate)
                {
                    TrasfAbilitate t = new TrasfAbilitate();
                    t.SiglaCategoria = tra.SiglaCategoria;
                    t.Sede = tra.Sede.PadLeft(4, '0').Substring(0, 4);
                    t.Tipologia = tipoAppRuolo.ToString();

                    elencoTrasfAbilitate.Add(t);
                }
                elencoTrasfAbilitate.Add(new TrasfAbilitate());
            }

            if (elencoTrasfAbilitate.Count() < 2)
                gvTrasformazioni.EditIndex = 0;

            ViewState[EnumViewState.TrasformazioniAbilitate.ToString()] = elencoTrasfAbilitate;
        }

        private void gvTrasformazioni_Load()
        {
            try
            {
                gvTrasformazioni.DataSource = (List<TrasfAbilitate>)ViewState[EnumViewState.TrasformazioniAbilitate.ToString()];
                gvTrasformazioni.DataBind();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCGestioneAbilTrasf, Errore nel metodo gvTrasformazioni_Load " + ex);
            }
        }

        private void LoadDdlTipologia(DropDownList ddl)
        {
            try
            {
                foreach (AreaTrasformazioniAbilitate.DatiTrasformazioneAbilitata.Tipo tipo in (AreaTrasformazioniAbilitate.DatiTrasformazioneAbilitata.Tipo[])ViewState[EnumViewState.Tipologie.ToString()])
                {
                    ListItem li = new ListItem();
                    li.Text = tipo.ToString();
                    li.Value = tipo.ToString();
                    ddl.Items.Add(li);
                }
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCGestioneAbilTrasf, Errore nel metodo LoadDdlTipologia " + ex);
            }
        }

        private void ValorizzaTrasformazioneAbilitataPerDelete(int index)
        {
            List<TrasfAbilitate> elencoTrasfAbilitate = (List<TrasfAbilitate>)ViewState[EnumViewState.TrasformazioniAbilitate.ToString()];

            ValorizzaTrasformazioniAbilitate(elencoTrasfAbilitate, index, false);
        }

        private void ValorizzaTrasformazioneAbilitataPerSave(GridViewRow row)
        {
            List<TrasfAbilitate> elencoTrasfAbilitate = (List<TrasfAbilitate>)ViewState[EnumViewState.TrasformazioniAbilitate.ToString()];

            elencoTrasfAbilitate[row.DataItemIndex].SiglaCategoria = ((TextBox)row.Cells[1].Controls[1]).Text;
            elencoTrasfAbilitate[row.DataItemIndex].Sede = ((TextBox)row.Cells[2].Controls[1]).Text;
            elencoTrasfAbilitate[row.DataItemIndex].Tipologia = ((DropDownList)row.Cells[3].Controls[1]).SelectedValue;

            ValorizzaTrasformazioniAbilitate(elencoTrasfAbilitate, row.DataItemIndex, true);

        }

        private void ValorizzaTrasformazioniAbilitate(List<TrasfAbilitate> elencoTrasfAbilitate, int index, bool isSalvataggio)
        {
            this.datiTrasformazioneAbilitata = new AreaTrasformazioniAbilitate.DatiTrasformazioneAbilitata();

            datiTrasformazioneAbilitata.SiglaCategoria = elencoTrasfAbilitate[index].SiglaCategoria;
            if (isSalvataggio)
            {
                Office office = null;
                try
                {
                    office = (from o in INPS.DNA.Context.OfficeList.OfficeFullList
                              where o.AspnCode.PadLeft(4, '0').Substring(0, 4) == elencoTrasfAbilitate[index].Sede.PadLeft(4, '0')
                              select o).First<Office>();
                }
                catch (Exception)
                {
                    this.HasError = true;
                    this.ErrorMessage = "Sede non corretta";
                    RaiseShowAvviso(this, null);
                    return;
                }
            }
            datiTrasformazioneAbilitata.Sede = elencoTrasfAbilitate[index].Sede.PadLeft(4, '0');
            switch (elencoTrasfAbilitate[index].Tipologia)
            {
                case "FS":
                    datiTrasformazioneAbilitata.Tipologia = AreaTrasformazioniAbilitate.DatiTrasformazioneAbilitata.Tipo.FS;
                    break;
                case "CI":
                    datiTrasformazioneAbilitata.Tipologia = AreaTrasformazioniAbilitate.DatiTrasformazioneAbilitata.Tipo.CI;
                    break;
                case "AGO":
                    datiTrasformazioneAbilitata.Tipologia = AreaTrasformazioniAbilitate.DatiTrasformazioneAbilitata.Tipo.AGO;
                    break;
            }
        }

        private void Filtra()
        {
            int count = 0;
            List<TrasfAbilitate> elencoTrasformazioniAbilitate = (List<TrasfAbilitate>)ViewState[EnumViewState.TrasformazioniAbilitate.ToString()];
            if (!string.IsNullOrEmpty(txtFiltroSiglaCategoria.Text.Trim()))
            {
                count++;
                elencoTrasformazioniAbilitate = elencoTrasformazioniAbilitate.FindAll(x => x.SiglaCategoria == txtFiltroSiglaCategoria.Text.ToUpperInvariant());
            }
            if (!string.IsNullOrEmpty(txtFiltroSede.Text.Trim()))
            {
                count++;
                elencoTrasformazioniAbilitate = elencoTrasformazioniAbilitate.FindAll(x => x.Sede == txtFiltroSede.Text);
            }
            if (count > 0)
            {
                ViewState[EnumViewState.TrasformazioniAbilitate.ToString()] = elencoTrasformazioniAbilitate;
                elencoTrasformazioniAbilitate.Add(new TrasfAbilitate());
            }
        }

        private void AbilitaFiltro()
        {
            btnApplicaFiltro.Enabled = true;
            btnAnnullaFiltro.Enabled = false;
            txtFiltroSiglaCategoria.Enabled = true;
            txtFiltroSede.Enabled = true;
        }

        private void DisabilitaFiltro()
        {
            btnApplicaFiltro.Enabled = false;
            btnAnnullaFiltro.Enabled = true;
            txtFiltroSiglaCategoria.Enabled = false;
            txtFiltroSede.Enabled = false;
        }

        /// <summary>
        /// Ripulisce i campi del filtro di ricerca
        /// </summary>
        private void PulisciFiltro()
        {
            txtFiltroSiglaCategoria.Text = string.Empty;
            txtFiltroSede.Text = string.Empty;
        }
        #endregion private methods

        #region nested class
        [Serializable()]
        public class TrasfAbilitate
        {
            internal TrasfAbilitate() { }

            #region private properties
            private string _SiglaCategoria;
            private string _Sede;
            private string _Tipologia;
            private string _Trasformazione;
            private string _TipoAbilitazione;

            #endregion private properties

            #region public properties
            public string SiglaCategoria { get { return _SiglaCategoria; } set { _SiglaCategoria = value; } }
            public string Sede { get { return _Sede; } set { _Sede = value; } }
            public string Tipologia { get { return _Tipologia; } set { _Tipologia = value; } }
            public string Trasformazione { get { return _Trasformazione; } set { _Trasformazione = value; } }
            public string TipoAbilitazione { get { return _TipoAbilitazione; } set { _TipoAbilitazione = value; } }
            #endregion public properties
        }
        #endregion nested class

        #region Events
        public event EventHandler ShowAvviso;
        public event EventHandler ShowInfo;
        public event EventHandler HideInfo;

        protected void RaiseShowAvviso(object sender, EventArgs e)
        {
            ShowAvviso(sender, e);
        }

        protected void RaiseShowInfo(object sender, EventArgs e)
        {
            ShowInfo(sender, e);
        }

        protected void RaiseHideInfo(object sender, EventArgs e)
        {
            HideInfo(sender, e);
        }
        #endregion Events

        public enum EnumViewState
        {
            Tipologie,
            TrasformazioniAbilitate
        }
    }
}