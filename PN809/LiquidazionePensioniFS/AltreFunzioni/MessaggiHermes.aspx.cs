using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using INPS.DNA.Security;
using INPS.DNA.Security.Idm;
using INPS.DNA.UI.Web;
using INPS.DNA.UI.Web.Intranet;
using System.Data;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using INPS.Pensioni.LiquidazionePensione.View.Web.UserControls;
using INPS.Pensioni.LiquidazionePensione.Presenter.Enum;
using INPS.DNA;

namespace INPS.Pensioni.LiquidazionePensione.View.Web
{
    [NavigationRules(AllowedReferrer = AllowedReferrer.Application, CheckSequenceOnPostBack = false)]
    public partial class MessaggiHermes : CustomBasePage, IMessaggiHermes
    {
        #region IViewUI Members
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        #endregion IViewUI Members

        #region IMessaggiHermes
        public AreaMessaggiHermes areaMessaggiHermes { get; set; }
        public UtilityTipoAppartenenza? tipoApp { get; set; }
        #endregion IMessaggiHermes

        protected void Page_Load(object sender, EventArgs e)
        {
            this.tipoApp = (UtilityTipoAppartenenza)Utility.GetTipoAppartenenzaRuolo((Ruoli)Session["Ruolo"]);

            if (!IsPostBack)
            {
                //CodeUtility.SalvaFunzioneInSession(Utility.Funzione.MessaggiHermes);
                BindData();
            }
        }

        private void BindData()
        {
            this.HasError = false;
            this.ErrorMessage = string.Empty;
            if (ucAvviso.Visible)
                ucAvviso.Visible = false;

            PresenterMessaggiHermes presenter = new PresenterMessaggiHermes();
            presenter.GetMessaggiHermes(this);

            if (this.HasError)
            {
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = this.ErrorMessage;
                ucAvviso.Tipo = TipoAvviso.Ko;
                PanelMessaggiHermesVediTutti.Visible = false;
                return;
            }

            CodeUtility.SetMessaggiHermes(this.areaMessaggiHermes);
            ViewState["ElencoMessaggiHermesGenerici"] = this.areaMessaggiHermes.ElencoMessaggiHermes;
            grdListaMessaggiHermesVediTutti.DataSource = this.areaMessaggiHermes.ElencoMessaggiHermes;
            grdListaMessaggiHermesVediTutti.DataBind();
            return;
        }

        protected void grdListaMessaggiHermesVediTutti_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            int i = e.Row.DataItemIndex;
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                Label lblIdMessaggioHermes = (Label)e.Row.FindControl("lblIdMessaggioHermes");
                Label lblDataMessaggioHermes = (Label)e.Row.FindControl("lblDataMessaggioHermes");
                Label lblTitolo = (Label)e.Row.FindControl("lblTitolo");
                Label lblTesto = (Label)e.Row.FindControl("lblTesto");
                ImageButton imgbtnNascondiRendiVisibile = (ImageButton)e.Row.FindControl("imgbtnNascondiRendiVisibile");

                if (lblIdMessaggioHermes != null && lblDataMessaggioHermes != null && lblTitolo != null && lblTesto != null)
                {
                    lblIdMessaggioHermes.Text = this.areaMessaggiHermes.ElencoMessaggiHermes[i].Id.ToString();
                    lblDataMessaggioHermes.Text = this.areaMessaggiHermes.ElencoMessaggiHermes[i].TimeStamp.ToString("dd/MM/yyyy");
                    lblTitolo.Text = this.areaMessaggiHermes.ElencoMessaggiHermes[i].Titolo;
                    lblTesto.Text = this.areaMessaggiHermes.ElencoMessaggiHermes[i].Testo;
                    if (!(this.areaMessaggiHermes.ElencoMessaggiHermes[i].Attivo))
                    {
                        imgbtnNascondiRendiVisibile.ImageUrl = string.Format("../App_Themes/{0}/Images/turn_off.png", Page.Theme);
                        imgbtnNascondiRendiVisibile.ToolTip = "Messaggio Hermes non visibile. Clicca per modificarne la visibilità.";
                    }
                }
            }
        }

        protected void grdListaMessaggiHermesVediTutti_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName != "Page")
            {
                Label lblIdMessaggioHermes = ((GridViewRow)(((Control)(e.CommandSource)).NamingContainer)).FindControl("lblIdMessaggioHermes") as Label;
                if (lblIdMessaggioHermes != null)
                {
                    long IdMessaggioHermesSelezionato = -1;
                    long.TryParse(lblIdMessaggioHermes.Text, out IdMessaggioHermesSelezionato);

                    if (e.CommandName == "Visible")
                    {
                        AggiornaVisibilita(IdMessaggioHermesSelezionato);
                    }
                    else if (e.CommandName == "Delete")
                    {
                        EliminaMessaggioHermes(IdMessaggioHermesSelezionato);
                    }
                    else if (e.CommandName == "Update")
                    {
                        Response.Redirect("MessaggiHermesEdit.aspx?oper=2&id=" + IdMessaggioHermesSelezionato);
                    }
                }
            }
        }

        protected void AggiornaVisibilita(long IdMessaggioHermesSelezionato)
        {
            this.HasError = false;
            this.ErrorMessage = string.Empty;
            if (ucAvviso.Visible)
                ucAvviso.Visible = false;

            this.areaMessaggiHermes = new AreaMessaggiHermes();
            this.areaMessaggiHermes.ElencoMessaggiHermes = (Presenter.SvrLiquidazione.MessaggiHermes[])ViewState["ElencoMessaggiHermesGenerici"];
            Presenter.SvrLiquidazione.MessaggiHermes messUpdt = null;

            foreach (Presenter.SvrLiquidazione.MessaggiHermes mess in this.areaMessaggiHermes.ElencoMessaggiHermes)
                if (mess.Id == IdMessaggioHermesSelezionato)
                {
                    messUpdt = mess;
                    break;
                }

            if (messUpdt != null)
            {
                messUpdt.Attivo = (!(messUpdt.Attivo)); //change visibilità
                this.areaMessaggiHermes = new AreaMessaggiHermes();
                this.areaMessaggiHermes.ElencoMessaggiHermes = new Presenter.SvrLiquidazione.MessaggiHermes[1];
                this.areaMessaggiHermes.ElencoMessaggiHermes[0] = messUpdt;
                this.areaMessaggiHermes.ElencoMessaggiHermes[0].Tipologia = Utility.GetTipoAppartenenzaRuolo((Ruoli)Session["Ruolo"]).ToString();
                PresenterMessaggiHermes presenter = new PresenterMessaggiHermes();
                presenter.SalvaMessaggioHermes(this);

                if (!this.HasError)
                {
                    CodeUtility.SetMessaggiHermes(this.areaMessaggiHermes);
                    ViewState["ElencoMessaggiHermesGenerici"] = this.areaMessaggiHermes.ElencoMessaggiHermes;
                    grdListaMessaggiHermesVediTutti.DataSource = this.areaMessaggiHermes.ElencoMessaggiHermes;
                    grdListaMessaggiHermesVediTutti.DataBind();
                }
            }
            else
            {
                this.HasError = true;
                this.ErrorMessage = "Si è verificato un errore durante la modifica dell'MessaggioHermes.";
            }

            if (this.HasError)
            {
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = this.ErrorMessage;
                ucAvviso.Tipo = TipoAvviso.Ko;
                PanelMessaggiHermesVediTutti.Visible = false;
                return;
            }
        }

        protected void EliminaMessaggioHermes(long IdMessaggioHermesSelezionato)
        {
            this.HasError = false;
            this.ErrorMessage = string.Empty;
            if (ucAvviso.Visible)
                ucAvviso.Visible = false;

            this.areaMessaggiHermes = new AreaMessaggiHermes();
            this.areaMessaggiHermes.ElencoMessaggiHermes = (Presenter.SvrLiquidazione.MessaggiHermes[])ViewState["ElencoMessaggiHermesGenerici"];
            Presenter.SvrLiquidazione.MessaggiHermes messDel = null;

            foreach (Presenter.SvrLiquidazione.MessaggiHermes mess in this.areaMessaggiHermes.ElencoMessaggiHermes)
                if (mess.Id == IdMessaggioHermesSelezionato)
                {
                    messDel = mess;
                    break;
                }

            if (messDel != null)
            {
                messDel.Attivo = (!(messDel.Attivo)); //change visibilità
                this.areaMessaggiHermes = new AreaMessaggiHermes();
                this.areaMessaggiHermes.ElencoMessaggiHermes = new Presenter.SvrLiquidazione.MessaggiHermes[1];
                this.areaMessaggiHermes.ElencoMessaggiHermes[0] = messDel;
                PresenterMessaggiHermes presenter = new PresenterMessaggiHermes();
                presenter.DeleteMessaggioHermes(this);

                if (!this.HasError)
                {
                    CodeUtility.SetMessaggiHermes(this.areaMessaggiHermes);
                    ViewState["ElencoMessaggiHermesGenerici"] = this.areaMessaggiHermes.ElencoMessaggiHermes;
                    grdListaMessaggiHermesVediTutti.DataSource = this.areaMessaggiHermes.ElencoMessaggiHermes;
                    grdListaMessaggiHermesVediTutti.DataBind();
                }
            }
            else
            {
                this.HasError = true;
                this.ErrorMessage = "Si è verificato un errore durante l' eliminazione dell'MessaggioHermes.";
            }

            if (this.HasError)
            {
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = this.ErrorMessage;
                ucAvviso.Tipo = TipoAvviso.Ko;
                PanelMessaggiHermesVediTutti.Visible = false;
                return;
            }
        }

        protected void lnkBtnNuovoMessaggioHermes_Click(object sender, EventArgs e)
        {
            Response.Redirect("MessaggiHermesEdit.aspx?oper=1");
        }

        protected void grdListaMessaggiHermesVediTutti_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {

        }

        protected void grdListaMessaggiHermesVediTutti_onPageIndexChanging(Object sender, GridViewPageEventArgs e)
        {
            try
            {
                grdListaMessaggiHermesVediTutti.PageIndex = e.NewPageIndex;
                if (this.areaMessaggiHermes == null)
                    this.areaMessaggiHermes = new AreaMessaggiHermes();

                this.areaMessaggiHermes.ElencoMessaggiHermes = (Presenter.SvrLiquidazione.MessaggiHermes[])ViewState["ElencoMessaggiHermesGenerici"];
                grdListaMessaggiHermesVediTutti.DataSource = this.areaMessaggiHermes.ElencoMessaggiHermes;
                grdListaMessaggiHermesVediTutti.DataBind();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("MessaggiHermes, Errore nel metodo grdListaMessaggiHermesVediTutti_onPageIndexChanging: " + ex);
            }
        }

        protected string setImage(string name)
        {
            return string.Format("~/App_Themes/{0}/Images/{1}", Page.Theme ?? "BlueINPS1", name);
        }
    }
}

