using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using INPS.DNA.UI.Web;
using INPS.Pensioni.LiquidazionePensione.Presenter;
using INPS.Pensioni.LiquidazionePensione.Presenter.Enum;

namespace INPS.Pensioni.LiquidazionePensione.View.Web.UserControls
{
    public partial class UCChangeRuolo : CustomBaseUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (CodeUtility.IsMultiRuolo())
            {
                btnChangeRuolo.ImageUrl = "~/App_Themes/" + Page.Theme + "/Images/reload.png";
            }
            else
            {
                btnChangeRuolo.Visible = false;
            }
            if (btnChangeRuolo.Visible)
            {
                switch (GetCurrentPageName())
                {
                    case "Default.aspx":
                    case "default.aspx":
                    case "SceltaRuolo.aspx":
                        break;
                    default:
                        btnChangeRuolo.Visible = false;
                        break;
                }
            }
            FillFields();
        }

        public void ReloadControl()
        {
            FillFields();
        }

        private void FillFields()
        {
            if (Session["Ruolo"] != null)
            {
                lblRuolo.Text = Utility.GetDescription((Ruoli)Session["Ruolo"]);
                pnlChgRuolo.Visible = true;
            }
            else
            {
                pnlChgRuolo.Visible = false;
            }
        }

        protected void btnChangeRuolo_Click(object sender, ImageClickEventArgs e)
        {
            Session.Remove("Ruolo");
            Response.Redirect("SceltaRuolo.aspx");
        }

        private string GetCurrentPageName()
        {
            string sPath = System.Web.HttpContext.Current.Request.Url.AbsolutePath;
            System.IO.FileInfo oInfo = new System.IO.FileInfo(sPath);
            string sRet = oInfo.Name;
            return sRet;
        }
    }
}