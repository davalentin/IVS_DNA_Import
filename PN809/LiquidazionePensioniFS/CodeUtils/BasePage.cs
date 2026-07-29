using INPS.DNA.UI.Web.Intranet;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace INPS.Pensioni.LiquidazionePensione.View.Web
{
    public class BasePage : BaseViewPage
    {
        protected void Page_PreInit(object sender, EventArgs e)
        {

            if ((Session["IsSistemaUnico"] as bool?).GetValueOrDefault() || CodeUtility.GetCurrentPageName() == "SistemaUnico.aspx")
            {
                this.Page.Theme = "SistemaUnico";
                //Response.Headers.Remove("X-UA-Compatible");
            }
            else
            {
                //Session["isSirio"] = true;
                //if (Session["isSirio"] == null)
                //{
                //    var sirioParam = HttpContext.Current?.Request?.QueryString?["sirio"];
                //    bool isExplicitOldTheme = string.Equals(sirioParam, "0", StringComparison.Ordinal);
                //    Session["isSirio"] = !isExplicitOldTheme;
                //}
                if (Session["isSirio"] == null) Session["isSirio"] = false;

                if (Session["isIframe"] == null)
                {
                    Session["isIframe"] = IsTruthy(HttpContext.Current.Request.QueryString["iFrame"]);
                }

                this.Page.Theme = (bool)Session["isSirio"] ? "iFrame" : "BlueINPS1";
            }
        }

        private static bool IsTruthy(string value)
        {
            if (string.IsNullOrEmpty(value) || value.Trim().Length == 0)
                return false;

            value = value.Trim();

            bool b;
            if (bool.TryParse(value, out b))
                return b;

            int i;
            if (int.TryParse(value, out i))
                return i == 1;

            return false;
        }

    }
}