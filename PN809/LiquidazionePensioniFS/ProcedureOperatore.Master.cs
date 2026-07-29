using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using INPS.Pensioni.LiquidazionePensione.Presenter.Enum;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using INPS.Pensioni.LiquidazionePensione.Presenter;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using System.Web.UI.HtmlControls;

namespace INPS.Pensioni.LiquidazionePensione.View.Web
{
    public partial class ProcedureOperatore : System.Web.UI.MasterPage
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if ((bool)Session["isIframe"])
                {
                    this.tblIntestazione.Visible = false;
                    this.Header.Visible = false;
                    this.Footer.Visible = false;
                    this.user.Visible = false;
                    this.leftside.Visible = false;
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

                UcTestata.ValorizzaHiddenField("ProcedureOperatore");

                if (System.IO.Path.GetFileName(this.Request.PhysicalPath) != "ConfermaAcquisizione.aspx")
                {
                    if (Session["URLDPI"] != null)
                        Session.Remove("URLDPI");
                }
            }
        }
    }
}
