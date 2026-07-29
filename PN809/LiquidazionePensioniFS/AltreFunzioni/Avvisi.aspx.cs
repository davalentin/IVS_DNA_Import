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
using INPS.Pensioni.LiquidazionePensione.View.Web;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter;
using INPS.Pensioni.LiquidazionePensione.View.Web.UserControls;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using INPS.Pensioni.LiquidazionePensione.Presenter.Enum;
using INPS.DNA;

namespace INPS.Pensioni.LiquidazionePensione.View.Web
{
    [NavigationRules(AllowedReferrer = AllowedReferrer.Application, CheckSequenceOnPostBack = false)]
	public partial class Avvisi : CustomBasePage, IAvvisi
	{
		#region IViewUI Members
		public string ErrorMessage { get; set; }
		public bool HasError { get; set; }
        #endregion IViewUI Members

        #region IAvvisi
        public AreaAvvisi areaAvvisi { get; set; }
        public UtilityTipoAppartenenza? tipoApp { get; set; }
        #endregion IAvvisi

        protected void Page_Load(object sender, EventArgs e)
		{
            this.tipoApp = (UtilityTipoAppartenenza)Utility.GetTipoAppartenenzaRuolo((Ruoli)Session["Ruolo"]);

			if (!IsPostBack)
			{
                //CodeUtility.SalvaFunzioneInSession(Utility.Funzione.Avvisi);
				BindData();
			}
		}

		private void BindData()
		{
			this.HasError = false;
			this.ErrorMessage = string.Empty;
			if (ucAvviso.Visible)
				ucAvviso.Visible = false;

            PresenterAvvisi presenter = new PresenterAvvisi();
            presenter.GetAvvisi(this);

			if (this.HasError)
			{
				ucAvviso.Visible = true;
				ucAvviso.Messaggio = this.ErrorMessage;
				ucAvviso.Tipo = TipoAvviso.Ko;
				PanelAvvisiVediTutti.Visible = false;
				return;
			}

            CodeUtility.SetAvvisi(this.areaAvvisi);
            ViewState["ElencoAvvisiGenerici"] = this.areaAvvisi.ElencoAvvisi;
            grdListaAvvisiVediTutti.DataSource = this.areaAvvisi.ElencoAvvisi;
			grdListaAvvisiVediTutti.DataBind();
			return;
		}

		protected void grdListaAvvisiVediTutti_RowDataBound(object sender, GridViewRowEventArgs e)
		{
			int i = e.Row.DataItemIndex;
			if (e.Row.RowType == DataControlRowType.DataRow)
			{
				Label lblIdAvviso = (Label)e.Row.FindControl("lblIdAvviso");
				Label lblDataAvviso = (Label)e.Row.FindControl("lblDataAvviso");
				Label lblTitolo = (Label)e.Row.FindControl("lblTitolo");
				Label lblTesto = (Label)e.Row.FindControl("lblTesto");
				ImageButton imgbtnNascondiRendiVisibile = (ImageButton)e.Row.FindControl("imgbtnNascondiRendiVisibile");

				if (lblIdAvviso != null && lblDataAvviso != null && lblTitolo != null && lblTesto != null)
				{
                    lblIdAvviso.Text = this.areaAvvisi.ElencoAvvisi[i].Id.ToString();
                    lblDataAvviso.Text = this.areaAvvisi.ElencoAvvisi[i].TimeStamp.ToString("dd/MM/yyyy");
                    lblTitolo.Text = this.areaAvvisi.ElencoAvvisi[i].Titolo;
                    lblTesto.Text = this.areaAvvisi.ElencoAvvisi[i].Testo;
                    if (!(this.areaAvvisi.ElencoAvvisi[i].Attivo))
					{
                        imgbtnNascondiRendiVisibile.ImageUrl = string.Format("../App_Themes/{0}/Images/turn_off.png", Page.Theme);
                        imgbtnNascondiRendiVisibile.ToolTip = "Avviso non visibile. Clicca per modificarne la visibilità.";
					}
				}
			}
		}

		protected void grdListaAvvisiVediTutti_RowCommand(object sender, GridViewCommandEventArgs e)
		{
            if (e.CommandName != "Page")
            {
                Label lblIdAvviso = ((GridViewRow)(((Control)(e.CommandSource)).NamingContainer)).FindControl("lblIdAvviso") as Label;
                if (lblIdAvviso != null)
                {
                    long IdAvvisoSelezionato = -1;
                    long.TryParse(lblIdAvviso.Text, out IdAvvisoSelezionato);

                    if (e.CommandName == "Visible")
                    {
                        AggiornaVisibilita(IdAvvisoSelezionato);
                    }
                    else if (e.CommandName == "Delete")
                    {
                        EliminaAvviso(IdAvvisoSelezionato);
                    }
                    else if (e.CommandName == "Update")
                    {
                        Response.Redirect("AvvisiEdit.aspx?oper=2&id=" + IdAvvisoSelezionato);
                    }
                }
            }
		}

		protected void AggiornaVisibilita(long IdAvvisoSelezionato)
		{
			this.HasError = false;
			this.ErrorMessage = string.Empty;
			if (ucAvviso.Visible)
				ucAvviso.Visible = false;

            this.areaAvvisi = new AreaAvvisi();
            this.areaAvvisi.ElencoAvvisi = (Presenter.SvrLiquidazione.Avvisi[])ViewState["ElencoAvvisiGenerici"];
            Presenter.SvrLiquidazione.Avvisi avvsUpdt = null;

            foreach (Presenter.SvrLiquidazione.Avvisi avvs in this.areaAvvisi.ElencoAvvisi)
				if (avvs.Id == IdAvvisoSelezionato)
				{
					avvsUpdt = avvs;
					break;
				}

			if (avvsUpdt != null)
			{
				avvsUpdt.Attivo = (!(avvsUpdt.Attivo)); //change visibilità
                this.areaAvvisi = new AreaAvvisi();
                this.areaAvvisi.ElencoAvvisi = new Presenter.SvrLiquidazione.Avvisi[1];
                this.areaAvvisi.ElencoAvvisi[0] = avvsUpdt;
                this.areaAvvisi.ElencoAvvisi[0].Tipologia = Utility.GetTipoAppartenenzaRuolo((Ruoli)Session["Ruolo"]).ToString();
                PresenterAvvisi presenter = new PresenterAvvisi();
                presenter.SalvaAvviso(this);

                if (!this.HasError)
                {
                    CodeUtility.SetAvvisi(this.areaAvvisi);
                    ViewState["ElencoAvvisiGenerici"] = this.areaAvvisi.ElencoAvvisi;
                    grdListaAvvisiVediTutti.DataSource = this.areaAvvisi.ElencoAvvisi;
                    grdListaAvvisiVediTutti.DataBind();
                }
			}
			else 
            {
				this.HasError = true;
				this.ErrorMessage = "Si è verificato un errore durante la modifica dell'avviso.";
			}

			if (this.HasError)
			{
				ucAvviso.Visible = true;
				ucAvviso.Messaggio = this.ErrorMessage;
				ucAvviso.Tipo = TipoAvviso.Ko;
				PanelAvvisiVediTutti.Visible = false;
				return;
			}
		}

        protected void EliminaAvviso(long IdAvvisoSelezionato)
        {
            this.HasError = false;
            this.ErrorMessage = string.Empty;
            if (ucAvviso.Visible)
                ucAvviso.Visible = false;

            this.areaAvvisi = new AreaAvvisi();
            this.areaAvvisi.ElencoAvvisi = (Presenter.SvrLiquidazione.Avvisi[])ViewState["ElencoAvvisiGenerici"];
            Presenter.SvrLiquidazione.Avvisi avvsDel = null;

            foreach (Presenter.SvrLiquidazione.Avvisi avvs in this.areaAvvisi.ElencoAvvisi)
                if (avvs.Id == IdAvvisoSelezionato)
                {
                    avvsDel = avvs;
                    break;
                }

            if (avvsDel != null)
            {
                avvsDel.Attivo = (!(avvsDel.Attivo)); //change visibilità
                this.areaAvvisi = new AreaAvvisi();
                this.areaAvvisi.ElencoAvvisi = new Presenter.SvrLiquidazione.Avvisi[1];
                this.areaAvvisi.ElencoAvvisi[0] = avvsDel;
                PresenterAvvisi presenter = new PresenterAvvisi();
                presenter.DeleteAvviso(this);

                if (!this.HasError)
                {
                    CodeUtility.SetAvvisi(this.areaAvvisi);
                    ViewState["ElencoAvvisiGenerici"] = this.areaAvvisi.ElencoAvvisi;
                    grdListaAvvisiVediTutti.DataSource = this.areaAvvisi.ElencoAvvisi;
                    grdListaAvvisiVediTutti.DataBind();
                }
            }
            else
            {
                this.HasError = true;
                this.ErrorMessage = "Si è verificato un errore durante l' eliminazione dell'avviso.";
            }

            if (this.HasError)
            {
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = this.ErrorMessage;
                ucAvviso.Tipo = TipoAvviso.Ko;
                PanelAvvisiVediTutti.Visible = false;
                return;
            }
        }

		protected void lnkBtnNuovoAvviso_Click(object sender, EventArgs e)
		{
			Response.Redirect("AvvisiEdit.aspx?oper=1");
		}

        protected void grdListaAvvisiVediTutti_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {

        }

        protected void grdListaAvvisiVediTutti_onPageIndexChanging(Object sender, GridViewPageEventArgs e)
        {
            try
            {
                grdListaAvvisiVediTutti.PageIndex = e.NewPageIndex;
                if (this.areaAvvisi == null)
                    this.areaAvvisi = new AreaAvvisi();

                this.areaAvvisi.ElencoAvvisi = (Presenter.SvrLiquidazione.Avvisi[])ViewState["ElencoAvvisiGenerici"];
                grdListaAvvisiVediTutti.DataSource = this.areaAvvisi.ElencoAvvisi;
                grdListaAvvisiVediTutti.DataBind();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("Avvisi, Errore nel metodo grdListaAvvisiVediTutti_onPageIndexChanging: " + ex);
            }
        }

        protected string setImage(string name)
        {
            return string.Format("~/App_Themes/{0}/Images/{1}", Page.Theme ?? "BlueINPS1", name);
        }
	}
}
