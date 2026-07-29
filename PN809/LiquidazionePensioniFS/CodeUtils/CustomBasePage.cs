using INPS.Pensioni.LiquidazionePensione.Presenter;
using INPS.Pensioni.LiquidazionePensione.Presenter.Contract;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using INPS.Pensioni.LiquidazionePensione.View.Web.UserControls;
using System;
using System.Configuration;
using System.Globalization;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace INPS.Pensioni.LiquidazionePensione.View.Web
{
    public class CustomBasePage : BasePage
    {
        public InfoLiquidazione ValorizzaInfoLiquidazione(UCInfo ucInfoLiquidazione)
        {
            PresenterLiquidazione presenterLiquidazione = new PresenterLiquidazione();
            InfoLiquidazione InfoLiquidazione = new InfoLiquidazione();
            Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda Domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
            Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoAnagrafica Anagrafica = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoAnagrafica)Session["Anagrafica"];
            InfoLiquidazione.Domanda = Domanda.NumeroDomanda;
            InfoLiquidazione.CodiceFiscale = Anagrafica.CodiceFiscale;
            InfoLiquidazione.Categoria = Domanda.Categoria;
            InfoLiquidazione.Sede = Domanda.Sede;
            InfoLiquidazione.Certificato = Domanda.Certificato;
            InfoLiquidazione.Cognome = Anagrafica.Cognome;
            InfoLiquidazione.Nome = Anagrafica.Nome;
            InfoLiquidazione.Tipo = Domanda.Tipo;
            InfoLiquidazione.StatoDomanda = Domanda.Stato;
            BindData(InfoLiquidazione, ucInfoLiquidazione);
            return ucInfoLiquidazione.InfoLiquidazione;
        }

        private void BindData(InfoLiquidazione InfoLiquidazione, UCInfo ucInfoLiquidazione)
        {
            ucInfoLiquidazione.InfoLiquidazione = InfoLiquidazione;
            ucInfoLiquidazione.BindData();
        }

        protected override void OnLoad(EventArgs e)
        {
            if (IsSessionExpired())
            {
                Session.Add("PreviousPage", HttpContext.Current.Request.Url.AbsolutePath);
                Session.Add(CodeUtility.EnumSession.Courtesy_Type.ToString(), CodeUtility.CourtesyType.SessionExpired);
                Response.Redirect("~/Courtesy.aspx", true);
            }
            else
            {
                if (INPS.DNA.Context.OperationContextInfo.Current.CurrentOffice == null)
                {
                    switch (GetCurrentPageName())
                    {
                        case "UtilitySistema.aspx":
                        case "GestioneLiquidazioni.aspx":
                        case "TipologieNonAbilitate.aspx":
                        case "Monitoraggio.aspx":
                        case "Avvisi.aspx":
                        case "MessaggiHermes.aspx":
                        case "AvvisiEdit.aspx":
                        case "MessaggiHermesEdit.aspx":
                        case "Aggiornamenti.aspx":
                        case "AggiornamentiEdit.aspx":
                        case "SbloccoCancellazione.aspx":
                        case "LavorazioneManualeAutomatiche.aspx":
                        case "BypassControlli.aspx":
                        case "CambioDataSistema.aspx":
                        case "CambioDataINDCOM.aspx":
                        case "GestioneFAQ.aspx":
                        case "FAQEdit.aspx":
                        case "BypassTipologieNonAbilitate.aspx":
                        case "Aggiornamento.aspx":
                        case "GestioneAbilitazioneTrasformazioni.aspx":
                        case "GestioneBancheFideiussione.aspx":
                        case "GestioneAziendeVESO33.aspx":
                        case "GestioneAziendeCredito.aspx":
                        case "GestioneAziendeEditoriali.aspx":
                        case "GestioneAziendeEditorialiPerTipo0171.aspx":
                        case "GestioneAziendeEditorialiPerTipo0179.aspx":
                        case "GestioneProvvisoriePerCoefficienti.aspx":
                        case "GestioneAbilitazioneServizi.aspx":
                        case "GestioneAziendeVESO29.aspx":
                        case "GestioneAziendeVOESO.aspx":
                        case "GestioneAziendeESOTEL.aspx":
                        case "GestioneAziendeESPA.aspx":
                        case "GestioneAziendeESOPMI.aspx":
                        case "GestioneAziendeEditorialiLetteraB.aspx":
                        case "CambioDataPrepensionamentoLetteraB.aspx":
                        case "PrepensionamentoArt37Legge416198LetteraB.aspx":
                            break;
                        case "SbloccoDomanda.aspx":
                        case "RiassegnazioneDomanda.aspx":
                            Session.Add("PreviousPage", HttpContext.Current.Request.Url.AbsolutePath);
                            Response.Redirect("../SceltaSede.aspx", true);
                            break;
                        default:
                            Session.Add("PreviousPage", HttpContext.Current.Request.Url.AbsolutePath);
                            Response.Redirect("~/SceltaSede.aspx", true);
                            break;
                    }
                }
                base.OnLoad(e);
            }
        }

        protected override void InitializeCulture()
        {
            string selectedLanguage = "it-IT";

            UICulture = selectedLanguage;
            Culture = selectedLanguage;

            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(selectedLanguage);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(selectedLanguage);
            base.InitializeCulture();
        }

        // gestione sessione scaduta
        internal bool IsSessionExpired()
        {
            bool bSessionExpired = false;
            if (ConfigurationManager.AppSettings["SessionExpiredSkip"] != null && ConfigurationManager.AppSettings["SessionExpiredSkip"].ToUpperInvariant() == "NO" && Session["SessionAlive"] == null)
                bSessionExpired = true;

            return bSessionExpired;
        }

        internal AreaTitolare.DatiPensione GetDatiPensione(ITitolarePensione ITitolare)
        {
            if (Session["DatiPensione"] != null)
            {
                AreaTitolare.DatiPensione datiPensione = (AreaTitolare.DatiPensione)Session["DatiPensione"];
                return datiPensione;
            }
            else
            {
                AreaTitolare titolare = GetDatiTitolare(ITitolare);
                Session["DatiPensione"] = titolare.Pensione;
                return titolare.Pensione;
            }
        }

        internal AreaTitolare GetDatiTitolare(ITitolarePensione ITitolare)
        {
            PresenterTitolare presenterTitolare = new PresenterTitolare();
            AreaTitolare titolare = new AreaTitolare();
            titolare = presenterTitolare.CaricaTitolare(ITitolare);
            if (ITitolare.HasError)
            {
                titolare.Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                titolare.Esito.Messaggio = ITitolare.ErrorMessage;
            }
            return titolare;
        }

        internal void ValorizzaSemaforiTab(Image myImage, AreaQuadri.Semaforo tab, Control pnlTab)
        {
            string appTheme = Page.Theme;

            if (tab == AreaQuadri.Semaforo.Rosso_NonAbilitato)
            {
                pnlTab.Visible = false;
            }
            else if (tab == AreaQuadri.Semaforo.Rosso_Abilitato)
            {
                pnlTab.Visible = true;
                myImage.ImageUrl = string.Format("~/App_Themes/{0}/Images/rosso_tab.png", appTheme);
                if (string.IsNullOrEmpty((pnlTab as Panel).Attributes["class"]))
                    (pnlTab as Panel).Attributes["class"] = string.Empty;
                (pnlTab as Panel).Attributes["class"] = (pnlTab as Panel).Attributes["class"].Replace("optional", "");
                (pnlTab as Panel).Attributes["class"] = (pnlTab as Panel).Attributes["class"].Replace("mandatory", "");
                (pnlTab as Panel).Attributes["class"] = (pnlTab as Panel).Attributes["class"].Replace("saved", "");
                (pnlTab as Panel).Attributes["class"] = string.Format("{0} {1}", (pnlTab as Panel).Attributes["class"].Trim(), "mandatory");
            }

            else if (tab == AreaQuadri.Semaforo.Giallo)
            {
                pnlTab.Visible = true;
                myImage.ImageUrl = string.Format("~/App_Themes/{0}/Images/arancione_tab.png", appTheme);
                if (string.IsNullOrEmpty((pnlTab as Panel).Attributes["class"]))
                    (pnlTab as Panel).Attributes["class"] = string.Empty;
                (pnlTab as Panel).Attributes["class"] = (pnlTab as Panel).Attributes["class"].Replace("optional", "");
                (pnlTab as Panel).Attributes["class"] = (pnlTab as Panel).Attributes["class"].Replace("mandatory", "");
                (pnlTab as Panel).Attributes["class"] = (pnlTab as Panel).Attributes["class"].Replace("saved", "");
                (pnlTab as Panel).Attributes["class"] = string.Format("{0} {1}", (pnlTab as Panel).Attributes["class"].Trim(), "optional");
            }
            else if (tab == AreaQuadri.Semaforo.Verde)
            {
                pnlTab.Visible = true;
                myImage.ImageUrl = string.Format("~/App_Themes/{0}/Images/verde_tab.png", appTheme);
                if (string.IsNullOrEmpty((pnlTab as Panel).Attributes["class"]))
                    (pnlTab as Panel).Attributes["class"] = string.Empty;
                (pnlTab as Panel).Attributes["class"] = (pnlTab as Panel).Attributes["class"].Replace("optional", "");
                (pnlTab as Panel).Attributes["class"] = (pnlTab as Panel).Attributes["class"].Replace("mandatory", "");
                (pnlTab as Panel).Attributes["class"] = (pnlTab as Panel).Attributes["class"].Replace("saved", "");
                (pnlTab as Panel).Attributes["class"] = string.Format("{0} {1}", (pnlTab as Panel).Attributes["class"].Trim(), "saved");
            }
        }

        private string GetCurrentPageName()
        {
            string sPath = System.Web.HttpContext.Current.Request.Url.AbsolutePath;
            System.IO.FileInfo oInfo = new System.IO.FileInfo(sPath);
            string sRet = oInfo.Name;
            return sRet;
        }

        internal void ReloadUChangeSede()
        {
            //ricaricamento controllo master page
            UCChangeSede uc = (UCChangeSede)((UCIntestazione)Master.FindControl("UCIntestazione")).FindControl("ucChangeSede");
            if (uc != null)
                uc.ReloadControl();
        }
    }
}
