using System;
using System.Configuration;
using INPS.Pensioni.LiquidazionePensione.Presenter.Enum;
using INPS.Pensioni.LiquidazionePensione.Presenter;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;

namespace INPS.Pensioni.LiquidazionePensione.View.Web.UserControls
{
    public partial class UCMenuLeftProcedureOperatore : CustomBaseUserControl, IControlliDinamici
    {
        #region IControlliDinamici
        public DateTime? DataSistema { get; set; }
        public DateTime? DataINDCOM { get; set; }
        #endregion IControlliDinamici

        #region IViewUI
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        #endregion IViewUI

        protected void Page_Load(object sender, EventArgs e)
        {
            //imgExit.ImageUrl = "~/App_Themes/" + Page.Theme + "/Images/exit.png";

            if (!IsPostBack)
            {
                DataBind();

                if (Session["Ruolo"] != null)
                {
                    if (ConfigurationManager.AppSettings["CambioDataSistemaVisible"] != null &&
                             ConfigurationManager.AppSettings["CambioDataSistemaVisible"] == "SI")
                    {
                        this.liDataSistema.Visible = true;
                        GetDataSistema((UtilityTipoAppartenenza)Utility.GetTipoAppartenenzaRuolo((Ruoli)Session["Ruolo"]), this);
                        lblDataSistema.Text = string.Format("Data Sistema: {0:dd/MM/yyyy}", DataSistema.Value);
                    }
                }
            }
        }

        private void ChangeAttivato()
        {
            switch (GetCurrentPageName())
            {
                case "ElaborazionePosizione.aspx":
                case "RisultatoRicercaElaborazione.aspx":
                case "ConfermaAcquisizione.aspx":
                    liElaborazionePosizione.Attributes.Add("class", "attivato");
                    ClearSessionVisualizzaStatoPratiche();
                    break;
                case "TrasmissioneECalcolo.aspx":
                    liTrasmissioneECalcolo.Attributes.Add("class", "attivato");
                    ClearSessionVisualizzaStatoPratiche();
                    break;
                case "VisualizzazioneStatoPratiche.aspx":
                case "RisultatoVisualizzaStatoPratiche.aspx":
                    liVisualizzazioneStatoPratiche.Attributes.Add("class", "attivato");
                    break;
                case "Default.aspx":
                    liHome.Attributes.Add("class", "attivato");
                    ClearSessionVisualizzaStatoPratiche();
                    ClearSessionPageUnicarpe();
                    ClearSessionPageWebDom();
                    ClearSessionGestioneFAQ();
                    break;
                case "UtilitySistema.aspx":
                    liUtilitySistema.Attributes.Add("class", "attivato");
                    ClearSessionVisualizzaStatoPratiche();
                    break;
                default:
                    break;
            }
        }

        private string GetCurrentPageName()
        {
            string sPath = System.Web.HttpContext.Current.Request.Url.AbsolutePath;
            System.IO.FileInfo oInfo = new System.IO.FileInfo(sPath);
            string sRet = oInfo.Name;
            return sRet;
        }

        private void ClearSessionVisualizzaStatoPratiche()
        {
            Session.Remove("FlagBack");
            Session.Remove("CriteriComplete");
            Session.Remove("NumeroCriteri");
            Session.Remove("CriteriSelezionati");
            Session.Remove("Criteri");
            Session.Remove("Pratiche");
        }

        private void ClearSessionPageUnicarpe()
        {
            Session.Remove("Unicarpe");
        }

        private void ClearSessionPageWebDom()
        {
            Session.Remove("WebDom");
        }

        private void ClearSessionGestioneFAQ()
        {
            Session.Remove("PaginaGestioneFAQ");
        }

        private void RenderMenuLeftProcedureOperatore(bool isVisible)
        {
            this.aElaborazionePosizione.Visible = isVisible;
            this.liElaborazionePosizione.Visible = isVisible;
            this.aVisualizzazioneStatoPratiche.Visible = isVisible;
            this.liVisualizzazioneStatoPratiche.Visible = isVisible;
            this.aTrasmissioneECalcolo.Visible = isVisible;
            this.liTrasmissioneECalcolo.Visible = isVisible;
            this.aMonitoraggioProduttivita.Visible = isVisible;
            this.liMonitoraggioProduttivita.Visible = isVisible;

            if (ConfigurationManager.AppSettings["NascondiLink"] != null &&
                     ConfigurationManager.AppSettings["NascondiLink"] == "SI")
            {
                this.liTrasmissioneECalcolo.Visible = false;
                //this.liUtilitySistema.Visible = false;
                this.liMonitoraggioProduttivita.Visible = false;
            }

            if (Session["Ruolo"] != null &&
                (Session["Unicarpe"] == null || (bool)Session["Unicarpe"] == false) &&
                (Session["WebDom"] == null || (bool)Session["WebDom"] == false) &&
                (Session["SistemaUnico"] == null || (bool)Session["SistemaUnico"] == false))
            {
                this.aUtilitySistema.Visible = true;
                this.liUtilitySistema.Visible = true;
            }
            else
            {
                this.aUtilitySistema.Visible = false;
                this.liUtilitySistema.Visible = false;
            }
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            if (Session["Ruolo"] == null ||
                (Session["Unicarpe"] != null && (bool)Session["Unicarpe"] == true) ||
                (Session["WebDom"] != null && (bool)Session["WebDom"] == true) ||
                (Session["SistemaUnico"] != null && (bool)Session["SistemaUnico"] == true))
                RenderMenuLeftProcedureOperatore(false);
            else
                RenderMenuLeftProcedureOperatore(true);

            ChangeAttivato();
        }

        protected string GetlLstMenuClass()
        {
            var fileName = System.Web.VirtualPathUtility.GetFileName(Request.Path);
            // Esempio: se è About.aspx aggiungi "is-about", altrimenti "is-generic"
            return fileName.Equals("SceltaRuolo.aspx", StringComparison.OrdinalIgnoreCase)
                ? "no-leftside"
                : "";
        }
    }
}
