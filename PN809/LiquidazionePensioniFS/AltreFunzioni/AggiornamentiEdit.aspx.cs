using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using INPS.DNA.UI.Web;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using INPS.Pensioni.LiquidazionePensione.View.Web.UserControls;
using INPS.Pensioni.LiquidazionePensione.Presenter;
using INPS.Pensioni.LiquidazionePensione.Presenter.Enum;

namespace INPS.Pensioni.LiquidazionePensione.View.Web
{
    [NavigationRules(AllowedReferrer = AllowedReferrer.Application, CheckSequenceOnPostBack = false)]
    public partial class AggiornamentiEdit : CustomBasePage, IAggiornamenti
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
            if (!IsPostBack)
            {
                int oper = 0; //insert OR update AGGIORNAMENTO
                int.TryParse(Request.QueryString["oper"], out oper);
                long IdAggiornamentoSelectedRow = -1; //id update Aggiornamento
                long.TryParse(Request.QueryString["id"], out IdAggiornamentoSelectedRow);
                switch (oper)
                {
                    case 1:
                        lblIntestazione.Text = "Nuovo aggiornamento";
                        btnAggiorna.Text = "Salva";
                        break;
                    case 2:
                        lblIntestazione.Text = "Modifica aggiornamento";
                        loadInformationsUpdateAggiornamento(IdAggiornamentoSelectedRow);
                        ViewState[EnumViewState.IdAggiornamentoEdit.ToString()] = IdAggiornamentoSelectedRow;
                        btnAggiorna.Text = "Aggiorna";
                        break;
                    default:
                        break;
                }
                ViewState[EnumViewState.OperAggiornamentoEdit.ToString()] = oper;
            }
        }

        private void loadInformationsUpdateAggiornamento(long IdAggiornamentoSelectedRow)
        {
            List<INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione.Aggiornamenti> listaAggiornamenti = CodeUtility.GetAggiornamenti();
            if (listaAggiornamenti == null || listaAggiornamenti.Count == 0)
            {
                this.HasError = true;
                this.ErrorMessage = "Si è verificato un errore durante la modifica dell'aggiornamento.";
            }

            INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione.Aggiornamenti aggiornamentoUpdate = listaAggiornamenti.FirstOrDefault(x => x.Id == IdAggiornamentoSelectedRow);
            if (aggiornamentoUpdate == null)
            {
                this.HasError = true;
                this.ErrorMessage = "Si è verificato un errore durante la modifica dell'aggiornamento.";
            }

            try
            {
                HiddenFieldTitoloAggiornamento.Value = Server.UrlDecode(aggiornamentoUpdate.Titolo);
                HiddenFieldTextEditAggiornamento.Value = Server.UrlDecode(aggiornamentoUpdate.Testo);
                if (aggiornamentoUpdate.Attivo)
                    HiddenFieldVisibleAggiornamento.Value = string.Format("../App_Themes/{0}/Images/turn_on.png", Page.Theme);
                else
                    HiddenFieldVisibleAggiornamento.Value = string.Format("../App_Themes/{0}/Images/turn_off.png", Page.Theme);
                return;
            }
            catch (Exception)
            {
                this.HasError = true;
                this.ErrorMessage = "Si è verificato un errore durante la modifica dell'aggiornamento.";
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
            Response.Redirect("Aggiornamenti.aspx");
        }

        protected void btnAggiorna_Click(object sender, EventArgs e)
        {
            this.HasError = false;
            this.ErrorMessage = string.Empty;
            if (ucAvviso.Visible)
                ucAvviso.Visible = false;

            #region attributi aggiornamento
            long Id = -1;
            string Titolo = string.Empty;
            string Testo = string.Empty;
            DateTime DataAggiornamento = DateTime.Now;
            bool IsVisible = false;

            if ((int)ViewState[EnumViewState.OperAggiornamentoEdit.ToString()] == 2)
                Id = (long)ViewState[EnumViewState.IdAggiornamentoEdit.ToString()];
            Titolo = Server.UrlDecode(HiddenFieldTitoloAggiornamento.Value);
            Testo = Server.UrlDecode(HiddenFieldTextEditAggiornamento.Value);
            IsVisible = HiddenFieldVisibleAggiornamento.Value.Equals("../App_Themes/" + Page.Theme + "/Images/turn_on.png") ? true : false;
            #endregion attributi aggiornamento

            INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione.Aggiornamenti aggsUpdt = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione.Aggiornamenti();
            aggsUpdt.Id = Id;
            aggsUpdt.Titolo = Titolo;
            aggsUpdt.Testo = Testo;
            aggsUpdt.TimeStamp = DataAggiornamento;
            aggsUpdt.Attivo = IsVisible;
            aggsUpdt.Tipologia = Utility.GetTipoAppartenenzaRuolo((Ruoli)Session["Ruolo"]).ToString();

            this.areaAggiornamenti = new AreaAggiornamenti();
            this.areaAggiornamenti.ElencoAggiornamenti = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione.Aggiornamenti[1];
            this.areaAggiornamenti.ElencoAggiornamenti[0] = aggsUpdt;
            PresenterAggiornamenti presenter = new PresenterAggiornamenti();
            presenter.SalvaAggiornamento(this);

            if (!this.HasError)
                CodeUtility.SetAggiornamenti(this.areaAggiornamenti);
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

            if ((int)ViewState[EnumViewState.OperAggiornamentoEdit.ToString()] == 1)
                Response.Redirect("Aggiornamenti.aspx", false);
        }

        protected string setImage(string name)
        {
            return string.Format("~/App_Themes/{0}/Images/{1}", Page.Theme ?? "BlueINPS1", name);
        }

        #region Enum

        public enum EnumViewState
        {
            IdAggiornamentoEdit,
            OperAggiornamentoEdit
        }

        #endregion Enum
    }
}