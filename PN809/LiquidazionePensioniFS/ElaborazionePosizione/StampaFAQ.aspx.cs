using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter;
using System.IO;

namespace INPS.Pensioni.LiquidazionePensione.View.Web
{
    public partial class StampaFAQ : Page, IFaq
    {
        #region IFaq Members
        public Presenter.SvrLiquidazione.AreaFAQ areaFAQ { get; set; }
        public Presenter.SvrLiquidazione.UtilityTipoAppartenenza? tipoApp { get; set; }
        #endregion IFaq Members

        #region IViewUI Members
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        #endregion IViewUI Members

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                tipoApp = null;
                if (Server.HtmlEncode(Request.QueryString["TipoApp"]) != null)
                {
                    switch (Server.HtmlEncode(Request.QueryString["TipoApp"]))
                    {
                        case "FS":
                            tipoApp = Presenter.SvrLiquidazione.UtilityTipoAppartenenza.FS;
                            break;
                        case "AGO":
                            tipoApp = Presenter.SvrLiquidazione.UtilityTipoAppartenenza.AGO;
                            break;
                        case "CI":
                            tipoApp = Presenter.SvrLiquidazione.UtilityTipoAppartenenza.CI;
                            break;
                    }
                }
                else
                {
                    lblErrore.Visible = true;
                    lblErrore.Text = "Tipo appartenenza non disponibile";
                }

                if (tipoApp != null)
                {
                    PresenterFaq presenter = new PresenterFaq();
                    presenter.CaricaPdfFAQ(this);

                    if (this.areaFAQ != null && this.areaFAQ.PdfDoc != null)
                    {
                        byte[] bytes = this.areaFAQ.PdfDoc.ToArray();

                        string filename = "FAQ_" + tipoApp.ToString() + ".pdf";
                        Response.Clear();
                        Response.ContentType = "application/pdf";
                        Response.AppendHeader("Content-Disposition", "inline; filename=" + filename);
                        Response.AppendHeader("Content-Length", bytes.Length.ToString());
                        Response.AppendHeader("Accept-Ranges", "bytes");
                        Response.BinaryWrite(bytes);
                        Response.Flush();
                        Response.End();
                    }
                    else
                    {
                        lblErrore.Visible = true;
                        lblErrore.Text = "Nessuna FAQ presente";
                    }
                }
            }
        }
    }
}