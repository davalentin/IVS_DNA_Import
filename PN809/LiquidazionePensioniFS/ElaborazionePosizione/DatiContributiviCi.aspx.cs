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
    public partial class DatiContributiviCi : CustomBasePage, IInfoLiquidazione, IQuadriSemafori, IDatiContributiviCi
    {
        #region IInfoLiquidazione
        public InfoLiquidazione InfoLiquidazione { get; set; }
        #endregion IInfoLiquidazione

        #region IViewUI
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        #endregion IViewUI

        #region IDatiContributiviCi
        public INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneCi.AreaDatiContributivi areaDatiContributiviCi { get; set; }
        public Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda { get; set; }
        #endregion IDatiContributiviCi

        #region IQuadriSemafori
        public AreaQuadri areaQuadri { get; set; }
        public AreaInfoPratica areaInfoPratica { get; set; }
        #endregion IQuadriSemafori

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                this.InfoLiquidazione = ValorizzaInfoLiquidazione(ucInfoLiquidazione);
                InitData(true);
                if (Session["Pratiche"] != null)
                {
                    btnTornaPosizioni.Visible = true;
                    btnTornaARicerca.Visible = false;
                }
            }
        }

        protected void event_ucInitializeData(object sender, EventArgs e)
        {
            InitData(false);
        }

        private void InitData(bool isPageFrom)
        {
            CaricaDatiContributivi();

            if (!HasError)
            {
                ucDatiCalcoloCi.areaDatiContributiviCi = this.areaDatiContributiviCi;
                if (isPageFrom)
                {
                    ucProrataCi.areaDatiContributiviCi = this.areaDatiContributiviCi;
                    ucImportiEsteriCi.areaDatiContributiviCi = this.areaDatiContributiviCi;
                    ucMaternitaAcnaCi.areaDatiContributiviCi = this.areaDatiContributiviCi;
                    ucDatiPostDecOriginariaCi.areaDatiContributiviCi = this.areaDatiContributiviCi;
                    ucIntegrazioneVirtuale.areaDatiContributiviCi = this.areaDatiContributiviCi;
                }
            }
        }

        private void CaricaDatiContributivi()
        {
            PresenterDatiContributiviCI presenterDatiContributiviCi = new PresenterDatiContributiviCI();
            this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
            presenterDatiContributiviCi.GetDatiContributivi(this);
            ViewState["DatiContributiviCi"] = this.areaDatiContributiviCi;
            if (HasError)
            {
                ucAvviso.Visible = true;
                ucAvviso.Tipo = TipoAvviso.Warning;
                ucAvviso.Messaggio = ErrorMessage;
                pnlTabProrata.Visible = false;
                pnlTabDatiCalcoloCI.Visible = false;
                pnlTabImportiEsteriCI.Visible = false;
                pnlTabMaternitaAcnaCI.Visible = false;
                pnlTabDatiPostDecOriginariaCI.Visible = false;
                pnlTabIntegrazioneVirtuale.Visible = false;
                ucProrataCi.Visible = false;
                ucDatiCalcoloCi.Visible = false;
                ucImportiEsteriCi.Visible = false;
                ucMaternitaAcnaCi.Visible = false;
                ucDatiPostDecOriginariaCi.Visible = false;
                ucIntegrazioneVirtuale.Visible = false;
                btnSalva.Enabled = btnPopUpPage.Enabled = false;
                return;
            }
            else
            {
                ucAvviso.Visible = false;
                ucAvviso.Messaggio = "";
            }
        }

        protected void SalvaDatiContributivi_Click(object sender, EventArgs e)
        {
            PresenterDatiContributiviCI presenterDatiContributiviCi = new PresenterDatiContributiviCI();
            this.areaDatiContributiviCi = (Presenter.SvrLiquidazioneCi.AreaDatiContributivi)ViewState["DatiContributiviCi"];

            this.areaDatiContributiviCi.ProRata = ucProrataCi.GetDatiProRata();

            if (ucProrataCi.HasError)
            {
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = ucProrataCi.ErrorMessage;
                ucAvviso.Tipo = TipoAvviso.Warning;
                return;
            }

            ucProrataCi.AggiornaAnniIntegrazioneVirtuale(this, null);

            this.areaDatiContributiviCi.DatiCalcolo = ucDatiCalcoloCi.GetDatiCalcolo();

            GestioneContribPensioniCiImportiValuta[] listImportiEsteri = ucImportiEsteriCi.GetDatiImportiEsteri();
            if (listImportiEsteri == null)
                this.areaDatiContributiviCi.LimportiEsteriValuta = null;
            else
                this.areaDatiContributiviCi.LimportiEsteriValuta = listImportiEsteri;

            GestioneContribMaternitaAcna[] listMaternitaAcna = ucMaternitaAcnaCi.GetDatiMaternitaAcna();
            if (listMaternitaAcna == null || listMaternitaAcna.Count() == 0)
                this.areaDatiContributiviCi.LMaternitaAcna = null;
            else
                this.areaDatiContributiviCi.LMaternitaAcna = ucMaternitaAcnaCi.GetDatiMaternitaAcna();

            GestioneContribRedditiPerIntegrazioneVirtuale[] listRedditiIV = ucIntegrazioneVirtuale.GetRedditiPerIntegrazioneVirtuale();
            if (listRedditiIV == null)
                this.areaDatiContributiviCi.LRedditiPerIntegrazioneVirtuale = null;
            else
                this.areaDatiContributiviCi.LRedditiPerIntegrazioneVirtuale = listRedditiIV;

            this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            presenterDatiContributiviCi.SalvaDatiContributiviCi(this);
            ucProrataCi.SetViewStateArea(this.areaDatiContributiviCi);

            if (HasError)
            {
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = ErrorMessage;
                ucAvviso.Tipo = TipoAvviso.Warning;

                ucDatiCalcoloCi.GestioneMaternitaAcna();
            }
            else
            {
                ucProrataCi.HideDatiStato();
                ucAvviso.Visible = true;
                ucAvviso.Tipo = TipoAvviso.Ok;
                ucAvviso.Messaggio = "Dati Contributivi salvati correttamente.";
            }

            this.areaInfoPratica = new AreaInfoPratica();
            List<AreaQuadri.Tab> elencoTab = new List<AreaQuadri.Tab>();
            elencoTab.Add(AreaQuadri.Tab.DatiCalcolo);
            this.areaInfoPratica.ElencoTab = elencoTab.ToArray();

            CodeUtility.AggiornaSemafori(this, this, ucInfoLiquidazione);
        }

        protected void event_ucShowAvvisoDatiCalcoloCi(object sender, EventArgs e)
        {
            INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DatiContributiviCi.UCDatiCalcoloCi1 tabCalcoloCi = (INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DatiContributiviCi.UCDatiCalcoloCi1)sender;
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            this.areaInfoPratica = new AreaInfoPratica();
            List<AreaQuadri.Tab> elencoTab = new List<AreaQuadri.Tab>();
            elencoTab.Add(AreaQuadri.Tab.DatiCalcolo);
            this.areaInfoPratica.ElencoTab = elencoTab.ToArray();

            CodeUtility.AggiornaSemafori(this, this, ucInfoLiquidazione);

            if ((tabCalcoloCi.HasError) || (!tabCalcoloCi.HasError && !string.IsNullOrEmpty(tabCalcoloCi.ErrorMessage)))
            {
                ucAvviso.Tipo = TipoAvviso.Warning;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = tabCalcoloCi.ErrorMessage;
            }
            else
            {
                ucAvviso.Tipo = TipoAvviso.Ok;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = "Dati Calcolo salvati correttamente";
            }
        }

        protected void event_ucShowErrorDatiCalcoloCi(object sender, EventArgs e)
        {
            INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DatiContributiviCi.UCDatiCalcoloCi1 tabCalcoloCi = (INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DatiContributiviCi.UCDatiCalcoloCi1)sender;
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            this.areaInfoPratica = new AreaInfoPratica();
            List<AreaQuadri.Tab> elencoTab = new List<AreaQuadri.Tab>();
            elencoTab.Add(AreaQuadri.Tab.DatiCalcolo);
            this.areaInfoPratica.ElencoTab = elencoTab.ToArray();

            CodeUtility.AggiornaSemafori(this, this, ucInfoLiquidazione);

            if (tabCalcoloCi.HasError)
            {
                ucAvviso.Tipo = TipoAvviso.Warning;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = tabCalcoloCi.ErrorMessage;
            }
        }

        protected void event_ucShowAvvisoEliminaDatiCalcoloCi(object sender, EventArgs e)
        {
            INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DatiContributiviCi.UCDatiCalcoloCi1 tabCalcoloCi = (INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DatiContributiviCi.UCDatiCalcoloCi1)sender;
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            this.areaInfoPratica = new AreaInfoPratica();
            List<AreaQuadri.Tab> elencoTab = new List<AreaQuadri.Tab>();
            elencoTab.Add(AreaQuadri.Tab.DatiCalcolo);
            this.areaInfoPratica.ElencoTab = elencoTab.ToArray();

            CodeUtility.AggiornaSemafori(this, this, ucInfoLiquidazione);

            if ((tabCalcoloCi.HasError) || (!tabCalcoloCi.HasError && !String.IsNullOrEmpty(tabCalcoloCi.ErrorMessage)))
            {
                ucAvviso.Tipo = TipoAvviso.Warning;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = tabCalcoloCi.ErrorMessage;
            }
            else
            {
                ucAvviso.Tipo = TipoAvviso.Ok;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = "Dati Calcolo eliminati correttamente";
            }
        }

        protected void event_ucShowAvvisoDatiProRata(object sender, EventArgs e)
        {
            INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DatiContributiviCi.UCProrataCi tabProrata = (INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DatiContributiviCi.UCProrataCi)sender;
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            this.areaInfoPratica = new AreaInfoPratica();
            List<AreaQuadri.Tab> elencoTab = new List<AreaQuadri.Tab>();
            elencoTab.Add(AreaQuadri.Tab.DatiCalcolo);
            this.areaInfoPratica.ElencoTab = elencoTab.ToArray();

            CodeUtility.AggiornaSemafori(this, this, ucInfoLiquidazione);

            if (tabProrata.HasError)
            {
                ucAvviso.Tipo = TipoAvviso.Warning;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = tabProrata.ErrorMessage;
            }
            else
            {
                ucAvviso.Tipo = TipoAvviso.Ok;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = "Dati Istituzione Estera salvati correttamente";
            }
        }

        protected void event_ucShowAvvisoEliminaDatiProRata(object sender, EventArgs e)
        {
            INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DatiContributiviCi.UCProrataCi tabDatiProrata = (INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DatiContributiviCi.UCProrataCi)sender;
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            this.areaInfoPratica = new AreaInfoPratica();
            List<AreaQuadri.Tab> elencoTab = new List<AreaQuadri.Tab>();
            elencoTab.Add(AreaQuadri.Tab.DatiCalcolo);
            this.areaInfoPratica.ElencoTab = elencoTab.ToArray();

            CodeUtility.AggiornaSemafori(this, this, ucInfoLiquidazione);

            if (tabDatiProrata.HasError)
            {
                ucAvviso.Tipo = TipoAvviso.Warning;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = tabDatiProrata.ErrorMessage;
            }
            else
            {
                ucAvviso.Tipo = TipoAvviso.Ok;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = "Dati Pro Rata eliminata correttamente";
            }
        }

        protected void event_ucAggiornaSemaforoTabIntegrazioneVirtuale(object sender, EventArgs e)
        {
            INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DatiContributiviCi.UCProrataCi tabDatiProrata = (INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DatiContributiviCi.UCProrataCi)sender;
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            PresenterDatiContributiviCI presenterDatiContributiviCi = new PresenterDatiContributiviCI();
            this.areaDatiContributiviCi = ucProrataCi.ConfermaModificheProrata();
            GestioneContribProRata datiProrata = ucProrataCi.GetViewStateProRata();
            List<GestioneContribRedditiPerIntegrazioneVirtuale> listRedditiIV = ucIntegrazioneVirtuale.GetListaAnni(datiProrata);

            if (listRedditiIV == null)
                this.areaDatiContributiviCi.LRedditiPerIntegrazioneVirtuale = null;
            else
                this.areaDatiContributiviCi.LRedditiPerIntegrazioneVirtuale = listRedditiIV.ToArray();

            ucProrataCi.SetViewStateArea(this.areaDatiContributiviCi);
        }

        protected void event_ucAggiornaAnniTabIntegrazioneVirtuale(object sender, EventArgs e)
        {
            INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DatiContributiviCi.UCProrataCi tabDatiProrata = (INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DatiContributiviCi.UCProrataCi)sender;
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            PresenterDatiContributiviCI presenterDatiContributiviCi = new PresenterDatiContributiviCI();
            GestioneContribProRata datiProrata = ucProrataCi.GetViewStateProRata();
            ucIntegrazioneVirtuale.GetDatiRedditiPerIntegrazioneVirtuale(datiProrata);
        }

        protected void event_ucShowAvvisoMaternitaAcnaCi(object sender, EventArgs e)
        {
            INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DatiContributiviCi.UCMaternitaAcnaCi tabMaternitaAcnaCi = (INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DatiContributiviCi.UCMaternitaAcnaCi)sender;
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            this.areaInfoPratica = new AreaInfoPratica();
            List<AreaQuadri.Tab> elencoTab = new List<AreaQuadri.Tab>();
            elencoTab.Add(AreaQuadri.Tab.DatiCalcolo);
            this.areaInfoPratica.ElencoTab = elencoTab.ToArray();

            CodeUtility.AggiornaSemafori(this, this, ucInfoLiquidazione);

            if (tabMaternitaAcnaCi.HasError)
            {
                ucAvviso.Tipo = TipoAvviso.Warning;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = tabMaternitaAcnaCi.ErrorMessage;
            }
            else
            {
                ucAvviso.Tipo = TipoAvviso.Ok;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = "Dati Maternità / Acna salvati correttamente";
            }
        }

        protected void event_ucShowAvvisoEliminaMaternitaAcnaCi(object sender, EventArgs e)
        {
            INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DatiContributiviCi.UCMaternitaAcnaCi tabMaternitaAcnaCi = (INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DatiContributiviCi.UCMaternitaAcnaCi)sender;
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            this.areaInfoPratica = new AreaInfoPratica();
            List<AreaQuadri.Tab> elencoTab = new List<AreaQuadri.Tab>();
            elencoTab.Add(AreaQuadri.Tab.DatiCalcolo);
            this.areaInfoPratica.ElencoTab = elencoTab.ToArray();

            CodeUtility.AggiornaSemafori(this, this, ucInfoLiquidazione);

            if (tabMaternitaAcnaCi.HasError)
            {
                ucAvviso.Tipo = TipoAvviso.Warning;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = tabMaternitaAcnaCi.ErrorMessage;
            }
            else
            {
                ucAvviso.Tipo = TipoAvviso.Ok;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = "Dati Maternità / Acna eliminati correttamente";
            }
        }

        protected void event_ucShowAvvisoImportiEsteriCi(object sender, EventArgs e)
        {
            INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DatiContributiviCi.UCImportiEsteriCi tabImportiEsteriCi = (INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DatiContributiviCi.UCImportiEsteriCi)sender;
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            this.areaInfoPratica = new AreaInfoPratica();
            List<AreaQuadri.Tab> elencoTab = new List<AreaQuadri.Tab>();
            elencoTab.Add(AreaQuadri.Tab.DatiCalcolo);
            this.areaInfoPratica.ElencoTab = elencoTab.ToArray();

            CodeUtility.AggiornaSemafori(this, this, ucInfoLiquidazione);

            if (tabImportiEsteriCi.HasError)
            {
                ucAvviso.Tipo = TipoAvviso.Warning;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = tabImportiEsteriCi.ErrorMessage;
            }
            else
            {
                ucAvviso.Tipo = TipoAvviso.Ok;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = "Dati Importi Esteri salvati correttamente";
            }
        }

        protected void event_ucShowAvvisoEliminaImportiEsteriCi(object sender, EventArgs e)
        {
            INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DatiContributiviCi.UCImportiEsteriCi tabImportiEsteriCi = (INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DatiContributiviCi.UCImportiEsteriCi)sender;
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            this.areaInfoPratica = new AreaInfoPratica();
            List<AreaQuadri.Tab> elencoTab = new List<AreaQuadri.Tab>();
            elencoTab.Add(AreaQuadri.Tab.DatiCalcolo);
            this.areaInfoPratica.ElencoTab = elencoTab.ToArray();

            CodeUtility.AggiornaSemafori(this, this, ucInfoLiquidazione);

            if (tabImportiEsteriCi.HasError)
            {
                ucAvviso.Tipo = TipoAvviso.Warning;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = tabImportiEsteriCi.ErrorMessage;
            }
            else
            {
                ucAvviso.Tipo = TipoAvviso.Ok;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = "Dati Importi Esteri eliminati correttamente";
            }
        }

        protected void event_ucShowAvvisoDatiPostDecOriginariaCi(object sender, EventArgs e)
        {
            INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DatiContributiviCi.UCDatiPostDecOriginariaCi tabDatiPostDecOriginariaCi = (INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DatiContributiviCi.UCDatiPostDecOriginariaCi)sender;
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            this.areaInfoPratica = new AreaInfoPratica();
            List<AreaQuadri.Tab> elencoTab = new List<AreaQuadri.Tab>();
            elencoTab.Add(AreaQuadri.Tab.DatiCalcolo);
            this.areaInfoPratica.ElencoTab = elencoTab.ToArray();

            CodeUtility.AggiornaSemafori(this, this, ucInfoLiquidazione);

            if (tabDatiPostDecOriginariaCi.HasError)
            {
                ucAvviso.Tipo = TipoAvviso.Warning;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = tabDatiPostDecOriginariaCi.ErrorMessage;
            }
            else
            {
                ucAvviso.Tipo = TipoAvviso.Ok;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = "Dati Post Decorrenza Originaria salvati correttamente";
            }
        }

        protected void event_ucShowAvvisoEliminaDatiPostDecOriginariaCi(object sender, EventArgs e)
        {
            INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DatiContributiviCi.UCDatiPostDecOriginariaCi tabDatiPostDecOriginariaCi = (INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DatiContributiviCi.UCDatiPostDecOriginariaCi)sender;
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            this.areaInfoPratica = new AreaInfoPratica();
            List<AreaQuadri.Tab> elencoTab = new List<AreaQuadri.Tab>();
            elencoTab.Add(AreaQuadri.Tab.DatiCalcolo);
            this.areaInfoPratica.ElencoTab = elencoTab.ToArray();

            CodeUtility.AggiornaSemafori(this, this, ucInfoLiquidazione);

            if (tabDatiPostDecOriginariaCi.HasError)
            {
                ucAvviso.Tipo = TipoAvviso.Warning;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = tabDatiPostDecOriginariaCi.ErrorMessage;
            }
            else
            {
                ucAvviso.Tipo = TipoAvviso.Ok;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = "Dati Dati Post Decorrenza Originaria eliminati correttamente";
            }
        }

        protected void event_ucShowAvvisoIntegrazioneVirtuale(object sender, EventArgs e)
        {
            INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DatiContributiviCi.UCIntegrazioneVirtuale tabIntegrazioneVirtuale = (INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DatiContributiviCi.UCIntegrazioneVirtuale)sender;
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            this.areaInfoPratica = new AreaInfoPratica();
            List<AreaQuadri.Tab> elencoTab = new List<AreaQuadri.Tab>();
            elencoTab.Add(AreaQuadri.Tab.DatiCalcolo);
            this.areaInfoPratica.ElencoTab = elencoTab.ToArray();

            CodeUtility.AggiornaSemafori(this, this, ucInfoLiquidazione);

            if (tabIntegrazioneVirtuale.HasError)
            {
                ucAvviso.Tipo = TipoAvviso.Warning;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = tabIntegrazioneVirtuale.ErrorMessage;
            }
            else
            {
                ucAvviso.Tipo = TipoAvviso.Ok;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = "Redditi per integrazione virtuale salvati correttamente";
            }
        }

        protected void event_ucShowAvvisoEliminaIntegrazioneVirtuale(object sender, EventArgs e)
        {
            INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DatiContributiviCi.UCIntegrazioneVirtuale tabIntegrazioneVirtuale = (INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DatiContributiviCi.UCIntegrazioneVirtuale)sender;
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            this.areaInfoPratica = new AreaInfoPratica();
            List<AreaQuadri.Tab> elencoTab = new List<AreaQuadri.Tab>();
            elencoTab.Add(AreaQuadri.Tab.DatiCalcolo);
            this.areaInfoPratica.ElencoTab = elencoTab.ToArray();

            CodeUtility.AggiornaSemafori(this, this, ucInfoLiquidazione);

            if (tabIntegrazioneVirtuale.HasError)
            {
                ucAvviso.Tipo = TipoAvviso.Warning;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = tabIntegrazioneVirtuale.ErrorMessage;
            }
            else
            {
                ucAvviso.Tipo = TipoAvviso.Ok;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = "Redditi per integrazione virtuale eliminati correttamente";
            }
        }

        protected void event_ucAbilitaPopUpDatiContributivi(object sender, EventArgs args)
        {
            btnPopUpPage.Style.Add("display", "inline-block");
            btnSalva.Style.Add("display", "none");
        }

        protected void event_ucDisabilitaPopUpDatiContributivi(object sender, EventArgs args)
        {
            btnSalva.Style.Add("display", "inline-block");
            btnPopUpPage.Style.Add("display", "none");
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            try
            {
                if ((AreaQuadri)Session["Semaforo"] != null)
                {
                    this.areaQuadri = (AreaQuadri)Session["Semaforo"];
                    ValorizzaSemaforiTab(imgProrataEstera, this.areaQuadri.QuadroDatiContributivi.TabProRata, pnlTabProrata);
                    ValorizzaSemaforiTab(imgDatiCalcoloCI, this.areaQuadri.QuadroDatiContributivi.TabDatiCalcolo, pnlTabDatiCalcoloCI);
                    ValorizzaSemaforiTab(imgImportiEsteriCI, this.areaQuadri.QuadroDatiContributivi.TabContrEsteri, pnlTabImportiEsteriCI);
                    ValorizzaSemaforiTab(imgDatiPostDecOriginariaCI, this.areaQuadri.QuadroDatiContributivi.TabDatiPostDecOriginaria, pnlTabDatiPostDecOriginariaCI);
                    ValorizzaSemaforiTab(imgMaternitaAcnaCI, this.areaQuadri.QuadroDatiContributivi.TabMaternAcna, pnlTabMaternitaAcnaCI);
                    ValorizzaSemaforiTab(imgTabIntegrazioneVirtuale, this.areaQuadri.QuadroDatiContributivi.TabIntegrazioneVirtuale, pnlTabIntegrazioneVirtuale);
                }
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("DatiContributiviCi, Errore nel metodo Page_PreRender " + ex);
            }
        }

        #region Show UCAvviso

        /* Funzioni utilizzate per l'abilitazione del pannello  ucavviso */


        protected void event_ucNascondiAvviso(object sender, EventArgs e)
        {
            ucAvviso.Messaggio = "";
            ucAvviso.Visible = false;
        }

        #endregion Show UCAvviso

        #region Gestione Tasto Salva

        protected void event_ucAbilitaTastoSalva(object sender, EventArgs e)
        {
            if (btnSalva.Enabled == false)
                btnSalva.Enabled = true;
            if (btnPopUpPage.Enabled == false)
                btnPopUpPage.Enabled = true;

        }

        protected void event_ucDisabilitaTastoSalva(object sender, EventArgs e)
        {
            if (btnSalva.Enabled == true)
                btnSalva.Enabled = false;
            if (btnPopUpPage.Enabled == true)
                btnPopUpPage.Enabled = false;
        }

        #endregion Gestione Tasto Salva
    }
}
