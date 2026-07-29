using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace INPS.Pensioni.LiquidazionePensione.View.Web.UserControls
{
    public partial class UCFooter : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            if (!IsPostBack)
            {
                hValutazione.Value = ConfigurationManager.AppSettings["QuestionarioValutazione"];
                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["QuestionarioValutazione"]))
                    hValutazione.Value = ConfigurationManager.AppSettings["QuestionarioValutazione"];
            }

        }
    }
}