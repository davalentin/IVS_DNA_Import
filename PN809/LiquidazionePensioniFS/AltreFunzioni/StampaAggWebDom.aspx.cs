using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;

namespace INPS.Pensioni.LiquidazionePensione.View.Web
{
    public partial class StampaAggWebDom : System.Web.UI.Page, IAggiornamentoWebDom
    {
        #region IAggiornamentoWebDom Members
        public Presenter.SvrLiquidazione.UtilityTipoAppartenenza? TipoApp { get; set; }
        public AreaAggiornamentoWebDom areaAggiornamentoWebDom { get; set; }
        #endregion

        #region IViewUI Members
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                TipoApp = null;
                if (Server.HtmlEncode(Request.QueryString["TipoApp"]) != null)
                {
                    switch (Server.HtmlEncode(Request.QueryString["TipoApp"]))
                    {
                        case "FS":
                            TipoApp = Presenter.SvrLiquidazione.UtilityTipoAppartenenza.FS;
                            break;
                        case "AGO":
                            TipoApp = Presenter.SvrLiquidazione.UtilityTipoAppartenenza.AGO;
                            break;
                        case "CI":
                            TipoApp = Presenter.SvrLiquidazione.UtilityTipoAppartenenza.CI;
                            break;
                    }
                }
                else
                {
                    lblErrore.Visible = true;
                    lblErrore.Text = "Tipo appartenenza non disponibile";
                }

                if (TipoApp != null)
                {
                    PresenterAggiornamentoWebDom presenter = new PresenterAggiornamentoWebDom();
                    presenter.CaricaPdfAggiornamentoWebDom(this);

                    if (this.areaAggiornamentoWebDom != null && this.areaAggiornamentoWebDom.PdfDoc != null)
                    {
                        byte[] bytes = this.areaAggiornamentoWebDom.PdfDoc.ToArray();

                        string filename = "AggiornamentoWebDom_" + TipoApp.ToString() + ".pdf";
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
                        lblErrore.Text = "Nessuna domanda con esito negativo.";
                    }
                }
            }
        }
    }
}