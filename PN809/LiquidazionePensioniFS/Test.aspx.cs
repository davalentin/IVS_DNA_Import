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
    public partial class Test : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
            }
        }

        public void btnLIQPENS_U_Click(object sender, EventArgs e)
        {
            string sede = Crypt.Encrypt(txtSede.Text);
            string nDomus = Crypt.Encrypt(txtDomus.Text);
            string CO = Crypt.Encrypt(txtCO.Text);

            Response.Redirect(ConfigurationManager.AppSettings["LINK-TEST"] + "Unicarpe.aspx?Sede=" + sede + "&CentroOperativo=" + CO + "&NumDomus=" + nDomus);
        }

        public void btnLIQPENS_W_Click(object sender, EventArgs e)
        {
            //Per WebDom non c'è criptaggio della query string
            //string sede = Crypt.Encrypt(txtSede.Text);
            //string nDomus = Crypt.Encrypt(txtDomus.Text);
            //string CO = Crypt.Encrypt(txtCO.Text);
            string sede = txtSede.Text;
            string nDomus = txtDomus.Text;
            string CO = txtCO.Text;

            Response.Redirect(ConfigurationManager.AppSettings["LINK-TEST"] + "WebDom.aspx?Sede=" + sede + "&CentroOperativo=" + CO + "&NumDomus=" + nDomus);
        }

        public void btnLIQPENS_S_Click(object sender, EventArgs e)
        {
            //Per WebDom non c'è criptaggio della query string
            //string sede = Crypt.Encrypt(txtSede.Text);
            //string nDomus = Crypt.Encrypt(txtDomus.Text);
            //string CO = Crypt.Encrypt(txtCO.Text);
            string sede = txtSede.Text;
            string nDomus = txtDomus.Text;
            string CO = txtCO.Text;

            Response.Redirect(ConfigurationManager.AppSettings["LINK-TEST"] + "SistemaUnico.aspx?Sede=" + sede + "&CentroOperativo=" + CO + "&NumDomus=" + nDomus);
        }

        public void btnLIQPENS_P_Click(object sender, EventArgs e)
        {
            string sede = Crypt.Encrypt(txtSede.Text);
            string nDomus = Crypt.Encrypt(txtDomus.Text);
            string CO = Crypt.Encrypt(txtCO.Text);

            Response.Redirect(ConfigurationManager.AppSettings["LINK-TEST"] + "Previsan.aspx?Sede=" + sede + "&CentroOperativo=" + CO + "&NumDomus=" + nDomus);
        }

        public void btnLIQPENS_SCRIWO_Click(object sender, EventArgs e)
        {
            string sede = txtSede.Text;
            string nDomus = txtDomus.Text;
            string CO = txtCO.Text;
            string gestione = txtGestione.Text;
            string indConvInt = chkIndConvInt.Checked ? "1" : "0";
            string tipoVisualizzazione = ddlTipoVisualizzazione.SelectedValue;

            Response.Redirect(ConfigurationManager.AppSettings["LINK-TEST"] + "Scriwo.aspx?Sede=" + sede + "&CentroOperativo=" + CO + "&NumDomus=" + nDomus + "&Gestione=" + gestione + "&IndConvInt=" + indConvInt + "&Redir=" + tipoVisualizzazione);
        }
    }
}