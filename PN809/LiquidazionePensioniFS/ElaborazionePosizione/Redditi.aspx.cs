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


namespace INPS.Pensioni.LiquidazionePensione.View.Web
{
    [NavigationRules(AllowedReferrer = AllowedReferrer.Application, CheckSequenceOnPostBack = false)]
    public partial class Redditi : CustomBasePage, IInfoLiquidazione, IRedditi, IQuadriSemafori
    {
        #region IInfoLiquidazione
        public InfoLiquidazione InfoLiquidazione { get; set; }
        #endregion IInfoLiquidazione

        #region IViewUI
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        #endregion IViewUI

        #region IRedditi
        public AreaRedditi areaRedditi { get; set; }
        public INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda { get; set; }
        public bool IsSalvataggio { get; set; }
        #endregion IRedditi

        #region IQuadriSemafori
        public AreaQuadri areaQuadri { get; set; }
        public AreaInfoPratica areaInfoPratica { get; set; }
        #endregion IQuadriSemafori

        string _service_error = "#_SERVICE_ERROR_#";
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.InfoLiquidazione = ValorizzaInfoLiquidazione(ucInfoLiquidazione);
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
                AreaTitolare.DatiPensione datiPensione = (AreaTitolare.DatiPensione)Session["DatiPensione"];
                this.areaQuadri = (AreaQuadri)Session["Semaforo"];
                if (datiPensione.TipoAppartenenzaDomanda != AreaTitolare.DatiPensione.TipoAppDomanda.AGO && this.domanda.IsDomandaRiapertura && (datiPensione.IsDomandaQuota100OrRicostituzione || datiPensione.IsDomandaAPEPrecociOrRicostituzione || datiPensione.IsDomandaQuota102OrRicostituzione || datiPensione.IsDomandaAnticipataFlessibileOrRicostituzione || 
                    datiPensione.IsDomandaAnticipataFlessibileOpzioneContributivoOrRicostituzione || datiPensione.IsDomandaAnticipataFlessibileLeggeBilancio2024OrRicostituzione || datiPensione.IsDomandaAnticipataFlessibileLeggeBilancio2024OpzioneContributivoOrRicostituzione) && (areaQuadri.QuadroOneri.TabOneri == AreaQuadri.Semaforo.Giallo || areaQuadri.QuadroOneri.TabOneri == AreaQuadri.Semaforo.Rosso_Abilitato))
                {
                    ucAvviso.Visible = true;
                    ucAvviso.Tipo = TipoAvviso.Ko;
                    ucAvviso.Messaggio = "E' necessario salvare il quadro oneri prima di procedere con la compilazione del quadro redditi.";
                    btnSalva.Enabled = false;
                    ucRedditi.Visible = false;
                }
                else
                {
                    CaricaRedditi();
                    ucRedditi.ValorizzaRedditi();
                }
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
                ValorizzaSemaforiTab(imgRedditi, this.areaQuadri.QuadroRedditi.TabRedditi, pnlTabRedditi);
            }
        }

        protected void event_ucAcquisizioneRedditi(object sender, EventArgs e)
        {
            this.areaRedditi = (AreaRedditi)ViewState["Redditi"];
            Response.Redirect(this.areaRedditi.Redditi.Url);
        }

        protected void event_ucAggiornamentoRedditi(object sender, EventArgs e)
        {
            Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
            this.areaRedditi = (AreaRedditi)ViewState["Redditi"];
            this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
            ucRedditi.areaRedditi = this.areaRedditi;
            PresenterRedditi presenterRedditi = new PresenterRedditi();
            this.IsSalvataggio = false;
            presenterRedditi.SalvaRedditi(this);
            ucRedditi.areaRedditi = this.areaRedditi;
            ucRedditi.ValorizzaRedditi();
            if (HasError)
            {
                ucAvviso.Visible = true;
                if (areaRedditi.Redditi.Esito == GestioneRedditiTipoRitornoRedditi.Informativa)
                {
                    ucAvviso.Tipo = INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.TipoAvviso.Info;
                    ucAvviso.Messaggio = areaRedditi.Redditi.MessaggioVideo;
                    //btnSalva.Enabled = true;
                }
                else if (areaRedditi.Redditi.Esito == GestioneRedditiTipoRitornoRedditi.Errore)
                {
                    if (this.ErrorMessage.Contains(_service_error))
                    {
                        this.ErrorMessage = this.ErrorMessage.Substring(_service_error.Length);
                        ucAvviso.Tipo = TipoAvviso.Ko;
                        ucAvviso.Messaggio = this.ErrorMessage;
                    }
                    else
                    {
                        ucAvviso.Tipo = TipoAvviso.Warning;
                        ucAvviso.Messaggio = areaRedditi.Redditi.MessaggioVideo;
                        //btnSalva.Enabled = false;
                    }
                }
            }
            else
            {
                ucAvviso.Visible = false;
                ucAvviso.Messaggio = "";
            }
            ViewState["Redditi"] = this.areaRedditi;
            return;
        }

        protected void event_ucEliminazioneRedditi(object sender, EventArgs e)
        {
            PresenterRedditi presenterRedditi = new PresenterRedditi();
            this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
            presenterRedditi.EliminaRedditi(this);
            ucRedditi.areaRedditi = this.areaRedditi;
            ucRedditi.ValorizzaRedditi();
            ViewState["Redditi"] = this.areaRedditi;
            if (HasError)
            {
                ucAvviso.Tipo = TipoAvviso.Warning;
                ucAvviso.Visible = true;

                ucAvviso.Messaggio = areaRedditi.Redditi.MessaggioVideo;
                if (areaRedditi.Redditi.Esito == GestioneRedditiTipoRitornoRedditi.Informativa)
                {
                    ucAvviso.Tipo = TipoAvviso.Info;
                }
                else if (areaRedditi.Redditi.Esito == GestioneRedditiTipoRitornoRedditi.Errore)
                {
                    if (this.ErrorMessage.Contains(_service_error))
                    {
                        this.ErrorMessage = this.ErrorMessage.Substring(_service_error.Length);
                        ucAvviso.Tipo = TipoAvviso.Ko;
                        ucAvviso.Messaggio = this.ErrorMessage;
                    }
                    else
                    {
                        ucAvviso.Tipo = TipoAvviso.Warning;
                    }
                }
            }
            else
            {
                ucAvviso.Visible = true;
                ucAvviso.Tipo = TipoAvviso.Ok;
                ucAvviso.Messaggio = "Dati Redditi eliminati correttamente.";
            }

            this.areaInfoPratica = new AreaInfoPratica();
            List<AreaQuadri.Tab> elencoTab = new List<AreaQuadri.Tab>();
            elencoTab.Add(AreaQuadri.Tab.Redditi);
            elencoTab.Add(AreaQuadri.Tab.RichiestaBonus);
            this.areaInfoPratica.ElencoTab = elencoTab.ToArray();

            CodeUtility.AggiornaSemafori(this, this, ucInfoLiquidazione);
        }

        private void CaricaRedditi()
        {
            PresenterRedditi presenterRedditi = new PresenterRedditi();
            this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
            presenterRedditi.GetRedditi(this);
            ucRedditi.areaRedditi = this.areaRedditi;
            ViewState["Redditi"] = this.areaRedditi;
            if (HasError)
            {
                ucAvviso.Visible = true;
                if (areaRedditi.Redditi.Esito == GestioneRedditiTipoRitornoRedditi.Informativa)
                {
                    ucAvviso.Tipo = INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.TipoAvviso.Info;
                    ucAvviso.Messaggio = areaRedditi.Redditi.MessaggioVideo;
                    //btnSalva.Enabled = true;
                }
                else if (areaRedditi.Redditi.Esito == GestioneRedditiTipoRitornoRedditi.Errore)
                {
                    if (this.ErrorMessage.Contains(_service_error))
                    {
                        this.ErrorMessage = this.ErrorMessage.Substring(_service_error.Length);
                        ucAvviso.Tipo = TipoAvviso.Ko;
                        ucAvviso.Messaggio = this.ErrorMessage;
                    }
                    else
                    {
                        ucAvviso.Tipo = TipoAvviso.Warning;
                        ucAvviso.Messaggio = areaRedditi.Redditi.MessaggioVideo;
                        //btnSalva.Enabled = false;
                    }
                    return;
                }
            }
        }

        protected void SalvaRedditi(Object sender, EventArgs e)
        {
            PresenterRedditi presenterRedditi = new PresenterRedditi();
            this.areaRedditi = (AreaRedditi)ViewState["Redditi"];
            this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
            ucRedditi.areaRedditi = this.areaRedditi;
            this.IsSalvataggio = true;
            presenterRedditi.SalvaRedditi(this);
            if (HasError)
            {
                ucAvviso.Tipo = TipoAvviso.Warning;
                ucAvviso.Visible = true;

                ucAvviso.Messaggio = areaRedditi.Redditi.MessaggioVideo;
                ucRedditi.areaRedditi = this.areaRedditi;
                ucRedditi.ValorizzaRedditi();
                if (areaRedditi.Redditi.Esito == GestioneRedditiTipoRitornoRedditi.Informativa)
                {
                    ucAvviso.Tipo = TipoAvviso.Info;
                }
                else if (areaRedditi.Redditi.Esito == GestioneRedditiTipoRitornoRedditi.Errore)
                {
                    if (this.ErrorMessage.Contains(_service_error))
                    {
                        this.ErrorMessage = this.ErrorMessage.Substring(_service_error.Length);
                        ucAvviso.Tipo = TipoAvviso.Ko;
                        ucAvviso.Messaggio = this.ErrorMessage;
                    }
                    else
                    {
                        ucAvviso.Tipo = TipoAvviso.Warning;
                    }
                }
                //if(areaRedditi.Redditi.Esito == GestioneRedditiTipoRitornoRedditi.Informativa)
                //    btnSalva.Enabled = true;
                //else
                //    btnSalva.Enabled = false;
            }
            else
            {
                ucAvviso.Visible = true;
                ucAvviso.Tipo = TipoAvviso.Ok;
                ucAvviso.Messaggio = "Dati Redditi salvati correttamente.";
            }
            ViewState["Redditi"] = this.areaRedditi;

            this.areaInfoPratica = new AreaInfoPratica();
            List<AreaQuadri.Tab> elencoTab = new List<AreaQuadri.Tab>();
            elencoTab.Add(AreaQuadri.Tab.Redditi);
            elencoTab.Add(AreaQuadri.Tab.RichiestaBonus);
            this.areaInfoPratica.ElencoTab = elencoTab.ToArray();

            CodeUtility.AggiornaSemafori(this, this, ucInfoLiquidazione);
        }

    }
}

