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
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs;
using INPS.DNA.Presenter.Interface;

namespace INPS.Pensioni.LiquidazionePensione.View.Web
{
    [NavigationRules(AllowedReferrer = AllowedReferrer.Application, CheckSequenceOnPostBack = false)]
    public partial class LiquidazionePensione : CustomBasePage, IInfoLiquidazione, ILiquidazionePensione, IQuadriSemafori, ITitolarePensione, IRecordFondo
    {
        #region ILiquidazionePensione
        public INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs.AreaLiquidazionePensione areaLiquidazionePensioneFS { get; set; }
        public AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda { get; set; }
        #endregion ILiquidazionePensione

        #region IQuadriSemafori
        public AreaQuadri areaQuadri { get; set; }
        public AreaInfoPratica areaInfoPratica { get; set; }
        #endregion IQuadriSemafori

        #region ITitolarePensione
        public AreaTitolare TitolarePensione { get; set; }

        #endregion ITitolarePensione

        #region IInfoLiquidazione
        public InfoLiquidazione InfoLiquidazione { get; set; }
        #endregion IInfoLiquidazione

        #region IViewUI
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        #endregion IViewUI

        #region IRecordFondo
        public RecordFondo[] areaArrayRecordFondo { get; set; }
        #endregion IRecordFondo

        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.domanda == null)
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            if (!IsPostBack)
            {
                this.InfoLiquidazione = ValorizzaInfoLiquidazione(ucInfoLiquidazione);
                SwitchUserControls();
                CaricaDatiLiquidazione();
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
                ValorizzaSemaforiTab(imgDatiGenerici, this.areaQuadri.QuadroLiquidazionePensione.TabDatiGenerici, pnlTabDatiGenerici);
                ValorizzaSemaforiTab(imgDatiAssicurativi, this.areaQuadri.QuadroLiquidazionePensione.TabDatiAssicurativi, pnlTabDatiAssicurativi);
                ValorizzaSemaforiTab(imgIstruttoria, this.areaQuadri.QuadroLiquidazionePensione.TabIstruttoria, pnlTabIstruttoria);
                ValorizzaSemaforiTab(imgOpzione, this.areaQuadri.QuadroLiquidazionePensione.TabOpzione, pnlTabOpzione);
                ValorizzaSemaforiTab(imgPrecedentePensione, this.areaQuadri.QuadroLiquidazionePensione.TabPrecedentePensione, pnlTabPrecedentePensione);
                ValorizzaSemaforiTab(imgBititolaritaINAIL, this.areaQuadri.QuadroLiquidazionePensione.TabInail, pnlTabBititolaritaINAIL);
                ValorizzaSemaforiTab(imgLegge460, this.areaQuadri.QuadroLiquidazionePensione.TabDatiLegge460, pnlTabLegge460);
                ValorizzaSemaforiTab(imgStorico, this.areaQuadri.QuadroLiquidazionePensione.TabStorico, pnlTabStorico);
            }
        }

        protected void SalvaLiquidazionePensione_Click(object sender, EventArgs e)
        {
            areaLiquidazionePensioneFS = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs.AreaLiquidazionePensione();
            Presenter.PresenterLiquidazionePensione presenterLiquidazione = new PresenterLiquidazionePensione();
            List<RecordFondo> listaRecordFondo = null;
            if (this.domanda.IsDomandaINPDAP)
            {
                areaLiquidazionePensioneFS.DatiGenericiINPDAP = ucDatiGenericiINPDAP.GetDatiGenerici();
                areaLiquidazionePensioneFS.DatiAssicurativiINPDAP = ucDatiAssicurativiINPDAP.GetDatiAssicurativi();
                areaLiquidazionePensioneFS.ListaRipartizioneINPDAP = ucDatiAssicurativiINPDAP.GetRipartizioniINPDAP();
            }
            else
            {
                areaLiquidazionePensioneFS.DatiGenerici = GetDatiGenerici(this.domanda.Tipofondo);
                areaLiquidazionePensioneFS.DatiAssicurativi = GetDatiAssicurativi(this.domanda.Tipofondo, out listaRecordFondo);
            }
            if (!(bool)ViewState["IsDomandaConNuovaGestioneDatiFondoFSPT"] && listaRecordFondo != null)
                areaLiquidazionePensioneFS.ListaRecordFondo = listaRecordFondo.ToArray();
            areaLiquidazionePensioneFS.DatiPrecedentePensione = ucPrecedentePensione.GetPrecedentePensione();
            areaLiquidazionePensioneFS.DatiBititolaritaInail = ucBititolaritaInail.GetValoriBititolaritaInail();
            areaLiquidazionePensioneFS.DatiLegge460 = ucLegge460.GetDatiLegge460();

            if (ViewState["DatiStoricoGP"] != null)
                areaLiquidazionePensioneFS.DatiLiquidazionePensioneStorico = (DatiLiquidazionePensioneStorico)ViewState["DatiStoricoGP"];

            presenterLiquidazione.SalvaLiquidazionePensione(this);
            if (this.HasError)
            {
                ucAvviso.Tipo = TipoAvviso.Warning;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = this.ErrorMessage;

                SwitchGestioneAOI(this.domanda.Tipofondo);
            }
            else
            {
                if (areaLiquidazionePensioneFS.DatiGenerici != null &&
                (!areaLiquidazionePensioneFS.DatiGenerici.TrasformazioneAOI.HasValue || !areaLiquidazionePensioneFS.DatiGenerici.TrasformazioneAOI.Value))
                {
                    areaLiquidazionePensioneFS.DatiPrecedentePensione = new DatiPrecedentePensione();
                    ucPrecedentePensione.ResettaEtichettePrecedentePensione();
                }

                SwitchClearBonusSection(this.domanda.Tipofondo);
                UpdateCodiceComunicazione3(this.domanda.Tipofondo);
                ucAvviso.Tipo = TipoAvviso.Ok;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = "Dati Liquidazione Pensione salvati correttamente";

            }

            AreaTitolare.DatiPensione datiPensione = new AreaTitolare.DatiPensione();
            datiPensione = GetDatiPensione(this);

            this.areaInfoPratica = new AreaInfoPratica();
            List<AreaQuadri.Tab> elencoTab = new List<AreaQuadri.Tab>();
            elencoTab.Add(AreaQuadri.Tab.LiquidazionePensione);
            if (CodeUtility.IsRicostituzioneContributiva(datiPensione) || CodeUtility.IsRicostituzioneSupplemento(datiPensione) ||
                this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.FS || this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PT ||
                this.domanda.IsDomandaINPDAP)
                elencoTab.Add(AreaQuadri.Tab.Supplementi);
            elencoTab.Add(AreaQuadri.Tab.MaggiorazioniEBenefici);
            elencoTab.Add(AreaQuadri.Tab.DatiCalcolo);
            elencoTab.Add(AreaQuadri.Tab.Detrazioni);
            elencoTab.Add(AreaQuadri.Tab.Redditi);
            elencoTab.Add(AreaQuadri.Tab.DatiNoCalcolo);
            elencoTab.Add(AreaQuadri.Tab.RichiestaBonus);
            this.areaInfoPratica.ElencoTab = elencoTab.ToArray();

            CodeUtility.AggiornaSemafori(this, this, ucInfoLiquidazione);
        }


        private void UpdateCodiceComunicazione3(AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo? tipoFondo)
        {
            CodeUtility areaDecodifica = new CodeUtility();
            Presenter.SvrLiquidazione.AreaDecodifica datiDecodifica = areaDecodifica.GetValuesDecodifica();

            if (tipoFondo.HasValue)
            {
                switch (tipoFondo.Value)
                {
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.EL:
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.TT:
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.ET:
                        ucDatiGenericiEL_TT_ET.SetDdlCodiceComunicazione3(datiDecodifica, this);
                        break;
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.VL:
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.FS:
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PT:
                        UCDatiGenericiVL_FS_PT.SetDdlCodiceComunicazione3(datiDecodifica, this);
                        break;
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PI:
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.GAS:
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.CL:
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PL:
                        ucDatiGenericiPI_GAS_CL.SetDdlCodiceComunicazione3(datiDecodifica, this);
                        break;
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.DZ:
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.ES:
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PM:
                        ucDatiGenericiDZ_ES_PM.SetDdlCodiceComunicazione3(datiDecodifica, this);
                        break;
                }
            }
        }

        #region Private Methods

        private void SwitchUserControls()
        {
            if (this.domanda.IsDomandaINPDAP)
            {
                ucDatiGenericiINPDAP.Visible = true;
                ucDatiAssicurativiINPDAP.Visible = true;
            }
            else
            {
                switch (this.domanda.Tipofondo)
                {
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.EL:
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.TT:
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.ET:
                        ucDatiGenericiEL_TT_ET.Visible = true;
                        ucDatiAssicurativiEL_TT_ET.Visible = true;
                        ucStorico.Visible = true;
                        break;
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.VL:
                        UCDatiGenericiVL_FS_PT.Visible = true;
                        ucDatiAssicurativiVL_FS_PT.Visible = true;
                        ucStorico.Visible = true;
                        break;
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.FS:
                        UCDatiGenericiVL_FS_PT.Visible = true;
                        ucDatiAssicurativiVL_FS_PT.Visible = true;
                        break;
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PT:
                        UCDatiGenericiVL_FS_PT.Visible = true;
                        ucDatiAssicurativiVL_FS_PT.Visible = true;
                        ucLegge460.Visible = true;
                        break;
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PI:
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PL:
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.GAS:
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.CL:
                        ucDatiGenericiPI_GAS_CL.Visible = true;
                        ucDatiAssicurativiPI_GAS_CL.Visible = true;
                        break;
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.DZ:
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.ES:
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PM:
                        ucDatiGenericiDZ_ES_PM.Visible = true;
                        ucDatiAssicurativiDZ_ES_PM.Visible = true;
                        break;
                    default:
                        ucDatiGenericiEL_TT_ET.Visible = false;
                        ucDatiAssicurativiEL_TT_ET.Visible = false;
                        UCDatiGenericiVL_FS_PT.Visible = false;
                        ucDatiAssicurativiVL_FS_PT.Visible = false;
                        ucDatiGenericiPI_GAS_CL.Visible = false;
                        ucDatiAssicurativiPI_GAS_CL.Visible = false;
                        ucDatiGenericiDZ_ES_PM.Visible = false;
                        ucDatiAssicurativiDZ_ES_PM.Visible = false;
                        ucLegge460.Visible = false;
                        break;
                }
            }
        }

        private void SwitchValorizzaEtichette(AreaTitolare.DatiPensione datiPensione, bool IsDomandaSperDonna, bool isDomandaInabilitaAmianto)
        {
            if (this.domanda.IsDomandaINPDAP)
            {
                ucDatiGenericiINPDAP.ValorizzaEtichetteDatiGenerici(this, datiPensione, IsDomandaSperDonna);
                ucDatiAssicurativiINPDAP.ValorizzaEtichetteDatiAssicurativi(this, datiPensione, IsDomandaSperDonna, isDomandaInabilitaAmianto);
            }
            else
            {
                switch (this.domanda.Tipofondo)
                {
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.EL:
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.TT:
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.ET:
                        ucDatiGenericiEL_TT_ET.ValorizzaEtichetteDatiGenerici(this, datiPensione, IsDomandaSperDonna);
                        ucDatiAssicurativiEL_TT_ET.ValorizzaEtichetteDatiAssicurativi(this, datiPensione, IsDomandaSperDonna, isDomandaInabilitaAmianto);
                        ucStorico.ValorizzaEtichette(this);
                        break;

                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.VL:
                        UCDatiGenericiVL_FS_PT.ValorizzaEtichetteDatiGenerici(this, datiPensione, IsDomandaSperDonna);
                        ucDatiAssicurativiVL_FS_PT.ValorizzaEtichetteDatiAssicurativi(this, datiPensione, IsDomandaSperDonna, isDomandaInabilitaAmianto);
                        ucStorico.ValorizzaEtichette(this);
                        break;
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.FS:
                        UCDatiGenericiVL_FS_PT.ValorizzaEtichetteDatiGenerici(this, datiPensione, IsDomandaSperDonna);
                        ucDatiAssicurativiVL_FS_PT.ValorizzaEtichetteDatiAssicurativi(this, datiPensione, IsDomandaSperDonna, isDomandaInabilitaAmianto);
                        break;
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PT:
                        UCDatiGenericiVL_FS_PT.ValorizzaEtichetteDatiGenerici(this, datiPensione, IsDomandaSperDonna);
                        ucDatiAssicurativiVL_FS_PT.ValorizzaEtichetteDatiAssicurativi(this, datiPensione, IsDomandaSperDonna, isDomandaInabilitaAmianto);
                        ucLegge460.ValorizzaEtichetteDatiLegge460(this);
                        break;
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PI:
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.GAS:
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.CL:
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PL:
                        ucDatiGenericiPI_GAS_CL.ValorizzaEtichetteDatiGenerici(this, datiPensione, IsDomandaSperDonna);
                        ucDatiAssicurativiPI_GAS_CL.ValorizzaEtichetteDatiAssicurativi(this, datiPensione, IsDomandaSperDonna, isDomandaInabilitaAmianto);
                        break;
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.DZ:
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.ES:
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PM:
                        ucDatiGenericiDZ_ES_PM.ValorizzaEtichetteDatiGenerici(this, datiPensione, IsDomandaSperDonna);
                        ucDatiAssicurativiDZ_ES_PM.ValorizzaEtichetteDatiAssicurativi(this, datiPensione, IsDomandaSperDonna, isDomandaInabilitaAmianto);
                        break;
                }
            }
        }

        private void CaricaDatiLiquidazione()
        {
            AreaTitolare.DatiPensione datiPensione = new AreaTitolare.DatiPensione();
            datiPensione = GetDatiPensione(this);
            bool IsDomandaSperDonna = CodeUtility.IsDomandaSperimentaleDonna(datiPensione);

            PresenterLiquidazionePensione presenterLiquidazione = new PresenterLiquidazionePensione();
            presenterLiquidazione.GetLiquidazionePensione(this);
            if (HasError)
            {
                ucAvviso.Visible = true;
                ucAvviso.Tipo = TipoAvviso.Ko;
                ucAvviso.Messaggio = ErrorMessage;
                pnlLiquidazionePensione.Enabled = false;
                return;
            }
            ViewState["IsDomandaConNuovaGestioneDatiFondoFSPT"] = this.areaLiquidazionePensioneFS.IsDomandaConNuovaGestioneDatiFondoFSPT.GetValueOrDefault();
            ViewState["DatiStoricoGP"] = (this.areaLiquidazionePensioneFS != null && this.areaLiquidazionePensioneFS.DatiLiquidazionePensioneStorico != null) ? (DatiLiquidazionePensioneStorico)this.areaLiquidazionePensioneFS.DatiLiquidazionePensioneStorico : null;
            bool IsPensioneTipoContributivo = (this.areaLiquidazionePensioneFS.IsPensioneTipoContributivo.GetValueOrDefault() || datiPensione.IsDomandaVecchiaiaAOICalcoloContributivo); //ENG - MEMO 166/2023
            hdnIsContributiva.Value = IsPensioneTipoContributivo.ToString();
            if (datiPensione.DataPerfezionamentoRequisiti.HasValue)
            {
                var trimestre = (datiPensione.DataPerfezionamentoRequisiti.Value.Month - 1) / 3 + 1;
                var anno = datiPensione.DataPerfezionamentoRequisiti.Value.Year;
                hdnRequisitiAnte247Trimestre.Value = trimestre.ToString();
                hdnRequisitiAnte247Anno.Value = anno.ToString();
            }
            SwitchValorizzaEtichette(datiPensione, IsDomandaSperDonna, datiPensione.IsDomandaInabilitaAmiantoOrRicostituzione);

            ucPrecedentePensione.ValorizzaEtichettePrecedentePensione(this, datiPensione);
            ucBititolaritaInail.ValorizzaEtichetteBititolaritaINAIL(this);

            if (this.areaLiquidazionePensioneFS.DatiPrecedentePensione != null && Utility.IsDomandaRipristino(datiPensione))
                SetHiddenPrecedentePensioneValue(this.domanda.Tipofondo);
        }

        private void SetHiddenPrecedentePensioneValue(AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo? tipoFondo)
        {
            if (tipoFondo.HasValue)
            {
                switch (tipoFondo.Value)
                {
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.EL:
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.TT:
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.ET:
                        ucDatiGenericiEL_TT_ET.SetHiddenPrecedentePensioneValue("true");
                        break;
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.VL:
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.FS:
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PT:
                        UCDatiGenericiVL_FS_PT.SetHiddenPrecedentePensioneValue("true");
                        break;
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PI:
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.GAS:
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.CL:
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PL:
                        ucDatiGenericiPI_GAS_CL.SetHiddenPrecedentePensioneValue("true");
                        break;
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.DZ:
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.ES:
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PM:
                        ucDatiGenericiDZ_ES_PM.SetHiddenPrecedentePensioneValue("true");
                        break;
                }
            }
        }

        private DatiAssicurativi GetDatiAssicurativi(AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo? tipoFondo, out List<RecordFondo> listaRecordFondo)
        {
            DatiAssicurativi datiAssicurativi = new DatiAssicurativi();
            listaRecordFondo = new List<RecordFondo>();
            if (tipoFondo.HasValue)
            {
                switch (tipoFondo.Value)
                {
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.EL:
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.TT:
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.ET:
                        datiAssicurativi = ucDatiAssicurativiEL_TT_ET.GetDatiAssicurativi(tipoFondo.Value, out listaRecordFondo);
                        break;
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.VL:
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.FS:
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PT:
                        datiAssicurativi = ucDatiAssicurativiVL_FS_PT.GetDatiAssicurativi(tipoFondo.Value, out listaRecordFondo);
                        break;
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PI:
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.GAS:
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.CL:
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PL:
                        datiAssicurativi = ucDatiAssicurativiPI_GAS_CL.GetDatiAssicurativi(tipoFondo.Value, out listaRecordFondo);
                        break;
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.DZ:
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.ES:
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PM:
                        datiAssicurativi = ucDatiAssicurativiDZ_ES_PM.GetDatiAssicurativi(tipoFondo.Value, out listaRecordFondo);
                        break;
                }
            }
            return datiAssicurativi;
        }

        private DatiGenerici GetDatiGenerici(AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo? tipoFondo)
        {
            DatiGenerici datiGenerici = new DatiGenerici();
            if (tipoFondo.HasValue)
            {
                switch (tipoFondo.Value)
                {
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.EL:
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.TT:
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.ET:
                        datiGenerici = ucDatiGenericiEL_TT_ET.GetDatiGenerici(tipoFondo.Value);
                        break;
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.VL:
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.FS:
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PT:
                        datiGenerici = UCDatiGenericiVL_FS_PT.GetDatiGenerici(tipoFondo.Value);
                        break;
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PI:
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.GAS:
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.CL:
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PL:
                        datiGenerici = ucDatiGenericiPI_GAS_CL.GetDatiGenerici(tipoFondo.Value);
                        break;
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.DZ:
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.ES:
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PM:
                        datiGenerici = ucDatiGenericiDZ_ES_PM.GetDatiGenerici(tipoFondo.Value);
                        break;
                }
            }
            return datiGenerici;
        }

        private void SwitchGestioneAOI(AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo? tipoFondo)
        {
            if (tipoFondo.HasValue)
            {
                switch (tipoFondo.Value)
                {
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.EL:
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.TT:
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.ET:
                        this.ucDatiGenericiEL_TT_ET.GestioneAOI();
                        break;
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.VL:
                        this.UCDatiGenericiVL_FS_PT.GestioneAOI();
                        break;
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.GAS:
                        this.ucDatiGenericiPI_GAS_CL.GestioneAOI();
                        break;
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.DZ:
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.ES:
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PM:
                        this.ucDatiGenericiDZ_ES_PM.GestioneAOI();
                        break;
                }
            }
            else
            {
                if (this.domanda.IsDomandaINPDAP)
                {
                    this.ucDatiGenericiINPDAP.GestioneAOI();
                }
            }
        }

        private void SwitchClearBonusSection(AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo? tipoFondo)
        {
            if (tipoFondo.HasValue)
            {
                switch (tipoFondo.Value)
                {
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.EL:
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.TT:
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.ET:
                        this.ucDatiGenericiEL_TT_ET.ClearBonusSection();
                        break;
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.VL:
                        this.UCDatiGenericiVL_FS_PT.ClearBonusSection();
                        break;
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PI:
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.GAS:
                        this.ucDatiGenericiPI_GAS_CL.ClearBonusSection();
                        break;
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.DZ:
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.ES:
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PM:
                        // Non è presente il pannello Bonus
                        break;
                }
            }
        }

        #endregion Private Methods

        #region Events

        private void GetDataEventsTabAssicurativi(object sender, Utility.CustomEventArgs e, out bool hasError, out string errorMsg)
        {
            hasError = false;
            errorMsg = string.Empty;

            AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo? tipo = (AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo?)e.TipoFondo;

            if (tipo.HasValue)
            {
                switch (tipo.Value)
                {
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.EL:
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.TT:
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.ET:
                        UserControls.LiquidazionePensione.UCDatiAssicurativiEL_TT_ET tabDatiAssicurativiEL_TT_ET = (UserControls.LiquidazionePensione.UCDatiAssicurativiEL_TT_ET)sender;
                        hasError = tabDatiAssicurativiEL_TT_ET.HasError;
                        errorMsg = tabDatiAssicurativiEL_TT_ET.ErrorMessage;
                        break;
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.VL:
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.FS:
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PT:
                        UserControls.LiquidazionePensione.UCDatiAssicurativiVL_FS_PT tabDatiAssicurativiVL_FS_PT = (UserControls.LiquidazionePensione.UCDatiAssicurativiVL_FS_PT)sender;
                        hasError = tabDatiAssicurativiVL_FS_PT.HasError;
                        errorMsg = tabDatiAssicurativiVL_FS_PT.ErrorMessage;
                        break;
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PI:
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.GAS:
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.CL:
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PL:
                        UserControls.LiquidazionePensione.UCDatiAssicurativiPI_GAS_CL tabDatiAssicurativiPI_GAS_CL = (UserControls.LiquidazionePensione.UCDatiAssicurativiPI_GAS_CL)sender;
                        hasError = tabDatiAssicurativiPI_GAS_CL.HasError;
                        errorMsg = tabDatiAssicurativiPI_GAS_CL.ErrorMessage;
                        break;
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.DZ:
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.ES:
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PM:
                        UserControls.LiquidazionePensione.UCDatiAssicurativiDZ_ES_PM tabDatiAssicurativiDZ_ES_PM = (UserControls.LiquidazionePensione.UCDatiAssicurativiDZ_ES_PM)sender;
                        hasError = tabDatiAssicurativiDZ_ES_PM.HasError;
                        errorMsg = tabDatiAssicurativiDZ_ES_PM.ErrorMessage;
                        break;
                }
            }
            else
            {
                if (this.domanda.IsDomandaINPDAP)
                {
                    IViewUI interfaccia = (IViewUI)sender;
                    hasError = interfaccia.HasError;
                    errorMsg = interfaccia.ErrorMessage;
                }
            }
        }

        private void GetDataEventsTabGenerici(object sender, Utility.CustomEventArgs e, out bool hasError, out string errorMsg)
        {
            hasError = false;
            errorMsg = string.Empty;

            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo? tipoFondo = (AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo?)e.TipoFondo;
            if (tipoFondo.HasValue)
            {
                switch (tipoFondo.Value)
                {
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.EL:
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.TT:
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.ET:
                        UserControls.LiquidazionePensione.UCDatiGenericiEL_TT_ET tabDatiGenericiEL_TT_ET = (UserControls.LiquidazionePensione.UCDatiGenericiEL_TT_ET)sender;
                        hasError = tabDatiGenericiEL_TT_ET.HasError;
                        errorMsg = tabDatiGenericiEL_TT_ET.ErrorMessage;
                        break;
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.VL:
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.FS:
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PT:
                        UserControls.LiquidazionePensione.UCDatiGenericiVL_FS_PT tabDatiGenericiVL_FS_PT = (UserControls.LiquidazionePensione.UCDatiGenericiVL_FS_PT)sender;
                        hasError = tabDatiGenericiVL_FS_PT.HasError;
                        errorMsg = tabDatiGenericiVL_FS_PT.ErrorMessage;
                        break;
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PI:
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.GAS:
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.CL:
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PL:
                        UserControls.LiquidazionePensione.UCDatiGenericiPI_GAS_CL tabDatiGenericiPI_GAS_CL = (UserControls.LiquidazionePensione.UCDatiGenericiPI_GAS_CL)sender;
                        hasError = tabDatiGenericiPI_GAS_CL.HasError;
                        errorMsg = tabDatiGenericiPI_GAS_CL.ErrorMessage;
                        break;
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.DZ:
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.ES:
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PM:
                        UserControls.LiquidazionePensione.UCDatiGenericiDZ_ES_PM tabDatiGenericiDZ_ES_PM = (UserControls.LiquidazionePensione.UCDatiGenericiDZ_ES_PM)sender;
                        hasError = tabDatiGenericiDZ_ES_PM.HasError;
                        errorMsg = tabDatiGenericiDZ_ES_PM.ErrorMessage;
                        break;
                }
            }
            else
            {
                if (this.domanda.IsDomandaINPDAP)
                {
                    IViewUI interfaccia = (IViewUI)sender;
                    hasError = interfaccia.HasError;
                    errorMsg = interfaccia.ErrorMessage;
                }
            }
        }

        protected void event_ucShowAvvisoDatiAssicurativi(object sender, Utility.CustomEventArgs e)
        {
            bool hasError;
            string errorMsg;
            GetDataEventsTabAssicurativi(sender, e, out hasError, out errorMsg);

            if (hasError)
            {
                ucAvviso.Tipo = TipoAvviso.Warning;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = errorMsg;
            }
            else
            {
                ucAvviso.Tipo = TipoAvviso.Ok;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = "Dati Assicurativi salvati correttamente";
            }

            this.areaInfoPratica = new AreaInfoPratica();
            List<AreaQuadri.Tab> elencoTab = new List<AreaQuadri.Tab>();
            elencoTab.Add(AreaQuadri.Tab.LiquidazionePensione);
            elencoTab.Add(AreaQuadri.Tab.DatiNoCalcolo);
            elencoTab.Add(AreaQuadri.Tab.MaggiorazioniEBenefici);
            this.areaInfoPratica.ElencoTab = elencoTab.ToArray();

            CodeUtility.AggiornaSemafori(this, this, ucInfoLiquidazione);
        }

        protected void event_ucShowAvvisoEliminaDatiAssicurativi(object sender, Utility.CustomEventArgs e)
        {
            bool hasError;
            string errorMsg;
            GetDataEventsTabAssicurativi(sender, e, out hasError, out errorMsg);

            if (hasError)
            {
                ucAvviso.Tipo = TipoAvviso.Warning;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = errorMsg;
            }
            else
            {
                ucAvviso.Tipo = TipoAvviso.Ok;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = "Dati Assicurativi eliminati correttamente";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ValorizzazioneCodNatura", "SetCodiceNaturaDatiAssicurativi();", true);
            }

            this.areaInfoPratica = new AreaInfoPratica();
            List<AreaQuadri.Tab> elencoTab = new List<AreaQuadri.Tab>();
            elencoTab.Add(AreaQuadri.Tab.LiquidazionePensione);
            elencoTab.Add(AreaQuadri.Tab.DatiNoCalcolo);
            elencoTab.Add(AreaQuadri.Tab.MaggiorazioniEBenefici);
            this.areaInfoPratica.ElencoTab = elencoTab.ToArray();

            CodeUtility.AggiornaSemafori(this, this, ucInfoLiquidazione);
        }

        protected void event_ucShowAvvisoPrecedentePensione(object sender, EventArgs e)
        {
            INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.LiquidazionePensione.UCPrecedentePensione tabPrecedentePensione = (INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.LiquidazionePensione.UCPrecedentePensione)sender;

            this.areaInfoPratica = new AreaInfoPratica();
            List<AreaQuadri.Tab> elencoTab = new List<AreaQuadri.Tab>();
            elencoTab.Add(AreaQuadri.Tab.LiquidazionePensione);
            this.areaInfoPratica.ElencoTab = elencoTab.ToArray();

            CodeUtility.AggiornaSemafori(this, this, ucInfoLiquidazione);

            if (tabPrecedentePensione.HasError)
            {
                ucAvviso.Tipo = TipoAvviso.Warning;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = tabPrecedentePensione.ErrorMessage;
            }
            else
            {
                ucAvviso.Tipo = TipoAvviso.Ok;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = "Dati Precedente Pensione salvati correttamente";
            }
        }

        protected void event_ucShowAvvisoDatiGenerici(object sender, Utility.CustomEventArgs e)
        {
            bool hasError;
            string errorMsg;
            GetDataEventsTabGenerici(sender, e, out hasError, out errorMsg);

            if (hasError)
            {
                ucAvviso.Tipo = TipoAvviso.Warning;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = errorMsg;
            }
            else
            {
                ucAvviso.Tipo = TipoAvviso.Ok;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = "Dati Generici salvati correttamente";
            }

            this.areaInfoPratica = new AreaInfoPratica();
            List<AreaQuadri.Tab> elencoTab = new List<AreaQuadri.Tab>();
            elencoTab.Add(AreaQuadri.Tab.LiquidazionePensione);
            elencoTab.Add(AreaQuadri.Tab.Supplementi);
            elencoTab.Add(AreaQuadri.Tab.MaggiorazioniEBenefici);
            elencoTab.Add(AreaQuadri.Tab.DatiCalcolo);
            elencoTab.Add(AreaQuadri.Tab.Detrazioni);
            elencoTab.Add(AreaQuadri.Tab.Redditi);
            elencoTab.Add(AreaQuadri.Tab.Oneri);
            elencoTab.Add(AreaQuadri.Tab.RichiestaBonus);
            this.areaInfoPratica.ElencoTab = elencoTab.ToArray();

            CodeUtility.AggiornaSemafori(this, this, ucInfoLiquidazione);
        }

        protected void event_ucShowErrorDatiGenerici(object sender, Utility.CustomEventArgs e)
        {
            bool hasError;
            string errorMsg;
            GetDataEventsTabGenerici(sender, e, out hasError, out errorMsg);

            if (hasError)
            {
                ucAvviso.Tipo = TipoAvviso.Warning;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = errorMsg;
            }
        }

        protected void event_ucShowAvvisoEliminaDatiGenerici(object sender, Utility.CustomEventArgs e)
        {
            bool hasError;
            string errorMsg;
            GetDataEventsTabGenerici(sender, e, out hasError, out errorMsg);

            if (hasError)
            {
                ucAvviso.Tipo = TipoAvviso.Warning;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = errorMsg;
            }
            else
            {
                ucAvviso.Tipo = TipoAvviso.Ok;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = "Dati Generici eliminati correttamente";
            }

            this.areaInfoPratica = new AreaInfoPratica();
            List<AreaQuadri.Tab> elencoTab = new List<AreaQuadri.Tab>();
            elencoTab.Add(AreaQuadri.Tab.LiquidazionePensione);
            elencoTab.Add(AreaQuadri.Tab.Supplementi);
            elencoTab.Add(AreaQuadri.Tab.MaggiorazioniEBenefici);
            elencoTab.Add(AreaQuadri.Tab.DatiCalcolo);
            elencoTab.Add(AreaQuadri.Tab.Detrazioni);
            elencoTab.Add(AreaQuadri.Tab.RichiestaBonus);
            this.areaInfoPratica.ElencoTab = elencoTab.ToArray();

            CodeUtility.AggiornaSemafori(this, this, ucInfoLiquidazione);
        }

        protected void event_ucShowAvvisoEliminaPrecedentePensione(object sender, EventArgs e)
        {
            INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.LiquidazionePensione.UCPrecedentePensione tabDatiprecPensione = (INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.LiquidazionePensione.UCPrecedentePensione)sender;

            this.areaInfoPratica = new AreaInfoPratica();
            List<AreaQuadri.Tab> elencoTab = new List<AreaQuadri.Tab>();
            elencoTab.Add(AreaQuadri.Tab.LiquidazionePensione);
            this.areaInfoPratica.ElencoTab = elencoTab.ToArray();

            CodeUtility.AggiornaSemafori(this, this, ucInfoLiquidazione);

            if (tabDatiprecPensione.HasError)
            {
                ucAvviso.Tipo = TipoAvviso.Warning;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = tabDatiprecPensione.ErrorMessage;
            }
            else
            {
                ucAvviso.Tipo = TipoAvviso.Ok;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = "Dati Precedente Pensione eliminati correttamente";
            }

        }

        protected void event_ucShowAvvisoBititolaritaInail(object sender, EventArgs e)
        {
            INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.LiquidazionePensione.UCBititolaritaINAIL tabBititolaritaINAIL = (INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.LiquidazionePensione.UCBititolaritaINAIL)sender;

            this.areaInfoPratica = new AreaInfoPratica();
            List<AreaQuadri.Tab> elencoTab = new List<AreaQuadri.Tab>();
            elencoTab.Add(AreaQuadri.Tab.LiquidazionePensione);
            this.areaInfoPratica.ElencoTab = elencoTab.ToArray();

            CodeUtility.AggiornaSemafori(this, this, ucInfoLiquidazione);

            if (tabBititolaritaINAIL.HasError)
            {
                ucAvviso.Tipo = TipoAvviso.Warning;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = tabBititolaritaINAIL.ErrorMessage;
            }
            else
            {
                ucAvviso.Tipo = TipoAvviso.Ok;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = "Dati BititolaritaInail salvati correttamente";
            }
        }

        protected void event_ucShowAvvisoEliminaBititolaritaInail(object sender, EventArgs e)
        {
            INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.LiquidazionePensione.UCBititolaritaINAIL tabBititolaritaINAIL = (INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.LiquidazionePensione.UCBititolaritaINAIL)sender;

            this.areaInfoPratica = new AreaInfoPratica();
            List<AreaQuadri.Tab> elencoTab = new List<AreaQuadri.Tab>();
            elencoTab.Add(AreaQuadri.Tab.LiquidazionePensione);
            this.areaInfoPratica.ElencoTab = elencoTab.ToArray();

            CodeUtility.AggiornaSemafori(this, this, ucInfoLiquidazione);

            if (tabBititolaritaINAIL.HasError)
            {
                ucAvviso.Tipo = TipoAvviso.Warning;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = tabBititolaritaINAIL.ErrorMessage;
            }
            else
            {
                ucAvviso.Tipo = TipoAvviso.Ok;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = "Dati BititolaritaInail eliminati correttamente";
            }
        }

        protected void event_ucShowAvvisoLegge460(object sender, EventArgs e)
        {
            INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.LiquidazionePensione.UCLegge460 tabLegge460 = (INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.LiquidazionePensione.UCLegge460)sender;

            this.areaInfoPratica = new AreaInfoPratica();
            List<AreaQuadri.Tab> elencoTab = new List<AreaQuadri.Tab>();
            elencoTab.Add(AreaQuadri.Tab.LiquidazionePensione);
            this.areaInfoPratica.ElencoTab = elencoTab.ToArray();

            CodeUtility.AggiornaSemafori(this, this, ucInfoLiquidazione);

            if (tabLegge460.HasError)
            {
                ucAvviso.Tipo = TipoAvviso.Warning;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = tabLegge460.ErrorMessage;
            }
            else
            {
                ucAvviso.Tipo = TipoAvviso.Ok;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = "Dati Legge 4/60 salvati correttamente";
            }
        }

        protected void event_ucShowAvvisoEliminaLegge460(object sender, EventArgs e)
        {
            INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.LiquidazionePensione.UCLegge460 tabLegge460 = (INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.LiquidazionePensione.UCLegge460)sender;

            this.areaInfoPratica = new AreaInfoPratica();
            List<AreaQuadri.Tab> elencoTab = new List<AreaQuadri.Tab>();
            elencoTab.Add(AreaQuadri.Tab.LiquidazionePensione);
            this.areaInfoPratica.ElencoTab = elencoTab.ToArray();

            CodeUtility.AggiornaSemafori(this, this, ucInfoLiquidazione);

            if (tabLegge460.HasError)
            {
                ucAvviso.Tipo = TipoAvviso.Warning;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = tabLegge460.ErrorMessage;
            }
            else
            {
                ucAvviso.Tipo = TipoAvviso.Ok;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = "Dati Legge 4/60 eliminati correttamente";
            }
        }

        protected void event_ucAbilitaTastoSalva(object sender, EventArgs e)
        {
            if (btnSalvaLiquidazionePensione.Enabled == false)
                btnSalvaLiquidazionePensione.Enabled = true;
        }

        protected void event_ucShowAvviso(object sender, EventArgs e)
        {
            IViewUI interfaccia = (IViewUI)sender;

            this.areaInfoPratica = new AreaInfoPratica();
            List<AreaQuadri.Tab> elencoTab = new List<AreaQuadri.Tab>();
            elencoTab.Add(AreaQuadri.Tab.LiquidazionePensione);
            this.areaInfoPratica.ElencoTab = elencoTab.ToArray();

            CodeUtility.AggiornaSemafori(this, this, ucInfoLiquidazione);

            if (interfaccia.HasError)
                ucAvviso.Tipo = TipoAvviso.Warning;
            else
                ucAvviso.Tipo = TipoAvviso.Ok;

            ucAvviso.Visible = true;
            ucAvviso.Messaggio = interfaccia.ErrorMessage;
        }

        protected void event_ucHideAvviso(object sender, EventArgs e)
        {
            ucAvviso.Visible = false;
            ucAvviso.Messaggio = string.Empty;
        }

        protected void event_ucDisabilitaTastoSalva(object sender, EventArgs e)
        {
            if (btnSalvaLiquidazionePensione.Enabled == true)
                btnSalvaLiquidazionePensione.Enabled = false;
        }

        protected void ManageCodiceNoCalcoloPIU(object sender, EventArgs e)
        {
            List<RecordFondo> listaRecordFondo = ucDatiAssicurativiPI_GAS_CL.GetElencoRecordFondo();
            ucDatiGenericiPI_GAS_CL.ManageCodiceNoCalcoloPIU(listaRecordFondo);
        }

        protected void ManageExCombattente(object sender, Utility.EventMessageArgs args)
        {
            if (args.Message == "false")
            {
                //non visibilie ex comb
                ucDatiGenericiPI_GAS_CL.ManageVisibilityPnlCheckExCombattente(false);

            }
            else
            {
                ucDatiGenericiPI_GAS_CL.ManageVisibilityPnlCheckExCombattente(true);
            }

        }

        //ENG - Aggiornamento Memo86
        protected void event_ucShowAvvisoTrattenutaFondoCredito(object sender, EventArgs e)
        {
            ucAvviso.Tipo = TipoAvviso.Warning;
            ucAvviso.Visible = true;
            ucAvviso.Messaggio = "Attenzione, il campo 'Trattenuta fondo credito' e il campo 'Decorrenza trattenuta fondo credito' sono stati modificati. Verificare il conguaglio impostando opportunamente il campo 'Decorrenza arretrati'";
        }

        #endregion Events
    }

    public class extAreaRecordFondo : RecordFondo
    {
        public extAreaRecordFondo(RecordFondo area)
        {
            this._CodiceNatura3 = area._CodiceNatura3;
            this._CodiceNatura1 = area._CodiceNatura1;
            this._CodiceNatura2 = area._CodiceNatura2;
            this._CodiceNonCalcolo = area._CodiceNonCalcolo;
            this._DecorrenzaValiditaDati = area._DecorrenzaValiditaDati;
            this._DataSospensione = area._DataSospensione;
            this._IsFromDB = area._IsFromDB;
        }

        public String strDecorrenzaValidita
        {
            get
            {
                if (!this._DecorrenzaValiditaDati.HasValue || this._DecorrenzaValiditaDati.Value == DateTime.MinValue)
                    return "";
                return this._DecorrenzaValiditaDati.Value.ToString("MM/yyyy");
            }
        }

        public String strDataSospensione
        {
            get
            {
                if (!this._DataSospensione.HasValue)
                    return "";
                return String.Format("{0:MM/yyyy}", this._DataSospensione);
            }
        }

        public String strCodiceNoCalcolo
        {
            get
            {
                if (!this._CodiceNonCalcolo.HasValue || this._CodiceNonCalcolo == ' ')
                    return "";
                int tmp = this._CodiceNonCalcolo.Value == 'S' ? 1 : 0;
                return string.Format("{0} - {1}", tmp.ToString(), _CodiceNonCalcolo.ToString());
            }
        }

    }

    public class extAreaRecordFondoFS_PT : RecordFondo
    {
        public extAreaRecordFondoFS_PT(RecordFondo area)
        {
            this._CodiceNatura3 = area._CodiceNatura3;
            this._CodiceNatura1 = area._CodiceNatura1;
            this._CodiceNatura2 = area._CodiceNatura2;
            this._CodiceNonCalcolo = area._CodiceNonCalcolo;
            this._DecorrenzaValiditaDati = area._DecorrenzaValiditaDati;
            this._DataSospensione = area._DataSospensione;
        }

        public String strDecorrenzaValidita
        {
            get
            {
                if (!this._DecorrenzaValiditaDati.HasValue || this._DecorrenzaValiditaDati.Value == DateTime.MinValue)
                    return "";
                return this._DecorrenzaValiditaDati.Value.ToString("dd/MM/yyyy");
            }
        }

        public String strDataSospensione
        {
            get
            {
                if (!this._DataSospensione.HasValue)
                    return "";
                return String.Format("{0:dd/MM/yyyy}", this._DataSospensione);
            }
        }
    }
}
