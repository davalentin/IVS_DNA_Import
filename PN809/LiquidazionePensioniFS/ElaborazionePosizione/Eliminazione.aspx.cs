using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using INPS.DNA.UI.Web;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using INPS.Pensioni.LiquidazionePensione.View.Web.UserControls;
using INPS.Pensioni.LiquidazionePensione.Presenter.Contract;
using INPS.Pensioni.LiquidazionePensione.Presenter;

namespace INPS.Pensioni.LiquidazionePensione.View.Web
{
    [NavigationRules(AllowedReferrer = AllowedReferrer.Application, CheckSequenceOnPostBack = false)]
    public partial class Eliminazione : CustomBasePage, IQuadriSemafori, IEliminazione, IInfoLiquidazione, ITitolarePensione
    {
        #region IInfoLiquidazione
        public InfoLiquidazione InfoLiquidazione { get; set; }
        #endregion IInfoLiquidazione

        #region IEliminazione

        public AreaEliminazione areaEliminazione { get; set; }
        public Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda { get; set; }

        #endregion IEliminazione

        #region ITitolare
        public AreaTitolare TitolarePensione { get; set; }
        #endregion ITitolare

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
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            if (!Page.IsPostBack)
            {
                this.InfoLiquidazione = ValorizzaInfoLiquidazione(ucInfoLiquidazione);
                CaricaDati();
                //RenderSemafori();
                if (Session["Pratiche"] != null)
                {
                    btnTornaPosizioni.Visible = true;
                    btnTornaARicerca.Visible = false;
                }
            }
        }

        private void CaricaDati()
        {
            AreaTitolare.DatiPensione datiPensione = new AreaTitolare.DatiPensione();
            datiPensione = GetDatiPensione(this);
            //bool IsDomandaSperDonna = CodeUtility.IsDomandaSperimentaleDonna(datiPensione);

            PresenterEliminazione presenterEliminazione = new PresenterEliminazione();
            presenterEliminazione.GetDatiEliminazione(this);
            if (HasError)
            {
                ucAvviso.Visible = true;
                ucAvviso.Tipo = TipoAvviso.Ko;
                ucAvviso.Messaggio = ErrorMessage;
                pnlEliminazione.Enabled = false;
                return;
            }

            if (Utility.IsDomandaIndennitaUnaTantum_AGO(datiPensione))
            {
                AreaDanteCausa areaDanteCausa;
                presenterEliminazione.GetDatiDanteCausa(this, out areaDanteCausa);
                if (areaDanteCausa != null && areaDanteCausa.AnagraficaDC != null && areaDanteCausa.AnagraficaDC.DataMorte.HasValue)
                    Session["DataMorteDanteCausa"] = areaDanteCausa.AnagraficaDC.DataMorte;
            }
            ucEliminazione.areaEliminazione = this.areaEliminazione;
            ucEliminazione.SetHiddenFieldIsRicostituzione();
            ucEliminazione.ValorizzaEtichette();
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            if ((AreaQuadri)Session["Semaforo"] != null)
            {
                domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
                this.areaQuadri = (AreaQuadri)Session["Semaforo"];
                ValorizzaSemaforiTab(imgEliminazione, this.areaQuadri.QuadroEliminazione.TabEliminazione, pnlEliminazione);
            }
        }

        protected void SalvaEliminzione_Click(object sender, EventArgs e)
        {
            areaEliminazione = new AreaEliminazione();
            PresenterEliminazione presenterEliminazione = new PresenterEliminazione();

            ucEliminazione.RecuperaCampi();
            this.areaEliminazione = ucEliminazione.areaEliminazione;
            presenterEliminazione.SalvaDatiEliminazione(this);

            if (this.HasError)
            {
                ucAvviso.Tipo = TipoAvviso.Warning;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = this.ErrorMessage;
            }
            else
            {
                ucAvviso.Tipo = TipoAvviso.Ok;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = "Dati Eliminazione salvati correttamente";
            }

            this.areaInfoPratica = new AreaInfoPratica();
            List<AreaQuadri.Tab> elencoTab = new List<AreaQuadri.Tab>();
            elencoTab.Add(AreaQuadri.Tab.Eliminazione);
            elencoTab.Add(AreaQuadri.Tab.Redditi);
            this.areaInfoPratica.ElencoTab = elencoTab.ToArray();

            CodeUtility.AggiornaSemafori(this, this, ucInfoLiquidazione);
        }

        protected void event_ucShowAvvisoEliminaEliminazione(object sender, EventArgs e)
        {
            INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.Eliminazione.UCEliminazione tabElim = (INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.Eliminazione.UCEliminazione)sender;
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            this.areaInfoPratica = new AreaInfoPratica();
            List<AreaQuadri.Tab> elencoTab = new List<AreaQuadri.Tab>();
            elencoTab.Add(AreaQuadri.Tab.Eliminazione);

            this.areaInfoPratica.ElencoTab = elencoTab.ToArray();

            CodeUtility.AggiornaSemafori(this, this, ucInfoLiquidazione);

            if (tabElim.HasError)
            {
                ucAvviso.Tipo = TipoAvviso.Warning;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = tabElim.ErrorMessage;
            }
            else
            {
                ucAvviso.Tipo = TipoAvviso.Ok;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = "Dati Eliminazione eliminati correttamente";
            }
        }

        protected void event_ucShowAvvisoEliminazione(object sender, EventArgs e)
        {

            INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.Eliminazione.UCEliminazione tabEliminazione = (INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.Eliminazione.UCEliminazione)sender;
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            this.areaInfoPratica = new AreaInfoPratica();
            List<AreaQuadri.Tab> elencoTab = new List<AreaQuadri.Tab>();
            elencoTab.Add(AreaQuadri.Tab.Eliminazione);
            elencoTab.Add(AreaQuadri.Tab.Redditi);
            this.areaInfoPratica.ElencoTab = elencoTab.ToArray();

            CodeUtility.AggiornaSemafori(this, this, ucInfoLiquidazione);

            if (tabEliminazione.HasError)
            {
                ucAvviso.Tipo = TipoAvviso.Warning;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = tabEliminazione.ErrorMessage;
            }
            else
            {
                ucAvviso.Tipo = TipoAvviso.Ok;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = "Dati Eliminazione salvati correttamente";
            }
        }
    }
}