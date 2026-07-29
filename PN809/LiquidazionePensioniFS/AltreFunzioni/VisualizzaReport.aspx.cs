using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using Microsoft.Reporting.WebForms;
using System.Security.Principal;
using System.Net;
using System.Reflection;
using INPS.Pensioni.LiquidazionePensione.Presenter;
using INPS.Pensioni.LiquidazionePensione.Presenter.Enum;

namespace INPS.Pensioni.LiquidazionePensione.View.Web
{
    public partial class VisualizzaReport : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                string nameReport = string.Empty;

                if (Server.HtmlEncode(Request.QueryString["Name"]) != null)
                {
                    nameReport = this.Request.QueryString["Name"];
                }

                ReportViewer1.Reset();
                CreaReport(nameReport);
                Presenter.LogSicurezza.ScritturaLog("REP", null, int.Parse(ConfigurationManager.AppSettings["IDEVENTO-GEN-REPORT"]),
                    HttpContext.Current.Request.UserHostAddress, 0, string.Empty, string.Empty, string.Empty);
            }
        }

        private void CreaReport(string nameReport)
        {
            List<ReportParameter> listParam = new List<ReportParameter>();

            CreaReportCommon(ref ReportViewer1, nameReport, ref listParam);

            ValorizzaParametriOpzionali(ref listParam);

            try
            {
                ReportViewer1.ServerReport.SetParameters(listParam);
            }
            catch (Exception Ex)
            {
                INPS.DNA.Logging.Logger.LogException(Ex);
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alert", "alert('Si è verificato un errore durante la generazione del report. Si prega di riprovare.')", true);
                btnExportReport.Visible = false;
                return;
            }

            ReportViewer1.ZoomPercent = 100;
            ReportViewer1.ServerReport.Timeout = 100000;
            ReportViewer1.ServerReport.Refresh();
        }

        protected void btnExportReport_Click(object sender, EventArgs e)
        {
            /// Ho creato un nuovo ReportViewer perchè mi serve aggiungere un parametro
            /// Se usavo il vecchio ReportViewer mi dava problemi di navigazione nella finestra generata dal metodo CreaReport()
            #region nuovo ReportViewer

            string nameReport = string.Empty;

            if (Server.HtmlEncode(Request.QueryString["Name"]) != null)
            {
                nameReport = this.Request.QueryString["Name"];
            }

            ReportViewer reportViewerExport = new ReportViewer();

            reportViewerExport.Reset();

            List<ReportParameter> listParam = new List<ReportParameter>();

            CreaReportCommon(ref reportViewerExport, nameReport, ref listParam);

            ReportParameter param;
            param = new ReportParameter("MatricolaOperatore", ((INPS.DNA.Security.Idm.IdmIdentity)INPS.DNA.Security.DnaPrincipal.CurrentIdentity).Matricula);
            listParam.Add(param);

            ValorizzaParametriOpzionali(ref listParam);

            try
            {
                reportViewerExport.ServerReport.SetParameters(listParam);
            }
            catch (Exception Ex)
            {
                INPS.DNA.Logging.Logger.LogException(Ex);
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alert", "alert('Si è verificato un errore durante la generazione del report. Si prega di riprovare.')", true);
                return;
            }

            #endregion nuovo ReportViewer

            try
            {
                string mimeType;
                string encoding;
                string extension;
                string[] streamids;
                Warning[] warnings;

                //Esporto in EXCEL il report
                byte[] result = reportViewerExport.ServerReport.Render("EXCEL", null, out mimeType, out encoding, out extension, out streamids, out warnings);

                Presenter.LogSicurezza.ScritturaLog("XLS", null, int.Parse(ConfigurationManager.AppSettings["IDEVENTO-ESP-REPORT"]),
                    HttpContext.Current.Request.UserHostAddress, 0, string.Empty, string.Empty, string.Empty);

                Response.Clear();
                Response.ContentType = "application/xls";


                String NomeFile = string.Format("{0}_{1}", DateTime.Today.ToString("[dd_MM_yyyy]"), this.Request.QueryString["Name"]);

                Response.AddHeader("Content-disposition", "attachment; filename=" + NomeFile + ".xls");
                Response.BinaryWrite(result);
                Response.Flush();
                Response.End();
            }
            catch (Exception Ex)
            {
                INPS.DNA.Logging.Logger.LogException(Ex);
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alert", "alert('Si è verificato un errore durante la generazione del report. Si prega di riprovare.')", true);
                return;
            }
        }

        private void ValorizzaParametriOpzionali(ref List<ReportParameter> listParam)
        {
            ReportParameter param;

            if (Server.HtmlEncode(Request.QueryString["CSD"]) != null && this.Request.QueryString["CSD"] == "S")
            {
                param = new ReportParameter("CodiceSedeDestinazione", "1");
                listParam.Add(param);
            }
            if (Server.HtmlEncode(Request.QueryString["DPR"]) != null && this.Request.QueryString["DPR"] == "S")
            {
                param = new ReportParameter("DataPerfezionamentoRequisiti", "1");
                listParam.Add(param);
            }
            if (Server.HtmlEncode(Request.QueryString["MU"]) != null && this.Request.QueryString["MU"] == "S")
            {
                param = new ReportParameter("MatricolaUtenteAcquisizione", "1");
                listParam.Add(param);
            }
            if (Server.HtmlEncode(Request.QueryString["DO"]) != null && this.Request.QueryString["DO"] == "S")
            {
                param = new ReportParameter("DecorrenzaOriginaria", "1");
                listParam.Add(param);
            }
            if (Server.HtmlEncode(Request.QueryString["FU"]) != null && this.Request.QueryString["FU"] == "S")
            {
                param = new ReportParameter("FlagUnicarpe", "1");
                listParam.Add(param);
            }
        }

        private void CreaReportCommon(ref ReportViewer report, string nameReport, ref List<ReportParameter> listParam)
        {
            if (!ConfigurationManager.AppSettings.AllKeys.Contains("ReportUri"))
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alert", "alert('Funzionalità al momento non disponibile. Si prega di riprovare.')", true);
                btnExportReport.Visible = false;
                return;
            }

            report.ProcessingMode = ProcessingMode.Remote;
            report.ServerReport.ReportServerUrl = new Uri(ConfigurationManager.AppSettings["ReportUri"]);

            if (ConfigurationManager.AppSettings.AllKeys.Contains("ReportUserName"))
                report.ServerReport.ReportServerCredentials = new MyConfigFileCredentials();

            if (!String.IsNullOrEmpty(ConfigurationManager.AppSettings["ReportFolder"]))
            {
                string pathReport = ConfigurationManager.AppSettings["ReportFolder"];
                if (!pathReport.Substring(0, 1).Equals("/"))
                {
                    pathReport = "/" + pathReport;
                }
                if (!pathReport.Substring(pathReport.Length - 1, 1).Equals("/"))
                {
                    pathReport += "/";
                    report.ServerReport.ReportPath = pathReport;
                }
                else
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alert", "alert('Si è verificato un errore durante l'esportazione della lista. Si prega di riprovare.')", true);
                    btnExportReport.Visible = false;
                    return;
                }
            }
            else
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alert", "alert('Non è stato possibile recuperare la cartella dei Report. Si prega di riprovare.')", true);
                btnExportReport.Visible = false;
                return;
            }

            report.ServerReport.ReportPath += nameReport;

            ReportParameter param;
            param = new ReportParameter("Tipo", Utility.GetDescription(Utility.GetTipoAppartenenzaRuolo((Ruoli)Session["Ruolo"])));
            listParam.Add(param);
        }
    }

    [Serializable]
    public sealed class MyConfigFileCredentials : IReportServerCredentials
    {
        public WindowsIdentity ImpersonationUser
        {
            get { return null; }
        }

        public ICredentials NetworkCredentials
        {
            get
            {
                return new System.Net.NetworkCredential(
                    ConfigurationManager.AppSettings["ReportUserName"],
                    ConfigurationManager.AppSettings["ReportPassword"],
                    ConfigurationManager.AppSettings["ReportDomain"]
                    );
            }
        }

        public bool GetFormsCredentials(out Cookie authCookie, out string userName, out string password, out string authority)
        {
            authCookie = null;
            userName = null;
            password = null;
            authority = null;
            return false;
        }
    }
}
