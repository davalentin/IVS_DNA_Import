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

namespace INPS.Pensioni.LiquidazionePensione.View.Web
{
    [NavigationRules(AllowedReferrer = AllowedReferrer.Application, CheckSequenceOnPostBack = false)]
	public partial class AvvisiEdit : CustomBasePage, IAvvisi
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
			if (!IsPostBack)
			{
				int oper = 0; //insert OR update AVVISO
				int.TryParse(Request.QueryString["oper"], out oper);
				long IdAvvisoSelectedRow = -1; //id update Avviso
                long.TryParse(Request.QueryString["id"], out IdAvvisoSelectedRow);
				switch (oper)
				{
					case 1:
						lblIntestazione.Text = "Nuovo avviso";
						btnAggiorna.Text = "Salva";
						break;
					case 2:
						lblIntestazione.Text = "Modifica avviso";
						loadInformationsUpdateAvviso(IdAvvisoSelectedRow);
						ViewState["IdAvvisoEdit"] = IdAvvisoSelectedRow;
						btnAggiorna.Text = "Aggiorna";
						break;
					default:
						break;
				}
				ViewState["operAvvisoEdit"] = oper;
			}
		}

		private void loadInformationsUpdateAvviso(long IdAvvisoSelectedRow)
		{
            List<INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione.Avvisi> listaAvvisi = CodeUtility.GetAvvisi();
			if (listaAvvisi == null || listaAvvisi.Count == 0)
			{
				this.HasError = true;
				this.ErrorMessage = "Si è verificato un errore durante la modifica dell'avviso.";
			}

            INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione.Avvisi avvisoUpdate = listaAvvisi.FirstOrDefault(x => x.Id == IdAvvisoSelectedRow);
            if (avvisoUpdate == null)
            {
                this.HasError = true;
                this.ErrorMessage = "Si è verificato un errore durante la modifica dell'avviso.";
            }

			try
			{
                HiddenFieldTitoloAvviso.Value = Server.UrlDecode(avvisoUpdate.Titolo);
                HiddenFieldTextEditAvviso.Value = Server.UrlDecode(avvisoUpdate.Testo);
                if (avvisoUpdate.Attivo)
                    HiddenFieldVisibleAvviso.Value = string.Format("../App_Themes/{0}/Images/turn_on.png", Page.Theme);
				else
                    HiddenFieldVisibleAvviso.Value = string.Format("../App_Themes/{0}/Images/turn_off.png", Page.Theme);
				return;
			}
			catch (Exception)
			{
				this.HasError = true;
				this.ErrorMessage = "Si è verificato un errore durante la modifica dell'avviso.";
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
			Response.Redirect("Avvisi.aspx");
		}

		protected void btnAggiorna_Click(object sender, EventArgs e)
		{
			this.HasError = false;
			this.ErrorMessage = string.Empty;
			if (ucAvviso.Visible)
				ucAvviso.Visible = false;

			#region attributi avviso
			long Id = -1;
			string Titolo = string.Empty;
			string Testo = string.Empty;
			DateTime DataAvviso = DateTime.Now;
			bool IsVisible = false;

			if ((int)ViewState["operAvvisoEdit"] == 2)
				Id = (long)ViewState["IdAvvisoEdit"];
			Titolo = Server.UrlDecode(HiddenFieldTitoloAvviso.Value);
			Testo = Server.UrlDecode(HiddenFieldTextEditAvviso.Value);
            IsVisible = HiddenFieldVisibleAvviso.Value.Equals("../App_Themes/" + Page.Theme + "/Images/turn_on.png") ? true : false;
			#endregion attributi avviso

			INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione.Avvisi avvsUpdt = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione.Avvisi();
            avvsUpdt.Id = Id;
            avvsUpdt.Titolo = Titolo;
            avvsUpdt.Testo = Testo;
            avvsUpdt.TimeStamp = DataAvviso;
            avvsUpdt.Attivo = IsVisible;
            avvsUpdt.Tipologia = Utility.GetTipoAppartenenzaRuolo((Ruoli)Session["Ruolo"]).ToString();

            this.areaAvvisi = new AreaAvvisi();
            this.areaAvvisi.ElencoAvvisi = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione.Avvisi[1];
            this.areaAvvisi.ElencoAvvisi[0] = avvsUpdt;
            PresenterAvvisi presenter = new PresenterAvvisi();
            presenter.SalvaAvviso(this);

            if (!this.HasError)
                CodeUtility.SetAvvisi(this.areaAvvisi);
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

            if ((int)ViewState["operAvvisoEdit"] == 1)
                Response.Redirect("Avvisi.aspx", false);
		}

		protected string setImage(string name)
		{
			return string.Format("~/App_Themes/{0}/Images/{1}", Page.Theme ?? "BlueINPS1", name);
		}
	}
}
