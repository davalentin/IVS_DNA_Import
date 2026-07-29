using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using INPS.DNA.Security;
using INPS.DNA.Security.Idm;
using INPS.DNA.UI.Web;
using INPS.DNA.UI.Web.Intranet;
using INPS.Pensioni.LiquidazionePensione.Presenter;
using INPS.Pensioni.LiquidazionePensione.Presenter.Enum;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.View.Web.UserControls;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;

namespace INPS.Pensioni.LiquidazionePensione.View.Web
{
    [NavigationRules(AllowedReferrer = AllowedReferrer.Application, CheckSequenceOnPostBack = false)]
    public partial class UtilitySistema : CustomBasePage, IMenuLeftAltreFunzioni
    {
        #region IMenuLeftAltreFunzioni
        public AreaAltreFunzioni AltreFunzioni { get; set; }
        #endregion IMenuLeftAltreFunzioni

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
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
                    this.liGestioneBanchefideiussione.Visible = abilitazioni.IsGestioneAziendeVESO92;
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
                    this.liCambioDataINDCOM.Visible = false;

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
                    this.liGestioneAziendeVOESO.Visible = false;
                    this.liGestioneBanchefideiussione.Visible = false;
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

                Page.DataBind();
            }
        }
    }
}
