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
    public partial class GestioneFAQ : CustomBasePage, IFaq
    {
        #region IFaq Members
        public Presenter.SvrLiquidazione.AreaFAQ areaFAQ { get; set; }
        public Presenter.SvrLiquidazione.UtilityTipoAppartenenza? tipoApp { get; set; }
        #endregion IFaq Members

        #region IViewUI Members
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        #endregion IViewUI Members

        protected void Page_Load(object sender, EventArgs e)
        {
            this.tipoApp = (UtilityTipoAppartenenza)Utility.GetTipoAppartenenzaRuolo((Ruoli)Session["Ruolo"]);

            if (Session["PaginaGestioneFAQ"] == null)
                Session["PaginaGestioneFAQ"] = 0;

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

            PresenterFaq presenter = new PresenterFaq();
            presenter.GetFAQ(this);

            if (this.HasError)
            {
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = this.ErrorMessage;
                ucAvviso.Tipo = TipoAvviso.Ko;
                PanelFAQ.Visible = false;
                return;
            }

            ViewState["ElencoFAQ"] = this.areaFAQ.ElencoFAQ;
            grdListaFAQ.DataSource = this.areaFAQ.ElencoFAQ;
            if (Session["PaginaGestioneFAQ"] != null)
                grdListaFAQ.PageIndex = (int)Session["PaginaGestioneFAQ"];
            grdListaFAQ.DataBind();
            return;
        }

        protected void grdListaFAQ_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if(this.areaFAQ == null)
                this.areaFAQ = new AreaFAQ();
            if(this.areaFAQ.ElencoFAQ == null)
                this.areaFAQ.ElencoFAQ = (Presenter.SvrLiquidazione.FAQ[])ViewState["ElencoFAQ"];

            int i = e.Row.DataItemIndex;
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                Label lblIdFAQ = (Label)e.Row.FindControl("lblIdFAQ");
                Label lblCodiceFAQ = (Label)e.Row.FindControl("lblCodiceFAQ");
                Label lblDomanda = (Label)e.Row.FindControl("lblDomanda");
                Label lblTipologia = (Label)e.Row.FindControl("lblTipologia");
                ImageButton imgbtnNascondiRendiVisibile = (ImageButton)e.Row.FindControl("imgbtnNascondiRendiVisibile");

                if (lblIdFAQ != null && lblDomanda != null)
                {
                    lblIdFAQ.Text = this.areaFAQ.ElencoFAQ[i].Id.ToString();
                    lblCodiceFAQ.Text = this.areaFAQ.ElencoFAQ[i].Codice;
                    lblDomanda.Text = this.areaFAQ.ElencoFAQ[i].Domanda;
                    if (!(this.areaFAQ.ElencoFAQ[i].Visibilita))
                    {
                        imgbtnNascondiRendiVisibile.ImageUrl = string.Format("../App_Themes/{0}/Images/turn_off.png", Page.Theme);
                        imgbtnNascondiRendiVisibile.ToolTip = "FAQ non visibile. Clicca per modificarne la visibilità.";
                    }
                }
            }
        }

        protected void grdListaFAQ_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            long IdFAQSelezionato = -1;
            string codiceFAQSelezionato = string.Empty;
            if (e.CommandName != "Page")
            {
                int index = ((GridViewRow)(((Control)(e.CommandSource)).NamingContainer)).RowIndex;
                Label lblIdFAQ = (Label)grdListaFAQ.Rows[index].FindControl("lblIdFAQ");
                Label lblCodiceFAQ = (Label)grdListaFAQ.Rows[index].FindControl("lblCodiceFAQ");
                long.TryParse(lblIdFAQ.Text, out IdFAQSelezionato);
                codiceFAQSelezionato = lblCodiceFAQ.Text;
            }

            if (e.CommandName == "Visible")
            {
                AggiornaVisibilita(IdFAQSelezionato);
            }
            else if (e.CommandName == "Delete")
            {
                EliminaFAQ(IdFAQSelezionato);
            }
            else if (e.CommandName == "Update")
            {
                Session["IdFAQEdit"] = IdFAQSelezionato;
                Session["CodiceFAQEdit"] = codiceFAQSelezionato;
                Response.Redirect("FAQEdit.aspx?oper=2");
            }
            else if (e.CommandName == "ShowRisposta")
            {
                hdnTextDialog.Value = e.CommandArgument.ToString();

                ScriptManager.RegisterStartupScript(this, GetType(), "OpenModalDialog", "ShowRisposta();", true);
            }
        }

        protected void grdListaFAQ_onPageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            try
            {
                grdListaFAQ.EditIndex = -1;
                grdListaFAQ.PageIndex = e.NewPageIndex;
                Session["PaginaGestioneFAQ"] = e.NewPageIndex;
                grdListaFAQ_Load();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("GestioneFAQ, Errore nel metodo grdListaFAQ_onPageIndexChanging" + ex);
            }
        }

        protected void grdListaFAQ_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {

        }

        private void grdListaFAQ_Load()
        {
            try
            {
                grdListaFAQ.DataSource = (Presenter.SvrLiquidazione.FAQ[])ViewState["ElencoFAQ"];
                grdListaFAQ.DataBind();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("GestioneFAQ, Errore nel metodo grdListaFAQ_Load " + ex);
            }
        }

        private void AggiornaVisibilita(long idFAQ)
        {
            this.HasError = false;
            this.ErrorMessage = string.Empty;
            if (ucAvviso.Visible)
                ucAvviso.Visible = false;

            this.areaFAQ = new AreaFAQ();
            this.areaFAQ.ElencoFAQ = (Presenter.SvrLiquidazione.FAQ[])ViewState["ElencoFAQ"];
            Presenter.SvrLiquidazione.FAQ faqMod = null;

            foreach (Presenter.SvrLiquidazione.FAQ faq in this.areaFAQ.ElencoFAQ)
                if (faq.Id == idFAQ)
                {
                    faqMod = faq;
                    break;
                }

            if (faqMod != null)
            {
                faqMod.Visibilita = !faqMod.Visibilita; //change visibilità
                this.areaFAQ = new AreaFAQ();
                this.areaFAQ.ElencoFAQ = new Presenter.SvrLiquidazione.FAQ[1];
                this.areaFAQ.ElencoFAQ[0] = faqMod;
                PresenterFaq presenter = new PresenterFaq();
                presenter.SalvaFAQ(this);

                if (!this.HasError)
                {
                    ViewState["ElencoFAQ"] = this.areaFAQ.ElencoFAQ;
                    grdListaFAQ.DataSource = this.areaFAQ.ElencoFAQ;
                    grdListaFAQ.DataBind();
                }
            }
            else
            {
                this.HasError = true;
                this.ErrorMessage = "Si è verificato un errore durante la modifica della FAQ.";
            }

            if (this.HasError)
            {
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = this.ErrorMessage;
                ucAvviso.Tipo = TipoAvviso.Ko;
                PanelFAQ.Visible = false;
                return;
            }
        }

        protected void EliminaFAQ(long IdFAQSelezionato)
        {
            this.HasError = false;
            this.ErrorMessage = string.Empty;
            if (ucAvviso.Visible)
                ucAvviso.Visible = false;

            this.areaFAQ = new AreaFAQ();
            this.areaFAQ.ElencoFAQ = (Presenter.SvrLiquidazione.FAQ[])ViewState["ElencoFAQ"];
            Presenter.SvrLiquidazione.FAQ faqDel = null;

            foreach (Presenter.SvrLiquidazione.FAQ faq in this.areaFAQ.ElencoFAQ)
                if (faq.Id == IdFAQSelezionato)
                {
                    faqDel = faq;
                    break;
                }

            if (faqDel != null)
            {
                this.areaFAQ = new AreaFAQ();
                this.areaFAQ.ElencoFAQ = new Presenter.SvrLiquidazione.FAQ[1];
                this.areaFAQ.ElencoFAQ[0] = faqDel;
                PresenterFaq presenter = new PresenterFaq();
                presenter.DeleteFAQ(this);

                if (!this.HasError)
                {
                    ViewState["ElencoFAQ"] = this.areaFAQ.ElencoFAQ;
                    grdListaFAQ.DataSource = this.areaFAQ.ElencoFAQ;
                    grdListaFAQ.DataBind();
                }
            }
            else
            {
                this.HasError = true;
                this.ErrorMessage = "Si è verificato un errore durante l' eliminazione della FAQ.";
            }

            if (this.HasError)
            {
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = this.ErrorMessage;
                ucAvviso.Tipo = TipoAvviso.Ko;
                PanelFAQ.Visible = false;
                return;
            }
        }

        protected void lnkBtnNuovaDomanda_Click(object sender, EventArgs e)
        {
            Response.Redirect("FAQEdit.aspx?oper=1");
        }

        protected string setImage(string name)
        {
            return string.Format("~/App_Themes/{0}/Images/{1}", Page.Theme ?? "BlueINPS1", name);
        }
    }
}