using System;
using System.Collections.Generic;
using INPS.DNA.UI.Web;
using INPS.DNA;

using INPS.Pensioni.LiquidazionePensione.Presenter.Contract;
using INPS.Pensioni.LiquidazionePensione.Presenter;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using INPS.Pensioni.LiquidazionePensione.Presenter.Enum;

namespace INPS.Pensioni.LiquidazionePensione.View.Web
{
    [NavigationRules(AllowedReferrer = AllowedReferrer.Application, CheckSequenceOnPostBack = false)]
    public partial class RisultatoRicercaElaborazione : CustomBasePage, IRicercaPosizione
    {
        #region IElaborazionePosizione
        public RicercaPosizione RicercaPosizione { get; set; }
        public RicercaPosizione RicercaDanteCausa { get; set; }
        public List<Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda> ElencoDomande { get; set; }
        public List<Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoPensione> ElencoPensioni { get; set; }
        public List<Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoSinonimo> ElencoSinonimi { get; set; }
        public Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoAnagrafica RiepilogoAnagrafica { get; set; }
        public Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiEsitoCalcolo EsitoCalcolo { get; set; }
        public Presenter.SvrLiquidazione.AreaEsito.TipoEsito Esito { get; set; }
        public UtilityTipoAppartenenza TipoAppRuolo { get; set; }
        public UtilityRuolo Ruolo { get; set; }
        public bool IsDomandaDB { get; set; }
        public bool IsPaginaConferma { get; set; }
        public bool IsDomandaCalcolataProvvisoria { get; set; }
        public bool IsConsultazione { get; set; }
        public string SedeDiversa { get; set; }
        public bool IsRicercaManualeDA { get; set; }
        public bool IsNuovoCertificatoGeneratoEnpals { get; set; }
        //ENG - Pensioni Ovunque: gestione nuovo pannello
        public bool MostraPanelloMessBloccantePensioniOvunque { get; set; }
        public string SedePensioneGP1ALZ6 { get; set; }
        public string CodCategoriaPensione { get; set; }
        public string CertificatoInseguimentoPensione { get; set; }
        //ENG - Bypass "ELIMINAZIONE_CONTROLLO_SEDE"
        public bool IsPaginaVisualizzazioneStatoPratiche { get; set; }
        //ENG - Gestione Popup Memo 239
        public bool MostraPopupMemo239 { get; set; }
        //ENG - Gestione Popup Memo 31/2023
        public bool MostraPopupMemo312023 { get; set; }
        #endregion IElaborazionePosizione

        #region IViewUI
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        #endregion IViewUI

        protected void Page_Load(object sender, EventArgs e)
        {
            this.TipoAppRuolo = (UtilityTipoAppartenenza)Utility.GetTipoAppartenenzaRuolo((Ruoli)Session["Ruolo"]);
            this.Ruolo = this.Ruolo = CodeUtility.GetRuolo(Session["Ruolo"]);
            if (!IsPostBack)
            {
                try
                {
                    VisualizzaUCRisultatoRicerca();
                }
                catch (DnaExceptionBase)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    throw new INPS.DNA.DnaApplicationException("RisultatoRicercaElaborazione, Errore nel metodo PageLoad" + ex);
                }
            }
        }

        private void VisualizzaUCRisultatoRicerca()
        {
            try
            {
                List<Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoSinonimo> Sinonimi = (List<Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoSinonimo>)Session["Sinonimi"];
                Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoAnagrafica Anagrafica = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoAnagrafica)Session["Anagrafica"];

                if (Sinonimi != null)
                {
                    if (Session["TornaASinonimi"] == null)
                    {
                        Session["TornaASinonimi"] = 1;
                        ucSinonimi.Visible = true;
                        ucRisultatoRicerca.Visible = false;
                    }
                    else
                    {
                        Session.Remove("TornaASinonimi");
                        ucSinonimi.Visible = false;
                        ucRisultatoRicerca.Visible = true;
                    }
                    Session.Remove("InfoErroreWebDom");
                }

                else if (Anagrafica != null || Sinonimi == null)
                {
                    ucSinonimi.Visible = false;
                    ucRisultatoRicerca.Visible = true;
                }

                if (Session["InfoErroreWebDom"] != null &&
                    !string.IsNullOrEmpty(Session["InfoErroreWebDom"].ToString()))
                {
                    ucAvviso.Visible = true;
                    ucAvviso.Tipo = UserControls.TipoAvviso.Warning;
                    ucAvviso.Messaggio = Session["InfoErroreWebDom"].ToString();
                }
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("RisultatoRicercaElaborazione, Errore nel metodo VisualizzaUCRisultatoRicerca" + ex);
            }
        }

        protected void event_ReloadUChangeSede(object sender, EventArgs e)
        {
            ReloadUChangeSede();
        }
    }
}


