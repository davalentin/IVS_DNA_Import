using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using INPS.Pensioni.LiquidazionePensione.Presenter.Enum;
using INPS.Pensioni.LiquidazionePensione.Presenter;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using System.Web.UI.HtmlControls;
using System.IO;

namespace INPS.Pensioni.LiquidazionePensione.View.Web
{
    public partial class Liquidazione : System.Web.UI.MasterPage
    {
        public string BgTheme;
        public string AppTheme;

        protected void Page_Load(object sender, EventArgs e)
        {
            if ((bool)Session["isIframe"])
            {
                this.tblIntestazione.Visible = false;
                this.UcTestata.Visible = false;
                this.Footer.Visible = false;
                this.UCIntestazione.Visible = false;
                this.container.Attributes["class"] = "main-container--iframe main-container";
            }
            else
            {
                this.container.Attributes["class"] = "main-container";
            }

            if (ConfigurationManager.AppSettings["NascondiIntestazione"] != null &&
                ConfigurationManager.AppSettings["NascondiIntestazione"] == "SI")
            {
                this.tblIntestazione.Visible = false;
                this.TopBarImages.Visible = false;
            }

            if (!IsPostBack)
            {
                UcTestata.ValorizzaHiddenField("Liquidazione");
            }
            SetSCRIPE();

            AppTheme = Page.Theme;
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            UCMenu.GetSemaforo();
            Boolean lavorabile = true;
            if (Session["Lavorabile"] != null)
            {
                lavorabile = (Boolean)Session["Lavorabile"];
            }

            if (lavorabile == false)
            {
                UCMenu.DisabilitaDomanda();
            }

            if (Session["Domanda"] != null)
            {
                AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
                if (CodeUtility.IsConsultazione(domanda, Session["Ruolo"]) && !CodeUtility.ControlloNavigazioneCalcoloNoIndEditabile(domanda, this.Page))
                    CodeUtility.BloccaForm((AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"], this);
            }

            short count = 1;
            CodeUtility.SetTabIndex(this, ref count);
        }

        private void SetSCRIPE()
        {
            if (ConfigurationManager.AppSettings["SCRIPE"] != null)
            {
                HtmlGenericControl scripeInclude = new HtmlGenericControl("script");
                scripeInclude.Attributes.Add("type", "text/javascript");
                scripeInclude.Attributes.Add("src", ConfigurationManager.AppSettings["SCRIPE"]);
                scripeInclude.Attributes.Add("async", "async");
                this.Head1.Controls.Add(scripeInclude);
            }
        }
    }
}
