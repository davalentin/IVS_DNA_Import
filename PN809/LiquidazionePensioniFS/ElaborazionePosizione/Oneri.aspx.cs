using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using INPS.DNA.UI.Web;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.Contract;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using INPS.Pensioni.LiquidazionePensione.View.Web.UserControls;
using INPS.Pensioni.LiquidazionePensione.Presenter;

namespace INPS.Pensioni.LiquidazionePensione.View.Web
{
    [NavigationRules(AllowedReferrer = AllowedReferrer.Application, CheckSequenceOnPostBack = false)]
    public partial class Oneri : CustomBasePage, IInfoLiquidazione, IOneri, IQuadriSemafori
    {
        #region IInfoLiquidazione
        public InfoLiquidazione InfoLiquidazione { get; set; }
        #endregion IInfoLiquidazione

        #region IViewUI
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        #endregion IViewUI

        #region IOneri
        public Presenter.SvrLiquidazione.AreaOneri areaOneri { get; set; }
        public AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda { get; set; }
        #endregion IOneri

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
                CaricaDatiOneri();
                RenderSemafori();
                if (Session["Pratiche"] != null)
                {
                    btnTornaPosizioni.Visible = true;
                    btnTornaARicerca.Visible = false;
                }
            }
        }

        protected void SalvaOneri_Click(object sender, EventArgs e)
        {
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            areaOneri = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione.AreaOneri();
            PresenterOneri presenterOneri = new PresenterOneri();


            areaOneri.DatiOneriBenefParticolari = ucOneri.GetValoriOneri();

            areaOneri.DatiPrepensionamento = ucPrepensionamento.GetValoriPrepensionamento();

            presenterOneri.SalvaQuadroOneri(this);



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
                ucAvviso.Messaggio = "Dati Oneri salvati correttamente";
            }

            this.areaInfoPratica = new AreaInfoPratica();
            List<AreaQuadri.Tab> elencoTab = new List<AreaQuadri.Tab>();
            elencoTab.Add(AreaQuadri.Tab.Oneri);
            elencoTab.Add(AreaQuadri.Tab.Redditi);
            this.areaInfoPratica.ElencoTab = elencoTab.ToArray();

            CodeUtility.AggiornaSemafori(this, this, ucInfoLiquidazione);
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            if ((AreaQuadri)Session["Semaforo"] != null)
            {
                domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
                this.areaQuadri = (AreaQuadri)Session["Semaforo"];

                ValorizzaSemaforiTab(imgOneri, this.areaQuadri.QuadroOneri.TabOneri, pnlTabOneri);

                ValorizzaSemaforiTab(imgPrepensionamento, this.areaQuadri.QuadroOneri.TabPrepensionamento, pnlTabPrepensionamento);

                ValorizzaSemaforiTab(imgStorico, this.areaQuadri.QuadroOneri.TabStorico, pnlTabStorico);
            }
        }

        #region private methods
        private void RenderSemafori()
        {
            if ((AreaQuadri)Session["Semaforo"] != null)
            {
                domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
                this.areaQuadri = (AreaQuadri)Session["Semaforo"];

                if (this.areaQuadri.QuadroOneri.TabOneri != AreaQuadri.Semaforo.Rosso_NonAbilitato)
                    hdnSelected.Value = "#oneri";
                else if (this.areaQuadri.QuadroOneri.TabPrepensionamento != AreaQuadri.Semaforo.Rosso_NonAbilitato)
                    hdnSelected.Value = "#prepensionamento";
            }
        }

        private void CaricaDatiOneri()
        {
            AreaTitolare.DatiPensione datiPensione = (AreaTitolare.DatiPensione)Session["DatiPensione"];
            this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
            PresenterOneri presenterOneri = new PresenterOneri();
            presenterOneri.GetAreaOneri(this);
            if (HasError)
            {
                ucAvviso.Visible = true;
                ucAvviso.Tipo = TipoAvviso.Ko;
                ucAvviso.Messaggio = ErrorMessage;
                pnlOneri.Enabled = false;
                return;
            }

            ucOneri.ValorizzaEtichetteOneri(this);
            ucPrepensionamento.ValorizzaEtichettePrepensionamento(this);

            if (this.areaOneri.DatiOneriBenefParticolariStorico != null)
                ucOneriStoricoGP.ValorizzaEtichetteOneri(this);
        }
        #endregion private methods

        #region Events
        //protected void event_ucShowAvvisoExCombattente(object sender, EventArgs e)
        //{
        //    INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.MaggiorazioniEBeneficiAgo.UCExCombattente tabExCombattente = (INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.MaggiorazioniEBeneficiAgo.UCExCombattente)sender;
        //    if (this.domanda == null)
        //        this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

        //    this.areaInfoPratica = new AreaInfoPratica();
        //    List<AreaQuadri.Tab> elencoTab = new List<AreaQuadri.Tab>();
        //    elencoTab.Add(AreaQuadri.Tab.MaggiorazioniEBenefici);
        //    this.areaInfoPratica.ElencoTab = elencoTab.ToArray();

        //    CodeUtility.AggiornaSemafori(this, this, ucInfoLiquidazione);

        //    if (tabExCombattente.HasError)
        //    {
        //        ucAvviso.Tipo = TipoAvviso.Warning;
        //        ucAvviso.Visible = true;
        //        ucAvviso.Messaggio = tabExCombattente.ErrorMessage;
        //    }
        //    else
        //    {
        //        ucAvviso.Tipo = TipoAvviso.Ok;
        //        ucAvviso.Visible = true;
        //        ucAvviso.Messaggio = "Dati Ex Combattente salvati correttamente";
        //    }
        //}

        //protected void event_ucShowAvvisoEliminaExCombattente(object sender, EventArgs e)
        //{
        //    INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.MaggiorazioniEBeneficiAgo.UCExCombattente tabExCombattente = (INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.MaggiorazioniEBeneficiAgo.UCExCombattente)sender;
        //    if (this.domanda == null)
        //        this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

        //    this.areaInfoPratica = new AreaInfoPratica();
        //    List<AreaQuadri.Tab> elencoTab = new List<AreaQuadri.Tab>();
        //    elencoTab.Add(AreaQuadri.Tab.MaggiorazioniEBenefici);
        //    this.areaInfoPratica.ElencoTab = elencoTab.ToArray();

        //    CodeUtility.AggiornaSemafori(this, this, ucInfoLiquidazione);

        //    if (tabExCombattente.HasError)
        //    {
        //        ucAvviso.Tipo = TipoAvviso.Warning;
        //        ucAvviso.Visible = true;
        //        ucAvviso.Messaggio = tabExCombattente.ErrorMessage;
        //    }
        //    else
        //    {
        //        ucAvviso.Tipo = TipoAvviso.Ok;
        //        ucAvviso.Visible = true;
        //        ucAvviso.Messaggio = "Dati Ex Combattente eliminati correttamente";
        //    }
        //}

        //protected void event_ucShowAvvisoBenefici(object sender, EventArgs e)
        //{
        //    INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.Oneri.UCOneUCBenefici tabBenefici = (INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.MaggiorazioniEBeneficiAgo.UCBenefici)sender;
        //    if (this.domanda == null)
        //        this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

        //    this.areaInfoPratica = new AreaInfoPratica();
        //    List<AreaQuadri.Tab> elencoTab = new List<AreaQuadri.Tab>();
        //    elencoTab.Add(AreaQuadri.Tab.MaggiorazioniEBenefici);
        //    this.areaInfoPratica.ElencoTab = elencoTab.ToArray();

        //    CodeUtility.AggiornaSemafori(this, this, ucInfoLiquidazione);

        //    if (tabBenefici.HasError)
        //    {
        //        ucAvviso.Tipo = TipoAvviso.Warning;
        //        ucAvviso.Visible = true;
        //        ucAvviso.Messaggio = tabBenefici.ErrorMessage;
        //    }
        //    else
        //    {
        //        ucAvviso.Tipo = TipoAvviso.Ok;
        //        ucAvviso.Visible = true;
        //        ucAvviso.Messaggio = "Dati Benefici salvati correttamente";
        //    }
        //}

        //protected void event_ucShowAvvisoEliminaBenefici(object sender, EventArgs e)
        //{
        //    INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.MaggiorazioniEBeneficiAgo.UCBenefici tabBenefici = (INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.MaggiorazioniEBeneficiAgo.UCBenefici)sender;
        //    if (this.domanda == null)
        //        this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

        //    this.areaInfoPratica = new AreaInfoPratica();
        //    List<AreaQuadri.Tab> elencoTab = new List<AreaQuadri.Tab>();
        //    elencoTab.Add(AreaQuadri.Tab.MaggiorazioniEBenefici);
        //    this.areaInfoPratica.ElencoTab = elencoTab.ToArray();

        //    CodeUtility.AggiornaSemafori(this, this, ucInfoLiquidazione);

        //    if (tabBenefici.HasError)
        //    {
        //        ucAvviso.Tipo = TipoAvviso.Warning;
        //        ucAvviso.Visible = true;
        //        ucAvviso.Messaggio = tabBenefici.ErrorMessage;
        //    }
        //    else
        //    {
        //        ucAvviso.Tipo = TipoAvviso.Ok;
        //        ucAvviso.Visible = true;
        //        ucAvviso.Messaggio = "Dati Benefici eliminati correttamente";
        //    }
        //}

        protected void event_ucShowAvvisoOneri(object sender, EventArgs e)
        {
            INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.Oneri.UCOneri tabOneri = sender as INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.Oneri.UCOneri;
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            this.areaInfoPratica = new AreaInfoPratica();
            List<AreaQuadri.Tab> elencoTab = new List<AreaQuadri.Tab>();
            elencoTab.Add(AreaQuadri.Tab.Oneri);
            elencoTab.Add(AreaQuadri.Tab.Redditi);
            this.areaInfoPratica.ElencoTab = elencoTab.ToArray();

            CodeUtility.AggiornaSemafori(this, this, ucInfoLiquidazione);

            if (tabOneri.HasError)
            {
                ucAvviso.Tipo = TipoAvviso.Warning;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = tabOneri.ErrorMessage;
            }
            else
            {
                ucAvviso.Tipo = TipoAvviso.Ok;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = "Dati Oneri salvati correttamente";
            }
        }

        protected void event_ucAnnullaOnere(object sender, EventArgs e)
        {
            ucAvviso.Messaggio = "";
            ucAvviso.Visible = false;
        }

        protected void event_ucSalvaOnere(object sender, EventArgs e)
        {
            if (ucAvviso.Visible == true)
                ucAvviso.Visible = false;
        }

        protected void event_ucAbilitaTastoSalva(object sender, EventArgs e)
        {
            if (btnSalva.Enabled == false)
                btnSalva.Enabled = true;
        }

        protected void event_ucDisabilitaTastoSalva(object sender, EventArgs e)
        {
            if (btnSalva.Enabled == true)
                btnSalva.Enabled = false;
        }

        protected void event_ucShowAvvisoEliminaOneri(object sender, EventArgs e)
        {
            INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.Oneri.UCOneri tabOneri = sender as INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.Oneri.UCOneri;
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            this.areaInfoPratica = new AreaInfoPratica();
            List<AreaQuadri.Tab> elencoTab = new List<AreaQuadri.Tab>();
            elencoTab.Add(AreaQuadri.Tab.Oneri);
            this.areaInfoPratica.ElencoTab = elencoTab.ToArray();

            CodeUtility.AggiornaSemafori(this, this, ucInfoLiquidazione);

            if (tabOneri.HasError)
            {
                ucAvviso.Tipo = TipoAvviso.Warning;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = tabOneri.ErrorMessage;
            }
            else
            {
                ucAvviso.Tipo = TipoAvviso.Ok;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = "Dati Oneri eliminati correttamente";
            }
        }



        protected void event_ucShowAvvisoPrepensionamento(object sender, EventArgs e)
        {
            INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.Oneri.UCPrepensionamento tabPrepensionamento = (INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.Oneri.UCPrepensionamento)sender;
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            this.areaInfoPratica = new AreaInfoPratica();
            List<AreaQuadri.Tab> elencoTab = new List<AreaQuadri.Tab>();
            elencoTab.Add(AreaQuadri.Tab.Oneri);
            this.areaInfoPratica.ElencoTab = elencoTab.ToArray();

            CodeUtility.AggiornaSemafori(this, this, ucInfoLiquidazione);

            if (tabPrepensionamento.HasError)
            {
                ucAvviso.Tipo = TipoAvviso.Warning;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = tabPrepensionamento.ErrorMessage;
            }
            else
            {
                ucAvviso.Tipo = TipoAvviso.Ok;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = "Dati Prepensionamento salvati correttamente";
            }
        }

        protected void event_ucShowAvvisoEliminaPrepensionamento(object sender, EventArgs e)
        {
            INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.Oneri.UCPrepensionamento tabPrepensionamento = (INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.Oneri.UCPrepensionamento)sender;
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            this.areaInfoPratica = new AreaInfoPratica();
            List<AreaQuadri.Tab> elencoTab = new List<AreaQuadri.Tab>();
            elencoTab.Add(AreaQuadri.Tab.Oneri);
            this.areaInfoPratica.ElencoTab = elencoTab.ToArray();

            CodeUtility.AggiornaSemafori(this, this, ucInfoLiquidazione);

            if (tabPrepensionamento.HasError)
            {
                ucAvviso.Tipo = TipoAvviso.Warning;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = tabPrepensionamento.ErrorMessage;
            }
            else
            {
                ucAvviso.Tipo = TipoAvviso.Ok;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = "Dati Prepensionamento eliminati correttamente";
            }
        }

        #endregion Events
    }
}