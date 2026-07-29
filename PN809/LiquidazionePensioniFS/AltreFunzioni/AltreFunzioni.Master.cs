using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using System.Web.UI.HtmlControls;

namespace INPS.Pensioni.LiquidazionePensione.View.Web
{
    public partial class AltreFunzioni : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if ((bool)Session["isIframe"])
            {
                this.tblIntestazione.Visible = false;
                this.user.Visible = false;
                this.Header.Visible = false;
                this.Footer.Visible = false;
                this.leftside.Visible = false;
                this.container.Attributes["class"] = "main-container--iframe main-container";
            } else
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
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
           
        }
    }
}

