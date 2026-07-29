using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using INPS.Pensioni.LiquidazionePensione.Presenter;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using INPS.Pensioni.LiquidazionePensione.Presenter.Enum;
using INPS.DNA;

namespace INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.AltreFunzioni.GestioneLiquidazioni
{
    public partial class UCLiquidazioni : CustomBaseUserControl, ILiquidazioniAbilitate
    {
        #region ILiquidazioniAbilitate
        public AreaLiquidazioniAbilitate LiquidazioniAbilitate { get; set; }
        public AreaLiquidazioniAbilitate.DatiLiquidazioneAbilitata datiLiquidazioneAbilitata { get; set; }
        public UtilityTipoAppartenenza tipoAppRuolo { get; set; }
        #endregion ILiquidazioniAbilitate

        #region IViewUI
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        #endregion IViewUI

        protected void Page_Load(object sender, EventArgs e)
        {
            this.tipoAppRuolo = (UtilityTipoAppartenenza)Utility.GetTipoAppartenenzaRuolo((Ruoli)Session["Ruolo"]);
            if (!IsPostBack)
            {
                trLiquidazioniFSPT_INPDAP.Visible = this.tipoAppRuolo == UtilityTipoAppartenenza.FS;
                ValorizzaGriglia();
                AbilitaFiltro();
                //LoadDdlTipologia(ddlFiltroTipologia);
                txtFiltroTipologia.Text = tipoAppRuolo.ToString();
                LoadDdlSINO(ddlFiltroRicostituzione);
                LoadDdlOpOperazione(ddlOpOperazione);
                //LoadDdlTipologia(ddlOpTipologia);
                txtOpTipologia.Text = tipoAppRuolo.ToString();
                LoadDdlSINO(ddlOpRicostituzione);
            }
        }

        private void ValorizzaGriglia()
        {
            PresenterLiquidazioniAbilitate presenterLiquidazioniAbilitate = new PresenterLiquidazioniAbilitate();
            presenterLiquidazioniAbilitate.CaricaLiquidazioniAbilitate(this);
            if (this.HasError)
            {
                RaiseShowAvviso(this, null);
                return;
            }
            ViewState["SigleCategorieINPDAP"] = this.LiquidazioniAbilitate.ElencoSigleCategorieINPDAP.ToList();
            FormattaElencoLiquidazioniAbilitate();
            ViewState["Tipologie"] = this.LiquidazioniAbilitate.ElencoTipologie;
            GriglieLiquidazioni_Load();
        }

        private void FormattaElencoLiquidazioniAbilitate()
        {
            List<LiqAbilitate> elencoLiqAbilitateApp = new List<LiqAbilitate>();
            List<LiqAbilitate> elencoLiquidazioniAbilitate = new List<LiqAbilitate>();
            List<LiqAbilitate> elencoLiquidazioniAbilitateFSPT_INPDAP = new List<LiqAbilitate>();

            if (this.LiquidazioniAbilitate.ElencoLiquidazioniAbilitate != null)
            {
                foreach (AreaLiquidazioniAbilitate.DatiLiquidazioneAbilitata la in this.LiquidazioniAbilitate.ElencoLiquidazioniAbilitate)
                {
                    LiqAbilitate l = new LiqAbilitate();
                    l.SiglaCategoria = la.SiglaCategoria;
                    l.Sede = la.Sede.PadLeft(4, '0').Substring(0, 4);
                    l.Tipologia = tipoAppRuolo.ToString();
                    l.Ricostituzione = la.Ricostituzione ? "SI" : "NO";
                    l.ManualiAbilitate = la.AbilitazioneManuale ? "SI" : "NO";
                    l.RicostituzioneDaAutomatica = la.RicostituzioneDaAutomatica.HasValue ? (la.RicostituzioneDaAutomatica.GetValueOrDefault() ? "SI" : "NO") : string.Empty;
                    l.AutomaticheAbilitate = la.AbilitazioneAutomatica.HasValue ? (la.AbilitazioneAutomatica.GetValueOrDefault() ? "SI" : "NO") : string.Empty;
                    elencoLiqAbilitateApp.Add(l);
                }
            }
            elencoLiqAbilitateApp.Add(new LiqAbilitate());

            if (this.tipoAppRuolo == UtilityTipoAppartenenza.FS)
            {
                elencoLiquidazioniAbilitate = elencoLiqAbilitateApp.FindAll(x => CodeUtility.GetTipoFondoByCategoria(this.tipoAppRuolo, x.SiglaCategoria) != CodeUtility.TipoFondo.FS && 
                    CodeUtility.GetTipoFondoByCategoria(this.tipoAppRuolo, x.SiglaCategoria) != CodeUtility.TipoFondo.PT &&
                    !IsCategoriaINPDAP(x.SiglaCategoria));
                elencoLiquidazioniAbilitateFSPT_INPDAP = elencoLiqAbilitateApp.FindAll(x => CodeUtility.GetTipoFondoByCategoria(this.tipoAppRuolo, x.SiglaCategoria) == CodeUtility.TipoFondo.FS || 
                    CodeUtility.GetTipoFondoByCategoria(this.tipoAppRuolo, x.SiglaCategoria) == CodeUtility.TipoFondo.PT ||
                    IsCategoriaINPDAP(x.SiglaCategoria));
                elencoLiquidazioniAbilitateFSPT_INPDAP.Add(new LiqAbilitate());
            }
            else
                elencoLiquidazioniAbilitate = elencoLiqAbilitateApp;


            // Va all'ultima pagina
            gvLiquidazioni.PageIndex = elencoLiquidazioniAbilitate.Count / gvLiquidazioni.PageSize;
            if (elencoLiquidazioniAbilitate.Count < 2)
                gvLiquidazioni.EditIndex = 0;

            if (elencoLiquidazioniAbilitateFSPT_INPDAP != null)
            {
                // Va all'ultima pagina
                gvLiquidazioniFSPT_INPDAP.PageIndex = elencoLiquidazioniAbilitateFSPT_INPDAP.Count / gvLiquidazioniFSPT_INPDAP.PageSize;
                if (elencoLiquidazioniAbilitateFSPT_INPDAP.Count < 2)
                    gvLiquidazioniFSPT_INPDAP.EditIndex = 0;
            }

            ViewState["LiquidazioniAbilitate"] = elencoLiquidazioniAbilitate;
            ViewState["LiquidazioniAbilitateFSPT_INPDAP"] = elencoLiquidazioniAbilitateFSPT_INPDAP;
        }

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

        public event EventHandler ShowAvviso;

        public event EventHandler ShowInfo;

        public event EventHandler HideInfo;

        private void GriglieLiquidazioni_Load()
        {
            try
            {
                List<LiqAbilitate> elencoLiquidazioniAbilitate = (List<LiqAbilitate>)ViewState["LiquidazioniAbilitate"];
                List<LiqAbilitate> elencoLiquidazioniAbilitateFSPT_INPDAP = (List<LiqAbilitate>)ViewState["LiquidazioniAbilitateFSPT_INPDAP"];

                gvLiquidazioni.DataSource = elencoLiquidazioniAbilitate;
                gvLiquidazioni.DataBind();

                if (this.tipoAppRuolo == UtilityTipoAppartenenza.FS)
                {
                    trLiquidazioniFSPT_INPDAP.Visible = true;
                    if (elencoLiquidazioniAbilitateFSPT_INPDAP != null)
                    {
                        gvLiquidazioniFSPT_INPDAP.DataSource = elencoLiquidazioniAbilitateFSPT_INPDAP;
                        gvLiquidazioniFSPT_INPDAP.DataBind();
                    }
                }
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCLiquidazioni, Errore nel metodo GriglieLiquidazioni_Load " + ex);
            }
        }

        #region gvLiquidazioni
        protected void gvLiquidazioni_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            List<LiqAbilitate> elencoLiquidazioniAbilitate = (List<LiqAbilitate>)ViewState["LiquidazioniAbilitate"];
            if (e.CommandName == "Delete")
            {
                Control c = (Control)e.CommandSource;
                GridViewRow r = (GridViewRow)c.NamingContainer;

                PresenterLiquidazioniAbilitate presenterLiquidazioniAbilitate = new PresenterLiquidazioniAbilitate();
                ValorizzaLiquidazioneAbilitataPerDelete(r.DataItemIndex, elencoLiquidazioniAbilitate);
                presenterLiquidazioniAbilitate.EliminaLiquidazioneAbilitata(this);

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

                PresenterLiquidazioniAbilitate presenterLiquidazioniAbilitate = new PresenterLiquidazioniAbilitate();
                ValorizzaLiquidazioneAbilitataPerSave(r, elencoLiquidazioniAbilitate, false);
                if (this.HasError)
                    return;

                presenterLiquidazioniAbilitate.SalvaLiquidazioneAbilitata(this);

                if (this.HasError)
                {
                    RaiseShowAvviso(this, null);
                    return;
                }

                gvLiquidazioni.EditIndex = -1;

                ValorizzaGriglia();
                AbilitaFiltro();
                RaiseShowInfo(this, null);
            }
            else if (e.CommandName == "Cancel")
            {
                GridView r = (GridView)e.CommandSource;
                int index = r.EditIndex + (r.PageIndex * r.PageSize);
                if (index == elencoLiquidazioniAbilitate.Count - 1)
                    elencoLiquidazioniAbilitate[index] = new LiqAbilitate();
                RaiseHideInfo(this, null);
            }

        }

        protected void gvLiquidazioni_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            try
            {
                //Reset the edit index.
                if (((List<LiqAbilitate>)ViewState["LiquidazioniAbilitate"]).Count() < 2)
                    gvLiquidazioni.EditIndex = 0;
                else
                    gvLiquidazioni.EditIndex = -1;
                //Bind data to the GridView control.
                GriglieLiquidazioni_Load();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCLiquidazioni, Errore nel metodo gvLiquidazioni_RowCancelingEdit " + ex);
            }

        }

        protected void gvLiquidazioni_RowEditing(object sender, GridViewEditEventArgs e)
        {
            try
            {
                gvLiquidazioni.EditIndex = e.NewEditIndex;
                GriglieLiquidazioni_Load();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCLiquidazioni, Errore nel metodo gvLiquidazioni_RowEditing " + ex);
            }
        }

        protected void gvLiquidazioni_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    List<LiqAbilitate> elencoLiquidazioniAbilitate = (List<LiqAbilitate>)ViewState["LiquidazioniAbilitate"];
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
                        save.ValidationGroup = "UCLiquidazioni";
                        save.CommandName = "Salva";
                        save.OnClientClick = "BlockUI();";

                        TextBox txtSiglaCategoria = new TextBox();
                        txtSiglaCategoria = (TextBox)e.Row.FindControl("txtSiglaCategoria");
                        txtSiglaCategoria.Text = ((LiqAbilitate)e.Row.DataItem).SiglaCategoria;
                        if (!string.IsNullOrEmpty(txtSiglaCategoria.Text))
                            txtSiglaCategoria.Enabled = false;

                        TextBox txtSede = new TextBox();
                        txtSede = (TextBox)e.Row.FindControl("txtSede");
                        txtSede.Text = ((LiqAbilitate)e.Row.DataItem).Sede;
                        if (!string.IsNullOrEmpty(txtSede.Text))
                            txtSede.Enabled = false;

                        DropDownList ddlTipologia = new DropDownList();
                        ddlTipologia = (DropDownList)e.Row.FindControl("ddlTipologia");
                        LoadDdlTipologia(ddlTipologia);
                        ddlTipologia.SelectedValue = tipoAppRuolo.ToString();//((LiqAbilitate)e.Row.DataItem).Tipologia;
                        //if (!txtSiglaCategoria.Enabled && !txtSede.Enabled)
                        //    ddlTipologia.Enabled = false;

                        DropDownList ddlRicostituzione = (DropDownList)e.Row.FindControl("ddlRicostituzione");
                        LoadDdlSINO(ddlRicostituzione);
                        ddlRicostituzione.SelectedValue = ((LiqAbilitate)e.Row.DataItem).Ricostituzione;

                        DropDownList ddlManualiAbilitate = (DropDownList)e.Row.FindControl("ddlManualiAbilitateGrid");
                        LoadDdlSINO(ddlManualiAbilitate);
                        ddlManualiAbilitate.SelectedValue = ((LiqAbilitate)e.Row.DataItem).ManualiAbilitate;
                    }
                    else
                    {
                        if (e.Row.DataItemIndex == elencoLiquidazioniAbilitate.Count - 1)
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
                            if (index >= 0 && index <= elencoLiquidazioniAbilitate.Count - 2)
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
                throw new INPS.DNA.DnaApplicationException("UCLiquidazioni, Errore nel metodo gvLiquidazioni_RowDataBound " + ex);
            }
        }

        protected void gvLiquidazioni_onPageIndexChanging(Object sender, GridViewPageEventArgs e)
        {
            try
            {
                gvLiquidazioni.EditIndex = -1;
                gvLiquidazioni.PageIndex = e.NewPageIndex;
                GriglieLiquidazioni_Load();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCLiquidazioni, Errore nel metodo gvLiquidazioni_onPageIndexChanging" + ex);
            }
        }

        protected void gvLiquidazioni_onRowDeleting(object sender, GridViewDeleteEventArgs e)
        {

        }
        #endregion gvLiquidazioni

        #region gvLiquidazioniFSPT_INPDAP
        protected void gvLiquidazioniFSPT_INPDAP_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            List<LiqAbilitate> elencoLiquidazioniAbilitate = (List<LiqAbilitate>)ViewState["LiquidazioniAbilitateFSPT_INPDAP"];
            if (e.CommandName == "Delete")
            {
                Control c = (Control)e.CommandSource;
                GridViewRow r = (GridViewRow)c.NamingContainer;

                PresenterLiquidazioniAbilitate presenterLiquidazioniAbilitate = new PresenterLiquidazioniAbilitate();
                ValorizzaLiquidazioneAbilitataPerDelete(r.DataItemIndex, elencoLiquidazioniAbilitate);
                presenterLiquidazioniAbilitate.EliminaLiquidazioneAbilitata(this);

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

                PresenterLiquidazioniAbilitate presenterLiquidazioniAbilitate = new PresenterLiquidazioniAbilitate();
                ValorizzaLiquidazioneAbilitataPerSave(r, elencoLiquidazioniAbilitate, true);
                if (this.HasError)
                    return;

                presenterLiquidazioniAbilitate.SalvaLiquidazioneAbilitata(this);

                if (this.HasError)
                {
                    RaiseShowAvviso(this, null);
                    return;
                }

                gvLiquidazioniFSPT_INPDAP.EditIndex = -1;

                ValorizzaGriglia();
                AbilitaFiltro();
                RaiseShowInfo(this, null);
            }
            else if (e.CommandName == "Cancel")
            {
                GridView r = (GridView)e.CommandSource;
                int index = r.EditIndex + (r.PageIndex * r.PageSize);
                if (index == elencoLiquidazioniAbilitate.Count - 1)
                    elencoLiquidazioniAbilitate[index] = new LiqAbilitate();
                RaiseHideInfo(this, null);
            }
        }

        protected void gvLiquidazioniFSPT_INPDAP_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            try
            {
                //Reset the edit index.
                if (((List<LiqAbilitate>)ViewState["LiquidazioniAbilitateFSPT_INPDAP"]).Count() < 2)
                    gvLiquidazioniFSPT_INPDAP.EditIndex = 0;
                else
                    gvLiquidazioniFSPT_INPDAP.EditIndex = -1;
                //Bind data to the GridView control.
                GriglieLiquidazioni_Load();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCLiquidazioni, Errore nel metodo gvLiquidazioniFSPT_INPDAP_RowCancelingEdit " + ex);
            }

        }

        protected void gvLiquidazioniFSPT_INPDAP_RowEditing(object sender, GridViewEditEventArgs e)
        {
            try
            {
                gvLiquidazioniFSPT_INPDAP.EditIndex = e.NewEditIndex;
                GriglieLiquidazioni_Load();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCLiquidazioni, Errore nel metodo gvLiquidazioniFSPT_INPDAP_RowEditing " + ex);
            }
        }

        protected void gvLiquidazioniFSPT_INPDAP_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    List<LiqAbilitate> elencoLiquidazioniAbilitate = (List<LiqAbilitate>)ViewState["LiquidazioniAbilitateFSPT_INPDAP"];
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
                        save.ValidationGroup = "UCLiquidazioni";
                        save.CommandName = "Salva";
                        save.OnClientClick = "BlockUI();";

                        TextBox txtSiglaCategoria = new TextBox();
                        txtSiglaCategoria = (TextBox)e.Row.FindControl("txtSiglaCategoria");
                        txtSiglaCategoria.Text = ((LiqAbilitate)e.Row.DataItem).SiglaCategoria;
                        if (!string.IsNullOrEmpty(txtSiglaCategoria.Text))
                            txtSiglaCategoria.Enabled = false;

                        TextBox txtSede = new TextBox();
                        txtSede = (TextBox)e.Row.FindControl("txtSede");
                        txtSede.Text = ((LiqAbilitate)e.Row.DataItem).Sede;
                        if (!string.IsNullOrEmpty(txtSede.Text))
                            txtSede.Enabled = false;

                        DropDownList ddlTipologia = new DropDownList();
                        ddlTipologia = (DropDownList)e.Row.FindControl("ddlTipologia");
                        LoadDdlTipologia(ddlTipologia);
                        ddlTipologia.SelectedValue = tipoAppRuolo.ToString();//((LiqAbilitate)e.Row.DataItem).Tipologia;
                        //if (!txtSiglaCategoria.Enabled && !txtSede.Enabled)
                        //    ddlTipologia.Enabled = false;

                        DropDownList ddlRicostituzione = (DropDownList)e.Row.FindControl("ddlRicostituzione");
                        LoadDdlSINO(ddlRicostituzione);
                        ddlRicostituzione.SelectedValue = ((LiqAbilitate)e.Row.DataItem).Ricostituzione;

                        DropDownList ddlManualiAbilitate = (DropDownList)e.Row.FindControl("ddlManualiAbilitateGrid");
                        LoadDdlSINO(ddlManualiAbilitate);
                        ddlManualiAbilitate.SelectedValue = ((LiqAbilitate)e.Row.DataItem).ManualiAbilitate;

                        DropDownList ddlRicostituzioneDaAutomatica = (DropDownList)e.Row.FindControl("ddlRicostituzioneDaAutomatica");
                        LoadDdlSINO(ddlRicostituzioneDaAutomatica);
                        ddlRicostituzioneDaAutomatica.SelectedValue = ((LiqAbilitate)e.Row.DataItem).RicostituzioneDaAutomatica;

                        DropDownList ddlAutomaticheAbilitate = (DropDownList)e.Row.FindControl("ddlAutomaticheAbilitate");
                        LoadDdlSINO(ddlAutomaticheAbilitate);
                        ddlAutomaticheAbilitate.SelectedValue = ((LiqAbilitate)e.Row.DataItem).AutomaticheAbilitate;
                    }
                    else
                    {
                        if (e.Row.DataItemIndex == elencoLiquidazioniAbilitate.Count - 1)
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
                            if (index >= 0 && index <= elencoLiquidazioniAbilitate.Count - 2)
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
                throw new INPS.DNA.DnaApplicationException("UCLiquidazioni, Errore nel metodo gvLiquidazioniFSPT_INPDAP_RowDataBound " + ex);
            }
        }

        protected void gvLiquidazioniFSPT_INPDAP_onPageIndexChanging(Object sender, GridViewPageEventArgs e)
        {
            try
            {
                gvLiquidazioniFSPT_INPDAP.EditIndex = -1;
                gvLiquidazioniFSPT_INPDAP.PageIndex = e.NewPageIndex;
                GriglieLiquidazioni_Load();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCLiquidazioni, Errore nel metodo gvLiquidazioniFSPT_INPDAP_onPageIndexChanging" + ex);
            }
        }

        protected void gvLiquidazioniFSPT_INPDAP_onRowDeleting(object sender, GridViewDeleteEventArgs e)
        {

        }
        #endregion gvLiquidazioniFSPT_INPDAP

        private void LoadDdlSINO(DropDownList ddl)
        {
            try
            {
                ListItem li = new ListItem();
                li.Text = "NO";
                li.Value = "NO";
                ddl.Items.Add(li);
                li = new ListItem();
                li.Text = "SI";
                li.Value = "SI";
                ddl.Items.Add(li);
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCLiquidazioni, Errore nel metodo LoadDdlRicostituzione " + ex);
            }
        }

        private void LoadDdlTipologia(DropDownList ddl)
        {
            try
            {
                foreach (AreaLiquidazioniAbilitate.DatiLiquidazioneAbilitata.Tipo tipo in (AreaLiquidazioniAbilitate.DatiLiquidazioneAbilitata.Tipo[])ViewState["Tipologie"])
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
                throw new INPS.DNA.DnaApplicationException("UCLiquidazioni, Errore nel metodo LoadDdlTipologia " + ex);
            }
        }

        private void LoadDdlOpOperazione(DropDownList ddl)
        {
            try
            {
                ListItem li = new ListItem();
                li.Text = "Salvataggio su tutte le sedi";
                li.Value = "SAVE";
                ddl.Items.Add(li);
                li = new ListItem();
                li.Text = "Eliminazione su tutte le sedi";
                li.Value = "DELETE";
                ddl.Items.Add(li);

            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCLiquidazioni, Errore nel metodo LoadDdlTipologia " + ex);
            }
        }

        private void ValorizzaLiquidazioneAbilitataPerDelete(int index, List<LiqAbilitate> elencoLiqAbilitate)
        {
            ValorizzaLiquidazioniAbilitate(elencoLiqAbilitate, index, false);
        }

        private void ValorizzaLiquidazioneAbilitataPerSave(GridViewRow row, List<LiqAbilitate> elencoLiqAbilitate, bool isLiqAbilitateFSPT_INPDAP)
        {
            elencoLiqAbilitate[row.DataItemIndex].SiglaCategoria = ((TextBox)row.Cells[1].Controls[1]).Text.ToUpperInvariant();
            elencoLiqAbilitate[row.DataItemIndex].Sede = ((TextBox)row.Cells[2].Controls[1]).Text;
            elencoLiqAbilitate[row.DataItemIndex].Tipologia = ((DropDownList)row.Cells[3].Controls[1]).SelectedValue;
            elencoLiqAbilitate[row.DataItemIndex].Ricostituzione = ((DropDownList)row.Cells[4].Controls[1]).SelectedValue;
            elencoLiqAbilitate[row.DataItemIndex].ManualiAbilitate = ((DropDownList)row.FindControl("ddlManualiAbilitateGrid")).SelectedValue;
            if (row.FindControl("ddlRicostituzioneDaAutomatica") != null)
                elencoLiqAbilitate[row.DataItemIndex].RicostituzioneDaAutomatica = ((DropDownList)row.FindControl("ddlRicostituzioneDaAutomatica")).SelectedValue;
            if (row.FindControl("ddlAutomaticheAbilitate") != null)
                elencoLiqAbilitate[row.DataItemIndex].AutomaticheAbilitate = ((DropDownList)row.FindControl("ddlAutomaticheAbilitate")).SelectedValue;

            if (this.tipoAppRuolo == UtilityTipoAppartenenza.FS)
            {
                if (isLiqAbilitateFSPT_INPDAP)
                {
                    if (CodeUtility.GetTipoFondoByCategoria(this.tipoAppRuolo, elencoLiqAbilitate[row.DataItemIndex].SiglaCategoria) != CodeUtility.TipoFondo.FS &&
                        CodeUtility.GetTipoFondoByCategoria(this.tipoAppRuolo, elencoLiqAbilitate[row.DataItemIndex].SiglaCategoria) != CodeUtility.TipoFondo.PT &&
                        !IsCategoriaINPDAP(elencoLiqAbilitate[row.DataItemIndex].SiglaCategoria))
                    {
                        this.HasError = true;
                        this.ErrorMessage = "Categoria Pensione non corretta, selezionare una categoria FS o PT o INPDAP";
                        RaiseShowAvviso(this, null);
                        return;
                    }
                }
                else
                {
                    if (CodeUtility.GetTipoFondoByCategoria(this.tipoAppRuolo, elencoLiqAbilitate[row.DataItemIndex].SiglaCategoria) == CodeUtility.TipoFondo.FS ||
                        CodeUtility.GetTipoFondoByCategoria(this.tipoAppRuolo, elencoLiqAbilitate[row.DataItemIndex].SiglaCategoria) == CodeUtility.TipoFondo.PT ||
                        IsCategoriaINPDAP(elencoLiqAbilitate[row.DataItemIndex].SiglaCategoria))
                    {
                        this.HasError = true;
                        this.ErrorMessage = "Categoria Pensione non corretta, selezionare una categoria diversa da FS, PT, INPDAP";
                        RaiseShowAvviso(this, null);
                        return;
                    }
                }
            }

            ValorizzaLiquidazioniAbilitate(elencoLiqAbilitate, row.DataItemIndex, true);
        }

        private void ValorizzaLiquidazioniAbilitate(List<LiqAbilitate> elencoLiqAbilitate, int index, bool isSalvataggio)
        {
            this.datiLiquidazioneAbilitata = new AreaLiquidazioniAbilitate.DatiLiquidazioneAbilitata();

            datiLiquidazioneAbilitata.SiglaCategoria = elencoLiqAbilitate[index].SiglaCategoria;
            if (isSalvataggio)
            {
                Office office = null;
                try
                {
                    office = (from o in INPS.DNA.Context.OfficeList.OfficeFullList
                              where o.AspnCode.PadLeft(4, '0').Substring(0, 4) == elencoLiqAbilitate[index].Sede.PadLeft(4, '0')
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
            datiLiquidazioneAbilitata.Sede = elencoLiqAbilitate[index].Sede.PadLeft(4, '0');
            switch (elencoLiqAbilitate[index].Tipologia)
            {
                case "FS":
                    datiLiquidazioneAbilitata.Tipologia = AreaLiquidazioniAbilitate.DatiLiquidazioneAbilitata.Tipo.FS;
                    break;
                case "CI":
                    datiLiquidazioneAbilitata.Tipologia = AreaLiquidazioniAbilitate.DatiLiquidazioneAbilitata.Tipo.CI;
                    break;
                case "AGO":
                    datiLiquidazioneAbilitata.Tipologia = AreaLiquidazioniAbilitate.DatiLiquidazioneAbilitata.Tipo.AGO;
                    break;
            }
            datiLiquidazioneAbilitata.Ricostituzione = elencoLiqAbilitate[index].Ricostituzione == "SI" ? true : false;
            datiLiquidazioneAbilitata.AbilitazioneManuale = elencoLiqAbilitate[index].ManualiAbilitate == "SI" ? true : false;
            datiLiquidazioneAbilitata.RicostituzioneDaAutomatica = !string.IsNullOrEmpty(elencoLiqAbilitate[index].RicostituzioneDaAutomatica) ? (elencoLiqAbilitate[index].RicostituzioneDaAutomatica == "SI" ? true : false) : (bool?)null;
            datiLiquidazioneAbilitata.AbilitazioneAutomatica = !string.IsNullOrEmpty(elencoLiqAbilitate[index].AutomaticheAbilitate) ? (elencoLiqAbilitate[index].AutomaticheAbilitate == "SI" ? true : false) : (bool?)null;
        }

        protected void btnApplicaFiltro_Click(object sender, EventArgs e)
        {
            List<LiqAbilitate> elencoLiqAbilitateFiltrate = null;
            List<LiqAbilitate> elencoLiqAbilitateFSPT_INPDAPFiltrate = null;
            bool isListaFiltrata = false;
            Filtra((List<LiqAbilitate>)ViewState["LiquidazioniAbilitate"], out elencoLiqAbilitateFiltrate, out isListaFiltrata);
            if (isListaFiltrata)
                ViewState["LiquidazioniAbilitate"] = elencoLiqAbilitateFiltrate;
            if (tipoAppRuolo == UtilityTipoAppartenenza.FS)
            {
                Filtra((List<LiqAbilitate>)ViewState["LiquidazioniAbilitateFSPT_INPDAP"], out elencoLiqAbilitateFSPT_INPDAPFiltrate, out isListaFiltrata);
                if (isListaFiltrata)
                    ViewState["LiquidazioniAbilitateFSPT_INPDAP"] = elencoLiqAbilitateFSPT_INPDAPFiltrate;
            }
            GriglieLiquidazioni_Load();
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

        private void DisabilitaFiltro()
        {
            btnApplicaFiltro.Enabled = false;
            btnAnnullaFiltro.Enabled = true;
            txtFiltroSiglaCategoria.Enabled = false;
            txtFiltroSede.Enabled = false;
            //ddlFiltroTipologia.Enabled = false;
            ddlFiltroRicostituzione.Enabled = false;
            ddlFiltroManualiAbilitate.Enabled = false;
        }

        private void AbilitaFiltro()
        {
            btnApplicaFiltro.Enabled = true;
            btnAnnullaFiltro.Enabled = false;
            txtFiltroSiglaCategoria.Enabled = true;
            txtFiltroSede.Enabled = true;
            //ddlFiltroTipologia.Enabled = true;
            ddlFiltroRicostituzione.Enabled = true;
            ddlFiltroManualiAbilitate.Enabled = true;
        }

        /// <summary>
        /// Ripulisce i campi del filtro di ricerca
        /// </summary>
        private void PulisciFiltro()
        {
            txtFiltroSiglaCategoria.Text = string.Empty;
            txtFiltroSede.Text = string.Empty;
            ddlFiltroRicostituzione.SelectedIndex = 0;
            ddlFiltroManualiAbilitate.SelectedIndex = 0;
        }

        private void Filtra(List<LiqAbilitate> elencoLiquidazioniAbilitate, out List<LiqAbilitate> elencoLiquidazioniAbilitateFiltrate, out bool isListaFiltrata)
        {
            isListaFiltrata = false;
            elencoLiquidazioniAbilitateFiltrate = new List<LiqAbilitate>();
            elencoLiquidazioniAbilitateFiltrate.Add(new LiqAbilitate());

            if (elencoLiquidazioniAbilitate != null && elencoLiquidazioniAbilitate.Count > 0)
            {
                elencoLiquidazioniAbilitateFiltrate = elencoLiquidazioniAbilitate;

                if (!string.IsNullOrEmpty(txtFiltroSiglaCategoria.Text.Trim()))
                {
                    isListaFiltrata = true;
                    elencoLiquidazioniAbilitateFiltrate = elencoLiquidazioniAbilitateFiltrate.FindAll(x => x.SiglaCategoria == txtFiltroSiglaCategoria.Text.ToUpperInvariant());
                }
                if (!string.IsNullOrEmpty(txtFiltroSede.Text.Trim()))
                {
                    isListaFiltrata = true;
                    elencoLiquidazioniAbilitateFiltrate = elencoLiquidazioniAbilitateFiltrate.FindAll(x => x.Sede == txtFiltroSede.Text);
                }
                //if (!string.IsNullOrEmpty(ddlFiltroTipologia.SelectedValue.Trim()))
                //{
                //    isListaFiltrata = true;
                //    elencoLiquidazioniAbilitateFiltrate = elencoLiquidazioniAbilitateFiltrate.FindAll(x => x.Tipologia == ddlFiltroTipologia.SelectedValue);
                //}
                if (!string.IsNullOrEmpty(ddlFiltroRicostituzione.SelectedValue.Trim()))
                {
                    isListaFiltrata = true;
                    elencoLiquidazioniAbilitateFiltrate = elencoLiquidazioniAbilitateFiltrate.FindAll(x => x.Ricostituzione == ddlFiltroRicostituzione.SelectedValue);
                }
                if (!string.IsNullOrEmpty(ddlFiltroManualiAbilitate.SelectedValue.Trim()))
                {
                    isListaFiltrata = true;
                    elencoLiquidazioniAbilitateFiltrate = elencoLiquidazioniAbilitateFiltrate.FindAll(x => x.ManualiAbilitate == ddlFiltroManualiAbilitate.SelectedValue);
                }
                if (isListaFiltrata)
                    elencoLiquidazioniAbilitateFiltrate.Add(new LiqAbilitate());
            }
        }

        private bool IsCategoriaINPDAP(string siglaCategoria)
        {
            if (string.IsNullOrEmpty(siglaCategoria) || ViewState["SigleCategorieINPDAP"] == null)
                return false;
            return ((List<string>)ViewState["SigleCategorieINPDAP"]).Exists(x => x == siglaCategoria.Trim().ToUpper());
        }
        protected void btnEseguiOp_Click(object sender, EventArgs e)
        {
            if (ddlOpOperazione.SelectedIndex == 0 ||
                string.IsNullOrEmpty(txtOpSiglaCategoria.Text.Trim()) ||
                //ddlOpTipologia.SelectedIndex == 0 ||
                ddlOpRicostituzione.SelectedIndex == 0 ||
                ddlManualiAbilitateAllSedi.SelectedIndex == 0)
            {
                this.HasError = true;
                this.ErrorMessage = "Errore nell'esecuzione dell'operazione su tutte le sedi: parametri mancanti";
                RaiseShowAvviso(this, null);
                return;
            }

            this.datiLiquidazioneAbilitata = new AreaLiquidazioniAbilitate.DatiLiquidazioneAbilitata();
            this.datiLiquidazioneAbilitata.SiglaCategoria = txtOpSiglaCategoria.Text.Trim();
            switch (txtOpTipologia.Text)
            {
                case "FS":
                    this.datiLiquidazioneAbilitata.Tipologia = AreaLiquidazioniAbilitate.DatiLiquidazioneAbilitata.Tipo.FS;
                    break;
                case "CI":
                    this.datiLiquidazioneAbilitata.Tipologia = AreaLiquidazioniAbilitate.DatiLiquidazioneAbilitata.Tipo.CI;
                    break;
                case "AGO":
                    this.datiLiquidazioneAbilitata.Tipologia = AreaLiquidazioniAbilitate.DatiLiquidazioneAbilitata.Tipo.AGO;
                    break;
            }

            datiLiquidazioneAbilitata.Ricostituzione = ddlOpRicostituzione.SelectedValue == "SI" ? true : false;
            datiLiquidazioneAbilitata.AbilitazioneManuale = ddlManualiAbilitateAllSedi.SelectedValue == "SI" ? true : false;

            PresenterLiquidazioniAbilitate presenterLiquidazioniAbilitate = new PresenterLiquidazioniAbilitate();
            switch (ddlOpOperazione.SelectedValue)
            {
                case "SAVE":
                    presenterLiquidazioniAbilitate.SalvaLiquidazioniAbilitateSuTutteLeSedi(this);
                    break;
                case "DELETE":
                    presenterLiquidazioniAbilitate.EliminaLiquidazioniAbilitateSuTutteLeSedi(this);
                    break;
            }

            if (this.HasError)
            {
                RaiseShowAvviso(this, null);
                return;
            }

            ValorizzaGriglia();
            AbilitaFiltro();
            RaiseShowInfo(this, null);
        }

        [Serializable()]
        public class LiqAbilitate
        {
            internal LiqAbilitate() { }

            #region private properties
            private string _SiglaCategoria;
            private string _Sede;
            private string _Tipologia;
            private string _Ricostituzione;
            private string _ManualiAbilitate;
            private string _RicostituzioneDaAutomatica;
            private string _AutomaticheAbilitate;

            #endregion private properties

            #region public properties
            public string SiglaCategoria { get { return _SiglaCategoria; } set { _SiglaCategoria = value; } }
            public string Sede { get { return _Sede; } set { _Sede = value; } }
            public string Tipologia { get { return _Tipologia; } set { _Tipologia = value; } }
            public string Ricostituzione { get { return _Ricostituzione; } set { _Ricostituzione = value; } }
            public string ManualiAbilitate { get { return _ManualiAbilitate; } set { _ManualiAbilitate = value; } }
            public string RicostituzioneDaAutomatica { get { return _RicostituzioneDaAutomatica; } set { _RicostituzioneDaAutomatica = value; } }
            public string AutomaticheAbilitate { get { return _AutomaticheAbilitate; } set { _AutomaticheAbilitate = value; } }
            #endregion public properties
        }
    }
}