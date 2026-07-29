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
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneAgo;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using INPS.DNA;
using INPS.DNA.Presenter.Interface;

namespace INPS.Pensioni.LiquidazionePensione.View.Web
{
    [NavigationRules(AllowedReferrer = AllowedReferrer.Application, CheckSequenceOnPostBack = false)]
    public partial class RichiestaBonus : CustomBasePage, IInfoLiquidazione, IRichiestaBonus, IQuadriSemafori
    {
        #region IInfoLiquidazione
        public InfoLiquidazione InfoLiquidazione { get; set; }
        #endregion IInfoLiquidazione

        #region IViewUI
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        #endregion IViewUI

        #region IRichiestaBonus
        public AreaRichiestaBonus areaRichiestaBonus { get; set; }
        public INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda { get; set; }
        #endregion IRichiestaBonus

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

                CaricaRichiestaBonus();
                ucRichiestaBonus.ValorizzaRichiestaBonus();
                ucEsitoPrenotazione.ValorizzaEsitoPrenotazione();

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
                AreaTitolare.DatiPensione datiPensione = (AreaTitolare.DatiPensione)Session["DatiPensione"];
                ValorizzaSemaforiTab(imgRichiestaBonus, this.areaQuadri.QuadroRichiestaBonus.TabRichiestaBonus, pnlTabRichiestaBonus);
                if (datiPensione != null && datiPensione.IsRichiestaBonus.HasValue && datiPensione.IsRichiestaBonus.Value && this.domanda != null &&
                    this.domanda.Stato == Utility.GetDescription(CodeUtility.StatoPensione.Calcolata))
                {
                    this.areaQuadri.QuadroRichiestaBonus.TabEsitoPrenotazione = AreaQuadri.Semaforo.Rosso_Abilitato;
                }

                ValorizzaSemaforiTab(imgEsitoPrenotazione, this.areaQuadri.QuadroRichiestaBonus.TabEsitoPrenotazione, pnlTabEsitoPrenotazione);
            }
        }

        private void CaricaRichiestaBonus()
        {
            PresenterRichiestaBonus presenterRichiestaBonus = new PresenterRichiestaBonus();
            this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
            bool isDataFromDb = presenterRichiestaBonus.GetRichiestaBonus(this);

            ucRichiestaBonus.areaRichiestaBonus = this.areaRichiestaBonus;
            ucEsitoPrenotazione.areaRichiestaBonus = this.areaRichiestaBonus;
            ViewState["RichiestaBonus"] = this.areaRichiestaBonus;

            if (HasError)
            {
                ucAvviso.Visible = true;
                if (areaRichiestaBonus.RichiestaBonus.Esito == GestioneRichiestaBonusTipoRitornoRichiestaBonus.Errore)
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
                        ucAvviso.Messaggio = areaRichiestaBonus.RichiestaBonus.MessaggioVideo;
                    }
                    btnSalva.Enabled = false;
                    ucRichiestaBonus.EnabledBtnEliminaAnniRichiestaBonus(false);
                    ucRichiestaBonus.EnabledBtnSalvaAnniRichiestaBonus(false);
                    return;
                }
            }
            else
            {
                ucRichiestaBonus.EnabledBtnEliminaAnniRichiestaBonus(isDataFromDb);
            }
        }

        protected void SalvaRichiestaBonus(Object sender, EventArgs e)
        {
            PresenterRichiestaBonus presenterRichiestaBonus = new PresenterRichiestaBonus();
            this.areaRichiestaBonus = (AreaRichiestaBonus)ViewState["RichiestaBonus"];
            this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
            ucRichiestaBonus.areaRichiestaBonus = this.areaRichiestaBonus;
            ucRichiestaBonus.RecuperaCampi(this.areaRichiestaBonus);

            if (HiddenFieldIsConfermato.Value != "SI" && this.areaRichiestaBonus != null && this.areaRichiestaBonus.RichiestaBonus != null && this.areaRichiestaBonus.RichiestaBonus.DatiAnniRichiestaBonus != null &&
                this.areaRichiestaBonus.RichiestaBonus.DatiAnniRichiestaBonus.Count() > 0)
            {
                if (this.areaRichiestaBonus.RichiestaBonus.DatiAnniRichiestaBonus.Any(x => x.IsRichiestaBonus && x.Prescrizione == 1))
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "CallShowPopUpPrescrizione", "<script>ShowPopUpPrescrizione();</script>", false);
                    return;
                }
            }

            presenterRichiestaBonus.SalvaRichiestaBonus(this);
            ucRichiestaBonus.areaRichiestaBonus = this.areaRichiestaBonus;

            if (HasError)
            {
                ucAvviso.Tipo = TipoAvviso.Warning;
                ucAvviso.Visible = true;

                ucAvviso.Messaggio = areaRichiestaBonus.RichiestaBonus.MessaggioVideo;

                if (areaRichiestaBonus.RichiestaBonus.Esito == GestioneRichiestaBonusTipoRitornoRichiestaBonus.Errore)
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
                ucAvviso.Messaggio = "Dati Richiesta Bonus salvati correttamente.";
                ucRichiestaBonus.EnabledBtnEliminaAnniRichiestaBonus(true);
            }
            ViewState["RichiestaBonus"] = this.areaRichiestaBonus;
            ucRichiestaBonus.ValorizzaRichiestaBonus();
            this.areaInfoPratica = new AreaInfoPratica();
            List<AreaQuadri.Tab> elencoTab = new List<AreaQuadri.Tab>();
            elencoTab.Add(AreaQuadri.Tab.RichiestaBonus);
            this.areaInfoPratica.ElencoTab = elencoTab.ToArray();

            CodeUtility.AggiornaSemafori(this, this, ucInfoLiquidazione);

            HiddenFieldIsConfermato.Value = "";
        }

        #region Events

        protected void event_ucEliminazioneRichiestaBonus(object sender, EventArgs e)
        {
            PresenterRichiestaBonus presenterRichiestaBonus = new PresenterRichiestaBonus();
            this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
            presenterRichiestaBonus.EliminaRichiestaBonus(this);
            ucRichiestaBonus.areaRichiestaBonus = this.areaRichiestaBonus;
            ucRichiestaBonus.ValorizzaRichiestaBonus();
            ViewState["RichiestaBonus"] = this.areaRichiestaBonus;
            if (HasError)
            {
                ucAvviso.Tipo = TipoAvviso.Warning;
                ucAvviso.Visible = true;

                ucAvviso.Messaggio = areaRichiestaBonus.RichiestaBonus.MessaggioVideo;
                if (areaRichiestaBonus.RichiestaBonus.Esito == GestioneRichiestaBonusTipoRitornoRichiestaBonus.Errore)
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
                    btnSalva.Enabled = false;
                    ucRichiestaBonus.EnabledBtnEliminaAnniRichiestaBonus(false);
                    ucRichiestaBonus.EnabledBtnSalvaAnniRichiestaBonus(false);
                }
            }
            else
            {
                ucAvviso.Visible = true;
                ucAvviso.Tipo = TipoAvviso.Ok;
                ucAvviso.Messaggio = "Dati Richiesta Bonus eliminati correttamente.";
            }

            this.areaInfoPratica = new AreaInfoPratica();
            List<AreaQuadri.Tab> elencoTab = new List<AreaQuadri.Tab>();
            elencoTab.Add(AreaQuadri.Tab.RichiestaBonus);
            this.areaInfoPratica.ElencoTab = elencoTab.ToArray();

            CodeUtility.AggiornaSemafori(this, this, ucInfoLiquidazione);
        }

        protected void event_ucHideAvviso(object sender, EventArgs e)
        {
            ucAvviso.Visible = false;
            ucAvviso.Messaggio = string.Empty;
        }

        protected void event_ucShowAvviso(object sender, EventArgs e)
        {
            this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
            if (((IViewUI)sender).HasError)
            {
                ucAvviso.Tipo = TipoAvviso.Warning;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = ((IViewUI)sender).ErrorMessage;
            }
            else
            {
                ucAvviso.Tipo = TipoAvviso.Ok;
                ucAvviso.Messaggio = ((IViewUI)sender).ErrorMessage;// "Dati Richiesta Bonus salvati correttamente";
                ucAvviso.Visible = true;

                this.areaInfoPratica = new AreaInfoPratica();
                List<AreaQuadri.Tab> elencoTab = new List<AreaQuadri.Tab>();
                elencoTab.Add(AreaQuadri.Tab.RichiestaBonus);
                this.areaInfoPratica.ElencoTab = elencoTab.ToArray();

                CodeUtility.AggiornaSemafori(this, this, ucInfoLiquidazione);
            }
        }

        #endregion Events
    }
}