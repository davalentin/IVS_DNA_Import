using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace INPS.Pensioni.LiquidazionePensione.View.Web.UserControls
{
    public partial class UCAvvisiHomePage : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadingAvvisi.ImageUrl = "../App_Themes/" + Page.Theme + "/Images/ajax-loader.gif";
        }
    }
}