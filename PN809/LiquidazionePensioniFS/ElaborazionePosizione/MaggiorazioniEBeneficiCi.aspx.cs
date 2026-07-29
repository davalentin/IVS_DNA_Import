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
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneCi;
using INPS.DNA;

namespace INPS.Pensioni.LiquidazionePensione.View.Web
{
    [NavigationRules(AllowedReferrer = AllowedReferrer.Application, CheckSequenceOnPostBack = false)]
    public partial class MaggiorazioniEBeneficiCi : CustomBasePage, IInfoLiquidazione, IMaggiorazioneBeneficiCi, IQuadriSemafori
    {
        #region IInfoLiquidazione
        public InfoLiquidazione InfoLiquidazione { get; set; }
        #endregion IInfoLiquidazione

        #region IViewUI
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        #endregion IViewUI

        #region IMaggiorazioneBenefici
        public Presenter.SvrLiquidazioneCi.AreaMaggiorazioniBenefici areaMaggiorazioneBenefici { get; set; }
        public AreaRispostaRiepilogo.DatiRiepilogoDomanda areaRiepilogoDomanda { get; set; }
        #endregion IMaggiorazioneBenefici

        #region IQuadriSemafori
        public AreaQuadri areaQuadri { get; set; }
        public AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda { get; set; }
        public AreaInfoPratica areaInfoPratica { get; set; }
        #endregion IQuadriSemafori

        protected void Page_Load(object sender, EventArgs e)
        {
            imgExCombattente.ImageUrl = "~/App_Themes/" + Page.Theme + "/Images/rosso_tab.png";
            imgBenefici.ImageUrl = "~/App_Themes/" + Page.Theme + "/Images/rosso_tab.png";

            if (!Page.IsPostBack)
            {
                this.InfoLiquidazione = ValorizzaInfoLiquidazione(ucInfoLiquidazione);
                CaricaDatiMaggiorazioneBenefici();
                RenderSemafori();                
            }
            if (Session["Pratiche"] != null)
            {
                btnTornaPosizioni.Visible = true;
                btnTornaARicerca.Visible = false;
            }
        }      

        protected void event_ucShowAvvisoExCombattente(object sender, EventArgs e)
        {
            INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.MaggiorazioniEBeneficiCi.UCExCombattente tabExCombattente = (INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.MaggiorazioniEBeneficiCi.UCExCombattente)sender;
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            this.areaInfoPratica = new AreaInfoPratica();
            List<AreaQuadri.Tab> elencoTab = new List<AreaQuadri.Tab>();
            elencoTab.Add(AreaQuadri.Tab.MaggiorazioniEBenefici);
            this.areaInfoPratica.ElencoTab = elencoTab.ToArray();

            CodeUtility.AggiornaSemafori(this, this, ucInfoLiquidazione);

            if (tabExCombattente.HasError)
            {
                ucAvviso.Tipo = TipoAvviso.Warning;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = tabExCombattente.ErrorMessage;
            }
            else
            {
                ucAvviso.Tipo = TipoAvviso.Ok;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = "Dati Ex Combattente salvati correttamente";
            }
        }

        protected void event_ucShowAvvisoEliminaExCombattente(object sender, EventArgs e)
        {
            INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.MaggiorazioniEBeneficiCi.UCExCombattente tabExCombattente = (INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.MaggiorazioniEBeneficiCi.UCExCombattente)sender;
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            this.areaInfoPratica = new AreaInfoPratica();
            List<AreaQuadri.Tab> elencoTab = new List<AreaQuadri.Tab>();
            elencoTab.Add(AreaQuadri.Tab.MaggiorazioniEBenefici);
            this.areaInfoPratica.ElencoTab = elencoTab.ToArray();

            CodeUtility.AggiornaSemafori(this, this, ucInfoLiquidazione);

            if (tabExCombattente.HasError)
            {
                ucAvviso.Tipo = TipoAvviso.Warning;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = tabExCombattente.ErrorMessage;
            }
            else
            {
                ucAvviso.Tipo = TipoAvviso.Ok;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = "Dati Ex Combattente eliminati correttamente";
            }
        }

        protected void event_ucShowAvvisoBenefici(object sender, EventArgs e)
        {
            INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.MaggiorazioniEBeneficiCi.UCBenefici tabBenefici = (INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.MaggiorazioniEBeneficiCi.UCBenefici)sender;
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            this.areaInfoPratica = new AreaInfoPratica();
            List<AreaQuadri.Tab> elencoTab = new List<AreaQuadri.Tab>();
            elencoTab.Add(AreaQuadri.Tab.MaggiorazioniEBenefici);
            this.areaInfoPratica.ElencoTab = elencoTab.ToArray();

            CodeUtility.AggiornaSemafori(this, this, ucInfoLiquidazione);

            if (tabBenefici.HasError)
            {
                ucAvviso.Tipo = TipoAvviso.Warning;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = tabBenefici.ErrorMessage;
            }
            else
            {
                ucAvviso.Tipo = TipoAvviso.Ok;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = "Dati Benefici salvati correttamente";
            }
        }

        protected void event_ucShowAvvisoEliminaBenefici(object sender, EventArgs e)
        {
            INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.MaggiorazioniEBeneficiCi.UCBenefici tabBenefici = (INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.MaggiorazioniEBeneficiCi.UCBenefici)sender;
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            this.areaInfoPratica = new AreaInfoPratica();
            List<AreaQuadri.Tab> elencoTab = new List<AreaQuadri.Tab>();
            elencoTab.Add(AreaQuadri.Tab.MaggiorazioniEBenefici);
            this.areaInfoPratica.ElencoTab = elencoTab.ToArray();

            CodeUtility.AggiornaSemafori(this, this, ucInfoLiquidazione);

            if (tabBenefici.HasError)
            {
                ucAvviso.Tipo = TipoAvviso.Warning;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = tabBenefici.ErrorMessage;
            }
            else
            {
                ucAvviso.Tipo = TipoAvviso.Ok;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = "Dati Benefici eliminati correttamente";
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
            if (btnSalvaMaggiorazioniEBenefici.Enabled == false)
                btnSalvaMaggiorazioniEBenefici.Enabled = true;
        }

        protected void event_ucDisabilitaTastoSalva(object sender, EventArgs e)
        {
            if (btnSalvaMaggiorazioniEBenefici.Enabled == true)
                btnSalvaMaggiorazioniEBenefici.Enabled = false;
        }



        protected void event_ucShowAvvisoMaggiorazioni(object sender, EventArgs e)
        {
            INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.MaggiorazioniEBeneficiCi.UCMaggiorazioni tabMaggiorazioni = (INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.MaggiorazioniEBeneficiCi.UCMaggiorazioni)sender;
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            this.areaInfoPratica = new AreaInfoPratica();
            List<AreaQuadri.Tab> elencoTab = new List<AreaQuadri.Tab>();
            elencoTab.Add(AreaQuadri.Tab.MaggiorazioniEBenefici);
            this.areaInfoPratica.ElencoTab = elencoTab.ToArray();

            CodeUtility.AggiornaSemafori(this, this, ucInfoLiquidazione);

            if (tabMaggiorazioni.HasError)
            {
                ucAvviso.Tipo = TipoAvviso.Warning;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = tabMaggiorazioni.ErrorMessage;
            }
            else
            {
                ucAvviso.Tipo = TipoAvviso.Ok;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = "Dati Maggiorazioni salvati correttamente";
            }
        }

        protected void event_ucShowAvvisoEliminaMaggiorazioni(object sender, EventArgs e)
        {
            INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.MaggiorazioniEBeneficiCi.UCMaggiorazioni tabMaggiorazioni = (INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.MaggiorazioniEBeneficiCi.UCMaggiorazioni)sender;
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            this.areaInfoPratica = new AreaInfoPratica();
            List<AreaQuadri.Tab> elencoTab = new List<AreaQuadri.Tab>();
            elencoTab.Add(AreaQuadri.Tab.MaggiorazioniEBenefici);
            this.areaInfoPratica.ElencoTab = elencoTab.ToArray();

            CodeUtility.AggiornaSemafori(this, this, ucInfoLiquidazione);

            if (tabMaggiorazioni.HasError)
            {
                ucAvviso.Tipo = TipoAvviso.Warning;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = tabMaggiorazioni.ErrorMessage;
            }
            else
            {
                ucAvviso.Tipo = TipoAvviso.Ok;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = "Dati Maggiorazioni eliminati correttamente";
            }
        }

        protected void event_ucShowAvvisoVittime(object sender, EventArgs e)
        {
            INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.MaggiorazioniEBeneficiCi.UCVittimeCi tabVittime = (INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.MaggiorazioniEBeneficiCi.UCVittimeCi)sender;
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            this.areaInfoPratica = new AreaInfoPratica();
            List<AreaQuadri.Tab> elencoTab = new List<AreaQuadri.Tab>();
            elencoTab.Add(AreaQuadri.Tab.MaggiorazioniEBenefici);
            elencoTab.Add(AreaQuadri.Tab.DatiCalcolo);
            this.areaInfoPratica.ElencoTab = elencoTab.ToArray();

            CodeUtility.AggiornaSemafori(this, this, ucInfoLiquidazione);

            if (tabVittime.HasError)
            {
                ucAvviso.Tipo = TipoAvviso.Warning;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = tabVittime.ErrorMessage;
            }
            else
            {
                ucAvviso.Tipo = TipoAvviso.Ok;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = "Dati Vittime salvati correttamente";
            }
        }

        protected void event_ucShowAvvisoEliminaVittime(object sender, EventArgs e)
        {
            INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.MaggiorazioniEBeneficiCi.UCVittimeCi tabVittime = (INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.MaggiorazioniEBeneficiCi.UCVittimeCi)sender;
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            this.areaInfoPratica = new AreaInfoPratica();
            List<AreaQuadri.Tab> elencoTab = new List<AreaQuadri.Tab>();
            elencoTab.Add(AreaQuadri.Tab.MaggiorazioniEBenefici);
            elencoTab.Add(AreaQuadri.Tab.DatiCalcolo);
            this.areaInfoPratica.ElencoTab = elencoTab.ToArray();

            CodeUtility.AggiornaSemafori(this, this, ucInfoLiquidazione);

            if (tabVittime.HasError)
            {
                ucAvviso.Tipo = TipoAvviso.Warning;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = tabVittime.ErrorMessage;
            }
            else
            {
                ucAvviso.Tipo = TipoAvviso.Ok;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = "Dati Vittime eliminati correttamente";
            }
        }

        private void CaricaDatiMaggiorazioneBenefici()
        {
            AreaTitolare.DatiPensione datiPensione = (AreaTitolare.DatiPensione)Session["DatiPensione"];
            this.areaRiepilogoDomanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
            PresenterMaggiorazioneBenefici presenterMaggiorazioneBenefici = new PresenterMaggiorazioneBenefici();
            presenterMaggiorazioneBenefici.GetMaggiorazioneBeneficiCi(this);
            if (HasError)
            {
                ucAvviso.Visible = true;
                ucAvviso.Tipo = TipoAvviso.Ko;
                ucAvviso.Messaggio = ErrorMessage;
                pnlMaggiorazioniEBeneficiCi.Enabled = false;
                return;
            }

            ucExCombattente.ValorizzaEtichetteExCombattente(this);
            ucBenefici.ValorizzaEtichetteBenefici(this);
            ucMaggiorazioni.ValorizzaEtichetteMaggiorazioni(this);
            ucVittimeCi.ValorizzaEtichette(this);
        }

        private void RenderSemafori()
        {
            if ((AreaQuadri)Session["Semaforo"] != null)
            {
                domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
                this.areaQuadri = (AreaQuadri)Session["Semaforo"];

                if (this.areaQuadri.QuadroMaggiorazioniBenefici.TabExCombattente != AreaQuadri.Semaforo.Rosso_NonAbilitato)
                    hdnSelected.Value = "#exCombattente";
                else if (this.areaQuadri.QuadroMaggiorazioniBenefici.TabBenefici != AreaQuadri.Semaforo.Rosso_NonAbilitato)
                    hdnSelected.Value = "#benefici";
                else if (this.areaQuadri.QuadroMaggiorazioniBenefici.TabMaggiorazioni != AreaQuadri.Semaforo.Rosso_NonAbilitato)
                    hdnSelected.Value = "#maggiorazioni";
                else if (this.areaQuadri.QuadroMaggiorazioniBenefici.TabBeneficioVittimeTerrorismo != AreaQuadri.Semaforo.Rosso_NonAbilitato)
                    hdnSelected.Value = "#vittime";
            }
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            AreaTitolare.DatiPensione datiPensione = (AreaTitolare.DatiPensione)Session["DatiPensione"];

            if ((AreaQuadri)Session["Semaforo"] != null)
            {
                domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
                this.areaQuadri = (AreaQuadri)Session["Semaforo"];
                ValorizzaSemaforiTab(imgExCombattente, this.areaQuadri.QuadroMaggiorazioniBenefici.TabExCombattente, pnlTabExCombattente);
                ValorizzaSemaforiTab(imgBenefici, this.areaQuadri.QuadroMaggiorazioniBenefici.TabBenefici, pnlTabBenefici);
                ValorizzaSemaforiTab(imgMaggiorazioni, this.areaQuadri.QuadroMaggiorazioniBenefici.TabMaggiorazioni, pnlTabMaggiorazioni);
                ValorizzaSemaforiTab(imgVittime, this.areaQuadri.QuadroMaggiorazioniBenefici.TabBeneficioVittimeTerrorismo, pnlTabVittime);
            }

            if (Utility.IsRicEsenzioneFiscaleVittimeDelDovere(datiPensione) &&
                        (this.areaQuadri.QuadroMaggiorazioniBenefici.TabBeneficioVittimeTerrorismo != AreaQuadri.Semaforo.Rosso_NonAbilitato &&
                         this.areaQuadri.QuadroMaggiorazioniBenefici.TabExCombattente == AreaQuadri.Semaforo.Rosso_NonAbilitato &&
                         this.areaQuadri.QuadroMaggiorazioniBenefici.TabBenefici == AreaQuadri.Semaforo.Rosso_NonAbilitato &&
                         this.areaQuadri.QuadroMaggiorazioniBenefici.TabDL407 == AreaQuadri.Semaforo.Rosso_NonAbilitato &&
                         this.areaQuadri.QuadroMaggiorazioniBenefici.TabPrivilegiate == AreaQuadri.Semaforo.Rosso_NonAbilitato &&
                         this.areaQuadri.QuadroMaggiorazioniBenefici.TabArticolo2 == AreaQuadri.Semaforo.Rosso_NonAbilitato))
                btnSalvaMaggiorazioniEBenefici.Enabled = false;
        }

        protected void SalvaMaggiorazioniEBenefici_Click(object sender, EventArgs e)
        {
            areaRiepilogoDomanda = new AreaRispostaRiepilogo.DatiRiepilogoDomanda();
            areaRiepilogoDomanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
            this.domanda = areaRiepilogoDomanda;
            areaMaggiorazioneBenefici = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneCi.AreaMaggiorazioniBenefici();
            PresenterMaggiorazioneBenefici presenterMaggiorazioneBenefici = new PresenterMaggiorazioneBenefici();

            areaMaggiorazioneBenefici.DatiExCombattente = ucExCombattente.GetValoriExCombattente();
            areaMaggiorazioneBenefici.DatiBenefici = ucBenefici.GetValoriBenefici();

            areaMaggiorazioneBenefici.DatiMaggiorazioni = ucMaggiorazioni.GetValoriMaggiorazioni();

            presenterMaggiorazioneBenefici.SalvaMaggiorazioniBeneficiCi(this);

            ucExCombattente.ValorizzaEtichetteExCombattente(this);

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
                ucAvviso.Messaggio = "Dati Maggiorazioni e benefici salvati correttamente";
            }

            this.areaInfoPratica = new AreaInfoPratica();
            List<AreaQuadri.Tab> elencoTab = new List<AreaQuadri.Tab>();
            elencoTab.Add(AreaQuadri.Tab.MaggiorazioniEBenefici);
            this.areaInfoPratica.ElencoTab = elencoTab.ToArray();

            CodeUtility.AggiornaSemafori(this, this, ucInfoLiquidazione);
        }
    }
}
