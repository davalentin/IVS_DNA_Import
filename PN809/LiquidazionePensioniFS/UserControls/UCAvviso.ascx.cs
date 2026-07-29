using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace INPS.Pensioni.LiquidazionePensione.View.Web.UserControls
{
    public partial class UCAvviso : System.Web.UI.UserControl
    {


        protected void Page_Load(object sender, EventArgs e)
        {
            base.PreRender += new EventHandler(UCAvviso_PreRender);
        }

        void UCAvviso_PreRender(object sender, EventArgs e)
        {
            if (ShowClose == true)
            {
                imgClose.Visible = true;
            }

            string nomeTema = "~/App_Themes/" + Page.Theme;
            if (Tipo == TipoAvviso.Ok)
            {
                imgIcon.ImageUrl = nomeTema + "/Images/ok.png";
                imgIcon.AlternateText = "Esito OK";
                return;
            }
            if (Tipo == TipoAvviso.Ko)
            {
                imgIcon.ImageUrl = nomeTema + "/Images/ko.png";
                imgIcon.AlternateText = "Esito KO";
                return;
            }
            if (Tipo == TipoAvviso.Info) {
                imgIcon.ImageUrl = nomeTema + "/Images/info.png";
                imgIcon.AlternateText = "Informazione";
                return;
            }

            imgIcon.ImageUrl = nomeTema + "/Images/alert.png";
            imgIcon.AlternateText = "Attenzione";
        }

        protected void closeToast(object sender, EventArgs args)
        {
            CloseToastEvt(sender, args);
        }


        public TipoAvviso Tipo
        {
            get
            {
                if (ViewState["TipoAvviso"] == null)
                {
                    ViewState["TipoAvviso"] = TipoAvviso.Warning;
                }
                return (TipoAvviso)ViewState["TipoAvviso"];
            }
            set
            {
                ViewState["TipoAvviso"] = value;
            }
        }

        public bool ShowClose
        {
            get
            {
                object value = ViewState["ShowClose"];
                if (value == null)
                    return false;

                return (bool)value;
            }
            set
            {
                ViewState["ShowClose"] = value;
            }
        }

        public string Messaggio
        {
            get
            {
                return lblMsg.Text;
            }
            set
            {
                lblMsg.Text = value;
            }
        }

        public string Titolo
        {
            get
            {
                return lblTitle.Text;
            }
            set
            {
                lblTitle.Text = value;
            }
        }

        public event EventHandler CloseToastEvt;
    }

    public enum TipoAvviso { Ok, Ko, Warning, Info };
}