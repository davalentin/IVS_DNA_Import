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
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using INPS.Pensioni.LiquidazionePensione.Presenter;
using INPS.Pensioni.LiquidazionePensione.View.Web.UserControls;
using INPS.Pensioni.LiquidazionePensione.Presenter.Enum;

namespace INPS.Pensioni.LiquidazionePensione.View.Web
{
    [NavigationRules(AllowedReferrer = AllowedReferrer.Application, CheckSequenceOnPostBack = false)]
    public partial class MessaggiHermesEdit : CustomBasePage, IMessaggiHermes
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
            if (!IsPostBack)
            {
                int oper = 0; //insert OR update MESSAGGIOHERMES
                int.TryParse(Request.QueryString["oper"], out oper);
                long IdMessaggioHermesSelectedRow = -1; //id update MESSAGGIOHERMES
                long.TryParse(Request.QueryString["id"], out IdMessaggioHermesSelectedRow);
                switch (oper)
                {
                    case 1:
                        lblIntestazione.Text = "Nuovo messaggio Hermes";
                        btnAggiorna.Text = "Salva";
                        break;
                    case 2:
                        lblIntestazione.Text = "Modifica messaggio Hermes";
                        loadInformationsUpdateMessaggioHermes(IdMessaggioHermesSelectedRow);
                        ViewState["IdMessaggioHermesEdit"] = IdMessaggioHermesSelectedRow;
                        btnAggiorna.Text = "Aggiorna";
                        break;
                    default:
                        break;
                }
                ViewState["operMessaggioHermesEdit"] = oper;
            }
        }

        private void loadInformationsUpdateMessaggioHermes(long IdMessaggioHermesSelectedRow)
        {
            List<Presenter.SvrLiquidazione.MessaggiHermes> listaMessaggiHermes = CodeUtility.GetMessaggiHermes();
            if (listaMessaggiHermes == null || listaMessaggiHermes.Count == 0)
            {
                this.HasError = true;
                this.ErrorMessage = "Si è verificato un errore durante la modifica del messaggio Hermes.";
            }

            Presenter.SvrLiquidazione.MessaggiHermes messaggioHermesUpdate = listaMessaggiHermes.FirstOrDefault(x => x.Id == IdMessaggioHermesSelectedRow);
            if (messaggioHermesUpdate == null)
            {
                this.HasError = true;
                this.ErrorMessage = "Si è verificato un errore durante la modifica del messaggio Hermes.";
            }

            try
            {
                HiddenFieldTitoloMessaggioHermes.Value = Server.UrlDecode(messaggioHermesUpdate.Titolo);
                HiddenFieldUrlMessaggioHermes.Value = Server.UrlDecode(messaggioHermesUpdate.Url);
                HiddenFieldCategoriaMessaggioHermes.Value = Server.UrlDecode(messaggioHermesUpdate.Categoria);
                HiddenFieldTextEditMessaggioHermes.Value = Server.UrlDecode(messaggioHermesUpdate.Testo);
                if (messaggioHermesUpdate.Attivo)
                    HiddenFieldVisibleMessaggioHermes.Value = string.Format("../App_Themes/{0}/Images/turn_on.png", Page.Theme);
                else
                    HiddenFieldVisibleMessaggioHermes.Value = string.Format("../App_Themes/{0}/Images/turn_off.png", Page.Theme);
                return;
            }
            catch (Exception)
            {
                this.HasError = true;
                this.ErrorMessage = "Si è verificato un errore durante la modifica del messaggio Hermes.";
            }

            if (this.HasError)
            {
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = this.ErrorMessage;
                ucAvviso.Tipo = TipoAvviso.Ko;
                return;
            }
        }


        protected void btnIndietro_Click(object sender, EventArgs e)
        {
            Response.Redirect("MessaggiHermes.aspx");
        }

        protected void btnAggiorna_Click(object sender, EventArgs e)
        {
            this.HasError = false;
            this.ErrorMessage = string.Empty;
            if (ucAvviso.Visible)
                ucAvviso.Visible = false;

            #region attributi messaggio Hermes
            long Id = -1;
            string Titolo = string.Empty;
            string Testo = string.Empty;
            string Url = string.Empty;
            string Categoria = string.Empty;
            DateTime DataMessaggoHermes = DateTime.Now;
            bool IsVisible = false;

            if ((int)ViewState["operMessaggioHermesEdit"] == 2)
                Id = (long)ViewState["IdMessaggioHermesEdit"];
            Titolo = Server.UrlDecode(HiddenFieldTitoloMessaggioHermes.Value);
            Url = Server.UrlDecode(HiddenFieldUrlMessaggioHermes.Value);
            Categoria = Server.UrlDecode(HiddenFieldCategoriaMessaggioHermes.Value);
            Testo = Server.UrlDecode(HiddenFieldTextEditMessaggioHermes.Value);
            IsVisible = HiddenFieldVisibleMessaggioHermes.Value.Equals("../App_Themes/" + Page.Theme + "/Images/turn_on.png") ? true : false;
            #endregion attributi messaggio Hermes

            Presenter.SvrLiquidazione.MessaggiHermes messUpdt = new Presenter.SvrLiquidazione.MessaggiHermes();
            messUpdt.Id = Id;
            messUpdt.Titolo = Titolo;
            messUpdt.Testo = Testo;
            messUpdt.Url = Url;
            messUpdt.Categoria = Categoria;
            messUpdt.TimeStamp = DataMessaggoHermes;
            messUpdt.Attivo = IsVisible;
            messUpdt.Tipologia = Utility.GetTipoAppartenenzaRuolo((Ruoli)Session["Ruolo"]).ToString();

            this.areaMessaggiHermes = new AreaMessaggiHermes();
            this.areaMessaggiHermes.ElencoMessaggiHermes = new Presenter.SvrLiquidazione.MessaggiHermes[1];
            this.areaMessaggiHermes.ElencoMessaggiHermes[0] = messUpdt;
            PresenterMessaggiHermes presenter = new PresenterMessaggiHermes();
            presenter.SalvaMessaggioHermes(this);

            if (!this.HasError)
                CodeUtility.SetMessaggiHermes(this.areaMessaggiHermes);
            else
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

            if ((int)ViewState["operMessaggioHermesEdit"] == 1)
                Response.Redirect("MessaggiHermes.aspx", false);
        }

        protected string setImage(string name)
        {
            return string.Format("~/App_Themes/{0}/Images/{1}", Page.Theme ?? "BlueINPS1", name);
        }
    }
}
