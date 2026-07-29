using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using INPS.DNA;
using INPS.Pensioni.LiquidazionePensione.Presenter;
using INPS.Pensioni.LiquidazionePensione.Presenter.Contract;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using INPS.DNA.UI.Web;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using System.Configuration;
using INPS.Pensioni.LiquidazionePensione.Presenter.Enum;

namespace INPS.Pensioni.LiquidazionePensione.View.Web.UserControls
{
    public partial class UCMenuLeftAltreFunzioni : CustomBaseUserControl, IControlliDinamici, IMenuLeftAltreFunzioni
    {
        #region IControlliDinamici
        public DateTime? DataSistema { get; set; }

        public DateTime? DataINDCOM { get; set; }
        #endregion IControlliDinamici

        #region IViewUI
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        #endregion IViewUI

        #region IMenuLeftAltreFunzioni
        public AreaAltreFunzioni AltreFunzioni { get; set; }
        #endregion IMenuLeftAltreFunzioni

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                DataBind();

                if (!CodeUtility.IsAmministratore(Session["Ruolo"]))
                {
                    if (Session["AltreFunzioni"] == null)
                    {
                        PresenterMenuLeftAltreFunzioni presenter = new PresenterMenuLeftAltreFunzioni();
                        presenter.GetAbilitazioniAltreFunzioni(this);
                    }
                    else
                        this.AltreFunzioni = Session["AltreFunzioni"] as AreaAltreFunzioni;
                    Presenter.SvrLiquidazione.AltreFunzioni abilitazioni = this.AltreFunzioni.Abilitazioni;
                    this.liGestioneLiquidazioni.Visible = abilitazioni.IsGestioneLiquidazione;
                    this.liTipologieNonAbilitate.Visible = abilitazioni.IsTipologieNonAbilitate;
                    this.liMonitoraggio.Visible = abilitazioni.IsMonitoraggio;
                    this.liSbloccoDomanda.Visible = abilitazioni.IsSbloccoDomanda;
                    this.liAvvisi.Visible = abilitazioni.IsAvvisi;
                    this.liMessaggiHermes.Visible = abilitazioni.IsMessaggiHermes;
                    this.liAggiornamenti.Visible = abilitazioni.IsAggiornamenti;
                    this.liSbloccoCancellazione.Visible = abilitazioni.IsSbloccoCancellazione;
                    this.liBypassControlli.Visible = abilitazioni.IsBypassControlli;
                    this.liCambioDataSistema.Visible = abilitazioni.IsCambioDataSistema;
                    this.liGestioneFAQ.Visible = abilitazioni.IsGestioneFaq;
                    this.liCambioStatoDomanda.Visible = abilitazioni.IsCambioStatoDomanda;
                    this.liBypassTipologieNonAbilitate.Visible = abilitazioni.IsBypassTipologieNonAbilitate;
                    this.liAggiornamento.Visible = abilitazioni.IsFunzionalitaAggiornamentoPostCalcolo;
                    this.liGestioneTrasformazioni.Visible = abilitazioni.IsGestioneTrasformazioni;
                    this.liGestioneBancheFideiussione.Visible = abilitazioni.IsGestioneAziendeVESO92;
                    this.liGestioneAziendeVESO33.Visible = abilitazioni.IsGestioneAziendeVESO33;
                    this.liGestioneAziendeCredito.Visible = abilitazioni.IsGestioneAziendeCredito;
                    this.liGestioneProvvisoriePerCoefficienti.Visible = abilitazioni.IsGestioneProvvisoriePerCoefficienti;
                    this.liGestioneAbilitazioneServizi.Visible = abilitazioni.IsGestioneAbilitazioneChiavi;
                    this.liGestioneAziendeEditoriali.Visible = abilitazioni.IsGestioneAziendeEditoriali;
                    this.liGestioneAziendeVESO29.Visible = abilitazioni.IsGestioneAziendeVESO29;
                    this.liGestioneAziendeEditorialiPerTipo0171.Visible = abilitazioni.IsGestioneAziendeEditoriali0171;
                    this.liGestioneAziendeEditorialiPerTipo0179.Visible = abilitazioni.IsGestioneAziendeEditoriali0179;
                    this.liGestioneAziendeVOESO.Visible = abilitazioni.IsGestioneAziendeVOESO;
                    this.liGestioneAziendeESOTEL.Visible = abilitazioni.IsGestioneAziendeESOTEL;
                    this.liGestioneAziendeESOAMB.Visible = abilitazioni.IsGestioneAziendeESOAMB;
                    this.liGestioneAziendeESPA.Visible = abilitazioni.IsGestioneAziendeESPA;
                    this.liGestioneAziendeESOPMI.Visible = abilitazioni.IsGestioneAziendeESOPMI;
                    this.liGestioneAziendeEditorialiLetteraB.Visible = abilitazioni.IsGestioneAziendeEditorialiLetteraB;
                }

                if (!CodeUtility.IsAmministratoreAGO(Session["Ruolo"]))
                    this.lICambioDataINDCOM.Visible = false;

                if (!CodeUtility.IsDirettore_RdP(Session["Ruolo"]) && !CodeUtility.IsAmministratore(Session["Ruolo"]))
                    this.liRiassegnazioneDomanda.Visible = false;

                if (ConfigurationManager.AppSettings["BypassRiassegnazioneDomanda"] != null &&
                     ConfigurationManager.AppSettings["BypassRiassegnazioneDomanda"] == "SI")
                {
                    this.liRiassegnazioneDomanda.Visible = false;
                }

                if (ConfigurationManager.AppSettings["CambioDataSistemaVisible"] == null ||
                     ConfigurationManager.AppSettings["CambioDataSistemaVisible"] != "SI")
                {
                    this.liCambioDataSistema.Visible = false;
                }

                UtilityTipoAppartenenza tipoAppartenenza = (UtilityTipoAppartenenza)Utility.GetTipoAppartenenzaRuolo((Ruoli)Session["Ruolo"]);
                if (tipoAppartenenza != UtilityTipoAppartenenza.AGO)
                {
                    this.liGestioneProvvisoriePerCoefficienti.Visible = false;
                    this.liGestioneAziendeEditoriali.Visible = false;
                    this.liGestioneAziendeVESO29.Visible = false;
                    this.liGestioneAziendeEditorialiPerTipo0171.Visible = false;
                    this.liGestioneAziendeEditorialiPerTipo0179.Visible = false;
                    this.liGestioneAziendeVOESO.Visible = false;
                    this.liGestioneBancheFideiussione.Visible = false;
                    this.liGestioneAziendeCredito.Visible = false;
                    this.liGestioneAziendeVESO33.Visible = false;
                    this.liGestioneAziendeESOTEL.Visible = false;
                    this.liGestioneAziendeESOAMB.Visible = false;
                    this.liGestioneAziendeESPA.Visible = false;
                    this.liGestioneAziendeESOPMI.Visible = false;
                    this.liGestioneAziendeEditorialiLetteraB.Visible = false;
                }

                Ruoli ruolo = (Ruoli)Session["Ruolo"];
                if (ruolo == Ruoli.P8974)
                {
                    this.liBypassControlli.Visible = true;
                }
            }
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            ChangeAttivato();

            if (ConfigurationManager.AppSettings["CambioDataSistemaVisible"] != null &&
                         ConfigurationManager.AppSettings["CambioDataSistemaVisible"] == "SI")
            {
                this.liDataSistema.Visible = true;
                GetDataSistema((UtilityTipoAppartenenza)Utility.GetTipoAppartenenzaRuolo((Ruoli)Session["Ruolo"]), this);
                lblDataSistema.Text = string.Format("Data Sistema: {0:dd/MM/yyyy}", DataSistema.Value);
            }
        }

        internal void ChangeAttivato()
        {
            switch (GetCurrentPageName())
            {
                case "GestioneLiquidazioni.aspx":
                    liGestioneLiquidazioni.Attributes.Add("class", "attivato");
                    ClearSessionGestioneFAQ();
                    break;
                case "TipologieNonAbilitate.aspx":
                    liTipologieNonAbilitate.Attributes.Add("class", "attivato");
                    ClearSessionGestioneFAQ();
                    break;
                case "SbloccoDomanda.aspx":
                    liSbloccoDomanda.Attributes.Add("class", "attivato");
                    ClearSessionGestioneFAQ();
                    break;
                case "RiassegnazioneDomanda.aspx":
                    liRiassegnazioneDomanda.Attributes.Add("class", "attivato");
                    ClearSessionGestioneFAQ();
                    break;
                case "Monitoraggio.aspx":
                    liMonitoraggio.Attributes.Add("class", "attivato");
                    ClearSessionGestioneFAQ();
                    break;
                case "Avvisi.aspx":
                case "AvvisiEdit.aspx":
                    liAvvisi.Attributes.Add("class", "attivato");
                    ClearSessionGestioneFAQ();
                    break;
                case "MessaggiHermes.aspx":
                case "MessaggiHermesEdit.aspx":
                    liMessaggiHermes.Attributes.Add("class", "attivato");
                    ClearSessionGestioneFAQ();
                    break;
                case "Aggiornamenti.aspx":
                case "AggiornamentiEdit.aspx":
                    liAggiornamenti.Attributes.Add("class", "attivato");
                    ClearSessionGestioneFAQ();
                    break;
                case "SbloccoCancellazione.aspx":
                    liSbloccoCancellazione.Attributes.Add("class", "attivato");
                    ClearSessionGestioneFAQ();
                    break;
                case "LavorazioneManualeAutomatiche.aspx":
                    liLavorazioneManualeAutomatiche.Attributes.Add("class", "attivato");
                    ClearSessionGestioneFAQ();
                    break;
                case "BypassControlli.aspx":
                    liBypassControlli.Attributes.Add("class", "attivato");
                    ClearSessionGestioneFAQ();
                    break;
                case "CambioDataSistema.aspx":
                    liCambioDataSistema.Attributes.Add("class", "attivato");
                    ClearSessionGestioneFAQ();
                    break;
                case "CambioDataINDCOM.aspx":
                    lICambioDataINDCOM.Attributes.Add("class", "attivato");
                    ClearSessionGestioneFAQ();
                    break;
                case "GestioneFAQ.aspx":
                    liGestioneFAQ.Attributes.Add("class", "attivato");
                    break;
                case "CambioStatoDomanda.aspx":
                    liCambioStatoDomanda.Attributes.Add("class", "attivato");
                    ClearSessionGestioneFAQ();
                    break;
                case "BypassTipologieNonAbilitate.aspx":
                    liBypassTipologieNonAbilitate.Attributes.Add("class", "attivato");
                    ClearSessionGestioneFAQ();
                    break;
                case "Aggiornamento.aspx":
                    liAggiornamento.Attributes.Add("class", "attivato");
                    ClearSessionGestioneFAQ();
                    break;
                case "PulisciDomanda.aspx":
                    liPulisciDomanda.Attributes.Add("class", "attivato");
                    ClearSessionGestioneFAQ();
                    break;
                case "GestioneAbilitazioneTrasformazioni.aspx":
                    liGestioneTrasformazioni.Attributes.Add("class", "attivato");
                    ClearSessionGestioneFAQ();
                    break;
                case "GestioneBancheFideiussione.aspx":
                    liGestioneBancheFideiussione.Attributes.Add("class", "attivato");
                    ClearSessionGestioneFAQ();
                    break;
                case "GestioneAziendeVESO33.aspx":
                    liGestioneAziendeVESO33.Attributes.Add("class", "attivato");
                    ClearSessionGestioneFAQ();
                    break;
                case "GestioneAziendeCredito.aspx":
                    liGestioneAziendeCredito.Attributes.Add("class", "attivato");
                    ClearSessionGestioneFAQ();
                    break;
                case "GestioneAziendeEditoriali.aspx":
                    liGestioneAziendeEditoriali.Attributes.Add("class", "attivato");
                    ClearSessionGestioneFAQ();
                    break;
                case "PrepensionamentoArt37Legge416198LetteraB.aspx":
                    liGestioneAziendeEditorialiLetteraB.Attributes.Add("class", "attivato");
                    ClearSessionGestioneFAQ();
                    break;
                case "GestioneAziendeEditorialiPerTipo0171.aspx":
                    liGestioneAziendeEditorialiPerTipo0171.Attributes.Add("class", "attivato");
                    ClearSessionGestioneFAQ();
                    break;
                case "GestioneAziendeEditorialiPerTipo0179.aspx":
                    liGestioneAziendeEditorialiPerTipo0179.Attributes.Add("class", "attivato");
                    ClearSessionGestioneFAQ();
                    break;
                case "GestioneProvvisoriePerCoefficienti.aspx":
                    liGestioneProvvisoriePerCoefficienti.Attributes.Add("class", "attivato");
                    ClearSessionGestioneFAQ();
                    break;
                case "GestioneAbilitazioneServizi.aspx":
                    liGestioneAbilitazioneServizi.Attributes.Add("class", "attivato");
                    ClearSessionGestioneFAQ();
                    break;
                case "GestioneAziendeVESO29.aspx":
                    liGestioneAziendeVESO29.Attributes.Add("class", "attivato");
                    ClearSessionGestioneFAQ();
                    break;
                case "GestioneAziendeVOESO.aspx":
                    liGestioneAziendeVOESO.Attributes.Add("class", "attivato");
                    ClearSessionGestioneFAQ();
                    break;
                case "GestioneAziendeESOTEL.aspx":
                    liGestioneAziendeESOTEL.Attributes.Add("class", "attivato");
                    ClearSessionGestioneFAQ();
                    break;
                case "GestioneAziendeESOAMB.aspx":
                    liGestioneAziendeESOAMB.Attributes.Add("class", "attivato");
                    ClearSessionGestioneFAQ();
                    break;
                case "GestioneAziendeESPA.aspx":
                    liGestioneAziendeESPA.Attributes.Add("class", "attivato");
                    ClearSessionGestioneFAQ();
                    break;
                case "GestioneAziendeESOPMI.aspx":
                    liGestioneAziendeESOPMI.Attributes.Add("class", "attivato");
                    ClearSessionGestioneFAQ();
                    break;
            }
        }

        private void ClearSessionGestioneFAQ()
        {
            Session.Remove("PaginaGestioneFAQ");
        }

        private string GetCurrentPageName()
        {
            string sPath = System.Web.HttpContext.Current.Request.Url.AbsolutePath;
            System.IO.FileInfo oInfo = new System.IO.FileInfo(sPath);
            string sRet = oInfo.Name;
            return sRet;
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
