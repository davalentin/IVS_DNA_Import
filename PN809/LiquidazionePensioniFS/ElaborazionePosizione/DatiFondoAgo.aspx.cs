using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.Contract;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneAgo;
using INPS.DNA.Presenter.Interface;
using INPS.Pensioni.LiquidazionePensione.View.Web.UserControls;

namespace INPS.Pensioni.LiquidazionePensione.View.Web
{
    public partial class DatiFondoAgo : CustomBasePage, IInfoLiquidazione, IQuadriSemafori, IDatiFondoAgo
    {
        #region IInfoLiquidazione
        public InfoLiquidazione InfoLiquidazione { get; set; }
        #endregion

        #region IQuadriSemafori
        public AreaQuadri areaQuadri { get; set; }
        public AreaInfoPratica areaInfoPratica { get; set; }
        public AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda { get; set; }
        #endregion

        #region IViewUI
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        #endregion

        #region IDatiFondo
        public AreaDatiFondo areaDatiFondo { get; set; }
        #endregion IDatiFondo

        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.domanda == null)
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            if (!IsPostBack)
            {
                this.InfoLiquidazione = ValorizzaInfoLiquidazione(ucInfoLiquidazione);

                InitData();
            }

            ucAvviso.Visible = false;
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            if ((AreaQuadri)Session["Semaforo"] != null)
            {
                this.areaQuadri = (AreaQuadri)Session["Semaforo"];
                this.areaDatiFondo = (AreaDatiFondo)ViewState[EnumViewState.AreaDatiFondo.ToString()];
                if (ViewState[EnumViewState.IsRecordSelezionato.ToString()] != null && (bool)ViewState[EnumViewState.IsRecordSelezionato.ToString()])
                {
                    // L'enum AreaQuadri.Semaforo è il seguente
                    // Rosso_NonAbilitato = 0,
                    // Rosso_Abilitato = 1,
                    // Giallo = 2,
                    // Verde = 3
                    // Il byte avrà i seguenti valori
                    // Rosso_NonAbilitato = null,
                    // Rosso_Abilitato = 0,
                    // Giallo = 1,
                    // Verde = 2
                    // Quindi per avere il valore corretto nel caso in cui sia diverso da null bisogna fare il valore del byte + 1

                    ValorizzaSemaforiTab(imgRegistrazioniFondo, AreaQuadri.Semaforo.Rosso_NonAbilitato, pnlTabRegistrazioniFondo);
                    if (this.areaDatiFondo.DatiFondo.Semaforo.HasValue)
                        ValorizzaSemaforiTab(imgDatiFondo, (AreaQuadri.Semaforo)Enum.Parse(typeof(AreaQuadri.Semaforo), (this.areaDatiFondo.DatiFondo.Semaforo + 1).ToString()), pnlTabDatiFondo);
                    else
                        ValorizzaSemaforiTab(imgDatiFondo, AreaQuadri.Semaforo.Rosso_NonAbilitato, pnlTabDatiFondo);

                    if (this.areaDatiFondo.DatiCalcolo.Semaforo.HasValue)
                        ValorizzaSemaforiTab(imgDatiCalcolo, (AreaQuadri.Semaforo)Enum.Parse(typeof(AreaQuadri.Semaforo), (this.areaDatiFondo.DatiCalcolo.Semaforo + 1).ToString()), pnlTabDatiCalcolo);
                    else
                        ValorizzaSemaforiTab(imgDatiCalcolo, AreaQuadri.Semaforo.Rosso_NonAbilitato, pnlTabDatiCalcolo);

                    if (this.areaDatiFondo.DatiPrivilegiate.Semaforo.HasValue)
                        ValorizzaSemaforiTab(imgPrivilegiate, (AreaQuadri.Semaforo)Enum.Parse(typeof(AreaQuadri.Semaforo), (this.areaDatiFondo.DatiPrivilegiate.Semaforo + 1).ToString()), pnlTabPrivilegiate);
                    else
                        ValorizzaSemaforiTab(imgPrivilegiate, AreaQuadri.Semaforo.Rosso_NonAbilitato, pnlTabPrivilegiate);

                    if (this.areaDatiFondo.DatiArticolo2.Semaforo.HasValue)
                        ValorizzaSemaforiTab(imgArticolo2, (AreaQuadri.Semaforo)Enum.Parse(typeof(AreaQuadri.Semaforo), (this.areaDatiFondo.DatiArticolo2.Semaforo + 1).ToString()), pnlTabArticolo2);
                    else
                        ValorizzaSemaforiTab(imgArticolo2, AreaQuadri.Semaforo.Rosso_NonAbilitato, pnlTabArticolo2);
                }
                else
                {
                    ValorizzaSemaforiTab(imgRegistrazioniFondo, this.areaQuadri.QuadroDatiFondo.TabRegistrazioniFondo, pnlTabRegistrazioniFondo);
                    ValorizzaSemaforiTab(imgDatiFondo, AreaQuadri.Semaforo.Rosso_NonAbilitato, pnlTabDatiFondo);
                    ValorizzaSemaforiTab(imgDatiCalcolo, AreaQuadri.Semaforo.Rosso_NonAbilitato, pnlTabDatiCalcolo);
                    ValorizzaSemaforiTab(imgPrivilegiate, AreaQuadri.Semaforo.Rosso_NonAbilitato, pnlTabPrivilegiate);
                    ValorizzaSemaforiTab(imgArticolo2, AreaQuadri.Semaforo.Rosso_NonAbilitato, pnlTabArticolo2);
                }
            }
        }

        protected void SalvaFondo_Click(object sender, EventArgs e)
        {
            if (this.domanda == null)
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            if (this.areaDatiFondo == null)
                this.areaDatiFondo = new AreaDatiFondo();
            this.areaDatiFondo.IdRecordFondo = (long)ViewState[EnumViewState.IdRecordFondoSelezionato.ToString()];

            this.areaDatiFondo.DatiFondo = ucDatiFondo.RecuperaCampi();
            this.areaDatiFondo.DatiCalcolo = ucDatiCalcolo.RecuperaCampi();
            this.areaDatiFondo.DatiPrivilegiate = ucPrivilegiate.RecuperaCampi();
            this.areaDatiFondo.DatiArticolo2 = ucArticolo2.RecuperaCampi();

            Presenter.PresenterDatiFondoAgo presenter = new Presenter.PresenterDatiFondoAgo();
            presenter.StoreQuadroDatiFondoByIdRecordFondo(this);

            if (!this.HasError)
                this.ErrorMessage = "Dati Fondo salvati correttamente.";

            event_ucShowAvviso(this, null);
            event_ucUpdateSemaforoDatiFondo(this, null);
            event_ucUpdateSemaforoDatiCalcolo(this, null);
            event_ucUpdateSemaforoDatiPrivilegiate(this, null);
            event_ucUpdateSemaforoDatiArticolo2(this, null);
        }

        #region private methods
        private void InitData()
        {
            Presenter.PresenterDatiFondoAgo presenter = new Presenter.PresenterDatiFondoAgo();
            presenter.GetDatiFondo(this);

            ViewState[EnumViewState.AreaDatiFondo.ToString()] = this.areaDatiFondo;

            ucRegistrazioniFondo.ValorizzaEtichette(this.areaDatiFondo);
            ViewState[EnumViewState.IsRecordSelezionato.ToString()] = false;
        }
        #endregion private methods

        #region Events
        protected void event_ucShowAvviso(object sender, EventArgs e)
        {
            if (((IViewUI)sender).HasError)
            {
                ucAvviso.Tipo = TipoAvviso.Warning;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = ((IViewUI)sender).ErrorMessage;
            }
            else
            {
                ucAvviso.Tipo = TipoAvviso.Ok;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = ((IViewUI)sender).ErrorMessage;// "Dati Assicurativi salvati correttamente";

                this.areaInfoPratica = new AreaInfoPratica();
                List<AreaQuadri.Tab> elencoTab = new List<AreaQuadri.Tab>();
                elencoTab.Add(AreaQuadri.Tab.DatiFondo);
                this.areaInfoPratica.ElencoTab = elencoTab.ToArray();

                CodeUtility.AggiornaSemafori(this, this, ucInfoLiquidazione);
            }
        }

        protected void event_ucUpdateSemaforoDatiFondo(object sender, EventArgs e)
        {
            IDatiFondoAgo interfaccia = (IDatiFondoAgo)sender;
            ((AreaDatiFondo)ViewState[EnumViewState.AreaDatiFondo.ToString()]).DatiFondo.Semaforo = interfaccia.areaDatiFondo.DatiFondo.Semaforo;
        }

        protected void event_ucUpdateSemaforoDatiCalcolo(object sender, EventArgs e)
        {
            IDatiFondoAgo interfaccia = (IDatiFondoAgo)sender;
            ((AreaDatiFondo)ViewState[EnumViewState.AreaDatiFondo.ToString()]).DatiCalcolo.Semaforo = interfaccia.areaDatiFondo.DatiCalcolo.Semaforo;
        }

        protected void event_ucUpdateSemaforoDatiPrivilegiate(object sender, EventArgs e)
        {
            IDatiFondoAgo interfaccia = (IDatiFondoAgo)sender;
            ((AreaDatiFondo)ViewState[EnumViewState.AreaDatiFondo.ToString()]).DatiPrivilegiate.Semaforo = interfaccia.areaDatiFondo.DatiPrivilegiate.Semaforo;
        }

        protected void event_ucUpdateSemaforoDatiArticolo2(object sender, EventArgs e)
        {
            IDatiFondoAgo interfaccia = (IDatiFondoAgo)sender;
            ((AreaDatiFondo)ViewState[EnumViewState.AreaDatiFondo.ToString()]).DatiArticolo2.Semaforo = interfaccia.areaDatiFondo.DatiArticolo2.Semaforo;
        }

        protected void event_ucShowPulsanteSalva(object sender, EventArgs e)
        {
            btnSalvaFondo.Visible = true;
        }

        protected void event_ucHidePulsanteSalva(object sender, EventArgs e)
        {
            btnSalvaFondo.Visible = false;
        }

        protected void event_ucRecordSelezionato(object sender, EventArgs e)
        {
            ViewState[EnumViewState.IsRecordSelezionato.ToString()] = true;
            hdnSelected.Value = "#dati_Fondo";

            IDatiFondoAgo interfaccia = (IDatiFondoAgo)sender;
            this.areaDatiFondo = interfaccia.areaDatiFondo;
            ViewState[EnumViewState.AreaDatiFondo.ToString()] = this.areaDatiFondo;
            ViewState[EnumViewState.IdRecordFondoSelezionato.ToString()] = this.areaDatiFondo.IdRecordFondo;

            this.areaInfoPratica = new AreaInfoPratica();
            List<AreaQuadri.Tab> elencoTab = new List<AreaQuadri.Tab>();
            elencoTab.Add(AreaQuadri.Tab.DatiFondo);
            this.areaInfoPratica.ElencoTab = elencoTab.ToArray();
            CodeUtility.AggiornaSemafori(this, this, ucInfoLiquidazione);

            ucDatiFondo.ValorizzaEtichette(this.areaDatiFondo);
            ucDatiCalcolo.ValorizzaEtichette(this.areaDatiFondo);
            ucPrivilegiate.ValorizzaEtichette(this.areaDatiFondo);
            ucArticolo2.ValorizzaEtichette(this.areaDatiFondo);
        }

        protected void event_ucTornaARegistrazioniFondo(object sender, EventArgs e)
        {
            ViewState[EnumViewState.IsRecordSelezionato.ToString()] = false;
            hdnSelected.Value = "#dati_Registrazioni";
            ViewState[EnumViewState.IdRecordFondoSelezionato.ToString()] = null;

            InitData();
        }
        #endregion Events

        public enum EnumViewState
        {
            IsRecordSelezionato,
            AreaDatiFondo,
            IdRecordFondoSelezionato
        }
    }
}