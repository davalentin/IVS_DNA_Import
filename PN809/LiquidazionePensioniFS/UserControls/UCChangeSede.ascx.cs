using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using INPS.DNA.UI.Web;
using INPS.Pensioni.LiquidazionePensione.Presenter.Enum;

namespace INPS.Pensioni.LiquidazionePensione.View.Web.UserControls
{
    public partial class UCChangeSede : CustomBaseUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["Ruolo"] != null &&
                (INPS.DNA.Security.DnaPrincipal.Current.OfficeForCurrentApplication(((Ruoli)Session["Ruolo"]).ToString()).ToList<string>().Count > 1 ||
                CodeUtility.IsAmministratore(Session["Ruolo"])))
            {
                btnChangeSede.ImageUrl = "~/App_Themes/" + Page.Theme + "/Images/pencil_dark.png";
            }
            else
            {
                btnChangeSede.Visible = false;
            }
            if (btnChangeSede.Visible)
            {
                switch (GetCurrentPageName())
                {
                    case "ElaborazionePosizione.aspx":
                    case "TrasmissioneECalcolo.aspx":
                    case "Default.aspx":
                    case "default.aspx":
                    case "VisualizzazioneStatoPratiche.aspx":
                    case "SceltaSede.aspx":
                    case "SceltaRuolo.aspx":
                    case "RisultatoRicercaElaborazione.aspx":
                    case "RisultatoVisualizzaStatoPratiche.aspx":
                    case "UtilitySistema.aspx":
                    case "SbloccoDomanda.aspx":
                    case "RiassegnazioneDomanda.aspx":
                    case "GestioneLiquidazioni.aspx":
                    case "TipologieNonAbilitate.aspx":
                    case "Monitoraggio.aspx":
                    case "Avvisi.aspx":
                    case "MessaggiHermes.aspx":
                    case "AvvisiEdit.aspx":
                    case "MessaggiHermesEdit.aspx":
                    case "SbloccoCancellazione.aspx":
                        break;
                    default:
                        btnChangeSede.Visible = false;
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
            if (INPS.DNA.Context.OperationContextInfo.Current.CurrentOffice != null)
            {
                lblSede.Text = (INPS.DNA.Context.OperationContextInfo.Current.CurrentOffice.ExtendedProperties != null ? 
                    INPS.DNA.Context.OperationContextInfo.Current.CurrentOffice.ExtendedProperties["SEDE"].Trim() :
                    INPS.DNA.Context.OperationContextInfo.Current.CurrentOffice.Name.Trim());
                lblCentroOperativo.Text = " - " + INPS.DNA.Context.OperationContextInfo.Current.CurrentOffice.AspnCode;
                pnlChgSede.Visible = true;
            }
            else
            {
                pnlChgSede.Visible = false;
            }
        }

        protected void btnChangeSede_Click(object sender, ImageClickEventArgs e)
        {
            INPS.DNA.Context.OperationContextInfo.Current.CurrentOffice = null;
            Response.Redirect("~/SceltaSede.aspx");
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