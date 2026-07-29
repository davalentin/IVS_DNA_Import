using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using INPS.DNA.UI.Web;
using INPS.Pensioni.LiquidazionePensione.Presenter;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using INPS.Pensioni.LiquidazionePensione.Presenter.Contract;
using INPS.DNA;
using Microsoft.Reporting.WebForms;
using System.Configuration;
using System.Security.Principal;
using System.Net;

using INPS.DNA.UI.Web.Intranet;

namespace INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.AltreFunzioni.Monitoraggio
{
    public partial class UCMonitoraggio : CustomBaseUserControl
    {
        #region IViewUI
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        #endregion IViewUI

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
            }
        }

        private void ClearForm()
        {
            CodeUtility.ClearForm(this, 0);
        }

        protected void btnReport_Click(object sender, EventArgs e)
        {
            string path = "../AltreFunzioni/VisualizzaReport.aspx?Name=" + ddlReport.SelectedValue;

            if (chkSedDest.Checked == true)
            {
                string codiceSedeDestinazione = "S";
                path += "&CSD=" + codiceSedeDestinazione;
            }

            if (chkPerfReq.Checked == true)
            {
                string dataPerfReq = "S";
                path += "&DPR=" + dataPerfReq;
            }

            if (chkMatricola.Checked == true)
            {
                string matricolaUtenteAcquisizione = "S";
                path += "&MU=" + matricolaUtenteAcquisizione;
            }

            if (chkDecOrig.Checked == true)
            {
                string decorrenzaOriginaria = "S";
                path += "&DO=" + decorrenzaOriginaria;
            }

            if (chkUnicarpe.Checked == true)
            {
                string flagUnicarpe = "S";
                path += "&FU=" + flagUnicarpe;
            }

            this.Page.ClientScript.RegisterStartupScript(this.GetType(), "newWindow", String.Format("<script>window.open('{0}', '', 'toolbar=no,resizable=yes,scrollbars=no');</script>", path));
        }

        protected void RaiseShowAvviso(object sender, EventArgs e)
        {
            ShowAvviso(sender, e);
        }

        protected void RaiseShowInfo(object sender, EventArgs e)
        {
            ShowInfo(sender, e);
        }

        protected void RaiseHideInfo(object sender, EventArgs e)
        {
            HideInfo(sender, e);
        }

        public event EventHandler ShowAvviso;

        public event EventHandler ShowInfo;

        public event EventHandler HideInfo;

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
}