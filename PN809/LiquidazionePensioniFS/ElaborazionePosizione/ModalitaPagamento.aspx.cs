using System;
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
using INPS.Pensioni.LiquidazionePensione.Presenter.Contract;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.View.Web;
using INPS.Pensioni.LiquidazionePensione.View.Web.UserControls;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using INPS.DNA;

namespace INPS.Pensioni.LiquidazionePensione.View.Web
{
    [NavigationRules(AllowedReferrer = AllowedReferrer.Application, CheckSequenceOnPostBack = false)]
    public partial class ModalitaPagamento : CustomBasePage, IInfoLiquidazione, IPagamento, IQuadriSemafori
    {
        #region IInfoLiquidazione
        public InfoLiquidazione InfoLiquidazione { get; set; }
        #endregion IInfoLiquidazione

        #region IPagamento
        public AreaPagamento pagamentoPensione { get; set; }
        public Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda { get; set; }
        public RichiestaUfficiPagatori richiestaUfficiPagatori { get; set; }
        public UfficioPagatore[] ufficioPagatore { get; set; }
        #endregion IPagamento

        #region IViewUI
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        #endregion IViewUI

        #region IQuadriSemafori
        public AreaQuadri areaQuadri { get; set; }
        public AreaInfoPratica areaInfoPratica { get; set; }
        #endregion IQuadriSemafori


        protected void Page_Load(object sender, EventArgs e)
        {
            imgPagamento.ImageUrl = "~/App_Themes/" + Page.Theme + "/Images/rosso_tab.png";

            if (!IsPostBack)
            {
                this.InfoLiquidazione = ValorizzaInfoLiquidazione(ucInfoLiquidazione);
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
                btnPopUpPage.Enabled = false;
                btnSalvaPagamento.Enabled = false;              
                //ManageBtnPopup(this.domanda.IsDomandaENPALS && ucPagamento.pagamentoPensione.IsPolarizzazionePerGestioneENPALSAttiva);
                //Button btn = GetBtnSalvataggio(this.domanda.IsDomandaENPALS && ucPagamento.pagamentoPensione.IsPolarizzazionePerGestioneENPALSAttiva);
                //btn.Enabled = false;
                if (Session["Pratiche"] != null)
                {
                    btnTornaPosizioni.Visible = true;
                    btnTornaARicerca.Visible = false;
                }
            }
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            if ((AreaQuadri)Session["Semaforo"] != null)
            {
                this.areaQuadri = (AreaQuadri)Session["Semaforo"];
                ValorizzaSemaforiTab(imgPagamento, this.areaQuadri.QuadroPagamento.TabPagamento, pnlModalitaPagamento);
            }
        }

        protected void SalvaPagamento_Click(object sender, EventArgs e)
        {
            try
            {
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
                pagamentoPensione = new AreaPagamento();
                pagamentoPensione.Pagamento = new GestioneAreaPagamentoDatiPagamento();
                ucPagamento.pagamentoPensione = pagamentoPensione;
                ucPagamento.SalvaDatiPagamento(this, null);

                this.areaInfoPratica = new AreaInfoPratica();
                List<AreaQuadri.Tab> elencoTab = new List<AreaQuadri.Tab>();
                elencoTab.Add(AreaQuadri.Tab.Pagamento);
                this.areaInfoPratica.ElencoTab = elencoTab.ToArray();

                CodeUtility.AggiornaSemafori(this, this, ucInfoLiquidazione);

                if (!ucPagamento.IsBancaItaliaFromWebDom)
                    btnEliminaPagamento.Enabled = true;
                if (this.domanda.IsDomandaENPALS && ucPagamento.IsPolarizzazionePerGestioneENPALSAttiva)
                {
                    AreaTitolare.DatiPensione datiPensione = CodeUtility.GetDatiPensioneFromSession();
                    datiPensione.CentroOperativoDestinazione = ucPagamento.pagamentoPensione.CentroOperativoDestinazione;
                    datiPensione.CodiceSedeDestinazione = ucPagamento.pagamentoPensione.CodiceSedeDestinazione;
                    Session["DatiPensione"] = datiPensione;
                    this.InfoLiquidazione = ValorizzaInfoLiquidazione(ucInfoLiquidazione);
                }
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("ModalitaPagamento, Errore nel metodo SalvaPagamento_Click " + ex);
            }
        }

        protected void event_ucSalvaPagamento(object sender, EventArgs e)
        {
            ucAvviso.Tipo = TipoAvviso.Ok;
            ucAvviso.Messaggio = "Dati Pagamento salvati correttamente";
            ucAvviso.Visible = true;
            btnEliminaPagamento.Enabled = true;
        }

        protected void event_ucServiceErrorAvviso(object sender, EventArgs e)
        {
            ucAvviso.Tipo = TipoAvviso.Ko;
            ucAvviso.Messaggio = this.ucPagamento.ErrorMessage;
            ucAvviso.Visible = true;
        }

        protected void event_ucNessunaPosizioneTrovata(object sender, EventArgs e)
        {
            ucAvviso.Tipo = TipoAvviso.Warning;
            ucAvviso.Messaggio = "Nessuna posizione trovata per i parametri inseriti";
            ucAvviso.Visible = true;
        }

        protected void EliminaPagamento_Click(object sender, EventArgs e)
        {
            this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
            ucPagamento.EliminaPagamento(this, null);

            this.areaInfoPratica = new AreaInfoPratica();
            List<AreaQuadri.Tab> elencoTab = new List<AreaQuadri.Tab>();
            elencoTab.Add(AreaQuadri.Tab.Pagamento);
            this.areaInfoPratica.ElencoTab = elencoTab.ToArray();

            //CodeUtility.AggiornaSemafori(this, this,ucInfoLiquidazione);
            CodeUtility.AggiornaSemafori(this, this, ucInfoLiquidazione);
        }

        protected void event_ucEliminaPagamento(object sender, EventArgs e)
        {
            ucAvviso.Tipo = TipoAvviso.Ok;
            ucAvviso.Messaggio = "Dati Pagamento eliminati correttamente";
            ucAvviso.Visible = true;
            this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
            Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoAnagrafica anagraficaTitolare = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoAnagrafica)Session["Anagrafica"];
            Button btn = GetBtnSalvataggio(this.domanda.IsDomandaENPALS && GetVSIsPolarizzazionePerGestioneENPALSAttiva() && anagraficaTitolare.CodiceComuneResidenza.StartsWith("Z"));
            if (btn.Enabled == true)
                btn.Enabled = false;
            btnEliminaPagamento.Enabled = false;
        }

        protected void event_ucVisualizzaEliminaPagamento(object sender, EventArgs e)
        {
            btnEliminaPagamento.Enabled = true;
        }

        protected void event_ucBloccaEliminaPagamento(object sender, EventArgs e)
        {
            btnEliminaPagamento.Enabled = false;
        }

        protected void event_ucVisualizzaTastoSalva(object sender, EventArgs e)
        {
            btnPopUpPage.Enabled = true;
            btnSalvaPagamento.Enabled = true;
            ucAvviso.Messaggio = "";
            ucAvviso.Visible = false;
        }

        protected void event_ucNascondiPannelloAvviso(object sender, EventArgs e)
        {
            if (ucAvviso.Visible == true)
            {
                ucAvviso.Messaggio = "";
                ucAvviso.Visible = false;
            }
        }

        protected void event_ucParametriNonValidi(object sender, EventArgs e)
        {
            ucAvviso.Tipo = TipoAvviso.Warning;
            ucAvviso.Messaggio = "Parametri inseriti non validi";
            ucAvviso.Visible = true;
        }

        protected void event_ucManageBtnPopup(object sender, EventArgs e)
        {
            this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
            Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoAnagrafica anagraficaTitolare = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoAnagrafica)Session["Anagrafica"];
            ViewState["VSIsPolarizzazionePerGestioneENPALSAttiva"] = ucPagamento.pagamentoPensione.IsPolarizzazionePerGestioneENPALSAttiva;
            bool apriPopup = this.domanda.IsDomandaENPALS && ucPagamento.pagamentoPensione.IsPolarizzazionePerGestioneENPALSAttiva && !anagraficaTitolare.CodiceComuneResidenza.StartsWith("Z");
            ManageBtnPopup(apriPopup);
            if (apriPopup)
            {
                string controlloDinamico = string.Empty;
                Presenter.PresenterControlliDinamici presenter = new PresenterControlliDinamici();
                Presenter.SvrLiquidazione.AreaEsito esito = presenter.GetControlloDinamicoByNomeControllo("SedePoloENPALS", out controlloDinamico);
                if (esito != null && esito.RisultatoOperazione == Presenter.SvrLiquidazione.AreaEsito.TipoEsito.OK)
                {
                    if (!String.IsNullOrEmpty(controlloDinamico))
                        HdnCodiceSedePoloEnpals.Value = controlloDinamico;
                }
            }
        }

        private bool GetVSIsPolarizzazionePerGestioneENPALSAttiva()
        {
            bool isAttiva = false;
            if (ViewState["VSIsPolarizzazionePerGestioneENPALSAttiva"] != null)
                isAttiva = (bool)ViewState["VSIsPolarizzazionePerGestioneENPALSAttiva"];
            return isAttiva;
        }

        private void ManageBtnPopup(bool isBtnPopup)
        {
            Button btnToView = GetBtnSalvataggio(isBtnPopup);
            Button btnToHide = GetBtnSalvataggio(!isBtnPopup);
            btnToView.Style.Remove("display");
            btnToHide.Style.Remove("display");
            btnToHide.Style.Add("display", "none");
        }

        private Button GetBtnSalvataggio(bool isPopup)
        {
            return isPopup ? btnPopUpPage : btnSalvaPagamento;
        }
    }
}
