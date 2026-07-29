using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using INPS.Pensioni.LiquidazionePensione.Presenter;
using System.Configuration;

namespace INPS.Pensioni.LiquidazionePensione.View.Web
{
    public partial class UnicarpeTest : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
            }
        }

        public void btnLIQPENS_Click(object sender, EventArgs e)
        {
            string sede = Crypt.Encrypt(txtSede.Text);
            string nDomus = Crypt.Encrypt(txtDomus.Text);
            string CO = Crypt.Encrypt(txtCO.Text);

            Response.Redirect(ConfigurationManager.AppSettings["UNICARPE-TEST"].ToString() + "Unicarpe.aspx?Sede=" + sede + "&CentroOperativo=" + CO + "&NumDomus=" + nDomus);
        }
    }
}