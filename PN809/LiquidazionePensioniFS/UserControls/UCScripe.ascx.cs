using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace INPS.Pensioni.LiquidazionePensione.View.Web.UserControls
{
    public partial class UCScripe : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            SetSCRIPE();
        }
        private void SetSCRIPE()
        {
            if (ConfigurationManager.AppSettings["SCRIPE"] != null)
            {
                HtmlGenericControl scripeInclude = new HtmlGenericControl("script");
                scripeInclude.Attributes.Add("type", "text/javascript");
                scripeInclude.Attributes.Add("src", ConfigurationManager.AppSettings["SCRIPE"]);
                this.Controls.Add(scripeInclude);
            }
        }
    }
}