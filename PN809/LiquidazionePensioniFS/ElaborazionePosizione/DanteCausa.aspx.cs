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
    public partial class DanteCausa : CustomBasePage, IDanteCausa, IQuadriSemafori, ISedi
    {
        #region IDanteCausa
        public AreaDanteCausa areaDanteCausa { get; set; }
        public AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda { get; set; }
        public long numDomanda { get; set; }
        #endregion IDanteCausa

        #region IViewUI
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        #endregion IViewUI

        #region IQuadriSemafori
        public AreaQuadri areaQuadri { get; set; }
        public AreaInfoPratica areaInfoPratica { get; set; }
        #endregion IQuadriSemafori

        #region ISedi Members
        public string CommaSeparatedSedi { get; set; }
        public Dictionary<string, string> DictionaryOfficeList { get; set; }
        public string Sede { get; set; }
        public List<string> SediAbilitate { get; set; }
        public INPS.DNA.Office SelectedOffice { get; set; }
        #endregion ISedi Members

        #region IInfoLiquidazione
        public InfoLiquidazione InfoLiquidazione { get; set; }
        #endregion IInfoLiquidazione

        protected void Page_PreRender(object sender, EventArgs e)
        {
            if ((AreaQuadri)Session["Semaforo"] != null)
            {
                domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
                this.areaQuadri = (AreaQuadri)Session["Semaforo"];
                ValorizzaSemaforiTab(imgAnagrafica, this.areaQuadri.QuadroDanteCausa.TabAnagrafica, pnlTabAnagrafica);
                ValorizzaSemaforiTab(imgPensioneD, this.areaQuadri.QuadroDanteCausa.TabPensioneDiretta, pnlTabPensioneD);
                ValorizzaSemaforiTab(imgPensioneDC, this.areaQuadri.QuadroDanteCausa.TabAltraPensione, pnlTabAltraPensioneDC);
                ValorizzaSemaforiTab(imgPensioneCI, this.areaQuadri.QuadroDanteCausa.TabDatiPensioneCI, pnlTabPensioneCI);
                ValorizzaSemaforiTab(imgSentenza49593, this.areaQuadri.QuadroDanteCausa.TabSentenza49593, pnlTabSentenza49593);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
                this.InfoLiquidazione = ValorizzaInfoLiquidazione(ucInfoLiquidazione);
                CaricaDatiDanteCausa();
                if (Session["Pratiche"] != null)
                {
                    btnTornaPosizioni.Visible = true;
                    btnTornaARicerca.Visible = false;
                }
            }
        }

        private void CaricaDatiDanteCausa()
        {
            PresenterDanteCausa presenterDanteCausa = new PresenterDanteCausa();

            HiddenFieldSedi.Value = CodeUtility.LoadSedi();


            presenterDanteCausa.GetDatiDanteCausa(this);

            if (ViewState["TipoAppartenenzaDomanda"] == null)
                ViewState["TipoAppartenenzaDomanda"] = this.domanda.TipoAppartenenza;

            if ((AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp)ViewState["TipoAppartenenzaDomanda"] == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.AGO)
            {
                btnPopUpPage.Visible = true;
                btnSalva.Style.Add("display", "none");
            }

            //ENG - Ric Superstiti 024: in presenza del bypass NESSUN_DANTE_CAUSA allora il tab anagrafica obbligatorio e il tab diretta opzionale e tutti i campi sbloccati
            bool abilitaCampiTabDiretta = false;
            if ((AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp)ViewState["TipoAppartenenzaDomanda"] == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.FS)
            {
                if (this.areaDanteCausa.IsPresenteBypassNessunDanteCausa && CodeUtility.IsDomandaSuperstitiOrRicostituzione(this.domanda.Categoria)
                    && this.domanda.CodGruppo == "0031"
                    && (this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.FS || this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PT))
                {
                    abilitaCampiTabDiretta = true;
                }
            }

            ucanagrafica.ValorizzaControlliAnagraficaDC(this);

            if (this.areaDanteCausa.DatiPensioneDiretta != null || abilitaCampiTabDiretta)
                ucpensionediretta.ValorizzaControlliPensioneDiretta(this);

            ucaltrapensionedc.ValorizzaControlliAltraPensione(this);

            if ((((AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp)ViewState["TipoAppartenenzaDomanda"]) == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.AGO ||
                ((AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp)ViewState["TipoAppartenenzaDomanda"]) == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.CI))
            {
                ucsentenza49593.ValorizzaEtichetteDatiSentenza49593(this);
            }

            if (((AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp)ViewState["TipoAppartenenzaDomanda"]) == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.CI)
                ucpensioneci.ValorizzaControlliPensioneCI(this);
        }

        protected void event_ucShowAvvisoDanteAnagrafica(object sender, EventArgs e)
        {
            UserControls.DanteCausa.UCDanteAnagrafica tabAnagraficaDC = (UserControls.DanteCausa.UCDanteAnagrafica)sender;
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            this.areaInfoPratica = new AreaInfoPratica();
            List<AreaQuadri.Tab> elencoTab = new List<AreaQuadri.Tab>();
            elencoTab.Add(AreaQuadri.Tab.DanteCausa);
            elencoTab.Add(AreaQuadri.Tab.DatiCalcolo);
            this.areaInfoPratica.ElencoTab = elencoTab.ToArray();

            CodeUtility.AggiornaSemafori(this, this, ucInfoLiquidazione);

            if (tabAnagraficaDC.HasError)
            {
                ucAvviso.Tipo = TipoAvviso.Warning;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = tabAnagraficaDC.ErrorMessage;
            }
            else
            {
                ucAvviso.Tipo = TipoAvviso.Ok;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = "Dati Anagrafica Dante Causa salvati correttamente";
            }
        }

        protected void event_ucShowAvvisoDantePensioneDir(object sender, EventArgs e)
        {
            UserControls.DanteCausa.UCDantePensioneDiretta tabPensioneDiretta = (UserControls.DanteCausa.UCDantePensioneDiretta)sender;
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            this.areaInfoPratica = new AreaInfoPratica();
            List<AreaQuadri.Tab> elencoTab = new List<AreaQuadri.Tab>();
            elencoTab.Add(AreaQuadri.Tab.DanteCausa);
            this.areaInfoPratica.ElencoTab = elencoTab.ToArray();

            CodeUtility.AggiornaSemafori(this, this, ucInfoLiquidazione);

            if (tabPensioneDiretta.HasError)
            {
                ucAvviso.Tipo = TipoAvviso.Warning;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = tabPensioneDiretta.ErrorMessage;
            }
            else
            {
                ucAvviso.Tipo = TipoAvviso.Ok;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = "Dati Pensione Diretta salvati correttamente";
            }
        }

        protected void event_ucShowAvvisoDanteAltraPensione(object sender, EventArgs e)
        {
            UserControls.DanteCausa.UCDanteAltraPensioneDC tabAltraPensione = (UserControls.DanteCausa.UCDanteAltraPensioneDC)sender;
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            this.areaInfoPratica = new AreaInfoPratica();
            List<AreaQuadri.Tab> elencoTab = new List<AreaQuadri.Tab>();
            elencoTab.Add(AreaQuadri.Tab.DanteCausa);
            this.areaInfoPratica.ElencoTab = elencoTab.ToArray();

            CodeUtility.AggiornaSemafori(this, this, ucInfoLiquidazione);

            if (tabAltraPensione.HasError)
            {
                ucAvviso.Tipo = TipoAvviso.Warning;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = tabAltraPensione.ErrorMessage;
            }
            else
            {
                ucAvviso.Tipo = TipoAvviso.Ok;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = "Dati Altra Pensione salvati correttamente";
            }
        }

        protected void event_ucShowDanteDatiPensione(object sender, EventArgs e)
        {
            UserControls.DanteCausa.UCDanteDatiPensioneCI tabPensioneCIDC = (UserControls.DanteCausa.UCDanteDatiPensioneCI)sender;
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            this.areaInfoPratica = new AreaInfoPratica();
            List<AreaQuadri.Tab> elencoTab = new List<AreaQuadri.Tab>();
            elencoTab.Add(AreaQuadri.Tab.DanteCausa);
            this.areaInfoPratica.ElencoTab = elencoTab.ToArray();

            CodeUtility.AggiornaSemafori(this, this, ucInfoLiquidazione);

            if (tabPensioneCIDC.HasError)
            {
                ucAvviso.Tipo = TipoAvviso.Warning;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = tabPensioneCIDC.ErrorMessage;
            }
            else
            {
                ucAvviso.Tipo = TipoAvviso.Ok;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = "Dati 'Dati Pensione CI' Dante Causa salvati correttamente";
            }
        }

        protected void event_ucShowDanteSentenza49593(object sender, EventArgs e)
        {
            UserControls.DanteCausa.UCDanteSentenza49593 tabSentenza49593DC = (UserControls.DanteCausa.UCDanteSentenza49593)sender;
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            this.areaInfoPratica = new AreaInfoPratica();
            List<AreaQuadri.Tab> elencoTab = new List<AreaQuadri.Tab>();
            elencoTab.Add(AreaQuadri.Tab.DanteCausa);
            this.areaInfoPratica.ElencoTab = elencoTab.ToArray();

            CodeUtility.AggiornaSemafori(this, this, ucInfoLiquidazione);

            if (tabSentenza49593DC.HasError)
            {
                ucAvviso.Tipo = TipoAvviso.Warning;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = tabSentenza49593DC.ErrorMessage;
            }
            else
            {
                ucAvviso.Tipo = TipoAvviso.Ok;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = "Dati 'Sentenza 495/93' Dante Causa salvati correttamente";
            }
        }

        protected void event_ucShowAvvisoEliminaDanteSentenza49593(object sender, EventArgs e)
        {
            UserControls.DanteCausa.UCDanteSentenza49593 tabSentenza49593DC = (UserControls.DanteCausa.UCDanteSentenza49593)sender;
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            this.areaInfoPratica = new AreaInfoPratica();
            List<AreaQuadri.Tab> elencoTab = new List<AreaQuadri.Tab>();
            elencoTab.Add(AreaQuadri.Tab.DanteCausa);
            this.areaInfoPratica.ElencoTab = elencoTab.ToArray();

            CodeUtility.AggiornaSemafori(this, this, ucInfoLiquidazione);

            if (tabSentenza49593DC.HasError)
            {
                ucAvviso.Tipo = TipoAvviso.Warning;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = tabSentenza49593DC.ErrorMessage;
            }
            else
            {
                ucAvviso.Tipo = TipoAvviso.Ok;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = "Dati 'Sentenza 495/93' Dante Causa eliminati correttamente";
            }
        }

        protected void btnSalva_Click(object sender, EventArgs e)
        {
            this.domanda = new AreaRispostaRiepilogo.DatiRiepilogoDomanda();
            this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
            areaDanteCausa = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione.AreaDanteCausa();
            PresenterDanteCausa presenterDanteCausa = new PresenterDanteCausa();

            areaDanteCausa.AnagraficaDC = this.ucanagrafica.GetValoriAnagraficaDC();
            areaDanteCausa.DatiPensioneDiretta = this.ucpensionediretta.GetValoriPensioneDiretta();
            areaDanteCausa.AltraPensioneDC = this.ucaltrapensionedc.GetValoriAltraPensioneDC();
            areaDanteCausa.DatiPensioneCI = this.ucpensioneci.GetValoriPensioneCI();
            areaDanteCausa.DatiRedditiSentenza495_93 = this.ucsentenza49593.GetDatiQuadroSentenze495();
            areaDanteCausa.ImportoMensilePensioneEstera = this.ucsentenza49593.GetValorePensioneEstera();

            presenterDanteCausa.SalvaDanteCausaByDomanda(this);

            this.areaInfoPratica = new AreaInfoPratica();
            List<AreaQuadri.Tab> elencoTab = new List<AreaQuadri.Tab>();
            elencoTab.Add(AreaQuadri.Tab.DanteCausa);
            elencoTab.Add(AreaQuadri.Tab.DatiCalcolo);
            this.areaInfoPratica.ElencoTab = elencoTab.ToArray();

            CodeUtility.AggiornaSemafori(this, this, ucInfoLiquidazione);

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
                ucAvviso.Messaggio = "Dati Dante Causa salvati correttamente";
            }
        }
    }
}
