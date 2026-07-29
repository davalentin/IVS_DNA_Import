using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs;
using INPS.Pensioni.LiquidazionePensione.Presenter.Contract;
using INPS.DNA.Presenter.Interface;
using INPS.Pensioni.LiquidazionePensione.View.Web.UserControls;

namespace INPS.Pensioni.LiquidazionePensione.View.Web
{
    public partial class DatiNoCalcolo : CustomBasePage, IQuadriSemafori, IDatiNoCalcolo, IInfoLiquidazione
    {
        #region IInfoLiquidazione
        public InfoLiquidazione InfoLiquidazione { get; set; }
        #endregion IInfoLiquidazione

        #region IQuadriSemafori
        public AreaQuadri areaQuadri { get; set; }
        public AreaInfoPratica areaInfoPratica { get; set; }

        #endregion IQuadriSemafori

        #region IViewUI
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        #endregion IViewUI

        #region IDatiNoCalcolo

        public Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda { get; set; }
        public long IdRecordNoCalcolo { get; set; }
        public AreaNoCalcolo AreaNoCalcolo { get; set; }
        #endregion IDatiNoCalcolo

        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.domanda == null)
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            if (!IsPostBack)
            {
                this.InfoLiquidazione = ValorizzaInfoLiquidazione(ucInfoLiquidazione);
                InitData();
                if (Session["Pratiche"] != null)
                {
                    btnTornaPosizioni.Visible = true;
                    btnTornaARicerca.Visible = false;
                }
            }

            ucAvviso.Visible = false;
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            if ((AreaQuadri)Session["Semaforo"] != null)
            {
                this.areaQuadri = (AreaQuadri)Session["Semaforo"];
                this.AreaNoCalcolo = (AreaNoCalcolo)ViewState[VS_DatiNoCalcolo.AreaDatiNoCalcolo];
                if (ViewState[VS_DatiNoCalcolo.RecordSelezionato] != null && (bool)ViewState[VS_DatiNoCalcolo.RecordSelezionato])
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

                    ValorizzaSemaforiTab(imgRegistrazioniNoCalcolo, AreaQuadri.Semaforo.Rosso_NonAbilitato, pnlTabRegistrazioniNoCalcolo);

                    if (this.AreaNoCalcolo.DatiNoCalcolo.TabNoCalcolo.HasValue)
                        ValorizzaSemaforiTab(imgDatiNoCalcolo, (AreaQuadri.Semaforo)Enum.Parse(typeof(AreaQuadri.Semaforo), (this.AreaNoCalcolo.DatiNoCalcolo.TabNoCalcolo + 1).ToString()), pnlTabDatiNoCalcolo);
                }
                else
                {
                    ValorizzaSemaforiTab(imgRegistrazioniNoCalcolo, this.areaQuadri.QuadroDatiNoCalcolo.TabRecordNoCalcolo, pnlTabRegistrazioniNoCalcolo);
                    ValorizzaSemaforiTab(imgDatiNoCalcolo, AreaQuadri.Semaforo.Rosso_NonAbilitato, pnlTabDatiNoCalcolo);
                }
            }
        }

        protected void SalvaDatiNoCalcolo_Click(object sender, EventArgs e)
        {
            if (this.domanda == null)
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            this.AreaNoCalcolo = new AreaNoCalcolo();
            this.AreaNoCalcolo.DatiNoCalcolo = new Presenter.SvrLiquidazioneFs.DatiNoCalcolo();
            this.IdRecordNoCalcolo = ucDatiNoCalcolo.GetIdRecordNoCalcolo();
            this.AreaNoCalcolo.DatiNoCalcolo = ucDatiNoCalcolo.RecuperaCampi();
            Presenter.PresenterNoCalcolo presenter = new Presenter.PresenterNoCalcolo();
            presenter.StoreDatiNoCalcoloByIdRecord(this);

            //((AreaNoCalcolo)ViewState[VS_DatiNoCalcolo.AreaDatiNoCalcolo]).DatiNoCalcolo.TabNoCalcolo = areaNoCalcolo.DatiNoCalcolo.TabNoCalcolo;
            if (!this.HasError)
                this.ErrorMessage = "Dati No Calcolo salvati correttamente.";

            event_ucShowAvviso(this, null);
            event_ucUpdateSemaforoDatiNoCalcolo(this, null);
        }



        #region private methods
        private void InitData()
        {
            if (this.domanda == null)
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            Presenter.PresenterNoCalcolo presenter = new Presenter.PresenterNoCalcolo();
            presenter.GetRecordNoCalcolo(this);
            if (this.AreaNoCalcolo.LstRecordNoCalcolo != null && this.AreaNoCalcolo.LstRecordNoCalcolo.Count() > 0)
                ucRecordNoCalcolo.ValorizzaEtichette(this.AreaNoCalcolo);
            else
            {
                ucDatiNoCalcolo.ValorizzaEtichette(this);
                event_ucRecordSelezionato(this, null);
            }

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
                elencoTab.Add(AreaQuadri.Tab.DatiNoCalcolo);
                this.areaInfoPratica.ElencoTab = elencoTab.ToArray();

                CodeUtility.AggiornaSemafori(this, this, ucInfoLiquidazione);
            }
        }

        protected void event_ucUpdateSemaforoDatiNoCalcolo(object sender, EventArgs e)
        {
            IDatiNoCalcolo interfaccia = (IDatiNoCalcolo)sender;
            ((AreaNoCalcolo)ViewState[VS_DatiNoCalcolo.AreaDatiNoCalcolo]).DatiNoCalcolo.TabNoCalcolo = interfaccia.AreaNoCalcolo.DatiNoCalcolo.TabNoCalcolo;
        }

        protected void event_ucShowPulsanteSalva(object sender, EventArgs e)
        {
            btnSalvaDatiNoCalcolo.Visible = true;
        }

        protected void event_ucHidePulsanteSalva(object sender, EventArgs e)
        {
            btnSalvaDatiNoCalcolo.Visible = false;
        }

        protected void event_ucRecordSelezionato(object sender, EventArgs e)
        {
            hdnSelected.Value = "#dati_NoCalcolo";

            IDatiNoCalcolo interfaccia = (IDatiNoCalcolo)sender;
            this.AreaNoCalcolo = interfaccia.AreaNoCalcolo;
            this.IdRecordNoCalcolo = interfaccia.IdRecordNoCalcolo;
            ViewState[VS_DatiNoCalcolo.AreaDatiNoCalcolo] = this.AreaNoCalcolo;
            ViewState[VS_DatiNoCalcolo.RecordSelezionato] = true;

            this.areaInfoPratica = new AreaInfoPratica();
            List<AreaQuadri.Tab> elencoTab = new List<AreaQuadri.Tab>();
            elencoTab.Add(AreaQuadri.Tab.DatiNoCalcolo);
            this.areaInfoPratica.ElencoTab = elencoTab.ToArray();
            CodeUtility.AggiornaSemafori(this, this, ucInfoLiquidazione);
            btnSalvaDatiNoCalcolo.Visible = true;
            ucDatiNoCalcolo.ValorizzaEtichette(this);

        }

        protected void event_ucTornaARegistrazioniFondo(object sender, EventArgs e)
        {
            ViewState[VS_DatiNoCalcolo.RecordSelezionato] = false;
            InitData();
            hdnSelected.Value = "#dati_Registrazioni";
            btnSalvaDatiNoCalcolo.Visible = false;
        }


        #endregion Events

        public static class VS_DatiNoCalcolo
        {
            public const string RecordSelezionato = "RecordSelezionato";
            public const string AreaDatiNoCalcolo = "AreaDatiNoCalcolo";
        }
    }


}