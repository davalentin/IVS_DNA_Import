using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using INPS.DNA.UI.Web;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.View.Web.UserControls;
using INPS.Pensioni.LiquidazionePensione.Presenter;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using INPS.Pensioni.LiquidazionePensione.Presenter.Enum;

namespace INPS.Pensioni.LiquidazionePensione.View.Web
{
    [NavigationRules(AllowedReferrer = AllowedReferrer.Application, CheckSequenceOnPostBack = false)]
    public partial class FAQEdit : CustomBasePage, IFaq
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

            if (!IsPostBack)
            {
                int oper = 0; //insert OR update FAQ
                int.TryParse(Request.QueryString["oper"], out oper);
                long IdFAQSelectedRow = -1; //id update FAQ
                if(Session["IdFAQEdit"] != null)
                    IdFAQSelectedRow = (long)Session["IdFAQEdit"];
                Session.Remove("IdFAQEdit");
                string codiceFAQSelectedRow = string.Empty;
                if (Session["CodiceFAQEdit"] != null)
                    codiceFAQSelectedRow = (string)Session["CodiceFAQEdit"];
                Session.Remove("CodiceFAQEdit");

                PresenterFaq presenter = new PresenterFaq();
                presenter.GetFAQ(this);

                LoadDdl();

                switch (oper)
                {
                    case 1:
                        Session.Remove("PaginaGestioneFAQ");
                        lblIntestazione.Text = "Nuova FAQ";
                        btnAggiorna.Text = "Salva";
                        break;
                    case 2:
                        lblIntestazione.Text = "Modifica FAQ";
                        LoadInformationsUpdateFAQ(IdFAQSelectedRow);
                        ViewState["IdFAQEdit"] = IdFAQSelectedRow;
                        ViewState["CodiceFAQEdit"] = codiceFAQSelectedRow;
                        btnAggiorna.Text = "Aggiorna";
                        break;
                    default:
                        break;
                }
                ViewState["operFAQEdit"] = oper;
            }
        }

        private void LoadInformationsUpdateFAQ(long IdFAQSelectedRow)
        {
            if (this.areaFAQ.ElencoFAQ == null || this.areaFAQ.ElencoFAQ.Count() == 0)
            {
                this.HasError = true;
                this.ErrorMessage = "Si è verificato un errore durante la modifica del FAQ.";
            }

            INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione.FAQ faqUpdate = this.areaFAQ.ElencoFAQ.FirstOrDefault(x => x.Id == IdFAQSelectedRow);
            if (faqUpdate == null)
            {
                this.HasError = true;
                this.ErrorMessage = "Si è verificato un errore durante la modifica del FAQ.";
            }
            else
            {
                txtDomanda.Text = faqUpdate.Domanda;
                txtRisposta.Text = faqUpdate.Risposta;
                ddlTipologia.SelectedValue = faqUpdate.Tipologia;
                if (faqUpdate.Visibilita)
                    HiddenFieldVisibleFAQ.Value = string.Format("../App_Themes/{0}/Images/turn_on.png", Page.Theme);
                else
                    HiddenFieldVisibleFAQ.Value = string.Format("../App_Themes/{0}/Images/turn_off.png", Page.Theme);
            }

            if (this.HasError)
            {
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = this.ErrorMessage;
                ucAvviso.Tipo = TipoAvviso.Ko;
                return;
            }
        }

        private void LoadDdl()
        {
            if (this.areaFAQ != null && this.areaFAQ.ElencoTipologiaFAQ != null && this.areaFAQ.ElencoTipologiaFAQ.Count() > 0)
            {
                CodeUtility.SetValueDdl(ddlTipologia, string.Empty, string.Empty);
                foreach (TipologiaFAQ tipologiaFAQ in this.areaFAQ.ElencoTipologiaFAQ)
                    CodeUtility.SetValueDdl(ddlTipologia, tipologiaFAQ.Descrizione, tipologiaFAQ.Descrizione, tipologiaFAQ.Codice);
            }
        }

        protected void btnIndietro_Click(object sender, EventArgs e)
        {
            Response.Redirect("GestioneFAQ.aspx");
        }

        protected void btnAggiorna_Click(object sender, EventArgs e)
        {
            this.HasError = false;
            this.ErrorMessage = string.Empty;
            if (ucAvviso.Visible)
                ucAvviso.Visible = false;

            #region attributi
            long id = -1;
            string codice = string.Empty;

            if ((int)ViewState["operFAQEdit"] == 2)
            {
                id = (long)ViewState["IdFAQEdit"];
                codice = (string)ViewState["CodiceFAQEdit"];
            }
            #endregion attributi

            INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione.FAQ faqUpdt = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione.FAQ();
            faqUpdt.Id = id;
            faqUpdt.Domanda = txtDomanda.Text.Replace("\r\n", " ");
            faqUpdt.Risposta = txtRisposta.Text;
            faqUpdt.TipoApp = Utility.GetTipoAppartenenzaRuolo((Ruoli)Session["Ruolo"]).ToString();
            faqUpdt.Tipologia = ddlTipologia.SelectedValue;
            faqUpdt.Visibilita = HiddenFieldVisibleFAQ.Value.Equals("../App_Themes/" + Page.Theme + "/Images/turn_on.png") ? true : false;
            if (!string.IsNullOrEmpty(codice))
                faqUpdt.Codice = codice.Substring(0, 3) == ddlTipologia.SelectedValue ? codice : string.Empty;

            this.areaFAQ = new AreaFAQ();
            this.areaFAQ.ElencoFAQ = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione.FAQ[1];
            this.areaFAQ.ElencoFAQ[0] = faqUpdt;
            PresenterFaq presenter = new PresenterFaq();
            presenter.SalvaFAQ(this);

            if (this.HasError)
            {
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = this.ErrorMessage;
                ucAvviso.Tipo = TipoAvviso.Ko;
                return;
            }

            this.HasError = true;
            this.ErrorMessage = "Operazione eseguita con successo.";
            ucAvviso.Visible = true;
            ucAvviso.Messaggio = this.ErrorMessage;
            ucAvviso.Tipo = TipoAvviso.Ok;

            if ((int)ViewState["operFAQEdit"] == 1)
                Response.Redirect("GestioneFAQ.aspx", false);
        }

        protected string setImage(string name)
        {
            return string.Format("~/App_Themes/{0}/Images/{1}", Page.Theme ?? "BlueINPS1", name);
        }
    }
}