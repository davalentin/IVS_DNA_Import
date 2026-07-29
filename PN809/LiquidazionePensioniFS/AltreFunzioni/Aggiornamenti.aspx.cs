using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using INPS.DNA.UI.Web;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using INPS.Pensioni.LiquidazionePensione.Presenter;
using INPS.Pensioni.LiquidazionePensione.Presenter.Enum;
using INPS.Pensioni.LiquidazionePensione.View.Web.UserControls;
using INPS.DNA;

namespace INPS.Pensioni.LiquidazionePensione.View.Web
{
    [NavigationRules(AllowedReferrer = AllowedReferrer.Application, CheckSequenceOnPostBack = false)]
    public partial class Aggiornamenti : CustomBasePage, IAggiornamenti
    {
        #region IViewUI Members
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        #endregion IViewUI Members

        #region IAggiornamenti
        public AreaAggiornamenti areaAggiornamenti { get; set; }
        public UtilityTipoAppartenenza? tipoApp { get; set; }
        #endregion IAggiornamenti

        protected void Page_Load(object sender, EventArgs e)
        {
            this.tipoApp = (UtilityTipoAppartenenza)Utility.GetTipoAppartenenzaRuolo((Ruoli)Session["Ruolo"]);

            if (!IsPostBack)
            {
                BindData();
            }
        }

        private void BindData()
        {
            this.HasError = false;
            this.ErrorMessage = string.Empty;
            if (ucAvviso.Visible)
                ucAvviso.Visible = false;

            PresenterAggiornamenti presenter = new PresenterAggiornamenti();
            presenter.GetAggiornamenti(this);

            if (this.HasError)
            {
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = this.ErrorMessage;
                ucAvviso.Tipo = TipoAvviso.Ko;
                PanelAggiornamentiVediTutti.Visible = false;
                return;
            }

            CodeUtility.SetAggiornamenti(this.areaAggiornamenti);
            ViewState[EnumViewState.ElencoAggiornamentiGenerici.ToString()] = this.areaAggiornamenti.ElencoAggiornamenti;
            grdListaAggiornamentiVediTutti.DataSource = this.areaAggiornamenti.ElencoAggiornamenti;
            grdListaAggiornamentiVediTutti.DataBind();
            return;
        }

        protected void grdListaAggiornamentiVediTutti_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            int i = e.Row.DataItemIndex;
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                Label lblIdAggiornamento = (Label)e.Row.FindControl("lblIdAggiornamento");
                Label lblDataAggiornamento = (Label)e.Row.FindControl("lblDataAggiornamento");
                Label lblTitolo = (Label)e.Row.FindControl("lblTitolo");
                Label lblTesto = (Label)e.Row.FindControl("lblTesto");
                ImageButton imgbtnNascondiRendiVisibile = (ImageButton)e.Row.FindControl("imgbtnNascondiRendiVisibile");

                if (lblIdAggiornamento != null && lblDataAggiornamento != null && lblTitolo != null && lblTesto != null)
                {
                    lblIdAggiornamento.Text = this.areaAggiornamenti.ElencoAggiornamenti[i].Id.ToString();
                    lblDataAggiornamento.Text = this.areaAggiornamenti.ElencoAggiornamenti[i].TimeStamp.ToString("dd/MM/yyyy");
                    lblTitolo.Text = this.areaAggiornamenti.ElencoAggiornamenti[i].Titolo;
                    lblTesto.Text = this.areaAggiornamenti.ElencoAggiornamenti[i].Testo;
                    if (!(this.areaAggiornamenti.ElencoAggiornamenti[i].Attivo))
                    {
                        imgbtnNascondiRendiVisibile.ImageUrl = string.Format("../App_Themes/{0}/Images/turn_off.png", Page.Theme);
                        imgbtnNascondiRendiVisibile.ToolTip = "Aggiornamento non visibile. Clicca per modificarne la visibilità.";
                    }
                }
            }
        }

        protected void grdListaAggiornamentiVediTutti_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName != "Page")
            {
                Label lblIdAggiornamento = ((GridViewRow)(((Control)(e.CommandSource)).NamingContainer)).FindControl("lblIdAggiornamento") as Label;
                if (lblIdAggiornamento != null)
                {
                    long IdAggiornamentoSelezionato = -1;
                    long.TryParse(lblIdAggiornamento.Text, out IdAggiornamentoSelezionato);

                    if (e.CommandName == "Visible")
                    {
                        AggiornaVisibilita(IdAggiornamentoSelezionato);
                    }
                    else if (e.CommandName == "Delete")
                    {
                        EliminaAggiornamento(IdAggiornamentoSelezionato);
                    }
                    else if (e.CommandName == "Update")
                    {
                        Response.Redirect("AggiornamentiEdit.aspx?oper=2&id=" + IdAggiornamentoSelezionato);
                    }
                }
            }
        }

        protected void AggiornaVisibilita(long IdAggiornamentoSelezionato)
        {
            this.HasError = false;
            this.ErrorMessage = string.Empty;
            if (ucAvviso.Visible)
                ucAvviso.Visible = false;

            this.areaAggiornamenti = new AreaAggiornamenti();
            this.areaAggiornamenti.ElencoAggiornamenti = (Presenter.SvrLiquidazione.Aggiornamenti[])ViewState[EnumViewState.ElencoAggiornamentiGenerici.ToString()];
            Presenter.SvrLiquidazione.Aggiornamenti aggsUpdt = null;

            foreach (Presenter.SvrLiquidazione.Aggiornamenti aggs in this.areaAggiornamenti.ElencoAggiornamenti)
                if (aggs.Id == IdAggiornamentoSelezionato)
                {
                    aggsUpdt = aggs;
                    break;
                }

            if (aggsUpdt != null)
            {
                aggsUpdt.Attivo = (!(aggsUpdt.Attivo)); //change visibilità
                this.areaAggiornamenti = new AreaAggiornamenti();
                this.areaAggiornamenti.ElencoAggiornamenti = new Presenter.SvrLiquidazione.Aggiornamenti[1];
                this.areaAggiornamenti.ElencoAggiornamenti[0] = aggsUpdt;
                this.areaAggiornamenti.ElencoAggiornamenti[0].Tipologia = Utility.GetTipoAppartenenzaRuolo((Ruoli)Session["Ruolo"]).ToString();
                PresenterAggiornamenti presenter = new PresenterAggiornamenti();
                presenter.SalvaAggiornamento(this);

                if (!this.HasError)
                {
                    CodeUtility.SetAggiornamenti(this.areaAggiornamenti);
                    ViewState[EnumViewState.ElencoAggiornamentiGenerici.ToString()] = this.areaAggiornamenti.ElencoAggiornamenti;
                    grdListaAggiornamentiVediTutti.DataSource = this.areaAggiornamenti.ElencoAggiornamenti;
                    grdListaAggiornamentiVediTutti.DataBind();
                }
            }
            else
            {
                this.HasError = true;
                this.ErrorMessage = "Si è verificato un errore durante la modifica dell'aggiornamento.";
            }

            if (this.HasError)
            {
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = this.ErrorMessage;
                ucAvviso.Tipo = TipoAvviso.Ko;
                PanelAggiornamentiVediTutti.Visible = false;
                return;
            }
        }

        protected void EliminaAggiornamento(long IdAggiornamentoSelezionato)
        {
            this.HasError = false;
            this.ErrorMessage = string.Empty;
            if (ucAvviso.Visible)
                ucAvviso.Visible = false;

            this.areaAggiornamenti = new AreaAggiornamenti();
            this.areaAggiornamenti.ElencoAggiornamenti = (Presenter.SvrLiquidazione.Aggiornamenti[])ViewState[EnumViewState.ElencoAggiornamentiGenerici.ToString()];
            Presenter.SvrLiquidazione.Aggiornamenti aggsDel = null;

            foreach (Presenter.SvrLiquidazione.Aggiornamenti aggs in this.areaAggiornamenti.ElencoAggiornamenti)
                if (aggs.Id == IdAggiornamentoSelezionato)
                {
                    aggsDel = aggs;
                    break;
                }

            if (aggsDel != null)
            {
                aggsDel.Attivo = (!(aggsDel.Attivo)); //change visibilità
                this.areaAggiornamenti = new AreaAggiornamenti();
                this.areaAggiornamenti.ElencoAggiornamenti = new Presenter.SvrLiquidazione.Aggiornamenti[1];
                this.areaAggiornamenti.ElencoAggiornamenti[0] = aggsDel;
                PresenterAggiornamenti presenter = new PresenterAggiornamenti();
                presenter.DeleteAggiornamento(this);

                if (!this.HasError)
                {
                    CodeUtility.SetAggiornamenti(this.areaAggiornamenti);
                    ViewState[EnumViewState.ElencoAggiornamentiGenerici.ToString()] = this.areaAggiornamenti.ElencoAggiornamenti;
                    grdListaAggiornamentiVediTutti.DataSource = this.areaAggiornamenti.ElencoAggiornamenti;
                    grdListaAggiornamentiVediTutti.DataBind();
                }
            }
            else
            {
                this.HasError = true;
                this.ErrorMessage = "Si è verificato un errore durante l' eliminazione dell'aggiornamento.";
            }

            if (this.HasError)
            {
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = this.ErrorMessage;
                ucAvviso.Tipo = TipoAvviso.Ko;
                PanelAggiornamentiVediTutti.Visible = false;
                return;
            }
        }

        protected void lnkBtnNuovoAggiornamento_Click(object sender, EventArgs e)
        {
            Response.Redirect("AggiornamentiEdit.aspx?oper=1");
        }

        protected void grdListaAggiornamentiVediTutti_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {

        }

        protected void grdListaAggiornamentiVediTutti_onPageIndexChanging(Object sender, GridViewPageEventArgs e)
        {
            try
            {
                grdListaAggiornamentiVediTutti.PageIndex = e.NewPageIndex;
                if (this.areaAggiornamenti == null)
                    this.areaAggiornamenti = new AreaAggiornamenti();

                this.areaAggiornamenti.ElencoAggiornamenti = (Presenter.SvrLiquidazione.Aggiornamenti[])ViewState[EnumViewState.ElencoAggiornamentiGenerici.ToString()];
                grdListaAggiornamentiVediTutti.DataSource = this.areaAggiornamenti.ElencoAggiornamenti;
                grdListaAggiornamentiVediTutti.DataBind();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("Aggiornamenti, Errore nel metodo grdListaAggiornamentiVediTutti_onPageIndexChanging: " + ex);
            }
        }

        public enum EnumViewState
        {
            ElencoAggiornamentiGenerici
        }

        protected string setImage(string name)
        {
            return string.Format("~/App_Themes/{0}/Images/{1}", Page.Theme ?? "BlueINPS1", name);
        }
    }
}